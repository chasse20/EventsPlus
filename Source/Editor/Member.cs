using System;
using System.Reflection;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Abstract utility class for cached member information</summary>
	public abstract class Member<T> : IMember where T : MemberInfo
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Reflection info</summary>
		protected T _info;
		/// <summary>Cached serialized name of the member</summary>
		protected string _serializedName;
		
		//=======================
		// Constructor
		//=======================
		/// <summary>Constructor</summary>
		/// <param name="tInfo">Reflection info</param>
		public Member( T tInfo )
		{
			_info = tInfo;
			_serializedName = Utility.Serialize( _info );
		}
		
		//=======================
		// Accessors
		//=======================
		/// <summary>Gets the reflection <see cref="_info"/></summary>
		public MemberInfo info
		{
			get
			{
				return _info;
			}
		}
		
		/// <summary>Gets the <see cref="_serializedName"/> of the member</summary>
		public string serializedName
		{
			get
			{
				return _serializedName;
			}
		}
		
		/// <summary>Gets the display name of the member for use in drop-downs</summary>
		public abstract string displayName
		{
			get;
		}
	}
}