using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EventsPlus
{	
	//##########################
	// Class Declaration
	//##########################
	/// <summary>Utility class for editor functions and display</summary>
	public static class EditorUtility
	{
		//=======================
		// Settings
		//=======================
		/// <summary>Menu item that will select the <see cref="Settings"/> object from "Assets/EventsPlus/Resources" folder; this will create one if it doesn't exist</summary>
		[MenuItem( "Events Plus/Settings", false )]
		public static void OpenSettings()
		{
			string tempPath = "Assets/EventsPlus/Resources/Settings.asset";
			Settings tempSettings = AssetDatabase.LoadMainAssetAtPath( tempPath ) as Settings;
			if ( tempSettings == null )
			{
				tempSettings = ScriptableObject.CreateInstance<Settings>();
				AssetDatabase.CreateAsset( tempSettings, AssetDatabase.GenerateUniqueAssetPath( tempPath ) );
				AssetDatabase.SaveAssets();
				UnityEditor.EditorUtility.FocusProjectWindow();
			}
			
			Selection.activeObject = tempSettings;
		}
		
		//=======================
		// Serialized Property
		//=======================
		/// <summary>Returns the target object instance that owns <paramref name="tProperty"/></summary>
		/// <param name="tProperty">Property owned by the target instance</param>
		/// <returns>Target instance</returns>
		public static object GetTarget( this SerializedProperty tProperty )
		{
			if ( tProperty != null )
			{
				object tempObject = tProperty.serializedObject.targetObject;
				
				string[] tempPaths = tProperty.propertyPath.Replace( "Array.data", "" ).Split( '.' );
				int tempListLength = tempPaths.Length;
				for ( int i = 0; i < tempListLength; ++i )
				{
					if ( tempPaths[i][0] == '[' )
					{
						int tempIndex = Int32.Parse( tempPaths[i].Substring( 1, tempPaths[i].IndexOf( ']' ) - 1 ) );
						if ( tempIndex < ( tempObject as IList ).Count )
						{
							tempObject = ( tempObject as IList )[ tempIndex ];
						}
						else
						{
							return null;
						}
					}
					else
					{
						tempObject = tempObject.GetType().GetField( tempPaths[i], Utility.InstanceFlags ).GetValue( tempObject );
					}
				}
				
				return tempObject;
			}
			
			return null;
		}
		
		/// <summary>Returns the <see cref="Publisher"/> instance that owns <paramref name="tProperty"/></summary>
		/// <param name="tProperty">Property owned by the Publisher instance</param>
		/// <returns>Publisher instance</returns>
		public static Publisher GetPublisher( this SerializedProperty tProperty )
		{
			if ( tProperty != null )
			{
				object tempObject = tProperty.serializedObject.targetObject;
				
				string[] tempPaths = tProperty.propertyPath.Replace( "Array.data", "" ).Split( '.' );
				int tempListLength = tempPaths.Length;
				for ( int i = 0; i < tempListLength; ++i )
				{
					if ( tempPaths[i][0] == '[' )
					{
						int tempIndex = Int32.Parse( tempPaths[i].Substring( 1, tempPaths[i].IndexOf( ']' ) - 1 ) );
						if ( tempIndex < ( tempObject as IList ).Count )
						{
							tempObject = ( tempObject as IList )[ tempIndex ];
						}
						else
						{
							return null;
						}
					}
					else
					{
						tempObject = tempObject.GetType().GetField( tempPaths[i], Utility.InstanceFlags ).GetValue( tempObject );
					}
					
					Publisher tempPublisher = tempObject as Publisher;
					if ( tempPublisher != null )
					{
						return tempPublisher;
					}
				}
			}
			
			return null;
		}
		
		//=======================
		// Namespaces
		//=======================
		/// <summary>Returns a list of all assembly <see cref="Namespace"/>s that contain <see cref="UnityEngine.Object"/> classes</summary>
		/// <returns>List of namespaces</returns>
		public static List<Namespace> GetUnityObjectNamespaces()
		{
			// Generate
			List<Type> tempClasses;
			Dictionary<string,List<Type>> tempNamespaces = null;
			Assembly[] tempAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			for ( int i = ( tempAssemblies.Length - 1 ); i >= 0; --i )
			{
				if ( tempAssemblies[i].FullName.IndexOf( "UnityEditor" ) < 0 )
				{
					Type[] tempTypes = tempAssemblies[i].GetTypes();
					Type tempType;
					for ( int j = ( tempTypes.Length - 1 ); j >= 0; --j )
					{
						tempType = tempTypes[j];
						if ( tempType == typeof( UnityEngine.Object ) || tempType.IsSubclassOf( typeof( UnityEngine.Object ) ) )
						{
							bool tempIsNamespace = tempType.Namespace != null;
							if ( !tempIsNamespace || tempType.Namespace.IndexOf( "UnityEditor" ) < 0 )
							{
								string tempNamespace = tempIsNamespace ? tempType.Namespace : "NONE";
								if ( tempNamespaces == null )
								{
									tempNamespaces = new Dictionary<string,List<Type>>();
									tempClasses = new List<Type>();
									tempClasses.Add( tempType );
									tempNamespaces.Add( tempNamespace, tempClasses );
								}
								else if ( tempNamespaces.TryGetValue( tempNamespace, out tempClasses ) )
								{
									tempClasses.Add( tempType );
								}
								else
								{
									tempClasses = new List<Type>();
									tempClasses.Add( tempType );
									tempNamespaces.Add( tempNamespace, tempClasses );
								}
							}
						}
					}
				}
			}
			
			// Compile and order
			tempClasses = new List<Type>();
			tempClasses.Add( typeof( object ) );
			List<Namespace> tempOut = new List<Namespace>();
			tempOut.Add( new Namespace( "System", tempClasses ) );
			
			if ( tempNamespaces != null )
			{
				foreach ( KeyValuePair<string,List<Type>> tempPair in tempNamespaces )
				{
					tempPair.Value.Sort( ( tA, tB ) => string.Compare( tA.Name, tB.Name ) );
					tempOut.Add( new Namespace( tempPair.Key, tempPair.Value ) );
				}
				
				tempOut.Sort( ( tA, tB ) => string.Compare( tA.name, tB.name ) );
			}
			
			return tempOut;
		}
		
		//=======================
		// Members
		//=======================
		/// <summary>Returns a list of all <see cref="IMember"/>s belonging to the <paramref name="tType"/></summary>
		/// <param name="tType">Type to search</param>
		/// <param name="tIsFiltered">If true, will attempt to filter members defined in the <see cref="Settings"/></param>
		/// <returns>List of members</returns>
		public static List<IMember> GetMemberList( this Type tType, bool tIsFiltered = true )
		{
			if ( tType != null )
			{
				List<IMember> tempMembers = null;
				
				// Fields
				List<MemberField> tempFields = tType.GetFieldList( tIsFiltered );
				if ( tempFields != null )
				{
					if ( tempMembers == null )
					{
						tempMembers = new List<IMember>();
					}
					
					int tempListLength = tempFields.Count;
					for ( int i = 0; i < tempListLength; ++i )
					{
						tempMembers.Add( tempFields[i] );
					}
				}
				
				// Properties
				List<MemberProperty> tempProperties = tType.GetPropertyList( tIsFiltered );
				if ( tempProperties != null )
				{
					if ( tempMembers == null )
					{
						tempMembers = new List<IMember>();
					}
					
					int tempListLength = tempProperties.Count;
					for ( int i = 0; i < tempListLength; ++i )
					{
						tempMembers.Add( tempProperties[i] );
					}
				}
				
				// Methods
				List<MemberMethod> tempMethods = tType.GetMethodList( tIsFiltered );
				if ( tempMethods != null )
				{
					if ( tempMembers == null )
					{
						tempMembers = new List<IMember>();
					}
					
					int tempListLength = tempMethods.Count;
					for ( int i = 0; i < tempListLength; ++i )
					{
						tempMembers.Add( tempMethods[i] );
					}
				}
				
				return tempMembers;
			}
			
			return null;
		}
		
		/// <summary>Returns a list of all <see cref="MemberField"/>s belonging to the <paramref name="tType"/></summary>
		/// <param name="tType">Type to search</param>
		/// <param name="tIsFiltered">If true, will attempt to filter fields defined in the <see cref="Settings"/></param>
		/// <returns>List of fields</returns>
		public static List<MemberField> GetFieldList( this Type tType, bool tIsFiltered = true )
		{
			if ( tType != null )
			{
				// Flags
				BindingFlags tempFlags = BindingFlags.Public | BindingFlags.Instance;
				if ( Settings.instance.isPrivateDisplayed )
				{
					tempFlags |= BindingFlags.NonPublic;
				}
				
				// Filter member fields
				FieldInfo[] tempFields = tType.GetFields( tempFlags );
				int tempListLength = tempFields.Length;
				if ( tempListLength > 0 )
				{
					List<MemberField> tempOut = null;
					FieldInfo tempField;
					for ( int i = 0; i < tempListLength; ++i )
					{
						tempField = tempFields[i];
						if ( !tempField.IsInitOnly && !tempField.IsLiteral )
						{
							MemberField tempMember = new MemberField( tempField ); 
							if ( !tIsFiltered || !Settings.instance.isMemberFiltered( tempField.DeclaringType, tempField.ReflectedType, tempMember ) )
							{
								if ( tempOut == null )
								{
									tempOut = new List<MemberField>();
								}
								
								tempOut.Add( tempMember );
							}
						}
					}
					
					return tempOut;
				}
			}
			
			return null;
		}
		
		/// <summary>Returns a list of all <see cref="MemberProperty"/>s belonging to the <paramref name="tType"/></summary>
		/// <param name="tType">Type to search</param>
		/// <param name="tIsFiltered">If true, will attempt to filter properties defined in the <see cref="Settings"/></param>
		/// <returns>List of properties</returns>
		public static List<MemberProperty> GetPropertyList( this Type tType, bool tIsFiltered = true )
		{
			if ( tType != null )
			{
				// Flags
				BindingFlags tempFlags = BindingFlags.Public | BindingFlags.Instance;
				if ( Settings.instance.isPrivateDisplayed )
				{
					tempFlags |= BindingFlags.NonPublic;
				}
				
				// Filter member properties
				PropertyInfo[] tempProperties = tType.GetProperties( tempFlags );
				int tempListLength = tempProperties.Length;
				if ( tempListLength > 0 )
				{
					List<MemberProperty> tempOut = null;
					PropertyInfo tempProperty;
					for ( int i = 0; i < tempListLength; ++i )
					{
						tempProperty = tempProperties[i];
						if ( tempProperty.CanWrite )
						{
							MemberProperty tempMember = new MemberProperty( tempProperty ); 
							if ( !tIsFiltered || !Settings.instance.isMemberFiltered( tempProperty.DeclaringType, tempProperty.ReflectedType, tempMember ) )
							{
								if ( tempOut == null )
								{
									tempOut = new List<MemberProperty>();
								}
								
								tempOut.Add( tempMember );
							}
						}
					}
					
					return tempOut;
				}
			}
			
			return null;
		}
		
		/// <summary>Returns a list of all <see cref="MemberMethod"/>s belonging to the <paramref name="tType"/></summary>
		/// <param name="tType">Type to search</param>
		/// <param name="tIsFiltered">If true, will attempt to filter methods defined in the <see cref="Settings"/></param>
		/// <returns>List of methods</returns>
		public static List<MemberMethod> GetMethodList( this Type tType, bool tIsFiltered = true )
		{
			if ( tType != null )
			{
				// Flags
				BindingFlags tempFlags = BindingFlags.Public | BindingFlags.Instance;
				if ( Settings.instance.isPrivateDisplayed )
				{
					tempFlags |= BindingFlags.NonPublic;
				}
				
				// Filter member methods
				MethodInfo[] tempMethods = tType.GetMethods( tempFlags );
				int tempListLength = tempMethods.Length;
				if ( tempListLength > 0 )
				{
					List<MemberMethod> tempOut = null;
					MethodInfo tempMethod;
					for ( int i = 0; i < tempListLength; ++i )
					{
						tempMethod = tempMethods[i];
						if ( !tempMethod.IsSpecialName && !tempMethod.IsGenericMethod )
						{
							MemberMethod tempMember = new MemberMethod( tempMethod ); 
							if ( !tIsFiltered || !Settings.instance.isMemberFiltered( tempMethod.DeclaringType, tempMethod.ReflectedType, tempMember ) )
							{
								if ( tempOut == null )
								{
									tempOut = new List<MemberMethod>();
								}
								
								tempOut.Add( tempMember );
							}
						}
					}
					
					return tempOut;
				}
			}
			
			return null;
		}
		
		//=======================
		// Inspector
		//=======================
		/// <summary>Gets the width of Unity's indents in the inspector</summary>
		public static float IndentSize
		{
			get
			{
				return 15;
			}
		}
	}
}