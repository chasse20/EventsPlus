using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Inspector class for rendering <see cref="RawDelegate"/>s in the inspector</summary>
	public class DrawerRawDelegate<T> : PropertyDrawer where T : CacheRawDelegate
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Cached delegate drop-down data used for optimization</summary>
		protected Dictionary<string,T> cache = new Dictionary<string,T>();
		
		//=======================
		// Initialization
		//=======================
		/// <summary>Initializes the drawer and calculates the inspector height</summary>
		/// <param name="tProperty">Serialized delegate property</param>
		/// <param name="tLabel">GUI Label of the drawer</param>
		/// <returns>Height of the drawer</returns>
		public override float GetPropertyHeight( SerializedProperty tProperty, GUIContent tLabel )
		{
			// Initialize cache
			T tempCache;
			if ( !cache.TryGetValue( tProperty.propertyPath, out tempCache ) )
			{
				cache.Add( tProperty.propertyPath, createCache( tProperty ) );
			}

			return base.GetPropertyHeight( tProperty, tLabel );
		}
		
		/// <summary>Instantiates the delegate drop-down <see cref="cache"/></summary>
		/// <param name="tProperty">Serialized delegate property</param>
		/// <returns>Delegate cache</returns>
		protected virtual T createCache( SerializedProperty tProperty )
		{
			return Activator.CreateInstance( typeof( T ) ) as T;
		}
		
		//=======================
		// Render
		//=======================
		/// <summary>Renders the individual delegate property</summary>
		/// <param name="tPosition">Inspector position and size of <paramref name="tProperty"/></param>
		/// <param name="tProperty">Serialized delegate property</param>
		/// <param name="tLabel">GUI Label of the drawer</param>
		public override void OnGUI( Rect tPosition, SerializedProperty tProperty, GUIContent tLabel )
		{
			// Validate cache
			T tempCache = cache[ tProperty.propertyPath ];
			validate( tProperty, tempCache );
			
			// Target
			float tempFieldWidth = ( tPosition.width - EditorGUIUtility.labelWidth ) * 0.5f;
			tPosition.height = base.GetPropertyHeight( tProperty, tLabel );
			
			if ( tempCache.target == null ) // empty field
			{
				EditorGUI.BeginChangeCheck();
				UnityEngine.Object tempTarget = EditorGUI.ObjectField( tPosition, tLabel.text, null, typeof( UnityEngine.Object ), true );
				if ( EditorGUI.EndChangeCheck() )
				{
					tempCache.target = tempTarget;
					handleTargetUpdate( tProperty, tempCache );
				}
			}
			else // drop-down
			{
				tPosition.width = tempFieldWidth + EditorGUIUtility.labelWidth;
				
				EditorGUI.BeginChangeCheck();
				int tempSelectedTarget = EditorGUI.Popup( tPosition, tLabel.text, tempCache.selectedTarget, tempCache.targetNames );
				if ( EditorGUI.EndChangeCheck() )
				{
					tempCache.selectedTarget = tempSelectedTarget;
					handleTargetUpdate( tProperty, tempCache );
				}
			}
			
			// Members
			if ( tempCache.target != null )
			{
				float tempIndentSize = ( EditorGUI.indentLevel - 1 ) * EditorUtility.IndentSize;
				tPosition.x += tPosition.width - 13 - tempIndentSize;
				tPosition.width = tempFieldWidth + 13 + tempIndentSize;
				
				EditorGUI.BeginChangeCheck();
				int tempSelectedMember = EditorGUI.Popup( tPosition, tempCache.selectedMember, tempCache.memberNames );
				if ( EditorGUI.EndChangeCheck() )
				{
					tempCache.selectedMember = tempSelectedMember;
					handleMemberUpdate( tProperty, tempCache );
				}
			}
		}
		
		/// <summary>Validates the delegate property against the <paramref name="tCache"/></summary>
		/// <param name="tProperty">Serialized delegate property</param>
		/// <param name="tCache">Cached delegate drop-down data</param>
		protected virtual void validate( SerializedProperty tProperty, T tCache )
		{
			SerializedProperty tempMemberProperty = tProperty.FindPropertyRelative( "_member" );
			if ( !tCache.validateTarget( tProperty.FindPropertyRelative( "_target" ), tempMemberProperty ) )
			{
				handleTargetUpdate( tProperty, tCache );
			}
			else if ( !tCache.validateMember( tempMemberProperty ) )
			{
				handleMemberUpdate( tProperty, tCache );
			}
		}
		
		/// <summary>Applies the target property of the <see cref="RawDelegate"/></summary>
		/// <param name="tProperty">Serialized delegate property</param>
		/// <param name="tCache">Cached delegate drop-down data</param>
		protected virtual void handleTargetUpdate( SerializedProperty tProperty, T tCache )
		{
			tProperty.FindPropertyRelative( "_target" ).objectReferenceValue = tCache.target;
			handleMemberUpdate( tProperty, tCache );
		}
		
		/// <summary>Applies the member property of the <see cref="RawDelegate"/></summary>
		/// <param name="tProperty">Serialized delegate property</param>
		/// <param name="tCache">Cached delegate drop-down data</param>
		protected virtual void handleMemberUpdate( SerializedProperty tProperty, T tCache )
		{
			tProperty.FindPropertyRelative( "_member" ).stringValue = tCache.memberSerializedName;
			tProperty.serializedObject.ApplyModifiedProperties();
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Inspector class for rendering <see cref="RawDelegate"/>s in the inspector</summary>
	[CustomPropertyDrawer( typeof( RawDelegate ), true )]
	public class DrawerRawDelegate : DrawerRawDelegate<CacheRawDelegate>
	{
	}
}