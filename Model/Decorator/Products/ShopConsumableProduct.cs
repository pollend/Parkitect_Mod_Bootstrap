using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

public enum HandSide { Left, Right }
public enum ConsumeAnimation { generic, drink_straw, lick, with_hands }

[Serializable]
public class ShopConsumableProduct : ShopProduct
{
	[SerializeField]
	public HandSide HandSide;
	[SerializeField]
	public ConsumeAnimation ConsumeAnimation;
	[SerializeField]
	public Temperature Temprature;
	[SerializeField]
	public int Portions;

	#if UNITY_EDITOR
	public override void RenderInspectorGUI ()
	{
		HandSide = HandSide.Right;
		ConsumeAnimation = (ConsumeAnimation)EditorGUILayout.EnumPopup("Consume Animation ", ConsumeAnimation);
		Temprature = (Temperature)EditorGUILayout.EnumPopup("Temprature ", Temprature);
		Portions = EditorGUILayout.IntField("Portions ", Portions);

		base.RenderInspectorGUI ();
	}
	#endif


	public override List<XElement> Serialize()
	{
		List<XElement> elements = base.Serialize ();
		elements.Add (new XElement ("Hand", HandSide));
		elements.Add (new XElement ("ConsumeAnimation", ConsumeAnimation));
		elements.Add (new XElement ("Tempreature", Temprature));
		elements.Add (new XElement ("Portion", Portions));
		return elements;
	}

	public override void DeSerialize (XElement element)
	{
		if(element.Element ("Hand") != null)
			HandSide = (HandSide) Enum.Parse (typeof(HandSide), element.Element ("Hand").Value);
		if(element.Element ("ConsumeAnimation") != null)
			ConsumeAnimation = (ConsumeAnimation)Enum.Parse (typeof(ConsumeAnimation), element.Element ("ConsumeAnimation").Value);
		if(element.Element ("Temprature") != null)
			Temprature = (Temperature)Enum.Parse (typeof(Temperature), element.Element ("Temprature").Value);
		if(element.Element ("Portions") != null)
			Portions = int.Parse (element.Element ("Portion").Value);
	}
	
#if PARKITECT
	public override Product Decorate()
	{
		GameObject go = new GameObject();
		ConsumableProduct consumableProduct = go.AddComponent<CustomConsumableProduct>();
		
		consumableProduct.trash = AssetManager.Instance.getPrefab(Prefabs.CandyTrash).GetComponent<Trash>();

		AssetManager.Instance.registerObject(consumableProduct);
		switch (ConsumeAnimation)
		{
			case ConsumeAnimation.generic:
				consumableProduct.consumeAnimation = ConsumableProduct.ConsumeAnimation.GENERIC;
				break;
			case ConsumeAnimation.drink_straw:
				consumableProduct.consumeAnimation = ConsumableProduct.ConsumeAnimation.DRINK_STRAW;
				break;
			case ConsumeAnimation.lick:
				consumableProduct.consumeAnimation = ConsumableProduct.ConsumeAnimation.LICK;
				break;
			case ConsumeAnimation.with_hands:
				consumableProduct.consumeAnimation = ConsumableProduct.ConsumeAnimation.WITH_HANDS;
				break;
		}
		switch (Temprature)
		{
			case Temperature.NONE:
				consumableProduct.temperaturePreference = TemperaturePreference.NONE;
				break;
			case Temperature.COLD:
				consumableProduct.temperaturePreference = TemperaturePreference.COLD;
				break;
			case Temperature.HOT:
				consumableProduct.temperaturePreference = TemperaturePreference.HOT;
				break;
		}

		switch (HandSide)
		{
			case HandSide.Left:
				consumableProduct.handSide = Hand.Side.LEFT;
				break;
			case  HandSide.Right:
				consumableProduct.handSide = Hand.Side.RIGHT;
				break;
		}
		consumableProduct.portions = Portions;
		return consumableProduct;
		
	}
#endif

}
