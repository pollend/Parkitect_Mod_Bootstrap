using System;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModPayload : ScriptableSingleton<ModPayload>, IMod
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
		
		List<XElement> mod = new List<XElement>();
		mod.Add(new XElement("ParkitectObjects",xmlParkitectObjs));
		
		mod.Add(new XElement("ModName",modName));
		mod.Add(new XElement("Description",description));
		return mod;
	}

	public void Deserialize(XElement element)
	{
		ParkitectObjectType type = new ParkitectObjectType();

		foreach (XElement e in element.Elements("ParkitectObjects"))
		{
			ParkitectObj o = (ParkitectObj) Activator.CreateInstance(type.GetType(e.Name.NamespaceName));
			o.DeSerialize(e);
			ParkitectObjs.Add(o);
		}

		modName = element.Element("ModName").Value;
		description = element.Element("Description").Value;
	}


	public void onEnabled()
	{
		throw new NotImplementedException();
	}

	public void onDisabled()
	{
		throw new NotImplementedException();
	}

	public string Name => modName;
	public string Description => description;
	public string Identifier => modName.Replace(" ","_");
}
