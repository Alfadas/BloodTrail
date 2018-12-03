using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum BIOM { Desert, Grassland, Woods, Town, Farm, Water, Mountain };
/**
 *  (int)subbiom / 10 should be the (int)Biom 
 */
public enum SUBBIOM { Street=-1, DesertHot, DesertCold, Steppe=10, Greenfield, DarkWood=20, EdgeOfForest, TownCenter=30, CityWall , Harbor, Field=40, Farmhouse, Shallow=50, Deep,Impassable=60, Passable }
/*
[System.Serializable] public class BiomDictionary : SerializableDictionary<BIOM, int> { }
[System.Serializable] public class SubBiomDictionary : SerializableDictionary<SUBBIOM, int> { }
[System.Serializable] public class MaterialDictionary : SerializableDictionary<BIOM, Material> { }*/

public class MapManager : MonoBehaviour {

    private Map map;

    [SerializeField]
    private int seed =0;
    [SerializeField]
    private int width = 100;
    [SerializeField]
    private int height = 100;
    [SerializeField]
    private GameObject prefabTile;
    [SerializeField]
    private Material[] mat;

    Dictionary<BIOM, Material> materials;
    [SerializeField]
    private Dictionary<BIOM, int> biomChance;
    [SerializeField]
    private Dictionary<SUBBIOM, int> subBiomChance;
    // Use this for initialization
    void Start () {
        biomChance = new Dictionary<BIOM, int>();
        materials = new Dictionary<BIOM, Material>();
        subBiomChance = new Dictionary<SUBBIOM, int>();

        biomChance[BIOM.Water] = 60;
        biomChance[BIOM.Farm] = 30;
        biomChance[BIOM.Farm] = 30;
        
        biomChance[BIOM.Woods] = 200;
        biomChance[BIOM.Town] = 1;
        biomChance[BIOM.Grassland] = 100;



        biomChance[BIOM.Farm] = 100;
        biomChance[BIOM.Mountain] = 150;

        //subBiomChance[SUBBIOM.CityWall] = 2;

        subBiomChance[SUBBIOM.DarkWood] = 2;
        subBiomChance[SUBBIOM.Deep] = 2;
        subBiomChance[SUBBIOM.DesertCold] = 2;
        subBiomChance[SUBBIOM.DesertHot] = 2;
        subBiomChance[SUBBIOM.EdgeOfForest] = 2;
       
        subBiomChance[SUBBIOM.Field] = 4;
        subBiomChance[SUBBIOM.Farmhouse] = 12;
        subBiomChance[SUBBIOM.Greenfield] = 2;
        subBiomChance[SUBBIOM.Impassable] = 2;
        subBiomChance[SUBBIOM.Passable] = 2;
 
        subBiomChance[SUBBIOM.Steppe] = 2;
        subBiomChance[SUBBIOM.TownCenter] = 2;

        foreach (Material m in mat) {
            switch (m.name) {
                case "Desert":
                    materials[BIOM.Desert] = m;
                    break;
                case "Farm":
                    materials[BIOM.Farm] = m;
                    break;
                case "Grassland":
                    materials[BIOM.Grassland] = m;
                    break;
                case "Mountain":
                    materials[BIOM.Mountain] = m;
                    break;
                case "Town":
                    materials[BIOM.Town] = m;
                    break;
                case "Water":
                    materials[BIOM.Water] = m;
                    break;
                case "Woods":
                    materials[BIOM.Woods] = m;
                    break;
            }
           
        }
        map = new Map(seed == 0 ? Random.Range(-1000000, 1000000) : seed, width, height, prefabTile, biomChance, subBiomChance, materials);

    }
    public Map getMap() {
        return map;
    }
	// Update is called once per frame
	void Update () {
        
    }
}
