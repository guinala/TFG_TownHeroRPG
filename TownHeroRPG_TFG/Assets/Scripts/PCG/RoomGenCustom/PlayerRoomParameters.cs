using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerRoomParameters : ScriptableObject
{
    public GameObject PlayerPrefab;
    public List<Prop> Props;
}
