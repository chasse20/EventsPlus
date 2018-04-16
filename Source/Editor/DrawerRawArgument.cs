using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Editor drawer for displaying RawArguments in the inspector</summary>
	[CustomPropertyDrawer( typeof( RawArgument ), true )]
	public class DrawerRawArgument : PropertyDrawer
	{
		//=======================
		// Height
		//=======================
		/// <summary>Calculates the drawer height in the inspector</summary>
		/// <param name="tProperty">SerializedProperty of the RawArgument</param>
		/// <param name="tLabel">Inspector GUI Label for the <paramref name="tProperty"/></param>
		/// <returns>Pixel height of the drawer</returns>
		public override float GetPropertyHeight( SerializedProperty tProperty, GUIContent tLabel )
		{
			float tempHeight = EditorUtility.singleLineHeight;

			string tempType = tProperty.FindPropertyRelative( "type" ).stringValue;
			if ( tempType == "UnityEngine.Rect" )
			{
				tempHeight += EditorGUIUtility.singleLineHeight;
			}
			else if ( tempType == "UnityEngine.Bounds" )
			{
				tempHeight += EditorGUIUtility.singleLineHeight * 2;
			}
			
			return tempHeight;
		}
		
		//=======================
		// Draw
		//=======================
		/// <summary>Draws the appropriate field type of the RawArgument</summary>
		/// <param name="tRect">Inspector display rectangle</param>
		/// <param name="tProperty">SerializedProperty of the RawArgument</param>
		/// <param name="tLabel">Inspector GUI Label for the <paramref name="tProperty"/></param>
		public override void OnGUI( UnityEngine.Rect tRect, SerializedProperty tProperty, GUIContent tLabel )
		{
			string tempTypeName = tProperty.FindPropertyRelative( "type" ).stringValue;
			switch ( tempTypeName )
			{
				case "System.String":
					SerializedProperty tempString = tProperty.FindPropertyRelative( "stringValue" );
					tempString.stringValue = EditorGUI.TextField( tRect, tLabel, tempString.stringValue );
					break;
				case "System.Boolean":
					SerializedProperty tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempX1.floatValue = EditorGUI.Toggle( tRect, tLabel, tempX1.floatValue > 0 ) ? 1 : -1;
					break;
				case "System.Int32":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempX1.floatValue = EditorGUI.IntField( tRect, tLabel, (int)tempX1.floatValue );
					break;
				case "System.Int64":
					SerializedProperty tempLong = tProperty.FindPropertyRelative( "longValue" );
					tempLong.longValue = EditorGUI.LongField( tRect, tLabel, tempLong.longValue );
					break;
				case "System.Single":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempX1.floatValue = EditorGUI.FloatField( tRect, tLabel, tempX1.floatValue );
					break;
				case "System.Double":
					SerializedProperty tempDouble = tProperty.FindPropertyRelative( "doubleValue" );
					tempDouble.doubleValue = EditorGUI.DoubleField( tRect, tLabel, tempDouble.doubleValue );
					break;
				case "UnityEngine.Vector2":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					SerializedProperty tempY1 = tProperty.FindPropertyRelative( "_y1" );
					Vector2 tempVector2Old = new Vector2( tempX1.floatValue, tempY1.floatValue );
					Vector2 tempVector2New = EditorGUI.Vector2Field( tRect, tLabel, tempVector2Old );
					if ( tempVector2New != tempVector2Old )
					{
						tempX1.floatValue = tempVector2New.x;
						tempY1.floatValue = tempVector2New.y;
					}
					break;
				case "UnityEngine.Vector3":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempY1 = tProperty.FindPropertyRelative( "_y1" );
					SerializedProperty tempZ1 = tProperty.FindPropertyRelative( "_z1" );
					Vector3 tempVector3Old = new Vector3( tempX1.floatValue, tempY1.floatValue, tempZ1.floatValue );
					Vector3 tempVector3New = EditorGUI.Vector3Field( tRect, tLabel, tempVector3Old );
					if ( tempVector3New != tempVector3Old )
					{
						tempX1.floatValue = tempVector3New.x;
						tempY1.floatValue = tempVector3New.y;
						tempZ1.floatValue = tempVector3New.z;
					}
					break;
				case "UnityEngine.Vector4":
				case "UnityEngine.Quaternion":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempY1 = tProperty.FindPropertyRelative( "_y1" );
					tempZ1 = tProperty.FindPropertyRelative( "_z1" );
					SerializedProperty tempX2 = tProperty.FindPropertyRelative( "_x2" );
					Vector4 tempVector4Old = new Vector4( tempX1.floatValue, tempY1.floatValue, tempZ1.floatValue, tempX2.floatValue );
					Vector4 tempVector4New = EditorGUI.Vector4Field( tRect, tLabel, tempVector4Old );
					if ( tempVector4New != tempVector4Old )
					{
						tempX1.floatValue = tempVector4New.x;
						tempY1.floatValue = tempVector4New.y;
						tempZ1.floatValue = tempVector4New.z;
						tempX2.floatValue = tempVector4New.w;
					}
					break;
				case "UnityEngine.Rect":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempY1 = tProperty.FindPropertyRelative( "_y1" );
					tempZ1 = tProperty.FindPropertyRelative( "_z1" );
					tempX2 = tProperty.FindPropertyRelative( "_x2" );
					Rect tempRectOld = new Rect( tempX1.floatValue, tempY1.floatValue, tempZ1.floatValue, tempX2.floatValue );
					Rect tempRectNew = EditorGUI.RectField( tRect, tLabel, tempRectOld );
					if ( tempRectNew != tempRectOld )
					{
						tempX1.floatValue = tempRectNew.position.x;
						tempY1.floatValue = tempRectNew.position.y;
						tempZ1.floatValue = tempRectNew.size.x;
						tempX2.floatValue = tempRectNew.size.y;
					}
					break;
				case "UnityEngine.Bounds":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempY1 = tProperty.FindPropertyRelative( "_y1" );
					tempZ1 = tProperty.FindPropertyRelative( "_z1" );
					tempX2 = tProperty.FindPropertyRelative( "_x2" );
					SerializedProperty tempY2 = tProperty.FindPropertyRelative( "_y2" );
					SerializedProperty tempZ2 = tProperty.FindPropertyRelative( "_z2" );
					Bounds tempBoundsOld = new Bounds( new Vector3( tempX1.floatValue, tempY1.floatValue, tempZ1.floatValue ), new Vector3( tempX2.floatValue, tempY2.floatValue, tempZ2.floatValue ) );
					Bounds tempBoundsNew = EditorGUI.BoundsField( tRect, tLabel, tempBoundsOld );
					if ( tempBoundsNew != tempBoundsOld )
					{
						tempX1.floatValue = tempBoundsNew.center.x;
						tempY1.floatValue = tempBoundsNew.center.y;
						tempZ1.floatValue = tempBoundsNew.center.z;
						tempX2.floatValue = tempBoundsNew.size.x;
						tempY2.floatValue = tempBoundsNew.size.y;
						tempZ2.floatValue = tempBoundsNew.size.z;
					}
					break;
				case "UnityEngine.Color":
					tempX1 = tProperty.FindPropertyRelative( "_x1" );
					tempY1 = tProperty.FindPropertyRelative( "_y1" );
					tempZ1 = tProperty.FindPropertyRelative( "_z1" );
					tempX2 = tProperty.FindPropertyRelative( "_x2" );
					Color tempColorOld = new Color( tempX1.floatValue, tempY1.floatValue, tempZ1.floatValue, tempX2.floatValue );
					Color tempColorNew = EditorGUI.ColorField( tRect, tLabel, tempColorOld );
					if ( tempColorNew != tempColorOld )
					{
						tempX1.floatValue = tempColorNew.r;
						tempY1.floatValue = tempColorNew.g;
						tempZ1.floatValue = tempColorNew.b;
						tempX2.floatValue = tempColorNew.a;
					}
					break;
				case "UnityEngine.AnimationCurve":
					SerializedProperty tempCurve = tProperty.FindPropertyRelative( "animationCurveValue" );
					tempCurve.animationCurveValue = EditorGUI.CurveField( tRect, tLabel, tempCurve.animationCurveValue );
					break;
				default:
					// Convert string to type
					Type tempType = null;
					Assembly[] tempAssemblies = AppDomain.CurrentDomain.GetAssemblies();
					for ( int i = ( tempAssemblies.Length - 1 ); i >= 0 && tempType == null; --i )
					{
						tempType = tempAssemblies[i].GetType( tempTypeName );
					}
					
					if ( tempType != null )
					{
						// Enumerator
						if ( tempType.IsEnum )
						{
							tempX1 = tProperty.FindPropertyRelative( "_x1" );
							
							if ( tempType.IsDefined( typeof( FlagsAttribute ), false ) )
							{
								tempX1.floatValue = Convert.ToSingle( EditorGUI.EnumFlagsField( tRect, tLabel, (Enum)Enum.ToObject( tempType, (int)tempX1.floatValue ) ) );
							}
							else
							{
								tempX1.floatValue = Convert.ToSingle( EditorGUI.EnumPopup( tRect, tLabel, (Enum)Enum.ToObject( tempType, (int)tempX1.floatValue ) ) );
							}
							
							return;
						}
						// Unity object
						else if ( tempType.IsClass && ( tempType == typeof( System.Object ) || tempType.IsSubclassOf( typeof( System.Object ) ) ) )
						{
							SerializedProperty tempObject = tProperty.FindPropertyRelative( "objectValue" );
							tempObject.objectReferenceValue = EditorGUI.ObjectField( tRect, tLabel, tempObject.objectReferenceValue, tempType, true );
							
							return;
						}
					}
					
					// N/A
					EditorGUI.LabelField( tRect, ( tempTypeName + " Not Drawable" ) );
					break;
			}
		}
	}
}