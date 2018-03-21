using System;
using UnityEngine;
using System.Collections.Generic;
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

	public static Dictionary<string, object> SerializeVector(Vector2 v)
	{
		return new Dictionary<string, object>
		{
			{"X", v.x},
			{"Y", v.y}
		};
	}


	public static Vector2 DeseralizeVector2(Dictionary<string,object> elements)
	{

		Vector2 v = new Vector2 ();
		v.x = Convert.ToSingle(elements["X"]);
		v.y = Convert.ToSingle(elements["Y"]);
		return v;
	}

	public static Dictionary<string, object> SerializeVector(Vector3 v)
	{
		return new Dictionary<string, object>()
		{
			{"X", v.x},
			{"Y", v.y},
			{"Z", v.z}
		};
	}

	public static Vector3 DeseralizeVector3(Dictionary<string,object> elements)
	{
		Vector3 v = new Vector3 ();
		v.x = Convert.ToSingle(elements["X"]);
		v.y = Convert.ToSingle(elements["Y"]);
		v.z = Convert.ToSingle(elements["Z"]);
		return v;

	}

	public static Dictionary<string, object> SerializeColor(Color c)
	{
		return new Dictionary<string, object>
		{
			{"R", c.r},
			{"G", c.g},
			{"B", c.b},
			{"A", c.a}
		};
	}

	public static Dictionary<string, object> SerializeQuaternion(Quaternion quaternion)
	{
		return new Dictionary<string, object>()
		{
			{"X", quaternion.x},
			{"Y", quaternion.y},
			{"Z", quaternion.z},
			{"W", quaternion.w}
		};
	}

	public static Color DeSerializeColor(Dictionary<string, object> element)
	{
		Color c = new Color ();
		c.r = Convert.ToSingle(element["R"]);
		c.g = Convert.ToSingle(element["G"]);
		c.b = Convert.ToSingle(element["B"]);
		c.a = Convert.ToSingle(element["A"]);
		return c;

	}


	public static Quaternion DeSerializeQuaternion(Dictionary<string, object> elements)
	{
		Quaternion quaternion = new Quaternion ();
		quaternion.x = Convert.ToSingle(elements["X"]);
		quaternion.y = Convert.ToSingle(elements["Y"]);
		quaternion.z = Convert.ToSingle(elements["Z"]);
		quaternion.w = Convert.ToSingle(elements["W"]);
		return quaternion;
	}

	public static T GetByTypeName<T>(string name)
	{
		IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies ();
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


