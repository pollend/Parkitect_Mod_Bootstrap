#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
[RideAnimationEventTag("ChangePendulum")]
public class ChangePendulum : RideAnimationEvent
{
	[SerializeField] public PendulumRotator Rotator;
	public float Friction = 20f;
	public bool Pendulum;

#if UNITY_EDITOR
	float lastTime;
	private float startPendulumPosition;
	private float windUpAngleTarget = 100f;
#endif

	public override string EventName
	{
		get { return "ChangePendulum"; }
	}

	public override void Bootstrap()
	{
		base.Bootstrap();
	}

#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
	{

		if (rotator)
		{
			ColorIdentifier = rotator.ColorIdentifier;
			friction = EditorGUILayout.FloatField("Friction", friction);
			pendulum = EditorGUILayout.Toggle("Pendulum", pendulum);
		}

		foreach (PendulumRotator R in motors.OfType<PendulumRotator>().ToList())
		{
			if (R == rotator)
				GUI.color = Color.red / 1.3f;
			if (GUILayout.Button(R.Identifier))
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
		Rotator.SetActAsPendulum(Pendulum);
		Rotator.AngularFriction = Friction;
		Done = true;
	}

	public override void Run(Transform root)
	{
		if (Rotator)
		{

		}

	}

	public override void Deserialize(Dictionary<string, object> elements, Motor[] motors)
	{
		if (elements.ContainsKey("RotatorIndex"))
			Rotator = (PendulumRotator) motors[Convert.ToInt32(elements["RotatorIndex"])];

		if (elements.ContainsKey("pendulum"))
			Pendulum = (bool) elements["pendulum"];
		if (elements.ContainsKey("friction"))
			Friction = Convert.ToSingle(elements["friction"]);

		base.Deserialize(elements, motors);
	}


	public override Dictionary<string, object> Serialize(Transform root, Motor[] motors)
	{
		if (Rotator == null)
			return null;

		return new Dictionary<string, object>
		{
			{"RotatorIndex", Array.IndexOf(motors, Rotator)},
			{"pendulum", Pendulum},
			{"friction", Friction}
		};
	}
}
