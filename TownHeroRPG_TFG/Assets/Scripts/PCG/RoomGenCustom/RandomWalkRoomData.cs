using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalkRoomData : MonoBehaviour
{
    public RandomWalkDungeonRoom Room{ get; set; }
    public GameObject PlayerReference { get; set; }

    public void ClearData()
    {
        Room.PropObjects.ForEach(go => DestroyImmediate(go));
        Room.EnemiesInRoom.ForEach(go => DestroyImmediate(go));
        Room.SpecialItemsInRoom.ForEach(go => DestroyImmediate(go));
        
        DestroyImmediate(PlayerReference);
    }
}
 