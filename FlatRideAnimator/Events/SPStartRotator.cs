#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.Xml.Linq;
using System;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
[Serializable]
public class SPStartRotator : SPRideAnimationEvent
{
	public SPRotator rotator;
	float lastTime;
	public override string EventName {
		get {
			return "StartRotator";
		}
	}

#if UNITY_EDITOR
	public override void RenderInspectorGUI(SPMotor[] motors)
	{
		if (rotator) {
			ColorIdentifier = rotator.ColorIdentifier;
		}
		foreach (SPRotator R in motors.OfType<SPRotator>().ToList()) {
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
		lastTime = Time.realtimeSinceStartup;

		rotator.start ();
		base.Enter ();
	}

	public override void Run(Transform root)
	{
		if (rotator) {

			rotator.tick (Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			if (rotator.reachedFullSpeed ()) {
				done = true;
			}
			base.Run (root);
		}
	}

	public override List<XElement> Serialize (Transform root)
	{
		return new List<XElement> (new XElement[] {
			new XElement ("rotator", rotator.Serialize (root))
		});
	}
}

