using System;
using System.Reflection;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Utility class for a cached <see cref="System.Reflection.PropertyInfo"/></summary>
	public sealed class MemberProperty : Member<PropertyInfo>
	{		
		//=======================
		// Constructor
		//=======================
		public MemberProperty( PropertyInfo tInfo ) : base( tInfo )
		{
		}
		
		//=======================
		// Accessors
		//=======================
		public override string displayName
		{
			get
			{
				return "set " + _info.PropertyType.GetKeyword() + " " + _info.Name;
			}
		}
	}
}