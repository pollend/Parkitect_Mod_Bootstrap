using UnityEngine;
using System;

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

}
