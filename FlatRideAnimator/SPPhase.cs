using System;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.ObjectModel;

[ExecuteInEditMode]
[Serializable]
public class SPPhase : ScriptableObject
{
	[SerializeField]
	private List<SPRideAnimationEvent> events = new List<SPRideAnimationEvent>();
	public bool running = false;
	bool done = false;

	public  ReadOnlyCollection<SPRideAnimationEvent> Events{get{return events.AsReadOnly ();}}
	#if UNITY_EDITOR
	public void AddEvent(SPRideAnimationEvent animationEvent)
	{
		AssetDatabase.AddObjectToAsset (animationEvent,this);
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
		events.Add (animationEvent);
	}

	public void DeleteEvent(SPRideAnimationEvent animationEvent)
	{
		events.Remove (animationEvent);
		DestroyImmediate (animationEvent, true);
	}
	#endif

	public void CleanUp()
	{
		for(int x = 0; x < events.Count; x++)
		{
			if(events[x] != null)
				DestroyImmediate (events [x],true);
		}
	}

	public void Enter()
	{
		foreach (SPRideAnimationEvent RAE in events)
		{
			RAE.Enter();
		}
	}
	public SPPhase ShallowCopy()
	{
		return (SPPhase)this.MemberwiseClone();
	}
	public void Run(Transform root)
	{
		foreach (SPRideAnimationEvent RAE in events)
		{
			RAE.Run(root);
		}
		done = true;
		foreach (SPRideAnimationEvent RAE in events)
		{
			if (!RAE.done)
			{
				running = true;
				done = false;
				break;
			}
		}
		if (done)
		{
			running = false;
		}

	}
	public void Exit()
	{
		foreach (SPRideAnimationEvent RAE in events)
		{
			RAE.Exit();
		}
	}

	public virtual List<XElement> Serialize()
	{
		List<XElement> elements = new List<XElement> ();
		foreach (SPRideAnimationEvent rideEvent in events) {
			elements.Add (rideEvent.GetType ().Name, rideEvent.Serialize ());
		}
		return elements;
	}

}