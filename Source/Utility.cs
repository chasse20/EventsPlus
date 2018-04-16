using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;

#if !UNITY_IOS
	using System.Reflection.Emit;
#endif

namespace EventsPlus
{	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Utility class for reflection parsing and delegate creation</summary>
	public static class Utility
	{
		//=======================
		// Format
		//=======================
		/// <summary>Reads serialized delegate data and decomposes it out into the necessary parts for creating a delegate</summary>
		/// <param name="tData">Raw delegate data</param>
		/// <param name="tType">Type of delegate to make</param>
		/// <param name="tName">Name of the method, property or variable that a delegate should target</param>
		/// <param name="tParameterTypes">Type array of the delegate parameters</param>
		/// <returns>True if <paramref name="tData"/> is successfully decomposed</returns>
		public static bool ReadReflectionData( string tData, out InfoType tType, out string tName, out Type[] tParameterTypes )
		{
			if ( !String.IsNullOrEmpty( tData ) )
			{
				// Type determined by the first character
				switch ( tData[0] )
				{
					case 'f':
						tType = InfoType.Field;
						break;
					case 'p':
						tType = InfoType.Property;
						break;
					default:
						tType = InfoType.Method;
						break;
				}
				
				// Name
				string[] tempDecomposed = tData.Split( ',' );
				tName = tempDecomposed[0].Substring( 1, tempDecomposed[0].Length - 1 );
				
				// Parameter types
				if ( tempDecomposed.Length > 1 )
				{
					Type tempType;
					Assembly[] tempAssemblies = AppDomain.CurrentDomain.GetAssemblies();
					tParameterTypes = new Type[ tempDecomposed.Length - 1 ];
					for ( int i = ( tParameterTypes.Length - 1 ); i >= 0; --i )
					{
						tempType = null;
						for ( int j = ( tempAssemblies.Length - 1 ); j >= 0 && tempType == null; --j )
						{
							tempType = tempAssemblies[j].GetType( tempDecomposed[ i + 1 ] );
						}
						
						tParameterTypes[i] = tempType;
					}
				}
				else
				{
					tParameterTypes = new Type[0];
				}
				
				return true;
			}
			
			tType = InfoType.Method;
			tName = null;
			tParameterTypes = new Type[0];
			return false;
		}
		
		/// <summary>Converts delegate data into a compressed, serializable format</summary>
		/// <param name="tType">Type of delegate</param>
		/// <param name="tName">Name of the method, property or variable that the delegate targets</param>
		/// <param name="tParameterTypes">Type array of the delegate parameters</param>
		/// <returns>String representing the compressed format of the delegate data</returns>
		public static string WriteReflectionData( InfoType tType, string tName, Type[] tParameterTypes = null )
		{
			if ( !String.IsNullOrEmpty( tName ) )
			{
				// Type determined by the first character
				StringBuilder tempData;
				switch ( tType )
				{
					case InfoType.Field:
						tempData = new StringBuilder( "f" );
						break;
					case InfoType.Property:
						tempData = new StringBuilder( "p" );
						break;
					default:
						tempData = new StringBuilder( "m" );
						break;
				}
				
				// Name
				tempData.Append( tName );
				
				// Parameter types
				if ( tParameterTypes != null )
				{
					int tempListLength = tParameterTypes.Length;
					for ( int i = 0; i < tempListLength; ++i )
					{
						tempData.Append( ',' );
						tempData.Append( tParameterTypes[i].ToString() );
					}
				}
				
				return tempData.ToString();
			}
			
			return null;
		}
		
		//=======================
		// Reflection
		//=======================
		/// <summary>Tries to retrieve the value from an instance variable via reflection</summary>
		/// <param name="tTarget">Owning object of the variable called <paramref name="tVariable"/></param>
		/// <param name="tVariable">The variable name that is assumed to be contained in <paramref name="tTarget"/></param>
		/// <param name="tFlags">Binding flags for reflection</param>
		/// <returns>The value of the value-type variable if found, defaulted if not</returns>
		public static T GetVariable<T>( object tTarget, string tVariable, BindingFlags tFlags ) where T : struct
		{
			if ( tTarget != null && !String.IsNullOrEmpty( tVariable ) )
			{
				FieldInfo tempField = tTarget.GetType().GetField( tVariable, ( tFlags | BindingFlags.Instance ) );
				if ( tempField != null )
				{
					return (T)tempField.GetValue( tTarget );
				}
			}
		
			return default(T);
		}
		
		/// <summary>Tries to retrieve the value from a nullable instance variable via reflection</summary>
		/// <param name="tTarget">Owning object of the variable called <paramref name="tVariable"/></param>
		/// <param name="tVariable">The variable name that is assumed to be contained in <paramref name="tTarget"/></param>
		/// <param name="tFlags">Binding flags for reflection</param>
		/// <returns>The value of the reference variable if found, null if not</returns>
		public static T GetNullableVariable<T>( object tTarget, string tVariable, BindingFlags tFlags ) where T : class
		{
			if ( tTarget != null && !String.IsNullOrEmpty( tVariable ) )
			{
				FieldInfo tempField = tTarget.GetType().GetField( tVariable, ( tFlags | BindingFlags.Instance ) );
				if ( tempField != null )
				{
					return tempField.GetValue( tTarget ) as T;
				}
			}
		
			return null;
		}
		
		//=======================
		// Delegate
		//=======================
		/// <summary>Creates a delegate for a variable</summary>
		/// <param name="tTarget">Owner instance of the variable</param>
		/// <param name="tFieldInfo">FieldInfo of the target variable</param>
		/// <returns>Delegate pointing to the variable if successful, null if not</returns>
		public static System.Delegate CreateDelegate( object tTarget, FieldInfo tFieldInfo )
		{
			if ( tTarget != null && tFieldInfo != null )
			{
				return typeof( Utility ).GetMethod( "CreateFieldSingleAction" ).MakeGenericMethod( new Type[] { tFieldInfo.ReflectedType, tFieldInfo.FieldType } ).Invoke( null, new object[] { tTarget, tFieldInfo } ) as System.Delegate;
			}
			
			return null;
		}
		
		/// <summary>Creates a delegate for a property</summary>
		/// <param name="tTarget">Owner instance of the property</param>
		/// <param name="tPropertyInfo">PropertyInfo of the target property</param>
		/// <returns>Delegate pointing to the property if successful, null if not</returns>
		public static System.Delegate CreateDelegate( object tTarget, PropertyInfo tPropertyInfo )
		{
			if ( tTarget != null && tPropertyInfo != null )
			{
				return Delegate.CreateDelegate( typeof( Action<> ).MakeGenericType( new Type[] { tPropertyInfo.PropertyType } ), tTarget, tPropertyInfo.GetSetMethod(), false ) as System.Delegate;
			}
		
			return null;
		}
		
		/// <summary>Factory helper that will try to create a delegate for a method</summary>
		/// <param name="tTarget">Owner instance of the method</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tParameterTypes">Parameter types of the method's definition</param>
		/// <param name="tFactoryAction">Factory function to use for creating a delegate from the method if it doesn't have a return type</param>
		/// <param name="tFactoryFunc">Factory function to use for creating a delegate from the method if it does have a return type</param>
		/// <returns>Delegate pointing to the method if successful, null if not</returns>
		public static System.Delegate CreateDelegate( object tTarget, MethodInfo tMethodInfo, Type[] tParameterTypes, Func<object,MethodInfo,Type[],System.Delegate> tFactoryAction, Func<object,MethodInfo,Type[],System.Delegate> tFactoryFunc )
		{
			if ( tTarget != null && tMethodInfo != null )
			{
				// Action
				if ( tMethodInfo.ReturnType == typeof( void ) )
				{
					Func<object,MethodInfo,Type[],System.Delegate> tempFactory = tFactoryAction;
					if ( tempFactory != null )
					{
						return tempFactory( tTarget, tMethodInfo, tParameterTypes );
					}
				}
				// Func (assumes last parameter is return type)
				else
				{
					Func<object,MethodInfo,Type[],System.Delegate> tempFactory = tFactoryFunc;
					if ( tempFactory != null )
					{
						int tempListLength = tParameterTypes == null ? 1 : 1 + tParameterTypes.Length;
						Type[] tempFuncTypes = new Type[ tempListLength ];
						tempFuncTypes[ tempListLength - 1 ] = tMethodInfo.ReturnType;
						tParameterTypes.CopyTo( tempFuncTypes, 0 );
						
						return tempFactory( tTarget, tMethodInfo, tempFuncTypes );
					}
				}
			}
		
			return null;
		}
		
		/// <summary>Factory function that tries to create a delegate for a method without a return type</summary>
		/// <param name="tTarget">Owner instance of the method</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tParameterTypes">Parameter types of the method's definition</param>
		/// <returns>Delegate pointing to the method if successful, null if not</returns>
		public static System.Delegate CreateActionDelegate( object tTarget, MethodInfo tMethodInfo, Type[] tParameterTypes )
		{
			int tempListLength = tParameterTypes == null ? 0 : tParameterTypes.Length;
			if ( tempListLength <= 6 && tTarget != null && tMethodInfo != null && tMethodInfo.ReturnType == typeof( void ) )
			{
				switch ( tempListLength )
				{
					case 0:
						return Delegate.CreateDelegate( typeof( Action ), tTarget, tMethodInfo, false );
					case 1:
						return Delegate.CreateDelegate( typeof( Action<> ).MakeGenericType( tParameterTypes ), tTarget, tMethodInfo, false );
					case 2:
						return Delegate.CreateDelegate( typeof( Action<,> ).MakeGenericType( tParameterTypes ), tTarget, tMethodInfo, false );
					case 3:
						return Delegate.CreateDelegate( typeof( Action<,,> ).MakeGenericType( tParameterTypes ), tTarget, tMethodInfo, false );
					case 4:
						return Delegate.CreateDelegate( typeof( Action<,,,> ).MakeGenericType( tParameterTypes ), tTarget, tMethodInfo, false );
					case 5:
						return Delegate.CreateDelegate( typeof( Action<,,,,> ).MakeGenericType( tParameterTypes ), tTarget, tMethodInfo, false );
					case 6:
						return Delegate.CreateDelegate( typeof( Action<,,,,,> ).MakeGenericType( tParameterTypes ), tTarget, tMethodInfo, false );
					default:
						break;
				}
			}
			
			return null;
		}
		
		/// <summary>Factory function that tries to create a delegate for a method with a return type</summary>
		/// <param name="tTarget">Owner instance of the method</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tParameterTypes">Parameter types of the method's definition, assumes the last is the method's return type</param>
		/// <returns>Delegate pointing to the method if successful, null if not</returns>
		public static System.Delegate CreateFuncDelegate( object tTarget, MethodInfo tMethodInfo, Type[] tParameterTypes )
		{
			int tempListLength = tParameterTypes == null ? 0 : tParameterTypes.Length;
			if ( tempListLength >= 1 && tempListLength <= 7 && tTarget != null && tMethodInfo != null && tMethodInfo.ReturnType != typeof( void ) )
			{
				switch ( tempListLength )
				{
					case 1:
						return typeof( Utility ).GetMethod( "CreateFuncAction" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo } ) as System.Delegate;
					case 2:
						return typeof( Utility ).GetMethod( "CreateFuncAction1" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo } ) as System.Delegate;
					case 3:
						return typeof( Utility ).GetMethod( "CreateFuncAction2" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo } ) as System.Delegate;
					case 4:
						return typeof( Utility ).GetMethod( "CreateFuncAction3" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo } ) as System.Delegate;
					case 5:
						return typeof( Utility ).GetMethod( "CreateFuncAction4" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo } ) as System.Delegate;
					case 6:
						return typeof( Utility ).GetMethod( "CreateFuncAction5" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo } ) as System.Delegate;
					case 7:
						return typeof( Utility ).GetMethod( "CreateFuncAction6" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo } ) as System.Delegate;
					default:
						break;
				}
			}
			
			return null;
		}
		
		/// <summary>Factory helper that will try to create a delegate that passes predefined arguments into a method</summary>
		/// <param name="tTarget">Owner instance of the method</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tParameterTypes">Parameter types of the method's definition</param>
		/// <param name="tArguments">Arguments to pass in</param>
		/// <param name="tFactoryAction">Factory function to use for creating a delegate from the method if it doesn't have a return type</param>
		/// <param name="tFactoryFunc">Factory function to use for creating a delegate from the method if it does have a return type</param>
		/// <returns>Action delegate that passes arguments into the target method if successful, null if not</returns>
		public static Action CreateCall( object tTarget, MethodInfo tMethodInfo, Type[] tParameterTypes, object[] tArguments, Func<object,MethodInfo,Type[],object[],Action> tFactoryAction, Func<object,MethodInfo,Type[],object[],Action> tFactoryFunc )
		{
			if ( tTarget != null && tMethodInfo != null )
			{
				// Action
				if ( tMethodInfo.ReturnType == typeof( void ) )
				{
					Func<object,MethodInfo,Type[],object[],Action> tempFactory = tFactoryAction;
					if ( tempFactory != null )
					{
						return tempFactory( tTarget, tMethodInfo, tParameterTypes, tArguments );
					}
				}
				// Func (assumes last parameter is return type)
				else
				{
					Func<object,MethodInfo,Type[],object[],Action> tempFactory = tFactoryFunc;
					if ( tempFactory != null )
					{
						int tempListLength = tParameterTypes == null ? 1 : 1 + tParameterTypes.Length;
						Type[] tempFuncTypes = new Type[ tempListLength ];
						tempFuncTypes[ tempListLength - 1 ] = tMethodInfo.ReturnType;
						tParameterTypes.CopyTo( tempFuncTypes, 0 );
						
						return tempFactory( tTarget, tMethodInfo, tempFuncTypes, tArguments );
					}
				}
			}
		
			return null;
		}
		
		/// <summary>Factory function that tries to create a delegate that passes predefined arguments into a method without a return type</summary>
		/// <param name="tTarget">Owner instance of the method</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tParameterTypes">Parameter types of the method's definition</param>
		/// <param name="tArguments">Arguments to pass in</param>
		/// <returns>Action delegate that passes arguments into the target method when invoked</returns>
		public static Action CreateActionCall( object tTarget, MethodInfo tMethodInfo, Type[] tParameterTypes, object[] tArguments )
		{
			int tempListLength = tParameterTypes == null ? 0 : tParameterTypes.Length;
			if ( tempListLength >= 1 && tempListLength <= 6 && tTarget != null && tArguments != null && tArguments.Length >= tempListLength && tMethodInfo != null && tMethodInfo.ReturnType == typeof( void ) )
			{
				switch ( tempListLength )
				{
					case 1:
						return typeof( Utility ).GetMethod( "CreateCall1" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo, tArguments[0] } ) as Action;
					case 2:
						return typeof( Utility ).GetMethod( "CreateCall2" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo, tArguments[0], tArguments[1] } ) as Action;
					case 3:
						return typeof( Utility ).GetMethod( "CreateCall3" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo, tArguments[0], tArguments[1], tArguments[2] } ) as Action;
					case 4:
						return typeof( Utility ).GetMethod( "CreateCall4" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo, tArguments[0], tArguments[1], tArguments[2], tArguments[3] } ) as Action;
					case 5:
						return typeof( Utility ).GetMethod( "CreateCall5" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo, tArguments[0], tArguments[1], tArguments[2], tArguments[3], tArguments[4] } ) as Action;
					case 6:
						return typeof( Utility ).GetMethod( "CreateCall6" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo, tArguments[0], tArguments[1], tArguments[2], tArguments[3], tArguments[4], tArguments[5] } ) as Action;
					default:
						break;
				}
			}
			
			return null;
		}
		
		/// <summary>Factory function that tries to create a delegate that passes predefined arguments into a method with a return type</summary>
		/// <param name="tTarget">Owner instance of the method</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tParameterTypes">Parameter types of the method's definition</param>
		/// <param name="tArguments">Arguments to pass in</param>
		/// <returns>Action delegate that passes arguments into the target method when invoked</returns>
		public static Action CreateFuncCall( object tTarget, MethodInfo tMethodInfo, Type[] tParameterTypes, object[] tArguments )
		{
			int tempListLength = tParameterTypes == null ? 0 : tParameterTypes.Length;
			if ( tempListLength >= 2 && tempListLength <= 7 && tTarget != null && tArguments != null && tArguments.Length >= ( tempListLength - 1 ) && tMethodInfo != null && tMethodInfo.ReturnType != typeof( void ) )
			{
				switch ( tempListLength )
				{
					case 2:
						return typeof( Utility ).GetMethod( "CreateFuncCall1" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo, tArguments[0] } ) as Action;
					case 3:
						return typeof( Utility ).GetMethod( "CreateFuncCall2" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo, tArguments[0], tArguments[1] } ) as Action;
					case 4:
						return typeof( Utility ).GetMethod( "CreateFuncCall3" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo, tArguments[0], tArguments[1], tArguments[2] } ) as Action;
					case 5:
						return typeof( Utility ).GetMethod( "CreateFuncCall4" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo, tArguments[0], tArguments[1], tArguments[2], tArguments[3] } ) as Action;
					case 6:
						return typeof( Utility ).GetMethod( "CreateFuncCall5" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo, tArguments[0], tArguments[1], tArguments[2], tArguments[3], tArguments[4] } ) as Action;
					case 7:
						return typeof( Utility ).GetMethod( "CreateFuncCall6" ).MakeGenericMethod( tParameterTypes ).Invoke( null, new object[] { tTarget, tMethodInfo, tArguments[0], tArguments[1], tArguments[2], tArguments[3], tArguments[4], tArguments[5] } ) as Action;
					default:
						break;
				}
			}
			
			return null;
		}
		
		//=======================
		// Field Action
		//=======================
		/// <summary>Tries to create a 1-parameter delegate for a variable</summary>
		/// <param name="tTarget">Owner instance of the variable</param>
		/// <param name="tFieldInfo">FieldInfo of the target variable</param>
		/// <returns>1-parameter Action delegate pointing to the variable if successful, null if not</returns>
		public static Action<A> CreateFieldSingleAction<T,A>( T tTarget, FieldInfo tFieldInfo )
		{
			if ( tTarget != null && tFieldInfo != null && !tFieldInfo.IsStatic && tFieldInfo.ReflectedType == typeof(T) && tFieldInfo.FieldType == typeof(A) )
			{
				#if UNITY_IOS
					Action<A> tempAction = ( A tA ) =>
					{
						tFieldInfo.SetValue( tTarget, tA );
					};
					
					return tempAction;
				#else
					Action<T,A> tempDelegate = CreateFieldAction<T,A>( tFieldInfo );
					if ( tempDelegate != null )
					{
						Action<A> tempAction = ( A tA ) =>
						{
							tempDelegate( tTarget, tA );
						};
						
						return tempAction;
					}
				#endif
			}
			
			return null;
		}
		
		#if !UNITY_IOS
			/// <summary>Tries to create a 2-parameter delegate for a variable</summary>
			/// <param name="tFieldInfo">FieldInfo of the target variable</param>
			/// <returns>2-parameter Action delegate pointing to the variable if successful, null if not</returns>
			public static Action<T,A> CreateFieldAction<T,A>( FieldInfo tFieldInfo )
			{
				if ( tFieldInfo != null && !tFieldInfo.IsStatic && tFieldInfo.ReflectedType == typeof(T) && tFieldInfo.FieldType == typeof(A) )
				{
					// Black magic, compiles code at runtime to make subsequent calls quicker
					DynamicMethod tempMethod = new DynamicMethod( ( tFieldInfo.ReflectedType.FullName + ".set_" + tFieldInfo.Name ), null, new Type[2] { typeof(T), typeof(A) }, true );
				
					ILGenerator tempGenerator = tempMethod.GetILGenerator();
					tempGenerator.Emit( OpCodes.Ldarg_0 );
					tempGenerator.Emit( OpCodes.Ldarg_1 );
					tempGenerator.Emit( OpCodes.Stfld, tFieldInfo );
					tempGenerator.Emit( OpCodes.Ret );
					
					return tempMethod.CreateDelegate( typeof( Action<T,A> ) ) as Action<T,A>;
				}
				
				return null;
			}
		#endif
		
		//=======================
		// Call
		//=======================
		/// <summary>Tries to create a delegate that will set a variable to a predefined argument value</summary>
		/// <param name="tTarget">Owner instance of the variable</param>
		/// <param name="tFieldInfo">FieldInfo of the target variable</param>
		/// <param name="tA">Argument to set the variable to</param>
		/// <returns>Action delegate that will set the variable to <paramref name="tA"/> if invoked</returns>
		public static Action CreateFieldCall<T,A>( T tTarget, FieldInfo tFieldInfo, A tA )
		{
			if ( tTarget != null && tFieldInfo != null && !tFieldInfo.IsStatic && tFieldInfo.ReflectedType == typeof(T) && tFieldInfo.FieldType == typeof(A) )
			{
				#if UNITY_IOS
					Action tempAction = () =>
					{
						tFieldInfo.SetValue( tTarget, tA );
					};
					
					return tempAction;
				#else
					Action<T,A> tempDelegate = CreateFieldAction<T,A>( tFieldInfo );
					if ( tempDelegate != null )
					{
						Action tempAction = () =>
						{
							tempDelegate( tTarget, tA );
						};
						
						return tempAction;
					}
				#endif
			}
			
			return null;
		}
		
		/// <summary>Tries to create a delegate that will pass 1 predefined argument into a method</summary>
		/// <param name="tTarget">Owner instance of the method</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tA">Argument to pass</param>
		/// <returns>Action delegate that will pass <paramref name="tA"/> into the method if invoked</returns>
		public static Action CreateCall1<A>( object tTarget, MethodInfo tMethodInfo, A tA )
		{
			if ( tTarget != null && tMethodInfo != null )
			{
				Action<A> tempDelegate = Delegate.CreateDelegate( typeof( Action<A> ), tTarget, tMethodInfo, false ) as Action<A>;
				if ( tempDelegate != null )
				{
					Action tempAction = () =>
					{
						tempDelegate( tA );
					};
					
					return tempAction;
				}
			}
			
			return null;
		}
		
		/// <summary>Tries to create a delegate that will pass 2 predefined arguments into a method</summary>
		/// <param name="tTarget">Owner instance of the method</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tA">Argument to pass</param>
		/// <param name="tB">Argument to pass</param>
		/// <returns>Action delegate that will pass the arguments into the method if invoked</returns>
		public static Action CreateCall2<A,B>( object tTarget, MethodInfo tMethodInfo, A tA, B tB )
		{
			if ( tTarget != null && tMethodInfo != null )
			{
				Action<A,B> tempDelegate = Delegate.CreateDelegate( typeof( Action<A,B> ), tTarget, tMethodInfo, false ) as Action<A,B>;
				if ( tempDelegate != null )
				{
					Action tempAction = () =>
					{
						tempDelegate( tA, tB );
					};
					
					return tempAction;
				}
			}
			
			return null;
		}
		
		/// <summary>Tries to create a delegate that will pass 3 predefined arguments into a method</summary>
		/// <param name="tTarget">Owner instance of the method</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tA">Argument to pass</param>
		/// <param name="tB">Argument to pass</param>
		/// <param name="tC">Argument to pass</param>
		/// <returns>Action delegate that will pass the arguments into the method if invoked</returns>
		public static Action CreateCall3<A,B,C>( object tTarget, MethodInfo tMethodInfo, A tA, B tB, C tC )
		{
			if ( tTarget != null && tMethodInfo != null )
			{
				Action<A,B,C> tempDelegate = Delegate.CreateDelegate( typeof( Action<A,B,C> ), tTarget, tMethodInfo, false ) as Action<A,B,C>;
				if ( tempDelegate != null )
				{
					Action tempAction = () =>
					{
						tempDelegate( tA, tB, tC );
					};
					
					return tempAction;
				}
			}
			
			return null;
		}
		
		/// <summary>Tries to create a delegate that will pass 4 predefined arguments into a method</summary>
		/// <param name="tTarget">Owner instance of the method</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tA">Argument to pass</param>
		/// <param name="tB">Argument to pass</param>
		/// <param name="tC">Argument to pass</param>
		/// <param name="tD">Argument to pass</param>
		/// <returns>Action delegate that will pass the arguments into the method if invoked</returns>
		public static Action CreateCall4<A,B,C,D>( object tTarget, MethodInfo tMethodInfo, A tA, B tB, C tC, D tD )
		{
			if ( tTarget != null && tMethodInfo != null )
			{
				Action<A,B,C,D> tempDelegate = Delegate.CreateDelegate( typeof( Action<A,B,C,D> ), tTarget, tMethodInfo, false ) as Action<A,B,C,D>;
				if ( tempDelegate != null )
				{
					Action tempAction = () =>
					{
						tempDelegate( tA, tB, tC, tD );
					};
					
					return tempAction;
				}
			}
			
			return null;
		}
		
		/// <summary>Tries to create a delegate that will pass 5 predefined arguments into a method</summary>
		/// <param name="tTarget">Owner instance of the method</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tA">Argument to pass</param>
		/// <param name="tB">Argument to pass</param>
		/// <param name="tC">Argument to pass</param>
		/// <param name="tD">Argument to pass</param>
		/// <param name="tE">Argument to pass</param>
		/// <returns>Action delegate that will pass the arguments into the method if invoked</returns>
		public static Action CreateCall5<A,B,C,D,E>( object tTarget, MethodInfo tMethodInfo, A tA, B tB, C tC, D tD, E tE )
		{
			if ( tTarget != null && tMethodInfo != null )
			{
				Action<A,B,C,D,E> tempDelegate = Delegate.CreateDelegate( typeof( Action<A,B,C,D,E> ), tTarget, tMethodInfo, false ) as Action<A,B,C,D,E>;
				if ( tempDelegate != null )
				{
					Action tempAction = () =>
					{
						tempDelegate( tA, tB, tC, tD, tE );
					};
					
					return tempAction;
				}
			}
			
			return null;
		}
		
		/// <summary>Tries to create a delegate that will pass 6 predefined arguments into a method</summary>
		/// <param name="tTarget">Owner instance of the method</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tA">Argument to pass</param>
		/// <param name="tB">Argument to pass</param>
		/// <param name="tC">Argument to pass</param>
		/// <param name="tD">Argument to pass</param>
		/// <param name="tE">Argument to pass</param>
		/// <param name="tF">Argument to pass</param>
		/// <returns>Action delegate that will pass the arguments into the method if invoked</returns>
		public static Action CreateCall6<A,B,C,D,E,F>( object tTarget, MethodInfo tMethodInfo, A tA, B tB, C tC, D tD, E tE, F tF )
		{
			if ( tTarget != null && tMethodInfo != null )
			{
				Action<A,B,C,D,E,F> tempDelegate = Delegate.CreateDelegate( typeof( Action<A,B,C,D,E,F> ), tTarget, tMethodInfo, false ) as Action<A,B,C,D,E,F>;
				if ( tempDelegate != null )
				{
					Action tempAction = () =>
					{
						tempDelegate( tA, tB, tC, tD, tE, tF );
					};
					
					return tempAction;
				}
			}
			
			return null;
		}
	}
}