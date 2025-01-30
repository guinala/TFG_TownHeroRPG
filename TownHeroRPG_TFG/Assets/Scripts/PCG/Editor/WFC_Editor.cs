using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System;
using UnityEngine.UIElements;
using NUnit.Framework.Constraints;
using UnityEditor.SceneManagement;


[CustomEditor(typeof(MapGenerator2dTilePattern))]
public class WFC_Editor : Editor
{
    private bool isDirty = false;
    private bool showGeneratorActions = true;

    public override void OnInspectorGUI()
    {
        var mapGenerator = (MapGenerator2dTilePattern)target;

        DrawDefaultInspector();

        EditorGUILayout.Space(20);
        CreateTilePatternButtons(mapGenerator);
        EditorGUILayout.Space(20);
        CreateFooterButtons(mapGenerator);
        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField(@$"#Map: {mapGenerator.UsedSeed}");
        EditorGUILayout.LabelField(@$"Is Running for: {mapGenerator.ElapsedTime.ToString("0.0")}s");
        EditorGUILayout.LabelField(@$"Iteration/s: {mapGenerator.IterationPerSecond.ToString("0.0")} - ({mapGenerator.IterationNumber} of {mapGenerator.MaxIterationNumber})");

        if (GUI.changed)
        {
            EditorUtility.SetDirty(mapGenerator);
            EditorSceneManager.MarkSceneDirty(mapGenerator.gameObject.scene);
        }
    }

    private void CreateTilePatternButtons(MapGenerator2dTilePattern mapGenerator)
    {
        var serializedGenerator = new SerializedObject(mapGenerator);
        var patternList = serializedGenerator.FindProperty(nameof(MapGenerator2dTilePattern.TilePatternList));

        EditorGUILayout.LabelField($@"Base Tile", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();
        CreateTileInput(ref mapGenerator.BaseTile, 1);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(16);

        EditorGUILayout.LabelField(@$"Tile Patterns Config", EditorStyles.boldLabel);

        if (GUILayout.Button("Add tile pattern"))
        {
            mapGenerator.AddTilePattern();
            patternList.InsertArrayElementAtIndex(mapGenerator.TilePatternList.Count - 1);
            patternList.GetArrayElementAtIndex(mapGenerator.TilePatternList.Count - 1).serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space(8);

        for (int i = 0; i < patternList.arraySize; i++)
        {
            mapGenerator.TilePatternList[i].ShowOnInspector = EditorGUILayout.Foldout(mapGenerator.TilePatternList[i].ShowOnInspector, new GUIContent($@"PATTERN #{i}"), true);

            if (mapGenerator.TilePatternList[i].ShowOnInspector)
            {
                // EditorGUILayout.BeginHorizontal();
                // EditorGUILayout.LabelField($@"PATTERN #{i}", EditorStyles.boldLabel);
                mapGenerator.TilePatternList[i].Enabled = EditorGUILayout.Toggle("Enabled", mapGenerator.TilePatternList[i].Enabled);
                // EditorGUILayout.EndHorizontal();

                mapGenerator.TilePatternList[i].Frequency = (int)EditorGUILayout.Slider("Frequency", mapGenerator.TilePatternList[i].Frequency, 1, 100);
                var tileFreq = mapGenerator.TilePatternList[i].Frequency;

                EditorGUILayout.Space(8);

                // Base Out - Top Tiles
                EditorGUILayout.BeginHorizontal();
                CreateTileInput(ref mapGenerator.TilePatternList[i].BaseOutTopLeft, 1f + tileFreq / 100f);
                CreateTileInput(ref mapGenerator.TilePatternList[i].BaseOutTop, 1f + tileFreq / 100f);
                CreateTileInput(ref mapGenerator.TilePatternList[i].BaseOutTopRight, 1f + tileFreq / 100f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                CreateTileInput(ref mapGenerator.TilePatternList[i].BaseOutCenterLeft, 1f + tileFreq / 100f);
                CreateTileInput(ref mapGenerator.TilePatternList[i].BaseOutCenter, 1f + tileFreq / 100f);
                CreateTileInput(ref mapGenerator.TilePatternList[i].BaseOutCenterRight, 1f + tileFreq / 100f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                CreateTileInput(ref mapGenerator.TilePatternList[i].BaseOutBottomLeft, 1f + tileFreq / 100f);
                CreateTileInput(ref mapGenerator.TilePatternList[i].BaseOutBottom, 1f + tileFreq / 100f);
                CreateTileInput(ref mapGenerator.TilePatternList[i].BaseOutBottomRight, 1f + tileFreq / 100f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);

                // Enabling Inside Out tiles
                mapGenerator.TilePatternList[i].EnableInsideOut = EditorGUILayout.Toggle("Enable inside out", mapGenerator.TilePatternList[i].EnableInsideOut);

                if (mapGenerator.TilePatternList[i].EnableInsideOut)
                {
                    // Base In - Top Tiles
                    EditorGUILayout.BeginHorizontal();
                    CreateTileInput(ref mapGenerator.TilePatternList[i].BaseInTopLeft, 1f + tileFreq / 100f);
                    CreateTileInput(ref mapGenerator.TilePatternList[i].BaseInTop, 1f + tileFreq / 100f);
                    CreateTileInput(ref mapGenerator.TilePatternList[i].BaseInTopRight, 1f + tileFreq / 100f);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    CreateTileInput(ref mapGenerator.TilePatternList[i].BaseInCenterLeft, 1f + tileFreq / 100f);

                    // Draw base tile on center
                    EditorGUILayout.Space(8);
                    Texture2D sprite = AssetPreview.GetAssetPreview(mapGenerator.BaseTile.GetSprite());
                    GUILayout.Label(string.Empty, GUILayout.Height(48), GUILayout.Width(48));
                    GUI.DrawTexture(GUILayoutUtility.GetLastRect(), sprite);
                    EditorGUILayout.Space(32);

                    CreateTileInput(ref mapGenerator.TilePatternList[i].BaseInCenterRight, 1f + tileFreq / 100f);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    CreateTileInput(ref mapGenerator.TilePatternList[i].BaseInBottomLeft, 1f + tileFreq / 100f);
                    CreateTileInput(ref mapGenerator.TilePatternList[i].BaseInBottom, 1f + tileFreq / 100f);
                    CreateTileInput(ref mapGenerator.TilePatternList[i].BaseInBottomRight, 1f + tileFreq / 100f);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space(16);

                if (GUILayout.Button($@"Remove pattern #{i}"))
                {
                    mapGenerator.RemoveTilePattern(i);
                    patternList.DeleteArrayElementAtIndex(i);
                    EditorUtility.SetDirty(mapGenerator);
                }

                EditorGUILayout.Space(16);

                patternList.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            isDirty = true;
        }
    }

    private void CreateTileInput(ref Tile tile, float tileFrequency)
    {
        EditorGUILayout.BeginVertical();
        tile = (Tile)EditorGUILayout.ObjectField(tile, typeof(Tile), false);

        EditorGUILayout.Space(4);
        if (tile)
        {
            tile.Weight = (int)Math.Round(tile.ReferenceWeight * tileFrequency);

            Texture2D sprite = AssetPreview.GetAssetPreview(tile.GetSprite());
            GUILayout.Label(string.Empty, GUILayout.Height(48), GUILayout.Width(48));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), sprite);
        }
        EditorGUILayout.EndVertical();
    }

    private void CreateFooterButtons(MapGenerator2dTilePattern mapGenerator)
    {
        showGeneratorActions = EditorGUILayout.Foldout(showGeneratorActions, new GUIContent($@"Actions"), true);

        if (showGeneratorActions)
        {
            if (GUILayout.Button("Generate map"))
            {
                mapGenerator.GenerateMap();
            }

            if (mapGenerator.Generate && GUILayout.Button("Stop generation"))
            {
                mapGenerator.Generate = false;
            }

            if (!mapGenerator.Generate && GUILayout.Button("Clear grid"))
            {
                mapGenerator.ClearGridObjects();
            }

            EditorGUILayout.Space(16);

            if (!mapGenerator.Generate && GUILayout.Button("Save Changes"))
            {
                SaveChanges();
            }
        }
    }

    public override void SaveChanges()
    {
        isDirty = false;
        var serializedGenerator = new SerializedObject(target);
        serializedGenerator.ApplyModifiedProperties();
        Debug.Log($"{this} saved successfully!!!");
        base.SaveChanges();
    }
}
