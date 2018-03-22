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
[RideAnimationEventTag("SpinRotator")]
public class SpinRotater : RideAnimationEvent
{
	[SerializeField] public Rotator rotator;
	[SerializeField] public bool spin;
	[SerializeField] public float spins = 1;
	float lastTime;


	public override string EventName
	{
		get { return "SpinRotator"; }
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
	{
		if (rotator)
		{
			ColorIdentifier = rotator.ColorIdentifier;
			spin = EditorGUILayout.Toggle("amountOfSpins ", spin);
			if (spin)
				spins = EditorGUILayout.FloatField("spins ", spins);

			EditorGUILayout.LabelField("Amount " + rotator.GetRotationsCount());
		}

		foreach (Rotator R in motors.OfType<Rotator>().ToList())
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
		lastTime = Time.realtimeSinceStartup;
		rotator.ResetRotations();
		base.Enter();
	}

	public override void Run(Transform root)
	{
		if (rotator)
		{
			rotator.tick(Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			if (spin)
			{
				if (rotator.GetRotationsCount() >= spins)
				{
					Done = true;
				}
			}
			else
			{
				Done = true;
			}

			base.Run(root);
		}
	}

	public override void Deserialize(Dictionary<string, object> elements)
	{
		if (elements.ContainsKey("rotator"))
		{
			rotator = new Rotator();
			rotator.Deserialize((Dictionary<string, object>) elements["rotator"]);
		}

		if (elements.ContainsKey("spin"))
		{
			spin = (bool) elements["spin"];
		}

		if (elements.ContainsKey("spins"))
		{
			spins = Convert.ToInt32(elements["spins"]);
		}

		base.Deserialize(elements);
	}

	public override Dictionary<string, object> Serialize(Transform root)
	{
		if (rotator == null)
			return null;

		return new Dictionary<string, object>
		{
			{"rotator", rotator.Serialize(root)},
			{"spin", spin},
			{"spins", spins}
		};
	}
}
