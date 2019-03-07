using System;
using System.Reflection;

namespace EventsPlus
{
	//##########################
	// Struct Declaration
	//##########################
	/// <summary>Utility container for cached method argument data</summary>
	public struct Argument
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Parameter name/label</summary>
		private string _name;
		/// <summary>Argument type</summary>
		private Type _type;
		
		//=======================
		// Constructor
		//=======================
		/// <summary>Constructor</summary>
		/// <param name="tName">Name of the parameter</param>
		/// <param name="tType">Type of argument</param>
		public Argument( string tName, Type tType )
		{
			_name = tName;
			_type = tType;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="tField"><see cref="System.Reflection.FieldInfo"/> to instantiate the argument information</param>
		public Argument( FieldInfo tField )
		{
			_name = tField.Name;
			_type = tField.FieldType;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="tProperty"><see cref="System.Reflection.PropertyInfo"/> to instantiate the argument information</param>
		public Argument( PropertyInfo tProperty )
		{
			_name = tProperty.Name;
			_type = tProperty.PropertyType;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="tParameter"><see cref="System.Reflection.ParameterInfo"/> to instantiate the argument information</param>
		public Argument( ParameterInfo tParameter )
		{
			_name = tParameter.Name;
			_type = tParameter.ParameterType;
		}
		
		//=======================
		// Accessors
		//=======================
		/// <summary>Gets the <see cref="_name"/> of the argument</summary>
		public string name
		{
			get
			{
				return _name;
			}
		}
		
		/// <summary>Gets the <see cref="_type"/> of the argument</summary>
		public Type type
		{
			get
			{
				return _type;
			}
		}
	}
}