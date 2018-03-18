using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public enum EffectTypes { HUNGER, THIRST, HAPPINESS, TIREDNESS, SUGARBOOST }

[Serializable]
public class SPIngredient
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
