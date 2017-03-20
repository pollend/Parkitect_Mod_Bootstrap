#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Linq;


#endif
using System;
using UnityEngine;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
[Serializable]
public class SPApplyRotation : SPRideAnimationEvent
{
	[SerializeField]
	public SPMultipleRotations rotator;
	float lastTime;


	public override string EventName
	{
		get
		{
			return "ApplyRotations";
		}
	}
#if UNITY_EDITOR
public override void RenderInspectorGUI(SPMotor[] motors)
{

    if (rotator)
    {
        ColorIdentifier = rotator.ColorIdentifier;
    }
	foreach (SPMultipleRotations R in motors.OfType<SPMultipleRotations>().ToList())
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

	}
	public override void Run(Transform root)
	{
		if (rotator)
		{
			rotator.tick(Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			done = true;
			base.Run(root);
		}

	}


	public override List<XElement> Serialize (Transform root)
	{
		return new List<XElement> (new XElement[] {
			new XElement("rotator",rotator.Serialize(root))
		});
	}

}
