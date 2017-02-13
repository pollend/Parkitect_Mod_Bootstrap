using System;
using System.Collections.Generic;
using UnityEngine;
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

}
