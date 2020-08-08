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
	/// This custom Adventure Creator action refocuses the dialogue UI's response 
    /// menu if it's active. It may be useful to use this action when closing AC's
    /// Pause menu to allow the response menu to regain controller focus.
    /// The dialogue UI must be a Unity UI Dialogue UI.
	/// </summary>
	[System.Serializable]
	public class ActionDialogueSystemRefocusResponseMenu : Action
	{
		
		public int constantID = 0;
		public string luaCode = string.Empty;
		public bool syncData = false;
		public bool updateQuestTracker = false;
		public bool storeResult = false;
		public int variableID = 0;

		public ActionDialogueSystemRefocusResponseMenu()
		{
			this.isDisplayed = true;
			category = ActionCategory.ThirdParty;
			title = "Dialogue System Refocus Response Menu";
			description = "Refocuses the response menu if active and Auto Focus is ticked.";
		}
		
		override public float Run ()
		{
            if (DialogueManager.DisplaySettings.dialogueUI != null)
            {
                var dialogueUI = DialogueManager.DisplaySettings.dialogueUI.GetComponent<UnityUIDialogueUI>();
                if (dialogueUI != null && dialogueUI.autoFocus)
                {
                    if (dialogueUI.dialogue.responseMenu.panel != null &&
                        dialogueUI.dialogue.responseMenu.panel.gameObject.activeInHierarchy)
                    {
                        dialogueUI.dialogue.responseMenu.AutoFocus();
                    }
                    else if (dialogueUI.dialogue.npcSubtitle.panel != null &&
                        dialogueUI.dialogue.npcSubtitle.panel.gameObject.activeInHierarchy)
                    {
                        dialogueUI.dialogue.npcSubtitle.AutoFocus();
                    }
                    else if (dialogueUI.dialogue.pcSubtitle.panel != null &&
                        dialogueUI.dialogue.pcSubtitle.panel.gameObject.activeInHierarchy)
                    {
                        dialogueUI.dialogue.pcSubtitle.AutoFocus();
                    }
                }
            }
            return 0;
		}
	
	}

}