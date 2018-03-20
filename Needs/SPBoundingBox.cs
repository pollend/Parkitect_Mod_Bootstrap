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
			min.x = (float)(double)element["XMin"];
		if (element.ContainsKey ("YMin"))
			min.y = (float)(double)element["YMin"];
		if (element.ContainsKey ("ZMin"))
			min.z = (float)(double)element["ZMin"];

		Vector3 max = new Vector3 ();
		if (element.ContainsKey ("XMax") )
			max.x = (float)(double)element["XMax"];
		if (element.ContainsKey ("YMax"))
			max.y = (float)(double)element["YMax"];
		if (element.ContainsKey ("ZMax"))
			max.z = (float)(double)element["ZMax"];

		b.min = min;
		b.max = max;

		SPBoundingBox box = new SPBoundingBox ();
		box.bounds = b;
		return box;
	
	}
}
