using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace EventsPlus
{	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Serializable reflection data that gets converted into a Request by a Subscriber</summary>
	[Serializable]
	public class RawRequest : IRawRequest
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
		/// <summary>List of tags that will be used when matching to potential Publishers</summary>
		[SerializeField]
		protected List<string> _tags;
	
		//=======================
		// Constructor
		//=======================
		public RawRequest()
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
		
		public virtual List<string> tags
		{
			get
			{
				return _tags;
			}
		}
		
		//=======================
		// Request
		//=======================
		/// <summary>Tries to create and register a Request from this raw form</summary>
		/// <param name="tSubscriber">Subscriber instance to associate the Request with</param>
		/// <returns>True if successfully created and added to the <paramref name="tSubscriber"/></returns>
		public virtual bool createAndRegister( ISubscriber tSubscriber )
		{
			if ( tSubscriber != null )
			{
				return tSubscriber.addRequest( createRequest( tSubscriber ) );
			}
			
			return false;
		}
		
		/// <summary>Tries to create a Request from this raw form</summary>
		/// <param name="tSubscriber">Subscriber instance to associate the Request with</param>
		/// <returns>Request instance if successful, null if not</returns>
		public virtual IRequest createRequest( ISubscriber tSubscriber )
		{
			if ( tSubscriber != null && _target != null && _tags != null && _tags.Count > 0 )
			{
				InfoType tempType;
				string tempName;
				Type[] tempTypes;
				if ( Utility.ReadReflectionData( _targetMethod, out tempType, out tempName, out tempTypes ) )
				{
					switch ( tempType )
					{
						case InfoType.Field:
							return createRequest( tSubscriber, _target.GetType().GetField( tempName ) );
						case InfoType.Property:
							return createRequest( tSubscriber, _target.GetType().GetProperty( tempName ) );
						case InfoType.Method:
							return createRequest( tSubscriber, _target.GetType().GetMethod( tempName, tempTypes ), tempTypes );
						default:
							break;
					}
				}
			}
			
			return null;
		}
		
		/// <summary>Tries to create a Request from this raw form that targets a variable</summary>
		/// <param name="tSubscriber">Subscriber instance to associate the Request with</param>
		/// <param name="tFieldInfo">FieldInfo of the target variable</param>
		/// <returns>Request instance if successful, null if not</returns>
		protected virtual IRequest createRequest( ISubscriber tSubscriber, FieldInfo tFieldInfo )
		{
			System.Delegate tempDelegate = Utility.CreateDelegate( _target, tFieldInfo );
			if ( tempDelegate != null )
			{
				return Activator.CreateInstance( typeof( Request<> ).MakeGenericType( new Type[] { tFieldInfo.FieldType } ), new object[] { tSubscriber, _tags, tempDelegate } ) as IRequest;
			}
			
			return null;
		}
		
		/// <summary>Tries to create a Request from this raw form that targets a property</summary>
		/// <param name="tSubscriber">Subscriber instance to associate the Request with</param>
		/// <param name="tPropertyInfo">PropertyInfo of the target property</param>
		/// <returns>Request instance if successful, null if not</returns>
		protected virtual IRequest createRequest( ISubscriber tSubscriber, PropertyInfo tPropertyInfo )
		{
			System.Delegate tempDelegate = Utility.CreateDelegate( _target, tPropertyInfo );
			if ( tempDelegate != null )
			{
				return Activator.CreateInstance( typeof( Request<> ).MakeGenericType( new Type[] { tPropertyInfo.PropertyType } ), new object[] { tSubscriber, _tags, tempDelegate } ) as IRequest;
			}
			
			return null;
		}
		
		/// <summary>Tries to create a Request from this raw form that targets a method</summary>
		/// <param name="tSubscriber">Subscriber instance to associate the Request with</param>
		/// <param name="tMethodInfo">MethodInfo of the target method</param>
		/// <param name="tParameterTypes">Parameter types of the target method</param>
		/// <returns>Request instance if successful, null if not</returns>
		protected virtual IRequest createRequest( ISubscriber tSubscriber, MethodInfo tMethodInfo, Type[] tParameterTypes )
		{
			int tempListLength = tParameterTypes == null ? 0 : tParameterTypes.Length;
			if ( tempListLength <= 6 )
			{
				System.Delegate tempDelegate = Utility.CreateDelegate( _target, tMethodInfo, tParameterTypes, Utility.CreateActionDelegate, Utility.CreateFuncDelegate );
				if ( tempDelegate != null )
				{
					switch ( tempListLength )
					{
						case 0:
							return Activator.CreateInstance( typeof( Request ), new object[] { tSubscriber, _tags, tempDelegate } ) as IRequest;
						case 1:
							return Activator.CreateInstance( typeof( Request<> ).MakeGenericType( tParameterTypes ), new object[] { tSubscriber, _tags, tempDelegate } ) as IRequest;
						case 2:
							return Activator.CreateInstance( typeof( Request<,> ).MakeGenericType( tParameterTypes ), new object[] { tSubscriber, _tags, tempDelegate } ) as IRequest;
						case 3:
							return Activator.CreateInstance( typeof( Request<,,> ).MakeGenericType( tParameterTypes ), new object[] { tSubscriber, _tags, tempDelegate } ) as IRequest;
						case 4:
							return Activator.CreateInstance( typeof( Request<,,,> ).MakeGenericType( tParameterTypes ), new object[] { tSubscriber, _tags, tempDelegate } ) as IRequest;
						case 5:
							return Activator.CreateInstance( typeof( Request<,,,,> ).MakeGenericType( tParameterTypes ), new object[] { tSubscriber, _tags, tempDelegate } ) as IRequest;
						case 6:
							return Activator.CreateInstance( typeof( Request<,,,,,> ).MakeGenericType( tParameterTypes ), new object[] { tSubscriber, _tags, tempDelegate } ) as IRequest;
						default:
							break;
					}
				}
			}
			
			return null;
		}
	}
}