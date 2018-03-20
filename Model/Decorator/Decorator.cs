using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Decorator : ScriptableObject
{
	public Decorator()
	{
	}

	public virtual String[] getAssets()
	{
		return new string[] { };
	}
	public virtual void RenderSceneGui(ParkitectObj parkitectObj) { }
	public virtual void RenderInspectorGui(ParkitectObj parkitectObj) { }
	public virtual void CleanUp(ParkitectObj parkitectObj) { }
	public virtual void Load(ParkitectObj parkitectObj) { }
	public virtual void PrepareExport(ParkitectObj parkitectObj) { }
	public virtual Dictionary<string,object> Serialize(ParkitectObj parkitectObj){return null;}
	public virtual void Deserialize( Dictionary<string,object> elements){}
#if PARKITECT
	public virtual void Decorate(GameObject go, GameObject hider,ParkitectObj parkitectObj, AssetBundle bundle){}
#endif
}

