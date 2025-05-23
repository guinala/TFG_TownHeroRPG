using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RandomWalkDungeonParameters : ScriptableObject
{
    public GameObject PlayerPrefab;
    public GameObject[] EnemyPrefabs;
    public int EnemyCount;
    public GameObject ChestPrefab;
    public GameObject BossPrefab;
    public List<Prop> Props;

}
