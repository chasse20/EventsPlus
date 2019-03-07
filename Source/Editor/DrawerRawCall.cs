using UnityEngine;
using UnityEditor;
using System;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Inspector class for rendering <see cref="RawCall"/>s in the inspector</summary>
	[CustomPropertyDrawer( typeof( RawCall ), true )]
	public class DrawerRawCall : DrawerRawDelegate<CacheRawCall>
	{
		//=======================
		// Initialization
		//=======================
		public override float GetPropertyHeight( SerializedProperty tProperty, GUIContent tLabel )
		{
			float tempHeight = base.GetPropertyHeight( tProperty, tLabel );
			
			// Dynamic toggle height
			SerializedProperty tempDynamicProperty = tProperty.FindPropertyRelative( "_isDynamic" );
			CacheRawCall tempCache = cache[ tProperty.propertyPath ];
			if ( tempCache.isDynamicable )
			{
				tempHeight += EditorGUI.GetPropertyHeight( tempDynamicProperty ) + EditorGUIUtility.standardVerticalSpacing;
			}
			
			// Arguments height
			SerializedProperty tempArgumentsProperty = tProperty.FindPropertyRelative( "_arguments" );
			if ( tempArgumentsProperty.arraySize > 0 && !tempDynamicProperty.boolValue )
			{
				tempHeight += EditorGUI.GetPropertyHeight( tempArgumentsProperty, false ) + EditorGUIUtility.standardVerticalSpacing;
				if ( tempArgumentsProperty.isExpanded )
				{
					tempHeight += ( EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing ) * tempArgumentsProperty.arraySize;
				}
			}

			return tempHeight;
		}
		
		protected override CacheRawCall createCache( SerializedProperty tProperty )
		{
			Publisher tempPublisher = tProperty.GetPublisher();
			return new CacheRawCall( tempPublisher == null ? null : tempPublisher.types );
		}
		
		//=======================
		// Render
		//=======================
		public override void OnGUI( Rect tPosition, SerializedProperty tProperty, GUIContent tLabel )
		{
			// Inheritance
			base.OnGUI( tPosition, tProperty, tLabel );
			
			CacheRawCall tempCache = cache[ tProperty.propertyPath ];
			
			// Dynamic toggle
			EditorGUI.indentLevel += 1;
			
			tPosition.height = base.GetPropertyHeight( tProperty, tLabel );
			if ( tempCache.isDynamicable )
			{
				SerializedProperty tempDynamicProperty = tProperty.FindPropertyRelative( "_isDynamic" );
				
				tPosition.y += tPosition.height + EditorGUIUtility.standardVerticalSpacing;
				tPosition.height = EditorGUI.GetPropertyHeight( tempDynamicProperty );
				
				EditorGUI.BeginChangeCheck();
				bool tempIsDynamic = EditorGUI.Toggle( tPosition, tempDynamicProperty.displayName, tempCache.isDynamic );
				if ( EditorGUI.EndChangeCheck() )
				{
					tempCache.isDynamic = tempIsDynamic;
					handleDynamicUpdate( tProperty, tempCache );
				}
			}
			
			// Arguments
			if ( !tempCache.isDynamic )
			{
				SerializedProperty tempArgumentsProperty = tProperty.FindPropertyRelative( "_arguments" );
				if ( tempArgumentsProperty.arraySize > 0 )
				{
					tPosition.y += tPosition.height + EditorGUIUtility.standardVerticalSpacing;
					tPosition.height = EditorGUI.GetPropertyHeight( tempArgumentsProperty, false );
					
					tempArgumentsProperty.isExpanded = EditorGUI.Foldout( tPosition, tempArgumentsProperty.isExpanded, tempArgumentsProperty.displayName );
					if ( tempArgumentsProperty.isExpanded )
					{
						++EditorGUI.indentLevel;

						int tempListLength = tempArgumentsProperty.arraySize;
						for ( int i = 0; i < tempListLength; ++i )
						{
							DrawArgument( ref tPosition, tempArgumentsProperty.GetArrayElementAtIndex( i ), i, cache[ tProperty.propertyPath ] );
						}

						--EditorGUI.indentLevel;
					}
				}
			}
			
			EditorGUI.indentLevel -= 1;
		}
		
		protected override void validate( SerializedProperty tProperty, CacheRawCall tCache )
		{
			SerializedProperty tempMemberProperty = tProperty.FindPropertyRelative( "_member" );
			SerializedProperty tempDynamicProperty = tProperty.FindPropertyRelative( "_isDynamic" );
			if ( !tCache.validateTarget( tProperty.FindPropertyRelative( "_target" ), tempMemberProperty, tempDynamicProperty ) )
			{
				handleTargetUpdate( tProperty, tCache );
			}
			else if ( !tCache.validateMember( tempMemberProperty, tempDynamicProperty ) )
			{
				handleMemberUpdate( tProperty, tCache );
			}
		}
		
		/// <summary>Utility function for rendering a <see cref="RawArgument"/> property</summary>
		/// <param name="tPosition">Inspector position and size of <paramref name="tArgument"/></param>
		/// <param name="tArgument">Serialized argument property</param>
		/// <param name="tIndex">Index of the <paramref name="tArgument"/></param>
		/// <param name="tCache">Cached call drop-down data</param>
		protected static void DrawArgument( ref Rect tPosition, SerializedProperty tArgument, int tIndex, CacheRawCall tCache )
		{
			tPosition.y += tPosition.height + EditorGUIUtility.standardVerticalSpacing;
			tPosition.height = EditorGUI.GetPropertyHeight( tArgument );
			
			EditorGUI.PropertyField( tPosition, tArgument, new GUIContent( tCache.arguments[ tIndex ].name ) );
		}
		
		protected override void handleMemberUpdate( SerializedProperty tProperty, CacheRawCall tCache )
		{
			tProperty.FindPropertyRelative( "_member" ).stringValue = tCache.memberSerializedName;
			handleDynamicUpdate( tProperty, tCache );
		}
		
		/// <summary>Applies the dynamic toggle and arguments properties of the <see cref="RawCall"/></summary>
		/// <param name="tProperty">Serialized call property</param>
		/// <param name="tCache">Cached call drop-down data</param>
		protected virtual void handleDynamicUpdate( SerializedProperty tProperty, CacheRawCall tCache )
		{
			// Properties
			SerializedProperty tempDynamicProperty = tProperty.FindPropertyRelative( "_isDynamic" );
			SerializedProperty tempArgumentsProperty = tProperty.FindPropertyRelative( "_arguments" );
			
			// Dynamic, erase arguments
			if ( tCache.isDynamic )
			{
				tempDynamicProperty.boolValue = true;
				tempArgumentsProperty.ClearArray();
			}
			// Not dynamic, generate arguments if not void
			else
			{
				tempDynamicProperty.boolValue = false;
				Argument[] tempArguments = tCache.arguments;
				if ( tempArguments == null )
				{
					tempArgumentsProperty.ClearArray();
				}
				else
				{
					tempArgumentsProperty.arraySize = tempArguments.Length;
					for ( int i = ( tempArguments.Length - 1 ); i >= 0; --i )
					{
						tempArgumentsProperty.GetArrayElementAtIndex( i ).FindPropertyRelative( "type" ).stringValue = tempArguments[i].type.FullName;
					}
				}
			}
			
			tProperty.serializedObject.ApplyModifiedProperties();
		}
	}
}