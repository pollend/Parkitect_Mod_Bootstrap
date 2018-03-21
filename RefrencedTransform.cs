using System;
using UnityEngine;
using System.Collections.Generic;


[Serializable]
public class RefrencedTransform
{
	[SerializeField]
	private string _key;

	[SerializeField]
	private Transform _prefabTransform;

	[System.NonSerialized]
	private Transform _cachedSceneRefrence;
	[System.NonSerialized]
	private Transform _root;

	public Transform PrefabTransform
	{
		get
		{
			return _prefabTransform;
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

		if (_key != refrence.Key)
			_root = null;
		_key = refrence.Key;

	}

	public Transform FindSceneRefrence(Transform root)
	{
		if (this._root != root || _cachedSceneRefrence == null)
		{
			this._root = root;
			_cachedSceneRefrence = Refrence.FindTransformByKey(root, _key);
		}
		return _cachedSceneRefrence;
	}

	public void UpdatePrefabRefrence(Transform root)
	{
		if (this._root != null)
		{
			this._prefabTransform = Refrence.FindTransformByKey(root, _key);
		}
	}

	struct Pairs{
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
				current = current.GetChild (Convert.ToInt32(e.Value));
			}
		}

		return current;

	}

	public Dictionary<string,object> Serialize (Transform root)
	{
		Dictionary<string,object> elements = new Dictionary<string,object>();
		if(root != null)
		{
			Transform current = Refrence.FindTransformByKey(root, _key);
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


