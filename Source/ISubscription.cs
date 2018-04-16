using System;
using System.Collections.Generic;

namespace EventsPlus
{
	//##########################
	// Interface Declaration
	//##########################
	public interface ISubscription
	{
		ISubscriber subscriber { get; }
		IPublisher publisher { get; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface ISubscriptionOut
	{
		Action action { get; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface ISubscriptionOut<A>
	{
		Action<A> action { get; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface ISubscriptionOut<A,B>
	{
		Action<A,B> action { get; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface ISubscriptionOut<A,B,C>
	{
		Action<A,B,C> action { get; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface ISubscriptionOut<A,B,C,D>
	{
		Action<A,B,C,D> action { get; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface ISubscriptionOut<A,B,C,D,E>
	{
		Action<A,B,C,D,E> action { get; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface ISubscriptionOut<A,B,C,D,E,F>
	{
		Action<A,B,C,D,E,F> action { get; }
	}
}