#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

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
	[SerializeField] public bool IsInterestingToLookAt ;
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

	public virtual Dictionary<string,object> Serialize()
	{
		List<object> ing= new List<object>();
		for (int x = 0; x < Ingredients.Count; x++)
		{
			ing.Add(Ingredients[x].Serialize());
		}

		Dictionary<string, object> items = new Dictionary<string, object>()
		{
			{"Name", name},
			{"Price", Price},
			{"Ingredients", ing},
			{"key", Key},
			{"ProductType", ProductType}
		};


			if (ProductType == ProductType.ON_GOING || ProductType == ProductType.CONSUMABLE)
		{
			items.Add ("Hand", HandSide);
			items.Add ("IsInterestingToLookAt", IsInterestingToLookAt);
			items.Add ("IsTwoHanded", IsTwoHanded);
		}

		
		switch (ProductType)
		{
			case ProductType.ON_GOING:
				items.Add ("Duration", Duration);
				items.Add ("RemoveWhenDepleted", RemoveWhenDepleted);
				items.Add ("DestroyWhenDepleted", DestroyWhenDepleted);
				break;
			case ProductType.CONSUMABLE:
				items.Add ("ConsumeAnimation", ConsumeAnimation);
				items.Add ("Tempreature", Temprature);
				items.Add ("Portion", Portions);
				break;
			case ProductType.WEARABLE:
				items.Add ("BodyLocation", BodyLocation);
				items.Add ("SeasonalPrefrence", SeasonalPrefrence);
				items.Add ("TempreaturePrefrence", TempreaturePrefrence);
				items.Add ("HideOnRide", HideOnRide);
				items.Add ("HideHair", HideHair);
				break;
		}
		
		return items;
	}

	public virtual void DeSerialize(Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("Name") )
			name = (string) elements["Name"];

		if (elements.ContainsKey("Price"))
			Price = (float)(double)elements["Price"];

		if (elements.ContainsKey("key") )
			Key = (string)elements["key"];

		if (elements.ContainsKey("ProductType") )
			ProductType = (ProductType) Enum.Parse (typeof(ProductType),(string)elements["ProductType"]);

		if (ProductType == ProductType.ON_GOING || ProductType == ProductType.CONSUMABLE)
		{
			if (elements.ContainsKey("Hand"))
				HandSide = (HandSide) Enum.Parse(typeof(HandSide), (string)elements["Hand"]);
			if (elements.ContainsKey("IsInterestingToLookAt") )
				IsInterestingToLookAt = (bool)elements["IsInterestingToLookAt"];
			if (elements.ContainsKey("IsTwoHanded") )
				IsTwoHanded = (bool)elements["IsTwoHanded"];
		}

		switch (ProductType)
		{
			case ProductType.ON_GOING:
			{
				if (elements.ContainsKey("Duration"))
					Duration = (int)(long)elements["Duration"];
				if (elements.ContainsKey("RemoveWhenDepleted"))
					RemoveWhenDepleted = (bool)elements["RemoveWhenDepleted"];
				if (elements.ContainsKey("DestroyWhenDepleted"))
					DestroyWhenDepleted = (bool)elements["DestroyWhenDepleted"];
			}
				break;
			case ProductType.CONSUMABLE:
			{
				if(elements.ContainsKey("ConsumeAnimation") )
					ConsumeAnimation = (ConsumeAnimation)Enum.Parse (typeof(ConsumeAnimation),(string)elements["ConsumeAnimation"]);
				if(elements.ContainsKey("Temprature"))
					Temprature = (Temperature)Enum.Parse (typeof(Temperature), (string)elements["Temprature"]);
				if(elements.ContainsKey("Portions"))
					Portions =  (int)(long)elements["Portion"];
			}
				break;
			case ProductType.WEARABLE:
			{
				if (elements.ContainsKey("BodyLocation"))
					BodyLocation = (Body) Enum.Parse(typeof(Body), (string) elements["BodyLocation"]);
				if (elements.ContainsKey("SeasonalPrefrence"))
					SeasonalPrefrence = (Seasonal) Enum.Parse(typeof(Seasonal), (string) elements["SeasonalPrefrence"]);
				if (elements.ContainsKey("TempreaturePrefrence"))
					TempreaturePrefrence = (Temperature) Enum.Parse(typeof(Temperature), (string) elements["TempreaturePrefrence"]);
				if (elements.ContainsKey("HideOnRide") )
					HideOnRide = (bool) elements["HideOnRide"];
				if (elements.ContainsKey("HideHair"))
					HideHair = (bool) elements["HideHair"];
			}
				break;
		}

		if (elements.ContainsKey("Ingredients"))
		{
			foreach (var ing in (List<Dictionary<string,object>>)elements["Ingredients"])
			{
				ShopIngredient ingredient = new ShopIngredient();
				ingredient.DeSerialize(ing);
				Ingredients.Add(ingredient);
			}
		}
	}

#if PARKITECT
	public Product Decorate(GameObject go, GameObject hider, ParkitectObj parkitectObj,AssetBundle assetBundle)
	{
		GameObject prod;
		if (String.IsNullOrEmpty(Key))
		{
			prod = new GameObject();
		}
		else
		{
			prod = assetBundle.LoadAsset<GameObject>(Key);
		}
		prod.transform.parent = hider.transform;
		Product product = null;
		
		switch (ProductType)
		{
			case ProductType.ON_GOING:
			{
				CustomOngoingEffectProduct c = prod.AddComponent<CustomOngoingEffectProduct>();
				c.duration = Duration;
				c.removeFromInventoryWhenDepleted = RemoveWhenDepleted;
				c.destroyWhenDepleted = DestroyWhenDepleted;
				product = c;
			}
				break;
			case ProductType.CONSUMABLE:
			{
				CustomConsumableProduct c = prod.AddComponent<CustomConsumableProduct>();
				switch (ConsumeAnimation)
				{
					case ConsumeAnimation.GENERIC:
						c.consumeAnimation = ConsumableProduct.ConsumeAnimation.GENERIC;
						break;
					case ConsumeAnimation.DRINK_STRAW:

						c.consumeAnimation = ConsumableProduct.ConsumeAnimation.DRINK_STRAW;
						break;
					case ConsumeAnimation.LICK:

						c.consumeAnimation = ConsumableProduct.ConsumeAnimation.LICK;
						break;
					case ConsumeAnimation.WITH_HANDS:

						c.consumeAnimation = ConsumableProduct.ConsumeAnimation.WITH_HANDS;
						break;
				}

				switch (Temprature)
				{
					case Temperature.COLD:
						c.temperaturePreference = TemperaturePreference.COLD;
						break;
					case Temperature.HOT:
						c.temperaturePreference = TemperaturePreference.HOT;
						break;
					case Temperature.NONE:
						c.temperaturePreference = TemperaturePreference.NONE;
						break;
				}

				c.portions = Portions;
				product = c;

			}
				break;
			case ProductType.WEARABLE:
			{
				CustomWerableProduct c = prod.AddComponent<CustomWerableProduct>();
				switch (TempreaturePrefrence)
				{
					case Temperature.COLD:
						c.temperaturePreference = TemperaturePreference.COLD;
						break;
					case Temperature.HOT:
						c.temperaturePreference = TemperaturePreference.HOT;
						break;
					case Temperature.NONE:
						c.temperaturePreference = TemperaturePreference.NONE;
						break;
				}

				switch (SeasonalPrefrence)
				{
					case Seasonal.WINTER:
						c.seasonalPreference = WearableProduct.SeasonalPreference.WINTER;
						break;
					case Seasonal.SPRING:
						c.seasonalPreference = WearableProduct.SeasonalPreference.SPRING;
						break;
					case Seasonal.SUMMER:
						c.seasonalPreference = WearableProduct.SeasonalPreference.SUMMER;
						break;
					case Seasonal.AUTUMN:
						c.seasonalPreference = WearableProduct.SeasonalPreference.AUTUMN;
						break;
					case Seasonal.NONE:
						c.seasonalPreference = WearableProduct.SeasonalPreference.NONE;
						break;
				}
				switch (BodyLocation)
				{
					case Body.HEAD:
						c.bodyLocation = WearableProduct.BodyLocation.HEAD;
						break;
					case Body.FACE:
						c.bodyLocation = WearableProduct.BodyLocation.FACE;
						break;
					case Body.BACK:
						c.bodyLocation = WearableProduct.BodyLocation.BACK;
						break;
				}

				c.hideOnRides = HideOnRide;
				c.dontHideHair = HideHair;
				
				product = c;
			}
				break;
		}

		if (ProductType == ProductType.ON_GOING || ProductType == ProductType.CONSUMABLE)
		{
			switch (HandSide)
			{
				case HandSide.LEFT:
					product.handSide = Hand.Side.LEFT;
					break;
				case HandSide.RIGHT:
					product.handSide = Hand.Side.RIGHT;
					break;
			}

			product.interestingToLookAt = IsInterestingToLookAt;
			product.isTwoHanded = IsTwoHanded;
		}
		
		BindingFlags flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
		typeof(Product).GetField("displayName", flags).SetValue(product, Name);

		product.ingredients = generateIngredient();
		product.defaultPrice = Price;
		return product;
	}

	public Ingredient[] generateIngredient()
	{
		List<Ingredient> results = new List<Ingredient>();
		foreach (var t in Ingredients)
		{
			Ingredient ing = new Ingredient();
			ing.defaultAmount = t.Amount;
			ing.tweakable = t.Tweakable;
			results.Add(ing);

			var resource = CreateInstance<Resource>();
			ing.resource = resource;
			resource.name = t.Name;
			resource.setDisplayName(t.Name);
			resource.setCosts(t.Price);
			resource.getResourceSettings().percentage = 1f;

			List<ConsumableEffect> consumableEffects = new List<ConsumableEffect>();
			foreach (var t1 in t.effects)
			{
				var consumableEffect = new ConsumableEffect();
				consumableEffect.amount = t1.amount;
				switch (t1.Type)
				{
					case EffectTypes.HUNGER:
						consumableEffect.affectedStat = ConsumableEffect.AffectedStat.HUNGER;
						break;
					case EffectTypes.THIRST:

						consumableEffect.affectedStat = ConsumableEffect.AffectedStat.THIRST;
						break;
					case EffectTypes.HAPPINESS:

						consumableEffect.affectedStat = ConsumableEffect.AffectedStat.HAPPINESS;
						break;
					case EffectTypes.TIREDNESS:

						consumableEffect.affectedStat = ConsumableEffect.AffectedStat.TIREDNESS;
						break;
					case EffectTypes.SUGARBOOST:

						consumableEffect.affectedStat = ConsumableEffect.AffectedStat.SUGARBOOST;
						break;
					default:
						throw new ArgumentOutOfRangeException();

				}

				consumableEffects.Add(consumableEffect);
			}

			resource.effects = consumableEffects.ToArray();
		}

		return results.ToArray();
	}
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

	public Dictionary<string,object> Serialize()
	{
		List<object> eff = new List<object>();
		foreach (var t in effects)
		{
			eff.Add(t.Serialize());
		}

		return new Dictionary<string, object>
		{
			{"Name", Name},
			{"Price", Price},
			{"Amount", Amount},
			{"Tweakable", Tweakable},
			{"Effects", eff}
		};
	}

	public void DeSerialize(Dictionary<string,object> element)
	{
		if (element.ContainsKey("Name"))
			Name = (string) element["Name"];
		if (element.ContainsKey("Price") )
			Price = (float)(double)element["Price"];
		if (element.ContainsKey("Amount"))
			Amount = (int)(long)element["Amount"];
		if (element.ContainsKey("Tweakable"))
			Tweakable = (bool) element["Tweakable"];
		if (element.ContainsKey("Effects"))
		{
			foreach (var eff in (List<Dictionary<string,object>>)element["Effects"])
			{
				Effect effect = new Effect();
				effect.DeSerialize(eff);
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

	public Dictionary<string, object> Serialize()
	{
		return new Dictionary<string, object>
		{
			{"Type", Type},
			{"Amount", amount}
		};
	}

	public void DeSerialize(Dictionary<string,object> elements)
	{
		if(elements.ContainsKey("Type"))
			Type = (EffectTypes)Enum.Parse(typeof(EffectTypes), (string) elements["Type"]);
		if(elements.ContainsKey("Amount"))
			amount = (float)(double)elements["Amount"];
	}
}
