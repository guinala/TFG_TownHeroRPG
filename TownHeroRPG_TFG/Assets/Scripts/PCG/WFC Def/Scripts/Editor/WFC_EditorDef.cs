using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(WaveFunctionCollapseAlgorithmNope))]
public class WFC_EdiDeftom : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        WaveFunctionCollapseAlgorithmNope wfcGenerator = (WaveFunctionCollapseAlgorithmNope)target;

        if (GUILayout.Button("Generar Mapa"))
        {
            wfcGenerator.Generate();
        }

        if (wfcGenerator.AvailableTiles == null || wfcGenerator.AvailableTiles.Count == 0)
        {
            EditorGUILayout.HelpBox("No hay tiles asignados", MessageType.Error);
        }
    }
}