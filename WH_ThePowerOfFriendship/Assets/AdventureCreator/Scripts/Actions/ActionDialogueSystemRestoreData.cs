using UnityEngine;
using System.Collections;
using AC;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.AdventureCreatorSupport;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{

	/// <summary>
	/// This custom Adventure Creator action restores the Dialogue System's data
	/// that was previously saved in an Adventure Creator global variable with the
	/// Dialogue System Save Data action.
	/// </summary>
	[System.Serializable]
	public class ActionDialogueSystemRestoreData : Action
	{
		
		public int constantID = 0;

		public ActionDialogueSystemRestoreData ()
		{
			this.isDisplayed = true;
			category = ActionCategory.ThirdParty;
			title = "Dialogue System Restore Data";
			description = "Restores the Dialogue System's data from an Adventure Creator global variable.";
		}
		
		
		override public float Run ()
		{
			/* 
			 * This function is called when the action is performed.
			 * 
			 * The float to return is the time that the game
			 * should wait before moving on to the next action.
			 * Return 0f to make the action instantenous.
			 * 
			 * For actions that take longer than one frame,
			 * you can return "defaultPauseTime" to make the game
			 * re-run this function a short time later. You can
			 * use the isRunning boolean to check if the action is
			 * being run for the first time, eg: 
			 */

			AdventureCreatorBridge.LoadDialogueSystemFromGlobalVariable();

			return 0;
		}
		
		
		#if UNITY_EDITOR

		/*
		override public void ShowGUI ()
		{
			// Action-specific Inspector GUI code here
			luaCode = EditorGUILayout.TextField(new GUIContent("Lua Code:", "The Lua code to run"), luaCode);
			syncData = EditorGUILayout.Toggle(new GUIContent("Sync Data:", "Synchronize AC data with Lua environment"), syncData);
			updateQuestTracker = EditorGUILayout.Toggle(new GUIContent("Update Quest Tracker:", "Update quest tracker after running"), updateQuestTracker);

			AfterRunningOption ();
		}
		
		
		public override string SetLabel ()
		{
			// Return a string used to describe the specific action's job.
			
			string labelAdd = "";
			if (!string.IsNullOrEmpty(luaCode))
			{
				labelAdd = " (" + luaCode + ")";
			}
			return labelAdd;
		}*/
		
		#endif
		
	}

}