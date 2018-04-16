using System;
using System.Collections.Generic;

namespace EventsPlus
{
	//##########################
	// Interface Declaration
	//##########################
	public interface IPublisher
	{
		void initialize();
		void clear();
		Type[] typeArray { get; }
		int tagHash { get; }
		string tag { get; set; }
		void publish();
		event Action onVoid;
		bool addDelegate( System.Delegate tDelegate );
		bool removeDelegate( System.Delegate tDelegate );
		List<ISubscription> subscriptions { get; }
		bool hasSubscription( ISubscription tSubscription );
		bool validateSubscription( ISubscription tSubscription, bool tIsSubscriberChecked = true );
		bool addSubscription( ISubscription tSubscription, bool tIsPreValidated = false );
		bool removeSubscription( ISubscription tSubscription );
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IPublisher<T> : IPublisher where T : IRawSubscription
	{
		List<T> rawSubscriptions { set; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IPublisherOut<A>
	{
		event Action<A> onPublish;
		void publish( A tA );
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IPublisherOut<A,B>
	{
		event Action<A,B> onPublish;
		void publish( A tA, B tB );
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IPublisherOut<A,B,C>
	{
		event Action<A,B,C> onPublish;
		void publish( A tA, B tB, C tC );
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IPublisherOut<A,B,C,D>
	{
		event Action<A,B,C,D> onPublish;
		void publish( A tA, B tB, C tC, D tD );
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IPublisherOut<A,B,C,D,E>
	{
		event Action<A,B,C,D,E> onPublish;
		void publish( A tA, B tB, C tC, D tD, E tE );
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IPublisherOut<A,B,C,D,E,F>
	{
		event Action<A,B,C,D,E,F> onPublish;
		void publish( A tA, B tB, C tC, D tD, E tE, F tF );
	}
}