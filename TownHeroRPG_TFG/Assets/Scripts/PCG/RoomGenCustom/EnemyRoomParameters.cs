using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyRoomParameters : ScriptableObject
{
    public GameObject[] EnemyPrefabs;
    public List<Prop> Props;
    public int EnemyCountMin;
    public int EnemyCountMax;
}

