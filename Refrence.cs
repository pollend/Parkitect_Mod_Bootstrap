using System;
using UnityEngine;

public class Refrence : MonoBehaviour
{
	[SerializeField]
	private string refrence;

	public string GetRefrence()
	{
		return refrence;
	}

	public String Key
	{
		get
		{
			if (string.IsNullOrEmpty(refrence))
				refrence = Guid.NewGuid().ToString();
			return refrence;
		}
	}
		
	public static Transform FindTransformByKey(Transform root, String key)
	{

		for (int i = 0; i < root.childCount; i++)
		{
			var temp = root.GetChild(i);
			var refrence = temp.gameObject.GetComponent<Refrence>();

			if (refrence != null)
			{
				if (refrence.Key == key)
					return temp;
			}
			Transform result = FindTransformByKey(temp, key);
			if (result != null)
				return result;
		}

		var refr = root.gameObject.GetComponent<Refrence>();
		if (refr != null)
		{
			if (refr.Key == key)
			{
				return root;
			}
		}
		return null;
	}

}

