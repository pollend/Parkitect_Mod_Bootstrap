#if UNITY_EDITOR
using UnityEngine;
using System.Linq;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
[RideAnimationEventTag("StopRotator")]
public class StopRotator : RideAnimationEvent
{
	public Rotator Rotator;
	private float _lastTime;
	public override string EventName
	{
		get { return "StopRotator"; }
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
	{
		if (Rotator)
		{
			ColorIdentifier = Rotator.ColorIdentifier;
		}

		foreach (Rotator R in motors.OfType<Rotator>().ToList())
		{
			if (R == Rotator)
				GUI.color = Color.red / 1.3f;
			if (GUILayout.Button(R.Identifier))
			{
				Rotator = R;
			}

			GUI.color = Color.white;
		}

		base.RenderInspectorGUI(motors);
	}
#endif

	public override void Enter()
	{
		_lastTime = Time.realtimeSinceStartup;

		Rotator.Stop();
		base.Enter();
	}

	public override void Run(Transform root)
	{
		if (Rotator)
		{
			Rotator.Tick(Time.realtimeSinceStartup - _lastTime, root);
			_lastTime = Time.realtimeSinceStartup;
			if (Rotator.IsStopped())
			{
				Done = true;
			}

			base.Run(root);
		}
	}

	public override void Deserialize(Dictionary<string, object> elements, Motor[] motors)
	{
		if (elements.ContainsKey("RotatorIndex"))
			Rotator = (Rotator) motors[Convert.ToInt32(elements["RotatorIndex"])];

		base.Deserialize(elements,motors);
	}

	public override Dictionary<string, object> Serialize(Transform root, Motor[] motors)
	{
		if (Rotator == null)
			return null;
		
		return new Dictionary<string,object> {
			{"RotatorIndex", Array.IndexOf(motors, Rotator)}
		};
	}
}

