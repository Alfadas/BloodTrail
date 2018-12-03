using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildEncounter : MonoBehaviour
{
    [SerializeField] CaracterArrayHolder caracterArrayHolder;
    [SerializeField] MapManager mapManager;
    [Header("TileProperties")]
    [SerializeField] int zLength = 25;
    [SerializeField] float sideSwitchDivMin = 1.9f;
    [SerializeField] float sideSwitchDivMax = 2.1f;
    [SerializeField] int npcXSpacing = 2;
    [Header("Pos")]
    [SerializeField] Transform edgePosL;
    [SerializeField] Transform edgePosR;
    [Header("Grounds")]
    [SerializeField] GameObject groundFarm;
    [SerializeField] GameObject groundForest;
    [SerializeField] GameObject groundGrass;
    [SerializeField] GameObject groundTown;
    [Header("Edges")]
    [SerializeField] GameObject[] edgeForest;
    [SerializeField] GameObject[] edgeGrass;
    [SerializeField] GameObject[] edgeFarm;
    [SerializeField] GameObject[] edgeTown;
    [Header("EdgeMaxObjects")]
    [SerializeField] int grassMaxEdgeObjects;
    [SerializeField] int forestEdgeMaxEdgeObjects;
    [SerializeField] int forestMaxEdgeObjects;
    [SerializeField] int townMaxEdgeObjects;
    [SerializeField] int townWallMaxEdgeObjects;
    [SerializeField] int farmMaxEdgeObjects;
    [SerializeField] int streetMaxEdgeObjects;
    [Header("EdgeMinObjects")]
    [SerializeField] int grassMinEdgeObjects;
    [SerializeField] int forestEdgeMinEdgeObjects;
    [SerializeField] int forestMinEdgeObjects;
    [SerializeField] int townMinEdgeObjects;
    [SerializeField] int townWallMinEdgeObjects;
    [SerializeField] int farmMinEdgeObjects;
    [SerializeField] int streetMinEdgeObjects;
    [Header("EdgeWidth")]
    [SerializeField] int grassEdgeWidth;
    [SerializeField] int streetEdgeWidth;
    [SerializeField] int forestEdgeWidth;
    [Header("Midds")]
    [SerializeField] GameObject[] middStreet;
    [SerializeField] GameObject[] middTown;
    [SerializeField] GameObject[] middTownWall;
    [Header("Backs")]
    [SerializeField] string[] objName;
    [SerializeField] GameObject[] backObj;
    [Header("Enemys")]
    [SerializeField] int enemyCount;
    [SerializeField] int heavyEnemyCount;

    Dictionary<string, GameObject> backObjs;

    GameObject ground;
    public List<GameObject> midd;
    public List<GameObject> edge;
    public List<GameObject> back;
    List<Character> enemies;
    List<Character> playerGroup;
    Map map;


    int edgeWidth = 5;
    bool isStreet;
    public string[] encounterOb;

    // Use this for initialization
    void Start()
    {
        objName = new string[] { "Solider1", "Solider2", "Solider3", "Knight", "Thief1", "Thief2", "Thief3", "Thief4", "Bandit1", "Bandit2", "Bandit3", "Bandit4", "Bandit5", "Barricade" };
        backObjs = new Dictionary<string, GameObject>();
        for (int i = 0; i < objName.Length; i++)
        {
            backObjs.Add(objName[i], backObj[i]);
        }
    }

    public void CategorizeTile(MapTile mapTile, string[] encounterObj, int encounterEnemyCount)
    {
        map = mapManager.getMap();
        ResetEncounter();
        ground = null;
        edge = new List<GameObject>();
        midd = new List<GameObject>();
        back = new List<GameObject>();
        enemies = new List<Character>();
        playerGroup = new List<Character>();
        int edgeObjectCount = 0;
        isStreet = false;
        encounterOb = encounterObj;


        switch (mapTile.getBiom())
        {
            case BIOM.Grassland:
                ground = groundGrass;
                edge.AddRange(edgeGrass);
                if (mapTile.getSubBiom() == SUBBIOM.Steppe || mapTile.getSubBiom() == SUBBIOM.Greenfield)
                {
                    edgeObjectCount = Random.Range(grassMinEdgeObjects, grassMaxEdgeObjects + 1);
                    edgeWidth = grassEdgeWidth;
                    break;
                }
                else if (mapTile.getSubBiom() == SUBBIOM.Street)
                {
                    edgeObjectCount = Random.Range(streetMinEdgeObjects, streetMaxEdgeObjects + 1);
                    midd.Add(middStreet[Random.Range(0, middStreet.Length)]);
                    edgeWidth = streetEdgeWidth;
                    isStreet = true;
                }
                else
                {
                    Debug.LogWarning("no SubBiom case: " + mapTile.getSubBiom());
                }
                break;
            case BIOM.Woods:
                ground = groundForest;
                edge.AddRange(edgeForest);
                if (mapTile.getSubBiom() == SUBBIOM.EdgeOfForest)
                {
                    edgeObjectCount = Random.Range(forestEdgeMinEdgeObjects, forestEdgeMaxEdgeObjects + 1);
                    edgeWidth = forestEdgeWidth;
                    break;
                }
                else if (mapTile.getSubBiom() == SUBBIOM.DarkWood)
                {
                    edgeObjectCount = Random.Range(forestMinEdgeObjects, forestMaxEdgeObjects + 1);
                    edgeWidth = forestEdgeWidth;
                    break;
                }
                else if (mapTile.getSubBiom() == SUBBIOM.Street)
                {
                    edgeObjectCount = Random.Range(streetMinEdgeObjects, streetMaxEdgeObjects + 1);
                    midd.Add(middStreet[Random.Range(0, middStreet.Length)]);
                    edgeWidth = streetEdgeWidth;
                    isStreet = true;
                }
                else
                {
                    Debug.LogWarning("no SubBiom case: " + mapTile.getSubBiom());
                }
                break;
            case BIOM.Town:
                ground = groundTown;
                edgeWidth = streetEdgeWidth;
                GameObject tile = mapTile.getTile();
                if (map.isTileCityWall(new Vector2Int(Mathf.RoundToInt(tile.transform.position.x), Mathf.RoundToInt(tile.transform.position.z))))
                {
                    edge.AddRange(edgeGrass);
                    edgeObjectCount = Random.Range(grassMinEdgeObjects, grassMaxEdgeObjects + 1);
                    midd.Add(middTownWall[Random.Range(0, middTownWall.Length)]);
                    midd.Add(middStreet[Random.Range(0, middStreet.Length)]);
                    break;
                }
                else
                {
                    edge.AddRange(edgeTown);
                    if (mapTile.getSubBiom() == SUBBIOM.TownCenter)
                    {
                        edgeObjectCount = Random.Range(townMinEdgeObjects, townMaxEdgeObjects + 1);
                        midd.Add(middTown[Random.Range(0, middTown.Length)]);
                        break;
                    }
                    else if (mapTile.getSubBiom() == SUBBIOM.Street)
                    {
                        edgeObjectCount = Random.Range(streetMinEdgeObjects, streetMaxEdgeObjects + 1);
                        midd.Add(middStreet[Random.Range(0, middStreet.Length)]);
                        isStreet = true;
                    }
                    else
                    {
                        Debug.LogWarning("no SubBiom case: " + mapTile.getSubBiom());
                    }
                }
                break;
            case BIOM.Farm:
                ground = groundFarm;
                if (mapTile.getSubBiom() == SUBBIOM.Farmhouse)
                {
                    edgeObjectCount = Random.Range(farmMinEdgeObjects, farmMaxEdgeObjects + 1);
                    edge.AddRange(edgeFarm);
                    edgeWidth = streetEdgeWidth;
                }
                else if (mapTile.getSubBiom() == SUBBIOM.Field)
                {
                    break;
                }
                else
                {
                    Debug.LogWarning("no SubBiom case: " + mapTile.getSubBiom());
                }
                break;
            default:
                ground = groundGrass;
                Debug.LogWarning("no Biom case: " + mapTile.getBiom());
                break;
        }
        for (int i = 0; i < encounterObj.Length; i++)
        {
            if (encounterObj[i] == "randomE")
            {
                int roll;
                roll = Random.Range(0, enemyCount);
                if (roll == 0)
                {
                    if (!AddToBack("Thief1")) { i--; continue; }
                }
                else if (roll == 1)
                {
                    if (!AddToBack("Thief2")) { i--; continue; }
                }
                else if (roll == 2)
                {
                    if (!AddToBack("Thief3")) { i--; continue; }
                }
                else if (roll == 3)
                {
                    if (!AddToBack("Thief4")) { i--; continue; }
                }
                else if (roll == 4)
                {
                    if (!AddToBack("Bandit1")) { i--; continue; }
                }
                else if (roll == 5)
                {
                    if (!AddToBack("Bandit2")) { i--; continue; }
                }
                else if (roll == 6)
                {
                    if (!AddToBack("Bandit3")) { i--; continue; }
                }
                else if (roll == 7)
                {
                    if (!AddToBack("Bandit4")) { i--; continue; }
                }
                else if (roll == 8)
                {
                    if (!AddToBack("Bandit5")) { i--; continue; }
                }
            }
            else if (encounterObj[i] == "randomHE")
            {
                int roll;
                roll = Random.Range(0, heavyEnemyCount);
                if (roll == 0)
                {
                    if (!AddToBack("Solider1")) { i--; continue; }
                }
                else if (roll == 1)
                {
                    if (!AddToBack("Solider2")) { i--; continue; }
                }
                else if (roll == 2)
                {
                    if (!AddToBack("Solider3")) { i--; continue; }
                }
                else if (roll == 3)
                {
                    if (!AddToBack("Knight")) { i--; continue; }
                }
            }
            else
            {
                GameObject backObj;
                backObjs.TryGetValue(encounterObj[i], out backObj);
                back.Add(backObj);
            }
        }
        BuildTile(edgeObjectCount, encounterEnemyCount);
    }
    private void BuildTile( int edgeObjectCount, int encounterEnemyCount)
    {
        int edgeCounter = 1;
        int backCounter = 1;
        int sideSwitchPoint = Mathf.RoundToInt(edgeObjectCount / Random.Range(sideSwitchDivMin, sideSwitchDivMax));
        ground.transform.position = gameObject.transform.position;
        foreach (GameObject edgeO in edge)
        {
            int roll;
            roll = Random.Range(0, edge.Count);
            GameObject[] edgeObj = edge.ToArray();
            if (edgeCounter > edgeObjectCount)
            {
                break;
            }
            else if (edgeCounter > sideSwitchPoint)
            {
                EdgePositionControll edgePositionControll = edgeObj[roll].GetComponent<EdgePositionControll>();
                edgePositionControll.SetBuildNumber(edgeCounter);
                edgePositionControll.placeTry = 0;
                RollEdgePosition("l", edgeObj[roll]);
            }
            else
            {
                EdgePositionControll edgePositionControll = edgeObj[roll].GetComponent<EdgePositionControll>();
                edgePositionControll.SetBuildNumber(edgeCounter);
                edgePositionControll.placeTry = 0;
                RollEdgePosition("r", edgeObj[roll]);
            }
            edgeCounter++;
        }
        foreach (GameObject middObj in midd)
        {
            middObj.transform.position = gameObject.transform.position;
        }
        foreach (GameObject backObj in back)
        {
            if (backCounter == 2 && encounterEnemyCount >= 2)
            {
                backObj.transform.position = new Vector3(gameObject.transform.position.x + npcXSpacing, gameObject.transform.position.y, gameObject.transform.position.z);
            }
            else if (backCounter == 3 && encounterEnemyCount >= 3)
            {
                backObj.transform.position = new Vector3(gameObject.transform.position.x - npcXSpacing, gameObject.transform.position.y, gameObject.transform.position.z);
            }
            else if (backCounter == 4 && encounterEnemyCount >= 4)
            {
                backObj.transform.position = new Vector3(gameObject.transform.position.x + (npcXSpacing * 2), gameObject.transform.position.y, gameObject.transform.position.z);
            }
            else if (backCounter == 5 && encounterEnemyCount >= 5)
            {
                backObj.transform.position = new Vector3(gameObject.transform.position.x - (npcXSpacing * 2), gameObject.transform.position.y, gameObject.transform.position.z);
            }
            else
            {
                backObj.transform.position = gameObject.transform.position;
            }
            if (backCounter <= encounterEnemyCount)
            {
                Character newEnemy = backObj.GetComponent<Character>();
                newEnemy.reRollStats();
                enemies.Add(newEnemy);
            }
            backCounter++;
        }
        playerGroup.AddRange(caracterArrayHolder.playerGroup);
        int frontCounter = 1;
        foreach (Character character in playerGroup)
        {
            if (frontCounter == 2)
            {
                character.gameObject.transform.position = new Vector3(gameObject.transform.position.x + npcXSpacing, gameObject.transform.position.y, gameObject.transform.position.z);
            }
            else if (frontCounter == 3)
            {
                character.gameObject.transform.position = new Vector3(gameObject.transform.position.x - npcXSpacing, gameObject.transform.position.y, gameObject.transform.position.z);
            }
            else if (frontCounter == 4)
            {
                character.gameObject.transform.position = new Vector3(gameObject.transform.position.x + (npcXSpacing * 2), gameObject.transform.position.y, gameObject.transform.position.z);
            }
            else if (frontCounter == 5)
            {
                character.gameObject.transform.position = new Vector3(gameObject.transform.position.x - (npcXSpacing * 2), gameObject.transform.position.y, gameObject.transform.position.z);
            }
            else
            {
                character.gameObject.transform.position = gameObject.transform.position;
            }
            frontCounter++;
        }
    }
    public void RollEdgePosition(string side, GameObject edgeObj)
    {
        int rollX;
        int rollZ;
        rollX = Random.Range(0, edgeWidth);
        rollZ = Random.Range(0, zLength);
        if (isStreet)
        {
            if (side == "r")
            {
                edgeObj.transform.position = new Vector3(edgePosR.position.x - rollX, edgePosR.position.y+1, edgePosR.position.z + rollZ);
                edgeObj.transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            if (side == "l")
            {
                edgeObj.transform.position = new Vector3(edgePosL.position.x + rollX, edgePosL.position.y+1, edgePosL.position.z + rollZ);
                edgeObj.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
        else
        {
            if (side == "r")
            {
                edgeObj.transform.position = new Vector3(edgePosR.position.x - rollX, edgePosR.position.y, edgePosR.position.z + rollZ);
                edgeObj.transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            if (side == "l")
            {
                edgeObj.transform.position = new Vector3(edgePosL.position.x + rollX, edgePosL.position.y, edgePosL.position.z + rollZ);
                edgeObj.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }
    bool AddToBack(string npc)
    {
        GameObject backObj;
        backObjs.TryGetValue(npc, out backObj);
        if (back.Contains(backObj))
        {
            return false;
        }
        else
        {
            back.Add(backObj);
            return true;
        }
    }
    void ResetEncounter()
    {
        if(ground != null)
        {
            ground.transform.localPosition = Vector3.zero;
            foreach (GameObject middObj in midd)
            {
                middObj.transform.localPosition = Vector3.zero;
            }
            foreach (GameObject backObj in back)
            {
                backObj.transform.localPosition = Vector3.zero;
            }
            foreach (GameObject edgeObj in edge)
            {
                EdgePositionControll edgePositionControll = edgeObj.GetComponent<EdgePositionControll>();
                edgePositionControll.SetBuildNumber(0);
                edgePositionControll.placeTry = 0;
                edgeObj.transform.localPosition = Vector3.zero;
            }
        }
    }
    public List<Character> GetEnemies()
    {
        return enemies;
    }
}
