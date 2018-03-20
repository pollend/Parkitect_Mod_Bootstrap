#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using System;
using UnityEngine;

public class ShopDecorator : Decorator
{
	public List<ShopProduct> products = new List<ShopProduct>();

#if UNITY_EDITOR
	[NonSerialized]
	private Vector2 scrollPos2;
	[NonSerialized]
	private ShopProduct selected;
#endif

#if UNITY_EDITOR
	public override void RenderInspectorGui (ParkitectObj parkitectObj)
	{

		foreach (ShopProduct p in products)
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
		for (int i = 0; i < products.Count; i++)
		{
			Color gui = GUI.color;
			if (products[i] == selected)
			{ GUI.color = Color.red; }

			if (GUILayout.Button(products[i].Name + "    $" + products[i].Price + " (" + products[i].ProductType + ")"))
			{

				GUI.FocusControl("");
				if (e.button == 1)
				{
					DestroyImmediate(products[i],true);
					products.RemoveAt(i);
					return;
				}

				if (selected == products[i])
				{
					selected = null;
					return;
				}
				selected = products[i];
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
			products.Add (product);
		}
		if(selected != null)
		{
			if(!products.Contains(selected))
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
		foreach (var t in products)
		{
			DestroyImmediate(t, true);
		}
		base.CleanUp (parkitectObj);
	}
#endif

	public override Dictionary<string,object> Serialize(ParkitectObj parkitectObj)
	{
		List<object> elements = new List<object>();
		foreach (var prod in products)
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
		foreach (var ele in (List<Dictionary<string,object>>)elements["Products"])
		{
			ShopProduct product = CreateInstance<ShopProduct>();
			product.DeSerialize(ele);
			products.Add(product);
		}
	}
#if PARKITECT
	public override void Decorate(GameObject go, GameObject hider, ParkitectObj parkitectObj, AssetBundle bundle)
	{
		CustomShop customShop = go.AddComponent<CustomShop>();
		customShop.walkableFlag = Block.WalkableFlagType.FORWARD;
		List<Product> result = new List<Product>();
		foreach (var p in products)
		{
			result.Add(p.Decorate(go, hider, parkitectObj, bundle));
		}

		customShop.products = result.ToArray();
	}
#endif
}




