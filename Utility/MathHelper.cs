﻿using System;
using UnityEngine;

public class MathHelper
{
	public static float Hermite(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, value * value * (3f - 2f * value));
	}
}
