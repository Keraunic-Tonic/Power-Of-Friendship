using UnityEngine;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.AdventureCreatorSupport;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{

	/// <summary>
	/// This custom Adventure Creator action runs Lua code in the 
	/// Dialogue System's Lua environment.
	/// </summary>
	[System.Serializable]
	public class ActionDialogueSystemLua : Action
	{
		
		public int constantID = 0;

        public int luaCodeParameterID = -1;
        public bool luaCodeFromParameter = false;

        public string luaCode = string.Empty;
		public bool syncData = false;
		public bool updateQuestTracker = false;
		public bool storeResult = false;
		public int variableID = 0;

		public ActionDialogueSystemLua ()
		{
			this.isDisplayed = true;
			category = ActionCategory.ThirdParty;
			title = "Dialogue System Lua";
			description = "Runs Lua code in the Dialogue System's Lua environment.";
		}

        override public void AssignValues(List<ActionParameter> parameters)
        {
            if (luaCodeFromParameter) luaCode = AssignString(parameters, luaCodeParameterID, luaCode);
        }

        override public float Run ()
		{
			var bridge = DialogueManager.Instance.GetComponent<AdventureCreatorBridge>();
			if (syncData && (bridge != null)) bridge.SyncAdventureCreatorToLua();
			Lua.Result luaResult = Lua.NoResult;
			if (!string.IsNullOrEmpty(luaCode)) 
			{
				luaResult = Lua.Run(luaCode, DialogueDebug.LogInfo);
			}
			if (syncData && (bridge != null)) bridge.SyncLuaToAdventureCreator();
			if (updateQuestTracker) DialogueManager.SendUpdateTracker();
			DialogueManager.CheckAlerts();
			if (storeResult)
			{
				AC.GlobalVariables.SetStringValue(variableID, luaResult.AsString);
			}
			return 0;
		}

        #if UNITY_EDITOR

        override public void ShowGUI(List<ActionParameter> parameters)
        {
            // Lua Code:
            luaCodeFromParameter = EditorGUILayout.Toggle(new GUIContent("Code is parameter?", "Tick to use a parameter value for the Lua code."), luaCodeFromParameter);
            if (luaCodeFromParameter)
            {
                luaCodeParameterID = Action.ChooseParameterGUI("Lua Code:", parameters, luaCodeParameterID, ParameterType.String);
            }
            else
            {
                luaCode = EditorGUILayout.TextField(new GUIContent("Lua Code:", "The Lua code to run"), luaCode);
            }

            // Etc:
			syncData = EditorGUILayout.Toggle(new GUIContent("Sync Data:", "Synchronize AC data with Lua environment"), syncData);
			updateQuestTracker = EditorGUILayout.Toggle(new GUIContent("Update Quest Tracker:", "Update quest tracker after running"), updateQuestTracker);
			storeResult = EditorGUILayout.Toggle(new GUIContent("Store Result?", "Store the result in a string variable"), storeResult);
			if (storeResult)
			{
				variableID = EditorGUILayout.IntField(new GUIContent("Variable ID", "ID of a global string variable to hold the result"), variableID);
			}

			AfterRunningOption ();
		}
		
		public override string SetLabel ()
		{
			// Return a string used to describe the specific action's job.
			string labelAdd = "";
            if (luaCodeFromParameter)
            {
                labelAdd = " (parameter)";
            }
            else if (!string.IsNullOrEmpty(luaCode))
            {
                labelAdd = " (" + luaCode + ")";
			}
			return labelAdd;
		}
		
		#endif
		
	}

}