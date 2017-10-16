using UnityEngine;
using System;

/// <summary>
///
/// Replacement for the Debug.Log 
/// 
/// 
/// Utility to send Debug log messages to console while in the editor, but hides in production.
/// 
/// If you need to show the debug messages outside of the editor, you can add a *Scripting Define Symbols* to the project.
/// To set the global define from the application menu:
/// **EDIT** > **Project Settings** > **Player** 
/// Then enter the define name of `SHOW_DEBUG_MESSAGES` in the "*Scripting Define Symbols*" field. 
/// 
/// ###### Example Usage
/// 
/// Logger.Log ("This is an error message", Logger.LOG_TYPE.Error);
/// Logger.Log ("This is a warning message", Logger.LOG_TYPE.Warning);
/// Logger.Log ("This is just some useful info", Logger.LOG_TYPE.Info);
/// Logger.Log ("This is a normal log message", Logger.LOG_TYPE.Log);
/// Logger.Log ("This is a regular log");
/// 
/// </summary>



public class Logger {
	
	public enum LOG_TYPE {
		Error,
		Warning,
		Log,
		Info
	}

	public static void Log (string logMsg) {
		#if (UNITY_EDITOR || SHOW_DEBUG_MESSAGES)
		Debug.Log (logMsg);
		#endif
	}

	public static void Log (string logMsg, LOG_TYPE lt) {
		#if (UNITY_EDITOR || SHOW_DEBUG_MESSAGES)

		//HERE INCASE YOU WANT TO FORMAT THE DATE / TIME TO YOUR STANDARDS
		DateTime now = DateTime.Now;

		//EACH LOG TYPE IS CUSTOMIZABLE TO FIT YOUR COLOR NEEDS
		switch (lt) {
		case LOG_TYPE.Error:
			Debug.LogError ("<color=black>[ " + now + " ]</color> <color=red><b>" + logMsg + "</b></color>");
			break;
			
		case LOG_TYPE.Warning:
			Debug.LogWarning ("<color=black>[ " + now + " ]</color> <color=yellow><b>" + logMsg + "</b></color>");
			break;

		case LOG_TYPE.Info:
			Debug.Log ("<color=black>[ " + now + " ]</color> <color=navy><b>" + logMsg + "</b></color>");
			break;

		case LOG_TYPE.Log:
			Debug.Log ("<color=black>[ " + now + " ]</color><b>" + logMsg + "</b>");
			break;
			
		default:
			Debug.Log ("[ " + now + " ] " + logMsg);
			break;
		}
		#endif

	}


}

