namespace Lox;

public class OperationFailed : Exception
{
	protected OperationFailed(string message, int fileLineNumber = 0, string filePath = "") : base(message + GetClickableStacktraceLine(fileLineNumber, filePath)) { }

	private static string GetClickableStacktraceLine(int fileLineNumber = 0, string filePath = "") =>
		$@"{
			(filePath.Contains("\\")
				? $"\\n   :at {filePath.Substring(filePath.LastIndexOf('\\') + 1, filePath.Length - filePath.LastIndexOf('\\') - 1)}"
				: "")
		} in {
			(string.IsNullOrEmpty(filePath)
				? ""
				: filePath)
		} :line {
			fileLineNumber
		}";
}