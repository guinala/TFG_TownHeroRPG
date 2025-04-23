using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdvancedWFC))]
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
            AdvancedWFC advancedWFC = (AdvancedWFC)target;

            // Llama al método que inicia la generación.
            advancedWFC.StartGeneration();
        }
    }
}
