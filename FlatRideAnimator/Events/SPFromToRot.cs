#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

[ExecuteInEditMode]
[Serializable]
public class SPFromToRot : SPRideAnimationEvent
{
	public SPRotateBetween rotator;
	float lastTime;

	public override string EventName {
		get {
			return "From-To Rot";
		}
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(SPMotor[] motors)
	{
		if (rotator) {
			ColorIdentifier = rotator.ColorIdentifier;
		}
		foreach (SPRotateBetween R in motors.OfType<SPRotateBetween>().ToList()) {
			if (R == rotator)
				GUI.color = Color.red;
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
		lastTime = Time.realtimeSinceStartup;

		rotator.startFromTo();
		base.Enter();
	}
	public override void Run(Transform root)
	{

		if (rotator) {
			rotator.tick (Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			if (rotator.isStopped ()) {
				done = true;
			}
			base.Run (root);
		}

	}

	public override List<XElement> Serialize (Transform root)
	{
		return new List<XElement> (new XElement[] {
			new XElement ("rotator", rotator.Serialize (root)),
		});
	}
}
