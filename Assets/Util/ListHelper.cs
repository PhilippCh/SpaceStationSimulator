using System;
using System.Linq;
using System.Collections.Generic;

namespace SpaceStation.Util
{
	public static class ListHelper
	{
		public static T PopAt<T>(this List<T> list, int index)
		{
			T r = list[index];

			list.RemoveAt(index);

			return r;
		}

		public static List<T> Clone<T>(this List<T> listToClone) where T: ICloneable
		{
			return listToClone.Select(item => (T)item.Clone()).ToList();
		}
	}
}

