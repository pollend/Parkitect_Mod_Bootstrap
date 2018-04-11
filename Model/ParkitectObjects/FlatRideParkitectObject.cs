#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ParkitectObjectTag("FlatRide")]
[Serializable]
public class FlatRideParkitectObject : ParkitectObj
{
	public static readonly string[] ATTRACTION_CATEGORY_TAGS = new string[5]
	{
		"Attractions/Calm Rides",
		"Attractions/Thrill Rides",
		"Attractions/Coasters",
		"Attractions/Transport Rides",
		"Attractions/Water Rides"
	};
	
	public static readonly string[] ATTRACTION_CATEGORY_INDEX = new string[5]
	{
		"Calm Rides",
		"Thrill Rides",
		"Coasters",
		"Transport Rides",
		"Water Rides"
	};

	[SerializeField] public float Excitement;
	[SerializeField] public float Intensity;
	[SerializeField] public float Nausea;
	[SerializeField] public int XSize = 1;
	[SerializeField] public int ZSize = 1;
	[SerializeField] public Vector3 ClosedAngleRetraints;
	[SerializeField] public string FlatRideCategory = ATTRACTION_CATEGORY_TAGS[0];
	

	public override Type[] SupportedDecorators()
	{
		return new[]
		{
			typeof(BaseDecorator),
			typeof(WaypointDecorator),
			typeof(BoundingBoxDecorator),
			typeof(SeatDecorator),
			typeof(ColorDecorator)
		};
	}



#if UNITY_EDITOR

	public override void RenderInspectorGui()
	{

		base.RenderInspectorGui();

		GUILayout.Space(10);
		GUILayout.Label("Rating", EditorStyles.boldLabel);
		Excitement = EditorGUILayout.Slider("Excitement (" + GetRatingCategory(Excitement) + ")", Excitement, 0, 1);
		Intensity = EditorGUILayout.Slider("Intensity (" + GetRatingCategory(Intensity) + ")", Intensity, 0, 1);
		Nausea = EditorGUILayout.Slider("Nausea (" + GetRatingCategory(Nausea) + ")", Nausea, 0, 1);
		GUILayout.Space(10);
		ClosedAngleRetraints = EditorGUILayout.Vector3Field("Closed Restraints Angle", ClosedAngleRetraints);

		GUILayout.Space(10);
		GUI.color = Color.white;
		XSize = EditorGUILayout.IntField("X", XSize);
		ZSize = EditorGUILayout.IntField("Z", ZSize);

		FlatRideCategory = ATTRACTION_CATEGORY_TAGS[EditorGUILayout.Popup ("category", Array.IndexOf(ATTRACTION_CATEGORY_TAGS,FlatRideCategory), ATTRACTION_CATEGORY_INDEX)];

		WaypointDecorator waypointDecorator = (WaypointDecorator) GetDecorator(typeof(WaypointDecorator), false);
		if (waypointDecorator != null && waypointDecorator.EnableEditing)
		{
			if (GUILayout.Button("Generate outer grid"))
			{

				float minX = -XSize / 2;
				float maxX = XSize / 2;
				float minZ = -ZSize / 2;
				float maxZ = ZSize / 2;
				for (int xi = 0; xi < Mathf.RoundToInt(maxX - minX); xi++)
				{
					for (int zi = 0; zi < Mathf.RoundToInt(maxZ - minZ); zi++)
					{
						float x = minX + xi;
						float z = minZ + zi;
						if (!(x == minX || x == maxX - 1) && !(z == minZ || z == maxZ - 1))
						{
							continue;
						}

						SPWaypoint newWaypoint = new SPWaypoint();
						newWaypoint.localPosition = new Vector3(x + 0.5f, waypointDecorator.HelperPlaneY, z + 0.5f);
						newWaypoint.isOuter = true;
						waypointDecorator.Waypoints.Add(newWaypoint);
					}
				}
			}
		}

	}

	public override void RenderSceneGui()
	{
		GameObject refrence = GetGameObjectRef(false);
		
		if (refrence == null)
			return;

		Vector3 topLeft = new Vector3(-((float)XSize) / 2.0f, 0, (float)ZSize / 2.0f) + refrence.transform.position;
		Vector3 topRight = new Vector3(((float)XSize) / 2.0f, 0, (float)ZSize / 2.0f) + refrence.transform.position;
		Vector3 bottomLeft = new Vector3(-((float)XSize) / 2.0f, 0, -(float)ZSize / 2.0f) + refrence.transform.position;
		Vector3 bottomRight = new Vector3(((float)XSize) / 2.0f, 0, -(float)ZSize / 2.0f) + refrence.transform.position;

		Color fill = Color.white;
		fill.a = 0.1f;
		Handles.zTest = CompareFunction.LessEqual;
		Handles.color = Color.white;
		Handles.DrawSolidRectangleWithOutline(new[] {topLeft, topRight, bottomRight, bottomLeft}, fill, Color.black);

		base.RenderSceneGui();
	}
#endif


#if (PARKITECT)
	private CustomFlatRide _flatRide;
	public override void BindToParkitect(GameObject hider, AssetBundle bundle)
	{
		BaseDecorator baseDecorator = DecoratorByInstance<BaseDecorator>();
		WaypointDecorator waypointDecorator = DecoratorByInstance<WaypointDecorator>();
		BoundingBoxDecorator boundingBoxDecorator = DecoratorByInstance<BoundingBoxDecorator>();
		ColorDecorator colorDecorator = DecoratorByInstance<ColorDecorator>();
		
		GameObject gameObject = Instantiate(bundle.LoadAsset<GameObject>(Key));
		RemapUtility.RemapMaterials(gameObject);

		waypointDecorator.Decorate(gameObject, hider, this, bundle);

		
		CustomFlatRide flatride = gameObject.AddComponent<CustomFlatRide>();
		baseDecorator.Decorate(gameObject,hider,this,bundle);
		colorDecorator.Decorate(gameObject,hider,this,bundle);
		
		
		_flatRide = flatride;
		_flatRide.name = Key;
		flatride.xSize = XSize;
		flatride.zSize = ZSize;
		flatride.excitementRating = Excitement;
		flatride.intensityRating = Intensity;
		flatride.nauseaRating = Nausea;
		

		RestraintRotationController controller = gameObject.AddComponent<RestraintRotationController>();
		controller.closedAngles = ClosedAngleRetraints;


		//Basic FlatRide Settings
		flatride.fenceStyle = AssetManager.Instance.rideFenceStyles.rideFenceStyles[0].identifier;
		flatride.entranceGO = AssetManager.Instance.attractionEntranceGO;
		flatride.exitGO = AssetManager.Instance.attractionExitGO;
		flatride.categoryTag = FlatRideCategory;
		flatride.defaultEntranceFee = 1f;
		flatride.entranceExitBuilderGO = AssetManager.Instance.flatRideEntranceExitBuilderGO;
		
		AssetManager.Instance.registerObject(_flatRide);
	}

	public override void UnBindToParkitect(GameObject hider)
	{
		AssetManager.Instance.unregisterObject(_flatRide);
	}
#endif

	private string GetRatingCategory(float ratingValue)
	{
		ratingValue /= 100f;
		if (ratingValue > 0.9f)
		{
			return "Very High";
		}

		if (ratingValue > 0.6f)
		{
			return "High";
		}

		if (ratingValue > 0.3f)
		{
			return "Medium";
		}

		return "Low";
	}


	public override void DeSerialize(Dictionary<string, object> elements)
	{
		if (elements.ContainsKey("Excitement"))
			Excitement = Convert.ToSingle(elements["Excitement"]);
		if (elements.ContainsKey("Intensity"))
			Intensity = Convert.ToSingle(elements["Intensity"]);
		if (elements.ContainsKey("Nausea"))
			Nausea = Convert.ToSingle(elements["Nausea"]);
		if (elements.ContainsKey("XSize"))
			XSize = Convert.ToInt32(elements["XSize"]);
		if (elements.ContainsKey("ZSize"))
			ZSize = Convert.ToInt32(elements["ZSize"]);
		if (elements.ContainsKey("Category"))
			FlatRideCategory = (string) elements["Category"];
		base.DeSerialize(elements);
	}


	public override Dictionary<string, object> Serialize()
	{
		Dictionary<string, object> result = base.Serialize();

		result.Add("Excitement", Excitement);
		result.Add("Intensity", Intensity);
		result.Add("Nausea", Nausea);
		result.Add("XSize", XSize);
		result.Add("ZSize", ZSize);
		result.Add("Category", FlatRideCategory);

		return result;
	}

}


