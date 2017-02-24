using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

[Serializable]
public class SPBoundingBox
{
	public Bounds bounds;
	private Bounds liveBounds;

	public List<XElement> Serialize()
	{
		return new List<XElement>(new XElement[]{
			new XElement("XMin",bounds.min.x),
			new XElement("XMax",bounds.max.x),
			new XElement("YMin",bounds.min.y),
			new XElement("YMax",bounds.max.y),
			new XElement("ZMin",bounds.min.z),
			new XElement("ZMax",bounds.max.z)
		});
		
	}

	public static SPBoundingBox Deserialize(XElement element)
	{
		Bounds b = new Bounds ();
		Vector3 min = new Vector3 ();
		if (element.Element ("XMin") != null)
			min.x = float.Parse(element.Element ("XMin").Value);
		if (element.Element ("YMin") != null)
			min.y = float.Parse(element.Element ("YMin").Value);
		if (element.Element ("ZMin") != null)
			min.z = float.Parse(element.Element ("ZMin").Value);

		Vector3 max = new Vector3 ();
		if (element.Element ("XMax") != null)
			max.x = float.Parse(element.Element ("XMax").Value);
		if (element.Element ("YMax") != null)
			max.y = float.Parse(element.Element ("YMax").Value);
		if (element.Element ("ZMax")  != null)
			max.z = float.Parse(element.Element ("ZMax").Value);

		b.min = min;
		b.max = max;

		SPBoundingBox box = new SPBoundingBox ();
		box.bounds = b;
		return box;
	
	}
}
