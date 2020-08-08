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
	/// This custom Adventure Creator action shows a gameplay alert
	/// through the Dialogue System's dialogue UI.
	/// </summary>
	[System.Serializable]
	public class ActionDialogueSystemAlert : Action
	{
		
		public int constantID = 0;

        public int messageParameterID = -1;
        public int durationParameterID = -1;
        public bool messageFromParameter = false;
        public bool durationFromParameter = false;

        public string message = string.Empty;
		public float duration = 0;
		public bool syncData = false;
		
		public ActionDialogueSystemAlert ()
		{
			this.isDisplayed = true;
			category = ActionCategory.ThirdParty;
			title = "Dialogue System Alert";
			description = "Shows an alert message through the Dialogue System.";
		}

        override public void AssignValues(List<ActionParameter> parameters)
        {
            if (messageFromParameter) message = AssignString(parameters, messageParameterID, message);
            if (durationFromParameter) duration = AssignFloat(parameters, durationParameterID, duration);
        }
        
        override public float Run ()
		{
            if (syncData)
            {
                var bridge = DialogueManager.Instance.GetComponent<AdventureCreatorBridge>();
                if (bridge != null) bridge.SyncAdventureCreatorToLua();
            }
            if (Mathf.Approximately(0, duration))
            {
                DialogueManager.ShowAlert(message);
            }
            else
            {
                DialogueManager.ShowAlert(message, duration);
            }
			return 0;
		}

        #if UNITY_EDITOR

        override public void ShowGUI(List<ActionParameter> parameters)
        {
            // Message:
            messageFromParameter = EditorGUILayout.Toggle(new GUIContent("Message is parameter?", "Tick to use a parameter value for the message."), messageFromParameter);
            if (messageFromParameter)
            {
                messageParameterID = Action.ChooseParameterGUI("Message:", parameters, messageParameterID, ParameterType.String);
            }
            else
            {
                message = EditorGUILayout.TextField(new GUIContent("Message:", "The message to show. Can contain markup tags."), message);
            }

            // Duration:
            durationFromParameter = EditorGUILayout.Toggle(new GUIContent("Duration is parameter?", "Tick to use a parameter value for the duration."), durationFromParameter);
            if (durationFromParameter)
            {
                durationParameterID = Action.ChooseParameterGUI("Duration:", parameters, durationParameterID, ParameterType.Float);
            }
            else
            {
                duration = EditorGUILayout.FloatField(new GUIContent("Duration:", "The duration in seconds to show the message, or zero to base duration on message length."), duration);
            }

            // Sync Data:
			syncData = EditorGUILayout.Toggle(new GUIContent("Sync Data:", "Synchronize AC data with Lua environment"), syncData);

			AfterRunningOption ();
		}
		
		public override string SetLabel ()
		{
			// Return a string used to describe the specific action's job.
			string labelAdd = "";
            if (messageFromParameter)
            {
                labelAdd = " (parameter)";
            }
			else if (!string.IsNullOrEmpty(message))
			{
				labelAdd = " (" + message + ")";
			}
			return labelAdd;
		}
		
		#endif
		
	}

}
