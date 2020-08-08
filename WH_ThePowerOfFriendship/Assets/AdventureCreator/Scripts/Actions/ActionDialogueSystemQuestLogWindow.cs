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
    /// This custom Adventure Creator action shows or hides the quest log window.
    /// </summary>
    [System.Serializable]
    public class ActionDialogueSystemQuestLogWindow : Action
    {

        public int constantID = 0;
        public bool show = true;

        public ActionDialogueSystemQuestLogWindow()
        {
            this.isDisplayed = true;
            category = ActionCategory.ThirdParty;
            title = "Dialogue System Quest Log Window";
            description = "Shows or hides the quest log window.";
        }

        override public float Run()
        {
            var questLogWindow = FindObjectOfType<QuestLogWindow>();
            if (questLogWindow != null)
            {
                if (show) questLogWindow.Open();
                else questLogWindow.Close();
            }
            return 0;
        }

#if UNITY_EDITOR

        public override void ShowGUI()
        {
            show = EditorGUILayout.Toggle(new GUIContent("Show?", "Show or hide the quest log window"), show);
            AfterRunningOption();
        }

        public override string SetLabel()
        {
            return show ? " (show)" : " (hide)";
        }

#endif
    }

}