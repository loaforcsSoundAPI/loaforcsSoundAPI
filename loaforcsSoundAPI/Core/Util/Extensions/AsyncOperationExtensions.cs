using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace loaforcsSoundAPI.Core.Util.Extensions;

public static class AsyncOperationExtensions {
	public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp) {
		TaskCompletionSource<AsyncOperation> tcs = new TaskCompletionSource<AsyncOperation>();
		asyncOp.completed += operation => { tcs.SetResult(operation); };
		return ((Task) tcs.Task).GetAwaiter();
	}
}