#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Xml.Linq;
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

			if (GUILayout.Button(products[i].Name + "    $" + products[i].Price + " --" + products[i]))
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

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Wearable Product"))
		{
			AddProduct<ShopWearableProduct> ();
		}
		if (GUILayout.Button("Add Consumable Product"))
		{
			AddProduct<ShopConsumableProduct> ();
		}
		if (GUILayout.Button("Add OnGoing Product"))
		{
			AddProduct<ShopOngoingProduct> ();
		}
		EditorGUILayout.EndHorizontal();
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

	public void AddProduct<T>() where T : ShopProduct
	{
		ShopProduct product = CreateInstance<T> ();
		AssetDatabase.AddObjectToAsset (product,this);
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
		products.Add (product);
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

	public override List<XElement> Serialize (ParkitectObj parkitectObj)
	{
		List<XElement> elements = new List<XElement> ();
		for (int x = 0; x < products.Count; x++) {
			if (products[x] is ShopConsumableProduct)
			{
				elements.Add(new XElement("consumable", products[x].Serialize()));
			}
			else if (products[x] is ShopOngoingProduct)
			{
				elements.Add(new XElement("ongoing", products[x].Serialize()));
			}
		}
		return new List<XElement>{
			new XElement("Products",elements)
		};
	}

	public override void Deserialize (XElement element)
	{
		foreach(var ele in element.Element("Products").Elements())
		{
			ShopProduct product = Utility.GetByTypeName<ShopProduct> (ele.Name.NamespaceName);
			product.DeSerialize (ele);
			products.Add (product);
		}
		base.Deserialize (element);
	}
#if PARKITECT
	public override void Decorate(GameObject go, GameObject hider, ParkitectObj parkitectObj,List<SerializedMonoBehaviour> register)
	{
		CustomShop customShop =  go.AddComponent<CustomShop>();
		customShop.walkableFlag = Block.WalkableFlagType.FORWARD;
		List<Product> productResults = new List<Product>();
		foreach (var product in products)
		{
			Product p = product.Decorate();
			
			BindingFlags flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
			typeof(Product).GetField("displayName", flags).SetValue(p, product.name);
			
			p.defaultPrice = product.Price;
			register.Add(p);
			productResults.Add(p);
		}

		customShop.products = productResults.ToArray();

	}
#endif
}




