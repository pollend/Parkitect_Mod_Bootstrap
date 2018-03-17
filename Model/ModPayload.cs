using System;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModPayload : ScriptableSingleton<ModPayload>
{
	[SerializeField]
	public List<ParkitectObj> ParkitectObjs = new List<ParkitectObj>();

	[SerializeField] 
	public ParkitectObj selectedParkitectObject;

	[SerializeField]
	public string modName;
	[SerializeField]
	public string description;

    public AssetBundle Bundle { get; set; }

	public List<XElement> Serialize()
	{
		List<XElement> xmlParkitectObjs = new List<XElement> ();
		for (int i = 0; i < ParkitectObjs.Count; i++) {
			xmlParkitectObjs.Add (new XElement (ParkitectObjs [i].GetType ().ToString (), ParkitectObjs [i].Serialize ()));
		}
		return xmlParkitectObjs;
	}

    public void Deserialize(XElement element)
    {
        ParkitectObjectType type = new ParkitectObjectType();

        foreach (XElement e in element.Elements())
        {
            ParkitectObj o = (ParkitectObj)Activator.CreateInstance(type.GetType(e.Name.NamespaceName));
            o.Bundle = Bundle;
            o.DeSerialize(e);
            ParkitectObjs.Add(o);
        }

    }

    public void bind()
    {
        foreach (var o in ParkitectObjs)
        {
            BaseDecorator dec = o.DecoratorByInstance<BaseDecorator>();
            if (dec != null)
                Debug.Log("---------------------------" + dec.InGameName + "---------------------------");
        }
    }


#if UNITY_EDITOR
	public void GetAssetbundlePaths(List<string> path)
	{
		path.Add (AssetDatabase.GetAssetPath (this));
		for (int x = 0; x < ParkitectObjs.Count; x++) {
			ParkitectObjs [x].UpdatePrefab ();

			ParkitectObjs [x].GetAssetbundlePaths (path);
		}
	}
#endif
}
