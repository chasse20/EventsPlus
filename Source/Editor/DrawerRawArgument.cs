using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Inspector class for rendering <see cref="RawArgument"/>s in the inspector</summary>
	[CustomPropertyDrawer( typeof( RawArgument ), true )]
	public class DrawerRawArgument : PropertyDrawer
	{		
		//=======================
		// Render
		//=======================
		/// <summary>Calculates the inspector height</summary>
		/// <param name="tProperty">Serialized <see cref="RawArgument"/> property</param>
		/// <param name="tLabel">GUI Label of the drawer</param>
		/// <returns>Height of the drawer</returns>
		public override float GetPropertyHeight( SerializedProperty tProperty, GUIContent tLabel )
		{
			float tempHeight = base.GetPropertyHeight( tProperty, tLabel );
			
			SerializedProperty tempType = tProperty.FindPropertyRelative( "type" );
			if ( tempType.stringValue == "UnityEngine.Rect" )
			{
				tempHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}
			else if ( tempType.stringValue == "UnityEngine.Bounds" )
			{
				tempHeight += ( EditorGUIUtility.singleLineHeight * 2 ) + EditorGUIUtility.standardVerticalSpacing;
			}
			
			return tempHeight;
		}
		
		/// <summary>Renders the appropriate input field of the <see cref="RawArgument"/> property</summary>
		/// <param name="tPosition">Inspector position and size of <paramref name="tProperty"/></param>
		/// <param name="tProperty">Serialized <see cref="RawArgument"/> property</param>
		/// <param name="tLabel">GUI Label of the drawer</param>
		public override void OnGUI( UnityEngine.Rect tPosition, SerializedProperty tProperty, GUIContent tLabel )
		{
			SerializedProperty tempTypeProperty = tProperty.FindPropertyRelative( "type" );
			switch ( tempTypeProperty.stringValue )
			{
				case "System.String":
					SerializedProperty tempString = tProperty.FindPropertyRelative( "stringValue" );
					tempString.stringValue = EditorGUI.TextField( tPosition, tLabel, tempString.stringValue );
					break;
				case "System.Boolean":
					SerializedProperty tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempX1.floatValue = EditorGUI.Toggle( tPosition, tLabel, tempX1.floatValue > 0 ) ? 1 : -1;
					break;
				case "System.Int32":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempX1.floatValue = EditorGUI.IntField( tPosition, tLabel, (int)tempX1.floatValue );
					break;
				case "System.Int64":
					SerializedProperty tempLong = tProperty.FindPropertyRelative( "longValue" );
					tempLong.longValue = EditorGUI.LongField( tPosition, tLabel, tempLong.longValue );
					break;
				case "System.Single":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempX1.floatValue = EditorGUI.FloatField( tPosition, tLabel, tempX1.floatValue );
					break;
				case "System.Double":
					SerializedProperty tempDouble = tProperty.FindPropertyRelative( "doubleValue" );
					tempDouble.doubleValue = EditorGUI.DoubleField( tPosition, tLabel, tempDouble.doubleValue );
					break;
				case "UnityEngine.Vector2":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					SerializedProperty tempY1 = tProperty.FindPropertyRelative( "_y1" );
					
					EditorGUI.BeginChangeCheck();
					Vector2 tempVector2 = EditorGUI.Vector2Field( tPosition, tLabel, new Vector2( tempX1.floatValue, tempY1.floatValue ) );
					if ( EditorGUI.EndChangeCheck() )
					{
						tempX1.floatValue = tempVector2.x;
						tempY1.floatValue = tempVector2.y;
					}
					break;
				case "UnityEngine.Vector3":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempY1 = tProperty.FindPropertyRelative( "_y1" );
					SerializedProperty tempZ1 = tProperty.FindPropertyRelative( "_z1" );
					
					EditorGUI.BeginChangeCheck();
					Vector3 tempVector3 = EditorGUI.Vector3Field( tPosition, tLabel, new Vector3( tempX1.floatValue, tempY1.floatValue, tempZ1.floatValue ) );
					if ( EditorGUI.EndChangeCheck() )
					{
						tempX1.floatValue = tempVector3.x;
						tempY1.floatValue = tempVector3.y;
						tempZ1.floatValue = tempVector3.z;
					}
					break;
				case "UnityEngine.Vector4":
				case "UnityEngine.Quaternion":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempY1 = tProperty.FindPropertyRelative( "_y1" );
					tempZ1 = tProperty.FindPropertyRelative( "_z1" );
					SerializedProperty tempX2 = tProperty.FindPropertyRelative( "_x2" );
					
					EditorGUI.BeginChangeCheck();
					Vector4 tempVector4 = EditorGUI.Vector4Field( tPosition, tLabel, new Vector4( tempX1.floatValue, tempY1.floatValue, tempZ1.floatValue, tempX2.floatValue ) );
					if ( EditorGUI.EndChangeCheck() )
					{
						tempX1.floatValue = tempVector4.x;
						tempY1.floatValue = tempVector4.y;
						tempZ1.floatValue = tempVector4.z;
						tempX2.floatValue = tempVector4.w;
					}
					break;
				case "UnityEngine.Rect":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempY1 = tProperty.FindPropertyRelative( "_y1" );
					tempZ1 = tProperty.FindPropertyRelative( "_z1" );
					tempX2 = tProperty.FindPropertyRelative( "_x2" );
					
					EditorGUI.BeginChangeCheck();
					Rect tempRect = EditorGUI.RectField( tPosition, tLabel,  new Rect( tempX1.floatValue, tempY1.floatValue, tempZ1.floatValue, tempX2.floatValue ) );
					if ( EditorGUI.EndChangeCheck() )
					{
						tempX1.floatValue = tempRect.position.x;
						tempY1.floatValue = tempRect.position.y;
						tempZ1.floatValue = tempRect.size.x;
						tempX2.floatValue = tempRect.size.y;
					}
					break;
				case "UnityEngine.Bounds":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempY1 = tProperty.FindPropertyRelative( "_y1" );
					tempZ1 = tProperty.FindPropertyRelative( "_z1" );
					tempX2 = tProperty.FindPropertyRelative( "_x2" );
					SerializedProperty tempY2 = tProperty.FindPropertyRelative( "_y2" );
					SerializedProperty tempZ2 = tProperty.FindPropertyRelative( "_z2" );
					
					EditorGUI.BeginChangeCheck();
					Bounds tempBounds = EditorGUI.BoundsField( tPosition, tLabel, new Bounds( new Vector3( tempX1.floatValue, tempY1.floatValue, tempZ1.floatValue ), new Vector3( tempX2.floatValue, tempY2.floatValue, tempZ2.floatValue ) ) );
					if ( EditorGUI.EndChangeCheck() )
					{
						tempX1.floatValue = tempBounds.center.x;
						tempY1.floatValue = tempBounds.center.y;
						tempZ1.floatValue = tempBounds.center.z;
						tempX2.floatValue = tempBounds.size.x;
						tempY2.floatValue = tempBounds.size.y;
						tempZ2.floatValue = tempBounds.size.z;
					}
					break;
				case "UnityEngine.Color":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempY1 = tProperty.FindPropertyRelative( "_y1" );
					tempZ1 = tProperty.FindPropertyRelative( "_z1" );
					tempX2 = tProperty.FindPropertyRelative( "_x2" );
					
					EditorGUI.BeginChangeCheck();
					Color tempColor = EditorGUI.ColorField( tPosition, tLabel, new Color( tempX1.floatValue, tempY1.floatValue, tempZ1.floatValue, tempX2.floatValue ) );
					if ( EditorGUI.EndChangeCheck() )
					{
						tempX1.floatValue = tempColor.r;
						tempY1.floatValue = tempColor.g;
						tempZ1.floatValue = tempColor.b;
						tempX2.floatValue = tempColor.a;
					}
					break;
				case "UnityEngine.AnimationCurve":
					SerializedProperty tempCurve = tProperty.FindPropertyRelative( "animationCurveValue" );
					tempCurve.animationCurveValue = EditorGUI.CurveField( tPosition, tLabel, tempCurve.animationCurveValue );
					break;
				default:
					if ( String.IsNullOrEmpty( tempTypeProperty.stringValue ) )
					{
						// N/A
						EditorGUI.LabelField( tPosition, ( tProperty.displayName + " Not Drawable" ) );
					}
					else
					{
						// Convert string to type
						Type tempType = null;
						Assembly[] tempAssemblies = AppDomain.CurrentDomain.GetAssemblies();
						for ( int i = ( tempAssemblies.Length - 1 ); i >= 0 && tempType == null; --i )
						{
							tempType = tempAssemblies[i].GetType( tempTypeProperty.stringValue );
						}
						
						if ( tempType != null )
						{
							// Enumerator
							if ( tempType.IsEnum )
							{
								tempX1 = tProperty.FindPropertyRelative( "_x1" );
								
								if ( tempType.IsDefined( typeof( FlagsAttribute ), false ) )
								{
									tempX1.floatValue = Convert.ToSingle( EditorGUI.EnumFlagsField( tPosition, tLabel, (Enum)Enum.ToObject( tempType, (int)tempX1.floatValue ) ) );
								}
								else
								{
									tempX1.floatValue = Convert.ToSingle( EditorGUI.EnumPopup( tPosition, tLabel, (Enum)Enum.ToObject( tempType, (int)tempX1.floatValue ) ) );
								}
								
								return;
							}
							// Unity object
							else if ( tempType.IsClass && ( tempType == typeof( UnityEngine.Object ) || tempType.IsSubclassOf( typeof( UnityEngine.Object ) ) ) )
							{
								SerializedProperty tempObject = tProperty.FindPropertyRelative( "objectValue" );
								tempObject.objectReferenceValue = EditorGUI.ObjectField( tPosition, tLabel, tempObject.objectReferenceValue, tempType, true );
								
								return;
							}
						}
						
						// N/A
						EditorGUI.LabelField( tPosition, ( tempTypeProperty.stringValue + " Not Drawable" ) );
					}
					
					break;
			}
		}
	}
}