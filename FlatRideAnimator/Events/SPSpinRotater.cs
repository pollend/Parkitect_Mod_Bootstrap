﻿#if UNITY_EDITOR
using UnityEditor;
using System.Xml.Linq;
using System.Collections.Generic;


#endif
using System.Collections.Generic;
using System.Xml.Linq;
using System;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
[Serializable]
public class SPSpinRotater : SPRideAnimationEvent
{
	[SerializeField]
	public SPRotator rotator;
	[SerializeField]
	public bool spin = false;
	[SerializeField]
	public float spins = 1;
	float lastTime;


	public override string EventName {
		get {
			return "SpinRotator";
		}
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(SPMotor[] motors)
	{
		if (rotator) {
			ColorIdentifier = rotator.ColorIdentifier;
			spin = EditorGUILayout.Toggle ("amountOfSpins ", spin);
			if (spin)
				spins = EditorGUILayout.FloatField ("spins ", spins);

			EditorGUILayout.LabelField ("Amount " + rotator.getRotationsCount ());
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
		rotator.resetRotations ();
		base.Enter ();
	}

	public override void Run(Transform root)
	{
		if (rotator) {
			rotator.tick (Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			if (spin) {
				if (rotator.getRotationsCount () >= spins) {
					done = true;
				}
			} else {
				done = true;
			}

			base.Run (root);
		}
	}

	public override List<XElement> Serialize (Transform root)
	{
		return new List<XElement> (new XElement[] {
			new XElement ("rotator", rotator.Serialize (root)),
			new XElement ("spin", spin),
			new XElement ("spins", spins)
		});
	}


}
