using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class CategoryDecorator : Decorator
{
	public string Category;
	public String SubCategory;

#if UNITY_EDITOR
	public override void RenderInspectorGui(ParkitectObj parkitectObj)
	{
		category = EditorGUILayout.TextField("Category: ", category);
		subCategory = EditorGUILayout.TextField("Sub-Category: ", this.subCategory);

		base.RenderInspectorGui(parkitectObj);
	}
#endif

	public override Dictionary<string, object> Serialize(ParkitectObj parkitectObj)
	{
		return new Dictionary<string, object>
		{
			{"Category", Category},
			{"SubCategory", SubCategory}
		};
	}

	public override void Deserialize(Dictionary<string,object> elements)
	{
		Category = (string) elements["Category"];
		SubCategory = (string) elements["SubCategory"];

		base.Deserialize(elements);
	}
	
#if PARKITECT
	public override void Decorate(GameObject go, GameObject hider, ParkitectObj parkitectObj, AssetBundle bundle)
	{
		BuildableObject buildableObject = go.GetComponent<BuildableObject>();
		if (!String.IsNullOrEmpty(SubCategory))
		{
			buildableObject.categoryTag = Category + "/" + SubCategory;
		}
		else
		{
			buildableObject.categoryTag = Category;
		}
	}
#endif
}

