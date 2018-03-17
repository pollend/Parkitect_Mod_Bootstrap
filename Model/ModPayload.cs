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

	public List<XElement> Serialize()
	{
		List<XElement> xmlParkitectObjs = new List<XElement> ();
		for (int i = 0; i < ParkitectObjs.Count; i++) {
			xmlParkitectObjs.Add (new XElement (ParkitectObjs [i].GetType ().ToString (), ParkitectObjs [i].Serialize ()));
		}
		return xmlParkitectObjs;
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
