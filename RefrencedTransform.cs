using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RefrencedTransform
{
	[SerializeField]
	private string _key;
	[SerializeField]
	private Transform _prefabTransform;
	[NonSerialized]
	private Transform _root;

	private CachedTransform _cachedTransform;


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

	public CachedTransform GetCachedTransform(Transform root)
	{
		if(root == null)
			return null;

		CachedTransform transform = root.gameObject.GetComponent<CachedTransform>();
		if (transform == null)
			return root.gameObject.AddComponent<CachedTransform>();
		return transform;
	}

	public Transform FindSceneRefrence(Transform root)
	{
	
		CachedTransform cache = GetCachedTransform(root);

		if (!cache.ContainsKey(_key))
		{
			cache.SetTransform(_key, Refrence.FindTransformByKey(root, _key));
		}
		return cache.GetCachedTransform(_key);
	}

	public void UpdatePrefabRefrence(Transform root)
	{
		CachedTransform cache = GetCachedTransform(root);

		if (_root != null)
		{
			_prefabTransform = Refrence.FindTransformByKey(root, _key);
			
			cache.SetTransform(_key,_prefabTransform);
		}
	}

	public Transform Deserialize(Transform root,List<object> element)
	{
		Transform current = null;
		foreach(var e in element)
		{
			if (current == null) {
				current = root;
			} else {
				current = current.GetChild (Convert.ToInt32(e));
			}
		}

		Refrence refs = current.gameObject.AddComponent<Refrence>();
		_key = refs.Key;
		GetCachedTransform(root).SetTransform(_key,current);
		
		return current;

	}

	public List<int> Serialize (Transform root)
	{
		List<int> elements = new List<int>();
		if(root != null)
		{
			Transform current = Refrence.FindTransformByKey(root, _key);
			do
			{
				elements.Add(current.GetSiblingIndex());
				if(current == root)
					break;
				current = current.parent;
			}
			while(current != null);
		}
		elements.Reverse();
		return elements;
	}

}


