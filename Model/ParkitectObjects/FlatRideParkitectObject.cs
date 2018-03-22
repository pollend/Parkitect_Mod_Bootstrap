#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;


[ParkitectObjectTag("FlatRide")]
[Serializable]
public class FlatRideParkitectObject : ParkitectObj
{
	[SerializeField] public List<Motor> Motors = new List<Motor>();
	[SerializeField] public List<Phase> Phases = new List<Phase>();
	[SerializeField] public Phase CurrentPhase;
	private int _phaseNum;
	[SerializeField] public bool Animating;
	[SerializeField] public float Excitement;
	[SerializeField] public float Intensity;
	[SerializeField] public float Nausea;
	[SerializeField] public int XSize = 1;
	[SerializeField] public int ZSize = 1;
	[SerializeField] public Vector3 ClosedAngleRetraints;


	public FlatRideParkitectObject()
	{
	}

	public override Type[] SupportedDecorators()
	{
		return new Type[]
		{
			typeof(BaseDecorator),
			typeof(WaypointDecorator),
			typeof(BoundingBoxDecorator),
			typeof(SeatDecorator)
		};
	}

	

	
	public void Animate(Transform root)
	{
		Motors.RemoveAll(x => x == null);
		Phases.RemoveAll(x => x == null);

		foreach (Motor m in Motors)
		{
			m.Enter(root);
		}

		if (Phases.Count <= 0)
		{
			Animating = false;
			foreach (Motor m in Motors)
			{
				m.Reset(root);
			}

			foreach (MultipleRotations R in Motors.OfType<MultipleRotations>())
			{
				R.Reset(root);
			}

			return;
		}

		foreach (Motor m in Motors)
		{
			m.Enter(root);
		}

		Animating = true;
		_phaseNum = 0;
		CurrentPhase = Phases[_phaseNum];
		CurrentPhase.Running = true;
		CurrentPhase.Enter();
		CurrentPhase.Run(root);
	}

	void NextPhase(Transform root)
	{

		CurrentPhase.Exit();
		CurrentPhase.Running = false;
		_phaseNum++;
		if (Phases.Count > _phaseNum)
		{
			CurrentPhase = Phases[_phaseNum];
			CurrentPhase.Running = true;
			CurrentPhase.Enter();
			CurrentPhase.Run(root);
			return;
		}

		Animating = false;
		foreach (Rotator m in Motors.OfType<Rotator>())
		{
			m.Enter(root);

		}

		foreach (Rotator m in Motors.OfType<Rotator>())
		{
			Transform transform = m.Axis.FindSceneRefrence(root);
			if (transform != null)
				transform.localRotation = m.OriginalRotationValue;

		}

		foreach (RotateBetween m in Motors.OfType<RotateBetween>())
		{
			Transform transform = m.Axis.FindSceneRefrence(root);
			if (transform != null)
				transform.localRotation = m.OriginalRotationValue;

		}

		foreach (Mover m in Motors.OfType<Mover>())
		{
			Transform transform = m.Axis.FindSceneRefrence(root);
			if (transform != null)
				transform.localPosition = m.OriginalRotationValue;

		}

		CurrentPhase = null;
	}



	public void Run(Transform transform)
	{
		if (CurrentPhase != null)
		{
			CurrentPhase.Run(transform);
			if (!CurrentPhase.Running)
			{
				NextPhase(transform);
			}
		}
	}
#if UNITY_EDITOR
	public void AddMotor(Motor motor)
	{
		AssetDatabase.AddObjectToAsset(motor, this);
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();

		Motors.Add(motor);
	}

	public void RemoveMotor(Motor motor)
	{
		Motors.Remove(motor);
		DestroyImmediate(motor, true);
	}

	public void AddPhase(Phase phase)
	{
		AssetDatabase.AddObjectToAsset(phase, this);
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();

		Phases.Add(phase);
	}

	public void RemovePhase(Phase phase)
	{
		Phases.Remove(phase);
		DestroyImmediate(phase, true);
	}

	public override void PrepareForExport()
	{
		for (int x = 0; x < Motors.Count; x++)
		{
			Motors[x].PrepareExport(this);
		}

		base.PrepareForExport();
	}
	
	public override void CleanUp()
	{
		for (int x = 0; x < Phases.Count; x++)
		{
			if (Phases[x] != null)
			{
				Phases[x].CleanUp();
				DestroyImmediate(Phases[x], true);
			}
		}

		for (int x = 0; x < Motors.Count; x++)
		{
			if (Motors[x] != null)
			{
				DestroyImmediate(Motors[x], true);
			}
		}

		Motors.Clear();
		Phases.Clear();


		base.CleanUp();
	}

	
	public override void RenderInspectorGui()
	{

		base.RenderInspectorGui();

		GUILayout.Space(10);
		GUILayout.Label("Rating", EditorStyles.boldLabel);
		Excitement = EditorGUILayout.Slider("Excitement (" + getRatingCategory(Excitement) + ")", Excitement, 0, 100);
		Intensity = EditorGUILayout.Slider("Intensity (" + getRatingCategory(Intensity) + ")", Intensity, 0, 100);
		Nausea = EditorGUILayout.Slider("Nausea (" + getRatingCategory(Nausea) + ")", Nausea, 0, 100);
		GUILayout.Space(10);
		ClosedAngleRetraints = EditorGUILayout.Vector3Field("Closed Restraints Angle", ClosedAngleRetraints);

		GUILayout.Space(10);
		GUI.color = Color.white;
		XSize = EditorGUILayout.IntField("X", XSize);
		ZSize = EditorGUILayout.IntField("Z", ZSize);

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

					WaypointDecorator waypointDecorator = (WaypointDecorator) GetDecorator(typeof(WaypointDecorator), false);
					SPWaypoint newWaypoint = new SPWaypoint();
					newWaypoint.localPosition = new Vector3(x + 0.5f, waypointDecorator.HelperPlaneY, z + 0.5f);
					newWaypoint.isOuter = true;
					waypointDecorator.Waypoints.Add(newWaypoint);
				}
			}
		}

	}

	public override void RenderSceneGui()
	{
		GameObject refrence = getGameObjectRef(false);
		if (refrence == null)
			return;

		Vector3 topLeft = new Vector3(-XSize / 2.0f, 0, ZSize / 2.0f) + refrence.transform.position;
		Vector3 topRight = new Vector3(XSize / 2.0f, 0, ZSize / 2.0f) + refrence.transform.position;
		Vector3 bottomLeft = new Vector3(-XSize / 2.0f, 0, -ZSize / 2.0f) + refrence.transform.position;
		Vector3 bottomRight = new Vector3(XSize / 2.0f, 0, -ZSize / 2.0f) + refrence.transform.position;

		Color fill = Color.white;
		fill.a = 0.1f;
		Handles.zTest = CompareFunction.LessEqual;
		Handles.DrawSolidRectangleWithOutline(new[] {topLeft, topRight, bottomRight, bottomLeft}, fill, Color.black);

		base.RenderSceneGui();
	}
#endif


#if (PARKITECT)
	private CustomFlatRide _flatRide;
	public override void BindToParkitect(GameObject hider, AssetBundle bundle)
	{
		BaseDecorator baseDecorator = this.DecoratorByInstance<BaseDecorator>();
		WaypointDecorator waypointDecorator = this.DecoratorByInstance<WaypointDecorator>();
		FlatrideDecorator flatrideDecorator = this.DecoratorByInstance<FlatrideDecorator>();
		BoundingBoxDecorator boundingBoxDecorator = this.DecoratorByInstance<BoundingBoxDecorator>();
		AnimatorDecorator animatorDecorator = this.DecoratorByInstance<AnimatorDecorator>();

		GameObject gameObject = Instantiate(bundle.LoadAsset<GameObject>(Key));

		waypointDecorator.Decorate(gameObject, hider, this, bundle);
		flatrideDecorator.Decorate(gameObject, hider, this, bundle);


		CustomFlatRide flatride = gameObject.AddComponent<CustomFlatRide>();
		_flatRide = flatride;
		_flatRide.name = Key;
		flatride.xSize = flatrideDecorator.XSize;
		flatride.zSize = flatrideDecorator.ZSize;
		flatride.excitementRating = flatrideDecorator.Excitement;
		flatride.intensityRating = flatrideDecorator.Intensity;
		flatride.nauseaRating = flatrideDecorator.Nausea;


		//Setup Animation
		flatride.motors = new List<SPMotor>(animatorDecorator.Motors);
		flatride.phases = new List<SPPhase>(animatorDecorator.Phases);


		//Basic FlatRide Settings
		flatride.fenceStyle = AssetManager.Instance.rideFenceStyles.rideFenceStyles[0].identifier;
		flatride.entranceGO = Extra.FlatRideEntrance(flatride.gameObject);
		flatride.exitGO = AssetManager.Instance.attractionExitGO;
		flatride.categoryTag = "Attractions/Flat Ride";
		flatride.defaultEntranceFee = 1f;
		flatride.entranceExitBuilderGO = AssetManager.Instance.flatRideEntranceExitBuilderGO;
		AssetManager.Instance.registerObject(_flatRide);
	}

	public override void UnBindToParkitect(GameObject hider)
	{
		AssetManager.Instance.unregisterObject(_flatRide);
	}
#endif

	private string getRatingCategory(float ratingValue)
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
		foreach (var ele in (List<object>) elements["Phases"])
		{
			Phase phase = CreateInstance<Phase>();
			phase.Deserialize(ele as Dictionary<string, object>);
			Phases.Add(phase);
		}

		foreach (var ele in (List<object>) elements["Motors"])
		{
			Dictionary<string, object> serializedMotor = ele as Dictionary<string, object>;
			Motor motor = (Motor) CreateInstance(Motor.FindMotorTypeByTag((string) serializedMotor["@Tag"]));
			motor.Deserialize(serializedMotor);
			Motors.Add(motor);
		}

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
	}


	public override Dictionary<string, object> Serialize()
	{
		Dictionary<string, object> result = base.Serialize();
		List<Dictionary<string, object>> serializedPhases = new List<Dictionary<string, object>>();
		foreach (var phase in Phases)
		{
			serializedPhases.Add(phase.Serialize(Prefab.transform));
		}

		List<Dictionary<string, object>> serializedMotors = new List<Dictionary<string, object>>();
		foreach (var motor in Motors)
		{
			Dictionary<string, object> serializedMotor = motor.Serialize(Prefab.transform);
			serializedMotor.Add("@Tag", Motor.GetTagFromMotor(motor.GetType()));
			serializedMotors.Add(serializedMotor);
		}


		result.Add("Excitement", Excitement);
		result.Add("Intensity", Intensity);
		result.Add("Nausea", Nausea);
		result.Add("XSize", XSize);
		result.Add("ZSize", ZSize);

		result.Add("Phases", serializedPhases);
		result.Add("Motors", serializedMotors);
		return result;
	}

}


