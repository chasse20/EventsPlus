using System;

namespace EventsPlus
{	
	//##########################
	// Struct Declaration
	//##########################
	/// <summary>Editor container used to store a parameter's name and type string</summary>
	public struct CachedParameter
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Readable name of the parameter</summary>
		public string name;
		/// <summary>Parameter type in string format</summary>
		public string typeName;
		
		//=======================
		// Constructor
		//=======================
		public CachedParameter( string tName, string tTypeName )
		{
			name = tName;
			typeName = tTypeName;
		}
	}
}