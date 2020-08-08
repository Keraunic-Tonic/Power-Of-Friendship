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
	/// This custom Adventure Creator sends a message to the Dialogue System sequencer.
	/// </summary>
	[System.Serializable]
	public class ActionDialogueSystemSequencerMessage : Action
	{
		
		public int constantID = 0;

        public int messageParameterID = -1;
        public bool messageFromParameter = false;

        public string message = string.Empty;

		public ActionDialogueSystemSequencerMessage ()
		{
			this.isDisplayed = true;
			category = ActionCategory.ThirdParty;
			title = "Dialogue System Sequencer Message";
			description = "Sends a message to the Dialogue System sequencer.";
		}

        override public void AssignValues(List<ActionParameter> parameters)
        {
            if (messageFromParameter) message = AssignString(parameters, messageParameterID, message);
        }

        override public float Run ()
		{
			if (DialogueDebug.LogInfo) Debug.Log("<color=cyan>Sending message to Dialogue System sequencer: " + message + "</color>");
			Sequencer.Message(message);
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
                message = EditorGUILayout.TextField(new GUIContent("Message:", "The message to send to the sequencer"), message);
            }

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