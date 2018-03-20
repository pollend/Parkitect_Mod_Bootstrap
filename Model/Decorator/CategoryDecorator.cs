using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class CategoryDecorator : Decorator
{
	public string category;
	public String subCategory;

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
			{"Category", category},
			{"SubCategory", subCategory}
		};
	}

	public override void Deserialize(Dictionary<string,object> elements)
	{
		category = (string) elements["Category"];
		subCategory = (string) elements["SubCategory"];

		base.Deserialize(elements);
	}
	
#if PARKITECT
	public override void Decorate(GameObject go, GameObject hider, ParkitectObj parkitectObj, AssetBundle bundle)
	{
		BuildableObject buildableObject = go.GetComponent<BuildableObject>();
		if (String.IsNullOrEmpty(subCategory))
		{
			buildableObject.categoryTag = category + "/" + subCategory;
		}
		else
		{
			buildableObject.categoryTag = category;
		}
	}
#endif
}

