
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(EnvQuerySystem))]
public class QuerySystemEditor : Editor
{
    private SerializedProperty strategiesProp;
    private string[] strategyTypeNames;
    private Type[] strategyTypes;
    private int selectedTypeIndex = 0;

    private void OnEnable()
    {
        strategiesProp = serializedObject.FindProperty("_envQueryStrategies");

        // StrategyBaseを継承する型を取得
        strategyTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(asm => asm.GetTypes())
            .Where(t => typeof(EnvQueryStrategy).IsAssignableFrom(t) && !t.IsAbstract)
            .ToArray();

        strategyTypeNames = strategyTypes.Select(t => t.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();

        //EditorGUILayout.PropertyField(strategiesProp, true);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Add New Strategy", EditorStyles.boldLabel);

        selectedTypeIndex = EditorGUILayout.Popup("Strategy Type", selectedTypeIndex, strategyTypeNames);

        if (GUILayout.Button("Add Strategy"))
        {
            object instance = Activator.CreateInstance(strategyTypes[selectedTypeIndex]);
            strategiesProp.arraySize++;
            strategiesProp.GetArrayElementAtIndex(strategiesProp.arraySize - 1).managedReferenceValue = instance;
        }

        serializedObject.ApplyModifiedProperties();
    }
}


