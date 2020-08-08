using UnityEngine;
using System.Collections;
using AC;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.AdventureCreatorSupport;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Implements the Adventure Creator sequencer command AC(actionList[, nowait[, stepNum]]), where:
    /// 
    /// - actionList is the name of a GameObject containing an ActionList component or an
    ///   ActionListAsset inside a Resources folder, asset bundle, or Addressable, and
    /// - if the optional second parameter is "nowait", the actionList runs in the background and
    /// control passes immediately to the next stage of the conversation.
    /// - stepNum is the step number to start at (default: 0)
    /// </summary>
    public class SequencerCommandAC : SequencerCommand
    {

        private ActionList actionList = null;
        private ActionListManager actionListManager = null;
        private AdventureCreatorBridge bridge = null;
        private bool mustDestroyAsset = false;

        public void Start()
        {
            bridge = DialogueManager.Instance.GetComponent<AdventureCreatorBridge>();
            actionListManager = KickStarter.actionListManager;
            string actionListSpecifier = GetParameter(0);
            bool wait = !string.Equals(GetParameter(1), "nowait");
            int startAt = GetParameterAsInt(2);
            bool addToSkipQueue = true;

            // Look for a GameObject in the scene matching the specified name:
            GameObject actionListObject = GameObject.Find(actionListSpecifier);
            if (actionListObject != null)
            {
                actionList = actionListObject.GetComponent<ActionList>();
                if (actionList != null)
                {
                    if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Sequencer: AC({1},{2},startAt={3}) starting action list", DialogueDebug.Prefix, actionListSpecifier, (wait ? "wait" : "nowait"), startAt), actionListObject);
                    if (bridge != null && DialogueManager.IsConversationActive) bridge.SyncLuaToAdventureCreator();
                    if (startAt == 0)
                    {
                        actionList.Interact();
                    }
                    else
                    {
                        actionList.Interact(startAt, addToSkipQueue);
                    }
                }
            }

            // Failing that, look for other GameObjects in the scene matching the name:
            if (actionList == null)
            {
                foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
                {
                    if (string.Equals(go.name, actionListSpecifier))
                    {
                        actionList = go.GetComponent<ActionList>();
                        if (actionList != null)
                        {
                            if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Sequencer: AC({1},{2},startAt={3}) starting action list", DialogueDebug.Prefix, actionListSpecifier, (wait ? "wait" : "nowait"), startAt), go);
                            if (bridge != null && DialogueManager.IsConversationActive) bridge.SyncLuaToAdventureCreator();
                            if (startAt == 0)
                            {
                                actionList.Interact();
                            }
                            else
                            {
                                actionList.Interact(startAt, addToSkipQueue);
                            }
                            break;
                        }
                    }
                }
            }

            // Failing that, try loading it as an asset:
            if (actionList == null)
            {
                DialogueManager.LoadAsset(actionListSpecifier, typeof(ActionListAsset),
                    (asset) =>
                    {
                        var actionListAsset = asset as ActionListAsset;
                        if (actionListAsset != null)
                        {
                            if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Sequencer: AC({1},{2},startAt={3}) starting action list asset", DialogueDebug.Prefix, actionListSpecifier, (wait ? "wait" : "nowait"), startAt));
                            if (bridge != null && DialogueManager.IsConversationActive) bridge.SyncLuaToAdventureCreator();
                            if (startAt == 0)
                            {
                                actionList = AdvGame.RunActionListAsset(actionListAsset);
                            }
                            else
                            {
                                actionList = AdvGame.RunActionListAsset(actionListAsset, startAt, addToSkipQueue);
                            }
                            mustDestroyAsset = true;
                        }
                        else
                        {
                            // Failing that, look for an ActionListAsset in all resources of the project:
                            foreach (ActionListAsset anAsset in Resources.FindObjectsOfTypeAll(typeof(ActionListAsset)) as ActionListAsset[])
                            {
                                if (string.Equals(actionListSpecifier, anAsset.name, System.StringComparison.OrdinalIgnoreCase))
                                {
                                    if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Sequencer: AC({1},{2},startAt={3}) starting action list asset", DialogueDebug.Prefix, actionListSpecifier, (wait ? "wait" : "nowait"), startAt));
                                    if (bridge != null && DialogueManager.IsConversationActive) bridge.SyncLuaToAdventureCreator();
                                    if (startAt == 0)
                                    {
                                        actionList = AdvGame.RunActionListAsset(anAsset);
                                    }
                                    else
                                    {
                                        actionList = AdvGame.RunActionListAsset(anAsset, startAt, addToSkipQueue);
                                    }
                                    break;
                                }
                            }
                        }
                        if (actionList == null)
                        {
                            if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: AC(): Can't find action list '{1}'", DialogueDebug.Prefix, actionListSpecifier));
                            Stop();
                        }
                        else
                        {
                            if (!wait) Stop();
                        }
                    });
            }
            else
            {
                if (!wait) Stop();
            }
        }

        public void Update()
        {
            if ((actionListManager == null) || !actionListManager.IsListRunning(actionList))
            {
                if (bridge != null)
                {
                    bridge.SyncAdventureCreatorToLua();
                    if (DialogueManager.IsConversationActive) bridge.SetConversationGameState();
                }
                Stop();
            }
        }

        private void OnDestroy()
        {
            if (mustDestroyAsset) DialogueManager.UnloadAsset(actionList);
        }

    }

}
