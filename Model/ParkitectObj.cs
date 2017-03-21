using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


[Serializable]
public class ParkitectObj : ScriptableObject
{
	//Basic
	[SerializeField]
	private List<Decorator> decorators = new List<Decorator>();
	[SerializeField]
	private GameObject prefab;



	[SerializeField]
	public string key;

	public float XSize;

	[System.NonSerialized]
	public GameObject sceneRef;

	public string getKey { get { return key; } }
	public GameObject Prefab { get { return prefab; } }

    public  AssetBundle Bundle { get; set; }

	#if UNITY_EDITOR
	public void UpdatePrefab()
	{
		var refrence = getGameObjectRef (false);
		if (refrence != null) {
			prefab = PrefabUtility.ReplacePrefab(refrence, PrefabUtility.GetPrefabParent(refrence), ReplacePrefabOptions.ConnectToPrefab);
		}
		LoadDecorators ();
	}

	public GameObject getGameObjectRef(bool createInstance)
	{
		GameObject[] rootSceneNodes =  EditorSceneManager.GetActiveScene ().GetRootGameObjects ();
		for (int x = 0; x < rootSceneNodes.Length; x++) {
			var gameObject = rootSceneNodes [x].transform.Find (this.key);
			if (gameObject != null)
				return gameObject.parent.gameObject;
		}

		if (createInstance) {
			GameObject refrence = Instantiate (Prefab);
			refrence.name = Prefab.name;
			PrefabUtility.ConnectGameObjectToPrefab (refrence, Prefab);

			return getGameObjectRef(false);
		}

		return null;
	}

	public void LoadDecorators()
	{
		for (int i = 0; i < decorators.Count; i++) {
			decorators [i].Load (this);
		}
	}


	public virtual GameObject SetGameObject(GameObject g)
	{
		Transform gameRef =  null;

		for(int i = 0; i < g.transform.childCount; i++)
		{
			if (g.transform.GetChild (i).name.StartsWith ("pkref-")) {
				gameRef = g.transform.GetChild (i);
				break;
			}
		}

		if (gameRef == null) {
			gameRef = new GameObject ("pkref-" + System.Guid.NewGuid().ToString()).transform;
			gameRef.transform.parent = g.transform;

		}

		var path = "Assets/Resources/" + g.name + ".prefab";

		this.key = gameRef.name;
		GameObject prefab =  PrefabUtility.CreatePrefab (path, g);
		PrefabUtility.ConnectGameObjectToPrefab (g, prefab);


		this.prefab = prefab;
		EditorUtility.SetDirty (this);
		return prefab;
	}
	#endif

	public void Load(ParkitectObj parkitectObj)
	{
		this.decorators = parkitectObj.decorators;
		this.prefab = parkitectObj.prefab;
		this.key = parkitectObj.getKey;

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
		this.UpdatePrefab ();
		for (int x = 0; x < decorators.Count; x++) {
			decorators [x].PrepareExport (this);
		}
	}

	public Decorator GetDecorator(Type t,bool createInstance)
	{
		
		for (int x = 0; x < decorators.Count; x++)
		{
			if (decorators [x] != null) {
				if (decorators [x].GetType ().Equals (t)) {
					return decorators [x];
				}
			}
		}

		if (createInstance) {
			Decorator dec = (Decorator)ScriptableObject.CreateInstance (t);

			AssetDatabase.AddObjectToAsset (dec, this);
			EditorUtility.SetDirty (dec);
			AssetDatabase.SaveAssets ();
			if (!EditorSceneManager.GetActiveScene ().isDirty)
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		

			decorators.Add (dec);
			return dec;
		}
		return null;
	}

	public void GetAssetbundlePaths(List<string> path)
	{
		path.Add (AssetDatabase.GetAssetPath (this.prefab));
	}

	public void CleanUp()
	{
		
	    for (int x = 0; x < decorators.Count; x++) {
			if (decorators [x] != null) {
				decorators [x].CleanUp (this);
				UnityEngine.Object.DestroyImmediate (decorators [x], true);
			}
	    }
		decorators.Clear ();
	}

	#endif

	public List<XElement> Serialize()
	{
		List<XElement> elements = new List<XElement> ();
		for (int i = 0; i < decorators.Count; i++) {
			elements.Add (new XElement (decorators[i].GetType().ToString(),decorators[i].Serialize(this)));
		}

		return new List<XElement>(new XElement[]{ 
			new XElement("Decorators",elements),
			new XElement("Prefab",Prefab.name),
		    new XElement("Key", this.key),
		});
	}


    public T  LoadAsset<T>(string prefabName) where T : UnityEngine.Object
    {
        try
        {
            T asset;
            asset = Bundle.LoadAsset<T>(prefabName);
            Bundle.Unload(false);
            return asset;

        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
            return null;
        }
    }

	public void DeSerialize(XElement element)
	{
        var prefabElement = element.Element ("Prefab");
        if (prefabElement != null) prefab = LoadAsset<GameObject>( prefabElement.Value);

	    var keyElement = element.Element("key");
	    if (keyElement != null) key = keyElement.Value;

        var decoratorElement = element.Element("Decorators");
	    if (decoratorElement != null)
	    {
	        foreach (var decorator in decoratorElement.Elements())
	        {
	            Decorator dec = Utility.GetByTypeName<Decorator>(decorator.Name.LocalName);
	            dec.Deserialize(decorator);
	            this.decorators.Add(dec);
	        }
	    }

	}

    public T DecoratorByInstance<T>() where T : Decorator
    {
        return (T)decorators.SingleOrDefault(x => x.GetType() == typeof(T));
    }

#if (!UNITY_EDITOR)

    public virtual void BindToParkitect()
    {
        AssetManager.Instance.registerObject(this.prefab);
    }
#endif
}


