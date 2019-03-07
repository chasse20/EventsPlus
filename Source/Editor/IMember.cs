using System;
using System.Reflection;

namespace EventsPlus
{
	//##########################
	// Interface Declaration
	//##########################
	public interface IMember
	{
		MemberInfo info { get; }
		string serializedName { get; }
		string displayName { get; }
	}
}