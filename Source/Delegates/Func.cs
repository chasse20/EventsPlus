using System;

namespace EventsPlus
{
	//##########################
	// Delegate Declaration
	//##########################
	/// <summary>5-Parameter Func for NET 3.5 and under</summary>
	public delegate T Func<A,B,C,D,E,T>( A tA, B tB, C tC, D tD, E tE );
	/// <summary>6-Parameter Func for NET 3.5 and under</summary>
	public delegate T Func<A,B,C,D,E,F,T>( A tA, B tB, C tC, D tD, E tE, F tF );
	/// <summary>7-Parameter Func for NET 3.5 and under</summary>
	public delegate T Func<A,B,C,D,E,F,G,T>( A tA, B tB, C tC, D tD, E tE, F tF, G tG );
	/// <summary>8-Parameter Func for NET 3.5 and under</summary>
	public delegate T Func<A,B,C,D,E,F,G,H,T>( A tA, B tB, C tC, D tD, E tE, F tF, G tG, H tH );
	/// <summary>9-Parameter Func for NET 3.5 and under</summary>
	public delegate T Func<A,B,C,D,E,F,G,H,I,T>( A tA, B tB, C tC, D tD, E tE, F tF, G tG, H tH, I tI );
	/// <summary>10-Parameter Func for NET 3.5 and under</summary>
	public delegate T Func<A,B,C,D,E,F,G,H,I,J,T>( A tA, B tB, C tC, D tD, E tE, F tF, G tG, H tH, I tI, J tJ );
}