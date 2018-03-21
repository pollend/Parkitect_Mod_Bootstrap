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
	[SerializeField] public float Seconds;
	private float _timeLimit;
	public override string EventName
	{
		get { return "Wait"; }
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(SPMotor[] motors)
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

	public override void Deserialize(Dictionary<string, object> elements)
	{

		if (elements.ContainsKey("timeLimit"))
		{
			_timeLimit = Convert.ToInt32(elements["timeLimit"]);
		}

		base.Deserialize(elements);
	}

	public override Dictionary<string, object> Serialize(Transform root)
	{
		return new Dictionary<string, object>
		{
			{"timeLimit", _timeLimit}
		};
	}
}

