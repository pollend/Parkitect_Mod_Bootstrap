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
}
