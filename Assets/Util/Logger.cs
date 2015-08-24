using UnityEngine;
using System.Collections;

using SpaceStation;

namespace SpaceStation.Util {

	public class Logger {

		public static void Info(string callee, string message, params object[] parameters) {
			Debug.Log(FormatLogMessage(callee, message, parameters));
		}

		public static void Warn(string callee, string message, params object[] parameters) {
			Debug.LogWarning(FormatLogMessage(callee, message, parameters));
		}

		public static void Error(string callee, string message, params object[] parameters) {
			Debug.LogError(FormatLogMessage(callee, message, parameters));
		}

		public static void QuickInfo(string message) {
			if (!Application.isEditor) {
				Logger.Warn("Logger", "Using QuickInfo in Release mode.");
			}

			Debug.Log(message);
		}

		private static string FormatLogMessage(string callee, string message, params object[] parameters) {
			string formattedMessage;
			
			// Prepend calling class name to ease debugging
			formattedMessage = string.Format("[{0}] ", callee);
			formattedMessage += string.Format(message, parameters);

			return formattedMessage;
		}

	}

}