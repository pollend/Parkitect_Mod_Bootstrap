using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParkitectObjectType
{
	[System.NonSerialized]
	private List<string> options = new List<string>();
	[System.NonSerialized]
	private List<Type> parkitectObjs = new List<Type>();
	[System.NonSerialized]
	private Dictionary<String,Type> parkitectObjectTypeMapping = new Dictionary<string, Type> ();

	public ParkitectObjectType ()
	{
		IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies ().Where ((Assembly assembly) => assembly.FullName.Contains ("Assembly"));
		foreach (Assembly assembly in scriptAssemblies) 
		{
			foreach (Type type in assembly.GetTypes().Where(T => T.IsClass && T.IsSubclassOf(typeof(ParkitectObj))))
			{
				object[] nodeAttributes = type.GetCustomAttributes(typeof(ParkitectObjectTag), false);                    
				ParkitectObjectTag attr = nodeAttributes[0] as ParkitectObjectTag;
				if (attr != null) {
					parkitectObjs.Add (type);
					options.Add (attr.Name);
					parkitectObjectTypeMapping.Add (type.Name, type);
				}
			}
		}
	}

	public Type GetType(String typeName)
	{
		Type type = null;
		if (parkitectObjectTypeMapping.TryGetValue (typeName,out type)) {
			return type;
		}
		return null;
	}

	public String[] Options{ get { return this.options.ToArray ();} }

}


