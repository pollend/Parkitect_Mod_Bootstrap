﻿#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;


[ExecuteInEditMode]
[Serializable]
public class SPToFromRot : SPRideAnimationEvent
{
	public SPRotateBetween rotator;

	float lastTime;
	public override string EventName {
		get {
			return "To-From Rot";
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

		rotator.startToFrom ();
		base.Enter ();
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
	public override void Deserialize (Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("rotator") ) {
			rotator = new SPRotateBetween ();
			rotator.Deserialize ((Dictionary<string, object>) elements["rotator"]);
		}

		base.Deserialize (elements);
	}
	public override Dictionary<string,object> Serialize (Transform root)
	{
		if (rotator == null)
			return null;
		return new Dictionary<string, object>{
			{"rotator", rotator.Serialize (root)}
		};
	}

}

