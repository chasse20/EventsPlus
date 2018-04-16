using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventsPlus
{	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Manager class for tracking Requests and automatically registering them to Publishers</summary>
	public static class Manager
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Managed Publishers sorted by tag hash</summary>
		private static Dictionary<int,HashSet<IPublisher>> _trackedPublishers;
		/// <summary>Event callback for when a unique Publisher was added or removed</summary>
		public static event Action<IPublisher,bool> onPublisherSet;
		
		//=======================
		// Publisher tracking
		//=======================
		/// <summary>Tries to return a list of Publishers that correspond to a tag hash</summary>
		/// <param name="tTag">Tag hash</param>
		/// <param name="tPublishers">Copied list of Publishers belonging to the <paramref name="tTag"/> key</param>
		/// <returns>True if <paramref name="tPublishers"/> found</returns>
		public static bool GetPublishers( int tTag, out List<IPublisher> tPublishers )
		{
			HashSet<IPublisher> tempPublishers;
			if ( _trackedPublishers != null && _trackedPublishers.TryGetValue( tTag, out tempPublishers ) && tempPublishers != null )
			{
				tPublishers = new List<IPublisher>( tempPublishers );
				return true;
			}
			
			tPublishers = null;
			return false;
		}
		
		/// <summary>Adds a Publisher to be managed</summary>
		/// <param name="tPublisher">Publisher to be added</param>
		/// <returns>True if <paramref name="tPublisher"/> doesn't already exist and successfully added</returns>
		public static bool AddPublisher( IPublisher tPublisher )
		{
			if ( tPublisher != null )
			{
				if ( _trackedPublishers == null )
				{
					_trackedPublishers = new Dictionary<int,HashSet<IPublisher>>();
				}
				
				if ( !_trackedPublishers.ContainsKey( tPublisher.tagHash ) )
				{
					_trackedPublishers[ tPublisher.tagHash ] = new HashSet<IPublisher>();
				}
				
				if ( _trackedPublishers[ tPublisher.tagHash ].Add( tPublisher ) )
				{
					if ( onPublisherSet != null )
					{
						onPublisherSet( tPublisher, true );
					}
					
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Removes a Publisher from management</summary>
		/// <param name="tPublisher">Publisher to be removed</param>
		/// <returns>True if <paramref name="tPublisher"/> exists and successfully removed</returns>
		public static bool RemovePublisher( IPublisher tPublisher )
		{
			if ( tPublisher != null && _trackedPublishers != null )
			{
				HashSet<IPublisher> tempPublishers;
				if ( _trackedPublishers.TryGetValue( tPublisher.tagHash, out tempPublishers ) && tempPublishers.Remove( tPublisher ) )
				{
					if ( tempPublishers.Count == 0 )
					{
						_trackedPublishers.Remove( tPublisher.tagHash );
						if ( _trackedPublishers.Count == 0 )
						{
							_trackedPublishers = null;
						}
					}
					
					if ( onPublisherSet != null )
					{
						onPublisherSet( tPublisher, false );
					}
					
					return true;
				}
			}
			
			return false;
		}
	}
}