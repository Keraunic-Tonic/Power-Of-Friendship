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
	/// This custom Adventure Creator action plays a Dialogue System conversation.
	/// </summary>
	[System.Serializable]
	public class ActionDialogueSystemConversation : Action
	{

        public enum ActionMode { StartConversation, StopConversation }

        public ActionMode actionMode = ActionMode.StartConversation;

		public int constantID = 0;

        public int conversationParameterID = -1;
        public bool conversationFromParameter = false;

        public string conversation = string.Empty;
        public bool specifyEntryID = false;
        public int entryID = -1;
		public Transform actor = null;
		public Transform conversant = null;
        public bool overrideBridge = false;
        public UseDialogState useDialogState = UseDialogState.IfPlayerInvolved;
        public UseDialogState takeCameraControl = UseDialogState.IfPlayerInvolved;
        public bool waitUntilFinish = true;
		public bool stopConversationOnSkip = true;

        private AdventureCreatorBridge bridge = null;
        private ActiveConversationRecord activeConversationRecord = null;

		public ActionDialogueSystemConversation ()
		{
			this.isDisplayed = true;
			category = ActionCategory.ThirdParty;
			title = "Dialogue System Conversation";
			description = "Starts a Dialogue System conversation. Leave Actor blank to default to the player.";
		}

        override public void AssignValues(List<ActionParameter> parameters)
        {
            if (conversationFromParameter) conversation = AssignString(parameters, conversationParameterID, conversation);
        }

        override public float Run ()
		{
			if (!isRunning)
			{

                // If StopConversation, just stop and exit immediately:
                if (actionMode == ActionMode.StopConversation)
                {
                    DialogueManager.StopConversation();
                    return 0;
                }

				// Sync AC data to Lua:
                if (bridge == null) bridge = DialogueManager.Instance.GetComponent<AdventureCreatorBridge>();
				if (bridge != null) {
					bridge.SyncAdventureCreatorToLua();
					bridge.skipSyncToLua = true;
                    bridge.overrideBridge = overrideBridge;
                    bridge.overrideUseDialogState = useDialogState;
                    bridge.overrideTakeCameraControl = takeCameraControl;
				}

				// If the actor is null, use the player's transform:
				if (actor == null) {
					GameObject actorGameObject = GameObject.FindGameObjectWithTag("Player");
					if (actorGameObject != null) actor = actorGameObject.transform;
				}

                // Start the conversation:
                if (specifyEntryID)
                {
                    DialogueManager.StartConversation(conversation, actor, conversant, entryID);
                }
                else
                {
                    DialogueManager.StartConversation(conversation, actor, conversant);
                }

                // Get the active conversation record, in case we're running multiple simultaneous conversations:
                activeConversationRecord = null;
                if (waitUntilFinish && DialogueManager.allowSimultaneousConversations)
                {
                    if (DialogueManager.Instance.activeConversations != null && DialogueManager.Instance.activeConversations.Count > 0)
                    {
                        activeConversationRecord = DialogueManager.instance.activeConversations[DialogueManager.instance.activeConversations.Count - 1];
                    }
                }

				isRunning = true;
				return waitUntilFinish ? defaultPauseTime : 0;
			}
			else
			{
				isRunning = (activeConversationRecord != null) ? activeConversationRecord.isConversationActive : DialogueManager.IsConversationActive;
				return (isRunning && waitUntilFinish) ? defaultPauseTime : 0;
			}
		}

		override public void Skip ()
		{
			if (stopConversationOnSkip) DialogueManager.StopConversation();
		}

        public override ActionEnd End(List<Action> actions)
        {
            if (bridge == null) bridge = DialogueManager.Instance.GetComponent<AdventureCreatorBridge>();
            if (bridge != null) bridge.overrideBridge = false;
            return base.End(actions);
        }

#if UNITY_EDITOR

        public ActionConversationPicker conversationPicker = null;
		public DialogueDatabase selectedDatabase = null;
		public bool usePicker = true;

        override public void ShowGUI(List<ActionParameter> parameters)
        {
            // Action mode:
            actionMode = (ActionMode)EditorGUILayout.EnumPopup("Action:", actionMode);
            if (actionMode != ActionMode.StopConversation)
            {

                // Conversation:
                conversationFromParameter = EditorGUILayout.Toggle(new GUIContent("Conversation is parameter?", "Tick to use a parameter value for the conversation title."), conversationFromParameter);
                if (conversationFromParameter)
                {
                    conversationParameterID = Action.ChooseParameterGUI("Conversation:", parameters, conversationParameterID, ParameterType.String);
                }
                else
                {
                    if (conversationPicker == null)
                    {
                        conversationPicker = new ActionConversationPicker(selectedDatabase, conversation, usePicker);
                    }
                    conversationPicker.Draw();
                    conversation = conversationPicker.currentConversation;
                }

                // Entry ID:
                specifyEntryID = EditorGUILayout.Toggle(new GUIContent("Specify entry ID?", "Tick to specify the entry ID to start at."), specifyEntryID);
                if (specifyEntryID)
                {
                    entryID = EditorGUILayout.IntField(new GUIContent("Start at entry ID:"), entryID);
                }

                // Actor, Conversant, etc:
                actor = (Transform)EditorGUILayout.ObjectField(new GUIContent("Actor:", "The primary speaker, usually the player."), actor, typeof(Transform), true);
                conversant = (Transform)EditorGUILayout.ObjectField(new GUIContent("Conversant:", "The other speaker, usually an NPC"), conversant, typeof(Transform), true);
                overrideBridge = EditorGUILayout.Toggle(new GUIContent("Override bridge control?", "Use different game state and camera control settings for this conversation than what's configured on the Adventure Creator Bridge component"), overrideBridge);
                if (overrideBridge)
                {
                    useDialogState = (UseDialogState)EditorGUILayout.EnumPopup(new GUIContent("Use dialog state:", "Specifies whether this conversation should switch to AC's dialog state, which pauses the game and shows the cursor"), useDialogState);
                    takeCameraControl = (UseDialogState)EditorGUILayout.EnumPopup(new GUIContent("Take camera control:", "Specifies whether this conversation should take camera control"), takeCameraControl);
                }
                waitUntilFinish = EditorGUILayout.Toggle(new GUIContent("Wait until finish?", "Wait until the conversation ends"), waitUntilFinish);
                stopConversationOnSkip = EditorGUILayout.Toggle(new GUIContent("Stop on skip?", "Stop the conversation if the player skips the cutscene"), stopConversationOnSkip);
            }

			AfterRunningOption ();
		}
		
		public override string SetLabel ()
		{
			// Return a string used to describe the specific action's job.
			
			string labelAdd = "";
            if (conversationFromParameter)
            {
                labelAdd = " (parameter)";
            }
            else if (!string.IsNullOrEmpty(conversation))
            {
                labelAdd = " (" + conversation + ")";
			}
			return labelAdd;
		}
		
		#endif
		
	}

}
