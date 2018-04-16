using System;
using System.Collections.Generic;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>0-Parameter Request</summary>
	public class Request : RequestBase, IRequestOut
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action _action;
		
		//=======================
		// Constructor
		//=======================
		public Request( Subscriber tSubscriber, List<string> tTags, Action tAction ) : base( tSubscriber )
		{
			_action = tAction;
			tags = tTags;
		}
		
		//=======================
		// Destructor
		//=======================
		~Request()
		{
			_action = null;
		}
		
		//=======================
		// Accessors
		//=======================
		public virtual Action action
		{
			get
			{
				return _action;
			}
		}
		
		//=======================
		// Subscription
		//=======================
		public override ISubscription createSubscription( IPublisher tPublisher )
		{
			if ( tPublisher != null && _subscriber != null && _action != null )
			{
				return new Subscription( _subscriber, tPublisher, _action );
			}
			
			return null;
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>1-Parameter Request</summary>
	public class Request<A> : RequestBase, IRequestOut<A>
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action<A> _action;
		
		//=======================
		// Constructor
		//=======================
		public Request( Subscriber tSubscriber, List<string> tTags, Action<A> tAction ) : base( tSubscriber )
		{
			_action = tAction;
			tags = tTags;
		}
		
		//=======================
		// Destructor
		//=======================
		~Request()
		{
			_action = null;
		}
		
		//=======================
		// Accessors
		//=======================
		public virtual Action<A> action
		{
			get
			{
				return _action;
			}
		}
		
		//=======================
		// Publishers
		//=======================
		public override bool validatePublisher( IPublisher tPublisher )
		{
			return base.validatePublisher( tPublisher ) && tPublisher is IPublisherOut<A>;
		}
		
		//=======================
		// Subscription
		//=======================
		public override ISubscription createSubscription( IPublisher tPublisher )
		{
			if ( tPublisher != null && _subscriber != null && _action != null )
			{
				return new Subscription<A>( _subscriber, tPublisher, _action );
			}
			
			return null;
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>2-Parameter Request</summary>
	public class Request<A,B> : RequestBase, IRequestOut<A,B>
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action<A,B> _action;
		
		//=======================
		// Constructor
		//=======================
		public Request( Subscriber tSubscriber, List<string> tTags, Action<A,B> tAction ) : base( tSubscriber )
		{
			_action = tAction;
			tags = tTags;
		}
		
		//=======================
		// Destructor
		//=======================
		~Request()
		{
			_action = null;
		}
		
		//=======================
		// Accessors
		//=======================
		public virtual Action<A,B> action
		{
			get
			{
				return _action;
			}
		}
		
		//=======================
		// Publishers
		//=======================
		public override bool validatePublisher( IPublisher tPublisher )
		{
			return base.validatePublisher( tPublisher ) && tPublisher is IPublisherOut<A,B>;
		}
		
		//=======================
		// Subscription
		//=======================
		public override ISubscription createSubscription( IPublisher tPublisher )
		{
			if ( tPublisher != null && _subscriber != null && _action != null )
			{
				return new Subscription<A,B>( _subscriber, tPublisher, _action );
			}
			
			return null;
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>3-Parameter Request</summary>
	public class Request<A,B,C> : RequestBase, IRequestOut<A,B,C>
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action<A,B,C> _action;
		
		//=======================
		// Constructor
		//=======================
		public Request( Subscriber tSubscriber, List<string> tTags, Action<A,B,C> tAction ) : base( tSubscriber )
		{
			_action = tAction;
			tags = tTags;
		}
		
		//=======================
		// Destructor
		//=======================
		~Request()
		{
			_action = null;
		}
		
		//=======================
		// Accessors
		//=======================
		public virtual Action<A,B,C> action
		{
			get
			{
				return _action;
			}
		}
		
		//=======================
		// Publishers
		//=======================
		public override bool validatePublisher( IPublisher tPublisher )
		{
			return base.validatePublisher( tPublisher ) && tPublisher is IPublisherOut<A,B,C>;
		}
		
		//=======================
		// Subscription
		//=======================
		public override ISubscription createSubscription( IPublisher tPublisher )
		{
			if ( tPublisher != null && _subscriber != null && _action != null )
			{
				return new Subscription<A,B,C>( _subscriber, tPublisher, _action );
			}
			
			return null;
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>4-Parameter Request</summary>
	public class Request<A,B,C,D> : RequestBase, IRequestOut<A,B,C,D>
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action<A,B,C,D> _action;
		
		//=======================
		// Constructor
		//=======================
		public Request( Subscriber tSubscriber, List<string> tTags, Action<A,B,C,D> tAction ) : base( tSubscriber )
		{
			_action = tAction;
			tags = tTags;
		}
		
		//=======================
		// Destructor
		//=======================
		~Request()
		{
			_action = null;
		}
		
		//=======================
		// Accessors
		//=======================
		public virtual Action<A,B,C,D> action
		{
			get
			{
				return _action;
			}
		}
		
		//=======================
		// Publishers
		//=======================
		public override bool validatePublisher( IPublisher tPublisher )
		{
			return base.validatePublisher( tPublisher ) && tPublisher is IPublisherOut<A,B,C,D>;
		}
		
		//=======================
		// Subscription
		//=======================
		public override ISubscription createSubscription( IPublisher tPublisher )
		{
			if ( tPublisher != null && _subscriber != null && _action != null )
			{
				return new Subscription<A,B,C,D>( _subscriber, tPublisher, _action );
			}
			
			return null;
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>5-Parameter Request</summary>
	public class Request<A,B,C,D,E> : RequestBase, IRequestOut<A,B,C,D,E>
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action<A,B,C,D,E> _action;
		
		//=======================
		// Constructor
		//=======================
		public Request( Subscriber tSubscriber, List<string> tTags, Action<A,B,C,D,E> tAction ) : base( tSubscriber )
		{
			_action = tAction;
			tags = tTags;
		}
		
		//=======================
		// Destructor
		//=======================
		~Request()
		{
			_action = null;
		}
		
		//=======================
		// Accessors
		//=======================
		public virtual Action<A,B,C,D,E> action
		{
			get
			{
				return _action;
			}
		}
		
		//=======================
		// Publishers
		//=======================
		public override bool validatePublisher( IPublisher tPublisher )
		{
			return base.validatePublisher( tPublisher ) && tPublisher is IPublisherOut<A,B,C,D,E>;
		}
		
		//=======================
		// Subscription
		//=======================
		public override ISubscription createSubscription( IPublisher tPublisher )
		{
			if ( tPublisher != null && _subscriber != null && _action != null )
			{
				return new Subscription<A,B,C,D,E>( _subscriber, tPublisher, _action );
			}
			
			return null;
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>6-Parameter Request</summary>
	public class Request<A,B,C,D,E,F> : RequestBase, IRequestOut<A,B,C,D,E,F>
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action<A,B,C,D,E,F> _action;
		
		//=======================
		// Constructor
		//=======================
		public Request( Subscriber tSubscriber, List<string> tTags, Action<A,B,C,D,E,F> tAction ) : base( tSubscriber )
		{
			_action = tAction;
			tags = tTags;
		}
		
		//=======================
		// Destructor
		//=======================
		~Request()
		{
			_action = null;
		}
		
		//=======================
		// Accessors
		//=======================
		public virtual Action<A,B,C,D,E,F> action
		{
			get
			{
				return _action;
			}
		}
		
		//=======================
		// Publishers
		//=======================
		public override bool validatePublisher( IPublisher tPublisher )
		{
			return base.validatePublisher( tPublisher ) && tPublisher is IPublisherOut<A,B,C,D,E,F>;
		}
		
		//=======================
		// Subscription
		//=======================
		public override ISubscription createSubscription( IPublisher tPublisher )
		{
			if ( tPublisher != null && _subscriber != null && _action != null )
			{
				return new Subscription<A,B,C,D,E,F>( _subscriber, tPublisher, _action );
			}
			
			return null;
		}
	}
}