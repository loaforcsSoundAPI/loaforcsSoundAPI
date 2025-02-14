using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Logging;

namespace loaforcsSoundAPI.Core.Data;

/// <summary>
/// Allows validation of loaded data types.
/// </summary>
public interface IValidatable {
	/// <summary>
	/// Run validations
	/// </summary>
	/// <returns>Non-successful validations</returns>
	public List<ValidationResult> Validate();
	
	public enum ResultType {
		WARN,
		FAIL
	}

	/// <summary>
	/// A data class representing a non-succesful validation.
	/// </summary>
	/// <param name="resultType">Non-success result type</param>
	/// <param name="reason">Description of issue</param>
	public class ValidationResult(ResultType resultType, string reason = null) {
		public ResultType Status { get; private set; } = resultType;
		public string Reason { get; private set; } = reason ?? string.Empty;
	}

	private static readonly StringBuilder _stringBuilder = new();
	
	internal static bool LogAndCheckValidationResult(string context, List<ValidationResult> results, ManualLogSource logger) {
		if (results.Count == 0) {
			return true;
		}
		
		// it's a bit icky that i have to loop twice over it but it doesn't really matter and this formatting looks nice so :3
		// also none of this code should ever be ran by end users given that sound pack creators do the right thing
			
		int warns = 0, fails = 0;
			
		foreach (ValidationResult result in results) {
			switch (result.Status) {
				case ResultType.WARN:
					warns++;
					break;
				case ResultType.FAIL:
					fails++;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		_stringBuilder.Clear();

		if (fails != 0) {
			_stringBuilder.Append(fails);
			_stringBuilder.Append(" fail(s)");
		}
            
		if (warns != 0) {
			if (fails != 0) { // both warnings and fails were present
				_stringBuilder.Append(" and ");
			}
				
			_stringBuilder.Append(warns);
			_stringBuilder.Append(" warning(s)");
		}

		_stringBuilder.Append(" while ");
		_stringBuilder.Append(context);
		_stringBuilder.Append(": ");
			
		if (fails != 0) {
			logger.LogError(_stringBuilder);
		} else {
			logger.LogWarning(_stringBuilder);
		}
			
		foreach (ValidationResult result in results) {
			switch (result.Status) {
				case ResultType.WARN:
					logger.LogWarning($"WARN: {result.Reason}");
					break;
				case ResultType.FAIL:
					logger.LogError($"FAIL: {result.Reason}");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		return fails != 0;
	}
}