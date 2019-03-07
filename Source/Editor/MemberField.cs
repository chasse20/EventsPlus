using System;
using System.Reflection;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Utility class for a cached <see cref="System.Reflection.FieldInfo"/></summary>
	public sealed class MemberField : Member<FieldInfo>
	{		
		//=======================
		// Constructor
		//=======================
		public MemberField( FieldInfo tInfo ) : base( tInfo )
		{
		}
		
		//=======================
		// Accessors
		//=======================
		public override string displayName
		{
			get
			{
				return _info.FieldType.GetKeyword() + " " + _info.Name;
			}
		}
	}
}