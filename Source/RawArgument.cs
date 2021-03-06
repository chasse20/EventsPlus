﻿using System;
using UnityEngine;

namespace EventsPlus
{	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Serializable argument data for any type supported by Unity's default inspector</summary>
	[Serializable]
	public class RawArgument
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Type of argument used by serialization</summary>
		public string type;
		/// <summary>Object reference</summary>
		public UnityEngine.Object objectValue;
		/// <summary>String</summary>
		public string stringValue;
		/// <summary>General numbers data</summary>
		[SerializeField]
		protected float _x1;
		/// <summary>General numbers data</summary>
		[SerializeField]
		protected float _x2;
		/// <summary>General numbers data</summary>
		[SerializeField]
		protected float _y1;
		/// <summary>General numbers data</summary>
		[SerializeField]
		protected float _y2;
		/// <summary>General numbers data</summary>
		[SerializeField]
		protected float _z1;
		/// <summary>General numbers data</summary>
		[SerializeField]
		protected float _z2;
		/// <summary>Long</summary>
		public long longValue;
		/// <summary>Double</summary>
		public double doubleValue;
		/// <summary>Animation curve</summary>
		public AnimationCurve animationCurveValue;
		
		//=======================
		// Accessors
		//=======================
		/// <summary>Gets the generic value of the argument based on the <see cref="type"/></summary>
		public virtual object genericValue
		{
			get
			{
				if ( !String.IsNullOrEmpty( type ) )
				{
					switch ( type )
					{
						case "System.String":
							return stringValue;
						case "System.Boolean":
							return boolValue;
						case "System.Int32":
							return intValue;
						case "System.Int64":
							return longValue;
						case "System.Single":
							return floatValue;
						case "System.Double":
							return doubleValue;
						case "UnityEngine.Vector2":
							return vector2Value;
						case "UnityEngine.Vector3":
							return vector3Value;
						case "UnityEngine.Vector4":
							return vector4Value;
						case "UnityEngine.Quaternion":
							return quaternionValue;
						case "UnityEngine.Rect":
							return rectValue;
						case "UnityEngine.Bounds":
							return boundsValue;
						case "UnityEngine.Color":
							return colorValue;
						case "UnityEngine.AnimationCurve":
							return animationCurveValue;
						default:
							Type tempType = Type.GetType( type );
							if ( tempType != null )
							{
								if ( tempType.IsEnum )
								{
									return enumValue;
								}
								else if ( tempType.IsClass && tempType.IsSubclassOf( typeof( UnityEngine.Object ) ) )
								{
									return objectValue;
								}
							}
							break;
					}
				}
				
				return null;
			}
		}
		
		/// <summary>Gets the bool value of the argument</summary>
		public bool boolValue
		{
			get
			{
				return _x1 > 0;
			}
			set
			{
				_x1 = value ? 1 : -1;
			}
		}
		
		/// <summary>Gets the integer value of the argument</summary>
		public int intValue
		{
			get
			{
				return (int)_x1;
			}
			set
			{
				_x1 = value;
			}
		}
		
		/// <summary>Gets the enumeration value of the argument</summary>
		public Enum enumValue
		{
			get
			{
				Type tempType = Type.GetType( type );
				if ( tempType != null && tempType.IsEnum )
				{
					return (Enum)Enum.ToObject( tempType, (int)_x1 );
				}
			
				return default(Enum);
			}
			set
			{
				_x1 = Convert.ToInt32( value );
			}
		}
		
		/// <summary>Gets the floating point value of the argument</summary>
		public float floatValue
		{
			get
			{
				return _x1;
			}
			set
			{
				_x1 = value;
			}
		}
		
		/// <summary>Gets the Vector2 value of the argument</summary>
		public Vector2 vector2Value
		{
			get
			{
				return new Vector2( _x1, _y1 );
			}
			set
			{
				_x1 = value.x;
				_y1 = value.y;
			}
		}
		
		/// <summary>Gets the Vector3 value of the argument</summary>
		public Vector3 vector3Value
		{
			get
			{
				return new Vector4( _x1, _y1, _z1 );
			}
			set
			{
				_x1 = value.x;
				_y1 = value.y;
				_z1 = value.z;
			}
		}
		
		/// <summary>Gets the Vector4 value of the argument</summary>
		public Vector4 vector4Value
		{
			get
			{
				return new Vector4( _x1, _y1, _z1, _x2 );
			}
			set
			{
				_x1 = value.x;
				_y1 = value.y;
				_z1 = value.z;
				_x2 = value.w;
			}
		}
		
		/// <summary>Gets the quaternion value of the argument</summary>
		public Quaternion quaternionValue
		{
			get
			{
				return new Quaternion( _x1, _y1, _z1, _x2 );
			}
			set
			{
				_x1 = value.x;
				_y1 = value.y;
				_z1 = value.z;
				_x2 = value.w;
			}
		}
		
		/// <summary>Gets the Rect value of the argument</summary>
		public Rect rectValue
		{
			get
			{
				return new Rect( new Vector2( _x1, _y1 ), new Vector2( _z1, _x2 ) );
			}
			set
			{
				_x1 = value.position.x;
				_y1 = value.position.y;
				_z1 = value.size.x;
				_x2 = value.size.y;
			}
		}
		
		/// <summary>Gets the Bounds value of the argument</summary>
		public Bounds boundsValue
		{
			get
			{
				return new Bounds( new Vector3( _x1, _y1, _z1 ), new Vector3( _x2, _y2, _z2 ) );
			}
			set
			{
				_x1 = value.center.x;
				_y1 = value.center.y;
				_z1 = value.center.z;
				_x2 = value.size.x;
				_y2 = value.size.y;
				_z2 = value.size.z;
			}
		}
		
		/// <summary>Gets the color value of the argument</summary>
		public Color colorValue
		{
			get
			{
				return new Color( _x1, _y1, _z1, _x2 );
			}
			set
			{
				_x1 = value.r;
				_y1 = value.g;
				_z1 = value.b;
				_x2 = value.a;
			}
		}
	}
}