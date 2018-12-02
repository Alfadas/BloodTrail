using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {
    [SerializeField] BuildEncounter buildEncounter;
    List<GameObject> enemies;
    List<GameObject> playerGroup;

    public void StartFight()
    {
        playerGroup = new List<GameObject>(); //get Playergroup
        enemies = buildEncounter.GetEnemies();
    }
}
