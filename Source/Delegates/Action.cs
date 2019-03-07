using System;

namespace EventsPlus
{
	//##########################
	// Delegate Declaration
	//##########################
	/// <summary>5-Parameter Action for NET 3.5 and under</summary>
	public delegate void Action<A,B,C,D,E>( A tA, B tB, C tC, D tD, E tE );
	/// <summary>6-Parameter Action for NET 3.5 and under</summary>
	public delegate void Action<A,B,C,D,E,F>( A tA, B tB, C tC, D tD, E tE, F tF );
	/// <summary>7-Parameter Action for NET 3.5 and under</summary>
	public delegate void Action<A,B,C,D,E,F,G>( A tA, B tB, C tC, D tD, E tE, F tF, G tG );
	/// <summary>8-Parameter Action for NET 3.5 and under</summary>
	public delegate void Action<A,B,C,D,E,F,G,H>( A tA, B tB, C tC, D tD, E tE, F tF, G tG, H tH );
	/// <summary>9-Parameter Action for NET 3.5 and under</summary>
	public delegate void Action<A,B,C,D,E,F,G,H,I>( A tA, B tB, C tC, D tD, E tE, F tF, G tG, H tH, I tI );
	/// <summary>10-Parameter Action for NET 3.5 and under</summary>
	public delegate void Action<A,B,C,D,E,F,G,H,I,J>( A tA, B tB, C tC, D tD, E tE, F tF, G tG, H tH, I tI, J tJ );
}