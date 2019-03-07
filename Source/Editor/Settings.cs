using UnityEngine;
using System;
using System.Collections.Generic;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Singleton for managing options and member filters</summary>
	public sealed class Settings : ScriptableObject
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Singleton instance</summary>
		private static Settings _instance;
		/// <summary>Will expose private members in the inspector drop-downs if true</summary>
		public bool isPrivateDisplayed = false;
		/// <summary><see cref="Filter"/>s for both blacklisting (hiding) and whitelisting (override blacklisted members)</summary>
		[SerializeField]
		private Filter[] _filters = new Filter[]
		{
			new Filter( "System", "Object", null, new string[]
				{
					"8:GetType"
				}
			),
			new Filter( "UnityEngine", "Object", null, new string[]
				{
					"8:GetInstanceID",
					"8:ToString",
					"8:Equals,System.Object", 
					"8:GetHashCode"
				}
			),
			new Filter( "UnityEngine", "Component", null, new string[]
				{
					"8:CompareTag,System.String",
					"8:GetComponents,System.Type",
					"8:GetComponents,System.Type,System.Collections.Generic.List`1[UnityEngine.Component]",
					"8:GetComponentsInParent,System.Type,System.Boolean",
					"8:GetComponentsInParent,System.Type",
					"8:GetComponentInParent,System.Type",
					"8:GetComponentsInChildren,System.Type,System.Boolean",
					"8:GetComponentsInChildren,System.Type",
					"8:GetComponentInChildren,System.Type",
					"8:GetComponentInChildren,System.Type,System.Boolean",
					"8:GetComponent,System.String",
					"8:GetComponent,System.Type" 
				} 
			),
			new Filter( "UnityEngine", "MonoBehaviour", null, new string[]
				{
					"8:StopCoroutine,UnityEngine.Coroutine",
					"8:StopCoroutine,System.Collections.IEnumerator",
					"8:StartCoroutine_Auto,System.Collections.IEnumerator",
					"8:StartCoroutine,System.Collections.IEnumerator",
					"8:IsInvoking",
					"8:IsInvoking,System.String"
				}
			),
			new Filter( "UnityEngine", "GameObject", null, new string[]
				{
					"8:AddComponent,System.Type",
					"8:CompareTag,System.String",
					"8:GetComponentsInParent,System.Type,System.Boolean",
					"8:GetComponentsInParent,System.Type",
					"8:GetComponentsInChildren,System.Type,System.Boolean",
					"8:GetComponentsInChildren,System.Type",
					"8:GetComponents,System.Type,System.Collections.Generic.List`1[UnityEngine.Component]",
					"8:GetComponents,System.Type",
					"8:GetComponentInParent,System.Type",
					"8:GetComponentInChildren,System.Type",
					"8:GetComponentInChildren,System.Type,System.Boolean",
					"8:GetComponent,System.String",
					"8:GetComponent,System.Type"
				}
			)
		};
		/// <summary>Cached form of the <see cref="_filters"/> used for optimization</summary>
		private Dictionary<string,HashedFilter> _hashedFilters;
		
		//=======================
		// Singleton
		//=======================
		/// <summary>Gets the <see cref="_instance"/> of the settings; will lazy-instantiate one if not already set</summary>
		public static Settings instance
		{
			get
			{
				if ( _instance == null )
				{
					_instance = Resources.Load<Settings>( "Settings" ) as Settings;
					if ( _instance == null )
					{
						Debug.LogWarning( "No \"Settings\" ScriptableObject found in a \"Resources\" folder! Creating new instance!" );
						
						_instance = ScriptableObject.CreateInstance<Settings>();
					}
					
					_instance.initialize();
				}
				
				return _instance;
			}
		}
		
		//=======================
		// Initialize
		//=======================
		/// <summary>Initializes the <see cref="_hashedFilters"/></summary>
		internal void initialize()
		{
			filters = _filters;
		}
		
		//=======================
		// Validation
		//=======================
		/// <summary>Will force a refresh of the <see cref="_hashedFilters"/> if anything changed in the inspector</summary>
		public void OnValidate()
		{
			filters = _filters;
		}
		
		//=======================
		// Filters
		//=======================
		/// <summary>Gets/Sets the <see cref="_filters"/>; if set, caches them into <see cref="_hashedFilters"/></summary>
		public Filter[] filters
		{
			get
			{
				return _filters;
			}
			set
			{
				_filters = value;
				if ( _filters != null )
				{
					int tempListLength = _filters.Length;
					if ( tempListLength == 0 )
					{
						_hashedFilters = null;
					}
					else
					{
						Filter tempFilter;
						_hashedFilters = new Dictionary<string,HashedFilter>();
						for ( var i = ( tempListLength - 1 ); i >= 0; --i )
						{
							tempFilter = _filters[i];
							_hashedFilters[ tempFilter.typeNamespace + "." + tempFilter.typeClass ] = new HashedFilter( tempFilter );
						}
					}
				}
			}
		}
		
		/// <summary>Gets the cached <see cref="_hashedFilters"/></summary>
		public Dictionary<string,HashedFilter> hashedFilters
		{
			get
			{
				return _hashedFilters;
			}
		}
		
		/// <summary>Checks if a member name is blacklisted (hidden in the inspector) by the <paramref name="tDeclaringType"/> filter, or if it is shown (overrides the blacklist) by the <paramref name="tReflectedType"/> filter</summary>
		/// <param name="tDeclaringType">Declared type (class where the member originates) of the <paramref name="tMember"/></param>
		/// <param name="tReflectedType">Reflected type (class where the member is overidden) of the <paramref name="tMember"/></param>
		/// <param name="tMember">Member to check</param>
		/// <returns>True if the member is filtered</returns>
		public bool isMemberFiltered( Type tDeclaringType, Type tReflectedType, IMember tMember )
		{
			if ( _hashedFilters != null )
			{
				HashedFilter tempFilter;
				if ( _hashedFilters.TryGetValue( tDeclaringType.ToString(), out tempFilter ) && tempFilter.isBlackListed( tMember ) )
				{
					if ( ( tDeclaringType == tReflectedType || _hashedFilters.TryGetValue( tReflectedType.ToString(), out tempFilter ) ) && tempFilter.isWhiteListed( tMember ) )
					{
						return false;
					}
					
					return true;
				}
			}
			
			return false;
		}
	}
}