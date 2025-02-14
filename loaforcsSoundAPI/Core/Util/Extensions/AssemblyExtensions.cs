using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace loaforcsSoundAPI.Core.Util.Extensions;

static class AssemblyExtensions {
	internal static IEnumerable<Type> GetLoadableTypes(this Assembly assembly) {
		if(assembly == null) {
			throw new ArgumentNullException(nameof(assembly));
		}

		try {
			return assembly.GetTypes();
		} catch(ReflectionTypeLoadException ex) {
			return ex.Types.Where(t => t != null);
		}
	}
}