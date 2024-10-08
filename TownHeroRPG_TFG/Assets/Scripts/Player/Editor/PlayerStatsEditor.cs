using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerStats))]
public class PlayerStatsEditor : Editor
{
    public PlayerStats characterStats => target as PlayerStats;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reset Stats"))
        {
            characterStats.ResetStats();
        }
    }
}