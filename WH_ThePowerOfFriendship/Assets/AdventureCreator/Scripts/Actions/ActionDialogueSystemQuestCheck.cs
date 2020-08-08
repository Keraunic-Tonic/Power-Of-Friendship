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
    /// This custom action checks the state of a quest in the Dialogue System.
    /// </summary>
    [System.Serializable]
    public class ActionDialogueSystemQuestCheck : ActionCheck
    {
        public int constantID = 0;

        public enum Mode { MainState, EntryState }

        public string questName = string.Empty;

        public Mode mode = Mode.MainState;

        public int entryNumber = 0;

        public QuestState questState;

        public ActionDialogueSystemQuestCheck()
        {
            this.isDisplayed = true;
            category = ActionCategory.ThirdParty;
            title = "Dialogue System Quest Check";
            description = "Checks a Dialogue System quest state.";
        }

        override public ActionEnd End(List<AC.Action> actions)
        {
            var currentState = (mode == Mode.MainState) ? QuestLog.GetQuestState(questName)
                : QuestLog.GetQuestEntryState(questName, entryNumber);
            var result = (currentState == questState);
            return ProcessResult(result, actions);
        }

#if UNITY_EDITOR

        private ActionQuestPicker questPicker;
        public DialogueDatabase selectedDatabase = null;
        public bool usePicker = true;

        override public void ShowGUI()
        {
            if (questPicker == null)
            {
                questPicker = new ActionQuestPicker(selectedDatabase, questName, usePicker);
            }
            questPicker.Draw();
            questName = questPicker.currentQuest;
            mode = (Mode)EditorGUILayout.EnumPopup(new GUIContent("Check:", "Check main state or a quest entry state"), mode);
            if (mode == Mode.EntryState)
            {
                entryNumber = EditorGUILayout.IntField(new GUIContent("Entry #:", "Entry number to check"), entryNumber);
            }
            questState = (QuestState)EditorGUILayout.EnumPopup(new GUIContent("State:", "Required quest state"), questState);
        }

        public override string SetLabel()
        {
            string labelAdd = "";
            if (!string.IsNullOrEmpty(questName))
            {
                if (mode == Mode.MainState)
                {
                    labelAdd = " (" + questName + ")";
                }
                else
                {
                    labelAdd = " (" + questName + ":" + entryNumber + ")";
                }
            }
            return labelAdd;
        }

#endif
    }
}