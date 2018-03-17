#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;

public class ShopDecorator : Decorator
{
	public List<SPProduct> products = new List<SPProduct>();

#if UNITY_EDITOR
	[System.NonSerialized]
	private Vector2 scrollPos2;
	[System.NonSerialized]
	private SPProduct selected;
#endif

	public ShopDecorator()
	{
	}

	#if UNITY_EDITOR
	public override void RenderInspectorGUI (ParkitectObj parkitectObj)
	{

		foreach (SPProduct p in products)
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

			if (GUILayout.Button(products[i].Name + "    $" + products[i].Price + ".00" + " --" + products[i].ToString()))
			{

				GUI.FocusControl("");
				if (e.button == 1)
				{
					GameObject.DestroyImmediate(products[i]);
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
			AddProduct<SPWearableProduct> ();
		}
		if (GUILayout.Button("Add Consumable Product"))
		{
			AddProduct<SPConsumableProduct> ();
		}
		if (GUILayout.Button("Add OnGoing Product"))
		{
			AddProduct<SPOngoingProduct> ();
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
		base.RenderInspectorGUI (parkitectObj);
	}

	public void AddProduct<T>() where T : SPProduct
	{
		SPProduct product = ScriptableObject.CreateInstance<T> ();
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
			elements.Add(new XElement(products[x].GetType().ToString(),products[x].Serialize()));
		}
		return new List<XElement>{
			new XElement("Products",elements)
		};
	}

	public override void Deserialize (XElement element)
	{
		foreach(var ele in element.Element("Products").Elements())
		{
			SPProduct product = Utility.GetByTypeName<SPProduct> (ele.Name.NamespaceName);
			product.DeSerialize (ele);
			this.products.Add (product);
		}
		base.Deserialize (element);
	}
}




