using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class SPWearableProduct : SPProduct
{
	public enum Seasonal { WINTER, SPRING, SUMMER, AUTUMN, NONE }
	public enum Body { HEAD, FACE, BACK }

	[SerializeField]
	public Body BodyLocation = Body.HEAD;
	[SerializeField]
	public Seasonal SeasonalPrefrence = Seasonal.NONE;
	[SerializeField]
	public Tempreature TempreaturePrefrence = Tempreature.NONE;
	[SerializeField]
	public bool HideOnRide = false;
	[SerializeField]
	public bool HideHair = false;


	#if UNITY_EDITOR
	public override void RenderInspectorGUI()
	{
		BodyLocation = (Body)EditorGUILayout.EnumPopup("Body Location ", BodyLocation);
		SeasonalPrefrence = (Seasonal)EditorGUILayout.EnumPopup ("Seasonal Prefrence ", SeasonalPrefrence);
		TempreaturePrefrence = (Tempreature)EditorGUILayout.EnumPopup ("Tempreature Prefrence", TempreaturePrefrence);
		HideHair = EditorGUILayout.Toggle("Hide Hair", HideHair);
		HideOnRide = EditorGUILayout.Toggle ("Hide On Ride", HideOnRide);
		base.RenderInspectorGUI();
	}
	#endif

	public override List<XElement> Serialize()
	{
		List<XElement> elements = base.Serialize ();
		elements.Add (new XElement ("BodyLocation", BodyLocation));
		elements.Add (new XElement ("SeasonalPrefrence", SeasonalPrefrence));
		elements.Add (new XElement ("BodyLocation", BodyLocation));
		elements.Add (new XElement ("SeasonalPrefrence", SeasonalPrefrence));
		elements.Add (new XElement ("TempreaturePrefrence", TempreaturePrefrence));
		elements.Add (new XElement ("HideOnRide", HideOnRide));
		elements.Add (new XElement ("HideHair", HideHair));
		return elements;
	}
}
