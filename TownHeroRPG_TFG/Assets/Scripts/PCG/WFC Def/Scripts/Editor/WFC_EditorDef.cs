using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(WaveFunctionCollapseAlgorithm))]
public class WFC_EdiDeftom : Editor
{
    private bool isDirty = false;
    private bool showGeneratorActions = true;

    public override void OnInspectorGUI()
    {
        var mapGenerator = (WaveFunctionCollapseAlgorithm)target;

        DrawDefaultInspector();

        EditorGUILayout.Space(20);
        CreateFooterButtons(mapGenerator);
        EditorGUILayout.Space(20);

        // Mostrar estadísticas de generación
        EditorGUILayout.LabelField(@$"#Map: {mapGenerator.UsedSeed}");
        EditorGUILayout.LabelField(@$"Tiempo: {mapGenerator.ElapsedTime.ToString("0.0")}s");
        EditorGUILayout.LabelField(@$"Iteraciones: {mapGenerator.IterationNumber} de {mapGenerator.MaxIterationNumber}");

        if (GUI.changed)
        {
            EditorUtility.SetDirty(mapGenerator);
            EditorSceneManager.MarkSceneDirty(mapGenerator.gameObject.scene);
        }
    }

    // Eliminada toda la lógica relacionada con TilePattern (CreateTilePatternButtons, etc.)

    private void CreateFooterButtons(WaveFunctionCollapseAlgorithm mapGenerator)
    {
        showGeneratorActions = EditorGUILayout.Foldout(showGeneratorActions, "Acciones", true);

        if (showGeneratorActions)
        {
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Generar Mapa"))
            {
                mapGenerator.GenerateMap();
            }

            if (GUILayout.Button("Limpiar Mapa"))
            {
                mapGenerator.ClearGridObjects();
            }

            EditorGUILayout.EndHorizontal();

            // Mostrar advertencia si no hay tiles configurados
            if (mapGenerator.AvailableTiles == null || mapGenerator.AvailableTiles.Count == 0)
            {
                EditorGUILayout.HelpBox("¡No hay tiles asignados!", MessageType.Error);
            }
        }
    }

    public override void SaveChanges()
    {
        isDirty = false;
        var serializedGenerator = new SerializedObject(target);
        serializedGenerator.ApplyModifiedProperties();
        base.SaveChanges();
    }
}