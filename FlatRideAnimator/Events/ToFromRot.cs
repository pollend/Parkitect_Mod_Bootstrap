﻿#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using System.Linq;
namespace Spark
{
	[ExecuteInEditMode]
	[Serializable]
	public class ToFromRot : RideAnimationEvent
	{
		public RotateBetween rotator;

		float lastTime;
		public override string EventName
		{
			get
			{
				return "To-From Rot";
			}
		}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
    {
        if (rotator)
        {
            ColorIdentifier = rotator.ColorIdentifier;
        }
		foreach (RotateBetween R in motors.OfType<RotateBetween>().ToList())
        {
            if (R == rotator)
                GUI.color = Color.red;
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

			rotator.startToFrom();
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
	}
}
