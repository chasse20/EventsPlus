using System;
using System.Collections.Generic;

namespace EventsPlus
{
	//##########################
	// Interface Declaration
	//##########################
	public interface ISubscriber
	{
		void clear();
		void clearSubscriptions();
		void clearRequests();
		List<ISubscription> subscriptions { get; }
		bool hasSubscription( ISubscription tSubscription );
		bool validateSubscription( ISubscription tSubscription, bool tIsPublisherChecked = true );
		bool addSubscription( ISubscription tSubscription, bool tIsPreValidated = false );
		bool removeSubscription( ISubscription tSubscription );
		List<IRequest> requests { get; }
		bool hasRequest( IRequest tRequest );
		bool addRequest( IRequest tRequest );
		bool removeRequest( IRequest tRequest );
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface ISubscriber<T> : ISubscriber where T : IRawRequest
	{
		List<T> rawRequests { set; }
	}
}