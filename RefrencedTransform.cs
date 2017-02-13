using System;
using UnityEngine;

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


	public void SetSceneTransform(Transform transform)
	{
		if (transform == null)
			return;
		Refrence refrence = null;
		if (transform.GetComponent<Refrence> () != null)
			refrence = transform.gameObject.GetComponent<Refrence> ();
		else
			refrence = transform.gameObject.AddComponent<Refrence> ();
		
		if (key != refrence.Key)
			root = null;
		key = refrence.Key;
	
	}



	public Transform FindSceneRefrence(Transform root)
	{
		if (this.root != root || cachedSceneRefrence == null) {
			this.root = root;
			cachedSceneRefrence = Refrence.findTransformByKey (root, key);
		}
		return cachedSceneRefrence;
	}

	public void UpdatePrefabRefrence(Transform root)
	{
		if (this.root != null)
		{
			this.prefabTransform =  Refrence.findTransformByKey (root, key);
		}
	}
}


