using System;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;

[Serializable]
public class Decorator : ScriptableObject
{
	public Decorator()
	{
	}

	public virtual void RenderSceneGUI(ParkitectObj parkitectObj) { }
	public virtual void RenderInspectorGUI(ParkitectObj parkitectObj) { }
	public virtual void CleanUp(ParkitectObj parkitectObj) { }
	public virtual void Load(ParkitectObj parkitectObj) { }
	public virtual void PrepareExport(ParkitectObj parkitectObj) { }

	public virtual List<XElement> Serialize(){return null;}
	public virtual void Deserialize(){}
}

