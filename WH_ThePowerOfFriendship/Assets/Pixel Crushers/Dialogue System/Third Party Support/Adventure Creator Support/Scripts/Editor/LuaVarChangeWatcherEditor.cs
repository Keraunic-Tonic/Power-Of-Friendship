using UnityEngine;
using UnityEditor;
using PixelCrushers.DialogueSystem;

namespace AC
{

    [CustomEditor(typeof(LuaVarChangeWatcher))]
    [System.Serializable]
    public class LuaVarChangeWatcherEditor : CutsceneEditor
    {

        //---Unused: private string[] Options = { "On enter", "Continuous", "On exit" };


        public override void OnInspectorGUI()
        {
            LuaVarChangeWatcher _target = (LuaVarChangeWatcher)target;

            EditorGUILayout.BeginVertical("Button");
            EditorGUILayout.LabelField("Watcher properties", EditorStyles.boldLabel);

            _target.source = (ActionListSource)EditorGUILayout.EnumPopup("Actions source:", _target.source);
            if (_target.source == ActionListSource.AssetFile)
            {
                _target.assetFile = (ActionListAsset)EditorGUILayout.ObjectField("ActionList asset:", _target.assetFile, typeof(ActionListAsset), false);
            }

            _target.actionListType = (ActionListType)EditorGUILayout.EnumPopup("When running:", _target.actionListType);
            if (_target.actionListType == ActionListType.PauseGameplay)
            {
                _target.isSkippable = EditorGUILayout.Toggle("Is skippable?", _target.isSkippable);
            }

            EditorGUILayout.Space();

            _target.luaVar = EditorGUILayout.TextField("Lua Code:", _target.luaVar);
            _target.freqency = (LuaWatchFrequency)EditorGUILayout.EnumPopup("Frequency:", _target.freqency);

            EditorGUILayout.EndVertical();

            DrawSharedElements(_target as ActionList);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }

    }

}