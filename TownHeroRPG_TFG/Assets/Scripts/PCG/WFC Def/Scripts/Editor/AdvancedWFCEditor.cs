using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveFunctionCollapseAlgorithm))]
public class WFCAlgorithmEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            WaveFunctionCollapseAlgorithm advancedWFC = (WaveFunctionCollapseAlgorithm)target;

            advancedWFC.StartGeneration();
        }
    }
}
