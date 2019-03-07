using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Stores cached data for call delegate drop-downs; used by <see cref="DrawerPublisher"/></summary>
	public class CacheRawCall : CacheRawDelegate
	{
		//=======================
		// Variables
		//=======================
		/// <summary><see cref="Publisher"/> type definition used to determine if a delegate is treated as a direct invocation rather than having predefined arguments</summary>
		protected Type[] _dynamicTypes;
		/// <summary>True if the selected member can be invoked without predefined arguments</summary>
		protected bool _isDynamicable;
		/// <summary>True if the selected member is to be invoked without predefined arguments</summary>
		protected bool _isDynamic;
		/// <summary>Cached predefined arguments of the selected member</summary>
		protected Argument[] _arguments;
		
		//=======================
		// Constructor
		//=======================
		/// <summary>Constructor</summary>
		/// <param name="tDynamicTypes">Type definition used to check if this call is <see cref="_isDynamicable"/></param>
		public CacheRawCall( Type[] tDynamicTypes )
		{
			_dynamicTypes = tDynamicTypes;
		}
		
		//=======================
		// Target
		//=======================
		/// <summary>Checks for discrepancies between the <see cref="UnityEditor.SerializedProperty"/>s and the cached data; tries to match the cache to the properties</summary>
		/// <param name="tTarget">Selected target property</param>
		/// <param name="tMember">Selected member property</param>
		/// <param name="tDynamic">Dynamic toggle property</param>
		/// <returns>True if the data matches</returns>
		public virtual bool validateTarget( SerializedProperty tTarget, SerializedProperty tMember, SerializedProperty tDynamic )
		{
			if ( !base.validateTarget( tTarget, tMember ) )
			{
				isDynamic = tDynamic.boolValue;
				
				return false;
			}
			
			return true;
		}

		//=======================
		// Member
		//=======================
		/// <summary>Gets/Sets the <see cref="CacheRawDelegate._selectedMember"/>; if set, will determine if the call <see cref="_isDynamicable"/></summary>
		public override int selectedMember
		{
			get
			{
				return _selectedMember;
			}
			set
			{
				_selectedMember = value < 0 ? 0 : value;
				
				// Generate arguments and determine if can be dynamic
				_arguments = null;
				_isDynamicable = false;
				
				if ( _members != null )
				{
					IMember tempMember = _members[ _selectedMember ];
					switch ( tempMember.info.MemberType )
					{
						case MemberTypes.Field:
							_arguments = new Argument[] { new Argument( tempMember.info as FieldInfo ) };
							_isDynamicable = _dynamicTypes != null && _dynamicTypes.Length == 1 && _dynamicTypes[0] == _arguments[0].type;
							break;
						case MemberTypes.Property:
							_arguments = new Argument[] { new Argument( tempMember.info as PropertyInfo ) };
							_isDynamicable = _dynamicTypes != null && _dynamicTypes.Length == 1 && _dynamicTypes[0] == _arguments[0].type;
							break;
						case MemberTypes.Method:
							ParameterInfo[] tempParameters = ( tempMember.info as MethodInfo ).GetParameters();
							if ( tempParameters != null )
							{
								int tempListLength = tempParameters.Length;
								_arguments = new Argument[ tempListLength ];
								_isDynamicable = _dynamicTypes != null && tempListLength == _dynamicTypes.Length; // methods without arguments are not dynamic
								
								for ( int i = ( tempListLength - 1 ); i >= 0; --i )
								{
									_arguments[i] = new Argument( tempParameters[i] );
									if ( _isDynamicable && _arguments[i].type != _dynamicTypes[i] )
									{
										_isDynamicable = false;
									}
								}
							}
							break;
						default:
							break;
					}
				}
				
				// Turn off dynamic if not able
				if ( !_isDynamicable && _isDynamic )
				{
					isDynamic = false;
				}
			}
		}
		
		/// <summary>Checks for discrepancies between the <see cref="UnityEditor.SerializedProperty"/> and the cached data; tries to match the cache to the property</summary>
		/// <param name="tMember">Selected member property</param>
		/// <param name="tDynamic">Dynamic toggle property</param>
		/// <returns>True if the data matches</returns>
		public virtual bool validateMember( SerializedProperty tMember, SerializedProperty tDynamic )
		{
			if ( !base.validateMember( tMember ) )
			{
				isDynamic = tDynamic.boolValue;
				
				return false;
			}
			
			return true;
		}
		
		//=======================
		// Dynamic
		//=======================
		/// <summary>Gets the <see cref="_dynamicTypes"/></summary>
		public Type[] dynamicTypes
		{
			get
			{
				return _dynamicTypes;
			}
		}
		
		/// <summary>Gets the <see cref="_isDynamicable"/> toggle</summary>
		public bool isDynamicable
		{
			get
			{
				return _isDynamicable;
			}
		}
		
		/// <summary>Gets/Sets the <see cref="_isDynamic"/> toggle</summary>
		public virtual bool isDynamic
		{
			get
			{
				return _isDynamic;
			}
			set
			{
				bool tempIsDynamic = value && _isDynamicable;
				if ( tempIsDynamic != _isDynamic )
				{
					_isDynamic = tempIsDynamic;
				}
			}
		}
		
		//=======================
		// Arguments
		//=======================
		/// <summary>Gets the predefined <see cref="_arguments"/> of the selected member</summary>
		public Argument[] arguments
		{
			get
			{
				return _arguments;
			}
		}
	}
}