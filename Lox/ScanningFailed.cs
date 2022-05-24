namespace Lox;

public class ScanningFailed : Exception
{
	protected ScanningFailed(string message, int fileLineNumber, string filePath) : base(message + GetClickableStacktraceLine(fileLineNumber, filePath)) { }

	private static string GetClickableStacktraceLine(int fileLineNumber, string filePath) =>
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