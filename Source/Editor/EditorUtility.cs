using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EventsPlus
{	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Editor utility class for drawing custom fields and inspector display</summary>
	public static class EditorUtility
	{
		//=======================
		// Settings
		//=======================
		/// <summary>Selects the Events Plus Settings object from Resources/ folder, or will create one if it doesn't exist</summary>
		[MenuItem( "Events Plus/Settings", false )]
		public static void OpenSettings()
		{
			string tempPath = "Assets/EventsPlus/Resources/Settings.asset";
			Settings tempSettings = AssetDatabase.LoadMainAssetAtPath( tempPath ) as Settings;
			if ( tempSettings == null )
			{
				tempSettings = ScriptableObject.CreateInstance<Settings>();
				AssetDatabase.CreateAsset( tempSettings, AssetDatabase.GenerateUniqueAssetPath( tempPath ) );
				AssetDatabase.SaveAssets();
				UnityEditor.EditorUtility.FocusProjectWindow();
			}
			
			Selection.activeObject = tempSettings;
		}
		
		//=======================
		// Property Target
		//=======================
		/// <summary>Retrieves the actual object reference belonging to a SerializedProperty</summary>
		/// <param name="tProperty">SerializedProperty to seek through</param>
		/// <returns>Object reference that is used by the <paramref name="tProperty"/>, null if not found</returns>
		public static T GetNullablePropertyTarget<T>( SerializedProperty tProperty ) where T : class
		{
			if ( tProperty != null )
			{
				StringBuilder tempPath = new StringBuilder( tProperty.propertyPath );
				tempPath.Replace( "Array.data", "" );
				string[] tempPaths = tempPath.ToString().Split( '.' );
				object tempTarget = tProperty.serializedObject.targetObject;
				FieldInfo tempField = null;
				
				int tempListLength = tempPaths.Length;
				for ( int i = 0; i < tempListLength; ++i )
				{
					// Parse list element
					if ( tempPaths[i][0] == '[' && tempTarget is IList )
					{
						tempTarget = ( tempTarget as IList )[ Int32.Parse( tempPaths[i].Substring( 1, tempPaths[i].Length - 2 ) ) ];
					}
					// Parse object
					else
					{
						tempField = tempTarget.GetType().GetField( tempPaths[i] );
						tempTarget = tempField.GetValue( tempTarget );
					}
				}
				
				return tempTarget as T;
			}
			
			return null;
		}
		
		//=======================
		// Draw
		//=======================
		/// <summary>Draws a toggle field</summary>
		/// <param name="tRect">Inspector display rectangle</param>
		/// <param name="tToggle">SerializedProperty of the toggle input</param>
		/// <returns>True if <paramref name="tToggle"/> changed</returns>
		public static bool DrawToggle( Rect tRect, SerializedProperty tToggle )
		{
			if ( tToggle != null )
			{
				bool tempIsDynamic = EditorGUI.Toggle( tRect, tToggle.displayName, tToggle.boolValue );
				if ( tempIsDynamic != tToggle.boolValue )
				{
					tToggle.boolValue = tempIsDynamic;
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Draws a target field that behaves as a basic object field if null, or a component drop-down if not</summary>
		/// <param name="tRect">Inspector display rectangle</param>
		/// <param name="tTarget">SerializedProperty of the object input</param>
		/// <param name="tTargetNames">Cached <paramref name="tTarget"/> display names for the drop-down</param>
		/// <param name="tTargetObjects">Cached <paramref name="tTarget"/> references for the drop-down</param>
		/// <param name="tSelected">Selected index of the <paramref name="tTarget"/> determined by the drop-down</param>
		/// <returns>True if <paramref name="tTarget"/> changed</returns>
		public static bool DrawTargetField( Rect tRect, SerializedProperty tTarget, string[] tTargetNames, UnityEngine.Object[] tTargetObjects, ref int tSelected )
		{
			if ( tTarget != null && tTarget.propertyType == SerializedPropertyType.ObjectReference )
			{
				// Object field
				UnityEngine.Object tempSelected = null;
				if ( tTarget.objectReferenceValue == null )
				{
					tempSelected = EditorGUI.ObjectField( tRect, tTarget.displayName, tTarget.objectReferenceValue, typeof( UnityEngine.Object ), true );
				}
				// Dropdown
				else if ( tTargetNames != null && tTargetObjects != null )
				{
					if ( tTarget.objectReferenceValue != tTargetObjects[ tSelected ] ) // fix undo issue by recalculating the selected
					{
						for ( int i = ( tTargetObjects.Length - 1 ); i >= 0; --i )
						{
							if ( tTargetObjects[i] == tTarget.objectReferenceValue )
							{
								tSelected = i;
								return true;
							}
						}
					}
					
					tSelected = EditorGUI.Popup( tRect, tSelected, tTargetNames );
					tempSelected = tTargetObjects[ tSelected ];
				}
				// Null
				else
				{
					tSelected = -1;
				}
				
				// Apply
				if ( tempSelected != tTarget.objectReferenceValue )
				{
					tTarget.objectReferenceValue = tempSelected;
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Draws a method drop-down field</summary>
		/// <param name="tRect">Inspector display rectangle</param>
		/// <param name="tMethod">SerializedProperty of the method input</param>
		/// <param name="tMethodDisplayNames">Cached <paramref name="tMethod"/> display names for the drop-down</param>
		/// <param name="tSelected">Selected index of the <paramref name="tMethod"/> determined by the drop-down</param>
		/// <returns>True if <paramref name="tMethod"/> changed</returns>
		public static bool DrawMethodPopup( Rect tRect, SerializedProperty tMethod, string[] tMethodDisplayNames, ref int tSelected )
		{
			if ( tMethod != null && tMethod.propertyType == SerializedPropertyType.String && tMethodDisplayNames != null )
			{
				// Dropdown
				if ( tMethod.stringValue != tMethodDisplayNames[ tSelected ] ) // fix undo issue by recalculating the selected
				{
					for ( int i = ( tMethodDisplayNames.Length - 1 ); i >= 0; --i )
					{
						if ( tMethodDisplayNames[i] == tMethod.stringValue )
						{
							tSelected = i;
							return true;
						}
					}
				}
				
				tSelected = EditorGUI.Popup( tRect, tSelected, tMethodDisplayNames );
				
				// Apply
				if ( tMethodDisplayNames[ tSelected ] != tMethod.stringValue )
				{
					tMethod.stringValue = tMethodDisplayNames[ tSelected ];
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Draws a method drop-down field</summary>
		/// <param name="tRect">Inspector display rectangle</param>
		/// <param name="tMethod">SerializedProperty of the method input</param>
		/// <param name="tMethodReflectionNames">Cached <paramref name="tMethod"/> reflection data for the drop-down</param>
		/// <param name="tMethodDisplayNames">Cached <paramref name="tMethod"/> display names for the drop-down</param>
		/// <param name="tSelected">Selected index of the <paramref name="tMethod"/> determined by the drop-down</param>
		/// <returns>True if <paramref name="tMethod"/> changed</returns>
		public static bool DrawMethodPopup( Rect tRect, SerializedProperty tMethod, string[] tMethodReflectionNames, string[] tMethodDisplayNames, ref int tSelected )
		{
			if ( tMethod != null && tMethod.propertyType == SerializedPropertyType.String && tMethodReflectionNames != null && tMethodDisplayNames != null && tMethodReflectionNames.Length == tMethodDisplayNames.Length )
			{
				// Dropdown
				if ( tMethod.stringValue != tMethodReflectionNames[ tSelected ] ) // fix undo issue by recalculating the selected
				{
					for ( int i = ( tMethodReflectionNames.Length - 1 ); i >= 0; --i )
					{
						if ( tMethodReflectionNames[i] == tMethod.stringValue )
						{
							tSelected = i;
							return true;
						}
					}
				}
				
				tSelected = EditorGUI.Popup( tRect, tSelected, tMethodDisplayNames );
				
				// Apply
				if ( tMethodReflectionNames[ tSelected ] != tMethod.stringValue )
				{
					tMethod.stringValue = tMethodReflectionNames[ tSelected ];
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Returns an adjusted line-height for the inspector that includes proper vertical spacing</summary>
		public static float singleLineHeight
		{
			get
			{
				return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}
		}
		
		//=======================
		// Display Names
		//=======================
		/// <summary>Returns a more readable and formatted display name for a method/property/variable</summary>
		/// <param name="tReturnType">Return type for a method, or the actual type of property/variable</param>
		/// <param name="tName">Name of the method/property/variable</param>
		/// <param name="tParameters">Optional array of parameter types for a method</param>
		/// <param name="tIsProperty">Optional boolean to indicate if should be displayed as a property</param>
		/// <returns>Formatted display name of the method/property/variable</returns>
		public static string GetDisplayName( Type tReturnType, string tName, Type[] tParameters = null, bool tIsProperty = false )
		{
			// Property
			StringBuilder tempName = new StringBuilder();
			if ( tIsProperty )
			{
				tempName.Append( "set " );
			}
			
			// Return type
			if ( tReturnType != null )
			{
				tempName.Append( GetTypeKeyword( tReturnType ) );
				tempName.Append( " " );
			}
			
			// Name
			tempName.Append( tName );
			
			// Parameter types
			if ( tParameters != null )
			{
				tempName.Append( "(" );
				int tempListLength = tParameters.Length;
				for ( int i = 0; i < tempListLength; ++i )
				{
					tempName.Append( " " );
					tempName.Append( GetTypeKeyword( tParameters[i] ) );
					
					if ( i < ( tempListLength - 1 ) )
					{
						tempName.Append( "," );
					}
				}
				tempName.Append( " )" );
			}
			
			return tempName.ToString();
		}
		
		/// <summary>Converts a type into its short-hand keyword</summary>
		/// <param name="tType">Type to convert</param>
		/// <returns>Keyword if successfully read, null if not</returns>
		public static string GetTypeKeyword( Type tType )
		{
			if ( tType == typeof( void ) )
			{
				return "void";
			}
			else if ( tType == typeof( System.Delegate ) )
			{
				return "delegate";
			}
			else if ( tType == typeof( System.Enum ) )
			{
				return "enum";
			}
			else
			{
				switch ( Type.GetTypeCode( tType ) )
				{
					case TypeCode.Boolean:
						return "bool";
					case TypeCode.Byte:
						return "byte";
					case TypeCode.Char:
						return "char";
					case TypeCode.Decimal:
						return "decimal";
					case TypeCode.Double:
						return "double";
					case TypeCode.Int16:
						return "short";
					case TypeCode.Int32:
						return "int";
					case TypeCode.Int64:
						return "long";
					case TypeCode.Object:
						if ( tType == typeof( object ) )
						{
							return "object";
						}
						
						return tType.Name;
					case TypeCode.SByte:
						return "sbyte";
					case TypeCode.Single:
						return "float";
					case TypeCode.String:
						return "string";
					case TypeCode.UInt16:
						return "ushort";
					case TypeCode.UInt32:
						return "uint";
					case TypeCode.UInt64:
						return "ulong";
				}
			}
			
			return null;
		}
	}
}