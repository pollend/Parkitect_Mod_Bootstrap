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
	[SerializeField] public PendulumRotator rotator;
	public float friction = 20f;
	public bool pendulum;

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
		rotator.SetActAsPendulum(pendulum);
		rotator.AngularFriction = friction;
		Done = true;
	}

	public override void Run(Transform root)
	{
		if (rotator)
		{

		}

	}

	public override void Deserialize(Dictionary<string, object> elements)
	{
		if (elements.ContainsKey("rotator") )
		{
			rotator = new PendulumRotator();
			rotator.Deserialize((Dictionary<string, object>) elements["rotator"]);
		}

		if (elements.ContainsKey("pendulum") )
			pendulum = (bool) elements["pendulum"];
		if (elements.ContainsKey("friction") )
			friction = Convert.ToSingle(elements["friction"]);

		base.Deserialize(elements);
	}


	public override Dictionary<string, object> Serialize(Transform root)
	{
		if (rotator == null)
			return null;

		return new Dictionary<string, object>
		{
			{"rotator", rotator.Serialize(root)},
			{"pendulum", pendulum},
			{"friction", friction}
		};
	}
}
