using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Temperature{NONE,COLD,HOT}
public enum HandSide { LEFT, RIGHT }
public enum ConsumeAnimation { GENERIC, DRINK_STRAW, LICK, WITH_HANDS }
public enum ProductType {ON_GOING,CONSUMABLE,WEARABLE}
public enum Seasonal { WINTER, SPRING, SUMMER, AUTUMN, NONE }
public enum Body { HEAD, FACE, BACK }
public enum EffectTypes { HUNGER, THIRST, HAPPINESS, TIREDNESS, SUGARBOOST }

public class ShopProduct : ScriptableObject
{
	public List<ShopIngredient> Ingredients = new List<ShopIngredient>();

	[SerializeField] public ProductType ProductType;
	
	//base
	[SerializeField] public string Name;
	[SerializeField] public float Price;
	[SerializeField] public String Key;

	[SerializeField] public bool IsTwoHanded;
	[SerializeField] public bool IsInterestingToLookAt;
	[SerializeField] public HandSide HandSide;
	
	//ongoing
	[SerializeField] public int Duration;
	[SerializeField] public bool RemoveWhenDepleted;
	[SerializeField] public bool DestroyWhenDepleted;
	
	//wearable
	[SerializeField] public Body BodyLocation = Body.HEAD;
	[SerializeField] public Seasonal SeasonalPrefrence = Seasonal.NONE;
	[SerializeField] public Temperature TempreaturePrefrence = Temperature.NONE;
	[SerializeField] public bool HideOnRide;
	[SerializeField] public bool HideHair;
	
	//consumable
	[SerializeField] public ConsumeAnimation ConsumeAnimation;
	[SerializeField] public Temperature Temprature;
	[SerializeField] public int Portions;
	
	[NonSerialized] private Vector2 scrollPos = Vector2.zero;
	[NonSerialized] private ShopIngredient selected;
	
	
	

#if UNITY_EDITOR
	public virtual void RenderInspectorGUI()
	{
		Name = EditorGUILayout.TextField("Name", Name);
		Price = EditorGUILayout.FloatField("Price ", Price);

		ParkitectObj[] pkObjects = ModPayload.Instance.ParkitectObjs.Where(x => x.Prefab != null).ToArray();
		ParkitectObj pkObject = pkObjects.SingleOrDefault(x => x.Key == Key);

		int index = -1;
		if (pkObject == null)
		{
			Key = "";
		}
		else
		{
			index = Array.IndexOf(pkObjects, pkObject);
		}

		index = EditorGUILayout.Popup("object", index, pkObjects.Select(x => x.Prefab.name +  " (" + x.GetObjectTag() + ")").ToArray());
		if (index < pkObjects.Length && index >= 0)
		{
			Key = pkObjects[index].Key;
		}
		ProductType = (ProductType) EditorGUILayout.EnumPopup("Product Type", ProductType);

		if (ProductType == ProductType.ON_GOING || ProductType == ProductType.CONSUMABLE)
		{
			HandSide = (HandSide) EditorGUILayout.EnumPopup("Hand Side", HandSide);
			IsTwoHanded = EditorGUILayout.Toggle("Is Two Handed", IsTwoHanded);
			IsInterestingToLookAt = EditorGUILayout.Toggle("Is Interesting To Look At", IsInterestingToLookAt);
		}
		
		switch (ProductType)
		{
			case ProductType.ON_GOING:
			{
				Duration = EditorGUILayout.IntField("Duration ", Duration);
				RemoveWhenDepleted = EditorGUILayout.Toggle("Remove When Depleted", RemoveWhenDepleted);
				DestroyWhenDepleted = EditorGUILayout.Toggle("Destroy When Depleted", DestroyWhenDepleted);
			}
				break;
			case ProductType.CONSUMABLE:
			{
				ConsumeAnimation = (ConsumeAnimation) EditorGUILayout.EnumPopup("Consume Animation ", ConsumeAnimation);
				Temprature = (Temperature) EditorGUILayout.EnumPopup("Temprature ", Temprature);
				Portions = EditorGUILayout.IntField("Portions ", Portions);
			}
				break;
			case ProductType.WEARABLE:
			{
				BodyLocation = (Body) EditorGUILayout.EnumPopup("Body Location ", BodyLocation);
				SeasonalPrefrence = (Seasonal) EditorGUILayout.EnumPopup("Seasonal Prefrence ", SeasonalPrefrence);
				TempreaturePrefrence = (Temperature) EditorGUILayout.EnumPopup("Tempreature Prefrence", TempreaturePrefrence);
				HideHair = EditorGUILayout.Toggle("Hide Hair", HideHair);
				HideOnRide = EditorGUILayout.Toggle("Hide On Ride", HideOnRide);
			}
				break;
		}


		DrawIngredients();
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
			{
				GUI.color = Color.red;
			}

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
		if (selected != null)
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

				if (GUILayout.Button("Effector " + selected.effects[i].Type, "ShurikenModuleTitle"))
				{

					GUI.FocusControl("");
					if (e.button == 1)
					{
						selected.effects.RemoveAt(i);
						return;
					}
				}

				selected.effects[i].Type = (EffectTypes) EditorGUILayout.EnumPopup("Type ", selected.effects[i].Type);
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
		List<XElement> xmlIngredient = new List<XElement>();
		for (int x = 0; x < Ingredients.Count; x++)
		{
			xmlIngredient.Add(new XElement("Ingredient", Ingredients[x].Serialize()));
		}

		List<XElement> items = new List<XElement>(new[]
		{
			new XElement("Name", name),
			new XElement("Price", Price),
			new XElement("Ingredients", xmlIngredient),
			new XElement("key", Key),
			new XElement("ProductType",ProductType) 
		});
		

		if (ProductType == ProductType.ON_GOING || ProductType == ProductType.CONSUMABLE)
		{
			items.Add (new XElement ("Hand", HandSide));
			items.Add (new XElement ("IsInterestingToLookAt", IsInterestingToLookAt));
			items.Add (new XElement ("IsTwoHanded", IsTwoHanded));
		}

		
		switch (ProductType)
		{
			case ProductType.ON_GOING:
				items.Add (new XElement ("Duration", Duration));
				items.Add (new XElement ("RemoveWhenDepleted", RemoveWhenDepleted));
				items.Add (new XElement ("DestroyWhenDepleted", DestroyWhenDepleted));
				break;
			case ProductType.CONSUMABLE:
				items.Add (new XElement ("ConsumeAnimation", ConsumeAnimation));
				items.Add (new XElement ("Tempreature", Temprature));
				items.Add (new XElement ("Portion", Portions));
				break;
			case ProductType.WEARABLE:
				items.Add (new XElement ("BodyLocation", BodyLocation));
				items.Add (new XElement ("SeasonalPrefrence", SeasonalPrefrence));
				items.Add (new XElement ("TempreaturePrefrence", TempreaturePrefrence));
				items.Add (new XElement ("HideOnRide", HideOnRide));
				items.Add (new XElement ("HideHair", HideHair));
				break;
		}
		
		return items;
	}

	public virtual void DeSerialize(XElement element)
	{
		if (element.Element("Name") != null)
			name = element.Element("Name").Value;

		if (element.Element("Price") != null)
			Price = float.Parse(element.Element("Price").Value);

		if (element.Element("key") != null)
			Key = element.Element("key").Value;

		if (element.Element("ProductType") != null)
			ProductType = (ProductType) Enum.Parse (typeof(ProductType), element.Element("ProductType").Value);

		if (ProductType == ProductType.ON_GOING || ProductType == ProductType.CONSUMABLE)
		{
			if (element.Element("Hand") != null)
				HandSide = (HandSide) Enum.Parse(typeof(HandSide), element.Element("Hand").Value);
			if (element.Element("IsInterestingToLookAt") != null)
				IsInterestingToLookAt = bool.Parse(element.Element("IsInterestingToLookAt").Value);
			if (element.Element("IsTwoHanded") != null)
				IsTwoHanded = bool.Parse(element.Element("IsTwoHanded").Value);
		}

		switch (ProductType)
		{
			case ProductType.ON_GOING:
			{
				if (element.Element("Duration") != null)
					Duration = int.Parse(element.Element("Duration").Value);
				if (element.Element("RemoveWhenDepleted") != null)
					RemoveWhenDepleted = bool.Parse(element.Element("RemoveWhenDepleted").Value);
				if (element.Element("DestroyWhenDepleted") != null)
					DestroyWhenDepleted = bool.Parse(element.Element("DestroyWhenDepleted").Value);
			}
				break;
			case ProductType.CONSUMABLE:
			{
				if(element.Element ("ConsumeAnimation") != null)
					ConsumeAnimation = (ConsumeAnimation)Enum.Parse (typeof(ConsumeAnimation), element.Element ("ConsumeAnimation").Value);
				if(element.Element ("Temprature") != null)
					Temprature = (Temperature)Enum.Parse (typeof(Temperature), element.Element ("Temprature").Value);
				if(element.Element ("Portions") != null)
					Portions = int.Parse (element.Element ("Portion").Value);
			}
				break;
			case ProductType.WEARABLE:
			{
				if(element.Element ("BodyLocation") != null)
					BodyLocation = (Body)Enum.Parse (typeof(Body), element.Element ("BodyLocation").Value);
				if(element.Element ("SeasonalPrefrence") != null)
					SeasonalPrefrence = (Seasonal)Enum.Parse (typeof(Seasonal), element.Element ("SeasonalPrefrence").Value);
				if(element.Element ("TempreaturePrefrence") != null)
					TempreaturePrefrence = (Temperature)Enum.Parse (typeof(Temperature), element.Element ("TempreaturePrefrence").Value);
				if(element.Element ("HideOnRide") != null)
					HideOnRide = bool.Parse(element.Element ("HideOnRide").Value);
				if(element.Element ("HideHair") != null)
					HideHair = bool.Parse (element.Element ("HideHair").Value);
			}
				break;
		}

		if (element.Element("Ingredients") != null)
		{
			foreach (XElement xmlingredient in element.Element("Ingredients").Elements("Ingredient"))
			{
				ShopIngredient ingredient = new ShopIngredient();
				ingredient.DeSerialize(xmlingredient);
				Ingredients.Add(ingredient);
			}
		}
	}

#if PARKITECT
	public abstract Product Decorate();
#endif
}


[Serializable]
public class ShopIngredient
{

	[SerializeField] public string Name = "New Ingredient";
	[SerializeField] public float Price = 1;
	[SerializeField] public float Amount = 1;
	[SerializeField] public bool Tweakable = true;
	[SerializeField] public List<Effect> effects = new List<Effect>();

	public List<XElement> Serialize()
	{
		List<XElement> xmlEffect = new List<XElement>();
		for (int x = 0; x < effects.Count; x++)
		{
			xmlEffect.Add(new XElement("Effect", effects[x].Serialize()));
		}

		return new List<XElement>(new[]
		{
			new XElement("Name", Name),
			new XElement("Price", Price),
			new XElement("Amount", Amount),
			new XElement("Tweakable", Tweakable),
			new XElement("Effects", xmlEffect)
		});
	}

	public void DeSerialize(XElement element)
	{
		if (element.Element("Name") != null)
			Name = element.Element("Name").Value;
		if (element.Element("Price") != null)
			Price = float.Parse(element.Element("Price").Value);
		if (element.Element("Amount") != null)
			Amount = int.Parse(element.Element("Amount").Value);
		if (element.Element("Tweakable") != null)
			Tweakable = bool.Parse(element.Element("Tweakable").Value);
		if (element.Element("Effects") != null)
		{
			foreach (XElement xmlEffect in element.Elements("Effects"))
			{
				Effect effect = new Effect();
				effect.DeSerialize(xmlEffect);
				effects.Add(effect);
			}
		}
	}
}


[Serializable]
public class Effect
{
	[SerializeField]
	public EffectTypes Type = EffectTypes.HUNGER;
	[SerializeField]
	public float amount;

	public List<XElement> Serialize()
	{
		return new List<XElement> (new[] {
			new XElement("Type",Type),
			new XElement("Amount",amount)
		});
	}

	public void DeSerialize(XElement element)
	{
		if(element.Element ("Type") != null)
			Type = (EffectTypes)Enum.Parse(typeof(EffectTypes), element.Element ("Type").Value);
		if(element.Element ("Amount") != null)
			amount = float.Parse (element.Element ("Amount").Value);
	}
}
