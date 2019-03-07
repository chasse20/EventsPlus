using System;
using System.Reflection;
using UnityEngine;

namespace EventsPlus
{	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Serialized form of a delegate that can be wired up from a <see cref="Subscriber"/></summary>
	[Serializable]
	public class RawRequest : RawDelegate
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Tag keys used for hooking up to matching <see cref="Publisher"/>s</summary>
		[SerializeField]
		protected string[] _tags = new string[1]; // must always be at least one tag
		
		//=======================
		// Tags
		//=======================
		/// <summary>Gets the <see cref="_tags"/> array</summary>
		public string[] tags
		{
			get
			{
				return _tags;
			}
		}
		
		//=======================
		// Delegate
		//=======================
		public override System.Delegate createDelegate( MemberInfo tMember )
		{
			// Func wrapping
			if ( tMember != null && tMember.MemberType == MemberTypes.Method )
			{
				MethodInfo tempMethod = tMember as MethodInfo;
				if ( tempMethod.ReturnType != typeof( void ) )
				{
					// Parameters
					ParameterInfo[] tempParameters = tempMethod.GetParameters();
					int tempParametersLength = tempParameters.Length;
					Type[] tempParameterTypes = new Type[ tempParametersLength + 1 ];
					for ( int i = ( tempParametersLength - 1 ); i >= 0; --i )
					{
						tempParameterTypes[i] = tempParameters[i].ParameterType;
					}
					
					tempParameterTypes[ tempParametersLength ] = tempMethod.ReturnType;
					
					// Delegate
					return this.GetType().GetMethod( "createFunc" + tempParametersLength, Utility.InstanceFlags ).MakeGenericMethod( tempParameterTypes ).Invoke( this, new object[] { tempMethod } ) as System.Delegate;
				}
			}

			// Inheritance
			return base.createDelegate( tMember );
		}
		
		//=======================
		// Func
		//=======================
		/// <summary>Utility method for creating a method delegate from a <see cref="System.Reflection.MethodInfo"/> that has a return value</summary>
		/// <param name="tMethod">MethodInfo used to generate a delegate</param>
		/// <returns>Generic action delegate if successful, null if not able to convert</returns>
		protected virtual Action createFunc0<T>( MethodInfo tMethod )
		{
			Func<T> tempDelegate = Delegate.CreateDelegate( typeof( Func<T> ), _target, tMethod, false ) as Func<T>;
			Action tempAction = () =>
			{
				tempDelegate();
			};
			
			return tempAction;
		}
		
		/// <summary>Utility method for creating a 1-parameter method delegate from a <see cref="System.Reflection.MethodInfo"/> that has a return value</summary>
		/// <param name="tMethod">MethodInfo used to generate a delegate</param>
		/// <returns>Generic action delegate if successful, null if not able to convert</returns>
		protected virtual Action<A> createFunc1<A,T>( MethodInfo tMethod )
		{
			Func<A,T> tempDelegate = Delegate.CreateDelegate( typeof( Func<A,T> ), _target, tMethod, false ) as Func<A,T>;
			Action<A> tempAction = ( A tA ) =>
			{
				tempDelegate( tA );
			};
			
			return tempAction;
		}
		
		/// <summary>Utility method for creating a 2-parameter method delegate from a <see cref="System.Reflection.MethodInfo"/> that has a return value</summary>
		/// <param name="tMethod">MethodInfo used to generate a delegate</param>
		/// <returns>Generic action delegate if successful, null if not able to convert</returns>
		protected virtual Action<A,B> createFunc2<A,B,T>( MethodInfo tMethod )
		{
			Func<A,B,T> tempDelegate = Delegate.CreateDelegate( typeof( Func<A,B,T> ), _target, tMethod, false ) as Func<A,B,T>;
			Action<A,B> tempAction = ( A tA, B tB ) =>
			{
				tempDelegate( tA, tB );
			};
			
			return tempAction;
		}
		
		/// <summary>Utility method for creating a 3-parameter method delegate from a <see cref="System.Reflection.MethodInfo"/> that has a return value</summary>
		/// <param name="tMethod">MethodInfo used to generate a delegate</param>
		/// <returns>Generic action delegate if successful, null if not able to convert</returns>
		protected virtual Action<A,B,C> createFunc3<A,B,C,T>( MethodInfo tMethod )
		{
			Func<A,B,C,T> tempDelegate = Delegate.CreateDelegate( typeof( Func<A,B,C,T> ), _target, tMethod, false ) as Func<A,B,C,T>;
			Action<A,B,C> tempAction = ( A tA, B tB, C tC ) =>
			{
				tempDelegate( tA, tB, tC );
			};
			
			return tempAction;
		}
		
		/// <summary>Utility method for creating a 4-parameter method delegate from a <see cref="System.Reflection.MethodInfo"/> that has a return value</summary>
		/// <param name="tMethod">MethodInfo used to generate a delegate</param>
		/// <returns>Generic action delegate if successful, null if not able to convert</returns>
		protected virtual Action<A,B,C,D> createFunc4<A,B,C,D,T>( MethodInfo tMethod )
		{
			Func<A,B,C,D,T> tempDelegate = Delegate.CreateDelegate( typeof( Func<A,B,C,D,T> ), _target, tMethod, false ) as Func<A,B,C,D,T>;
			Action<A,B,C,D> tempAction = ( A tA, B tB, C tC, D tD ) =>
			{
				tempDelegate( tA, tB, tC, tD );
			};
			
			return tempAction;
		}
		
		/// <summary>Utility method for creating a 5-parameter method delegate from a <see cref="System.Reflection.MethodInfo"/> that has a return value</summary>
		/// <param name="tMethod">MethodInfo used to generate a delegate</param>
		/// <returns>Generic action delegate if successful, null if not able to convert</returns>
		protected virtual Action<A,B,C,D,E> createFunc5<A,B,C,D,E,T>( MethodInfo tMethod )
		{
			Func<A,B,C,D,E,T> tempDelegate = Delegate.CreateDelegate( typeof( Func<A,B,C,D,E,T> ), _target, tMethod, false ) as Func<A,B,C,D,E,T>;
			Action<A,B,C,D,E> tempAction = ( A tA, B tB, C tC, D tD, E tE ) =>
			{
				tempDelegate( tA, tB, tC, tD, tE );
			};
			
			return tempAction;
		}
		
		/// <summary>Utility method for creating a 6-parameter method delegate from a <see cref="System.Reflection.MethodInfo"/> that has a return value</summary>
		/// <param name="tMethod">MethodInfo used to generate a delegate</param>
		/// <returns>Generic action delegate if successful, null if not able to convert</returns>
		protected virtual Action<A,B,C,D,E,F> createFunc6<A,B,C,D,E,F,T>( MethodInfo tMethod )
		{
			Func<A,B,C,D,E,F,T> tempDelegate = Delegate.CreateDelegate( typeof( Func<A,B,C,D,E,F,T> ), _target, tMethod, false ) as Func<A,B,C,D,E,F,T>;
			Action<A,B,C,D,E,F> tempAction = ( A tA, B tB, C tC, D tD, E tE, F tF ) =>
			{
				tempDelegate( tA, tB, tC, tD, tE, tF );
			};
			
			return tempAction;
		}
		
		/// <summary>Utility method for creating a 7-parameter method delegate from a <see cref="System.Reflection.MethodInfo"/> that has a return value</summary>
		/// <param name="tMethod">MethodInfo used to generate a delegate</param>
		/// <returns>Generic action delegate if successful, null if not able to convert</returns>
		protected virtual Action<A,B,C,D,E,F,G> createFunc7<A,B,C,D,E,F,G,T>( MethodInfo tMethod )
		{
			Func<A,B,C,D,E,F,G,T> tempDelegate = Delegate.CreateDelegate( typeof( Func<A,B,C,D,E,F,G,T> ), _target, tMethod, false ) as Func<A,B,C,D,E,F,G,T>;
			Action<A,B,C,D,E,F,G> tempAction = ( A tA, B tB, C tC, D tD, E tE, F tF, G tG ) =>
			{
				tempDelegate( tA, tB, tC, tD, tE, tF, tG );
			};
			
			return tempAction;
		}
		
		/// <summary>Utility method for creating a 8-parameter method delegate from a <see cref="System.Reflection.MethodInfo"/> that has a return value</summary>
		/// <param name="tMethod">MethodInfo used to generate a delegate</param>
		/// <returns>Generic action delegate if successful, null if not able to convert</returns>
		protected virtual Action<A,B,C,D,E,F,G,H> createFunc8<A,B,C,D,E,F,G,H,T>( MethodInfo tMethod )
		{
			Func<A,B,C,D,E,F,G,H,T> tempDelegate = Delegate.CreateDelegate( typeof( Func<A,B,C,D,E,F,G,H,T> ), _target, tMethod, false ) as Func<A,B,C,D,E,F,G,H,T>;
			Action<A,B,C,D,E,F,G,H> tempAction = ( A tA, B tB, C tC, D tD, E tE, F tF, G tG, H tH ) =>
			{
				tempDelegate( tA, tB, tC, tD, tE, tF, tG, tH );
			};
			
			return tempAction;
		}
		
		/// <summary>Utility method for creating a 9-parameter method delegate from a <see cref="System.Reflection.MethodInfo"/> that has a return value</summary>
		/// <param name="tMethod">MethodInfo used to generate a delegate</param>
		/// <returns>Generic action delegate if successful, null if not able to convert</returns>
		protected virtual Action<A,B,C,D,E,F,G,H,I> createFunc9<A,B,C,D,E,F,G,H,I,T>( MethodInfo tMethod )
		{
			Func<A,B,C,D,E,F,G,H,I,T> tempDelegate = Delegate.CreateDelegate( typeof( Func<A,B,C,D,E,F,G,H,I,T> ), _target, tMethod, false ) as Func<A,B,C,D,E,F,G,H,I,T>;
			Action<A,B,C,D,E,F,G,H,I> tempAction = ( A tA, B tB, C tC, D tD, E tE, F tF, G tG, H tH, I tI ) =>
			{
				tempDelegate( tA, tB, tC, tD, tE, tF, tG, tH, tI );
			};
			
			return tempAction;
		}
		
		/// <summary>Utility method for creating a 10-parameter method delegate from a <see cref="System.Reflection.MethodInfo"/> that has a return value</summary>
		/// <param name="tMethod">MethodInfo used to generate a delegate</param>
		/// <returns>Generic action delegate if successful, null if not able to convert</returns>
		protected virtual Action<A,B,C,D,E,F,G,H,I,J> createFunc10<A,B,C,D,E,F,G,H,I,J,T>( MethodInfo tMethod )
		{
			Func<A,B,C,D,E,F,G,H,I,J,T> tempDelegate = Delegate.CreateDelegate( typeof( Func<A,B,C,D,E,F,G,H,I,J,T> ), _target, tMethod, false ) as Func<A,B,C,D,E,F,G,H,I,J,T>;
			Action<A,B,C,D,E,F,G,H,I,J> tempAction = ( A tA, B tB, C tC, D tD, E tE, F tF, G tG, H tH, I tI, J tJ ) =>
			{
				tempDelegate( tA, tB, tC, tD, tE, tF, tG, tH, tI, tJ );
			};
			
			return tempAction;
		}
	}
}