using UnityEngine;
using UnityEditor;
using System;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Inspector class for rendering <see cref="RawRequest"/>s in the inspector</summary>
	[CustomPropertyDrawer( typeof( RawRequest ), true )]
	public class DrawerRawRequest : DrawerRawDelegate
	{
		//=======================
		// Initialization
		//=======================
		public override float GetPropertyHeight( SerializedProperty tProperty, GUIContent tLabel )
		{
			float tempHeight = base.GetPropertyHeight( tProperty, tLabel );
			
			// Tags height
			SerializedProperty tempTagsProperty = tProperty.FindPropertyRelative( "_tags" );
			tempHeight += EditorGUI.GetPropertyHeight( tempTagsProperty, false ) + EditorGUIUtility.standardVerticalSpacing;
			if ( tempTagsProperty.isExpanded )
			{
				tempHeight += ( EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing ) * ( tempTagsProperty.arraySize + 1 );
			}
			
			return tempHeight;
		}
		
		//=======================
		// Render
		//=======================
		public override void OnGUI( Rect tPosition, SerializedProperty tProperty, GUIContent tLabel )
		{
			// Inheritance
			base.OnGUI( tPosition, tProperty, tLabel );
			
			// Draw tags
			EditorGUI.indentLevel += 1;
			
			SerializedProperty tempTagsProperty = tProperty.FindPropertyRelative( "_tags" );
			
			tPosition.y += base.GetPropertyHeight( tProperty, tLabel ) + EditorGUIUtility.standardVerticalSpacing;
			tPosition.height = EditorGUI.GetPropertyHeight( tempTagsProperty, false );
			
			tempTagsProperty.isExpanded = EditorGUI.Foldout( tPosition, tempTagsProperty.isExpanded, tempTagsProperty.displayName );
			if ( tempTagsProperty.isExpanded )
			{
				++EditorGUI.indentLevel;

				// Size
				tPosition.y += tPosition.height + EditorGUIUtility.standardVerticalSpacing;
				tPosition.height = EditorGUIUtility.singleLineHeight;
				int tempSize = EditorGUI.IntField( tPosition, "Size", tempTagsProperty.arraySize );
				if ( tempSize < 1 )
				{
					tempSize = 1; // must have at least 1 tag
				}
				tempTagsProperty.arraySize = tempSize;
				
				// Draw elements
				int tempListLength = tempTagsProperty.arraySize;
				for ( int i = 0; i < tempListLength; ++i )
				{
					DrawTag( ref tPosition, tempTagsProperty.GetArrayElementAtIndex( i ) );
				}

				--EditorGUI.indentLevel;
			}
			
			EditorGUI.indentLevel -= 1;
		}
		
		/// <summary>Utility function for rendering a tag element</summary>
		/// <param name="tPosition">Inspector position and size of <paramref name="tTag"/></param>
		/// <param name="tTag">Serialized tag property</param>
		protected static void DrawTag( ref Rect tPosition, SerializedProperty tTag )
		{
			tPosition.y += tPosition.height + EditorGUIUtility.standardVerticalSpacing;
			tPosition.height = EditorGUI.GetPropertyHeight( tTag );
			
			EditorGUI.PropertyField( tPosition, tTag );
		}
	}
}