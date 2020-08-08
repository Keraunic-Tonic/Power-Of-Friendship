using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace AC
{

    /// <summary>
    /// This is an action list triggered by a change in a Dialogue System Lua variable.
    /// Contributed by Chad Kilgore.
    /// </summary>
    [System.Serializable]
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/Adventure Creator/Lua Var Change Watcher")]
    public class LuaVarChangeWatcher : ActionList
    {

        [Tooltip("Watch this Lua variable for changes.")]
        public string luaVar = string.Empty;

        public LuaWatchFrequency freqency = LuaWatchFrequency.EveryUpdate;

#if !UNITY_EDITOR
		
		void OnEnable()
		{
			DialogueManager.AddLuaObserver(luaVar, freqency, OnVarChanged);
		}
		
		void OnDisable()
		{
			DialogueManager.RemoveLuaObserver(luaVar, freqency, OnVarChanged);
		}
		
#endif

        void OnVarChanged(LuaWatchItem item, Lua.Result value)
        {
            Interact();
        }
    }
}
