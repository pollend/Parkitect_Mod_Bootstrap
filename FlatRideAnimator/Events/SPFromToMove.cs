#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
[Serializable]
public class SPFromToMove : SPRideAnimationEvent
{
	public SPMover mover;

	float lastTime;
	public override string EventName {
		get {
			return "From-To Move";
		}
	}

#if UNITY_EDITOR
	public override void RenderInspectorGUI(SPMotor[] motors)
	{
		if (mover) {
			ColorIdentifier = mover.ColorIdentifier;
		}
		foreach (SPMover R in motors.OfType<SPMover>().ToList()) {
			if (R == mover)
				GUI.color = Color.red / 1.3f;
			if (GUILayout.Button (R.Identifier)) {
				mover = R;
			}
			GUI.color = Color.white;
		}
		base.RenderInspectorGUI (motors);
	}
#endif

	public override void Enter()
	{
		lastTime = Time.realtimeSinceStartup;

		mover.startFromTo ();
		base.Enter ();
	}
	public override void Run(Transform root)
	{
		if (mover) {
			mover.tick (Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			if (mover.reachedTarget ()) {
				done = true;
			}
			base.Run (root);
		}

	}


	public override void Deserialize (XElement elements)
	{
		if (elements.Element ("mover") != null) {
			this.mover = new SPMover ();
			mover.Deserialize (elements.Element ("mover"));
		}
		base.Deserialize (elements);
	}

	public override List<XElement> Serialize (Transform root)
	{
		if (mover == null)
			return null;
		return new List<XElement> (new XElement[] {
			new XElement ("mover", mover.Serialize (root)),
		});
	}
}

