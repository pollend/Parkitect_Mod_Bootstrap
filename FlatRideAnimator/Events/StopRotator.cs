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
	public Rotator rotator;
	float lastTime;
	public override string EventName
	{
		get { return "StopRotator"; }
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
	{
		if (rotator)
		{
			ColorIdentifier = rotator.ColorIdentifier;
		}

		foreach (Rotator R in motors.OfType<Rotator>().ToList())
		{
			if (R == rotator)
				GUI.color = Color.red / 1.3f;
			if (GUILayout.Button(R.Identifier))
			{
				rotator = R;
			}

			GUI.color = Color.white;
		}

		base.RenderInspectorGUI(motors);
	}
#endif

	public override void Enter()
	{
		lastTime = Time.realtimeSinceStartup;

		rotator.Stop();
		base.Enter();
	}

	public override void Run(Transform root)
	{
		if (rotator)
		{
			rotator.tick(Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			if (rotator.IsStopped())
			{
				Done = true;
			}

			base.Run(root);
		}
	}

	public override void Deserialize(Dictionary<string, object> elements)
	{
		if (elements.ContainsKey("rotator"))
		{
			rotator = new Rotator();
			rotator.Deserialize((Dictionary<string, object>) elements["rotator"]);
		}

		base.Deserialize(elements);
	}

	public override Dictionary<string, object> Serialize(Transform root)
	{
		if (rotator == null)
			return null;

		return new Dictionary<string, object>
		{
			{"rotator", rotator.Serialize(root)}
		};
	}
}

