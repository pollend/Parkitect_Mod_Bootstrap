#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using System.Linq;
namespace Spark
{
	[ExecuteInEditMode]
	[Serializable]
	public class ChangePendulum : RideAnimationEvent
	{
		[SerializeField]
		public PendulumRotator rotator;
		public float Friction = 20f;
		public bool Pendulum;

		#if UNITY_EDITOR
		float lastTime;
		private float startPendulumPosition;
		private float windUpAngleTarget = 100f;
		#endif

		public override string EventName
		{
			get
			{
				return "ChangePendulum";
			}
		}
		public override void Bootstrap()
		{
			base.Bootstrap();
		}

#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
    {

        if (rotator)
        {
            ColorIdentifier = rotator.ColorIdentifier;
            Friction = EditorGUILayout.FloatField("Friction", Friction);
            Pendulum = EditorGUILayout.Toggle("Pendulum", Pendulum);
        }
		foreach (PendulumRotator R in motors.OfType<PendulumRotator>().ToList())
        {
            if (R == rotator)
                GUI.color = Color.red / 1.3f;
            if (GUILayout.Button(R.Identifier))
            {
                rotator = R;

            }
            GUI.color = Color.white;
        }
		base.RenderInspectorGUI(motors);
    }
#endif

		public override void Enter()
		{
			rotator.setActAsPendulum(Pendulum);
			rotator.angularFriction = Friction;
			done = true;
		}
		public override void Run(Transform root)
		{
			if (rotator)
			{

			}

		}
	}
}