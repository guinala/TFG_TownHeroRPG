using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dungeon), true)]
public class RandomDungeonGeneratorEditor : Editor
{
    Dungeon dungeonGenerator;

    private void Awake()
    {
        dungeonGenerator = (Dungeon)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
        {
            dungeonGenerator.Generate();
        }
    }
}
