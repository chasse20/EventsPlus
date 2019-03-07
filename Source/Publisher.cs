using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Manages delegate event registration and invocation</summary>
	[Serializable]
	public class Publisher
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Global event that gets fired when a Publisher is instantiated</summary>
		public static event Action<Publisher> OnLoaded;
		/// <summary>Immutable tag key that is used to wire up events with a <see cref="Subscriber"/></summary>
		[SerializeField]
		protected string _tag = null;
		/// <summary>List of raw <see cref="RawCall"/> objects that this Publisher invokes using predefined arguments</summary>
		[SerializeField]
		protected List<RawCall> _calls;
		/// <summary>Managed <see cref="RawRequest"/> tracker for handling event wiring a <see cref="Subscriber"/></summary>
		protected Dictionary<Subscriber,List<RawRequest>> requests;
		/// <summary>Event for 0-Parameter delegates and calls</summary>
		public event Action onVoid;
		
		//=======================
		// Constructor
		//=======================
		/// <summary>Initializes the <see cref="_tag"/></summary>
		/// <param name="tTag">Tag key to bind to this instance</param>
		public Publisher( string tTag = null )
		{
			_tag = tTag;
		}
		
		/// <summary>Initializes <see cref="_calls"/> and registers with any potential <see cref="Subscriber"/> instances</summary>
		public virtual void initialize()
		{
			// Initialize and ingest calls
			if ( _calls != null )
			{
				int tempListLength = _calls.Count;
				for ( int i = 0; i < tempListLength; ++i )
				{
					_calls[i].initialize( this );
					effectsCallAdded( _calls[i] );
				}
			}
			
			// Subscribe to Subscriber
			Subscriber.OnLoaded += onSubscriberLoaded;
			Subscriber.OnDestroyed += onSubscriberDestroyed;
			
			// Fire load event
			Action<Publisher> tempEvent = OnLoaded;
			if ( tempEvent != null )
			{
				tempEvent( this );
			}
		}
		
		//=======================
		// Destructor
		//=======================
		/// <summary>Unregisters from <see cref="Subscriber"/> instances and clears memory usage</summary>
		~Publisher()
		{
			// Unsubscribe from Subscriber
			Subscriber.OnLoaded -= onSubscriberLoaded;
			Subscriber.OnDestroyed -= onSubscriberDestroyed;
			
			// Clear memory
			_calls = null;
			requests = null;
			onVoid = null;
		}
		
		//=======================
		// Subscriber
		//=======================
		/// <summary>Callback for when a <see cref="Subscriber"/> is loaded; this will attempt to add all of the Subscriber's event requests</summary>
		/// <param name="tSubscriber">Subscriber that was loaded</param>
		public virtual void onSubscriberLoaded( Subscriber tSubscriber )
		{
			if ( tSubscriber != null )
			{
				RawRequest[] tempRequests = tSubscriber.requests;
				if ( tempRequests != null )
				{
					int tempListLength = tempRequests.Length;
					for ( int i = 0; i < tempListLength; ++i )
					{
						addRequest( tSubscriber, tempRequests[i] );
					}
				}
			}
		}
		
		/// <summary>Callback for when a <see cref="Subscriber"/> is destroyed; this will attempt to remove all of the Subscriber's event requests</summary>
		/// <param name="tSubscriber">Subscriber that was destroyed</param>
		protected virtual void onSubscriberDestroyed( Subscriber tSubscriber )
		{
			if ( requests != null )
			{
				List<RawRequest> tempRequests;
				if ( requests.TryGetValue( tSubscriber, out tempRequests ) )
				{
					for ( int i = ( tempRequests.Count - 1 ); i >= 0; --i )
					{
						removeRequest( tSubscriber, tempRequests[i] );
					}
				}
			}
		}
		
		//=======================
		// Types
		//=======================
		/// <summary>Gets array of Types that define this instance; this is used by the inspector to manage drop-downs</summary>
		public virtual Type[] types
		{
			get
			{
				return null;
			}
		}

		//=======================
		// Tag
		//=======================
		/// <summary>Gets the <see cref="_tag"/> key that is bound to this instance</summary>
		public string tag
		{
			get
			{
				return _tag;
			}
		}
		
		//=======================
		// Calls
		//=======================
		/// <summary>Gets/sets the <see cref="_calls"/> array</summary>
		public List<RawCall> calls
		{
			get
			{
				return _calls;
			}
			set
			{
				// Remove old
				if ( _calls != null )
				{
					for ( int i = ( _calls.Count -1 ); i >= 0; --i )
					{
						removeCall( i );
					}
					
					_calls = null;
				}
				
				// Add new
				if ( value != null )
				{
					int tempListLength = value.Count;
					for ( int i = 0; i < tempListLength; ++i )
					{
						addCall( value[i] );
					}
				}
			}
		}
		
		/// <summary>Attempts to add a <see cref="RawCall"/> to the Publisher's internal array and event(s)</summary>
		/// <param name="tCall">RawCall to add</param>
		/// <returns>True if successful</returns>
		public bool addCall( RawCall tCall )
		{
			if ( tCall != null )
			{
				if ( _calls == null )
				{
					_calls = new List<RawCall>();
				}
				_calls.Add( tCall );
				
				effectsCallAdded( tCall );
				
				return true;
			}
			
			return false;
		}
		
		/// <summary>Handles the <see cref="RawCall"/> that was added and registers its delegate to the Publisher's matching event(s)</summary>
		/// <param name="tCall">RawCall that was added</param>
		protected virtual void effectsCallAdded( RawCall tCall )
		{
			Action tempDelegate = tCall.delegateInstance as Action;
			if ( tempDelegate != null )
			{
				onVoid += tempDelegate;
			}
		}
		
		/// <summary>Attempts to remove a <see cref="RawCall"/> from the Publisher's internal array and event(s)</summary>
		/// <param name="tCall">RawCall to remove</param>
		/// <returns>True if successful</returns>
		public bool removeCall( RawCall tCall )
		{
			return tCall != null && _calls != null && removeCall( _calls.IndexOf( tCall ) );
		}
		
		/// <summary>Attempts to remove a <see cref="RawCall"/> from the Publisher's internal array</summary>
		/// <param name="tIndex">Index of <see cref="_calls"/> array to remove</param>
		/// <returns>True if successful</returns>
		public bool removeCall( int tIndex )
		{
			if ( tIndex >= 0 && _calls != null && tIndex < _calls.Count )
			{
				RawCall tempCall = _calls[ tIndex ];
				
				_calls.RemoveAt( tIndex );
				if ( _calls.Count == 0 )
				{
					_calls = null;
				}
				
				effectsCallRemoved( tempCall, tIndex );
				
				return true;
			}
			
			return false;
		}
		
		/// <summary>Handles the <see cref="RawCall"/> that was removed and removes its delegate from the Publisher's matching event(s)</summary>
		/// <param name="tCall">RawCall that was removed</param>
		/// <param name="tIndex">Index of the RawCall that was removed</param>
		protected virtual void effectsCallRemoved( RawCall tCall, int tIndex )
		{
			Action tempDelegate = tCall.delegateInstance as Action;
			if ( tempDelegate != null )
			{
				onVoid -= tempDelegate;
			}
		}
		
		//=======================
		// Requests
		//=======================
		/// <summary>Attempts to add a <see cref="RawRequest"/> to the Publisher's internal tracker and event(s)</summary>
		/// <param name="tSubscriber">Owning <see cref="Subscriber"/> instance of <paramref name="tRequest"/></param>
		/// <param name="tRequest">RawRequest to add</param>
		/// <returns>True if successful</returns>
		public bool addRequest( Subscriber tSubscriber, RawRequest tRequest )
		{
			if ( tSubscriber != null && tRequest != null && _tag != null && Array.IndexOf( tRequest.tags, _tag ) >= 0 )
			{
				List<RawRequest> tempRequests;
				if ( requests == null )
				{
					requests = new Dictionary<Subscriber,List<RawRequest>>();
					tempRequests = new List<RawRequest>();
					tempRequests.Add( tRequest );
					requests.Add( tSubscriber, tempRequests );
				}
				else if ( requests.TryGetValue( tSubscriber, out tempRequests ) )
				{
					tempRequests.Add( tRequest );
				}
				else
				{
					tempRequests = new List<RawRequest>();
					tempRequests.Add( tRequest );
					requests.Add( tSubscriber, tempRequests );
				}
				
				effectsRequestAdded( tSubscriber, tRequest );
				
				return true;
			}
			
			return false;
		}
		
		/// <summary>Handles the <see cref="RawRequest"/> that was added and registers its delegate to the Publisher's matching event(s)</summary>
		/// <param name="tSubscriber">Owning <see cref="Subscriber"/> instance of <paramref name="tRequest"/></param>
		/// <param name="tRequest">RawRequest that was added</param>
		protected virtual void effectsRequestAdded( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action tempDelegate = tRequest.delegateInstance as Action;
			if ( tempDelegate != null )
			{
				onVoid += tempDelegate;
			}
		}
		
		/// <summary>Attempts to remove a <see cref="RawRequest"/> from the Publisher's internal tracker and event(s)</summary>
		/// <param name="tSubscriber">Owning <see cref="Subscriber"/> instance of <paramref name="tRequest"/></param>
		/// <param name="tRequest">RawRequest to remove</param>
		/// <returns>True if successful</returns>
		public bool removeRequest( Subscriber tSubscriber, RawRequest tRequest )
		{
			if ( tSubscriber != null && tRequest != null && requests != null )
			{
				List<RawRequest> tempRequests;
				if ( requests.TryGetValue( tSubscriber, out tempRequests ) )
				{
					int tempIndex = tempRequests.IndexOf( tRequest );
					if ( tempIndex >= 0 )
					{
						tempRequests.RemoveAt( tempIndex );
						if ( tempRequests.Count == 0 )
						{
							requests.Remove( tSubscriber );
							if ( requests.Count == 0 )
							{
								requests = null;
							}
						}
						
						effectsRequestRemoved( tSubscriber, tRequest );
						
						return true;
					}
				}
			}
			
			return false;
		}
		
		/// <summary>Handles the <see cref="RawRequest"/> that was removed and removes its delegate from the Publisher's matching event(s)</summary>
		/// <param name="tSubscriber">Owning <see cref="Subscriber"/> instance of <paramref name="tRequest"/></param>
		/// <param name="tRequest">RawRequest that was removed</param>
		protected virtual void effectsRequestRemoved( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action tempDelegate = tRequest.delegateInstance as Action;
			if ( tempDelegate != null )
			{
				onVoid -= tempDelegate;
			}
		}
		
		//=======================
		// Publish
		//=======================
		/// <summary>Invokes the <see cref="onVoid"/> event</summary>
		public void publish()
		{
			Action tempVoid = onVoid;
			if ( tempVoid != null )
			{
				tempVoid();
			}
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>1-Parameter Publisher</summary>
	public class Publisher<A> : Publisher
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Event for 1-Parameter delegates</summary>
		public event Action<A> onEvent;
		
		//=======================
		// Constructor
		//=======================
		public Publisher( string tTag = null ) : base ( tTag )
		{
		}
		
		//=======================
		// Destructor
		//=======================
		~Publisher()
		{
			onEvent = null;
		}
		
		//=======================
		// Types
		//=======================
		public override Type[] types
		{
			get
			{
				return new Type[] { typeof( A ) };
			}
		}
		
		//=======================
		// Call
		//=======================
		protected override void effectsCallAdded( RawCall tCall )
		{
			Action<A> tempDelegate = tCall.delegateInstance as Action<A>;
			if ( tempDelegate == null )
			{
				base.effectsCallAdded( tCall );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsCallRemoved( RawCall tCall, int tIndex )
		{
			Action<A> tempDelegate = tCall.delegateInstance as Action<A>;
			if ( tempDelegate == null )
			{
				base.effectsCallRemoved( tCall, tIndex );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}
		
		//=======================
		// Requests
		//=======================
		protected override void effectsRequestAdded( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A> tempDelegate = tRequest.delegateInstance as Action<A>;
			if ( tempDelegate == null )
			{
				base.effectsRequestAdded( tSubscriber, tRequest );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsRequestRemoved( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A> tempDelegate = tRequest.delegateInstance as Action<A>;
			if ( tempDelegate == null )
			{
				base.effectsRequestRemoved( tSubscriber, tRequest );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}

		//=======================
		// Publish
		//=======================
		/// <summary>Invokes the <see cref="Publisher.onVoid"/> and <see cref="onEvent"/> events</summary>
		public void publish( A tA )
		{
			publish();
			
			Action<A> tempEvent = onEvent;
			if ( tempEvent != null )
			{
				tempEvent( tA );
			}
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>2-Parameter Publisher</summary>
	public class Publisher<A,B> : Publisher
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Event for 2-Parameter delegates</summary>
		public event Action<A,B> onEvent;
		
		//=======================
		// Constructor
		//=======================
		public Publisher( string tTag = null ) : base ( tTag )
		{
		}
		
		//=======================
		// Destructor
		//=======================
		~Publisher()
		{
			onEvent = null;
		}
		
		//=======================
		// Types
		//=======================
		public override Type[] types
		{
			get
			{
				return new Type[] { typeof( A ), typeof( B ) };
			}
		}
		
		//=======================
		// Call
		//=======================
		protected override void effectsCallAdded( RawCall tCall )
		{
			Action<A,B> tempDelegate = tCall.delegateInstance as Action<A,B>;
			if ( tempDelegate == null )
			{
				base.effectsCallAdded( tCall );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsCallRemoved( RawCall tCall, int tIndex )
		{
			Action<A,B> tempDelegate = tCall.delegateInstance as Action<A,B>;
			if ( tempDelegate == null )
			{
				base.effectsCallRemoved( tCall, tIndex );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}
		
		//=======================
		// Requests
		//=======================
		protected override void effectsRequestAdded( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B> tempDelegate = tRequest.delegateInstance as Action<A,B>;
			if ( tempDelegate == null )
			{
				base.effectsRequestAdded( tSubscriber, tRequest );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsRequestRemoved( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B> tempDelegate = tRequest.delegateInstance as Action<A,B>;
			if ( tempDelegate == null )
			{
				base.effectsRequestRemoved( tSubscriber, tRequest );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}

		//=======================
		// Publish
		//=======================
		/// <summary>Invokes the <see cref="Publisher.onVoid"/> and <see cref="onEvent"/> events</summary>
		public void publish( A tA, B tB )
		{
			publish();
			
			Action<A,B> tempEvent = onEvent;
			if ( tempEvent != null )
			{
				tempEvent( tA, tB );
			}
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>3-Parameter Publisher</summary>
	public class Publisher<A,B,C> : Publisher
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Event for 3-Parameter delegates</summary>
		public event Action<A,B,C> onEvent;
		
		//=======================
		// Constructor
		//=======================
		public Publisher( string tTag = null ) : base ( tTag )
		{
		}
		
		//=======================
		// Destructor
		//=======================
		~Publisher()
		{
			onEvent = null;
		}
		
		//=======================
		// Types
		//=======================
		public override Type[] types
		{
			get
			{
				return new Type[] { typeof( A ), typeof( B ), typeof( C ) };
			}
		}
		
		//=======================
		// Call
		//=======================
		protected override void effectsCallAdded( RawCall tCall )
		{
			Action<A,B,C> tempDelegate = tCall.delegateInstance as Action<A,B,C>;
			if ( tempDelegate == null )
			{
				base.effectsCallAdded( tCall );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsCallRemoved( RawCall tCall, int tIndex )
		{
			Action<A,B,C> tempDelegate = tCall.delegateInstance as Action<A,B,C>;
			if ( tempDelegate == null )
			{
				base.effectsCallRemoved( tCall, tIndex );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}
		
		//=======================
		// Requests
		//=======================
		protected override void effectsRequestAdded( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C> tempDelegate = tRequest.delegateInstance as Action<A,B,C>;
			if ( tempDelegate == null )
			{
				base.effectsRequestAdded( tSubscriber, tRequest );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsRequestRemoved( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C> tempDelegate = tRequest.delegateInstance as Action<A,B,C>;
			if ( tempDelegate == null )
			{
				base.effectsRequestRemoved( tSubscriber, tRequest );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}

		//=======================
		// Publish
		//=======================
		/// <summary>Invokes the <see cref="Publisher.onVoid"/> and <see cref="onEvent"/> events</summary>
		public void publish( A tA, B tB, C tC )
		{
			publish();
			
			Action<A,B,C> tempEvent = onEvent;
			if ( tempEvent != null )
			{
				tempEvent( tA, tB, tC );
			}
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>4-Parameter Publisher</summary>
	public class Publisher<A,B,C,D> : Publisher
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Event for 4-Parameter delegates</summary>
		public event Action<A,B,C,D> onEvent;
		
		//=======================
		// Constructor
		//=======================
		public Publisher( string tTag = null ) : base ( tTag )
		{
		}
		
		//=======================
		// Destructor
		//=======================
		~Publisher()
		{
			onEvent = null;
		}
		
		//=======================
		// Types
		//=======================
		public override Type[] types
		{
			get
			{
				return new Type[] { typeof( A ), typeof( B ), typeof( C ), typeof( D ) };
			}
		}
		
		//=======================
		// Call
		//=======================
		protected override void effectsCallAdded( RawCall tCall )
		{
			Action<A,B,C,D> tempDelegate = tCall.delegateInstance as Action<A,B,C,D>;
			if ( tempDelegate == null )
			{
				base.effectsCallAdded( tCall );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsCallRemoved( RawCall tCall, int tIndex )
		{
			Action<A,B,C,D> tempDelegate = tCall.delegateInstance as Action<A,B,C,D>;
			if ( tempDelegate == null )
			{
				base.effectsCallRemoved( tCall, tIndex );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}
		
		//=======================
		// Requests
		//=======================
		protected override void effectsRequestAdded( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D>;
			if ( tempDelegate == null )
			{
				base.effectsRequestAdded( tSubscriber, tRequest );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsRequestRemoved( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D>;
			if ( tempDelegate == null )
			{
				base.effectsRequestRemoved( tSubscriber, tRequest );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}

		//=======================
		// Publish
		//=======================
		/// <summary>Invokes the <see cref="Publisher.onVoid"/> and <see cref="onEvent"/> events</summary>
		public void publish( A tA, B tB, C tC, D tD )
		{
			publish();
			
			Action<A,B,C,D> tempEvent = onEvent;
			if ( tempEvent != null )
			{
				tempEvent( tA, tB, tC, tD );
			}
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>5-Parameter Publisher</summary>
	public class Publisher<A,B,C,D,E> : Publisher
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Event for 5-Parameter delegates</summary>
		public event Action<A,B,C,D,E> onEvent;
		
		//=======================
		// Constructor
		//=======================
		public Publisher( string tTag = null ) : base ( tTag )
		{
		}
		
		//=======================
		// Destructor
		//=======================
		~Publisher()
		{
			onEvent = null;
		}
		
		//=======================
		// Types
		//=======================
		public override Type[] types
		{
			get
			{
				return new Type[] { typeof( A ), typeof( B ), typeof( C ), typeof( D ), typeof( E ) };
			}
		}
		
		//=======================
		// Call
		//=======================
		protected override void effectsCallAdded( RawCall tCall )
		{
			Action<A,B,C,D,E> tempDelegate = tCall.delegateInstance as Action<A,B,C,D,E>;
			if ( tempDelegate == null )
			{
				base.effectsCallAdded( tCall );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsCallRemoved( RawCall tCall, int tIndex )
		{
			Action<A,B,C,D,E> tempDelegate = tCall.delegateInstance as Action<A,B,C,D,E>;
			if ( tempDelegate == null )
			{
				base.effectsCallRemoved( tCall, tIndex );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}
		
		//=======================
		// Requests
		//=======================
		protected override void effectsRequestAdded( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D,E> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D,E>;
			if ( tempDelegate == null )
			{
				base.effectsRequestAdded( tSubscriber, tRequest );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsRequestRemoved( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D,E> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D,E>;
			if ( tempDelegate == null )
			{
				base.effectsRequestRemoved( tSubscriber, tRequest );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}

		//=======================
		// Publish
		//=======================
		/// <summary>Invokes the <see cref="Publisher.onVoid"/> and <see cref="onEvent"/> events</summary>
		public void publish( A tA, B tB, C tC, D tD, E tE )
		{
			publish();
			
			Action<A,B,C,D,E> tempEvent = onEvent;
			if ( tempEvent != null )
			{
				tempEvent( tA, tB, tC, tD, tE );
			}
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>6-Parameter Publisher</summary>
	public class Publisher<A,B,C,D,E,F> : Publisher
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Event for 6-Parameter delegates</summary>
		public event Action<A,B,C,D,E,F> onEvent;
		
		//=======================
		// Constructor
		//=======================
		public Publisher( string tTag = null ) : base ( tTag )
		{
		}
		
		//=======================
		// Destructor
		//=======================
		~Publisher()
		{
			onEvent = null;
		}
		
		//=======================
		// Types
		//=======================
		public override Type[] types
		{
			get
			{
				return new Type[] { typeof( A ), typeof( B ), typeof( C ), typeof( D ), typeof( E ), typeof( F ) };
			}
		}
		
		//=======================
		// Call
		//=======================
		protected override void effectsCallAdded( RawCall tCall )
		{
			Action<A,B,C,D,E,F> tempDelegate = tCall.delegateInstance as Action<A,B,C,D,E,F>;
			if ( tempDelegate == null )
			{
				base.effectsCallAdded( tCall );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsCallRemoved( RawCall tCall, int tIndex )
		{
			Action<A,B,C,D,E,F> tempDelegate = tCall.delegateInstance as Action<A,B,C,D,E,F>;
			if ( tempDelegate == null )
			{
				base.effectsCallRemoved( tCall, tIndex );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}
		
		//=======================
		// Requests
		//=======================
		protected override void effectsRequestAdded( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D,E,F> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D,E,F>;
			if ( tempDelegate == null )
			{
				base.effectsRequestAdded( tSubscriber, tRequest );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsRequestRemoved( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D,E,F> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D,E,F>;
			if ( tempDelegate == null )
			{
				base.effectsRequestRemoved( tSubscriber, tRequest );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}

		//=======================
		// Publish
		//=======================
		/// <summary>Invokes the <see cref="Publisher.onVoid"/> and <see cref="onEvent"/> events</summary>
		public void publish( A tA, B tB, C tC, D tD, E tE, F tF )
		{
			publish();
			
			Action<A,B,C,D,E,F> tempEvent = onEvent;
			if ( tempEvent != null )
			{
				tempEvent( tA, tB, tC, tD, tE, tF );
			}
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>7-Parameter Publisher</summary>
	public class Publisher<A,B,C,D,E,F,G> : Publisher
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Event for 7-Parameter delegates</summary>
		public event Action<A,B,C,D,E,F,G> onEvent;
		
		//=======================
		// Constructor
		//=======================
		public Publisher( string tTag = null ) : base ( tTag )
		{
		}
		
		//=======================
		// Destructor
		//=======================
		~Publisher()
		{
			onEvent = null;
		}
		
		//=======================
		// Types
		//=======================
		public override Type[] types
		{
			get
			{
				return new Type[] { typeof( A ), typeof( B ), typeof( C ), typeof( D ), typeof( E ), typeof( F ), typeof( G ) };
			}
		}
		
		//=======================
		// Call
		//=======================
		protected override void effectsCallAdded( RawCall tCall )
		{
			Action<A,B,C,D,E,F,G> tempDelegate = tCall.delegateInstance as Action<A,B,C,D,E,F,G>;
			if ( tempDelegate == null )
			{
				base.effectsCallAdded( tCall );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsCallRemoved( RawCall tCall, int tIndex )
		{
			Action<A,B,C,D,E,F,G> tempDelegate = tCall.delegateInstance as Action<A,B,C,D,E,F,G>;
			if ( tempDelegate == null )
			{
				base.effectsCallRemoved( tCall, tIndex );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}
		
		//=======================
		// Requests
		//=======================
		protected override void effectsRequestAdded( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D,E,F,G> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D,E,F,G>;
			if ( tempDelegate == null )
			{
				base.effectsRequestAdded( tSubscriber, tRequest );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsRequestRemoved( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D,E,F,G> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D,E,F,G>;
			if ( tempDelegate == null )
			{
				base.effectsRequestRemoved( tSubscriber, tRequest );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}

		//=======================
		// Publish
		//=======================
		/// <summary>Invokes the <see cref="Publisher.onVoid"/> and <see cref="onEvent"/> events</summary>
		public void publish( A tA, B tB, C tC, D tD, E tE, F tF, G tG )
		{
			publish();
			
			Action<A,B,C,D,E,F,G> tempEvent = onEvent;
			if ( tempEvent != null )
			{
				tempEvent( tA, tB, tC, tD, tE, tF, tG );
			}
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>8-Parameter Publisher</summary>
	public class Publisher<A,B,C,D,E,F,G,H> : Publisher
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Event for 8-Parameter delegates</summary>
		public event Action<A,B,C,D,E,F,G,H> onEvent;
		
		//=======================
		// Constructor
		//=======================
		public Publisher( string tTag = null ) : base ( tTag )
		{
		}
		
		//=======================
		// Destructor
		//=======================
		~Publisher()
		{
			onEvent = null;
		}
		
		//=======================
		// Types
		//=======================
		public override Type[] types
		{
			get
			{
				return new Type[] { typeof( A ), typeof( B ), typeof( C ), typeof( D ), typeof( E ), typeof( F ), typeof( G ), typeof( H ) };
			}
		}
		
		//=======================
		// Call
		//=======================
		protected override void effectsCallAdded( RawCall tCall )
		{
			Action<A,B,C,D,E,F,G,H> tempDelegate = tCall.delegateInstance as Action<A,B,C,D,E,F,G,H>;
			if ( tempDelegate == null )
			{
				base.effectsCallAdded( tCall );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsCallRemoved( RawCall tCall, int tIndex )
		{
			Action<A,B,C,D,E,F,G,H> tempDelegate = tCall.delegateInstance as Action<A,B,C,D,E,F,G,H>;
			if ( tempDelegate == null )
			{
				base.effectsCallRemoved( tCall, tIndex );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}
		
		//=======================
		// Requests
		//=======================
		protected override void effectsRequestAdded( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D,E,F,G,H> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D,E,F,G,H>;
			if ( tempDelegate == null )
			{
				base.effectsRequestAdded( tSubscriber, tRequest );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsRequestRemoved( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D,E,F,G,H> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D,E,F,G,H>;
			if ( tempDelegate == null )
			{
				base.effectsRequestRemoved( tSubscriber, tRequest );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}

		//=======================
		// Publish
		//=======================
		/// <summary>Invokes the <see cref="Publisher.onVoid"/> and <see cref="onEvent"/> events</summary>
		public void publish( A tA, B tB, C tC, D tD, E tE, F tF, G tG, H tH )
		{
			publish();
			
			Action<A,B,C,D,E,F,G,H> tempEvent = onEvent;
			if ( tempEvent != null )
			{
				tempEvent( tA, tB, tC, tD, tE, tF, tG, tH );
			}
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>9-Parameter Publisher</summary>
	public class Publisher<A,B,C,D,E,F,G,H,I> : Publisher
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Event for 9-Parameter delegates</summary>
		public event Action<A,B,C,D,E,F,G,H,I> onEvent;
		
		//=======================
		// Constructor
		//=======================
		public Publisher( string tTag = null ) : base ( tTag )
		{
		}
		
		//=======================
		// Destructor
		//=======================
		~Publisher()
		{
			onEvent = null;
		}
		
		//=======================
		// Types
		//=======================
		public override Type[] types
		{
			get
			{
				return new Type[] { typeof( A ), typeof( B ), typeof( C ), typeof( D ), typeof( E ), typeof( F ), typeof( G ), typeof( H ), typeof( I ) };
			}
		}
		
		//=======================
		// Call
		//=======================
		protected override void effectsCallAdded( RawCall tCall )
		{
			Action<A,B,C,D,E,F,G,H,I> tempDelegate = tCall.delegateInstance as Action<A,B,C,D,E,F,G,H,I>;
			if ( tempDelegate == null )
			{
				base.effectsCallAdded( tCall );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsCallRemoved( RawCall tCall, int tIndex )
		{
			Action<A,B,C,D,E,F,G,H,I> tempDelegate = tCall.delegateInstance as Action<A,B,C,D,E,F,G,H,I>;
			if ( tempDelegate == null )
			{
				base.effectsCallRemoved( tCall, tIndex );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}
		
		//=======================
		// Requests
		//=======================
		protected override void effectsRequestAdded( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D,E,F,G,H,I> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D,E,F,G,H,I>;
			if ( tempDelegate == null )
			{
				base.effectsRequestAdded( tSubscriber, tRequest );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsRequestRemoved( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D,E,F,G,H,I> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D,E,F,G,H,I>;
			if ( tempDelegate == null )
			{
				base.effectsRequestRemoved( tSubscriber, tRequest );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}

		//=======================
		// Publish
		//=======================
		/// <summary>Invokes the <see cref="Publisher.onVoid"/> and <see cref="onEvent"/> events</summary>
		public void publish( A tA, B tB, C tC, D tD, E tE, F tF, G tG, H tH, I tI )
		{
			publish();
			
			Action<A,B,C,D,E,F,G,H,I> tempEvent = onEvent;
			if ( tempEvent != null )
			{
				tempEvent( tA, tB, tC, tD, tE, tF, tG, tH, tI );
			}
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>10-Parameter Publisher</summary>
	public class Publisher<A,B,C,D,E,F,G,H,I,J> : Publisher
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Event for 10-Parameter delegates</summary>
		public event Action<A,B,C,D,E,F,G,H,I,J> onEvent;
		
		//=======================
		// Constructor
		//=======================
		public Publisher( string tTag = null ) : base ( tTag )
		{
		}
		
		//=======================
		// Destructor
		//=======================
		~Publisher()
		{
			onEvent = null;
		}
		
		//=======================
		// Types
		//=======================
		public override Type[] types
		{
			get
			{
				return new Type[] { typeof( A ), typeof( B ), typeof( C ), typeof( D ), typeof( E ), typeof( F ), typeof( G ), typeof( H ), typeof( I ), typeof( J ) };
			}
		}
		
		//=======================
		// Call
		//=======================
		protected override void effectsCallAdded( RawCall tCall )
		{
			Action<A,B,C,D,E,F,G,H,I,J> tempDelegate = tCall.delegateInstance as Action<A,B,C,D,E,F,G,H,I,J>;
			if ( tempDelegate == null )
			{
				base.effectsCallAdded( tCall );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsCallRemoved( RawCall tCall, int tIndex )
		{
			Action<A,B,C,D,E,F,G,H,I,J> tempDelegate = tCall.delegateInstance as Action<A,B,C,D,E,F,G,H,I,J>;
			if ( tempDelegate == null )
			{
				base.effectsCallRemoved( tCall, tIndex );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}
		
		//=======================
		// Requests
		//=======================
		protected override void effectsRequestAdded( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D,E,F,G,H,I,J> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D,E,F,G,H,I,J>;
			if ( tempDelegate == null )
			{
				base.effectsRequestAdded( tSubscriber, tRequest );
			}
			else
			{
				onEvent += tempDelegate;
			}
		}
		
		protected override void effectsRequestRemoved( Subscriber tSubscriber, RawRequest tRequest )
		{
			Action<A,B,C,D,E,F,G,H,I,J> tempDelegate = tRequest.delegateInstance as Action<A,B,C,D,E,F,G,H,I,J>;
			if ( tempDelegate == null )
			{
				base.effectsRequestRemoved( tSubscriber, tRequest );
			}
			else
			{
				onEvent -= tempDelegate;
			}
		}
		
		//=======================
		// Publish
		//=======================
		/// <summary>Invokes the <see cref="Publisher.onVoid"/> and <see cref="onEvent"/> events</summary>
		public void publish( A tA, B tB, C tC, D tD, E tE, F tF, G tG, H tH, I tI, J tJ )
		{
			publish();
			
			Action<A,B,C,D,E,F,G,H,I,J> tempEvent = onEvent;
			if ( tempEvent != null )
			{
				tempEvent( tA, tB, tC, tD, tE, tF, tG, tH, tI, tJ );
			}
		}
	}
}