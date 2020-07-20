using System.Linq;

namespace BaseLibrary.Utility
{
	public static class MathUtility
	{
		public static T Min<T>(params T[] array) => array.Min();
	}
}