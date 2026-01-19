using Serilog;

namespace F360.JobsProcessor.API;

public static class Logger {
	public static void Info(string message) {
		Log.Information(message);
	}

	public static void Error(string message) {
		Log.Error(message);
	}

	public static void Debug(string message) {
		Log.Debug(message);
	}

	public static void Fatal(string message) {
		Log.Fatal(message);
	}

	public static void Fatal(Exception exception) {
		Log.Fatal(exception, exception.Message);
	}

	public static void Warning(string message) {
		Log.Warning(message);
	}

	public static void Verbose(string message) {
		Log.Verbose(message);
	}
}