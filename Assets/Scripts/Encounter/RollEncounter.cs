using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENCOUNTER_TYPE
	{
	Fight,
	Dialogue,
	Trade,
	Loot
	}

public class RollEncounter : MonoBehaviour
	{
	[SerializeField] SoundManager soundmanager;
	[SerializeField] Transform cameraEncounterPos;
	[SerializeField] Transform cameraMapPos;
	[SerializeField] DialogeManager dialogeManager;
	[SerializeField] CombatManager combatManager;
	[SerializeField] CharacterManager characterManager;
	[SerializeField] BuildEncounter buildEncounter;
	[SerializeField] GroupManager groupManager;

	// [SerializeField] int noEnemyChance = 60;
	[Header("EncounterType")]
	[SerializeField] int eChanceFight = 10;
	[SerializeField] int eChanceDialoge = 35;
	[SerializeField] int eChanceTrader = 10;
	[SerializeField] int eChanceObjects = 20;
	[Header("EncounterSubtypeFight")]
	[SerializeField] int eChanceRoadblock = 50;
	[SerializeField] int eChanceThiefs = 35;
	// [SerializeField] int eChanceArmy = 15;
	[Header("EncounterSubtypeDialoge")]
	[SerializeField] int eChanceRefugees = 25;
	[SerializeField] int eChancePoor = 25;
	[SerializeField] int eChanceRich = 20;
	[SerializeField] int eChanceFarmer = 20;
	// [SerializeField] int eChanceDoctor = 10;
	[Header("EncounterSubtypeTrader")]
	// [SerializeField] int eChanceEquipment = 40;
	// [SerializeField] int eChanceAccident = 10;
	// [SerializeField] int eChanceFood = 50;
	[Header("EncounterSubtypeObjects")]
	// [SerializeField] int eChanceChest = 10;
	// [SerializeField] int eChanceBerries = 25;
	// [SerializeField] int eChanceCart = 15;
	// [SerializeField] int eChanceHouse = 5;
	// [SerializeField] int eChanceCorpse = 5;
	// [SerializeField] int eChanceCamp = 20;
	// [SerializeField] int eChanceRuin = 20;

	private static System.Random random = new System.Random(); // Generates ints, I like ints
	int eChanceRoadblockModified = 0;
	int eChanceThiefsModified = 0;
	int encounterEnemyCount = 0;
	string[] encounterObj;
	string dialoge;
	CameraManager cameraManager;

	public bool RollNewEncounter(MapTile mapTile)
		{
		if(mapTile.getBiom() != BIOM.Mountain)
			{
			encounterEnemyCount = 0;
			int roll;
			roll = Random.Range(1, 101);
			if(roll <= eChanceFight)
				{
				if(mapTile.getSubBiom() != SUBBIOM.Street)
					{
					eChanceRoadblockModified = 0;
					eChanceThiefsModified = eChanceThiefs + eChanceRoadblock;
					}
				else
					{
					eChanceRoadblockModified = eChanceRoadblock;
					eChanceThiefsModified = eChanceThiefs;
					}
				buildEncounter.CategorizeTile(mapTile, SpecializeEncounter(ENCOUNTER_TYPE.Fight), encounterEnemyCount);
				StartCoroutine(StartEncounter());
				combatManager.StartFight();
				return true;

				}
			else if(roll <= eChanceFight + eChanceDialoge)
				{
				buildEncounter.CategorizeTile(mapTile, SpecializeEncounter(ENCOUNTER_TYPE.Dialogue), encounterEnemyCount);
				StartCoroutine(StartEncounter());
				dialogeManager.StartDialoge(dialoge);
				return true;
				}
			else if(roll <= eChanceFight + eChanceDialoge + eChanceObjects + eChanceTrader)
				{
				buildEncounter.CategorizeTile(mapTile, SpecializeEncounter(ENCOUNTER_TYPE.Trade), encounterEnemyCount);
				StartCoroutine(StartEncounter());
				return true;
				}
			else if(roll <= eChanceFight + eChanceDialoge + eChanceObjects)
				{
				buildEncounter.CategorizeTile(mapTile, SpecializeEncounter(ENCOUNTER_TYPE.Loot), encounterEnemyCount);
				StartCoroutine(StartEncounter());
				return true;
				}
			else
				{
				return false; // Could cut this
				}
			}
		else
			{
			return false; // Could cut this
			}
		// Paste return false here -> -7 lines of code
		}

	//NPCs first
	Dictionary<BACK_OBJECT_TYPE, int> SpecializeEncounter(ENCOUNTER_TYPE encounterType)
		{
		int[] backObjectTypeCounts = new int[System.Enum.GetValues(typeof(BACK_OBJECT_TYPE)).Length];
		int roll = Random.Range(1, 101);
		if(encounterType == ENCOUNTER_TYPE.Fight)
			{
			if(roll <= eChanceRoadblockModified)//Roadblock
				{
				RandomEnemy(2);
				encounterEnemyCount += 2;

				for(int I = 0; I < encounterEnemyCount; ++I)
					{
					++backObjectTypeCounts[random.Next(0, 2)];  // 0 = Thief, 1 = Bandit
					}
				++backObjectTypeCounts[7];                      // 7 = Barricade
				}
			else if(roll <= eChanceRoadblock + eChanceThiefsModified)//Thiefs
				{
				RandomEnemy(3);
				encounterEnemyCount += 1;

				for(int I = 0; I < encounterEnemyCount; ++I)
					{
					++backObjectTypeCounts[random.Next(0, 2)];  // 0 = Thief, 1 = Bandit
					}
				}
			else//Army
				{
				RandomEnemy(1);
				encounterEnemyCount += 3;

				for(int I = 0; I < 2; ++I)
					{
					++backObjectTypeCounts[random.Next(2, 4)];  // 2 = Soldier, 4 = Knight
					}
				for(int I = 2; I < encounterEnemyCount; ++I)
					{
					++backObjectTypeCounts[random.Next(0, 2)];  // 0 = Thief, 1 = Bandit
					}
				}
			}
		else if(encounterType == ENCOUNTER_TYPE.Dialogue)
			{
			if(roll <= eChanceRefugees)//Refugees
				{
				dialoge = "refugee1";
				encounterEnemyCount = 3;

				for(int I = 0; I < encounterEnemyCount; ++I)
					{
					++backObjectTypeCounts[random.Next(4, 7)];  // 4 = SoldierN, 5 = WoodcutterN, 6 = WomanN
					}
				}
			/*else if(roll <= eChanceRefugees + eChancePoor)//Poor
				{
				// TODO: Implement
				}
			else if(roll <= eChanceRefugees + eChancePoor + eChanceRich)//Rich
				{
				// TODO: Implement
				}*/
			else if(roll <= eChanceRefugees + eChancePoor + eChanceRich + eChanceFarmer)//Farmer
				{
				dialoge = "farmer1";
				encounterEnemyCount = 1;
				
				++backObjectTypeCounts[random.Next(4, 7)];  // 4 = SoldierN, 5 = WoodcutterN, 6 = WomanN
				}
			else//Doctor
				{
				dialoge = "doctor1";
				encounterEnemyCount = 1;
				
				++backObjectTypeCounts[random.Next(4, 7)];  // 4 = SoldierN, 5 = WoodcutterN, 6 = WomanN
				}
			}
		/*else if(encounterType == ENCOUNTER_TYPE.Trade)
			{
			// TODO: Implement
			}
		else if(encounterType == ENCOUNTER_TYPE.Loot)
			{
			// TODO: Implement
			}*/

		Dictionary<BACK_OBJECT_TYPE, int> backObjects = new Dictionary<BACK_OBJECT_TYPE, int>(4);
		int U = 0;
		foreach(BACK_OBJECT_TYPE objectType in System.Enum.GetValues(typeof(BACK_OBJECT_TYPE)))
			{
			backObjects.Add(objectType, backObjectTypeCounts[U++]);
			}
		return backObjects;
		}

	private void RandomEnemy(int count)
		{
		for(int i = 1; i <= count; i++)
			{
			int rollE = Random.Range(1, 101);
			if(rollE > 60)
				{
				encounterEnemyCount++;
				}
			}
		}

	IEnumerator StartEncounter()
		{
		yield return new WaitForSeconds(1);
		cameraManager = Camera.main.gameObject.GetComponent<CameraManager>();
		if(cameraManager.mapactive)
			{
			cameraManager.mapactive = false;
			}
		else
			{
			Debug.Log("Bug confirmed, second encounter started :/"); // TODO: delete
			}
		// cameraMapPos.position = Camera.main.transform.position; // Sry für unangekuendigte Änderung, will dir aber nicht jedes Mal ne Nachricht schreiben, weil du gesagt hast, dass du das auch nicht magst.
		// cameraMapPos.rotation = Camera.main.transform.rotation; // Remembers the current map view to restore it after the encounter
		// nvm, caused bugs anyways, TODO: Maybe enable, fix and test later
		Camera.main.transform.position = cameraEncounterPos.position;
		Camera.main.transform.rotation = cameraEncounterPos.rotation;

		soundmanager.playFightMusic(); // TODO: probably shouldnt start fight music on peaceful encounters
		}

	public void EndEncounter(int proceed)
		{
		if(proceed == 0)
			{
			if(cameraManager != null)
				{
				Camera.main.transform.position = cameraMapPos.position;
				Camera.main.transform.rotation = cameraMapPos.rotation;
				cameraManager.mapactive = true; // TODO: NullReferenceException: Object reference not set to an instance of an object (at Assets/Scripts/Encounter/RollEncounter.cs:240)
				soundmanager.playMapMusic();
				groupManager.endEncounter();
				}
			else
				{
				Debug.Log("cameraManager is null :/ why tho?"); // TODO: delete
				}
			}
		else if(proceed == 1)
			{
			combatManager.StartFight();
			}
		/*else if (proceed == 2)
			{
            //characterManager.ad
			}*/
		}
	}


