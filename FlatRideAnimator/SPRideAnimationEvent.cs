using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

[ExecuteInEditMode]
[Serializable]
public class SPRideAnimationEvent : ScriptableObject
{
	public bool done = false;
	public bool showSettings;
	public bool isPlaying;
	public Color ColorIdentifier;
	public virtual string EventName { set; get; }

	public virtual void RenderInspectorGUI(SPMotor[] motors)
	{

	}
	public virtual void Enter()
	{
		isPlaying = true;
	}
	public virtual void Run(Transform root)
	{

	}
	public virtual void Exit()
	{

		isPlaying = false;
		done = false;
	}

	public virtual void Bootstrap()
	{
	}

	public virtual List<XElement> Serialize(Transform root){return null;}
	public virtual void Deserialize(XElement elements){}

}
