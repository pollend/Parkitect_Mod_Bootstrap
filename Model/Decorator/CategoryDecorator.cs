using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class CategoryDecorator : Decorator
{
	public string category;
	
	public CategoryDecorator ()
	{
	}
	#if UNITY_EDITOR
    public override void RenderInspectorGUI (ParkitectObj parkitectObj)
	{
		this.category = EditorGUILayout.TextField("Category: ", this.category);

        base.RenderInspectorGUI (parkitectObj);
	}
	#endif
}

