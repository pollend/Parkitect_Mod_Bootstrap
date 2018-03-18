using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ShopProduct : ScriptableObject
{
	public enum Temperature
	{
		NONE,
		COLD,
		HOT
	}

	public List<ShopIngredient> Ingredients = new List<ShopIngredient>();

	[SerializeField] 
	public string Name;
	[SerializeField] 
	public float Price;
	[SerializeField] 
	public String key;
	
	[NonSerialized]
	private Vector2 scrollPos = Vector2.zero;
	[NonSerialized]
	private ShopIngredient selected;

#if UNITY_EDITOR
	public virtual void RenderInspectorGUI(){
		Name = EditorGUILayout.TextField ("Name",Name);
		Price = EditorGUILayout.FloatField("Price ", Price);
		
		ParkitectObj[] pkObjects = ModPayload.Instance.ParkitectObjs.Where(x => x.Prefab != null).ToArray();
		ParkitectObj pkObject = pkObjects.SingleOrDefault(x => x.Key == key);
		
		int index = -1;
		if (pkObject == null)
		{
			key = "";
		}
		else
		{
			index = Array.IndexOf(pkObjects, pkObject);
		}

		index = EditorGUILayout.Popup("object", index, pkObjects.Select(x => x.Prefab.name).ToArray());
		if (index < pkObjects.Length && index >= 0)
		{
			key = pkObjects[index].Key;
		}

		
		DrawIngredients ();
	}

	private void DrawIngredients()
	{
		Event e = Event.current;
		EditorGUILayout.LabelField("Ingredients:", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal(GUILayout.Height(300));
		EditorGUILayout.BeginVertical("ShurikenEffectBg", GUILayout.Width(150));
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));

		for (int i = 0; i < Ingredients.Count; i++)
		{
			Color gui = GUI.color;
			if (Ingredients[i] == selected)
			{ GUI.color = Color.red; }

			if (GUILayout.Button(Ingredients[i].Name + "    $" + Ingredients[i].Price + ".00", "ShurikenModuleTitle"))
			{

				GUI.FocusControl("");
				if (e.button == 1)
				{
					Ingredients.RemoveAt(i);
					return;
				}

				if (selected == Ingredients[i])
				{
					selected = null;
					return;
				}
				selected = Ingredients[i];
			}
			GUI.color = gui;
		}
		EditorGUILayout.EndScrollView();
		
		if (GUILayout.Button("Add Ingredients"))
		{
			Ingredients.Add(new ShopIngredient());
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginVertical();
		if(selected != null)
		{
			if (!Ingredients.Contains(selected))
			{
				selected = null;
				return;
			}
			
			selected.Name = EditorGUILayout.TextField("Ingridient Name ", selected.Name);
			selected.Price = EditorGUILayout.FloatField("Price ", selected.Price);
			selected.Amount = EditorGUILayout.FloatField("Amount ", selected.Amount);
			selected.Tweakable = EditorGUILayout.Toggle("Tweakable ", selected.Tweakable);

			for (int i = 0; i < selected.effects.Count; i++)
			{
				Color gui = GUI.color;

				if (GUILayout.Button( "Effector " + selected.effects[i].Type, "ShurikenModuleTitle"))
				{

					GUI.FocusControl("");
					if (e.button == 1)
					{
						selected.effects.RemoveAt(i);
						return;
					}
				}

				selected.effects[i].Type = (EffectTypes)EditorGUILayout.EnumPopup("Type ", selected.effects[i].Type);
				selected.effects[i].amount = EditorGUILayout.Slider("Amount", selected.effects[i].amount, 1f, -1f);
				GUI.color = gui;
			}
			if (GUILayout.Button("Add Effect"))
			{
				selected.effects.Add(new Effect());
			}

		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndVertical();
	}
#endif

	public virtual List<XElement> Serialize()
	{
		List<XElement> xmlIngredient = new List<XElement> ();
		for (int x = 0; x < Ingredients.Count; x++) {
			xmlIngredient.Add (new XElement("Ingredient",Ingredients [x].Serialize ()));
		}

		return new List<XElement> (new[]{
			new XElement("Name",name),
			new XElement("Price",Price),
			new XElement("Ingredients",xmlIngredient),
			new XElement ("key", key)
		});
	}

	public virtual void DeSerialize(XElement element)
	{
		if (element.Element ("Name") != null)
			name = element.Element ("Name").Value;

		if (element.Element ("Price") != null)
			Price = float.Parse(element.Element ("Price").Value);

		if (element.Element("key") != null)
			key = element.Element("key").Value;
		
		if (element.Element ("Ingredients") != null) {
			foreach(XElement xmlingredient in element.Element("Ingredients").Elements("Ingredient"))
			{
				ShopIngredient ingredient = new ShopIngredient ();
				ingredient.DeSerialize (xmlingredient);
				Ingredients.Add (ingredient);
			}
		}
	}

#if PARKITECT
	public abstract Product Decorate();
#endif

}
