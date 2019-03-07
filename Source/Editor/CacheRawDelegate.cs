using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Stores cached data for delegate drop-downs; used by the delegate inspector drawers</summary>
	public class CacheRawDelegate
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Cached target objects</summary>
		protected List<UnityEngine.Object> _targets;
		/// <summary>Display names of each target in a drop-down</summary>
		protected string[] _targetNames;
		/// <summary>Index of the currently selected target</summary>
		protected int _selectedTarget;
		/// <summary>Cached members belonging to the selected target</summary>
		protected List<IMember> _members;
		/// <summary>Display names of each of the members belonging to the selected target</summary>
		protected string[] _memberNames;
		/// <summary>Index of the currently selected member</summary>
		protected int _selectedMember;
		
		//=======================
		// Target
		//=======================
		/// <summary>Gets the display <see cref="_targetNames"/></summary>
		public string[] targetNames
		{
			get
			{
				return _targetNames;
			}
		}
		
		/// <summary>Gets/Sets the selected target object; if set, regenerates the target's members</summary>
		public virtual UnityEngine.Object target
		{
			get
			{
				return _targets == null ? null : _targets[ _selectedTarget ];
			}
			set
			{
				selectedTarget = generateTargets( value, out _targets, out _targetNames );
			}
		}
		
		/// <summary>Gets/Sets the <see cref="_selectedTarget"/>; if set, regenerates the target's members</summary>
		public virtual int selectedTarget
		{
			get
			{
				return _selectedTarget;
			}
			set
			{
				_selectedTarget = value < 0 ? 0 : value;
				
				// Generate members and names
				if ( _targets == null || _selectedTarget == 0 )
				{
					_members = null;
					_memberNames = null;
				}
				else
				{
					_members = _targets[ _selectedTarget ].GetType().GetMemberList();
					int tempListLength = _members.Count;
					_memberNames = new string[ tempListLength ];
					for ( int i = ( tempListLength - 1 ); i >= 0; --i )
					{
						_memberNames[i] = _members[i].displayName;
					}
				}
				
				// Select member
				selectedMember = 0;
			}
		}
		
		/// <summary>Generates a drop-down list of a target's relatives and output display names</summary>
		/// <param name="tTarget">Target object</param>
		/// <param name="tTargets">Output list of drop-down targets</param>
		/// <param name="tTargetNames">Output list of drop-down target display names</param>
		/// <returns>The relative index of the <paramref name="tTarget"/> relative to the outputted <paramref name="tTargets"/></returns>
		protected virtual int generateTargets( UnityEngine.Object tTarget, out List<UnityEngine.Object> tTargets, out string[] tTargetNames )
		{
			if ( tTarget == null )
			{
				tTargets = null;
				tTargetNames = null;
			}
			else
			{
				int tempTarget = 1;
				
				// Try to get GameObject
				GameObject tempGameObject = tTarget as GameObject;
				bool tempIsGameObject = tempGameObject != null;
				if ( !tempIsGameObject )
				{
					Component tempComponent = tTarget as Component;
					if ( tempComponent != null )
					{
						tempGameObject = tempComponent.gameObject;
					}
				}
				
				// Component tree
				if ( tempGameObject != null )
				{
					List<string> tempNames = new List<string>();
					tempNames.Add( "NONE" );
					tempNames.Add( tempGameObject.name );
					
					tTargets = new List<UnityEngine.Object>();
					tTargets.Add( null );
					tTargets.Add( tempGameObject );
					
					Dictionary<string,int> tempUniqueTracker = new Dictionary<string,int>();
					Component[] tempComponents = tempGameObject.GetComponents<Component>();
					int tempListLength = tempComponents.Length;
					for ( int i = 0; i < tempListLength; ++i )
					{
						int tempUniqueIndex;
						string tempName = tempComponents[i].GetType().Name;
						if ( tempUniqueTracker.TryGetValue( tempName, out tempUniqueIndex ) )
						{
							++tempUniqueTracker[ tempName ];
							tempName += " (" + tempUniqueIndex + ")";
						}
						else
						{
							tempUniqueTracker.Add( tempName, 2 );
						}
						
						tempNames.Add( tempGameObject.name + "." + tempName );
						tTargets.Add( tempComponents[i] );
						
						if ( !tempIsGameObject && tempComponents[i] == tTarget )
						{
							tempTarget += i + 1;
						}
					}
					
					tTargetNames = tempNames.ToArray();
				}
				// Non-Components
				else
				{
					tTargets = new List<UnityEngine.Object> { null, tTarget };
					tTargetNames = new string[] { "NONE", tTarget.name };
				}
				
				return tempTarget;
			}
			
			return 0;
		}
		
		/// <summary>Checks for discrepancies between the <see cref="UnityEditor.SerializedProperty"/>s and the cached data; tries to match the cache to the properties</summary>
		/// <param name="tTarget">Selected target property</param>
		/// <param name="tMember">Selected member property</param>
		/// <returns>True if the data matches</returns>
		public virtual bool validateTarget( SerializedProperty tTarget, SerializedProperty tMember )
		{
			if ( _targets == null )
			{
				target = tTarget.objectReferenceValue;
				selectedMember = findMember( tMember.stringValue );
				
				return false;
			}
			else if ( tTarget.objectReferenceValue != _targets[ _selectedTarget ] )
			{
				int tempIndex = _targets.IndexOf( tTarget.objectReferenceValue );
				if ( tempIndex >= 0 )
				{
					selectedTarget = tempIndex;
				}
				else
				{
					target = tTarget.objectReferenceValue;
				}
				
				selectedMember = findMember( tMember.stringValue );
				
				return false;
			}
			
			return true;
		}
		
		//=======================
		// Member
		//=======================
		/// <summary>Gets the <see cref="_memberNames"/> of the selected target</summary>
		public string[] memberNames
		{
			get
			{
				return _memberNames;
			}
		}
		
		/// <summary>Gets the selected member</summary>
		public IMember member
		{
			get
			{
				return _members == null ? null : _members[ _selectedMember ];
			}
		}
		
		/// <summary>Gets the selected member name</summary>
		public string memberSerializedName
		{
			get
			{
				return _members == null ? "" : _members[ _selectedMember ].serializedName;
			}
		}
		
		/// <summary>Gets/Sets the <see cref="_selectedMember"/></summary>
		public virtual int selectedMember
		{
			get
			{
				return _selectedMember;
			}
			set
			{
				_selectedMember = value < 0 ? 0 : value;
			}
		}
		
		/// <summary>Finds the index of a serialized member name within the <see cref="_members"/> list</summary>
		/// <param name="tSerializedName">Serialized name of the member being searched</param>
		/// <returns>Index if found, -1 if not</returns>
		public int findMember( string tSerializedName )
		{
			if ( _members != null )
			{
				for ( int i = ( _members.Count - 1 ); i >= 0; --i )
				{
					if ( _members[i].serializedName == tSerializedName )
					{
						return i;
					}
				}
			}
		
			return -1;
		}
		
		/// <summary>Checks for discrepancies between the <see cref="UnityEditor.SerializedProperty"/> and the cached data; tries to match the cache to the property</summary>
		/// <param name="tMember">Selected member property</param>
		/// <returns>True if the data matches</returns>
		public virtual bool validateMember( SerializedProperty tMember )
		{
			if ( _members == null )
			{
				selectedMember = 0;
				
				return false;
			}
			else if ( tMember.stringValue != _memberNames[ _selectedMember ] )
			{
				selectedMember = findMember( tMember.stringValue );
				
				return false;
			}
			
			return true;
		}
	}
}