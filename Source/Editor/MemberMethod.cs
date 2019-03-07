using System;
using System.Text;
using System.Reflection;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Utility class for a cached <see cref="System.Reflection.MethodInfo"/></summary>
	public sealed class MemberMethod : Member<MethodInfo>
	{
		//=======================
		// Constructor
		//=======================
		public MemberMethod( MethodInfo tInfo ) : base( tInfo )
		{
		}
		
		//=======================
		// Accessors
		//=======================
		public override string displayName
		{
			get
			{
				StringBuilder tempName = new StringBuilder( _info.ReturnType.GetKeyword() );
				tempName.Append( " " );
				tempName.Append( _info.Name );
				
				ParameterInfo[] tempParameters = _info.GetParameters();
				if ( tempParameters != null )
				{
					tempName.Append( "(" );
					int tempListLength = tempParameters.Length;
					for ( int i = 0; i < tempListLength; ++i )
					{
						tempName.Append( " " );
						tempName.Append( tempParameters[i].ParameterType.GetKeyword() );
						
						if ( i < ( tempListLength - 1 ) )
						{
							tempName.Append( "," );
						}
					}
					tempName.Append( " )" );
				}
				
				return tempName.ToString();
			}
		}
	}
}