using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BossRoomParameters : ScriptableObject
{
    public GameObject BossPrefab;
    public List<Prop> Props;
}
