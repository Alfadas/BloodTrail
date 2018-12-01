using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplexNoise;

public class Map {

    private int width=100;
    private int height=100;
    private GameObject prefabTile;
    private Dictionary<BIOM, Material> materials;
    private Dictionary<Vector2Int, MapTile> tiles;

    private int seed;

    public MapTile getTile(Vector2Int coordinates) {
        MapTile t;
        return tiles.TryGetValue(coordinates,out t) ? t : null;
    }

    public Map(int seed,int width,int height,GameObject prefabTile, Dictionary<BIOM, int> biomChance, Dictionary<SUBBIOM, int> subBiomChance, Dictionary<BIOM, Material> materials)
    {
        if (seed > 100000||seed<0)
        {
            throw new System.Exception("Seed must be ]0-100000]");
        }
        this.width = width;
        this.height = height;
        this.prefabTile = prefabTile;
        this.seed = seed;
        this.materials = materials;
        this.tiles = new Dictionary<Vector2Int, MapTile>();

        int sumBiomChance = 0;
        foreach (int chance in biomChance.Values) {
            sumBiomChance += chance;
        }
        Dictionary<int, int> sumSubBiomChance = new Dictionary<int, int>();
        //Debug.Log(subBiomChance.Keys);
        foreach(KeyValuePair<SUBBIOM, int> sub in subBiomChance)
        {   if (!sumSubBiomChance.ContainsKey((int)sub.Key / 10))
                sumSubBiomChance[(int)sub.Key / 10] = 0;
            sumSubBiomChance[(int)sub.Key/ 10] += sub.Value;
        }

        int offset = 500;

        GameObject map = GameObject.Find("Map");
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float noiseValue = (Noise.Generate(x + seed, z + seed)+1)/2;
                float noiseValueSub = (Noise.Generate(x + seed+ offset, z + seed+ offset)+1)/2;

                float prop = 0f;
                BIOM biom=BIOM.Desert;

                foreach (KeyValuePair<BIOM, int> chance in biomChance) {
                    biom = chance.Key;
                    prop += chance.Value/(float)sumBiomChance;
                 
                    if (prop > noiseValue)
                        break;
                }
                Debug.Log(prop);
                float subProp = 0f;
                SUBBIOM subBiom = SUBBIOM.DesertHot;
                foreach (KeyValuePair<SUBBIOM, int> chance in subBiomChance) {
                   
                    if (((int)chance.Key/10) ==(int) biom)
                    {
                        subBiom = chance.Key;
                        subProp += chance.Value /(float) sumSubBiomChance[(int)biom];
                        
                        if (subProp > noiseValueSub)
                            break;
                    }
                }
                GameObject tile = UnityEngine.Object.Instantiate(prefabTile, new Vector3(x, 0, z), Quaternion.identity);
                tile.GetComponent<MeshRenderer>().material = materials[biom];
                tile.transform.parent = map.transform;

                
                tiles[new Vector2Int(x, z)] = new MapTile(tile, biom, subBiom);
               // Debug.Log(materials[0].name);
            }
        }
    }
}
