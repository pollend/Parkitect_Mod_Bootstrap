using System;


#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

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

	public Dictionary<String,Object> Serialize()
	{
		Dictionary<String,Object> payload = new Dictionary<string, object>();
		
		Dictionary<String,Object> items = new Dictionary<string, object>();
		for (int i = 0; i < ParkitectObjs.Count; i++) {
			items.Add (ParkitectObjs [i].GetType ().ToString (), ParkitectObjs [i].Serialize ());
		}
		
		payload.Add("ParkitectObjects",items);
		payload.Add("ModName",modName);
		payload.Add("Description",description);
		return payload;
	}

	public void Deserialize(Dictionary<String,object> entries)
	{
		ParkitectObjectType type = new ParkitectObjectType();

		foreach (var e in (Dictionary<String, object>) entries["ParkitectObjects"])
		{
			ParkitectObj o = (ParkitectObj) Activator.CreateInstance(type.GetType(e.Key));
			o.DeSerialize((Dictionary<String, object>) e.Value);
			ParkitectObjs.Add(o);
		}
		

		if(entries.ContainsKey("ModName"))
			modName = (string) entries["ModName"];
		if(entries.ContainsKey("Description"))
			description = (string)entries["Description"];
	}


	public void onEnabled()
	{
	}

	public void onDisabled()
	{
	}

}
