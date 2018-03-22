using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[ExecuteInEditMode]
[Serializable]
public class RideAnimationEvent : ScriptableObject
{
	public bool Done = false;
	public bool ShowSettings;
	public bool IsPlaying;
	public Color ColorIdentifier;
	public virtual string EventName { set; get; }

	public virtual void RenderInspectorGUI(Motor[] motors)
	{

	}
	public virtual void Enter()
	{
		IsPlaying = true;
	}
	public virtual void Run(Transform root)
	{

	}
	public virtual void Exit()
	{

		IsPlaying = false;
		Done = false;
	}

	public virtual void Bootstrap()
	{
	}


	public virtual Dictionary<string,object> Serialize(Transform root){return null;}
	public virtual void Deserialize(Dictionary<string,object> elements){}


	public static Type FindRideAnimationTypeByTag(String tag)
	{
		IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies ();
		foreach (Assembly assembly in scriptAssemblies) 
		{
			foreach (Type type in assembly.GetTypes().Where(T => T.IsClass && T.IsSubclassOf(typeof(RideAnimationEvent))))
			{
				object[] nodeAttributes = type.GetCustomAttributes(typeof(RideAnimationEventTag), false);
				if (nodeAttributes.Length <= 0) continue;
				RideAnimationEventTag attr = nodeAttributes[0] as RideAnimationEventTag;
				if (attr != null)
				{
					if (attr.Name.Equals(tag))
					{
						return type;
					}
				}
			}
		}

		return null;
	}
	
	public static String GetTagFromRideAnimationEvent(Type type)
	{
		object[] nodeAttributes = type.GetCustomAttributes(typeof(RideAnimationEventTag), false);
		if (nodeAttributes.Length <= 0) return null;
		RideAnimationEventTag attr = nodeAttributes[0] as RideAnimationEventTag;
		if (attr != null)
		{
			return attr.Name;
		}
		return null;
	}
}
