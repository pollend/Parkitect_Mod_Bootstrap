using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
			var gameObject = rootSceneNodes[x].transform.Find(Key);
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
			gameRef = new GameObject("pkref-" + Guid.NewGuid()).transform;
			gameRef.transform.parent = g.transform;

		}

		var path = "Assets/Resources/" + g.name + ".prefab";

		Key = gameRef.name;
		GameObject prefab = PrefabUtility.CreatePrefab(path, g);
		PrefabUtility.ConnectGameObjectToPrefab(g, prefab);


		Prefab = prefab;
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
		UpdatePrefab();
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
			Decorator dec = (Decorator) CreateInstance(t);

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
				DestroyImmediate(decorators[x], true);
			}
		}

		decorators.Clear();
	}

#endif

	public Dictionary<String, object> Serialize()
	{
		Dictionary<String, object> dictDecorators = new Dictionary<String, object>();
		foreach (var decorator in decorators)
		{
			dictDecorators.Add(decorator.GetType().ToString(), decorator.Serialize(this));
		}

		Dictionary<String, object> result = new Dictionary<string, object> {{"Decorators", dictDecorators}, {"Key", Key}};
		return result;
	}

	public void DeSerialize(Dictionary<string, object> element)
	{
		if(element.ContainsKey("Key"))
			Key = (string)element["Key"];

		
		if (element.ContainsKey("Decorators"))
		{
			var decoratorElement =  element["Decorators"] as Dictionary<string,object>;
			
			foreach (var decorator in SupportedDecorators())
			{
				if (decoratorElement.ContainsKey(decorator.Name))
				{
					Decorator dec = (Decorator)CreateInstance(decorator);
					dec.Deserialize((Dictionary<string,object>)decoratorElement[decorator.Name]);
					decorators.Add(dec);
				}
				else
				{
					Debug.LogError("Can't find decorator: " + decorator.Name);
				}
					
			}
		}
	}

	public String GetObjectTag()
	{
		object[] attributes = GetType().GetCustomAttributes(typeof(ParkitectObjectTag), false);
		return (attributes[0] as ParkitectObjectTag).Name;
	}

	public T DecoratorByInstance<T>() where T : Decorator
	{
		return (T) decorators.SingleOrDefault(x => x.GetType() == typeof(T));
	}

	public static Type FindByParkitectObjectByTag(String tag)
	{
		IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies ();
		foreach (Assembly assembly in scriptAssemblies) 
		{
			foreach (Type type in assembly.GetTypes().Where(T => T.IsClass && T.IsSubclassOf(typeof(ParkitectObj))))
			{
				object[] nodeAttributes = type.GetCustomAttributes(typeof(ParkitectObjectTag), false);                    
				ParkitectObjectTag attr = nodeAttributes[0] as ParkitectObjectTag;
				if (attr != null) {
					if (attr.Name.Equals(tag))
					{
						return type;
					}
				}
			}
		}

		return null;
	}

	public static String GetTagFromParkitectObject(Type type)
	{
		object[] nodeAttributes = type.GetCustomAttributes(typeof(ParkitectObjectTag), false);
		ParkitectObjectTag attr = nodeAttributes[0] as ParkitectObjectTag;
		if (attr != null)
		{
			return attr.Name;
		}
		return null;
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


