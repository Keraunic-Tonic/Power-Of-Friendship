using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.AdventureCreatorSupport;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{

	/// <summary>
	/// This custom action checks the result of a Lua expression in the Dialogue System.
	/// Contributed by Chad Kilgore.
	/// </summary>
    [System.Serializable]
    public class ActionDialogueSystemVarCheck : ActionCheck
    {
        public int constantID = 0;
        public string luaCode = string.Empty;
        public int variableID = 0;
		public bool syncData = false;

        public ActionDialogueSystemVarCheck()
        {
            this.isDisplayed = true;
            category = ActionCategory.ThirdParty;
            title = "Dialogue System Check";
            description = "Evaluates a Lua expression in the Dialogue System.";
        }
        
        override public ActionEnd End (List<AC.Action> actions)
        {
            if (variableID == -1)
            {
                return GenerateStopActionEnd();
            }

            bool luaResult = false;
            try
            {
				var bridge = DialogueManager.Instance.GetComponent<AdventureCreatorBridge>();
				if (syncData && (bridge != null)) bridge.SyncAdventureCreatorToLua();
				if (!string.IsNullOrEmpty(luaCode)) 
                    luaResult = Lua.IsTrue(luaCode, DialogueDebug.LogInfo);
            }
            catch
            {
                Debug.LogError("'luaCode' cannot be evaluated: " + luaCode);
            }

            return ProcessResult(luaResult, actions);
        }
        
        #if UNITY_EDITOR
        
        override public void ShowGUI ()
        {
            // Action-specific Inspector GUI code here
            luaCode = EditorGUILayout.TextField(new GUIContent("Lua Code:", "The Lua code to evaluate"), luaCode);
			syncData = EditorGUILayout.Toggle(new GUIContent("Sync Data:", "Synchronize AC data with Lua environment before evaluating"), syncData);
		}
        
        public override string SetLabel()
        {
            // Return a string used to describe the specific action's job.
            
            string labelAdd = "";
            if (!string.IsNullOrEmpty(luaCode))
            {
                labelAdd = " (" + luaCode + ")";
            }
            return labelAdd;
        }
        
        #endif
    }
}