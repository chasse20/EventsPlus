using System;
using System.Collections.Generic;

namespace EventsPlus
{
	//##########################
	// Interface Declaration
	//##########################
	public interface IRequest
	{
		void clear();
		void clearTags();
		void clearPublishers();
		ISubscriber subscriber { get; }
		List<int> tagHashes { get; set; }
		List<string> tags { set; }
		bool hasTag( int tTag );
		bool addTag( int tTag );
		bool removeTag( int tTag );
		bool validatePublisher( IPublisher tPublisher );
		bool addPublisher( IPublisher tPublisher );
		bool removePublisher( IPublisher tPublisher );
		ISubscription createSubscription( IPublisher tPublisher );
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IRequestOut
	{
		Action action { get; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IRequestOut<A>
	{
		Action<A> action { get; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IRequestOut<A,B>
	{
		Action<A,B> action { get; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IRequestOut<A,B,C>
	{
		Action<A,B,C> action { get; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IRequestOut<A,B,C,D>
	{
		Action<A,B,C,D> action { get; }
	}
	
	//##########################
	// Interface Declaration
	//#####################IRequestOut#####
	public interface IRequestOut<A,B,C,D,E>
	{
		Action<A,B,C,D,E> action { get; }
	}
	
	//##########################
	// Interface Declaration
	//##########################
	public interface IRequestOut<A,B,C,D,E,F>
	{
		Action<A,B,C,D,E,F> action { get; }
	}
}