﻿using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SPWaypoint
{
	public bool isOuter;

	public bool isRabbitHoleGoal;

	public Vector3 localPosition;

	public List<int> connectedTo = new List<int>();

	public Vector3 GetWorldPosition(Transform pivot)
	{
		return pivot.position + pivot.rotation * this.localPosition;
	}

	public Dictionary<string, object> Serialize()
	{
		return new Dictionary<string, object>()
		{
			{"Outer", isOuter},
			{"IsRabbitHoleGoal", isRabbitHoleGoal},
			{"LocalPosition", Utility.SerializeVector(localPosition)},
			{"ConnectedTo", connectedTo}
		};
	}


	public static SPWaypoint Deserialize(Dictionary<string,object> elements)
	{
		SPWaypoint waypoint = new SPWaypoint (); 

		if(elements.ContainsKey("Outer"))
			waypoint.isOuter = (bool) elements["Outer"];
		if(elements.ContainsKey ("IsRabbitHoleGoal") )
			waypoint.isRabbitHoleGoal = (bool) elements["IsRabbitHoleGoal"];
		if(elements.ContainsKey("LocalPosition"))
			waypoint.localPosition = Utility.DeseralizeVector3( (Dictionary<string, object>) elements["LocalPosition"]);
		
		if (elements.ContainsKey("ConnectedTo")) {
			
			foreach(var c in (List<object>)elements["ConnectedTo"])
			{
				waypoint.connectedTo.Add (Convert.ToInt32(c));	
			}
		}
		return waypoint;


	}
}
