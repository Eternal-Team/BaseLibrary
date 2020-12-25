using System;

namespace BaseLibrary.Utility
{
	public static class MathUtility
	{
		public static ulong Min(ulong a, ulong b, ulong c)
		{
			return Math.Min(a, Math.Min(b, c));
		}
		
		public static float Min(float a, float b, float c, float d)
		{
			return Math.Min(a, Math.Min(b, Math.Min(c, d)));
		}
		
		public static float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh) 
		{
			return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
		}

		public static float Lerp(float value, float min, float max)
		{
			return min * (1 - value) + max * value;
		}
	}
}