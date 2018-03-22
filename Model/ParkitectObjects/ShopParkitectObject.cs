#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

[ParkitectObjectTag("Shop")]
[Serializable]
public class ShopParkitectObject : ParkitectObj
{
	
	public List<ShopProduct> Products = new List<ShopProduct>();

#if UNITY_EDITOR
	[NonSerialized]
	private Vector2 scrollPos2;
	[NonSerialized]
	private ShopProduct selected;
#endif
	
	public override Type[] SupportedDecorators()
	{
		return new[] {
            typeof(BaseDecorator)
		};
	}


#if UNITY_EDITOR
	public override void RenderInspectorGui ()
	{
		base.RenderInspectorGui ();

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
		
	}

	public override void CleanUp()
	{
		foreach (var t in Products)
		{
			DestroyImmediate(t, true);
		}
		base.CleanUp();
	}
#endif
#if (PARKITECT)
	private CustomShop _shop;
	public override void BindToParkitect(GameObject hider, AssetBundle bundle)
	{

		var baseDecorator = DecoratorByInstance<BaseDecorator>();

		GameObject gameObject = Instantiate(bundle.LoadAsset<GameObject>(Key));
		gameObject.transform.parent = hider.transform;

		CustomShop customShop = gameObject.AddComponent<CustomShop>();
		customShop.walkableFlag = Block.WalkableFlagType.FORWARD;
		List<Product> result = new List<Product>();
		foreach (var p in Products)
		{
			result.Add(p.Decorate(go, hider, parkitectObj, bundle));
		}
		customShop.products = result.ToArray();

		baseDecorator.Decorate(gameObject, hider, this, bundle);

		CustomShop customShop = gameObject.GetComponent<CustomShop>();
		_shop = customShop;
		_shop.name = Key;
		AssetManager.Instance.registerObject(_shop);

		foreach (var prod in _shop.products)
		{
			AssetManager.Instance.registerObject(prod);
		}
	}

	public override void UnBindToParkitect(GameObject hider)
	{
		foreach (var prod in _shop.products)
		{
			AssetManager.Instance.unregisterObject(prod);
		}
		AssetManager.Instance.unregisterObject(_shop);
	}
#endif

	public override void DeSerialize(Dictionary<string, object> elements)
	{
		foreach (var ele in (List<object>)elements["Products"])
		{
			Dictionary<string, object> prod = (Dictionary<string, object>) ele;
			ShopProduct product = CreateInstance<ShopProduct>();
			product.DeSerialize(prod);
			Products.Add(product);
		}
		
		base.DeSerialize(elements);
	}

	public override Dictionary<string, object> Serialize()
	{
		Dictionary<string,object> result = base.Serialize();
		
		List<object> elements = new List<object>();
		foreach (var prod in Products)
		{
			elements.Add(prod.Serialize());
		}
		result.Add("Products", elements);
		return result;
	}
	
}



