using System;
using System.Reflection;
using UnityEngine;
#if !UNITY_IOS
	using System.Reflection.Emit;
#endif

namespace EventsPlus
{	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Serialized form of a delegate</summary>
	[Serializable]
	public class RawDelegate
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Target owner of the delegate's member</summary>
		[SerializeField]
		protected UnityEngine.Object _target;
		/// <summary>Serialized name of the target member</summary>
		[SerializeField]
		protected string _member;
		/// <summary>Cached delegate instance generated upon initialization</summary>
		protected System.Delegate _delegateInstance;
		
		//=======================
		// Initialization
		//=======================
		/// <summary>Initializes and deserializes the <see cref="_target"/> and <see cref="_member"/> information into an actual delegate</summary>
		public virtual void initialize()
		{
			_delegateInstance = createDelegate( Utility.Deserialize( _target == null ? null : _target.GetType(), _member ) );
		}
		
		//=======================
		// Target
		//=======================
		/// <summary>Gets the <see cref="_target"/> object of the delegate</summary>
		public UnityEngine.Object target
		{
			get
			{
				return _target;
			}
		}
		
		/// <summary>Gets the serialized <see cref="_member"/> name of the delegate</summary>
		public string member
		{
			get
			{
				return _member;
			}
		}
		
		//=======================
		// Delegate
		//=======================
		/// <summary>Gets the deserialized <see cref="_delegateInstance"/></summary>
		public System.Delegate delegateInstance
		{
			get
			{
				return _delegateInstance;
			}
		}
		
		/// <summary>Creates a delegate instance using the <see cref="_target"/> and an input <see cref="System.Reflection.MemberInfo"/></summary>
		/// <param name="tMember">MemberInfo to be cast into a delegate</param>
		/// <returns>Member delegate if successful, null if not able to properly convert</returns>
		public virtual System.Delegate createDelegate( MemberInfo tMember )
		{
			if ( tMember != null )
			{
				switch ( tMember.MemberType )
				{
					case MemberTypes.Field:
						FieldInfo tempField = tMember as FieldInfo;
						return typeof( RawDelegate ).GetMethod( "CreateFieldAction", BindingFlags.Static | BindingFlags.NonPublic ).MakeGenericMethod( tempField.ReflectedType, tempField.FieldType ).Invoke( null, new object[] { _target, tempField } ) as System.Delegate;
					case MemberTypes.Property:
						PropertyInfo tempProperty = tMember as PropertyInfo;
						return Delegate.CreateDelegate( typeof( Action<> ).MakeGenericType( tempProperty.PropertyType ), _target, tempProperty.GetSetMethod(), false );
					case MemberTypes.Method:
						MethodInfo tempMethod = tMember as MethodInfo;
						bool tempIsAction = tempMethod.ReturnType == typeof( void );
						
						// Parameters
						ParameterInfo[] tempParameters = tempMethod.GetParameters();
						int tempParametersLength = tempParameters.Length;
						Type[] tempParameterTypes = null;
						if ( tempParametersLength > 0 )
						{
							tempParameterTypes = new Type[ tempParametersLength + ( tempIsAction ? 0 : 1 ) ];
							for ( int i = ( tempParametersLength - 1 ); i >= 0; --i )
							{
								tempParameterTypes[i] = tempParameters[i].ParameterType;
							}
						}
						else if ( !tempIsAction )
						{
							tempParameterTypes = new Type[1];
						}
						
						// Action
						if ( tempIsAction )
						{
							switch ( tempParametersLength )
							{
								case 0:
									return Delegate.CreateDelegate( typeof( Action ), _target, tempMethod, false );
								case 1:
									return Delegate.CreateDelegate( typeof( Action<> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
								case 2:
									return Delegate.CreateDelegate( typeof( Action<,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
								case 3:
									return Delegate.CreateDelegate( typeof( Action<,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
								case 4:
									return Delegate.CreateDelegate( typeof( Action<,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
								case 5:
									return Delegate.CreateDelegate( typeof( Action<,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
								case 6:
									return Delegate.CreateDelegate( typeof( Action<,,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
								case 7:
									return Delegate.CreateDelegate( typeof( Action<,,,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
								case 8:
									return Delegate.CreateDelegate( typeof( Action<,,,,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
								case 9:
									return Delegate.CreateDelegate( typeof( Action<,,,,,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
								case 10:
									return Delegate.CreateDelegate( typeof( Action<,,,,,,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
								default:
									break;
							}
							
							return null;
						}
						
						// Func
						tempParameterTypes[ tempParametersLength ] = tempMethod.ReturnType;
						
						switch ( tempParametersLength )
						{
							case 0:
								return Delegate.CreateDelegate( typeof( Func<> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
							case 1:
								return Delegate.CreateDelegate( typeof( Func<,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
							case 2:
								return Delegate.CreateDelegate( typeof( Func<,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
							case 3:
								return Delegate.CreateDelegate( typeof( Func<,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
							case 4:
								return Delegate.CreateDelegate( typeof( Func<,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
							case 5:
								return Delegate.CreateDelegate( typeof( Func<,,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
							case 6:
								return Delegate.CreateDelegate( typeof( Func<,,,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
							case 7:
								return Delegate.CreateDelegate( typeof( Func<,,,,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
							case 8:
								return Delegate.CreateDelegate( typeof( Func<,,,,,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
							case 9:
								return Delegate.CreateDelegate( typeof( Func<,,,,,,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
							case 10:
								return Delegate.CreateDelegate( typeof( Func<,,,,,,,,,,> ).MakeGenericType( tempParameterTypes ), _target, tempMethod, false );
							default:
								break;
						}
						break;
					default:
						break;
				}
			}

			return null;
		}
		
		//=======================
		// Field
		//=======================
		/// <summary>Utility function for creating a field delegate from a <see cref="System.Reflection.FieldInfo"/></summary>
		/// <param name="tTarget">Target owner of the <paramref name="tField"/></param>
		/// <param name="tField">FieldInfo used to generate a delegate</param>
		/// <returns>Generic 1-parameter action delegate if successful, null if not able to convert</returns>
		protected static Action<A> CreateFieldAction<T,A>( T tTarget, FieldInfo tField )
		{
			#if UNITY_IOS
				Action<A> tempAction = ( A tA ) =>
				{
					tField.SetValue( tTarget, tA );
				};
				
				return tempAction;
			#else
				Action<T,A> tempSetter = CreateFieldSetter<T,A>( tField );
				Action<A> tempAction = ( A tA ) =>
				{
					tempSetter( tTarget, tA );
				};
				
				return tempAction;
			#endif
		}
		
		#if !UNITY_IOS
			/// <summary>Utility function for creating a field delegate from a <see cref="System.Reflection.FieldInfo"/></summary>
			/// <param name="tField">FieldInfo used to generate a delegate</param>
			/// <returns>Generic 2-parameter action delegate if successful, null if not able to convert</returns>
			protected static Action<T,A> CreateFieldSetter<T,A>( FieldInfo tField )
			{
				DynamicMethod tempMethod = new DynamicMethod( ( tField.ReflectedType.FullName + ".set_" + tField.Name ), null, new Type[2] { tField.ReflectedType, tField.FieldType }, true );
			
				ILGenerator tempGenerator = tempMethod.GetILGenerator();
				tempGenerator.Emit( OpCodes.Ldarg_0 );
				tempGenerator.Emit( OpCodes.Ldarg_1 );
				tempGenerator.Emit( OpCodes.Stfld, tField );
				tempGenerator.Emit( OpCodes.Ret );
				
				return tempMethod.CreateDelegate( typeof( Action<T,A> ) ) as Action<T,A>;
			}
		#endif
	}
}