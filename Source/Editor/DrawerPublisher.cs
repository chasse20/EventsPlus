using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;

namespace EventsPlus
{
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Editor drawer for displaying Publishers in the inspector</summary>
	[CustomPropertyDrawer( typeof( PublisherBase ), true )]
	public class DrawerPublisher : PropertyDrawer
	{
		//=======================
		// Variables
		//=======================
		/// <summary>Cached array that defines the Publisher type</summary>
		protected Type[] publisherTypeArray;
		/// <summary>Reorderable list of RawSubscriptions</summary>
		protected ReorderableList subscriptionsList;
		/// <summary>Cached data that matches each element of the <see cref="subscriptionsList"/>, used for extreme optimization purposes</summary>
		protected CachePublisher[] cache;
		
		//=======================
		// Height
		//=======================
		/// <summary>Calculates the drawer height in the inspector</summary>
		/// <param name="tProperty">SerializedProperty of the Publisher</param>
		/// <param name="tLabel">Inspector GUI Label for the <paramref name="tProperty"/></param>
		/// <returns>Pixel height of the drawer</returns>
		public override float GetPropertyHeight( SerializedProperty tProperty, GUIContent tLabel )
		{
			// Initialize
			if ( subscriptionsList == null )
			{
				publisherTypeArray = ( fieldInfo.GetValue( tProperty.serializedObject.targetObject ) as PublisherBase ).typeArray;
				SerializedProperty tempSubscriptions = tProperty.FindPropertyRelative( "_rawSubscriptions" );
				subscriptionsList = new ReorderableList( tempSubscriptions.serializedObject, tempSubscriptions, true, true, true, true );
				subscriptionsList.drawHeaderCallback += drawHeader;
				subscriptionsList.drawElementCallback += drawElement;
				subscriptionsList.elementHeightCallback += getElementHeight;
				subscriptionsList.onAddCallback += addSubscription;
				subscriptionsList.onChangedCallback += cacheList;
				cacheList( subscriptionsList );
			}

			// Calculate height
			float tempHeight = EditorUtility.singleLineHeight;
			if ( tProperty.isExpanded )
			{
				tempHeight += EditorUtility.singleLineHeight + subscriptionsList.GetHeight();
			}
			
			return tempHeight;
		}
		
		/// <summary>Calculates the inspector height for each RawSubscription element in the <see cref="subscriptionsList"/></summary>
		/// <param name="tIndex">Index of the element in the <see cref="subscriptionsList"/></param>
		/// <returns>Pixel height of the element drawn</returns>
		protected virtual float getElementHeight( int tIndex )
		{
			// Target
			float tempHeight = EditorUtility.singleLineHeight;
			SerializedProperty tempElement = subscriptionsList.serializedProperty.GetArrayElementAtIndex( tIndex );
			SerializedProperty tempProperty = tempElement.FindPropertyRelative( "_target" );
			
			// Subscriber owner if target is not null
			if ( tempProperty.objectReferenceValue != null )
			{
				tempHeight += EditorUtility.singleLineHeight;
				
				// Dynamic if method supports it
				if ( publisherTypeArray != null && cache[ tIndex ].hasDynamicMethod( tempElement.FindPropertyRelative( "_targetMethod" ).stringValue ) )
				{
					tempHeight += EditorUtility.singleLineHeight;
				}
				
				// Arguments
				tempProperty = tempElement.FindPropertyRelative( "_arguments" );
				if ( tempProperty.arraySize > 0 )
				{
					tempHeight += EditorUtility.singleLineHeight;
					if ( tempProperty.isExpanded )
					{
						int tempListLength = tempProperty.arraySize;
						for ( int i = 0; i < tempListLength; ++i )
						{
							tempHeight += EditorGUI.GetPropertyHeight( tempProperty.GetArrayElementAtIndex( i ), null, true );
						}
					}
				}
			}

			return tempHeight;
		}
		
		//=======================
		// Draw
		//=======================
		/// <summary>Draws the Publisher list of RawSubscriptions</summary>
		/// <param name="tRect">Inspector display rectangle</param>
		/// <param name="tProperty">SerializedProperty of the Subscriber</param>
		/// <param name="tLabel">Inspector GUI Label for the <paramref name="tProperty"/></param>
		public override void OnGUI( UnityEngine.Rect tRect, SerializedProperty tProperty, GUIContent tLabel )
		{
			tRect.height = EditorGUIUtility.singleLineHeight;
			tProperty.isExpanded = EditorGUI.Foldout( tRect, tProperty.isExpanded, tLabel, true );
			if ( tProperty.isExpanded )
			{
				++EditorGUI.indentLevel;
				
				tRect.position += new Vector2( 0, EditorUtility.singleLineHeight );
				EditorGUI.PropertyField( tRect, tProperty.FindPropertyRelative( "_tag" ) );
				
				tRect.position += new Vector2( 0, EditorUtility.singleLineHeight );
				tRect.height = subscriptionsList.GetHeight();
				subscriptionsList.DoList( tRect );
			}
		}
		
		/// <summary>Draws the header for the <see cref="subscriptionsList"/></summary>
		/// <param name="tRect">Inspector display rectangle</param>
		protected virtual void drawHeader( Rect tRect )
		{
			EditorGUI.LabelField( tRect, subscriptionsList.serializedProperty.displayName );
		}

		/// <summary>Draws the individual RawSubscription element for the <see cref="subscriptionsList"/></summary>
		/// <param name="tRect">Inspector display rectangle</param>
		/// <param name="tIndex">Index of the element in the <see cref="subscriptionsList"/></param>
		/// <param name="tIsActive">Whether the element is enabled</param>
		/// <param name="tIsFocused">Whether the element is selected</param>
		protected virtual void drawElement( Rect tRect, int tIndex, bool tIsActive, bool tIsFocused )
		{
			// Properties
			SerializedProperty tempElement = subscriptionsList.serializedProperty.GetArrayElementAtIndex( tIndex ); // doubles as arguments
			SerializedProperty tempTarget = tempElement.FindPropertyRelative( "_target" );
			SerializedProperty tempMethod = tempElement.FindPropertyRelative( "_targetMethod" );
			SerializedProperty tempSubscriberOwner = tempElement.FindPropertyRelative( "_subscriberOwner" );
			SerializedProperty tempSubscriberVariable = tempElement.FindPropertyRelative( "_subscriberVariable" );
			SerializedProperty tempIsDynamic = tempElement.FindPropertyRelative( "_isDynamic" );
			tempElement = tempElement.FindPropertyRelative( "_arguments" );
			
			// Draw target
			float tempOriginalWidth = tRect.width;
			if ( tempTarget.objectReferenceValue != null )
			{
				tRect.width *= 0.3f;
			}
			
			tRect.height = EditorGUIUtility.singleLineHeight;
			tRect.position += new Vector2( 0, EditorGUIUtility.standardVerticalSpacing );
			CachePublisher tempCache = cache[ tIndex ];
			if ( EditorUtility.DrawTargetField( tRect, tempTarget, tempCache.targetNames, tempCache.targetObjects, ref tempCache.selectedTarget ) )
			{
				// Clear everything if null
				if ( tempTarget.objectReferenceValue == null )
				{
					tempMethod.stringValue = null;
					tempSubscriberOwner.objectReferenceValue = null;
					tempSubscriberVariable.stringValue = null;
					tempIsDynamic.boolValue = false;
					tempElement.arraySize = 0;
					tempElement.isExpanded = false;
					tempCache.clear();
				}
				// Cache target and methods, determine if dynamic should toggled by default
				else
				{
					if ( tempCache.targetNames == null )
					{
						tempCache.cache( tempTarget.objectReferenceValue );
					}
					
					if ( tempCache.cache( tempMethod.stringValue ) )
					{
						tempIsDynamic.boolValue = tempCache.hasDynamicMethod( tempMethod.stringValue );
						adjustArguments( tempIsDynamic.boolValue, tempElement, tempCache.methodParameters[ tempCache.selectedMethod ] );
					}
				}
			}
			
			if ( tempTarget.objectReferenceValue != null )
			{
				// Draw methods, default dynamic as on, regen arguments if dynamic option is off
				float tempOriginalPosition = tRect.position.x;
				tRect.position = new Vector2( ( tRect.width + tempOriginalPosition - 10 ), tRect.position.y );
				tRect.width = tempOriginalWidth - tRect.width + 10;
				if ( EditorUtility.DrawMethodPopup( tRect, tempMethod, tempCache.methodReflectionNames, tempCache.methodDisplayNames, ref tempCache.selectedMethod ) )
				{
					tempIsDynamic.boolValue = tempCache.hasDynamicMethod( tempMethod.stringValue );
					adjustArguments( tempIsDynamic.boolValue, tempElement, tempCache.methodParameters[ tempCache.selectedMethod ] );
				}
				
				// Draw subscriber owner, generate subscriber list if not null, clear otherwise
				tRect.position = new Vector2( tempOriginalPosition, ( tRect.position.y + EditorUtility.singleLineHeight ) );
				tRect.width = tempOriginalWidth;
				if ( tempSubscriberOwner.objectReferenceValue != null )
				{
					tRect.width *= 0.3f;
				}
				
				if ( EditorUtility.DrawTargetField( tRect, tempSubscriberOwner, tempCache.subscriberOwnerNames, tempCache.subscriberOwnerObjects, ref tempCache.selectedSubscriberOwner ) )
				{
					if ( tempSubscriberOwner.objectReferenceValue == null )
					{
						tempSubscriberVariable.stringValue = null;
						tempCache.clearSubscribers();
					}
					else
					{
						if ( tempCache.subscriberOwnerNames == null )
						{
							if ( tempCache.cacheSubscriber( tempSubscriberOwner.objectReferenceValue ) )
							{
								tempCache.cacheSubscriber( tempSubscriberVariable.stringValue );
							}
							else
							{
								tempSubscriberOwner.objectReferenceValue = null;
								tempSubscriberVariable.stringValue = null;
								tempCache.clearSubscribers();
							}
						}
						else
						{
							tempCache.cacheSubscriber( tempSubscriberVariable.stringValue );
						}
					}
				}
				
				// Draw subscriber variable if subscriber owner not null
				if ( tempSubscriberOwner.objectReferenceValue != null )
				{
					tRect.position = new Vector2( ( tRect.width + tempOriginalPosition - 10 ), tRect.position.y );
					tRect.width = tempOriginalWidth - tRect.width + 10;
					EditorUtility.DrawMethodPopup( tRect, tempSubscriberVariable, tempCache.subscriberVariables, ref tempCache.selectedSubscriberVariable );
					tRect.width = tempOriginalWidth;
				}
				
				// Draw dynamic toggle if possible, adjust arguments if changed
				if ( tempCache.hasDynamicMethod( tempMethod.stringValue ) )
				{
					tRect.position = new Vector2( tempOriginalPosition, ( tRect.position.y + EditorUtility.singleLineHeight ) );
					if ( EditorUtility.DrawToggle( tRect, tempIsDynamic ) )
					{
						adjustArguments( tempIsDynamic.boolValue, tempElement, tempCache.methodParameters[ tempCache.selectedMethod ] );
					}
				}
				
				// Draw arguments
				if ( tempElement.arraySize > 0 )
				{
					tRect.position = new Vector2( tempOriginalPosition, ( tRect.position.y + EditorUtility.singleLineHeight ) );
					drawArguments( tRect, tempElement, tempCache.methodParameters[ tempCache.selectedMethod ] );
				}
			}
		}
		
		/// <summary>Draws the arguments drop-down for the RawSubscription element</summary>
		/// <param name="tRect">Inspector display rectangle</param>
		/// <param name="tArguments">SerializedProperty that points to the arguments array for the RawSubscription</param>
		/// <param name="tCachedParameters">Cached parameters list of the <paramref name="tArguments"/></param>
		protected virtual void drawArguments( Rect tRect, SerializedProperty tArguments, CachedParameter[] tCachedParameters )
		{
			tArguments.isExpanded = EditorGUI.Foldout( tRect, tArguments.isExpanded, tArguments.displayName );
			if ( tArguments.isExpanded )
			{
				++EditorGUI.indentLevel;
				bool tempIsWideModeOld = EditorGUIUtility.wideMode;
				EditorGUIUtility.wideMode = true; // fix for vector issue
				
				SerializedProperty tempProperty;
				float tempHeight = EditorUtility.singleLineHeight;
				int tempListLength = tArguments.arraySize;
				for ( int i = 0; i < tempListLength; ++i )
				{
					tempProperty = tArguments.GetArrayElementAtIndex( i );
					tRect.position += new Vector2( 0, tempHeight );
					EditorGUI.PropertyField( tRect, tempProperty, new GUIContent( tCachedParameters[i].name ), true );
					tempHeight = EditorGUI.GetPropertyHeight( tempProperty, null, true );
				}

				EditorGUIUtility.wideMode = tempIsWideModeOld;
				--EditorGUI.indentLevel;
			}
		}
		
		//=======================
		// Arguments
		//=======================
		/// <summary>Adjusts the number of RawArguments and their types to correspond to the reflected method information</summary>
		/// <param name="tIsDynamic">Whether the  method should be considered dynamic, thus emptying the arguments array attached to the RawSubscription</param>
		/// <param name="tArguments">SerializedProperty that points to the arguments array for the RawSubscription</param>
		/// <param name="tCachedParameters">Cached parameters list of the <paramref name="tArguments"/></param>
		protected virtual void adjustArguments( bool tIsDynamic, SerializedProperty tArguments, CachedParameter[] tCachedParameters )
		{
			if ( tIsDynamic || tCachedParameters == null )
			{
				tArguments.arraySize = 0;
				tArguments.isExpanded = false;
			}
			else if ( tCachedParameters != null )
			{
				tArguments.arraySize = tCachedParameters.Length;
				for ( int i = ( tCachedParameters.Length - 1 ); i >= 0; --i )
				{
					tArguments.GetArrayElementAtIndex( i ).FindPropertyRelative( "type" ).stringValue = tCachedParameters[i].typeName;
				}
			}
		}
		
		//=======================
		// Caching
		//=======================
		/// <summary>Creates a fresh, new element upon add input</summary>
		/// <param name="tList">ReorderableList reference</param>
		protected virtual void addSubscription( ReorderableList tList )
		{
			SerializedProperty tempProperty = tList.serializedProperty;
			tempProperty.arraySize += 1;
			tempProperty = tempProperty.GetArrayElementAtIndex( tempProperty.arraySize - 1 );
			tempProperty.FindPropertyRelative( "_target" ).objectReferenceValue = null;
			tempProperty.FindPropertyRelative( "_targetMethod" ).stringValue = null;
			tempProperty.FindPropertyRelative( "_subscriberOwner" ).objectReferenceValue = null;
			tempProperty.FindPropertyRelative( "_subscriberVariable" ).stringValue = null;
			tempProperty.FindPropertyRelative( "_isDynamic" ).boolValue = false;
			tempProperty.FindPropertyRelative( "_arguments" ).ClearArray();			
		}
		
		/// <summary>Caches drop-down data for the entire <see cref="subscriptionsList"/> for optimization</summary>
		/// <param name="tList">ReorderableList reference</param>
		protected virtual void cacheList( ReorderableList tList )
		{
			int tempListLength = tList.count;
			if ( tempListLength > 0 )
			{
				SerializedProperty tempElement;
				cache = new CachePublisher[ tempListLength ];
				for ( int i = ( tempListLength - 1 ); i >= 0; --i )
				{
					tempElement = tList.serializedProperty.GetArrayElementAtIndex( i );
					cache[i] = new CachePublisher( publisherTypeArray, tempElement.FindPropertyRelative( "_target" ).objectReferenceValue, tempElement.FindPropertyRelative( "_targetMethod" ).stringValue, tempElement.FindPropertyRelative( "_subscriberOwner" ).objectReferenceValue, tempElement.FindPropertyRelative( "_subscriberVariable" ).stringValue );
				}
			}
			else
			{
				cache = null;
			}
		}
	}
}