using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SPBoundingBox
{
	public Bounds bounds;
	private Bounds liveBounds;

	public Dictionary<string, object> Serialize()
	{
		return new Dictionary<string, object>()
		{
			{"XMin", bounds.min.x},
			{"XMax", bounds.max.x},
			{"YMin", bounds.min.y},
			{"YMax", bounds.max.y},
			{"ZMin", bounds.min.z},
			{"ZMax", bounds.max.z}
		};

	}

	public static SPBoundingBox Deserialize(Dictionary<string, object> element)
	{
		Bounds b = new Bounds ();
		Vector3 min = new Vector3 ();
		if (element.ContainsKey ("XMin") )
			min.x = Convert.ToSingle(element["XMin"]);
		if (element.ContainsKey ("YMin"))
			min.y = Convert.ToSingle(element["YMin"]);
		if (element.ContainsKey ("ZMin"))
			min.z = Convert.ToSingle(element["ZMin"]);

		Vector3 max = new Vector3 ();
		if (element.ContainsKey ("XMax") )
			max.x = Convert.ToSingle(element["XMax"]);
		if (element.ContainsKey ("YMax"))
			max.y = Convert.ToSingle(element["YMax"]);
		if (element.ContainsKey ("ZMax"))
			max.z = Convert.ToSingle(element["ZMax"]);

		b.min = min;
		b.max = max;

		SPBoundingBox box = new SPBoundingBox ();
		box.bounds = b;
		return box;
	
	}
}
