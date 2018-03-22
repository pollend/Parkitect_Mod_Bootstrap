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

	[NonSerialized] private List<int> _index;

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
		if (_key == null && _index != null)
		{
			Transform current = null;
			foreach (var e in _index)
			{
				if (current == null)
				{
					current = root;
				}
				else
				{
					current = current.GetChild(e);
				}
			}
			
			Refrence refs = current.gameObject.AddComponent<Refrence>();
			_key = refs.Key;
			GetCachedTransform(root).SetTransform(_key,current);
		}

		if (root == null)
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

	public void Deserialize(List<object> element)
	{
		_index = new List<int>();
		
		foreach(var e in element)
		{
			_index.Add(Convert.ToInt32(e));
		}
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


