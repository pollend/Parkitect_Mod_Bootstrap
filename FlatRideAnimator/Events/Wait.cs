#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
[RideAnimationEventTag("Wait")]
public class Wait : RideAnimationEvent
{
	[SerializeField] public float Seconds;
	private float _timeLimit;
	public override string EventName
	{
		get { return "Wait"; }
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
	{
		Seconds = EditorGUILayout.FloatField("Seconds", Seconds);
		if (IsPlaying)
		{
			GUILayout.Label("Time" + (_timeLimit - Time.realtimeSinceStartup));
		}

		base.RenderInspectorGUI(motors);
	}
#endif

	public override void Enter()
	{
		_timeLimit = Time.realtimeSinceStartup + Seconds;
		base.Enter();
	}

	public override void Run(Transform root)
	{
		if (Time.realtimeSinceStartup > _timeLimit)
		{
			Done = true;
		}

		base.Run(root);
	}

	public override void Deserialize(Dictionary<string, object> elements, Motor[] motors)
	{
		if (elements.ContainsKey("Seconds"))
			Seconds = Convert.ToInt32(elements["Seconds"]);

		base.Deserialize(elements, motors);
	}

	public override Dictionary<string, object> Serialize(Transform root, Motor[] motors)
	{
		return new Dictionary<string, object>
		{
			{"Seconds", Seconds}
		};
	}
}

