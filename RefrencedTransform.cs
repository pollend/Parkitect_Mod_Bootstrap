using System;
using UnityEngine;
using System.Collections.Generic;


[Serializable]
public class RefrencedTransform
{
	[SerializeField]
	private string key;

	[SerializeField]
	private Transform prefabTransform;

	[System.NonSerialized]
	private Transform cachedSceneRefrence;
	[System.NonSerialized]
	private Transform root;

	public Transform PrefabTransform
	{
		get
		{
			return prefabTransform;
		}
	}


	public void SetSceneTransform(Transform transform)
	{
		if (transform == null)
			return;
		Refrence refrence = null;
		if (transform.GetComponent<Refrence>() != null)
			refrence = transform.gameObject.GetComponent<Refrence>();
		else
			refrence = transform.gameObject.AddComponent<Refrence>();

		if (key != refrence.Key)
			root = null;
		key = refrence.Key;

	}

	public Transform FindSceneRefrence(Transform root)
	{
		if (this.root != root || cachedSceneRefrence == null)
		{
			this.root = root;
			cachedSceneRefrence = Refrence.findTransformByKey(root, key);
		}
		return cachedSceneRefrence;
	}

	public void UpdatePrefabRefrence(Transform root)
	{
		if (this.root != null)
		{
			this.prefabTransform = Refrence.findTransformByKey(root, key);
		}
	}

	struct pairs{
		String element;
		int index;
	}

	public Transform Deserialize(Transform root,Dictionary<string,object> element)
	{
		Transform current = null;
		foreach(var e in element)
		{
			if (current == null) {
				current = root;
			} else {
				current = current.GetChild ((int)(long)e.Value);
			}
		}

		return current;

	}

	public Dictionary<string,object> Serialize (Transform root)
	{
		Dictionary<string,object> elements = new Dictionary<string,object>();
		if(root != null)
		{
			Transform current = Refrence.findTransformByKey(root, key);
			do
			{
				elements.Add(current.name,current.GetSiblingIndex());
				if(current == root)
					break;
				current = current.parent;
			}
			while(current != null);
		}
		//element.Reverse ();
		return elements;
	}

}


