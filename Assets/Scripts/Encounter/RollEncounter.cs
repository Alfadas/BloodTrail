using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollEncounter : MonoBehaviour {
    [SerializeField] BuildEncounter buildEncounter;
    [SerializeField] int noEnemyChance = 60;
    [Header("EncounterType")]
    [SerializeField] int eChanceFight = 10;
    [SerializeField] int eChanceDialoge = 35;
    [SerializeField] int eChanceTrader = 10;
    [SerializeField] int eChanceObjects = 20;
    [Header("EncounterSubtypeFight")]
    [SerializeField] int eChanceRoadblock = 50;
    [SerializeField] int eChanceThiefs = 35;
    [SerializeField] int eChanceArmy = 15;
    [Header("EncounterSubtypeDialoge")]
    [SerializeField] int eChanceRefugees = 25;
    [SerializeField] int eChancePoor = 25;
    [SerializeField] int eChanceRitch = 20;
    [SerializeField] int eChanceFarmer = 20;
    [SerializeField] int eChanceDoctor = 10;
    [Header("EncounterSubtypeTrader")]
    [SerializeField] int eChanceEquipment = 40;
    [SerializeField] int eChanceAccident = 10;
    [SerializeField] int eChanceFood = 50;
    [Header("EncounterSubtypeObjects")]
    [SerializeField] int eChanceChest = 10;
    [SerializeField] int eChanceBerries = 25;
    [SerializeField] int eChanceCart = 15;
    [SerializeField] int eChanceHouse = 5;
    [SerializeField] int eChanceCorpse = 5;
    [SerializeField] int eChanceCamp = 20;
    [SerializeField] int eChanceRuin = 20;


    int eChanceRoadblockModified = 0;
    int eChanceThiefsModified = 0;
    int encounterEnemyCount = 0;
    string[] encounterObj;

    public void RollNewEncounter(MapTile mapTile)
    {
        if (mapTile.getBiom() != BIOM.Mountain)
        {
            encounterEnemyCount = 0;
            int roll;
            roll = Random.Range(1, 101);
            if (roll <= eChanceFight)
            {
                if (mapTile.getSubBiom() != SUBBIOM.Street)
                {
                    eChanceRoadblockModified = 0;
                    eChanceThiefsModified = eChanceThiefs + eChanceRoadblock;
                }
                else
                {
                    eChanceRoadblockModified = eChanceRoadblock;
                    eChanceThiefsModified = eChanceThiefs;
                }
                buildEncounter.CategorizeTile(mapTile, SpecializeEncounterFight(), encounterEnemyCount);
            }
            else if (roll <= eChanceFight + eChanceDialoge)
            {
                buildEncounter.CategorizeTile(mapTile, SpecializeEncounterDialoge(), encounterEnemyCount);
            }
            else if (roll <= eChanceFight + eChanceDialoge + eChanceObjects)
            {
                buildEncounter.CategorizeTile(mapTile, SpecializeEncounterObjects(), encounterEnemyCount);
            }
            else if (roll <= eChanceFight + eChanceDialoge + eChanceObjects + eChanceTrader)
            {
                buildEncounter.CategorizeTile(mapTile, SpecializeEncounterTrader(), encounterEnemyCount);
            }
        }
    }

    //NPCs first
    string[] SpecializeEncounterFight()
    {
        int roll;
        roll = Random.Range(1, 101);
        if (roll <= eChanceRoadblockModified)//Roadblock
        {
            RandomEnemy(2);
            encounterEnemyCount += 2;
            if (encounterEnemyCount == 2)
            {
                return new string[] { "randomE", "randomE", "Barricade" };
            }
            else if (encounterEnemyCount == 3)
            {
                return new string[] { "randomE", "randomE", "randomE", "Barricade" };
            }
            else
            {
                return new string[] { "randomE", "randomE", "randomE", "randomE", "Barricade" };
            }
            
            
        }
        else if (roll <= eChanceRoadblock + eChanceThiefsModified)//Thiefs
        {
            RandomEnemy(3);
            encounterEnemyCount += 1;
            if (encounterEnemyCount == 1)
            {
                return new string[] { "randomE"};
            }
            else if (encounterEnemyCount == 2)
            {
                return new string[] { "randomE", "randomE"};
            }
            else if (encounterEnemyCount == 3)
            {
                return new string[] { "randomE", "randomE", "randomE" };
            }
            else
            {
                return new string[] { "randomE", "randomE", "randomE", "randomE" };
            }
        }
        else//Army
        {
            RandomEnemy(1);
            encounterEnemyCount += 3;
            if (encounterEnemyCount == 3)
            {
                return new string[] { "randomHE", "randomHE", "randomE" };
            }
            else
            {
                return new string[] { "randomHE", "randomHE", "randomE", "randomE" };
            }
            
        }
    }

    private void RandomEnemy(int count)
    {
        for (int i = 1; i<= count; i++)
        {
            int rollE = Random.Range(1, 101);
            if (rollE > 60)
            {
                encounterEnemyCount++;
            }
        }
    }

    string[] SpecializeEncounterDialoge()
    {
        int roll;
        roll = Random.Range(1, 101);
        if (roll <= eChanceRefugees)//Refugees
        {
            return new string[] { };
        }
        else if (roll <= eChanceRefugees + eChancePoor )//Poor
        {
            return new string[] { };
        }
        else if (roll <= eChanceRefugees + eChancePoor + eChanceRitch )//Ritch
        {
            return new string[] { };
        }
        else if (roll <= eChanceRefugees + eChancePoor + eChanceRitch + eChanceFarmer)//Farmer
        {
            return new string[] { };
        }
        else//Doctor
        {
            return new string[] { };
        }
        
    }
    string[] SpecializeEncounterObjects()
    {

        return new string[] { };
    }
    string[] SpecializeEncounterTrader()
    {

        return new string[] { };
    }
    
}
