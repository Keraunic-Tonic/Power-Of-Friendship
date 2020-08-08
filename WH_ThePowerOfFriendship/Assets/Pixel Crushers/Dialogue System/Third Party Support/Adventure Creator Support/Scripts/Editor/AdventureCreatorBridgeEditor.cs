using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PixelCrushers.DialogueSystem.AdventureCreatorSupport
{
    [CustomEditor(typeof(AdventureCreatorBridge), true)]
    public class AdventureCreatorBridgeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!Application.isPlaying)
            {
                if (GUILayout.Button(new GUIContent("Copy DS Vars To AC", "Copy all variables defined in dialogue database to AC's variables list.")))
                {
                    CopyDSVarsToAC();
                }
            }
        }

        protected void CopyDSVarsToAC()
        {
            var bridge = target as AdventureCreatorBridge;
            var database = EditorTools.FindInitialDatabase();
            if (database == null)
            {
                EditorUtility.DisplayDialog("Copy DS Vars To AC", "Can't find the dialogue database. Does your scene have a Dialogue Manager, and have you assigned a database to it?", "Close");
                return;
            }
            var choice = EditorUtility.DisplayDialogComplex("Copy DS Vars to AC", "If variables already exist in AC, overwrite them?", "Cancel", "Keep", "Overwrite");
            if (choice == 0) return;
            var overwrite = (choice == 2);
            Undo.RecordObject(AC.KickStarter.variablesManager, "Add DS variables");
            var vars = AC.KickStarter.variablesManager.vars;
            var s = "Copied variables from dialogue database '" + database.name + "' to Adventure Creator global variable list:\n";
            foreach (var variable in database.variables)
            {
                var acVar = vars.Find(x => string.Equals(x.label, variable.Name));
                if (overwrite || acVar == null)
                {
                    if (acVar == null)
                    {
                        vars.Add(new AC.GVar(GetIDArray(vars)));
                        acVar = vars[vars.Count - 1];
                        acVar.label = variable.Name;
                    }
                    switch (variable.Type)
                    {
                        case FieldType.Boolean:
                            acVar.type = AC.VariableType.Boolean;
                            acVar.BooleanValue = variable.InitialBoolValue;
                            break;
                        case FieldType.Actor:
                        case FieldType.Item:
                        case FieldType.Location:
                            acVar.type = AC.VariableType.Integer;
                            acVar.IntegerValue = (int)variable.InitialFloatValue;
                            break;
                        case FieldType.Number:
                            acVar.type = AC.VariableType.Float;
                            acVar.FloatValue = variable.InitialFloatValue;
                            break;
                        default:
                            acVar.type = AC.VariableType.String;
                            acVar.TextValue = variable.InitialValue;
                            break;
                    }
                    s += variable.Name + "\n";
                }
            }
            Debug.Log(s);
        }

        private static int[] GetIDArray(List<AC.GVar> _vars) // Copied from AC VariablesManager.cs.
        {
            // Returns a list of id's in the list

            List<int> idArray = new List<int>();

            foreach (AC.GVar variable in _vars)
            {
                idArray.Add(variable.id);
            }

            idArray.Sort();
            return idArray.ToArray();
        }

    }
}
