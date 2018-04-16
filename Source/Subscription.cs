using System;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>0-Parameter Subscription</summary>
	public class Subscription : SubscriptionBase, ISubscriptionOut
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action _action;
		
		//=======================
		// Constructor
		//=======================
		public Subscription( ISubscriber tSubscriber, IPublisher tPublisher, Action tAction ) : base( tSubscriber, tPublisher )
		{
			_action = tAction;
		}
		
		//=======================
		// Destructor
		//=======================
		~Subscription()
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
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>1-Parameter Subscription</summary>
	public class Subscription<A> : SubscriptionBase, ISubscriptionOut<A>
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action<A> _action;
		
		//=======================
		// Constructor
		//=======================
		public Subscription( ISubscriber tSubscriber, IPublisher tPublisher, Action<A> tAction ) : base( tSubscriber, tPublisher )
		{
			_action = tAction;
		}
		
		//=======================
		// Destructor
		//=======================
		~Subscription()
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
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>2-Parameter Subscription</summary>
	public class Subscription<A,B> : SubscriptionBase, ISubscriptionOut<A,B>
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action<A,B> _action;
		
		//=======================
		// Constructor
		//=======================
		public Subscription( ISubscriber tSubscriber, IPublisher tPublisher, Action<A,B> tAction ) : base( tSubscriber, tPublisher )
		{
			_action = tAction;
		}
		
		//=======================
		// Destructor
		//=======================
		~Subscription()
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
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>3-Parameter Subscription</summary>
	public class Subscription<A,B,C> : SubscriptionBase, ISubscriptionOut<A,B,C>
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action<A,B,C> _action;
		
		//=======================
		// Constructor
		//=======================
		public Subscription( ISubscriber tSubscriber, IPublisher tPublisher, Action<A,B,C> tAction ) : base( tSubscriber, tPublisher )
		{
			_action = tAction;
		}
		
		//=======================
		// Destructor
		//=======================
		~Subscription()
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
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>4-Parameter Subscription</summary>
	public class Subscription<A,B,C,D> : SubscriptionBase, ISubscriptionOut<A,B,C,D>
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action<A,B,C,D> _action;
		
		//=======================
		// Constructor
		//=======================
		public Subscription( ISubscriber tSubscriber, IPublisher tPublisher, Action<A,B,C,D> tAction ) : base( tSubscriber, tPublisher )
		{
			_action = tAction;
		}
		
		//=======================
		// Destructor
		//=======================
		~Subscription()
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
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>5-Parameter Subscription</summary>
	public class Subscription<A,B,C,D,E> : SubscriptionBase, ISubscriptionOut<A,B,C,D,E>
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action<A,B,C,D,E> _action;
		
		//=======================
		// Constructor
		//=======================
		public Subscription( ISubscriber tSubscriber, IPublisher tPublisher, Action<A,B,C,D,E> tAction ) : base( tSubscriber, tPublisher )
		{
			_action = tAction;
		}
		
		//=======================
		// Destructor
		//=======================
		~Subscription()
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
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>6-Parameter Subscription</summary>
	public class Subscription<A,B,C,D,E,F> : SubscriptionBase, ISubscriptionOut<A,B,C,D,E,F>
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable, actual delegate</summary>
		protected Action<A,B,C,D,E,F> _action;
		
		//=======================
		// Constructor
		//=======================
		public Subscription( ISubscriber tSubscriber, IPublisher tPublisher, Action<A,B,C,D,E,F> tAction ) : base( tSubscriber, tPublisher )
		{
			_action = tAction;
		}
		
		//=======================
		// Destructor
		//=======================
		~Subscription()
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
	}
}