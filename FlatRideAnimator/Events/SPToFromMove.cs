#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
[Serializable]
public class SPToFromMove : SPRideAnimationEvent
{
	public SPMover mover;

	float lastTime;
	public override string EventName {
		get {
			return "To-From Move";
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

		mover.StartToFrom ();
		base.Enter ();
	}
	public override void Run(Transform root)
	{
		if (mover) {
			mover.Tick (Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			if (mover.ReachedTarget ()) {
				Done = true;
			}
			base.Run (root);
		}
	}
	public override void Deserialize (Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("rotator") ) {
			this.mover = new SPMover ();
			mover.Deserialize ((Dictionary<string, object>) elements["rotator"]);
		}
		
		base.Deserialize (elements);
	}
	public override Dictionary<string,object> Serialize (Transform root)
	{
		if (mover == null)
			return null;
		return new Dictionary<string, object>{
			{"rotator", mover.Serialize (root)}
		};
	}


}
