#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public class Phase : ScriptableObject
{
	[SerializeField] private List<RideAnimationEvent> _events = new List<RideAnimationEvent>();
	public bool Running;
	private bool _done;

	public ReadOnlyCollection<RideAnimationEvent> Events
	{
		get { return _events.AsReadOnly(); }
	}
#if UNITY_EDITOR
	public void AddEvent(RideAnimationEvent animationEvent)
	{
		AssetDatabase.AddObjectToAsset(animationEvent, this);
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
		_events.Add(animationEvent);
	}

	public void DeleteEvent(RideAnimationEvent animationEvent)
	{
		_events.Remove(animationEvent);
		DestroyImmediate(animationEvent, true);
	}
#endif

	public void CleanUp()
	{
		for (int x = 0; x < _events.Count; x++)
		{
			if (_events[x] != null)
				DestroyImmediate(_events[x], true);
		}
	}

	public void Enter()
	{
		foreach (RideAnimationEvent RAE in _events)
		{
			RAE.Enter();
		}
	}

	public Phase ShallowCopy()
	{
		return (Phase) MemberwiseClone();
	}

	public void Run(Transform root)
	{
		foreach (RideAnimationEvent RAE in _events)
		{
			RAE.Run(root);
		}

		_done = true;
		foreach (RideAnimationEvent RAE in _events)
		{
			if (!RAE.Done)
			{
				Running = true;
				_done = false;
				break;
			}
		}

		if (_done)
		{
			Running = false;
		}

	}

	public void Exit()
	{
		foreach (RideAnimationEvent rideAnimationEvent in _events)
		{
			rideAnimationEvent.Exit();
		}
	}


	public Dictionary<string, object> Serialize(Transform root,Motor[] motors)
	{
		List<Dictionary<string, object>> eventSerialize = new List<Dictionary<string, object>>();

		foreach (var @event in _events)
		{
			Dictionary<string, object> o = @event.Serialize(root,motors);
			o.Add("@Tag", RideAnimationEvent.GetTagFromRideAnimationEvent(@event.GetType()));
			eventSerialize.Add(o);
		}

		return new Dictionary<string, object>
		{
			{"events", eventSerialize}
		};
	}

	public void Deserialize(Dictionary<string, object> elements,Motor[] motors)
	{
		
		if (elements.ContainsKey("events"))
		{
			foreach (var @event in (List<object>)elements["events"])
			{
				var ev = @event as Dictionary<string, object>;
				RideAnimationEvent rideAnimation =  (RideAnimationEvent) CreateInstance(RideAnimationEvent.FindRideAnimationTypeByTag((string) ev["@Tag"]));
				rideAnimation.Deserialize(ev,motors);
				_events.Add(rideAnimation);
			}
		}

	}
}