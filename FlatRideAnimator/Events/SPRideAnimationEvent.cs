using UnityEngine;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
[Serializable]
public class SPRideAnimationEvent : ScriptableObject
{
	public bool Done = false;
	public bool ShowSettings;
	public bool IsPlaying;
	public Color ColorIdentifier;
	public virtual string EventName { set; get; }

	public virtual void RenderInspectorGUI(SPMotor[] motors)
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


}
