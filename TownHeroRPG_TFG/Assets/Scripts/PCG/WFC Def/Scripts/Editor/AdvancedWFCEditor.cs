using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveFunctionCollapseAlgorithm))]
public class AdvancedWFCEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Dibuja la interfaz de usuario del Inspector por defecto.
        DrawDefaultInspector();

        // Añade un botón "Generate".
        if (GUILayout.Button("Generate"))
        {
            // Obtiene la instancia del script AdvancedWFC.
            WaveFunctionCollapseAlgorithm advancedWFC = (WaveFunctionCollapseAlgorithm)target;

            // Llama al método que inicia la generación.
            advancedWFC.StartGeneration();
        }
    }
}
