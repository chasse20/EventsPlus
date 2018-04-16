using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Manages and listens to Subscriptions and Requests</summary>
	public abstract class SubscriberBase : ISubscriber
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Managed Subscriptions</summary>
		protected HashSet<ISubscription> _subscriptions;
		/// <summary>Managed Requests</summary>
		protected HashSet<IRequest> _requests;
		
		//=======================
		// Destructor
		//=======================
		~SubscriberBase()
		{			
			clear();
		}
		
		/// <summary>Clears all managed Subscriptions and Requests</summary>
		public virtual void clear()
		{
			clearSubscriptions();
			clearRequests();
		}
		
		/// <summary>Clears all managed Subscriptions, along with any fulfilled Publishers from the managed Requests</summary>
		public virtual void clearSubscriptions()
		{
			// Clear subscriptions
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
			
			// Clear fulfilled publishers from Requests
			if ( _requests != null )
			{
				foreach ( IRequest tempRequest in _requests )
				{
					tempRequest.clearPublishers();
				}
			}
		}
		
		/// <summary>Clears all managed Requests</summary>
		public virtual void clearRequests()
		{
			if ( _requests != null )
			{
				List<IRequest> tempRequests = new List<IRequest>( _requests );
				for ( int i = ( tempRequests.Count - 1 ); i >= 0; --i )
				{
					removeRequest( tempRequests[i] );
				}
			
				if ( _requests != null )
				{
					_requests.Clear();
					_requests = null;
				}
			}
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
		
		/// <summary>Checks to see if a Subscription is valid in relationship to this Subscriber and its Publisher</summary>
		/// <param name="tSubscription">Subscription to check</param>
		/// <param name="tIsPublisherChecked">If true, also checks if valid by its Publisher</param>
		/// <returns>True if <paramref name="tSubscription"/> is valid</returns>
		public virtual bool validateSubscription( ISubscription tSubscription, bool tIsPublisherChecked = true )
		{
			if ( tSubscription != null && tSubscription.subscriber == this )
			{
				if ( tIsPublisherChecked )
				{
					IPublisher tempPublisher = tSubscription.publisher;
					return tempPublisher != null && tempPublisher.validateSubscription( tSubscription, false );
				}
				
				return true;
			}
			
			return false;
		}
		
		/// <summary>Adds a Subscription if it passes validation</summary>
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
					IPublisher tempPublisher = tSubscription.publisher;
					if ( tempPublisher != null )
					{
						tempPublisher.addSubscription( tSubscription, true ); // ensure subscribed to Publisher
					}
					
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Removes a Subscription</summary>
		/// <param name="tSubscription">Subscription to be removed</param>
		/// <returns>True if <paramref name="tSubscription"/> exists and successfully removed</returns>
		public virtual bool removeSubscription( ISubscription tSubscription )
		{
			if ( tSubscription != null &&  _subscriptions != null && _subscriptions.Remove( tSubscription ) )
			{
				if ( _subscriptions.Count == 0 )
				{
					_subscriptions = null;
				}
				
				IPublisher tempPublisher = tSubscription.publisher;
				if ( tempPublisher != null )
				{
					tempPublisher.removeSubscription( tSubscription ); // ensure unsubscribed from Publisher
				}
			
				return true;
			}
		
			return false;
		}
		
		//=======================
		// Request
		//=======================
		public virtual List<IRequest> requests
		{
			get
			{
				return _requests == null ? null : new List<IRequest>( _requests );
			}
		}
		
		/// <summary>Checks if this already contains a managed Request</summary>
		/// <param name="tRequest">Request to check</param>
		/// <returns>True if <paramref name="tRequest"/> exists</returns>
		public virtual bool hasRequest( IRequest tRequest )
		{
			return tRequest != null && _requests != null && _requests.Contains( tRequest );
		}
		
		/// <summary>Adds a Request</summary>
		/// <param name="tRequest">Request to be added</param>
		/// <returns>True if <paramref name="tRequest"/> doesn't already exist and successfully added</returns>
		public virtual bool addRequest( IRequest tRequest )
		{
			if ( tRequest != null && tRequest.subscriber == this )
			{			
				if ( _requests == null )
				{
					_requests = new HashSet<IRequest>();
				}

				return _requests.Add( tRequest );
			}
			
			return false;
		}
		
		/// <summary>Removes a Request</summary>
		/// <param name="tRequest">Request to be removed</param>
		/// <returns>True if <paramref name="tRequest"/> exists and successfully removed</returns>
		public virtual bool removeRequest( IRequest tRequest )
		{
			if ( tRequest != null && _requests != null && _requests.Remove( tRequest ) )
			{
				if ( _requests.Count == 0 )
				{
					_requests = null;
				}
				
				return true;
			}
			
			return false;
		}
	}
	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Subscriber type that allows for exposed RawRequests in the inspector</summary>
	public class SubscriberBase<T> : SubscriberBase, ISubscriber<T> where T : IRawRequest
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Raw data of Requests exposed in the inspector</summary>
		[SerializeField]
		protected List<T> _rawRequests;
	
		//=======================
		// Constructor
		//=======================
		/// <summary>Converts and registers <see cref="_rawRequests"/> into actual Requests</summary>
		public virtual void initialize()
		{
			// Create actual Requests from raw
			if ( _rawRequests != null )
			{
				rawRequests = _rawRequests;
				
				// Keep raws around for debugging if in editor
				#if !UNITY_EDITOR
					_rawRequests.Clear();
					_rawRequests = null;
				#endif
			}
		}
		
		//=======================
		// Destructor
		//=======================
		~SubscriberBase()
		{
			// Clear raw Requests
			if ( _rawRequests != null )
			{
				_rawRequests.Clear();
				_rawRequests = null;
			}
		}
		
		//=======================
		// Request
		//=======================
		/// <summary>Converts and registers the input array into actual Requests</summary>
		public virtual List<T> rawRequests
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
}