using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModPayload : ScriptableSingleton<ModPayload>
{
	[SerializeField]
	public List<ParkitectObj> ParkitectObjs;

	[SerializeField]
	public ParkitectObj selectedParkitectObject { get; set; }

	[SerializeField]
	public string modName;
	[SerializeField]
	public string description;

	public ModPayload()
	{
		if (ParkitectObjs == null)
			ParkitectObjs = new List<ParkitectObj>();
	}

#if UNITY_EDITOR
	public void GetAssetbundlePaths(List<string> path)
	{
		path.Add (AssetDatabase.GetAssetPath (this));
		for (int x = 0; x < ParkitectObjs.Count; x++) {
			ParkitectObjs [x].GetAssetbundlePaths (path);
		}
	}
#endif
}
