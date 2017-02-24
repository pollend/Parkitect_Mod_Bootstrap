using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;
using System.Linq;
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


	public static Vector2 DeseralizeVector2(XElement element)
	{

		Vector2 v = new Vector2 ();
		v.x = float.Parse(element.Element ("X").Value);
		v.y = float.Parse(element.Element ("Y").Value);
		return v;
	}

	public static List<XElement> SerializeVector(Vector3 v)
	{
		return new List<XElement> (new XElement[] {
			new XElement("X",v.x),
			new XElement("Y",v.y),
			new XElement("Z",v.z)
		});
	}

	public static Vector3 DeseralizeVector3(XElement element)
	{
		Vector3 v = new Vector3 ();
		v.x = float.Parse(element.Element ("X").Value);
		v.y = float.Parse(element.Element ("Y").Value);
		v.z = float.Parse(element.Element ("Z").Value);
		return v;

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

	public static List<XElement> SerializeQuaternion(Quaternion quaternion)
	{
		return new List<XElement> (new XElement[] {
			new XElement("X",quaternion.x),
			new XElement("Y",quaternion.y),
			new XElement("Z",quaternion.z),
			new XElement("W",quaternion.w)
		});
	}

	public static Color DeSerializeColor(XElement element)
	{
		Color c = new Color ();
		c.r = float.Parse(element.Element ("R").Value);
		c.g = float.Parse(element.Element ("G").Value);
		c.b = float.Parse(element.Element ("B").Value);
		c.a = float.Parse(element.Element ("A").Value);
		return c;

	}

	public static T GetByTypeName<T>(string name)
	{
		IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies ().Where ((Assembly assembly) => assembly.FullName.Contains ("Assembly"));
		foreach (Assembly assembly in scriptAssemblies) 
		{
			foreach (Type type in assembly.GetTypes().Where(subType => subType.IsClass && subType.IsSubclassOf(typeof(T))))
			{
				if (type.Name == name) {
					return (T)Activator.CreateInstance (type);
				}
			}
		}
		return default(T);
	}

}


