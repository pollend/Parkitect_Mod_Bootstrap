using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;


[Serializable]
public class SPWaypoint
{
	public bool isOuter;

	public bool isRabbitHoleGoal;

	public Vector3 localPosition;

	public List<int> connectedTo = new List<int>();

	public Vector3 getWorldPosition(Transform pivot)
	{
		return pivot.position + pivot.rotation * this.localPosition;
	}

	public List<XElement> Serialize ()
	{
		List<XElement> xmlConnected = new List<XElement> ();
		for (int i = 0; i < connectedTo.Count; i++) {
			xmlConnected.Add (new XElement ("I", connectedTo [i]));
		}

		return new List<XElement> (new XElement[] {
			new XElement("Outer",isOuter),
			new XElement("IsRabbitHoleGoal",isRabbitHoleGoal),
			new XElement("LocalPosition",Utility.SerializeVector(localPosition)),
			new XElement("ConnectedTo",xmlConnected)		
		});
	}

}
