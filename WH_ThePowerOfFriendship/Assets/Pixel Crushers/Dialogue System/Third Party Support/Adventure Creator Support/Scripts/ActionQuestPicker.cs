#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    public class ActionQuestPicker
    {

        public DialogueDatabase database = null;
        public string currentQuest = string.Empty;
        public bool usePicker = false;
        public bool showReferenceDatabase = true;

        private string[] titles = null;
        private int currentIndex = -1;

        public ActionQuestPicker(DialogueDatabase database, string currentQuest, bool usePicker)
        {
            this.database = database ?? FindInitialDatabase();
            this.currentQuest = currentQuest;
            this.usePicker = usePicker;
            UpdateTitles();
            bool currentQuestIsInDatabase = (database != null) || (currentIndex >= 0);
            if (usePicker && !string.IsNullOrEmpty(currentQuest) && !currentQuestIsInDatabase)
            {
                this.usePicker = false;
            }
        }

        public void UpdateTitles()
        {
            currentIndex = -1;
            if (database == null || database.items == null)
            {
                titles = new string[0];
            }
            else
            {
                List<string> list = new List<string>();
                foreach (var item in database.items)
                {
                    if (!item.IsItem)
                    {
                        list.Add(item.Name);
                    }
                }
                titles = list.ToArray();
                for (int i = 0; i < titles.Length; i++)
                {
                    if (string.Equals(currentQuest, titles[i]))
                    {
                        currentIndex = i;
                    }
                }
            }
        }

        public void Draw()
        {
            if (showReferenceDatabase)
            {

                // Show with reference database field:
                EditorGUILayout.BeginHorizontal();
                if (usePicker)
                {
                    var newDatabase = EditorGUILayout.ObjectField("Reference Database", database, typeof(DialogueDatabase), false) as DialogueDatabase;
                    if (newDatabase != database)
                    {
                        database = newDatabase;
                        UpdateTitles();
                    }
                }
                else
                {
                    currentQuest = EditorGUILayout.TextField("Quest Name", currentQuest);
                }
                DrawToggle();
                EditorGUILayout.EndHorizontal();

                if (usePicker)
                {
                    currentIndex = EditorGUILayout.Popup("Quest Name", currentIndex, titles);
                    if (0 <= currentIndex && currentIndex < titles.Length) currentQuest = titles[currentIndex];
                    if (!showReferenceDatabase)
                    {
                        DrawToggle();
                        DrawClearButton();
                    }
                }
            }
            else
            {

                // Show without reference database field:
                EditorGUILayout.BeginHorizontal();
                if (usePicker)
                {
                    currentIndex = EditorGUILayout.Popup("Quest Name", currentIndex, titles);
                    if (0 <= currentIndex && currentIndex < titles.Length) currentQuest = titles[currentIndex];
                    DrawClearButton();
                }
                else
                {
                    currentQuest = EditorGUILayout.TextField("Quest Name", currentQuest);
                }
                DrawToggle();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawToggle()
        {
            var newToggleValue = EditorGUILayout.Toggle(usePicker, EditorStyles.radioButton, GUILayout.Width(16));
            if (newToggleValue != usePicker)
            {
                usePicker = newToggleValue;
                if (usePicker && (database == null)) database = FindInitialDatabase();
                UpdateTitles();
            }
        }

        private void DrawClearButton()
        {
            if (GUILayout.Button("x", GUILayout.Width(18), GUILayout.Height(14)))
            {
                currentQuest = string.Empty;
                currentIndex = -1;
            }
        }

        private DialogueDatabase FindInitialDatabase()
        {
            var dialogueSystemController = Object.FindObjectOfType<DialogueSystemController>();
            return (dialogueSystemController == null) ? null : dialogueSystemController.initialDatabase;
        }

    }
}
#endif