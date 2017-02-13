using UnityEngine;
using System.Collections;
using System;

namespace Spark
{
	[Serializable]
	public class BoundingBox
	{
		public Bounds bounds;
		private Bounds liveBounds;
	}
}