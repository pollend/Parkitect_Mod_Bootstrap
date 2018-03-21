#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
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

		rotator.Start ();
		base.Enter ();
	}

	public override void Run(Transform root)
	{
		if (rotator) {

			rotator.tick (Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			if (rotator.ReachedFullSpeed ()) {
				Done = true;
			}
			base.Run (root);
		}
	}

	public override void Deserialize (Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("rotator") ) {
			this.rotator = new SPRotator ();
			rotator.Deserialize ((Dictionary<string, object>) elements["rotator"]);
		}

		base.Deserialize (elements);
	}

	public override Dictionary<string,object> Serialize (Transform root)
	{
		if (rotator == null)
			return null;
		
		return new Dictionary<string,object> {
			{"rotator", rotator.Serialize (root)}
		};
	}


}

