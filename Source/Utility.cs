using System;
using System.Text;
using System.Reflection;

namespace EventsPlus
{
	/// <summary>Utility class for delegate serialization</summary>
	public static class Utility
	{
		//=======================
		// Serialization
		//=======================
		/// <summary>Serializes a <see cref="System.Reflection.MemberInfo"/> into a string</summary>
		/// <param name="tInfo">MemberInfo to serialize</param>
		/// <returns>String value of the serialized member info, null if invalid</returns>
		public static string Serialize( MemberInfo tInfo )
		{
			if ( tInfo != null )
			{
				if ( tInfo.MemberType == MemberTypes.Method )
				{
					StringBuilder tempBuilder = new StringBuilder( ( (int)tInfo.MemberType ).ToString() );
					tempBuilder.Append( ":" );
					tempBuilder.Append( tInfo.Name );

					ParameterInfo[] tempParameters = ( tInfo as MethodInfo ).GetParameters();
					int tempListLength = tempParameters.Length;
					for ( int i = 0; i < tempListLength; ++i )
					{
						tempBuilder.Append( ',' );
						tempBuilder.Append( tempParameters[i].ParameterType );
					}

					return tempBuilder.ToString();
				}

				return (int)tInfo.MemberType + ":" + tInfo.Name;
			}

			return null;
		}
		
		/// <summary>Deserializes a coded string into a <see cref="MemberInfo"/></summary>
		/// <param name="tType">Type of target the desired member belongs</param>
		/// <param name="tSerialized">Raw, serialized form of the member that was created by <see cref="Serialize"/></param>
		/// <param name="tFlags">Binding flags used for the MemberInfo lookup, defaults to all instance members</param>
		/// <returns>MemberInfo reference of the desired member, null if not found</returns>
		public static MemberInfo Deserialize( Type tType, string tSerialized, BindingFlags tFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public )
		{
			if ( !String.IsNullOrEmpty( tSerialized ) )
			{
				int tempNameIndex = tSerialized.IndexOf( ':' );
				MemberTypes tempMemberType = (MemberTypes)( Int32.Parse( tSerialized.Substring( 0, tempNameIndex ) ) );

				switch ( tempMemberType )
				{
					case MemberTypes.Method:
						string[] tempRawTypes = tSerialized.Split( ',' );
						int tempTypesLength = tempRawTypes.Length - 1;
						if ( tempTypesLength > 0 )
						{
							Type[] tempTypes = new Type[ tempTypesLength ];
							for ( int i = ( tempTypesLength - 1 ); i >= 0; --i )
							{
								tempTypes[i] = Type.GetType( tempRawTypes[ i + 1 ] );
							}

							return tType.GetMethod( tempRawTypes[ 0 ].Substring( tempNameIndex + 1 ), tFlags, null, tempTypes, null );
						}

						return tType.GetMethod( tempRawTypes[ 0 ].Substring( tempNameIndex + 1 ), tFlags, null, Type.EmptyTypes, null );
					default:
						MemberInfo[] tempMembers = tType.GetMember( tSerialized.Substring( tempNameIndex + 1 ), tempMemberType, tFlags );
						if ( tempMembers.Length > 0 )
						{
							return tempMembers[0];
						}
						break;
				}
			}

			return null;
		}
		
		/// <summary>Converts a type into its short-hand keyword</summary>
		/// <param name="tType">Type to convert</param>
		/// <returns>Keyword if successfully read, null if not</returns>
		public static string GetKeyword( this Type tType )
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
		
		//=======================
		// Delegates
		//=======================
		/// <summary>Gets default flags for all instance members both public and private</summary>
		public static BindingFlags InstanceFlags
		{
			get
			{
				return BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			}
		}
	}
}