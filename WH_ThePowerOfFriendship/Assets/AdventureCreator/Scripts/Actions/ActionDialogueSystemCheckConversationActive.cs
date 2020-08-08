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
	/// This custom action does a checks the result of a Lua expression in the Dialogue System.
	/// Contributed by Chad Kilgore.
	/// </summary>
    [System.Serializable]
    public class ActionDialogueSystemCheckConversationActive : ActionCheck
    {
        public ActionDialogueSystemCheckConversationActive()
        {
            this.isDisplayed = true;
            category = ActionCategory.ThirdParty;
            title = "Dialogue System Check Is Conversation Active";
            description = "Checks if a Dialogue System conversation is active.";
        }
        
        override public ActionEnd End (List<AC.Action> actions)
        {
            return ProcessResult(DialogueManager.IsConversationActive, actions);
        }
        
    }
}