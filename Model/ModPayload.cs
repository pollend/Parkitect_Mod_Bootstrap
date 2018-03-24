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

	public Dictionary<string,object> Serialize()
	{
		var payload = new Dictionary<string, object>();
		
		List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
		foreach (var t in ParkitectObjs)
		{
			Dictionary<string, object> e = t.Serialize();
			e.Add("Tag", ParkitectObj.GetTagFromParkitectObject(t.GetType()));
			items.Add(e);
		}
		payload.Add("ParkitectObjects",items);
		payload.Add("ModName",ModName);
		payload.Add("Description",Description);
		return payload;
	}

	public void Deserialize(Dictionary<String,object> entries)
	{
		if(entries.ContainsKey("ModName"))
			ModName = (string) entries["ModName"];
		if(entries.ContainsKey("Description"))
			Description = (string)entries["Description"];
		
		Debug.Log("Loading Mod: " + ModName);

		foreach (var e in (List<object>) entries["ParkitectObjects"])
		{
			Dictionary<string, object> entry = e as Dictionary<string, object>;
			Type type = ParkitectObj.FindByParkitectObjectByTag((string) entry["Tag"]);
			if (type == null)
			{
				Debug.Log("Can't Find ParkitectObject For Tag: " + entry["Tag"]);
			}
			else
			{
				ParkitectObj o = (ParkitectObj) CreateInstance(type);
				o.DeSerialize(entry);
				ParkitectObjs.Add(o);
			}
		}
	}

	

	public void onEnabled()
	{
	}

	public void onDisabled()
	{
	}

}
