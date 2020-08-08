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
    /// This custom Adventure Creator action sets a quest or quest entry state
    /// </summary>
    [System.Serializable]
    public class ActionDialogueSystemQuestSet : Action
    {

        public int constantID = 0;

        public enum Mode { MainState, EntryState }

        [QuestPopup]
        public string questName = string.Empty;

        public Mode mode = Mode.MainState;

        public int entryNumber = 0;

        public QuestState questState;

        public ActionDialogueSystemQuestSet()
        {
            this.isDisplayed = true;
            category = ActionCategory.ThirdParty;
            title = "Dialogue System Quest Set";
            description = "Sets a Dialogue System quest state or quest entry state.";
        }

        override public float Run()
        {
            if (mode == Mode.MainState)
            {
                QuestLog.SetQuestState(questName, questState);
            }
            else
            {
                QuestLog.SetQuestEntryState(questName, entryNumber, questState);
            }
            return 0;
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
            mode = (Mode)EditorGUILayout.EnumPopup(new GUIContent("Set:", "Set main state or a quest entry state"), mode);
            if (mode == Mode.EntryState)
            {
                entryNumber = EditorGUILayout.IntField(new GUIContent("Entry #:", "Entry number to set"), entryNumber);
            }
            questState = (QuestState)EditorGUILayout.EnumPopup(new GUIContent("State:", "New state"), questState);
            AfterRunningOption();
        }

        public override string SetLabel()
        {
            string labelAdd = "";
            if (!string.IsNullOrEmpty(questName))
            {
                if (mode == Mode.MainState)
                {
                    labelAdd = " (" + questName + " " + questState + ")";
                }
                else
                {
                    labelAdd = " (" + questName + ":" + entryNumber + " " + questState + ")";
                }
            }
            return labelAdd;
        }

#endif

    }

}
