using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


[Serializable]
public class ParkitectObj : ScriptableObject
{
	//Basic
	[SerializeField] private List<Decorator> decorators = new List<Decorator>();
	[SerializeField] public GameObject Prefab;
	[SerializeField] public string Key;

	public float XSize;

	[NonSerialized] 
	public GameObject sceneRef;

#if UNITY_EDITOR
	public void UpdatePrefab()
	{
		var refrence = getGameObjectRef(false);
		if (refrence != null)
		{
			Prefab = PrefabUtility.ReplacePrefab(refrence, PrefabUtility.GetPrefabParent(refrence),
				ReplacePrefabOptions.ConnectToPrefab);
		}

		LoadDecorators();
	}

	public GameObject getGameObjectRef(bool createInstance)
	{
		GameObject[] rootSceneNodes = EditorSceneManager.GetActiveScene().GetRootGameObjects();
		for (int x = 0; x < rootSceneNodes.Length; x++)
		{
			var gameObject = rootSceneNodes[x].transform.Find(this.Key);
			if (gameObject != null)
				return gameObject.parent.gameObject;
		}

		if (createInstance)
		{
			GameObject refrence = Instantiate(Prefab);
			refrence.name = Prefab.name;
			PrefabUtility.ConnectGameObjectToPrefab(refrence, Prefab);

			return getGameObjectRef(false);
		}

		return null;
	}

	public void LoadDecorators()
	{
		for (int i = 0; i < decorators.Count; i++)
		{
			decorators[i].Load(this);
		}
	}


	public virtual GameObject SetGameObject(GameObject g)
	{
		Transform gameRef = null;

		for (int i = 0; i < g.transform.childCount; i++)
		{
			if (g.transform.GetChild(i).name.StartsWith("pkref-"))
			{
				gameRef = g.transform.GetChild(i);
				break;
			}
		}

		if (gameRef == null)
		{
			gameRef = new GameObject("pkref-" + System.Guid.NewGuid().ToString()).transform;
			gameRef.transform.parent = g.transform;

		}

		var path = "Assets/Resources/" + g.name + ".prefab";

		this.Key = gameRef.name;
		GameObject prefab = PrefabUtility.CreatePrefab(path, g);
		PrefabUtility.ConnectGameObjectToPrefab(g, prefab);


		this.Prefab = prefab;
		EditorUtility.SetDirty(this);
		return prefab;
	}
#endif

	public void Load(ParkitectObj parkitectObj)
	{
		decorators = parkitectObj.decorators;
		Prefab = parkitectObj.Prefab;
		Key = parkitectObj.Key;

		for (int x = 0; x < decorators.Count; x++)
		{
			decorators[x].Load(this);
		}

	}

	public virtual Type[] SupportedDecorators()
	{
		return new Type[] { };
	}


#if UNITY_EDITOR
	public virtual void PrepareForExport()
	{
		this.UpdatePrefab();
		for (int x = 0; x < decorators.Count; x++)
		{
			decorators[x].PrepareExport(this);
		}
	}

	public String[] getAssetPaths()
	{
		List<String> paths = new List<string>();
		for (int x = 0; x < decorators.Count; x++)
		{
			paths.AddRange(decorators[x].getAssets());
		}

		return paths.ToArray();
	}

	public Decorator GetDecorator(Type t, bool createInstance)
	{

		for (int x = 0; x < decorators.Count; x++)
		{
			if (decorators[x] != null)
			{
				if (decorators[x].GetType().Equals(t))
				{
					return decorators[x];
				}
			}
		}

		if (createInstance)
		{
			Decorator dec = (Decorator) ScriptableObject.CreateInstance(t);

			AssetDatabase.AddObjectToAsset(dec, this);
			EditorUtility.SetDirty(dec);
			AssetDatabase.SaveAssets();
			if (!EditorSceneManager.GetActiveScene().isDirty)
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());


			decorators.Add(dec);
			return dec;
		}

		return null;
	}

	public void CleanUp()
	{

		for (int x = 0; x < decorators.Count; x++)
		{
			if (decorators[x] != null)
			{
				decorators[x].CleanUp(this);
				UnityEngine.Object.DestroyImmediate(decorators[x], true);
			}
		}

		decorators.Clear();
	}

#endif

	public List<XElement> Serialize()
	{
		List<XElement> elements = new List<XElement>();
		for (int i = 0; i < decorators.Count; i++)
		{
			elements.Add(new XElement(decorators[i].GetType().ToString(), decorators[i].Serialize(this)));
		}

		return new List<XElement>(new[]
		{
			new XElement("Decorators", elements),
			new XElement("Key", Key)
		});
	}

	public void DeSerialize(XElement element)
	{
		var keyElement = element.Element("key");
		if (keyElement != null) Key = keyElement.Value;

		var decoratorElement = element.Element("Decorators");
		if (decoratorElement != null)
		{
			foreach (var decorator in decoratorElement.Elements())
			{
				Decorator dec = Utility.GetByTypeName<Decorator>(decorator.Name.LocalName);
				dec.Deserialize(decorator);
				decorators.Add(dec);
			}
		}
	}

	public String GetObjectTag()
	{
		object[] attributes = this.GetType().GetCustomAttributes(typeof(ParkitectObjectTag), false);
		return (attributes[0] as ParkitectObjectTag).Name;
	}

	public T DecoratorByInstance<T>() where T : Decorator
	{
		return (T) decorators.SingleOrDefault(x => x.GetType() == typeof(T));
	}

#if (PARKITECT)
	public virtual void BindToParkitect(GameObject hider, AssetBundle bundle)
	{
		throw new NotImplementedException();
	}
	
	public virtual void UnBindToParkitect(GameObject hider)
	{
		throw new NotImplementedException();
	}
	
#endif

}


