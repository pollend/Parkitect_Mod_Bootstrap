﻿#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Linq;


#endif
using System;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
[Serializable]
public class SPStopRotator : SPRideAnimationEvent
{
	public SPRotator rotator;
	float lastTime;
	public override string EventName
	{
		get
		{
			return "StopRotator";
		}
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(SPMotor[] motors)
{
    if (rotator)
    {
        ColorIdentifier = rotator.ColorIdentifier;
    }
	foreach (SPRotator R in motors.OfType<SPRotator>().ToList())
    {
        if (R == rotator)
            GUI.color = Color.red / 1.3f;
        if(GUILayout.Button(R.Identifier))
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

		rotator.stop();
		base.Enter();
	}
	public override void Run(Transform root)
	{
		if (rotator)
		{
			rotator.tick(Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			if (rotator.isStopped())
			{
				done = true;
			}
			base.Run(root);
		}
	}

	public override List<XElement> Serialize ()
	{
		return new List<XElement> () {
			new XElement("Rotator",rotator.Serialize())
		};
	}
}

