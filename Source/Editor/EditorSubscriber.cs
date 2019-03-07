using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Inspector class for rendering the <see cref="Subscriber"/> in the inspector</summary>
	[CustomEditor( typeof( Subscriber ) )]
	public class EditorSubscriber : Editor 
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Reorderable list of <see cree="RawRequest"/>s</summary>
		protected ReorderableList requests;

		//=======================
		// Initialization
		//=======================
		/// <summary>Initializes the <see cref="requests"/></summary>
		protected virtual void OnEnable()
		{
			requests = new ReorderableList( serializedObject, serializedObject.FindProperty( "_requests" ), true, false, true, true );
			requests.footerHeight = EditorGUIUtility.singleLineHeight;
			
			requests.drawElementCallback += ( Rect tPosition, int tIndex, bool tIsActive, bool tIsFocused ) =>
			{
				EditorGUI.PropertyField( tPosition, requests.serializedProperty.GetArrayElementAtIndex( tIndex ), true );
			};
			
			requests.elementHeightCallback += ( int tIndex ) =>
			{
				return EditorGUI.GetPropertyHeight( requests.serializedProperty.GetArrayElementAtIndex( tIndex ) ) + EditorGUIUtility.standardVerticalSpacing;
			};
		}
		
		//=======================
		// Render
		//=======================
		/// <summary>Draws the inspector including the <see cref="requests"/> list</summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			// Draw script field
			EditorGUI.BeginDisabledGroup( true );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_Script" ), true );
			EditorGUI.EndDisabledGroup();
			
			// Draw request list
			requests.serializedProperty.isExpanded = EditorGUILayout.Foldout( requests.serializedProperty.isExpanded, requests.serializedProperty.displayName );
			if ( requests.serializedProperty.isExpanded )
			{
				requests.DoLayoutList();
			}
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}