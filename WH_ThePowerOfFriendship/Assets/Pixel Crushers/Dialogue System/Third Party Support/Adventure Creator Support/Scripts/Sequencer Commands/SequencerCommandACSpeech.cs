using UnityEngine;
using AC;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Sequencer command ACSpeech(lineID, [nowait])
    /// 
    /// Plays a line using AC's speech features.
    /// 
    /// - lineID: The character name and line number (e.g., Player42).
    /// - `nowait`: (optional) If `nowait` is specified, doesn't wait for the clip to finish.
    /// </summary>
    public class SequencerCommandACSpeech : SequencerCommand
    {

        private AC.Char speakerChar;

        public void Start()
        {

            // Get and validate the arguments:
            if (!DialogueManager.IsConversationActive)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Sequencer command ACSpeech({1}): No conversation is active; can't run", DialogueDebug.Prefix, GetParameters()));
                return;
            }
            var subtitle = DialogueManager.CurrentConversationState.subtitle;
            if (subtitle == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Sequencer command ACSpeech({1}): The subtitle record is invalid", DialogueDebug.Prefix, GetParameters()));
                return;
            }
            if (string.IsNullOrEmpty(subtitle.dialogueEntry.DialogueText) && string.Equals(subtitle.dialogueEntry.Title, "START"))
            {
                return;
            }
            var subject = (subtitle.speakerInfo == null) ? null : subtitle.speakerInfo.transform;
            speakerChar = (subject == null) ? null : subject.GetComponent<AC.Char>();
            if (speakerChar == null)
            {
                foreach (var character in FindObjectsOfType<AC.Char>())
                {
                    if (string.Equals(character.name, subtitle.speakerInfo.Name))
                    {
                        speakerChar = character;
                        break;
                    }
                }
            }
            if (speakerChar == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Sequencer command ACSpeech({1}): Speaker character not found", DialogueDebug.Prefix, GetParameters()));
                return;
            }
            var speakerName = speakerChar.name;
            var isPlayer = (speakerChar.GetComponent<Player>() != null);
            if (isPlayer)
            {
                speakerChar = KickStarter.player;
                if ((KickStarter.settingsManager.playerSwitching == PlayerSwitching.Allow || !KickStarter.speechManager.usePlayerRealName))
                {
                    speakerName = "Player";
                }
            }
            var text = subtitle.formattedText.text;
            var lineID = GetParameter(0);
            var numberString = lineID.Substring(Mathf.Min(lineID.Length, speakerName.Length));
            var lineNumber = Tools.StringToInt(numberString);
            //--- No longer used: var language = Options.GetLanguageName();
            var isBackground = string.Equals(GetParameter(1), "nowait", System.StringComparison.OrdinalIgnoreCase);
            var noAnimation = false;
            Debug.Log(string.Format("{0}: Sequencer command ACSpeech({1}): speaker={2}, lineNumber={3}, text='{4}', isBackground={5}, isPlayer={6}",
                                    DialogueDebug.Prefix, GetParameters(), speakerName, lineNumber, text, isBackground, isPlayer));

            // Hook into AC's EventManager to be notified when speech stops:
            AC.EventManager.OnStopSpeech += OnStopSpeech;

            // Call AC's speech functionality. Credit: Based on AC's ActionSpeech.cs (c) Icebox Studios.
            KickStarter.dialog.KillDialog(false, true);
            KickStarter.dialog.StartDialog(speakerChar, text, isBackground, lineNumber, noAnimation);
        }

        private void OnStopSpeech(AC.Char speakingCharacter)
        {
            Stop();
        }

        private void OnDestroy()
        {
            // Unhook from AC's EventManager:
            AC.EventManager.OnStopSpeech -= OnStopSpeech;

            if (speakerChar != null && speakerChar.isTalking) speakerChar.StopSpeaking();
        }
    }

}
