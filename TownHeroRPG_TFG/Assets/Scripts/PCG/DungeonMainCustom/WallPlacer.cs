using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public static class WallPlacer
{
    private static readonly Dictionary<string, HashSet<int>> WallTypePatterns = new Dictionary<string, HashSet<int>>
    {
        // Basic walls (4-bit patterns, cardinal directions)
        ["Top"] = new HashSet<int> { 0b1111, 0b0110, 0b0011, 0b0010, 0b1010, 0b1100, 0b1110, 0b1011, 0b0111 },
        ["Bottom"] = new HashSet<int> { 0b1000 },
        ["Left"] = new HashSet<int> { 0b0100 },
        ["Right"] = new HashSet<int> { 0b0001 },

        // Corner walls (8-bit patterns, all directions)
        ["InnerCornerDownLeft"] = new HashSet<int>
        {
            0b11110001, 0b11100000, 0b11110000, 0b11100001, 0b10100000, 0b01010001, 0b11010001,
            0b01100001, 0b11010000, 0b01110001, 0b00010001, 0b10110001, 0b10100001, 0b10010000,
            0b00110001, 0b10110000, 0b00100001, 0b10010001
        },
        ["InnerCornerDownRight"] = new HashSet<int>
        {
            0b11000111, 0b11000011, 0b10000011, 0b10000111, 0b10000010, 0b01000101, 0b11000101,
            0b01000011, 0b10000101, 0b01000111, 0b01000100, 0b11000110, 0b11000010, 0b10000100,
            0b01000110, 0b10000110, 0b11000100, 0b01000010
        },
        ["DiagonalCornerDownLeft"] = new HashSet<int> { 0b01000000 },
        ["DiagonalCornerDownRight"] = new HashSet<int> { 0b00000001 },
        ["DiagonalCornerUpLeft"] = new HashSet<int> { 0b00010000, 0b01010000 },
        ["DiagonalCornerUpRight"] = new HashSet<int> { 0b00000100, 0b00000101 },
        ["Full"] = new HashSet<int> { 0b1101, 0b0101, 0b1001 }, // Removed duplicates
        ["FullEightDirections"] = new HashSet<int>
        {
            0b00010100, 0b11100100, 0b10010011, 0b01110100, 0b00010111, 0b00010110, 0b00110100,
            0b00010101, 0b01010100, 0b00010010, 0b00100100, 0b00010011, 0b01100100, 0b10010111,
            0b11110100, 0b10010110, 0b10110100, 0b11100101, 0b11010011, 0b11110101, 0b11010111,
            0b01110101, 0b01010111, 0b01100101, 0b01010011, 0b01010010, 0b00100101, 0b00110101,
            0b01010110, 0b11010101, 0b11010100, 0b10010101
        },
        ["BottomEightDirections"] = new HashSet<int> { 0b01000001 }
    };

    public static void GenerateWalls(HashSet<Vector2Int> floorPositions, TilemapPainter visualizer)
    {
        if (floorPositions == null || floorPositions.Count == 0)
        {
            Debug.LogWarning("No floor positions provided for wall generation.");
            return;
        }
        if (visualizer == null)
        {
            Debug.LogError("TilemapVisualizer is not assigned.");
            return;
        }

        HashSet<Vector2Int> basicWallPositions = FindWalls(floorPositions, Directions.cardinalDirections);
        HashSet<Vector2Int> cornerWallPositions = FindWalls(floorPositions, Directions.diagonalDirections);

        PlaceWalls(visualizer, basicWallPositions, floorPositions, Directions.cardinalDirections, isBasicWall: true);
        PlaceWalls(visualizer, cornerWallPositions, floorPositions, Directions.allDirections, isBasicWall: false);
    }

    private static HashSet<Vector2Int> FindWalls(HashSet<Vector2Int> floorPositions, List<Vector2Int> directions)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directions)
            {
                Vector2Int neighbor = position + direction;
                if (!floorPositions.Contains(neighbor))
                {
                    wallPositions.Add(neighbor);
                }
            }
        }
        return wallPositions;
    }

    private static void PlaceWalls(TilemapPainter painter, HashSet<Vector2Int> wallPositions,
        HashSet<Vector2Int> floorPositions, List<Vector2Int> directions, bool isBasicWall)
    {
        int failedCount = 0;
        foreach (var position in wallPositions)
        {
            string neighborPattern = GenerateNeighborPattern(position, floorPositions, directions);
            int patternValue = Convert.ToInt32(neighborPattern, 2);
            string wallType = DetermineWallType(patternValue, isBasicWall); 

            bool success;
            
            if (isBasicWall)
            {
                success = painter.PlaceBasicWall(position, wallType);
            }
            else
            {
                success = painter.PlaceCornerWall(position, wallType);
            }

            if (!success)
            {
                failedCount++;
            }
        }
        if (failedCount > 0)
        {
            Debug.LogWarning($"Failed to place {failedCount} walls.");
        }
    }

    private static string GenerateNeighborPattern(Vector2Int position, HashSet<Vector2Int> floorPositions, List<Vector2Int> directions)
    {
        string pattern = "";
        foreach (var direction in directions)
        {
            Vector2Int neighbor = position + direction;
            pattern += floorPositions.Contains(neighbor) ? "1" : "0";
        }
        return pattern;
    }

    private static string DetermineWallType(int patternValue, bool isBasicWall)
    {
        string binaryPattern = Convert.ToString(patternValue, 2).PadLeft(4, '0');

        if (isBasicWall)
        {
            if (WallTypePatterns["Top"].Contains(patternValue)) return "Top";
            if (WallTypePatterns["Bottom"].Contains(patternValue)) return "Bottom";
            if (WallTypePatterns["Left"].Contains(patternValue)) return "Left";
            if (WallTypePatterns["Right"].Contains(patternValue)) return "Right";
            if (WallTypePatterns["Full"].Contains(patternValue)) return "Full";
        }
        else
        {
            if (WallTypePatterns["InnerCornerDownLeft"].Contains(patternValue)) return "InnerCornerDownLeft";
            if (WallTypePatterns["InnerCornerDownRight"].Contains(patternValue)) return "InnerCornerDownRight";
            if (WallTypePatterns["DiagonalCornerDownLeft"].Contains(patternValue)) return "DiagonalCornerDownLeft";
            if (WallTypePatterns["DiagonalCornerDownRight"].Contains(patternValue)) return "DiagonalCornerDownRight";
            if (WallTypePatterns["DiagonalCornerUpLeft"].Contains(patternValue)) return "DiagonalCornerUpLeft";
            if (WallTypePatterns["DiagonalCornerUpRight"].Contains(patternValue)) return "DiagonalCornerUpRight";
            if (WallTypePatterns["FullEightDirections"].Contains(patternValue)) return "FullEightDirections";
            if (WallTypePatterns["BottomEightDirections"].Contains(patternValue)) return "BottomEightDirections";
        }


        return "Default"; // Fallback if no pattern matches
    }
}