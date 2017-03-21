using System;
using UnityEngine;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class SPProduct : ScriptableObject
{
	public enum Temperature
	{
		NONE,
		COLD,
		HOT
	}

	public List<SPIngredient> Ingredients = new List<SPIngredient>();


	public string Name;
	public float Price;

	[System.NonSerialized]
	private Vector2 scrollPos = Vector2.zero;
	[System.NonSerialized]
	private SPIngredient selected = null;

	public SPProduct()
	{
	}

#if UNITY_EDITOR
	public virtual void RenderInspectorGUI(){
		Name = EditorGUILayout.TextField ("Name",Name);
		Price = EditorGUILayout.FloatField("Price ", Price);
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
			Ingredients.Add(new SPIngredient());
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

		return new List<XElement> (new XElement[]{
			new XElement("Name",this.name),
			new XElement("Price",this.Price),
			new XElement("Ingredients",xmlIngredient)
		});
	}

	public virtual void DeSerialize(XElement element)
	{
		if (element.Element ("Name") != null)
			this.name = element.Element ("Name").Value;

		if (element.Element ("Price") != null)
			this.Price = float.Parse(element.Element ("Price").Value);

		if (element.Element ("Ingredients") != null) {
			foreach(XElement xmlingredient in element.Element("Ingredients").Elements("Ingredient"))
			{
				SPIngredient ingredient = new SPIngredient ();
				ingredient.DeSerialize (xmlingredient);
				Ingredients.Add (ingredient);
			}
		}
	}

}
