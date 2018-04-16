using System;
using System.Collections.Generic;

namespace EventsPlus
{
	//##########################
	// Interface Declaration
	//##########################
	public interface IRawSubscription
	{
		UnityEngine.Object target { get; }
		string targetMethod { get; }
		UnityEngine.Object subscriberOwner { get; }
		string subscriberVariable { get; }
		bool isDynamic { get; }
		ISubscriber subscriber { get; }
		bool createAndRegister( IPublisher tPublisher );
		ISubscription createSubscription( IPublisher tPublisher, ISubscriber tSubscriber );
		System.Delegate createDelegate();
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IRawSubscription<T> : IRawSubscription where T : RawArgument
	{
		T[] arguments { get; }
	}
}