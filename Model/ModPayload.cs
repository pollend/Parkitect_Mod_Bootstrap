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
	public ParkitectObj SelectedParkitectObject;

	[SerializeField]
	public string ModName;
	[SerializeField]
	public string Description;

	public Dictionary<String,Object> Serialize()
	{
		Dictionary<String,Object> payload = new Dictionary<string, object>();
		
		List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
		foreach (var t in ParkitectObjs)
		{
			Dictionary<string, object> e = t.Serialize();
			e.Add("tag", ParkitectObj.GetTagFromParkitectObject(t.GetType()));
			items.Add(e);
		}
		payload.Add("ParkitectObjects",items);
		payload.Add("ModName",ModName);
		payload.Add("Description",Description);
		return payload;
	}

	public void Deserialize(Dictionary<String,object> entries)
	{

		foreach (var e in (List<Dictionary<String, object>>) entries["ParkitectObjects"])
		{
			ParkitectObj o = (ParkitectObj) Activator.CreateInstance(ParkitectObj.FindByParkitectObjectByTag((string) e["tag"]));
			o.DeSerialize(e);
			ParkitectObjs.Add(o);
		}

		if(entries.ContainsKey("ModName"))
			ModName = (string) entries["ModName"];
		if(entries.ContainsKey("Description"))
			Description = (string)entries["Description"];
	}

	

	public void onEnabled()
	{
	}

	public void onDisabled()
	{
	}

}
