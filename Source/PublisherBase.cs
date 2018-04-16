using UnityEngine;
using System;
using System.Collections.Generic;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Manages and invokes Subscriptions as well as predefined calls</summary>
	public abstract class PublisherBase : IPublisher
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Immutable tag hash that is used by the Manager to automatically match Requests</summary>
		[SerializeField]
		protected string _tag;
		/// <summary>Immutable tag hash set by <see cref="tag"/></summary>
		protected int _tagHash;
		/// <summary>Managed Subscriptions</summary>
		protected HashSet<ISubscription> _subscriptions;
		/// <summary>Actual delegate for void methods and predefined calls</summary>
		protected Action _onVoid;
		
		//=======================
		// Constructor
		//=======================
		public PublisherBase()
		{
		}
		
		public PublisherBase( string tTag )
		{
			_tag = tTag;
		}
		
		/// <summary>Initializes and registers with the Manager</summary>
		public virtual void initialize()
		{
			tag = _tag;
		}
		
		//=======================
		// Destructor
		//=======================
		~PublisherBase()
		{
			clear();
		}
		
		/// <summary>Unregisters from Manager, clears and unsubscribes all Subscriptions from memory</summary>
		public virtual void clear()
		{
			// Unregister from Manager
			Manager.RemovePublisher( this );
			_tag = null;
		
			// Clear Subscriptions
			if ( _subscriptions != null )
			{
				List<ISubscription> tempSubscriptions = new List<ISubscription>( _subscriptions );
				for ( int i = ( tempSubscriptions.Count - 1 ); i >= 0; --i )
				{
					removeSubscription( tempSubscriptions[i] );
				}
				
				if ( _subscriptions != null )
				{
					_subscriptions.Clear();
					_subscriptions = null;
				}
			}
			
			// Clear void methods and calls from the event
			_onVoid = null;
		}
		
		//=======================
		// Type
		//=======================
		/// <summary>Gets array of Types that define this instance, returns null if not generic</summary>
		public virtual Type[] typeArray
		{
			get
			{
				return null;
			}
		}
		
		//=======================
		// Tag
		//=======================
		public virtual int tagHash
		{
			get
			{
				return _tagHash;
			}
		}
		
		/// <summary>Gets tag name, converts tag into hash and registers it to the manager</summary>
		public virtual string tag
		{
			get
			{
				return _tag;
			}
			set
			{
				if ( _tag != null )
				{
					Manager.RemovePublisher( this );
				}
				
				_tag = value;
				_tagHash = Animator.StringToHash( _tag );
				
				if ( _tag != null )
				{
					Manager.AddPublisher( this );
				}
			}
		}
		
		//=======================
		// Event
		//=======================
		public virtual event Action onVoid
		{
			add
			{
				_onVoid += value;
			}
			remove
			{
				_onVoid -= value;
			}
		}
		
		/// <summary>Invokes the <see cref="_onVoid"/> event</summary>
		public virtual void publish()
		{
			/*
				Thread safe version:
				
					// Event
					Action tempEvent = _onVoid;
					if ( tempEvent != null )
					{
						tempEvent();
					}
				
				Note: Unity is single-threaded by default...
			*/
			
			if ( _onVoid != null )
			{
				_onVoid();
			}
		}
		
		/// <summary>Attempts to register a delegate directly if it is valid</summary>
		/// <param name="tDelegate">Delegate to be added</param>
		/// <returns>True if <paramref name="tDelegate"/> is successfully added</returns>
		public virtual bool addDelegate( System.Delegate tDelegate )
		{
			Action tempDelegate = tDelegate as Action;
			if ( tempDelegate != null )
			{
				_onVoid += tempDelegate;
				return true;
			}
			
			return false;
		}
		
		/// <summary>Attempts to unregister a delegate directly if it is valid</summary>
		/// <param name="tDelegate">Delegate to be removed</param>
		/// <returns>True if <paramref name="tDelegate"/> is successfully removed</returns>
		public virtual bool removeDelegate( System.Delegate tDelegate )
		{
			Action tempDelegate = tDelegate as Action;
			if ( tempDelegate != null )
			{
				_onVoid -= tempDelegate;
				return true;
			}
			
			return false;
		}
		
		//=======================
		// Subscriptions
		//=======================
		public virtual List<ISubscription> subscriptions
		{
			get
			{
				return _subscriptions == null ? null : new List<ISubscription>( _subscriptions );
			}
		}
		
		/// <summary>Checks if this already contains a managed Subscription</summary>
		/// <param name="tSubscription">Subscription to check</param>
		/// <returns>True if <paramref name="tSubscription"/> exists</returns>
		public virtual bool hasSubscription( ISubscription tSubscription )
		{
			return tSubscription != null && _subscriptions != null && _subscriptions.Contains( tSubscription );
		}
		
		/// <summary>Checks to see if a Subscription is valid in relationship to this Publisher and its Subscriber</summary>
		/// <param name="tSubscription">Subscription to check</param>
		/// <param name="tIsSubscriberChecked">If true, also checks if valid by its Subscriber</param>
		/// <returns>True if <paramref name="tSubscription"/> is valid</returns>
		public virtual bool validateSubscription( ISubscription tSubscription, bool tIsSubscriberChecked = true )
		{
			if ( tSubscription != null && tSubscription.publisher == this )
			{
				if ( tIsSubscriberChecked )
				{
					ISubscriber tempSubscriber = tSubscription.subscriber;
					return tempSubscriber != null && tempSubscriber.validateSubscription( tSubscription, false );
				}
			
				return true;
			}
			
			return false;
		}
		
		/// <summary>Adds a Subscription and registers it to the internal event if it passes validation</summary>
		/// <param name="tSubscription">Subscription to be added</param>
		/// <param name="tIsPreValidated">Skips the validation if true, assuming it was already validated</param>
		/// <returns>True if <paramref name="tSubscription"/> doesn't already exist and successfully added</returns>
		public virtual bool addSubscription( ISubscription tSubscription, bool tIsPreValidated = false )
		{
			if ( tIsPreValidated || validateSubscription( tSubscription ) )
			{
				if ( _subscriptions == null )
				{
					_subscriptions = new HashSet<ISubscription>();
				}
				
				if ( _subscriptions.Add( tSubscription ) )
				{
					// Ensure subscribed to Subscriber
					ISubscriber tempSubscriber = tSubscription.subscriber;
					if ( tempSubscriber != null )
					{
						tempSubscriber.addSubscription( tSubscription, true );
					}
					
					// Register event if void type
					ISubscriptionOut tempSubscription = tSubscription as ISubscriptionOut;
					if ( tempSubscription != null )
					{
						_onVoid += tempSubscription.action;
					}
					
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Removes a Subscription and unregisters it to the internal event</summary>
		/// <param name="tSubscription">Subscription to be removed</param>
		/// <returns>True if <paramref name="tSubscription"/> exists and successfully removed</returns>
		public virtual bool removeSubscription( ISubscription tSubscription )
		{
			if ( tSubscription != null && _subscriptions != null && _subscriptions.Remove( tSubscription ) )
			{
				// Ensure unsubscribed from Subscriber
				ISubscriber tempSubscriber = tSubscription.subscriber;
				if ( tempSubscriber != null )
				{
					tempSubscriber.removeSubscription( tSubscription );
				}
				
				// Unregister event if void type
				ISubscriptionOut tempSubscription = tSubscription as ISubscriptionOut;
				if ( tempSubscription != null )
				{
					_onVoid -= tempSubscription.action;
				}
					
				if ( _subscriptions.Count == 0 )
				{
					_subscriptions = null;
				}
			
				return true;
			}
		
			return false;
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Publisher type that allows for exposed RawSubscriptions in the inspector</summary>
	public class PublisherBase<T> : PublisherBase, IPublisher<T> where T : IRawSubscription
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Raw data of Subscriptions exposed in the inspector</summary>
		[SerializeField]
		protected List<T> _rawSubscriptions;
		
		//=======================
		// Constructor
		//=======================
		public PublisherBase()
		{
		}
		
		public PublisherBase( string tTag ) : base( tTag )
		{
		}
		
		/// <summary>Converts and registers <see cref="_rawSubscriptions"/> into actual Subscriptions or delegate calls</summary>
		public override void initialize()
		{
			// Create actual Subscriptions/Calls from raw
			if ( _rawSubscriptions != null )
			{
				rawSubscriptions = _rawSubscriptions;
				
				// Keep raws around for debugging if in editor
				#if !UNITY_EDITOR
					_rawSubscriptions.Clear();
					_rawSubscriptions = null;
				#endif
			}
			
			// Inheritance
			base.initialize();
		}
		
		//=======================
		// Subscriptions
		//=======================
		/// <summary>Converts and registers the input array into actual Subscriptions or delegate calls</summary>
		public virtual List<T> rawSubscriptions
		{
			set
			{
				if ( value != null )
				{
					for ( int i = ( value.Count - 1 ); i >= 0; --i )
					{
						value[i].createAndRegister( this );
					}
				}
			}
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>1-Parameter Publisher</summary>
	public class PublisherBase<T,A> : PublisherBase<T>, IPublisherOut<A> where T : IRawSubscription
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Actual delegate</summary>
		protected Action<A> _onPublish;
		
		//=======================
		// Constructor
		//=======================
		public PublisherBase()
		{
		}
		
		public PublisherBase( string tTag ) : base( tTag )
		{
		}
		
		//=======================
		// Destructor
		//=======================
		~PublisherBase()
		{
			_onPublish = null;
		}
		
		//=======================
		// Type
		//=======================
		public override Type[] typeArray
		{
			get
			{
				return new Type[] { typeof( A ) };
			}
		}
		
		//=======================
		// Event
		//=======================
		public virtual event Action<A> onPublish
		{
			add
			{
				_onPublish += value;
			}
			remove
			{
				_onPublish -= value;
			}
		}
		
		/// <summary>Invokes the <see cref="_onPublish"/> and <see cref="PublisherBase._onVoid"/> events using a single argument</summary>
		/// <param name="tA">Generic argument</param>
		public virtual void publish( A tA )
		{
			/*
				Thread safe version:
				
					// Event
					Action<A> tempEvent = _onPublish;
					if ( tempEvent != null )
					{
						tempEvent( tA );
					}
					
					// Void (squeezes performance by not inheriting)
					Action tempVoid = _onVoid;
					if ( tempVoid != null )
					{
						tempVoid();
					}
				
				Note: Unity is single-threaded by default...
			*/
			
			// Event
			if ( _onPublish != null )
			{
				_onPublish( tA );
			}
			
			// Void (squeezes performance by not inheriting)
			if ( _onVoid != null )
			{
				_onVoid();
			}
		}
		
		public override bool addDelegate( System.Delegate tDelegate )
		{
			// Delegate
			Action<A> tempDelegate = tDelegate as Action<A>;
			if ( tempDelegate != null )
			{
				_onPublish += tempDelegate;
				return true;
			}
			
			// Inheritance
			return base.addDelegate( tDelegate );
		}
		
		public override bool removeDelegate( System.Delegate tDelegate )
		{
			// Event
			Action<A> tempDelegate = tDelegate as Action<A>;
			if ( tempDelegate != null )
			{
				_onPublish -= tempDelegate;
				return true;
			}
			
			// Inheritance
			return base.removeDelegate( tDelegate );
		}

		//=======================
		// Subscriptions
		//=======================		
		public override bool validateSubscription( ISubscription tSubscription, bool tIsSubscriberChecked = true )
		{
			return base.validateSubscription( tSubscription, tIsSubscriberChecked ) && ( tSubscription is ISubscriptionOut || tSubscription is ISubscriptionOut<A> );
		}
		
		public override bool addSubscription( ISubscription tSubscription, bool tIsPreValidated = false )
		{
			if ( base.addSubscription( tSubscription, tIsPreValidated ) )
			{
				// Register event
				ISubscriptionOut<A> tempSubscription = tSubscription as ISubscriptionOut<A>;
				if ( tempSubscription != null )
				{
					_onPublish += tempSubscription.action;
				}
				
				return true;
			}
			
			return false;
		}
		
		public override bool removeSubscription( ISubscription tSubscription )
		{
			if ( base.removeSubscription( tSubscription ) )
			{
				// Unregister event
				ISubscriptionOut<A> tempSubscription = tSubscription as ISubscriptionOut<A>;
				if ( tempSubscription != null )
				{
					_onPublish -= tempSubscription.action;
				}

				return true;
			}
		
			return false;
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>2-Parameter Publisher</summary>
	public class PublisherBase<T,A,B> : PublisherBase<T>, IPublisherOut<A,B> where T : IRawSubscription
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Actual delegate</summary>
		protected Action<A,B> _onPublish;
		
		//=======================
		// Constructor
		//=======================
		public PublisherBase()
		{
		}
		
		public PublisherBase( string tTag ) : base( tTag )
		{
		}
		
		//=======================
		// Destructor
		//=======================
		~PublisherBase()
		{
			_onPublish = null;
		}
		
		//=======================
		// Type
		//=======================
		public override Type[] typeArray
		{
			get
			{
				return new Type[] { typeof( A ), typeof( B ) };
			}
		}
		
		//=======================
		// Event
		//=======================
		public virtual event Action<A,B> onPublish
		{
			add
			{
				_onPublish += value;
			}
			remove
			{
				_onPublish -= value;
			}
		}
		
		/// <summary>Invokes the <see cref="_onPublish"/> and <see cref="PublisherBase._onVoid"/> events using 2 arguments</summary>
		/// <param name="tA">Generic argument</param>
		/// <param name="tB">Generic argument</param>
		public virtual void publish( A tA, B tB )
		{
			/*
				Thread safe version:
				
					// Event
					Action<A,B> tempEvent = _onPublish;
					if ( tempEvent != null )
					{
						tempEvent( tA, tB );
					}
					
					// Void (squeezes performance by not inheriting)
					Action tempVoid = _onVoid;
					if ( tempVoid != null )
					{
						tempVoid();
					}
				
				Note: Unity is single-threaded by default...
			*/
			
			// Event
			if ( _onPublish != null )
			{
				_onPublish( tA, tB );
			}
			
			// Void (squeezes performance by not inheriting)
			if ( _onVoid != null )
			{
				_onVoid();
			}
		}
		
		public override bool addDelegate( System.Delegate tDelegate )
		{
			// Delegate
			Action<A,B> tempDelegate = tDelegate as Action<A,B>;
			if ( tempDelegate != null )
			{
				_onPublish += tempDelegate;
				return true;
			}
			
			// Inheritance
			return base.addDelegate( tDelegate );
		}
		
		public override bool removeDelegate( System.Delegate tDelegate )
		{
			// Event
			Action<A,B> tempDelegate = tDelegate as Action<A,B>;
			if ( tempDelegate != null )
			{
				_onPublish -= tempDelegate;
				return true;
			}
			
			// Inheritance
			return base.removeDelegate( tDelegate );
		}

		//=======================
		// Subscriptions
		//=======================		
		public override bool validateSubscription( ISubscription tSubscription, bool tIsSubscriberChecked = true )
		{
			return base.validateSubscription( tSubscription, tIsSubscriberChecked ) && ( tSubscription is ISubscriptionOut || tSubscription is ISubscriptionOut<A,B> );
		}
		
		public override bool addSubscription( ISubscription tSubscription, bool tIsPreValidated = false )
		{
			if ( base.addSubscription( tSubscription, tIsPreValidated ) )
			{
				// Register event
				ISubscriptionOut<A,B> tempSubscription = tSubscription as ISubscriptionOut<A,B>;
				if ( tempSubscription != null )
				{
					_onPublish += tempSubscription.action;
				}
				
				return true;
			}
			
			return false;
		}
		
		public override bool removeSubscription( ISubscription tSubscription )
		{
			if ( base.removeSubscription( tSubscription ) )
			{
				// Unregister event
				ISubscriptionOut<A,B> tempSubscription = tSubscription as ISubscriptionOut<A,B>;
				if ( tempSubscription != null )
				{
					_onPublish -= tempSubscription.action;
				}

				return true;
			}
		
			return false;
		}
	}
}