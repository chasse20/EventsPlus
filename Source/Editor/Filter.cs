using System;
using UnityEngine;

namespace EventsPlus
{
	//##########################
	// Struct Declaration
	//##########################
	/// <summary>Editor container used by the <see cref="Settings"/> inspector to handle member filtering</summary>
	[Serializable]
	public struct Filter
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Namespace of the filtered member's class</summary>
		[SerializeField]
		private string _typeNamespace;
		/// <summary>Class name of the filtered member</summary>
		[SerializeField]
		private string _typeClass;
		/// <summary>Array of serialized member names for overriding the <see cref="_blackList"/> members</summary>
		[SerializeField]
		private string[] _whiteList;
		/// <summary>Array of members that will be hidden in the inspector</summary>
		[SerializeField]
		private string[] _blackList;
		
		//=======================
		// Constructor
		//=======================
		/// <summary>Constructor</summary>
		/// <param name="tNamespace">Owning namespace name of the filtered member(s) class</param>
		/// <param name="tClass">Owning class name of the filtered member(s)</param>
		/// <param name="tWhiteList">Whitelist array of member names to override the blacklist</param>
		/// <param name="tBlackList">Blacklist array of member names that will be hidden in the inspector</param>
		public Filter( string tNamespace, string tClass, string[] tWhiteList = null, string[] tBlackList = null )
		{
			_typeNamespace = tNamespace;
			_typeClass = tClass;
			_whiteList = tWhiteList;
			_blackList = tBlackList;
		}
		
		//=======================
		// Accessors
		//=======================
		/// <summary>Gets the <see cref="_typeNamespace"/></summary>
		public string typeNamespace
		{
			get
			{
				return _typeNamespace;
			}
		}
		
		/// <summary>Gets the <see cref="_typeClass"/></summary>
		public string typeClass
		{
			get
			{
				return _typeClass;
			}
		}
		
		/// <summary>Gets the <see cref="_whiteList"/></summary>
		public string[] whiteList
		{
			get
			{
				return _whiteList;
			}
		}
		
		/// <summary>Gets the <see cref="_blackList"/></summary>
		public string[] blackList
		{
			get
			{
				return _blackList;
			}
		}
	}
}