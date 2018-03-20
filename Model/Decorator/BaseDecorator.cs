#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BaseDecorator : Decorator
{
	[SerializeField]
		public string InGameName;

		[SerializeField]
		public float price;

#if UNITY_EDITOR

    public override void RenderInspectorGui (ParkitectObj parkitectObj)
	{
		InGameName = EditorGUILayout.TextField("In Game name: ", InGameName);
		price = EditorGUILayout.FloatField("Price: ", price);
        base.RenderInspectorGui (parkitectObj);
	}
#endif

	public override Dictionary<string,object> Serialize (ParkitectObj parkitectObj)
	{
		return new Dictionary<string, object>
		{
			{"InGameName",InGameName},
			{"Price",price}
		};
	}

	public override void Deserialize (Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("InGameName") )
			InGameName = (string) elements["InGameName"];
		if (elements.ContainsKey ("Price") )
			price = (float)(double)elements["Price"];
		
		base.Deserialize (elements);
	}

#if PARKITECT
	public override void Decorate(GameObject go, GameObject hider, ParkitectObj parkitectObj, AssetBundle bundle)
	{
		BuildableObject component = go.GetComponent<BuildableObject>();
		component.price = price;
		component.setDisplayName(InGameName);
	}
#endif
	
}

