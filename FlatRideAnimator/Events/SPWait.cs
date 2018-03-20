#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class SPWait : SPRideAnimationEvent
{
	[SerializeField] public float seconds;
	float timeLimit;
	public override string EventName
	{
		get { return "Wait"; }
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(SPMotor[] motors)
	{
		seconds = EditorGUILayout.FloatField("Seconds", seconds);
		if (isPlaying)
		{
			GUILayout.Label("Time" + (timeLimit - Time.realtimeSinceStartup));
		}

		base.RenderInspectorGUI(motors);
	}
#endif

	public override void Enter()
	{
		timeLimit = Time.realtimeSinceStartup + seconds;
		base.Enter();
	}

	public override void Run(Transform root)
	{
		if (Time.realtimeSinceStartup > timeLimit)
		{
			done = true;
		}
		base.Run(root);
	}

	public override void Deserialize(Dictionary<string, object> elements)
	{

		if (elements.ContainsKey("timeLimit"))
		{
			timeLimit = (int) (long) elements["timeLimit"];
		}

		base.Deserialize(elements);
	}

	public override Dictionary<string, object> Serialize(Transform root)
	{
		return new Dictionary<string, object>
		{
			{"timeLimit", timeLimit}
		};
	}
}

