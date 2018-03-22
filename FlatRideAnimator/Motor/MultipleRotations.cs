
 #if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
[Serializable]
[MotorTag("MultiplyRotations")]
public class MultipleRotations : Motor
{
	[SerializeField]
	public RefrencedTransform MainAxis = new RefrencedTransform();
	[SerializeField]
	public List<RefrencedTransform> Axiss = new List<RefrencedTransform>();
#if UNITY_EDITOR
	public override void InspectorGUI(Transform root)
	{

	    Identifier = EditorGUILayout.TextField("Name ", Identifier);
		MainAxis.SetSceneTransform((Transform)EditorGUILayout.ObjectField("MainAxis", MainAxis.FindSceneRefrence(root), typeof(Transform), true));
	    Transform axis = (Transform)EditorGUILayout.ObjectField("Add axis", null, typeof(Transform), true);
		if (axis) {
			var refrenceTransform = new RefrencedTransform ();
			refrenceTransform.SetSceneTransform (axis);
			Axiss.Add (refrenceTransform);
		}
		if (Selection.objects.Length > 0) {
			if (GUILayout.Button ("Add selection")) {
				foreach (GameObject GObj in Selection.objects) {
					var refrenceTransform = new RefrencedTransform ();
					refrenceTransform.SetSceneTransform (GObj.transform);
					Axiss.Add (refrenceTransform);
				}

			}
		}
		foreach (RefrencedTransform T in Axiss) {
			if (GUILayout.Button (T.FindSceneRefrence (root).gameObject.name, "ShurikenModuleTitle")) {
				if (Event.current.button == 1) {
					Axiss.Remove (T);
					return;
				}

				Selection.objects = new GameObject[] { T.FindSceneRefrence (root).gameObject };
				EditorGUIUtility.PingObject (T.FindSceneRefrence (root).gameObject);
			}
		}
		base.InspectorGUI(root);
	}
#endif
	public override void Reset(Transform root)
	{
		Transform transform = MainAxis.FindSceneRefrence (root);
		if (transform) {
			foreach (RefrencedTransform T in Axiss) {
				T.FindSceneRefrence (root).localRotation = transform.localRotation;
			}
		}
	}
	public void Tick(float dt, Transform root)
	{
		Transform transform = MainAxis.FindSceneRefrence (root);
		if (transform) {
			foreach (RefrencedTransform T in Axiss) {
				T.FindSceneRefrence (root).localRotation = transform.localRotation;
			}
		}
	}

	public override void PrepareExport(ParkitectObj parkitectObj)
	{
		for (int x = 0; x < Axiss.Count; x++) {
			Axiss [x].UpdatePrefabRefrence (parkitectObj.Prefab.transform);
		}
		base.PrepareExport (parkitectObj);
	}


	public override Dictionary<string,object> Serialize (Transform root)
	{
		List<object> axiss = new List<object> ();
		foreach (var axis in Axiss)
		{
			axiss.Add (axis.Serialize (root));
		}

		return new Dictionary<string, object> {
			{"mainTransform",MainAxis.Serialize(root)},
			{"axisses",axiss}
		};
	}

	public override void Deserialize(Dictionary<string, object> elements)
	{
		if (elements.ContainsKey("axisses"))
		{
			foreach (var axis in (List<object>)elements["axisses"])
			{
				RefrencedTransform refrence = new RefrencedTransform();
				refrence.Deserialize(axis as List<object>);
				Axiss.Add(refrence);
			}
		}
		
		if (elements.ContainsKey("mainTransform"))
			MainAxis.Deserialize(elements["mainTransform"] as List<object>);

		
		base.Deserialize(elements);
	}
}
