using System.Collections.Generic;

namespace loaforcsSoundAPI.Core.Util.Extensions;

public static class ListExtensions {
	public static void AddUnique<T>(this List<T> list, T item) {
		if(!list.Contains(item)) list.Add(item);
	}
}