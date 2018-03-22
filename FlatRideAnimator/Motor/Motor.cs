
 #if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Motor : ScriptableObject
{
	[NonSerialized]
	public bool ShowSettings;
	public string Identifier = "";
	public Color ColorIdentifier;
	public virtual string EventName { set; get; }
	public void Awake()
	{
		ColorIdentifier = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}
#if UNITY_EDITOR
	public virtual void InspectorGUI(Transform root)
	{
		ColorIdentifier = EditorGUILayout.ColorField("Color ", ColorIdentifier);
	}
#endif
	public virtual void Enter(Transform root)
	{

	}
	public virtual void Reset(Transform root)
	{

	}

	public virtual void PrepareExport(ParkitectObj parkitectObj)
	{
	}
		
	public virtual Dictionary<string,object> Serialize(Transform root){return null;}
	public virtual void Deserialize(Dictionary<string,object> elements){}


	public static Type FindMotorTypeByTag(String tag)
	{
		IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies ();
		foreach (Assembly assembly in scriptAssemblies) 
		{
			foreach (Type type in assembly.GetTypes().Where(T => T.IsClass && T.IsSubclassOf(typeof(Motor))))
			{
				object[] nodeAttributes = type.GetCustomAttributes(typeof(MotorTag), false);                    
				if (nodeAttributes.Length <= 0) continue;

				MotorTag attr = nodeAttributes[0] as MotorTag;
				if (attr != null) {
					if (attr.Name.Equals(tag))
					{
						return type;
					}
				}
			}
		}

		return null;
	}
	
	
	public static String GetTagFromMotor(Type type)
	{
		object[] nodeAttributes = type.GetCustomAttributes(typeof(MotorTag), false);
		if (nodeAttributes.Length <= 0) return null;
		MotorTag attr = nodeAttributes[0] as MotorTag;
		if (attr != null)
		{
			return attr.Name;
		}
		return null;
	}
}
