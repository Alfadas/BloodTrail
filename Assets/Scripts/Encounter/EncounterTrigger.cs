using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterTrigger : MonoBehaviour {
    GameObject encounter;
    GameObject world;
    RollEncounter rollEncounter;
    MapManager mapManager;
    Map map;
    private void OnMouseDown()
    {
        encounter = GameObject.FindGameObjectWithTag("EncounterController");
        world = GameObject.FindGameObjectWithTag("World");
        rollEncounter = encounter.GetComponent<RollEncounter>();
        mapManager = world.GetComponent<MapManager>();
        map = mapManager.getMap();
        rollEncounter.RollNewEncounter(map.getTile(new Vector2Int(Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.z))));
    }
}
