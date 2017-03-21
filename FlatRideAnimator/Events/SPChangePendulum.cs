#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

[ExecuteInEditMode]
[Serializable]
public class SPChangePendulum : SPRideAnimationEvent
{
	[SerializeField]
	public SPPendulumRotator rotator;
	public float friction = 20f;
	public bool pendulum;

	#if UNITY_EDITOR
	float lastTime;
	private float startPendulumPosition;
	private float windUpAngleTarget = 100f;
	#endif

	public override string EventName
	{
		get
		{
			return "ChangePendulum";
		}
	}
	public override void Bootstrap()
	{
		base.Bootstrap();
	}

#if UNITY_EDITOR
public override void RenderInspectorGUI(SPMotor[] motors)
{

    if (rotator)
    {
        ColorIdentifier = rotator.ColorIdentifier;
        friction = EditorGUILayout.FloatField("Friction", friction);
        pendulum = EditorGUILayout.Toggle("Pendulum", pendulum);
    }
	foreach (SPPendulumRotator R in motors.OfType<SPPendulumRotator>().ToList())
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
		rotator.setActAsPendulum(pendulum);
		rotator.angularFriction = friction;
		done = true;
	}
	public override void Run(Transform root)
	{
		if (rotator)
		{

		}

	}

	public override void Deserialize (XElement elements)
	{
		if (elements.Element ("rotator") != null) {
			this.rotator = new SPPendulumRotator ();
			rotator.Deserialize (elements.Element ("rotator"));
		}
	
		if(elements.Element ("pendulum") != null)
			this.pendulum = bool.Parse(elements.Element ("pendulum").Value);
		if(elements.Element ("friction") != null)
			this.friction = float.Parse (elements.Element ("friction").Value);

		base.Deserialize (elements);
	}


	public override List<XElement> Serialize (Transform root)
	{
		if (rotator == null)
			return null;
		
		return new List<XElement> (new XElement[] {
			new XElement("rotator",rotator.Serialize(root)),
			new XElement("pendulum",pendulum),
			new XElement("friction",friction)
		});

	}
}
