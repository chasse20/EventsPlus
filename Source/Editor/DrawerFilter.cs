using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Inspector class for rendering <see cref="Filter"/>s in the inspector</summary>
	[CustomPropertyDrawer( typeof( Filter ), true )]
	public sealed class DrawerFilter : PropertyDrawer
	{
		//=======================
		// Variables
		//=======================
		/// <summary>True if initialized</summary>
		private bool isInitialized;
		/// <summary>True if the drawer is rendering an array</summary>
		private bool isArray;
		/// <summary>Cached <see cref="Namespace"/>s for the drop-down</summary>
		private List<Namespace> _namespaces;
		/// <summary>Display names for the namespaces drop-down</summary>
		private string[] _namespaceNames;
		/// <summary>Cached filter drop-down data used for optimization</summary>
		private Dictionary<string,CacheFilter> cache = new Dictionary<string,CacheFilter>();
		
		//=======================
		// Initialization
		//=======================
		/// <summary>Initializes the drawer and calculates the inspector height</summary>
		/// <param name="tProperty">Serialized <see cref="Filter"/> property</param>
		/// <param name="tLabel">GUI Label of the drawer</param>
		/// <returns>Height of the drawer</returns>
		public override float GetPropertyHeight( SerializedProperty tProperty, GUIContent tLabel )
		{
			// Initialize
			if ( !isInitialized )
			{
				isInitialized = true;
				isArray = fieldInfo.FieldType.IsArray || fieldInfo.FieldType == typeof( List<Filter> );
				
				// Generate namespaces and names
				_namespaces = EditorUtility.GetUnityObjectNamespaces();
				int tempListLength = _namespaces.Count;
				_namespaceNames = new string[ tempListLength ];
				for ( int i = ( tempListLength - 1 ); i >= 0; --i )
				{
					_namespaceNames[i] = _namespaces[i].name;
				}
			}
			
			// Initialize cache
			SerializedProperty tempNamespaceProperty = tProperty.FindPropertyRelative( "_typeNamespace" );
			SerializedProperty tempClassProperty = tProperty.FindPropertyRelative( "_typeClass" );
			
			CacheFilter tempCache;
			if ( !cache.TryGetValue( tProperty.propertyPath, out tempCache ) )
			{
				cache.Add( tProperty.propertyPath, new CacheFilter( _namespaces, _namespaceNames ) );
			}
			
			// Calculate height
			float tempHeight = base.GetPropertyHeight( tProperty, tLabel );
			if ( tProperty.isExpanded )
			{
				tempHeight += EditorGUI.GetPropertyHeight( tempNamespaceProperty ) + EditorGUIUtility.standardVerticalSpacing;
				tempHeight += EditorGUI.GetPropertyHeight( tempClassProperty ) + EditorGUIUtility.standardVerticalSpacing;
				tempHeight += EditorGUI.GetPropertyHeight( tProperty.FindPropertyRelative( "_whiteList" ) ) + EditorGUIUtility.standardVerticalSpacing;
				tempHeight += EditorGUI.GetPropertyHeight( tProperty.FindPropertyRelative( "_blackList" ) ) + EditorGUIUtility.standardVerticalSpacing;
			}
			
			return tempHeight;
		}
		
		//=======================
		// Render
		//=======================
		/// <summary>Renders the individual <see cref="Filter"/> property</summary>
		/// <param name="tPosition">Inspector position and size of <paramref name="tProperty"/></param>
		/// <param name="tProperty">Serialized <see cref="Filter"/> property</param>
		/// <param name="tLabel">GUI Label of the drawer</param>
		public override void OnGUI( Rect tPosition, SerializedProperty tProperty, GUIContent tLabel )
		{
			// Properties
			SerializedProperty tempNamespaceProperty = tProperty.FindPropertyRelative( "_typeNamespace" );
			SerializedProperty tempClassProperty = tProperty.FindPropertyRelative( "_typeClass" );
			SerializedProperty tempWhiteListProperty = tProperty.FindPropertyRelative( "_whiteList" );
			SerializedProperty tempBlackListProperty = tProperty.FindPropertyRelative( "_blackList" );
			
			// Validate cache
			CacheFilter tempCache = cache[ tProperty.propertyPath ];
			if ( !tempCache.validateNamespace( tempNamespaceProperty, tempClassProperty ) )
			{
				tempNamespaceProperty.stringValue = tempCache.namespaceName;
				tempClassProperty.stringValue = tempCache.className;
				tProperty.serializedObject.ApplyModifiedProperties();
			}
			else if ( !tempCache.validateClass( tempClassProperty ) )
			{
				tempClassProperty.stringValue = tempCache.className;
				tProperty.serializedObject.ApplyModifiedProperties();
			}
			
			// Draw
			tPosition.height = base.GetPropertyHeight( tProperty, tLabel );
			tProperty.isExpanded = EditorGUI.Foldout( tPosition, tProperty.isExpanded, isArray ? ( tempNamespaceProperty.stringValue + "." + tempClassProperty.stringValue ) : tProperty.displayName );
			if ( tProperty.isExpanded )
			{
				++EditorGUI.indentLevel;
				
				// Namespace
				tPosition.y += tPosition.height + EditorGUIUtility.standardVerticalSpacing;
				tPosition.height = EditorGUI.GetPropertyHeight( tempNamespaceProperty );
				
				EditorGUI.BeginChangeCheck();
				int tempSelectedNamespace = EditorGUI.Popup( tPosition, tempNamespaceProperty.displayName, tempCache.selectedNamespace, _namespaceNames );
				if ( EditorGUI.EndChangeCheck() )
				{
					tempCache.selectedNamespace = tempSelectedNamespace;
					
					tempNamespaceProperty.stringValue = tempCache.namespaceName;
					tempClassProperty.stringValue = tempCache.className;
					tProperty.serializedObject.ApplyModifiedProperties();
				}
				
				// Classes
				tPosition.y += tPosition.height + EditorGUIUtility.standardVerticalSpacing;
				tPosition.height = EditorGUI.GetPropertyHeight( tempClassProperty );
				
				EditorGUI.BeginChangeCheck();
				int tempSelectedClass = EditorGUI.Popup( tPosition, tempClassProperty.displayName, tempCache.selectedClass, tempCache.classNames );
				if ( EditorGUI.EndChangeCheck() )
				{
					tempCache.selectedClass = tempSelectedClass;
					
					tempClassProperty.stringValue = tempCache.className;
					tProperty.serializedObject.ApplyModifiedProperties();
				}
				
				// Lists
				DrawFilterList( ref tPosition, tempWhiteListProperty, tempCache );
				DrawFilterList( ref tPosition, tempBlackListProperty, tempCache );
				
				--EditorGUI.indentLevel;
			}
		}
		
		/// <summary>Utility function for rendering a <see cref="Filter"/> list</summary>
		/// <param name="tPosition">Inspector position and size of <paramref name="tList"/></param>
		/// <param name="tList">Serialized list property</param>
		/// <param name="tCache">Cached filter drop-down data</param>
		private static void DrawFilterList( ref Rect tPosition, SerializedProperty tList, CacheFilter tCache )
		{
			tPosition.y += tPosition.height + EditorGUIUtility.standardVerticalSpacing;
			tPosition.height = EditorGUI.GetPropertyHeight( tList, false );
			
			tList.isExpanded = EditorGUI.Foldout( tPosition, tList.isExpanded, tList.displayName );
			if ( tList.isExpanded )
			{
				++EditorGUI.indentLevel;

				// Size
				tPosition.y += tPosition.height + EditorGUIUtility.standardVerticalSpacing;
				tPosition.height = EditorGUIUtility.singleLineHeight;
				int tempSize = EditorGUI.IntField( tPosition, "Size", tList.arraySize );
				if ( tempSize < 0 )
				{
					tempSize = 0;
				}
				tList.arraySize = tempSize;

				// Draw elements
				int tempListLength = tList.arraySize;
				for ( int i = 0; i < tempListLength; ++i )
				{
					DrawFilterListElement( ref tPosition, tList.GetArrayElementAtIndex( i ), tCache );
				}

				--EditorGUI.indentLevel;
			}
		}
		
		/// <summary>Utility function for rendering a <see cref="Filter"/> list element</summary>
		/// <param name="tPosition">Inspector position and size of <paramref name="tElement"/></param>
		/// <param name="tElement">Serialized element property</param>
		/// <param name="tCache">Cached filter drop-down data</param>
		private static void DrawFilterListElement( ref Rect tPosition, SerializedProperty tElement, CacheFilter tCache )
		{
			// Determine member selection
			int tempSelectedMember = tCache.findMember( tElement.stringValue );
			if ( tempSelectedMember < 0 )
			{
				tempSelectedMember = 0;
			}
			
			// Dropdown
			tPosition.y += tPosition.height + EditorGUIUtility.standardVerticalSpacing;
			tPosition.height = EditorGUI.GetPropertyHeight( tElement );
			
			EditorGUI.BeginChangeCheck();
			tempSelectedMember = EditorGUI.Popup( tPosition, tElement.displayName, tempSelectedMember, tCache.memberNames );
			if ( EditorGUI.EndChangeCheck() || String.IsNullOrEmpty( tElement.stringValue ) )
			{
				tElement.stringValue = tCache.members[ tempSelectedMember ].serializedName;
				tElement.serializedObject.ApplyModifiedProperties();
			}
		}
	}
}