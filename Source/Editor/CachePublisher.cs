using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EventsPlus
{	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Editor container used to store cached data, in order to improve performance, for the RawSubscription elements inside of the Publisher drawer</summary>
	public class CachePublisher : Cache
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Cached method parameter info that correspond to each drop-down element, used for displaying predefined arguments</summary>
		protected CachedParameter[][] _methodParameters;
		/// <summary>Cached type signature of the associated Publisher, used for validation</summary>
		protected Type[] _publisherTypes;
		/// <summary>Container for method names that are allowed to be dynamic</summary>
		protected HashSet<string> _dynamicMethods;
		/// <summary>Display names of all the objects containing Subscribers, used by a drop-down</summary>
		protected string[] _subscriberOwnerNames;
		/// <summary>Corresponding object references for each of the <see cref="_subscriberOwnerNames"/></summary>
		protected UnityEngine.Object[] _subscriberOwnerObjects;
		/// <summary>Current index for the active drop-down Subscriber owner</summary>
		public int selectedSubscriberOwner = -1;
		/// <summary>Display names of all the Subscribers in the selected owner, used by a drop-down</summary>
		protected string[] _subscriberVariables;
		/// <summary>Current index for the active drop-down Subscriber variable</summary>
		public int selectedSubscriberVariable = -1;
		
		//=======================
		// Constructor
		//=======================
		public CachePublisher( Type[] tPublisherTypes, UnityEngine.Object tTarget, string tMethod, UnityEngine.Object tSubscriberOwner, string tSubscriberVariable )
		{
			_publisherTypes = tPublisherTypes;
			
			if ( cache( tTarget ) )
			{
				cache( tMethod );
			}
			
			if ( cacheSubscriber( tSubscriberOwner ) )
			{
				cacheSubscriber( tSubscriberVariable );
			}
		}
		
		//=======================
		// Destructor
		//=======================
		/// <summary>Clears the target, method and Subscriber data from memory</summary>
		public override void clear()
		{
			// Inheritance
			base.clear();
			
			// Methods
			if ( _methodParameters != null )
			{
				Array.Clear( _methodParameters, 0, _methodParameters.Length );
				_methodParameters = null;
			}
			if ( _dynamicMethods != null )
			{
				_dynamicMethods.Clear();
				_dynamicMethods = null;
			}
			
			// Subscribers
			clearSubscribers();
		}
		
		/// <summary>Clears the Subscriber data from memory</summary>
		public virtual void clearSubscribers()
		{
			// Subscribers
			if ( _subscriberOwnerNames != null )
			{
				Array.Clear( _subscriberOwnerNames, 0, _subscriberOwnerNames.Length );
				_subscriberOwnerNames = null;
			}
			if ( _subscriberOwnerObjects != null )
			{
				Array.Clear( _subscriberOwnerObjects, 0, _subscriberOwnerObjects.Length );
				_subscriberOwnerObjects = null;
			}
			selectedSubscriberOwner = -1;
			
			// Variables
			if ( _subscriberVariables != null )
			{
				Array.Clear( _subscriberVariables, 0, _subscriberVariables.Length );
				_subscriberVariables = null;
			}
			selectedSubscriberVariable = -1;
		}
		
		//=======================
		// Accessors
		//=======================		
		public virtual CachedParameter[][] methodParameters
		{
			get
			{
				return _methodParameters;
			}
		}
		
		public virtual List<string> dynamicMethods
		{
			get
			{
				return _dynamicMethods == null ? null : new List<string>( _dynamicMethods );
			}
		}
		
		/// <summary>Checks if a method can be treated as a dynamic delegate</summary>
		/// <param name="tMethod">Method name to check</param>
		/// <returns>True if <paramref name="tMethod"/> can be treated as dynamic</returns>
		public virtual bool hasDynamicMethod( string tMethod )
		{
			return _dynamicMethods != null && _dynamicMethods.Contains( tMethod );
		}
		
		public virtual string[] subscriberOwnerNames
		{
			get
			{
				return _subscriberOwnerNames;
			}
		}
		
		public virtual UnityEngine.Object[] subscriberOwnerObjects
		{
			get
			{
				return _subscriberOwnerObjects;
			}
		}
		
		public virtual string[] subscriberVariables
		{
			get
			{
				return _subscriberVariables;
			}
		}
		
		//=======================
		// Target
		//=======================
		public override bool cache( UnityEngine.Object tTarget )
		{
			if ( _dynamicMethods != null )
			{
				_dynamicMethods.Clear();
				_dynamicMethods = null;
			}
			
			return base.cache( tTarget );
		}
		
		//=======================
		// Method
		//=======================
		public override bool cache( string tMethod )
		{
			if ( _dynamicMethods != null )
			{
				_dynamicMethods.Clear();
				_dynamicMethods = null;
			}
			
			if ( selectedTarget > 0 && _targetObjects != null && selectedTarget < _targetObjects.Length )
			{
				Settings tempSettings = Settings.instance;
				
				selectedMethod = -1;
				List<string> tempReflectionNames = null;
				List<string> tempDisplayNames = null;
				List<CachedParameter[]> tempCachedParameters = null;
				Type tempType = _targetObjects[ selectedTarget ].GetType();
				
				// Binding Flags
				BindingFlags tempFlags = BindingFlags.Public | BindingFlags.Instance;
				if ( Settings.instance.isPrivateDisplayed )
				{
					tempFlags |= BindingFlags.NonPublic;
				}
				
				// Fields
				FieldInfo[] tempFields = tempType.GetFields( tempFlags );
				int tempListLength = tempFields.Length;
				for ( int i = 0; i < tempListLength; ++i )
				{
					cache( tempFields[i], ref tempReflectionNames, ref tempDisplayNames, ref tempCachedParameters, ref _dynamicMethods, tMethod, ref selectedMethod );
				}
				
				// Properties
				PropertyInfo[] tempProperties = tempType.GetProperties( tempFlags );
				tempListLength = tempProperties.Length;
				for ( int i = 0; i < tempListLength; ++i )
				{
					cache( tempProperties[i], ref tempReflectionNames, ref tempDisplayNames, ref tempCachedParameters, ref _dynamicMethods, tMethod, ref selectedMethod );
				}
				
				// Methods
				MethodInfo[] tempMethodInfos = tempType.GetMethods( tempFlags );
				tempListLength = tempMethodInfos.Length;
				for ( int i = 0; i < tempListLength; ++i )
				{
					cache( tempMethodInfos[i], ref tempReflectionNames, ref tempDisplayNames, ref tempCachedParameters, ref _dynamicMethods, tMethod, ref selectedMethod );
				}
				
				// Apply
				if ( tempReflectionNames.Count > 0 )
				{
					_methodReflectionNames = tempReflectionNames.ToArray();
					_methodDisplayNames = tempDisplayNames.ToArray();
					_methodParameters = tempCachedParameters.ToArray();
					
					if ( selectedMethod == -1 )
					{
						selectedMethod = 0;
					}
					
					return true;
				}
			}
			
			_methodReflectionNames = null;
			_methodDisplayNames = null;
			_methodParameters = null;
			
			return false;
		}
		
		/// <summary>Caches variable data for the "method" drop-down, determines dynamic status if it matches the <see cref="_publisherTypes"/> signature, and determines the selected index</summary>
		/// <param name="tField">FieldInfo associated with the variable to extract information from</param>
		/// <param name="tReflectionNames">Reference to an array that stores formatted reflection data for each cached element</param>
		/// <param name="tDisplayNames">Reference to an array that stores display names for each cached element</param>
		/// <param name="tCachedParameters">Reference to an array that stores any parameter data for each cached element</param>
		/// <param name="tDynamic">Reference to a container that is used to mark whether the variable can be considered as a possible dynamic delegate</param>
		/// <param name="tSelectedName">Cached name of the currently selected "method" name, used to check if this variable is selected</param>
		/// <param name="tSelectedIndex">Reference of the currently selected "method", will be set if <paramref name="tSelectedName"/> matches the variable name</param>
		/// <returns>True if successfully cached</returns>
		protected virtual bool cache( FieldInfo tField, ref List<string> tReflectionNames, ref List<string> tDisplayNames, ref List<CachedParameter[]> tCachedParameters, ref HashSet<string> tDynamic, string tSelectedName, ref int tSelectedIndex )
		{
			if ( cache( tField, ref tReflectionNames, ref tDisplayNames, tSelectedName, ref tSelectedIndex ) )
			{
				// Dynamic
				if ( _publisherTypes != null && _publisherTypes.Length == 1 && tField.FieldType == _publisherTypes[0] )
				{
					if ( tDynamic == null )
					{
						tDynamic = new HashSet<string>();
					}
					tDynamic.Add( tReflectionNames[ tReflectionNames.Count - 1 ] );
				}
				
				// Types
				if ( tCachedParameters == null )
				{
					tCachedParameters = new List<CachedParameter[]>();
				}
				tCachedParameters.Add( new CachedParameter[] { new CachedParameter( tField.Name, tField.FieldType.ToString() ) } );
		
				return true;
			}
			
			return false;
		}
		
		/// <summary>Caches property data for the "method" drop-down, determines dynamic status if it matches the <see cref="_publisherTypes"/> signature, and determines the selected index</summary>
		/// <param name="tProperty">PropertyInfo associated with the property to extract information from</param>
		/// <param name="tReflectionNames">Reference to an array that stores formatted reflection data for each cached element</param>
		/// <param name="tDisplayNames">Reference to an array that stores display names for each cached element</param>
		/// <param name="tCachedParameters">Reference to an array that stores any parameter data for each cached element</param>
		/// <param name="tDynamic">Reference to a container that is used to mark whether the property can be considered as a possible dynamic delegate</param>
		/// <param name="tSelectedName">Cached name of the currently selected "method" name, used to check if this property is selected</param>
		/// <param name="tSelectedIndex">Reference of the currently selected "method", will be set if <paramref name="tSelectedName"/> matches the property name</param>
		/// <returns>True if successfully cached</returns>
		protected virtual bool cache( PropertyInfo tProperty, ref List<string> tReflectionNames, ref List<string> tDisplayNames, ref List<CachedParameter[]> tCachedParameters, ref HashSet<string> tDynamic, string tSelectedName, ref int tSelectedIndex )
		{
			if ( cache( tProperty, ref tReflectionNames, ref tDisplayNames, tSelectedName, ref tSelectedIndex ) )
			{
				// Dynamic
				if ( _publisherTypes != null && _publisherTypes.Length == 1 && tProperty.PropertyType == _publisherTypes[0] )
				{
					if ( tDynamic == null )
					{
						tDynamic = new HashSet<string>();
					}
					tDynamic.Add( tReflectionNames[ tReflectionNames.Count - 1 ] );
				}
				
				// Types
				if ( tCachedParameters == null )
				{
					tCachedParameters = new List<CachedParameter[]>();
				}
				tCachedParameters.Add( new CachedParameter[] { new CachedParameter( tProperty.Name, tProperty.PropertyType.ToString() ) } );
				
				return true;
			}
			
			return false;
		}
		
		/// <summary>Caches method data for the "method" drop-down, determines dynamic status if it matches the <see cref="_publisherTypes"/> signature, and determines the selected index</summary>
		/// <param name="tMethod">MethodInfo associated with the method to extract information from</param>
		/// <param name="tReflectionNames">Reference to an array that stores formatted reflection data for each cached element</param>
		/// <param name="tDisplayNames">Reference to an array that stores display names for each cached element</param>
		/// <param name="tCachedParameters">Reference to an array that stores any parameter data for each cached element</param>
		/// <param name="tDynamic">Reference to a container that is used to mark whether the method can be considered as a possible dynamic delegate</param>
		/// <param name="tSelectedName">Cached name of the currently selected "method" name, used to check if this method is selected</param>
		/// <param name="tSelectedIndex">Reference of the currently selected "method", will be set if <paramref name="tSelectedName"/> matches the method name</param>
		/// <returns>True if successfully cached</returns>
		protected virtual bool cache( MethodInfo tMethod, ref List<string> tReflectionNames, ref List<string> tDisplayNames, ref List<CachedParameter[]> tCachedParameters, ref HashSet<string> tDynamic, string tSelectedName, ref int tSelectedIndex )
		{
			if ( cache( tMethod, ref tReflectionNames, ref tDisplayNames, tSelectedName, ref tSelectedIndex ) )
			{
				// Dynamic
				ParameterInfo[] tempParameters = tMethod.GetParameters();
				CachedParameter[] tempCachedParameters = null;
				bool tempIsDynamic = false;
				if ( tempParameters != null && tempParameters.Length > 0 )
				{
					tempIsDynamic = _publisherTypes != null && tempParameters.Length == _publisherTypes.Length;
					tempCachedParameters = new CachedParameter[ tempParameters.Length ];
					for ( int i = ( tempCachedParameters.Length - 1 ); i >= 0; --i )
					{
						tempCachedParameters[i] = new CachedParameter( tempParameters[i].Name, tempParameters[i].ParameterType.ToString() );
						if ( tempIsDynamic && tempParameters[i].ParameterType != _publisherTypes[i] )
						{
							tempIsDynamic = false;
						}
					}
					
					if ( tempIsDynamic )
					{
						if ( tDynamic == null )
						{
							tDynamic = new HashSet<string>();
						}
						tDynamic.Add( tReflectionNames[ tReflectionNames.Count - 1 ] );
					}
				}
				
				// Types
				if ( tCachedParameters == null )
				{
					tCachedParameters = new List<CachedParameter[]>();
				}
				tCachedParameters.Add( tempCachedParameters );
				
				return true;
			}
			
			return false;
		}
		
		//=======================
		// Subscriber
		//=======================
		/// <summary>Caches a readable drop-down list of possible objects that own Subscriber variables, determines selected Subscriber owner, and adjusts Subscriber variable drop-down accordingly</summary>
		/// <param name="tSubscriberOwner">Subscriber owner object to generate cached data from</param>
		/// <returns>True if successfully cached</returns>
		public virtual bool cacheSubscriber( UnityEngine.Object tSubscriberOwner )
		{
			if ( selectedTarget >= 0 && tSubscriberOwner != null )
			{
				// Component options
				selectedSubscriberOwner = -1;
				List<string> tempNames = new List<string>();
				List<UnityEngine.Object> tempObjects = new List<UnityEngine.Object>();
				GameObject tempGameObject = tSubscriberOwner as GameObject;
				Component tempComponent = tSubscriberOwner as Component;
				if ( tempGameObject != null || tempComponent != null )
				{					
					if ( tempGameObject == null )
					{
						tempGameObject = tempComponent.gameObject;
					}
					
					Type tempType;
					bool tempIsFound = tempComponent == null;
					string tempPrefix = tempGameObject.name + ".";
					Component[] tempComponents = tempGameObject.GetComponents<Component>();
					int tempListLength = tempComponents.Length;
					for ( int i = 0; i < tempListLength; ++i )
					{
						tempType = tempComponents[i].GetType();
						if ( validateSubscriber( tempType ) )
						{
							if ( !tempIsFound && tempComponents[i] == tempComponent )
							{
								selectedSubscriberOwner = tempNames.Count + 1;
								tempIsFound = true;
							}
							
							tempNames.Add( tempPrefix + tempType.Name );
							tempObjects.Add( tempComponents[i] );
						}
					}
				}
				// Generic option
				else if ( validateSubscriber( tSubscriberOwner.GetType() ) )
				{
					tempNames.Add( tSubscriberOwner.name );
					tempObjects.Add( tSubscriberOwner );
				}
				
				if ( tempNames.Count > 0 )
				{
					// None option
					tempNames.Insert( 0, "None" );
					tempObjects.Insert( 0, null );
					
					// Apply
					_subscriberOwnerNames = tempNames.ToArray();
					_subscriberOwnerObjects = tempObjects.ToArray();
					if ( selectedSubscriberOwner == -1 )
					{
						selectedSubscriberOwner = 1;
					}
					
					return true;
				}
				
				_subscriberOwnerNames = null;
				_subscriberOwnerObjects = null;
				_subscriberVariables = null;
				selectedSubscriberVariable = -1;
			}
			
			return false;
		}
		
		/// <summary>Checks if <paramref name="tType"/> contains a Subscriber variable (including non-public)</summary>
		/// <param name="tType">Type to check</param>
		/// <returns>True if the <paramref name="tType"/>'s definition contains a Subscriber field</returns>
		protected virtual bool validateSubscriber( Type tType )
		{
			if ( tType != null )
			{
				FieldInfo[] tempFields = tType.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
				for ( int i = ( tempFields.Length - 1 ); i >= 0; --i )
				{
					if ( !tempFields[i].IsInitOnly && !tempFields[i].IsLiteral && tempFields[i].FieldType == typeof( Subscriber ) )
					{				
						return true;
					}
				}
			}
			
			return false;
		}
		
		//=======================
		// Subscriber Variable
		//=======================
		/// <summary>Caches a readable drop-down list of the selected owner's Subscriber variables, and determines the currently selected variable</summary>
		/// <param name="tVariable">Target Subscriber variable name to generate cached data from</param>
		/// <returns>True if successfully cached</returns>
		public virtual bool cacheSubscriber( string tVariable )
		{
			if ( selectedSubscriberOwner > 0 && _subscriberOwnerObjects != null && selectedSubscriberOwner < _subscriberOwnerObjects.Length )
			{
				selectedSubscriberVariable = -1;
				bool tempIsFound = String.IsNullOrEmpty( tVariable );
				List<string> tempNames = null;
				
				// Fields
				FieldInfo[] tempFields = _subscriberOwnerObjects[ selectedSubscriberOwner ].GetType().GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
				int tempListLength = tempFields.Length;
				for ( int i = 0; i < tempListLength; ++i )
				{
					if ( !tempFields[i].IsInitOnly && !tempFields[i].IsLiteral && tempFields[i].FieldType == typeof( Subscriber ) )
					{
						if ( !tempIsFound && tempFields[i].Name == tVariable )
						{
							selectedSubscriberVariable = tempNames.Count;
						}
						
						if ( tempNames == null )
						{
							tempNames = new List<string>();
						}
						tempNames.Add( EditorUtility.GetDisplayName( tempFields[i].FieldType, tempFields[i].Name ) );
					}
				}
				
				// Apply
				if ( tempNames.Count > 0 )
				{
					_subscriberVariables = tempNames.ToArray();
					if ( selectedSubscriberVariable == -1 )
					{
						selectedSubscriberVariable = 0;
					}
					
					return true;
				}

				_subscriberVariables = null;
			}
			
			return false;
		}
	}
}