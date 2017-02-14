using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Hand { Left, Right }
public enum ConsumeAnimation { generic, drink_straw, lick, with_hands }

[Serializable]
public class SPConsumableProduct : SPProduct
{
	[SerializeField]
	public Hand Hand;
	[SerializeField]
	public ConsumeAnimation ConsumeAnimation;
	[SerializeField]
	public Tempreature Temp;
	[SerializeField]
	public int portions;

	#if UNITY_EDITOR
	public override void RenderInspectorGUI ()
	{
		Hand = Hand.Right;
		ConsumeAnimation = (ConsumeAnimation)EditorGUILayout.EnumPopup("Consume Animation ", ConsumeAnimation);
		Temp = (Tempreature)EditorGUILayout.EnumPopup("Temprature ", Temp);
		portions = EditorGUILayout.IntField("Portions ", portions);

		base.RenderInspectorGUI ();
	}
	#endif

	public override List<XElement> Serialize()
	{
		List<XElement> elements = base.Serialize ();
		elements.Add (new XElement ("Hand", Hand));
		elements.Add (new XElement ("ConsumeAnimation", ConsumeAnimation));
		elements.Add (new XElement ("Tempreature", Temp));
		elements.Add (new XElement ("Portion", portions));
		return elements;
	}

}
