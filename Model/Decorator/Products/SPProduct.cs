using System;
using UnityEngine;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class SPProduct : ScriptableObject
{
	public enum Tempreature
	{
		NONE,
		COLD,
		HOT
	}

	public List<SPIngredient> ingredients = new List<SPIngredient>();


	public string Name;
	public float Price;

	[NonSerialized]
	private Vector2 scrollPos = Vector2.zero;
	[NonSerialized]
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

		for (int i = 0; i < ingredients.Count; i++)
		{
			Color gui = GUI.color;
			if (ingredients[i] == selected)
			{ GUI.color = Color.red; }

			if (GUILayout.Button(ingredients[i].Name + "    $" + ingredients[i].Price + ".00", "ShurikenModuleTitle"))
			{

				GUI.FocusControl("");
				if (e.button == 1)
				{
					ingredients.RemoveAt(i);
					return;
				}

				if (selected == ingredients[i])
				{
					selected = null;
					return;
				}
				selected = ingredients[i];
			}
			GUI.color = gui;
		}
		EditorGUILayout.EndScrollView();

		if (GUILayout.Button("Add Ingredients"))
		{
			ingredients.Add(new SPIngredient());
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginVertical();
		if(selected != null)
		{
			if (!ingredients.Contains(selected))
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
		for (int x = 0; x < ingredients.Count; x++) {
			xmlIngredient.Add (new XElement("Ingredient",ingredients [x].Serialize ()));
		}

		return new List<XElement> (new XElement[]{
			new XElement("Name",this.name),
			new XElement("Price",this.Price),
			new XElement("Ingredient",xmlIngredient)
		
		});
	}

}
