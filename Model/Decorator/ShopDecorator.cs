#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using System;
using UnityEngine;

public class ShopDecorator : Decorator
{
	public List<ShopProduct> Products = new List<ShopProduct>();

#if UNITY_EDITOR
	[NonSerialized]
	private Vector2 scrollPos2;
	[NonSerialized]
	private ShopProduct selected;
#endif

#if UNITY_EDITOR
	public override void RenderInspectorGui (ParkitectObj parkitectObj)
	{

		foreach (ShopProduct p in Products)
		{
			try
			{
				EditorUtility.SetDirty(p);
			}
			catch 
			{
			}
		}
		Event e = Event.current;

		GUILayout.Space(10);
		EditorGUILayout.LabelField("Products:", EditorStyles.boldLabel);
		scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, "GroupBox", GUILayout.Height(100));
		for (int i = 0; i < Products.Count; i++)
		{
			Color gui = GUI.color;
			if (Products[i] == selected)
			{ GUI.color = Color.red; }

			if (GUILayout.Button(Products[i].Name + "    $" + Products[i].Price + " (" + Products[i].ProductType + ")"))
			{

				GUI.FocusControl("");
				if (e.button == 1)
				{
					DestroyImmediate(Products[i],true);
					Products.RemoveAt(i);
					return;
				}

				if (selected == Products[i])
				{
					selected = null;
					return;
				}
				selected = Products[i];
			}
			GUI.color = gui;
		}
		EditorGUILayout.EndScrollView();

		
		if (GUILayout.Button("Add Product"))
		{
			ShopProduct product = CreateInstance<ShopProduct>();
			AssetDatabase.AddObjectToAsset (product,this);
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
			Products.Add (product);
		}
		if(selected != null)
		{
			if(!Products.Contains(selected))
			{
				selected = null;
				return;
			}
			GUILayout.Space(10);
			selected.RenderInspectorGUI();

		}
		base.RenderInspectorGui (parkitectObj);
	}

	

	public override void CleanUp (ParkitectObj parkitectObj)
	{
		foreach (var t in Products)
		{
			DestroyImmediate(t, true);
		}
		base.CleanUp (parkitectObj);
	}
#endif

	public override Dictionary<string,object> Serialize(ParkitectObj parkitectObj)
	{
		List<object> elements = new List<object>();
		foreach (var prod in Products)
		{
			elements.Add(prod.Serialize());
		}

		return new Dictionary<string, object>()
		{
			{"Products", elements}
		};
	}

	public override void Deserialize(Dictionary<string,object> elements)
	{
		foreach (var ele in (List<object>)elements["Products"])
		{
			Dictionary<string, object> prod = (Dictionary<string, object>) ele;
			ShopProduct product = CreateInstance<ShopProduct>();
			product.DeSerialize(prod);
			Products.Add(product);
		}
	}
#if PARKITECT
	public override void Decorate(GameObject go, GameObject hider, ParkitectObj parkitectObj, AssetBundle bundle)
	{
		CustomShop customShop = go.AddComponent<CustomShop>();
		customShop.walkableFlag = Block.WalkableFlagType.FORWARD;
		List<Product> result = new List<Product>();
		foreach (var p in Products)
		{
			result.Add(p.Decorate(go, hider, parkitectObj, bundle));
		}

		customShop.products = result.ToArray();
	}
#endif
}




