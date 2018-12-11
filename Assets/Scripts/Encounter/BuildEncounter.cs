using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BACK_OBJECT_TYPE // After 4h of coding Im not sure anymore, whether this is really better than a string[], cleaner and more error resistant for sure, but way more complicated in handling
	{
	Thief,
	Bandit,
	Soldier,
	Knight,
	SoldierN,
	WoodcutterN,
	WomanN,
	Barricade
	}

public class BuildEncounter : MonoBehaviour
	{
	[SerializeField] CharacterManager characterManager;
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
	[Header("Mids")]
	[SerializeField] GameObject[] midStreet;
	[SerializeField] GameObject[] midTown;
	[SerializeField] GameObject[] midTownWall;
	[Header("Backs")]
	[SerializeField] GameObject[] thieves; // NOUN  a thief | thieves
	[SerializeField] GameObject[] bandits;
	[SerializeField] GameObject[] soldiers;
	[SerializeField] GameObject[] knights;
	[SerializeField] GameObject[] soldiersN;
	[SerializeField] GameObject[] woodcuttersN;
	[SerializeField] GameObject[] womansN;
	[SerializeField] GameObject[] barricades; // Well, array is rather senseless here, but consistent at least, maybe you want more barricades later?
	[Header("Enemies")]
	[SerializeField] int enemyCount;
	[SerializeField] int heavyEnemyCount;
	// [SerializeField] int neutralCount = 4;

	private static System.Random random = new System.Random();

	private Dictionary<BACK_OBJECT_TYPE, GameObject[]> backObjectPrefabs; 
	GameObject ground;
	public List<GameObject> mid;
	public List<GameObject> edge;
	public List<GameObject> back;
	List<Character> enemies;
	List<Character> playerGroup;
	Map map;
	int edgeWidth = 5;
	bool isStreet;

	// Use this for initialization
	void Start()
		{
		// This idea didnt turn out as tidy, as I had expected, but still better than before imho, because the inspector should be better sorted and I could cut out chunks of the methods below
		backObjectPrefabs = new Dictionary<BACK_OBJECT_TYPE, GameObject[]>(System.Enum.GetValues(typeof(BACK_OBJECT_TYPE)).Length); // Initialize Dictionary size with number of values for the BACK_OBJECT_TYPE enum
		backObjectPrefabs[BACK_OBJECT_TYPE.Thief] = thieves;
		backObjectPrefabs[BACK_OBJECT_TYPE.Bandit] = bandits;
		backObjectPrefabs[BACK_OBJECT_TYPE.Soldier] = soldiers;
		backObjectPrefabs[BACK_OBJECT_TYPE.Knight] = knights;
		backObjectPrefabs[BACK_OBJECT_TYPE.SoldierN] = soldiersN;
		backObjectPrefabs[BACK_OBJECT_TYPE.WoodcutterN] = woodcuttersN;
		backObjectPrefabs[BACK_OBJECT_TYPE.WomanN] = womansN;
		backObjectPrefabs[BACK_OBJECT_TYPE.Barricade] = barricades;
		}

	public void CategorizeTile(MapTile mapTile, Dictionary<BACK_OBJECT_TYPE, int> requestedBackObjects, int encounterEnemyCount)
		{
		map = mapManager.getMap();
		ResetEncounter();
		ground = null;
		edge = new List<GameObject>();
		mid = new List<GameObject>();
		back = new List<GameObject>();
		enemies = new List<Character>();
		playerGroup = new List<Character>();
		int edgeObjectCount = 0;
		isStreet = false;

		switch(mapTile.getBiom())
			{
			case BIOM.Grassland:
				ground = groundGrass;
				edge.AddRange(edgeGrass);
				if(mapTile.getSubBiom() == SUBBIOM.Steppe || mapTile.getSubBiom() == SUBBIOM.Greenfield)
					{
					edgeObjectCount = Random.Range(grassMinEdgeObjects, grassMaxEdgeObjects + 1);
					edgeWidth = grassEdgeWidth;
					break;
					}
				else if(mapTile.getSubBiom() == SUBBIOM.Street)
					{
					edgeObjectCount = Random.Range(streetMinEdgeObjects, streetMaxEdgeObjects + 1);
					mid.Add(midStreet[Random.Range(0, midStreet.Length)]); // TODO: see below
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
				if(mapTile.getSubBiom() == SUBBIOM.EdgeOfForest)
					{
					edgeObjectCount = Random.Range(forestEdgeMinEdgeObjects, forestEdgeMaxEdgeObjects + 1);
					edgeWidth = forestEdgeWidth;
					break;
					}
				else if(mapTile.getSubBiom() == SUBBIOM.DarkWood)
					{
					edgeObjectCount = Random.Range(forestMinEdgeObjects, forestMaxEdgeObjects + 1);
					edgeWidth = forestEdgeWidth;
					break;
					}
				else if(mapTile.getSubBiom() == SUBBIOM.Street)
					{
					edgeObjectCount = Random.Range(streetMinEdgeObjects, streetMaxEdgeObjects + 1);
					mid.Add(midStreet[Random.Range(0, midStreet.Length)]);
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
				if(map.isTileCityWall(new Vector2Int(Mathf.RoundToInt(tile.transform.position.x), Mathf.RoundToInt(tile.transform.position.z))))
					{
					edge.AddRange(edgeGrass);
					edgeObjectCount = Random.Range(grassMinEdgeObjects, grassMaxEdgeObjects + 1);
					mid.Add(midTownWall[Random.Range(0, midTownWall.Length)]);
					mid.Add(midStreet[Random.Range(0, midStreet.Length)]);
					break;
					}
				else
					{
					edge.AddRange(edgeTown);
					if(mapTile.getSubBiom() == SUBBIOM.TownCenter)
						{
						edgeObjectCount = Random.Range(townMinEdgeObjects, townMaxEdgeObjects + 1);
						mid.Add(midTown[Random.Range(0, midTown.Length)]);
						break;
						}
					else if(mapTile.getSubBiom() == SUBBIOM.Street)
						{
						edgeObjectCount = Random.Range(streetMinEdgeObjects, streetMaxEdgeObjects + 1);
						mid.Add(midStreet[Random.Range(0, midStreet.Length)]); // TODO: IndexOutOfRangeException: Array index is out of range. (at Assets/Scripts/Encounter/BuildEncounter.cs:192), probably Length was 0, see Range() Documentation
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
				if(mapTile.getSubBiom() == SUBBIOM.Farmhouse)
					{
					edgeObjectCount = Random.Range(farmMinEdgeObjects, farmMaxEdgeObjects + 1);
					edge.AddRange(edgeFarm);
					edgeWidth = streetEdgeWidth;
					}
				else if(mapTile.getSubBiom() == SUBBIOM.Field)
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

		foreach(BACK_OBJECT_TYPE type in requestedBackObjects.Keys)
			{
			for(int I = 0; I < requestedBackObjects[type]; ++I)
				{
				back.Add(backObjectPrefabs[type][I]);
				}
			}

		BuildTile(edgeObjectCount, encounterEnemyCount);
		}

	private void BuildTile(int edgeObjectCount, int encounterEnemyCount)
		{
		int edgeCounter = 1;
		int backCounter = 1;
		int sideSwitchPoint = Mathf.RoundToInt(edgeObjectCount / Random.Range(sideSwitchDivMin, sideSwitchDivMax));
		ground.transform.position = gameObject.transform.position;
		foreach(GameObject edgeO in edge)
			{
			int roll;
			roll = Random.Range(0, edge.Count);
			GameObject[] edgeObj = edge.ToArray();
			if(edgeCounter > edgeObjectCount)
				{
				break;
				}
			else if(edgeCounter > sideSwitchPoint)
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
		foreach(GameObject middObj in mid)
			{
			middObj.transform.position = gameObject.transform.position;
			}
		foreach(GameObject backObj in back)
			{
			if(backCounter == 2 && encounterEnemyCount >= 2)
				{
				backObj.transform.position = new Vector3(gameObject.transform.position.x + npcXSpacing, gameObject.transform.position.y, gameObject.transform.position.z);
				}
			else if(backCounter == 3 && encounterEnemyCount >= 3)
				{
				backObj.transform.position = new Vector3(gameObject.transform.position.x - npcXSpacing, gameObject.transform.position.y, gameObject.transform.position.z);
				}
			else if(backCounter == 4 && encounterEnemyCount >= 4)
				{
				backObj.transform.position = new Vector3(gameObject.transform.position.x + (npcXSpacing * 2), gameObject.transform.position.y, gameObject.transform.position.z);
				}
			else if(backCounter == 5 && encounterEnemyCount >= 5)
				{
				backObj.transform.position = new Vector3(gameObject.transform.position.x - (npcXSpacing * 2), gameObject.transform.position.y, gameObject.transform.position.z);
				}
			else
				{
				backObj.transform.position = gameObject.transform.position;
				}
			if(backCounter <= encounterEnemyCount)
				{
				Character newEnemy = backObj.GetComponent<Character>();
				newEnemy.reRollStats();
				newEnemy.heal(1000);
				enemies.Add(newEnemy);
				}
			backCounter++;
			}
		playerGroup = characterManager.getCharacters();
		int frontCounter = 1;
		foreach(Character character in playerGroup)
			{
			if(frontCounter == 2)
				{
				character.gameObject.transform.position = new Vector3(gameObject.transform.position.x + npcXSpacing, gameObject.transform.position.y, gameObject.transform.position.z);
				}
			else if(frontCounter == 3)
				{
				character.gameObject.transform.position = new Vector3(gameObject.transform.position.x - npcXSpacing, gameObject.transform.position.y, gameObject.transform.position.z);
				}
			else if(frontCounter == 4)
				{
				character.gameObject.transform.position = new Vector3(gameObject.transform.position.x + (npcXSpacing * 2), gameObject.transform.position.y, gameObject.transform.position.z);
				}
			else if(frontCounter == 5)
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
		if(isStreet)
			{
			if(side == "r")
				{
				edgeObj.transform.position = new Vector3(edgePosR.position.x - rollX, edgePosR.position.y + 1, edgePosR.position.z + rollZ);
				edgeObj.transform.rotation = Quaternion.Euler(0, -90, 0);
				}
			if(side == "l")
				{
				edgeObj.transform.position = new Vector3(edgePosL.position.x + rollX, edgePosL.position.y + 1, edgePosL.position.z + rollZ);
				edgeObj.transform.rotation = Quaternion.Euler(0, 90, 0);
				}
			}
		else
			{
			if(side == "r")
				{
				edgeObj.transform.position = new Vector3(edgePosR.position.x - rollX, edgePosR.position.y, edgePosR.position.z + rollZ);
				edgeObj.transform.rotation = Quaternion.Euler(0, -90, 0);
				}
			if(side == "l")
				{
				edgeObj.transform.position = new Vector3(edgePosL.position.x + rollX, edgePosL.position.y, edgePosL.position.z + rollZ);
				edgeObj.transform.rotation = Quaternion.Euler(0, 90, 0);
				}
			}
		}

	// Returns an array of n unique integers from the specified range (inclusive) // TODO: Move to another class, maybe make a utility class? Maybe also add method for Object-arrays
	// Turns out I didnt need this, but I wouldnt delete it right now, as it could come in handy later
	public int[] rollWithoutReturn(int min, int max, int n)
		{
		// Bring all numbers into an input array and initialize output array
		int[] numbers = new int[max - min + 1];
		int[] randomnumbers = new int[n];
		for(int I = 0; I < numbers.Length; ++I)
			{
			numbers[I] = I + min;
			}

		for(int I = 0; I < n; ++I)
			{
			// Draw random number from all remaining numbers and save index and value
			int luckyindex = random.Next(numbers.Length - I);
			int luckynumber = numbers[luckyindex];

			// Swap the lucky number with the last valid value to draw
			numbers[luckyindex] = numbers[numbers.Length - I - 1];
			numbers[numbers.Length - I - 1] = luckynumber;

			// Save the result for return
			randomnumbers[I] = luckynumber;

			// Implicitely (by for-loop) reduce the amount of remaining numbers by 1 and therefore make the last swapped value impossible to draw again
			}
		return randomnumbers;
		}

	void ResetEncounter()
		{
		if(ground != null)
			{
			ground.transform.localPosition = Vector3.zero;
			foreach(GameObject middObj in mid)
				{
				middObj.transform.localPosition = Vector3.zero;
				}
			foreach(GameObject backObj in back)
				{
				backObj.transform.localPosition = Vector3.zero;
				}
			foreach(GameObject edgeObj in edge)
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
