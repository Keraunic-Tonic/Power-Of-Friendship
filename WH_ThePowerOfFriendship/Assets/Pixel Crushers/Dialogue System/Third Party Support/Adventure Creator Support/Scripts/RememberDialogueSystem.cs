using System.Collections;
using UnityEngine;
using AC;

namespace PixelCrushers.DialogueSystem.AdventureCreatorSupport
{

    /// <summary>
    /// This script ties into AC's save system. If the Dialogue Manager has a Save System
    /// component, it will use it. Otherwise it will only use the Dialogue System's 
    /// PersistentDataManager, which saves the dialogue database runtime values and
    /// any persistent data components but not Savers.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/Adventure Creator/Remember Dialogue System (on Dialogue Manager)")]
    public class RememberDialogueSystem : Remember
    {

        /// <summary>
        /// Tells the Dialogue System to save its state into an AC global variable
        /// prior to changing levels (or saving a game).
        /// </summary>
        public override string SaveData()
        {
            if (DialogueDebug.LogInfo) Debug.Log("Saving Dialogue System state to Adventure Creator.");
            if (FindObjectOfType<PixelCrushers.SaveSystem>() != null)
            {
                return PixelCrushers.SaveSystem.Serialize(PixelCrushers.SaveSystem.RecordSavedGameData());
            }
            else
            {
                return PersistentDataManager.GetSaveData();
            }
        }

        public override void LoadData(string stringData)
        {
            if (FindObjectOfType<PixelCrushers.SaveSystem>() != null)
            {
                if (DialogueDebug.LogInfo) Debug.Log("Restoring Dialogue System state from Adventure Creator.");
                PixelCrushers.SaveSystem.ApplySavedGameData(PixelCrushers.SaveSystem.Deserialize<PixelCrushers.SavedGameData>(stringData));
            }
            else
            {
                PersistentDataManager.ApplySaveData(stringData);
            }
            //UpdateSettingsFromAC();
        }

        public void UpdateSettingsFromAC()
        {
            if (AC.KickStarter.kickStarter == null || AC.KickStarter.kickStarter.gameObject == null) return;
            AC.KickStarter.kickStarter.StartCoroutine(DelayedUpdateSettingsFromAC());
        }

        /// <summary>
        /// Waits one frame, then updates settings from AC. We need to wait one frame because AC calls the 
        /// save/load options hooks before setting the language.
        /// </summary>
        protected IEnumerator DelayedUpdateSettingsFromAC()
        {
            yield return null;
            if (UILocalizationManager.instance != null)
            {
                UILocalizationManager.instance.currentLanguage = GetACLanguageName();
            }
        }

        protected string GetACLanguageName()
        {
            return (AC.Options.GetLanguage() > 0) ? AC.Options.GetLanguageName() : string.Empty;
        }

    }
}
