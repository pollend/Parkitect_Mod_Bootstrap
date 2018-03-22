#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
[Serializable]
[RideAnimationEventTag("To-FromMove")]
public class ToFromMove : RideAnimationEvent
{
	public Mover Mover;

	private float _lastTime;
	public override string EventName {
		get {
			return "To-From Move";
		}
	}

#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
	{
		if (Mover) {
			ColorIdentifier = Mover.ColorIdentifier;
		}
		foreach (Mover r in motors.OfType<Mover>().ToList()) {
			if (r == Mover)
				GUI.color = Color.red / 1.3f;
			if (GUILayout.Button (r.Identifier)) {
				Mover = r;
			}
			GUI.color = Color.white;
		}
		base.RenderInspectorGUI (motors);
	}
#endif

	public override void Enter()
	{
		_lastTime = Time.realtimeSinceStartup;

		Mover.StartToFrom ();
		base.Enter ();
	}
	public override void Run(Transform root)
	{
		if (Mover) {
			Mover.Tick (Time.realtimeSinceStartup - _lastTime, root);
			_lastTime = Time.realtimeSinceStartup;
			if (Mover.ReachedTarget ()) {
				Done = true;
			}
			base.Run (root);
		}
	}
	public override void Deserialize (Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("rotator") ) {
			Mover = new Mover ();
			Mover.Deserialize ((Dictionary<string, object>) elements["rotator"]);
		}
		
		base.Deserialize (elements);
	}
	public override Dictionary<string,object> Serialize (Transform root)
	{
		if (Mover == null)
			return null;
		return new Dictionary<string, object>{
			{"rotator", Mover.Serialize (root)}
		};
	}


}
