using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EventsPlus
{	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Editor container used to store cached data, in order to improve performance, for both the RawRequest and RawSubscription elements inside of the Subscriber and Publisher drawers respectively</summary>
	public class Cache
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Display names of all the delegate target objects, used by a drop-down</summary>
		protected string[] _targetNames;
		/// <summary>Corresponding object references for each of the <see cref="_targetNames"/></summary>
		protected UnityEngine.Object[] _targetObjects;
		/// <summary>Current index for the active drop-down target</summary>
		public int selectedTarget = -1;
		/// <summary>Display names of all the delegate target methods, used by a drop-down</summary>
		protected string[] _methodDisplayNames;
		/// <summary>Encoded, serialized method data that corresponds to each of the <see cref="_methodDisplayNames"/></summary>
		protected string[] _methodReflectionNames;
		/// <summary>Current index for the active drop-down method</summary>
		public int selectedMethod = -1;
		
		//=======================
		// Constructor
		//=======================
		public Cache()
		{
		}
		
		public Cache( UnityEngine.Object tTarget, string tMethod )
		{
			if ( cache( tTarget ) )
			{
				cache( tMethod );
			}
		}
		
		//=======================
		// Destructor
		//=======================
		~Cache()
		{
			clear();
		}
		
		/// <summary>Clears the target and method data from memory</summary>
		public virtual void clear()
		{
			// Targets
			if ( _targetNames != null )
			{
				Array.Clear( _targetNames, 0, _targetNames.Length );
				_targetNames = null;
			}
			if ( _targetObjects != null )
			{
				Array.Clear( _targetObjects, 0, _targetObjects.Length );
				_targetObjects = null;
			}
			selectedTarget = -1;
			
			// Methods
			if ( _methodReflectionNames != null )
			{
				Array.Clear( _methodReflectionNames, 0, _methodReflectionNames.Length );
				_methodReflectionNames = null;
			}
			if ( _methodDisplayNames != null )
			{
				Array.Clear( _methodDisplayNames, 0, _methodDisplayNames.Length );
				_methodDisplayNames = null;
			}
			selectedMethod = -1;
		}
		
		//=======================
		// Accessors
		//=======================
		public virtual string[] targetNames
		{
			get
			{
				return _targetNames;
			}
		}
		
		public virtual UnityEngine.Object[] targetObjects
		{
			get
			{
				return _targetObjects;
			}
		}
		
		public virtual string[] methodReflectionNames
		{
			get
			{
				return _methodReflectionNames;
			}
		}
		
		public virtual string[] methodDisplayNames
		{
			get
			{
				return _methodDisplayNames;
			}
		}
		
		//=======================
		// Target
		//=======================
		/// <summary>Caches a readable drop-down list of the possible target objects, determines the currently selected target, and adjusts associated method data accordingly</summary>
		/// <param name="tTarget">Target object to generate cached data from</param>
		/// <returns>True if successfully cached</returns>
		public virtual bool cache( UnityEngine.Object tTarget )
		{
			if ( tTarget != null )
			{
				// None option
				List<string> tempNames = new List<string>();
				List<UnityEngine.Object> tempObjects = new List<UnityEngine.Object>();
				tempNames.Add( "None" );
				tempObjects.Add( null );
				
				// Generic option
				selectedTarget = 1;
				GameObject tempGameObject = tTarget as GameObject;
				Component tempComponent = tTarget as Component;
				if ( tempGameObject == null && tempComponent == null )
				{
					ScriptableObject tempScriptableObject = tTarget as ScriptableObject;
					if ( tempScriptableObject == null )
					{					
						tempNames.Add( tTarget.name );
					}
					else
					{
						tempNames.Add( "ScriptableObject: " + tTarget.ToString() );
					}
					
					tempObjects.Add( tTarget );
				}
				else
				{
					// GameObject option
					if ( tempGameObject == null )
					{
						tempGameObject = tempComponent.gameObject;
					}
					tempNames.Add( tempGameObject.name );
					tempObjects.Add( tempGameObject );
					
					// Component options
					bool tempIsFound = tempComponent == null;
					string tempPrefix = tempGameObject.name + ".";
					Component[] tempComponents = tempGameObject.GetComponents<Component>();
					int tempListLength = tempComponents.Length;
					for ( int i = 0; i < tempListLength; ++i )
					{
						if ( !tempIsFound && tempComponents[i] == tempComponent )
						{
							selectedTarget = tempNames.Count;
							tempIsFound = true;
						}
						
						tempNames.Add( tempPrefix + tempComponents[i].GetType().Name );
						tempObjects.Add( tempComponents[i] );
					}
				}
				
				_targetNames = tempNames.ToArray();
				_targetObjects = tempObjects.ToArray();
			
				return true;
			}
			
			_targetNames = null;
			_targetObjects = null;
			selectedTarget = -1;
			_methodReflectionNames = null;
			_methodDisplayNames = null;
			selectedMethod = -1;
			
			return false;
		}
		
		//=======================
		// Method
		//=======================
		/// <summary>Caches a readable drop-down list of the selected target's methods, and determines the currently selected method</summary>
		/// <param name="tMethod">Target method name to generate cached data from</param>
		/// <returns>True if successfully cached</returns>
		public virtual bool cache( string tMethod )
		{
			if ( selectedTarget > 0 && _targetObjects != null && selectedTarget < _targetObjects.Length )
			{				
				selectedMethod = -1;
				List<string> tempReflectionNames = null;
				List<string> tempDisplayNames = null;
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
					cache( tempFields[i], ref tempReflectionNames, ref tempDisplayNames, tMethod, ref selectedMethod );
				}
				
				// Properties
				PropertyInfo[] tempProperties = tempType.GetProperties( tempFlags );
				tempListLength = tempProperties.Length;
				for ( int i = 0; i < tempListLength; ++i )
				{
					cache( tempProperties[i], ref tempReflectionNames, ref tempDisplayNames, tMethod, ref selectedMethod );
				}
				
				// Methods
				MethodInfo[] tempMethodInfos = tempType.GetMethods( tempFlags );
				tempListLength = tempMethodInfos.Length;
				for ( int i = 0; i < tempListLength; ++i )
				{
					cache( tempMethodInfos[i], ref tempReflectionNames, ref tempDisplayNames, tMethod, ref selectedMethod );
				}
				
				// Apply
				if ( tempReflectionNames.Count > 0 )
				{
					_methodReflectionNames = tempReflectionNames.ToArray();
					_methodDisplayNames = tempDisplayNames.ToArray();
					
					if ( selectedMethod == -1 )
					{
						selectedMethod = 0;
					}
					
					return true;
				}
			}
			
			_methodReflectionNames = null;
			_methodDisplayNames = null;
			
			return false;
		}
		
		/// <summary>Caches variable data for the "method" drop-down, and determines the selected index</summary>
		/// <param name="tField">FieldInfo associated with the variable to extract information from</param>
		/// <param name="tReflectionNames">Reference to an array that stores formatted reflection data for each cached element</param>
		/// <param name="tDisplayNames">Reference to an array that stores display names for each cached element</param>
		/// <param name="tSelectedName">Cached name of the currently selected "method" name, used to check if this variable is selected</param>
		/// <param name="tSelectedIndex">Reference of the currently selected "method", will be set if <paramref name="tSelectedName"/> matches the variable name</param>
		/// <returns>True if successfully cached</returns>
		protected virtual bool cache( FieldInfo tField, ref List<string> tReflectionNames, ref List<string> tDisplayNames, string tSelectedName, ref int tSelectedIndex )
		{
			if ( !tField.IsInitOnly && !tField.IsLiteral )
			{
				string tempReflectionName = Utility.WriteReflectionData( InfoType.Field, tField.Name );
				if ( !Settings.instance.isMethodFiltered( tField.DeclaringType, tField.ReflectedType, tempReflectionName ) ) // check if not filtered
				{
					// Names
					if ( tReflectionNames == null )
					{
						tReflectionNames = new List<string>();
						tDisplayNames = new List<string>();
					}
					
					tReflectionNames.Add( tempReflectionName );
					tDisplayNames.Add( EditorUtility.GetDisplayName( tField.FieldType, tField.Name ) );
					
					// Selected
					if ( tSelectedIndex < 0 && tReflectionNames[ tReflectionNames.Count - 1 ] == tSelectedName )
					{
						tSelectedIndex = tReflectionNames.Count - 1;
					}
					
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Caches property data for the "method" drop-down, and determines the selected index</summary>
		/// <param name="tProperty">PropertyInfo associated with the property to extract information from</param>
		/// <param name="tReflectionNames">Reference to an array that stores formatted reflection data for each cached element</param>
		/// <param name="tDisplayNames">Reference to an array that stores display names for each cached element</param>
		/// <param name="tSelectedName">Cached name of the currently selected "method" name, used to check if this property is selected</param>
		/// <param name="tSelectedIndex">Reference of the currently selected "method", will be set if <paramref name="tSelectedName"/> matches the property name</param>
		/// <returns>True if successfully cached</returns>
		protected virtual bool cache( PropertyInfo tProperty, ref List<string> tReflectionNames, ref List<string> tDisplayNames, string tSelectedName, ref int tSelectedIndex )
		{
			if ( tProperty.CanWrite )
			{
				string tempReflectionName = Utility.WriteReflectionData( InfoType.Property, tProperty.Name );
				if ( !Settings.instance.isMethodFiltered( tProperty.DeclaringType, tProperty.ReflectedType, tempReflectionName ) ) // check if not filtered
				{
					// Names
					if ( tReflectionNames == null )
					{
						tReflectionNames = new List<string>();
						tDisplayNames = new List<string>();
					}
					
					tReflectionNames.Add( tempReflectionName );
					tDisplayNames.Add( EditorUtility.GetDisplayName( tProperty.PropertyType, tProperty.Name, null, true ) );
					
					// Selected
					if ( tSelectedIndex < 0 && tReflectionNames[ tReflectionNames.Count - 1 ] == tSelectedName )
					{
						tSelectedIndex = tReflectionNames.Count - 1;
					}
					
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Caches method data for the "method" drop-down, and determines the selected index</summary>
		/// <param name="tMethod">MethodInfo associated with the method to extract information from</param>
		/// <param name="tReflectionNames">Reference to an array that stores formatted reflection data for each cached element</param>
		/// <param name="tDisplayNames">Reference to an array that stores display names for each cached element</param>
		/// <param name="tSelectedName">Cached name of the currently selected "method" name, used to check if this method is selected</param>
		/// <param name="tSelectedIndex">Reference of the currently selected "method", will be set if <paramref name="tSelectedName"/> matches the method name</param>
		/// <returns>True if successfully cached</returns>
		protected virtual bool cache( MethodInfo tMethod, ref List<string> tReflectionNames, ref List<string> tDisplayNames, string tSelectedName, ref int tSelectedIndex )
		{
			if ( !tMethod.IsSpecialName && !tMethod.IsGenericMethod )
			{
				// Names
				ParameterInfo[] tempParameters = tMethod.GetParameters();
				Type[] tempParameterTypes = null;
				if ( tempParameters != null && tempParameters.Length > 0 )
				{
					tempParameterTypes = new Type[ tempParameters.Length ];
					for ( int i = ( tempParameterTypes.Length - 1 ); i >= 0; --i )
					{
						tempParameterTypes[i] = tempParameters[i].ParameterType;
					}
				}
				
				string tempReflectionName = Utility.WriteReflectionData( InfoType.Method, tMethod.Name, tempParameterTypes );
				if ( !Settings.instance.isMethodFiltered( tMethod.DeclaringType, tMethod.ReflectedType, tempReflectionName ) ) // check if not filtered
				{
					if ( tReflectionNames == null )
					{
						tReflectionNames = new List<string>();
						tDisplayNames = new List<string>();
					}
					
					tReflectionNames.Add( tempReflectionName );
					tDisplayNames.Add( EditorUtility.GetDisplayName( tMethod.ReturnType, tMethod.Name, tempParameterTypes ) );
					
					// Selected
					if ( tSelectedIndex < 0 && tReflectionNames[ tReflectionNames.Count - 1 ] == tSelectedName )
					{
						tSelectedIndex = tReflectionNames.Count - 1;
					}
					
					return true;
				}
			}
			
			return false;
		}
	}
}