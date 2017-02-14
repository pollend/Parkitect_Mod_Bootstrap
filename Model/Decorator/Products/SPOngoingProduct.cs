using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class SPOngoingProduct : SPProduct
{
	[SerializeField]
	public int Duration;
	[SerializeField]
	public bool RemoveWhenDepleted;
	[SerializeField]
	public bool DestroyWhenDepleted;

#if UNITY_EDITOR
	public override void RenderInspectorGUI ()
	{

		Duration = EditorGUILayout.IntField("Duration ", Duration);
		RemoveWhenDepleted = EditorGUILayout.Toggle ("Remove When Depleted", RemoveWhenDepleted);
		DestroyWhenDepleted = EditorGUILayout.Toggle ("Destroy When Depleted", DestroyWhenDepleted);

		base.RenderInspectorGUI ();
	}
#endif
	public override List<XElement> Serialize()
	{
		List<XElement> elements = base.Serialize ();
		elements.Add (new XElement ("Duration", Duration));
		elements.Add (new XElement ("RemoveWhenDepleted", RemoveWhenDepleted));
		elements.Add (new XElement ("DestroyWhenDepleted", DestroyWhenDepleted));
		return elements;
	}
}
