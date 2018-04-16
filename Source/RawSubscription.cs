using System;
using System.Reflection;
using UnityEngine;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Serializable reflection data that gets converted into a Subscription by a Publisher</summary>
	public class RawSubscription<T> : IRawSubscription<T> where T : RawArgument
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Target object containing the target method</summary>
		[SerializeField]
		protected UnityEngine.Object _target;
		/// <summary>Method description belonging to the target</summary>
		[SerializeField]
		protected string _targetMethod;
		/// <summary>Optional object instance that contains a Subscriber variable</summary>
		[SerializeField]
		protected UnityEngine.Object _subscriberOwner;
		/// <summary>Variable name of the Subscriber contained inside of the Subscriber owner instance</summary>
		[SerializeField]
		protected string _subscriberVariable;
		/// <summary>Whether the target method should be considered a dynamic invocation or treated as predefined call</summary>
		[SerializeField]
		protected bool _isDynamic;
		/// <summary>Raw arguments for when the target method is not dynamic</summary>
		[SerializeField]
		protected T[] _arguments;
	
		//=======================
		// Constructor
		//=======================
		public RawSubscription()
		{
		}
		
		//=======================
		// Accessors
		//=======================
		public virtual UnityEngine.Object target
		{
			get
			{
				return _target;
			}
		}
		
		public virtual string targetMethod
		{
			get
			{
				return _targetMethod;
			}
		}
		
		public virtual UnityEngine.Object subscriberOwner
		{
			get
			{
				return _subscriberOwner;
			}
		}
		
		public virtual string subscriberVariable
		{
			get
			{
				return _subscriberVariable;
			}
		}
		
		public virtual bool isDynamic
		{
			get
			{
				return _isDynamic;
			}
		}
		
		public virtual T[] arguments
		{
			get
			{
				return _arguments;
			}
		}
		
		/// <summary>Tries to return a Subscriber instance via reflection using the <see cref="_subscriberOwner"/> and <see cref="_subscriberVariable"/></summary>
		public virtual ISubscriber subscriber
		{
			get
			{
				if ( _subscriberOwner != null && !String.IsNullOrEmpty( _subscriberVariable ) )
				{
					FieldInfo tempField = _subscriberOwner.GetType().GetField( _subscriberVariable, ( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) );
					if ( tempField != null )
					{
						return tempField.GetValue( _subscriberOwner ) as ISubscriber;
					}
				}
				
				return null;
			}
		}
		
		//=======================
		// Subscription
		//=======================
		/// <summary>Tries to create and register a Subscription if a Subscriber could be found or a basic delegate if the Subscriber is left empty</summary>
		/// <param name="tPublisher">Publisher instance to associate the Subscription with</param>
		/// <returns>True if successfully created and added to the <paramref name="tPublisher"/></returns>
		public virtual bool createAndRegister( IPublisher tPublisher )
		{
			ISubscriber tempSubscriber = subscriber;
			if ( tempSubscriber == null )
			{
				return tPublisher.addDelegate( createDelegate() );
			}
			else
			{
				return tPublisher.addSubscription( createSubscription( tPublisher, tempSubscriber ) );
			}
		}
		
		/// <summary>Tries to create a Subscription from this raw form</summary>
		/// <param name="tPublisher">Publisher instance to associate the Subscription with</param>
		/// <param name="tSubscriber">Subscriber instance to associate the Subscription with</param>
		/// <returns>Subscription instance if successful, null if not</returns>
		public virtual ISubscription createSubscription( IPublisher tPublisher, ISubscriber tSubscriber )
		{
			if ( tPublisher != null && tSubscriber != null )
			{
				InfoType tempType;
				string tempName;
				Type[] tempTypes;
				if ( Utility.ReadReflectionData( _targetMethod, out tempType, out tempName, out tempTypes ) )
				{
					switch ( tempType )
					{
						case InfoType.Field:
							return createSubscription( tPublisher, tSubscriber, _target.GetType().GetField( tempName ) );
						case InfoType.Property:
							return createSubscription( tPublisher, tSubscriber, _target.GetType().GetProperty( tempName ) );
						case InfoType.Method:
							return createSubscription( tPublisher, tSubscriber, _target.GetType().GetMethod( tempName, tempTypes ), tempTypes );
						default:
							break;
					}
				}
			}
			
			return null;
		}
		
		/// <summary>Tries to create a Subscription from this raw form that targets a variable</summary>
		/// <param name="tPublisher">Publisher instance to associate the Subscription with</param>
		/// <param name="tSubscriber">Subscriber instance to associate the Subscription with</param>
		/// <param name="tFieldInfo">FieldInfo of the target variable</param>
		/// <returns>Subscription instance if successful, null if not</returns>
		protected virtual ISubscription createSubscription( IPublisher tPublisher, ISubscriber tSubscriber, FieldInfo tFieldInfo )
		{
			if ( _isDynamic )
			{
				System.Delegate tempDelegate = Utility.CreateDelegate( _target, tFieldInfo );
				if ( tempDelegate != null )
				{
					return Activator.CreateInstance( typeof( Subscription<> ).MakeGenericType( new Type[] { tFieldInfo.FieldType } ), new object[] { tSubscriber, tPublisher, tempDelegate } ) as ISubscription;
				}
			}
			else if ( _arguments != null && _arguments.Length > 0 && tFieldInfo != null )
			{
				System.Delegate tempDelegate = typeof( Utility ).GetMethod( "CreateFieldCall" ).MakeGenericMethod( new Type[] { tFieldInfo.ReflectedType, tFieldInfo.FieldType } ).Invoke( null, new object[] { _target, tFieldInfo, _arguments[0].genericValue } ) as System.Delegate;
				if ( tempDelegate != null )
				{
					return Activator.CreateInstance( typeof( Subscription ), new object[] { tSubscriber, tPublisher, tempDelegate } ) as ISubscription;
				}
			}
			
			return null;
		}
		
		/// <summary>Tries to create a Subscription from this raw form that targets a property</summary>
		/// <param name="tPublisher">Publisher instance to associate the Subscription with</param>
		/// <param name="tSubscriber">Subscriber instance to associate the Subscription with</param>
		/// <param name="tPropertyInfo">PropertyInfo of the target property</param>
		/// <returns>Subscription instance if successful, null if not</returns>
		protected virtual ISubscription createSubscription( IPublisher tPublisher, ISubscriber tSubscriber, PropertyInfo tPropertyInfo )
		{
			if ( _isDynamic )
			{
				System.Delegate tempDelegate = Utility.CreateDelegate( _target, tPropertyInfo );
				if ( tempDelegate != null )
				{
					return Activator.CreateInstance( typeof( Subscription<> ).MakeGenericType( new Type[] { tPropertyInfo.PropertyType } ), new object[] { tSubscriber, tPublisher, tempDelegate } ) as ISubscription;
				}
			}
			else if ( _arguments != null && _arguments.Length > 0 && tPropertyInfo != null )
			{
				System.Delegate tempDelegate = typeof( Utility ).GetMethod( "CreateCall1" ).MakeGenericMethod( new Type[] { tPropertyInfo.PropertyType } ).Invoke( null, new object[] { _target, tPropertyInfo.GetSetMethod(), _arguments[0].genericValue } ) as System.Delegate;
				if ( tempDelegate != null )
				{
					return Activator.CreateInstance( typeof( Subscription ), new object[] { tSubscriber, tPublisher, tempDelegate } ) as ISubscription;
				}
			}
			
			return null;
		}
		
		/// <summary>Tries to create a Subscription from this raw form that targets a method</summary>
		/// <param name="tPublisher">Publisher instance to associate the Subscription with</param>
		/// <param name="tSubscriber">Subscriber instance to associate the Subscription with</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tParameterTypes">Parameter types of the target method</param>
		/// <returns>Subscription instance if successful, null if not</returns>
		protected virtual ISubscription createSubscription( IPublisher tPublisher, ISubscriber tSubscriber, MethodInfo tMethodInfo, Type[] tParameterTypes )
		{
			if ( _isDynamic || _arguments == null || _arguments.Length == 0 )
			{
				return createDynamicSubscription( tPublisher, tSubscriber, tMethodInfo, tParameterTypes );
			}
			else
			{
				object[] tempArguments = new object[ _arguments.Length ];
				for ( int i = ( tempArguments.Length - 1 ); i >= 0; --i )
				{
					tempArguments[i] = _arguments[i].genericValue;
				}
				
				Action tempDelegate = Utility.CreateCall( _target, tMethodInfo, tParameterTypes, tempArguments, Utility.CreateActionCall, Utility.CreateFuncCall );
				if ( tempDelegate != null )
				{
					return Activator.CreateInstance( typeof( Subscription ), new object[] { tSubscriber, tPublisher, tempDelegate } ) as ISubscription;
				}
			}
			
			return null;
		}
		
		/// <summary>Tries to create a Subscription from this raw form that targets a dynamic method without predefined arguments</summary>
		/// <param name="tPublisher">Publisher instance to associate the Subscription with</param>
		/// <param name="tSubscriber">Subscriber instance to associate the Subscription with</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tParameterTypes">Parameter types of the target method</param>
		/// <returns>Subscription instance if successful, null if not</returns>
		protected virtual ISubscription createDynamicSubscription( IPublisher tPublisher, ISubscriber tSubscriber, MethodInfo tMethodInfo, Type[] tParameterTypes )
		{
			int tempListLength = tParameterTypes == null ? 0 : tParameterTypes.Length;
			if ( tempListLength <= 6 )
			{
				Delegate tempDelegate = Utility.CreateDelegate( _target, tMethodInfo, tParameterTypes, Utility.CreateActionDelegate, Utility.CreateFuncDelegate );
				if ( tempDelegate != null )
				{
					switch ( tempListLength )
					{
						case 0:
							return Activator.CreateInstance( typeof( Subscription ), new object[] { tSubscriber, tPublisher, tempDelegate } ) as ISubscription;
						case 1:
							return Activator.CreateInstance( typeof( Subscription<> ).MakeGenericType( tParameterTypes ), new object[] { tSubscriber, tPublisher, tempDelegate } ) as ISubscription;
						case 2:
							return Activator.CreateInstance( typeof( Subscription<,> ).MakeGenericType( tParameterTypes ), new object[] { tSubscriber, tPublisher, tempDelegate } ) as ISubscription;
						case 3:
							return Activator.CreateInstance( typeof( Subscription<,,> ).MakeGenericType( tParameterTypes ), new object[] { tSubscriber, tPublisher, tempDelegate } ) as ISubscription;
						case 4:
							return Activator.CreateInstance( typeof( Subscription<,,,> ).MakeGenericType( tParameterTypes ), new object[] { tSubscriber, tPublisher, tempDelegate } ) as ISubscription;
						case 5:
							return Activator.CreateInstance( typeof( Subscription<,,,,> ).MakeGenericType( tParameterTypes ), new object[] { tSubscriber, tPublisher, tempDelegate } ) as ISubscription;
						case 6:
							return Activator.CreateInstance( typeof( Subscription<,,,,,> ).MakeGenericType( tParameterTypes ), new object[] { tSubscriber, tPublisher, tempDelegate } ) as ISubscription;
						default:
							break;
					}
				}
			}
			
			return null;
		}
		
		//=======================
		// Delegate
		//=======================
		/// <summary>Tries to create a delegate from this raw form</summary>
		/// <returns>Delegate if successful, null if not</returns>
		public virtual System.Delegate createDelegate()
		{
			InfoType tempType;
			string tempName;
			Type[] tempTypes;
			if ( Utility.ReadReflectionData( _targetMethod, out tempType, out tempName, out tempTypes ) )
			{
				switch ( tempType )
				{
					case InfoType.Field:
						return createDelegate( _target.GetType().GetField( tempName ) );
					case InfoType.Property:
						return createDelegate( _target.GetType().GetProperty( tempName ) );
					case InfoType.Method:
						return createDelegate( _target.GetType().GetMethod( tempName, tempTypes ), tempTypes );
					default:
						break;
				}
			}
			
			return null;
		}
		
		/// <summary>Tries to create a delegate from this raw form that targets a variable</summary>
		/// <param name="tFieldInfo">FieldInfo of the target variable</param>
		/// <returns>Delegate if successful, null if not</returns>
		protected virtual System.Delegate createDelegate( FieldInfo tFieldInfo )
		{
			if ( _isDynamic )
			{
				return Utility.CreateDelegate( _target, tFieldInfo );
			}
			else if ( _arguments != null && _arguments.Length > 0 && tFieldInfo != null )
			{
				return typeof( Utility ).GetMethod( "CreateFieldCall" ).MakeGenericMethod( new Type[] { tFieldInfo.ReflectedType, tFieldInfo.FieldType } ).Invoke( null, new object[] { _target, tFieldInfo, _arguments[0].genericValue } ) as System.Delegate;
			}
			
			return null;
		}
		
		/// <summary>Tries to create a delegate from this raw form that targets a property</summary>
		/// <param name="tPropertyInfo">PropertyInfo of the target property</param>
		/// <returns>Delegate if successful, null if not</returns>
		protected virtual System.Delegate createDelegate( PropertyInfo tPropertyInfo )
		{
			if ( _isDynamic )
			{
				return Utility.CreateDelegate( _target, tPropertyInfo );
			}
			else if ( _arguments != null && _arguments.Length > 0 && tPropertyInfo != null )
			{
				return typeof( Utility ).GetMethod( "CreateCall1" ).MakeGenericMethod( new Type[] { tPropertyInfo.PropertyType } ).Invoke( null, new object[] { _target, tPropertyInfo.GetSetMethod(), _arguments[0].genericValue } ) as System.Delegate;
			}
			
			return null;
		}
		
		/// <summary>Tries to create a delegate from this raw form that targets a method</summary>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tParameterTypes">Parameter types of the target method</param>
		/// <returns>Delegate if successful, null if not</returns>
		protected virtual System.Delegate createDelegate( MethodInfo tMethodInfo, Type[] tParameterTypes )
		{
			if ( _isDynamic || _arguments == null || _arguments.Length == 0 )
			{
				return Utility.CreateDelegate( _target, tMethodInfo, tParameterTypes, Utility.CreateActionDelegate, Utility.CreateFuncDelegate );
			}

			object[] tempArguments = new object[ _arguments.Length ];
			for ( int i = ( tempArguments.Length - 1 ); i >= 0; --i )
			{
				tempArguments[i] = _arguments[i].genericValue;
			}
			
			return Utility.CreateCall( _target, tMethodInfo, tParameterTypes, tempArguments, Utility.CreateActionCall, Utility.CreateFuncCall );
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Serializable, non-generic raw form of a subscription</summary>
	[Serializable]
	public class RawSubscription : RawSubscription<RawArgument>
	{
	}
}