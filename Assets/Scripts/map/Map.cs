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

    private float[] getDecrease(int x, int z){

        float retX = Mathf.Max(1 - Mathf.Log10(width-x ), 1 - Mathf.Log10(x ));



        float retZ = Mathf.Max(1 - Mathf.Log10(height - z ), 1 - Mathf.Log10(z ));
        //if (/* x > 6 &&x < width-6*/ )
            retX = 0;
        if (z > 6 /*&& z < height - 6*/)
            retZ = 0;
        
        //Debug.Log("retX="+retX+" retz="+retZ);
        float[] ret = new float[]{retX, retZ};
        return ret;

    }
    public Map(int seed,int width,int height,GameObject prefabTile, Dictionary<BIOM, int> biomChance, Dictionary<SUBBIOM, int> subBiomChance, Dictionary<BIOM, Material> materials)
    {
      
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
        float scale = 0.03f;
        float scale2 = 0.13f;
        Dictionary<Vector2Int, MapTile> cities = new Dictionary<Vector2Int, MapTile>(); 

        GameObject map = GameObject.Find("Map");
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float[] decrease = getDecrease(x, z);
                float noiseValue = (((Noise.Generate((x + seed)*scale, (z + seed) * scale) +1)/2+ 
                    (Noise.Generate((x + seed) * scale2, (z + seed) * scale2) + 1) / 2)/2) - Mathf.Max(decrease[0],decrease[1]);
                float noiseValueSub = (Noise.Generate((x + seed) * scale + offset, (z + seed)* scale + offset)+1)/2;

                float prop = 0f;
                BIOM biom=BIOM.Desert;

                foreach (KeyValuePair<BIOM, int> chance in biomChance) {
                    biom = chance.Key;
                    prop += chance.Value/(float)sumBiomChance;
                 
                    if (prop > noiseValue)
                        break;
                }
                //Debug.Log(prop);
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

                if (subBiom == SUBBIOM.TownCenter) {
                    cities[new Vector2Int(x, z)] = tiles[new Vector2Int(x, z)];
                }
                // Debug.Log(materials[0].name);
               
            }
           
        }
        //makeShoreLine(shoreSouth: true);
        makeCityStreets(cities);
    }
    private void makeCityStreets(Dictionary<Vector2Int,MapTile> cities) {


        Material road = Resources.Load("Materials/Map/Road", typeof(Material)) as Material;
        Material roadC = Resources.Load("Materials/Map/RoadCross", typeof(Material)) as Material;
        Material roadT = Resources.Load("Materials/Map/RoadT", typeof(Material)) as Material;
        Material roadD = Resources.Load("Materials/Map/RoadDiogonal", typeof(Material)) as Material;

        const int streetsPerCity = 1;
        const int weightWrong = 10;  
       
        Random.seed = this.seed;
        List<Vector2Int> keyList = new List<Vector2Int>(cities.Keys); 

        foreach (KeyValuePair < Vector2Int, MapTile> city in cities){
            for (int i=0; i < streetsPerCity; i++)
            {
                MapTile cityStart = city.Value;
                Vector2Int currentPos = city.Key;
              
                MapTile cityEnd = cities[keyList[Random.Range(0, keyList.Count)]];
                Vector3 cityEndVec = cityEnd.getTile().transform.position;
                Vector2Int cityEndPos = new Vector2Int((int)cityEndVec.x, (int)cityEndVec.z);
              
                Vector2Int LastPosition = new Vector2Int(-1,-1);

                while (currentPos != cityEndPos) {
                    Vector2Int direction = cityEndPos - currentPos;
                    int strongVal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? direction.x : direction.y;
                    int weakVal = Mathf.Abs(direction.x) < Mathf.Abs(direction.y) ? direction.x : direction.y;

                    Vector2Int strongDir = Mathf.Abs(direction.x) > Mathf.Abs(direction.y)?
                        (direction.x<0?Vector2Int.left: Vector2Int.right ):
                        (direction.y<0? Vector2Int.down: Vector2Int.up );
                    Vector2Int weakDir = Mathf.Abs(direction.x) < Mathf.Abs(direction.y) ?
                        (direction.x < 0 ? Vector2Int.left : Vector2Int.right) :
                        (direction.y < 0 ? Vector2Int.down : Vector2Int.up);
                    Vector2Int wrongDir = weakDir*(-1);
                    Vector2Int newPos;
                     int randVar = Random.Range(1, 100);
                    
                    if (randVar < weightWrong&& currentPos + wrongDir!=LastPosition)
                    {
                        newPos = currentPos+ wrongDir;
                        Debug.Log("wrong Dir");
                    }
                    else if (randVar < (100 - weightWrong) * (strongVal / ((float)strongVal + weakVal)) && currentPos + strongDir!=LastPosition)
                    {
                        newPos = currentPos + strongDir;
                        Debug.Log("strong Dir");
                    }
                    else if(currentPos + weakDir != LastPosition) {
                        newPos = currentPos + weakDir;
                        Debug.Log("weak Dir");
                    }
                    else
                    {
                        newPos = currentPos + strongDir;
                        Debug.Log("strong Dir");
                    }

                    //**check new pos**//
                    MapTile tileUp; 
                         tiles.TryGetValue(newPos + Vector2Int.up,out tileUp);
                    MapTile tileDown;
                         tiles.TryGetValue(newPos + Vector2Int.down, out tileDown);
                    MapTile tileLeft;
                         tiles.TryGetValue(newPos + Vector2Int.left, out tileLeft);
                    MapTile tileRight;
                         tiles.TryGetValue(newPos + Vector2Int.right,out tileRight);
                    Debug.Log(newPos);
                    GameObject newTile = tiles[newPos].getTile();

                    if (tiles[newPos].getBiom()==BIOM.Town) {
                        break;
                    }

                    bool up = tileUp!=null&&(tileUp.getBiom()==BIOM.Town|| tileUp.getSubBiom()==SUBBIOM.Street);
                    bool down = tileDown != null && (tileDown.getBiom() == BIOM.Town || tileDown.getSubBiom() == SUBBIOM.Street);
                    bool left = tileLeft != null && (tileLeft.getBiom() == BIOM.Town || tileLeft.getSubBiom() == SUBBIOM.Street);
                    bool right = tileRight != null && (tileRight.getBiom() == BIOM.Town || tileRight.getSubBiom() == SUBBIOM.Street);

                    int count = 0;
                    count += up ? 1 : 0;
                    count += down ? 1 : 0;
                    count += left ? 1 : 0;
                    count += right ? 1 : 0;

                    switch (count) {
                        case 1://normal Road
                            newTile.GetComponent<MeshRenderer>().material = road;
                            if (up || down)// turn road
                            {
                               newTile.transform.Rotate(Vector3.up * 90);      
                            }
                            Debug.Log("Road");
                            break;
                        case 2://t
                            newTile.GetComponent<MeshRenderer>().material = roadT; //when left right and down 
                            if (up&&left&&down) {
                                newTile.transform.Rotate(Vector3.up * 90);
                            } else if (left&up&right){
                               newTile.transform.Rotate(Vector3.up * 180);
                            } else if (up&right&down) {
                               newTile.transform.Rotate(Vector3.up * 270);
                            }
                            Debug.Log("t");
                            break;
                        case 3: //kreuzung
                        case 4:
                             newTile.GetComponent<MeshRenderer>().material = roadC; //when left right and down 
                            Debug.Log("c");
                            break;
                        default:
                            throw new System.Exception("something wong");
                    }

                    //check old possition
                    

                    tiles[newPos].setSubBiom(SUBBIOM.Street);
                    LastPosition = currentPos;
                    currentPos = newPos;
                }

            }
        }
    }

    private void makeShoreLine(bool shoreNord=false,bool shoreEast=false,bool shoreSouth= false ,bool shoreWest=false) {
    


        if (shoreSouth && !shoreNord && !shoreEast &&  !shoreWest) {
            int x = 0;
            int z = 0;

            MapTile positon = getTile(new Vector2Int(x, z));
            while (positon.getBiom() == BIOM.Water) {
                positon = getTile(new Vector2Int(x, ++z));
                Debug.Log(positon.getBiom());

            }
            //Debug.Log("troll");
            
        }

    }
    private void getShore(MapTile position,int x,int y) {
    /*    last
        Vector2int orientation = Vector2Int(Vector2.down);
        position();*/
    }
}
