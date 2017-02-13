﻿using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Spark
{
	[Serializable]
	public class ConsumableProduct : Product
	{
		public enum Hand { Left, Right }
		public enum consumeanimation { generic, drink_straw, lick, with_hands }

		[SerializeField]
		public Hand hand;
		[SerializeField]
		public consumeanimation ConsumeAnimation;
		[SerializeField]
		public Tempreature Temp;
		[SerializeField]
		public int portions;

#if UNITY_EDITOR
	public override void RenderInspectorGUI ()
	{
		hand = Hand.Right;
		ConsumeAnimation = (consumeanimation)EditorGUILayout.EnumPopup("Consume Animation ", ConsumeAnimation);
		Temp = (Tempreature)EditorGUILayout.EnumPopup("Temprature ", Temp);
		portions = EditorGUILayout.IntField("Portions ", portions);

		base.RenderInspectorGUI ();
	}
#endif

	}
}