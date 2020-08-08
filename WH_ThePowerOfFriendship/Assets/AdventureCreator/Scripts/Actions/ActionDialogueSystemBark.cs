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
	/// This custom Adventure Creator action plays a Dialogue System bark.
	/// The actor should have a bark UI component.
	/// </summary>
	[System.Serializable]
	public class ActionDialogueSystemBark : Action
	{
		
		public int constantID = 0;

        public int conversationParameterID = -1;
        public bool conversationFromParameter = false;

        public string conversation = string.Empty;
		public Transform actor = null;
		public Transform conversant = null;
		public bool syncData = false;
        public bool allowDuringConversations = true;
		
		public ActionDialogueSystemBark ()
		{
			this.isDisplayed = true;
			category = ActionCategory.ThirdParty;
			title = "Dialogue System Bark";
			description = "Plays a Dialogue System bark. Leave Actor blank to default to the player (the target of the bark).";
		}

        override public void AssignValues(List<ActionParameter> parameters)
        {
            if (conversationFromParameter) conversation = AssignString(parameters, conversationParameterID, conversation);
        }

        override public float Run ()
		{
            if (allowDuringConversations || !DialogueManager.IsConversationActive)
            {
                if (actor == null)
                {
                    GameObject actorGameObject = GameObject.FindGameObjectWithTag("Player");
                    if (actorGameObject != null) actor = actorGameObject.transform;
                }
                var bridge = DialogueManager.Instance.GetComponent<AdventureCreatorBridge>();
                if (syncData && (bridge != null)) bridge.SyncAdventureCreatorToLua();
                DialogueManager.Bark(conversation, actor, conversant);
                if (syncData && (bridge != null)) bridge.SyncAdventureCreatorToLua();
            }

			return 0;
		}
		
		
		#if UNITY_EDITOR
		
		public ActionConversationPicker conversationPicker = null;
		public DialogueDatabase selectedDatabase = null;
		public bool usePicker = true;

        override public void ShowGUI(List<ActionParameter> parameters)
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

            // Actor, Conversant, Sync Data:
            actor = (Transform) EditorGUILayout.ObjectField(new GUIContent("Actor:", "The primary speaker, usually the player"), actor, typeof(Transform), true);
			conversant = (Transform) EditorGUILayout.ObjectField(new GUIContent("Conversant:", "The other speaker, usually an NPC"), conversant, typeof(Transform), true);
			syncData = EditorGUILayout.Toggle(new GUIContent("Sync Data:", "Synchronize AC data with Lua environment"), syncData);
            allowDuringConversations = EditorGUILayout.Toggle(new GUIContent("Allow During Conversations:", "Allow bark to play even if a conversation is active"), allowDuringConversations);

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