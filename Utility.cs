using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;

public static class Utility
{
	public static void recursiveFindTransformsStartingWith(string name, Transform parentTransform, List<Transform> transforms)
	{
		Transform[] componentsInChildren = parentTransform.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Transform transform = componentsInChildren[i];
			if (transform.name.StartsWith(name))
			{
				transforms.Add(transform);
			}
		}
	}

	public static void findAllChildrenWithName(Transform transform,String name,List<GameObject> collection)
	{
		for(int i = 0; i < transform.childCount;i++ ) {
			var temp  = transform.GetChild(i);
			if (temp.name == name) {
				collection.Add (temp.gameObject);
			}
			findAllChildrenWithName (temp, name, collection);
		}
	}

	public static List<XElement> SerializeVector(Vector2 v)
	{
		return new List<XElement> (new XElement[] {
			new XElement("X",v.x),
			new XElement("Y",v.y)
		});
		
	}
	public static List<XElement> SerializeVector(Vector3 v)
	{
		return new List<XElement> (new XElement[] {
			new XElement("X",v.x),
			new XElement("Y",v.y),
			new XElement("Z",v.z)
		});
	}

	public static List<XElement> SerializeColor(Color c)
	{
		return new List<XElement> (new XElement[] {
			new XElement("R",c.r),
			new XElement("G",c.g),
			new XElement("B",c.b),
			new XElement("A",c.a)
		});
	}



}


