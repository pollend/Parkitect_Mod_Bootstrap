#if UNITY_EDITOR
using UnityEngine;
using System.Linq;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public class SPApplyRotation : SPRideAnimationEvent
{
	[SerializeField]
	public SPMultipleRotations rotator;
	float lastTime;


	public override string EventName {
		get {
			return "ApplyRotations";
		}
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(SPMotor[] motors)
	{

		if (rotator) {
			ColorIdentifier = rotator.ColorIdentifier;
		}
		foreach (SPMultipleRotations R in motors.OfType<SPMultipleRotations>().ToList()) {
			if (R == rotator)
				GUI.color = Color.red / 1.3f;
			if (GUILayout.Button (R.Identifier)) {
				rotator = R;

			}
			GUI.color = Color.white;
		}
		base.RenderInspectorGUI (motors);
	}
#endif

	public override void Enter()
	{

	}
	public override void Run(Transform root)
	{
		if (rotator)
		{
			rotator.tick(Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			done = true;
			base.Run(root);
		}

	}

	public override void Deserialize (Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("rotator")) {
			rotator = new SPMultipleRotations ();
			rotator.Deserialize ((Dictionary<string, object>) elements["rotator"]);
		}
		base.Deserialize (elements);
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
