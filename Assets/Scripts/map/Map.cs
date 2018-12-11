using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplexNoise;

public class Map {

    private int width=100;
    private int height=100;
    // private GameObject prefabTile;					// Only needed in this method, therfore no need to be instance variable
    // private Dictionary<BIOM, Material> materials;	// see above
    private Dictionary<Vector2Int, MapTile> tiles;
    private Dictionary<Vector2Int, MapTile> citySave;
    private float plageSpeed;
    private float plagePosition;


    private int seed;
    public int getWidth() {
        return width;
    }
    public int getHeight() {
        return height;
    }
    public MapTile getTile(Vector2Int coordinates) {
        MapTile t;
        return tiles.TryGetValue(coordinates,out t) ? t : null;
    }
    public bool isTileCityWall(Vector2Int v) {
        foreach (KeyValuePair<Vector2Int, MapTile> c in citySave) {
            float dist = Vector2Int.Distance(c.Key, v);
            if (dist < 2&& dist>0)
                return true;           
        }
        return false;
    }
    public float movePlage(float i=1) {
        plagePosition += i * plageSpeed;
        return plagePosition;

    }
    private Material getRandomMat(BIOM bio)
    {
        Material ret=null;
        int wat;
        switch (bio) {
            case BIOM.Woods:
               wat = Random.Range(0, 11);
           
                ret = Resources.Load("Materials/Map/Trees/Tree"+wat, typeof(Material)) as Material;

                break;
            case BIOM.Mountain:
                wat = Random.Range(2, 6);

                ret = Resources.Load("Materials/Map/Mountain/Mountain" + wat, typeof(Material)) as Material;

                break;
            case BIOM.Water:
                wat = Random.Range(1, 6);

                ret = Resources.Load("Materials/Map/Sea/Sea" + wat, typeof(Material)) as Material;

                break;

        }
        return ret;

    }
    public bool isTileCityWall(MapTile m) {

        return isTileCityWall(new Vector2Int((int)m.getTile().transform.position.x, (int)m.getTile().transform.position.y)); 
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
        this.plageSpeed = 1;
        this.plagePosition = 0;
        Debug.Log("seed= "+seed);
        if (seed < 0 && seed< 2147484) 
            throw new System.Exception("Seed musst be between [0 - 2147484]");

        this.width = width;
        this.height = height;
        // this.prefabTile = prefabTile;	// Only needed in this method, therfore no need to be instance variable
        this.seed = seed;
        // this.materials = materials;		// see above
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

                    if (prop > noiseValue) {
                        if (biom == BIOM.Farm && z < 10) {
                            biom = BIOM.Grassland;
                        }
                        if (biom == BIOM.Town && (x < 3 || x > width - 3 || z < 3 || z > height - 3))
                        {
                            continue;
                        }
                            break;
                        
                    }
                    
                       
                }
                //Debug.Log(prop);
                float subProp = 0f;
                SUBBIOM subBiom = SUBBIOM.DesertHot;
                foreach (KeyValuePair<SUBBIOM, int> chance in subBiomChance) {
                   
                    if (((int)chance.Key/10) ==(int) biom)
                    {
                        subBiom = chance.Key;
                        subProp += chance.Value /(float) sumSubBiomChance[(int)biom];

                        if ((int)subBiom / 10 == (int)BIOM.Woods && subProp > noiseValue)
                            break;
                        else if ((int)subBiom / 10 == (int)BIOM.Farm && subProp > noiseValue)
                            break;
                        else if (subBiom ==SUBBIOM.Deep && z > 10)
                        {
                            subBiom = SUBBIOM.Shallow; // lake 
                        }
                        if (subProp > noiseValueSub)
                            break;
                    }
                }
                GameObject tile = UnityEngine.Object.Instantiate(prefabTile, new Vector3(x, 0, z), Quaternion.identity);

                
                if(subBiom==SUBBIOM.Shallow)
                    tile.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Map/Lake", typeof(Material)) as Material;
                else if(subBiom==SUBBIOM.Farmhouse)
                    tile.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Map/FarmHouse", typeof(Material)) as Material;
                else if (biom == BIOM.Woods)
                    tile.GetComponent<MeshRenderer>().material = getRandomMat(BIOM.Woods);
                else if (biom == BIOM.Mountain)
                    tile.GetComponent<MeshRenderer>().material = getRandomMat(BIOM.Mountain);
                else if (biom == BIOM.Water)
                    tile.GetComponent<MeshRenderer>().material = getRandomMat(BIOM.Water);
                else
                    tile.GetComponent<MeshRenderer>().material = materials[biom];
                tile.transform.parent = map.transform;
                tiles[new Vector2Int(x, z)] = new MapTile(tile, biom, subBiom);

                if (subBiom == SUBBIOM.TownCenter) {
                    cities[new Vector2Int(x, z)] = tiles[new Vector2Int(x, z)];
                }
                // Debug.Log(materials[0].name);
               
            }
           
        }
        this.citySave = cities;
        makeHarbour(cities);
        //makeShoreLine(shoreSouth: true);
        makeCityStreets(cities);

         //Debug.Log("is City wall ? "+isTileCityWall(new Vector2Int(16, 14)));
    }
    private void makeHarbour(Dictionary<Vector2Int,MapTile> cities) {
         Material mat = Resources.Load("Materials/Map/Harbour", typeof(Material)) as Material;
        int offset=  Mathf.RoundToInt(width / 4);

        int t1c = 0;
        MapTile t1 = tiles[new Vector2Int( 0 + offset, t1c)];

        int t2c = 0;
        MapTile t2 = tiles[new Vector2Int(0 + offset * 2, t2c )];

        int t3c = 0;
        MapTile t3 = tiles[new Vector2Int(0 + offset * 3, t3c )];

        for (int i = 0; i<height;++i) {

            if (t1.getBiom() == BIOM.Water)
                t1 = tiles[new Vector2Int(0 + offset,++t1c )];
            if (t2.getBiom() == BIOM.Water)
                t2 = tiles[new Vector2Int(0 + offset * 2,++t2c)];
            if (t3.getBiom() == BIOM.Water)
                t3 = tiles[new Vector2Int(0 + offset * 3,++t3c)];

            if (t1.getBiom() != BIOM.Water && t2.getBiom() != BIOM.Water && t3.getBiom() != BIOM.Water)
                break;
        }
        t1.setBiom(BIOM.Town);
        t1.setSubBiom(SUBBIOM.Harbor);
        t1.getTile().GetComponent<MeshRenderer>().material = mat;
        cities[new Vector2Int(0 + offset, t1c)] = t1;
        

        t2.setBiom(BIOM.Town);
        t2.setSubBiom(SUBBIOM.Harbor);  
        t2.getTile().GetComponent<MeshRenderer>().material = mat;
        cities[new Vector2Int(0 + offset * 2, t2c)] = t2;

        t3.setBiom(BIOM.Town);
        t3.setSubBiom(SUBBIOM.Harbor);
        t3.getTile().GetComponent<MeshRenderer>().material = mat;
        cities[new Vector2Int(0 + offset * 3, t3c)] = t3;
    }
    private void makeCityStreets(Dictionary<Vector2Int,MapTile> cities) {
       const int minsteps = 6;//steps bevore road can turn in weak wrong direction 
       //  int maxCity = Mathf.RoundToInt( Mathf.Pow( (float) (width + height) / 2 , 1 / (float)2) );
        List<Vector2Int> roadsToCheck = new List<Vector2Int>();
        Dictionary<Vector2Int, List<Vector2Int>> cityConnectsToRoad = new Dictionary<Vector2Int, List<Vector2Int>>();
            


        Material road = Resources.Load("Materials/Map/Road", typeof(Material)) as Material;
        Material roadC = Resources.Load("Materials/Map/RoadCross", typeof(Material)) as Material;
        Material roadT = Resources.Load("Materials/Map/RoadT", typeof(Material)) as Material;
        Material roadD = Resources.Load("Materials/Map/RoadDiagonal", typeof(Material)) as Material;

        const int streetsPerCity = 1;
        const int weightWrong = 5;  
       
        Random.InitState(this.seed);
        Dictionary<Vector2Int, Vector2Int> fromToCities = new Dictionary<Vector2Int, Vector2Int>();
        List<Vector2Int> keyList = new List<Vector2Int>(cities.Keys);
        // int streetCounter = 0;
        //Debug.Log(maxCity);
        foreach (KeyValuePair < Vector2Int, MapTile> city in cities){ //jetzt durch while ersetzt
       // while(streetCounter < maxCity) { 
            for (int i=0; i < streetsPerCity; i++)
            {
                // int key = Random.Range(0, keyList.Count);
                //MapTile city = cities[keyList[key]];
                MapTile cityStart = city.Value;
                Vector2Int cityStartPos = city.Key;
                Vector2Int currentPos = city.Key;//keyList[key];


                MapTile cityEnd = cities[keyList[Random.Range(0, keyList.Count)]];
                Vector3 cityEndVec = cityEnd.getTile().transform.position;
                Vector2Int cityEndPos = new Vector2Int((int)cityEndVec.x, (int)cityEndVec.z);
               // Debug.Log("cityEnd: "+ cityEndPos);
                Vector2Int LastPosition = new Vector2Int(-1,-1);
                int steps = 0;
                bool stop=false;
                // bool stopNow = false;
                // bool nextToRoad = false;
                if ((currentPos == cityEndPos)||
                    (fromToCities.ContainsKey(city.Key) && fromToCities[city.Key] == cityEndPos) ||
                    (fromToCities.ContainsKey(cityEndPos) && fromToCities[cityEndPos] == city.Key) ){
                        continue;
                }

                // streetCounter++;
                fromToCities[city.Key] = cityEndPos;
                fromToCities[cityEndPos] = city.Key;
                // Debug.Log("start = " + city.Key + " End = " + cityEndPos);
                List <Vector2Int> thisRoad = new List<Vector2Int>();
                while (currentPos != cityEndPos) {

                    if(!cityConnectsToRoad.ContainsKey(cityStartPos))
                    cityConnectsToRoad[cityStartPos] = new List<Vector2Int>();
                    if (!cityConnectsToRoad.ContainsKey(cityEndPos))
                        cityConnectsToRoad[cityEndPos] = new List<Vector2Int>();

                    Vector2Int direction = cityEndPos - currentPos;
                    int strongVal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? direction.x : direction.y;
                    int weakVal = Mathf.Abs(direction.x) < Mathf.Abs(direction.y) ? direction.x : direction.y;

                    Vector2Int strongDir = Mathf.Abs(direction.x) > Mathf.Abs(direction.y)?
                        (direction.x<0?Vector2Int.left: Vector2Int.right ):
                        (direction.y<0? Vector2Int.down: Vector2Int.up );
                    Vector2Int weakDir = Mathf.Abs(direction.x) < Mathf.Abs(direction.y) ?
                        (direction.x > 0 ?  Vector2Int.right: Vector2Int.left ) :
                        (direction.y > 0 ?  Vector2Int.up: Vector2Int.down );
                    Vector2Int wrongDir = weakDir*(-1);
                    Vector2Int newPos;
                     int randVar = Random.Range(1, 100);

                    float distToDest = Vector2Int.Distance(currentPos, cityEndPos);
                    // Debug.Log("weak = strong?" + (weakDir == strongDir));
                    if (randVar < weightWrong && currentPos + wrongDir != LastPosition &&
                        steps > minsteps && tiles[currentPos + wrongDir].getBiom() != BIOM.Water &&
                        (currentPos + wrongDir).x < width - 1 && (currentPos + wrongDir).y < height - 1 && (currentPos + wrongDir).x > 0 && (currentPos + wrongDir).y > 0 && distToDest>4)
                    {
                        newPos = currentPos + wrongDir;

                        // Debug.Log("wrong Dir");
                    }
                    else if (randVar < (100 - weightWrong) * (strongVal / ((float)strongVal + weakVal)) &&
                        currentPos + strongDir != LastPosition && tiles[currentPos + strongDir].getBiom() != BIOM.Water &&
                        (currentPos + strongDir).x < width - 1 && (currentPos + strongDir).y < height - 1 && (currentPos + strongDir).x > 0 && (currentPos + strongDir).y > 0 )
                    {
                        newPos = currentPos + strongDir;
                       // Debug.Log("strong Dir");
                    }
                    else if (currentPos + weakDir != LastPosition && steps > minsteps && tiles[currentPos + weakDir].getBiom() != BIOM.Water &&
                        (currentPos + weakDir).x < width - 1 && (currentPos + weakDir).y < height - 1 && (currentPos + weakDir).x > 0 && (currentPos + weakDir).y > 0 )
                    {
                        newPos = currentPos + weakDir;
                       // Debug.Log("weak Dir");
                    } // try all dir for possible solution
                    else if (currentPos + wrongDir != LastPosition && tiles[currentPos + wrongDir].getBiom() != BIOM.Water &&
                        (currentPos + wrongDir).x < width - 1 && (currentPos + wrongDir).y < height - 1 && (currentPos + wrongDir).x > 0 && (currentPos + wrongDir).y > 0 )
                    {
                        newPos = currentPos + wrongDir;
                        //Debug.Log("wrong Dir");
                    }

                    else if (currentPos + weakDir != LastPosition && tiles[currentPos + weakDir].getBiom() != BIOM.Water &&
                        (currentPos + weakDir).x < width - 1 && (currentPos + weakDir).y < height - 1 && (currentPos + weakDir).x > 0 && (currentPos + weakDir).y > 0 )
                    {
                        newPos = currentPos + weakDir;
                        //Debug.Log("weak Dir");
                    }

                    else if (currentPos + strongDir != LastPosition && tiles[currentPos + strongDir].getBiom() != BIOM.Water &&
                        (currentPos + strongDir).x < width - 1 && (currentPos + strongDir).y < height - 1 && (currentPos + strongDir).x > 0 && (currentPos + strongDir).y > 0)
                    {
                        newPos = currentPos + strongDir;
                        //Debug.Log("strong Dir");
                    }//--- 
                    else if (currentPos + strongDir != LastPosition &&
                        (currentPos + strongDir).x < width - 1 && (currentPos + strongDir).y < height - 1 && (currentPos + strongDir).x > 0 && (currentPos + strongDir).y > 0 )
                    {
                        newPos = currentPos + strongDir;
                        //Debug.Log("strong Dir");
                    }
                    else if (currentPos + weakDir != LastPosition &&
                        (currentPos + weakDir).x < width - 1 && (currentPos + weakDir).y < height - 1 && (currentPos + weakDir).x > 0 && (currentPos + weakDir).y > 0)
                    {
                        newPos = currentPos + weakDir;
                        //Debug.Log("weak Dir");
                    }
                    else if (currentPos + wrongDir != LastPosition &&
                        (currentPos + wrongDir).x < width - 1 && (currentPos + wrongDir).y < height - 1 && (currentPos + wrongDir).x > 0 && (currentPos + wrongDir).y > 0)
                    {
                        newPos = currentPos + wrongDir;
                        //Debug.Log("wrong Dir");
                    }             
                    /*else if (currentPos + strongDir * (-1) != LastPosition)
                    {
                        newPos = currentPos + strongDir * (-1);
                      //  Debug.Log("false Dir");
                    }*/
                    else {
                        Debug.Log("last pos:" + LastPosition.x+","+LastPosition.y);
                        Debug.Log(" currentPos: " + currentPos.x + "," + currentPos.y );
                        Debug.Log(" weak: " + (currentPos + weakDir).x + "," + (currentPos + weakDir).y );
                        Debug.Log(" strong: " + (currentPos + strongDir).x + "," + (currentPos + strongDir).y);
                        Debug.Log(" wrong: " + (currentPos + wrongDir).x + "," + (currentPos + wrongDir).y );
                        throw new System.Exception("out of options");

                    }


                    //  Vector2Int newDir = newPos - currentPos;
                    if (

                        ((cityConnectsToRoad[cityStartPos].Contains(currentPos + Vector2Int.up) ||
                        cityConnectsToRoad[cityEndPos].Contains(currentPos + Vector2Int.up)) &&
                        !(thisRoad.Contains(currentPos + Vector2Int.up)))
                        ) {
                        stop = true;
                        newPos = currentPos + Vector2Int.up;
                        // Debug.Log(" stopt at" + newPos);
                    }


                    if((cityConnectsToRoad[cityStartPos].Contains(currentPos + Vector2Int.down) ||
                        cityConnectsToRoad[cityEndPos].Contains(currentPos + Vector2Int.down) )&&
                        !(thisRoad.Contains(currentPos + Vector2Int.down)))
                    {
                        stop = true;
                        newPos = currentPos + Vector2Int.down;
                        // Debug.Log(" stopt at" + newPos);
                    }

                    if ((cityConnectsToRoad[cityStartPos].Contains(currentPos + Vector2Int.left) ||
                        cityConnectsToRoad[cityEndPos].Contains(currentPos + Vector2Int.left)) &&
                        !(thisRoad.Contains(currentPos + Vector2Int.left))) 
                        {
                            stop = true;
                            newPos = currentPos + Vector2Int.left;
                        // Debug.Log(" stopt at" + newPos);
                    }
                    

                        if((cityConnectsToRoad[cityStartPos].Contains(currentPos + Vector2Int.right) ||
                        cityConnectsToRoad[cityEndPos].Contains(currentPos + Vector2Int.right)) &&
                        !(thisRoad.Contains(currentPos + Vector2Int.right)))
                    {
                        stop = true;
                        newPos = currentPos + Vector2Int.right;
                        // Debug.Log(" stopt at" + newPos);
                    }
                        
                   
                    thisRoad.Add(newPos);
                    cityConnectsToRoad[cityStartPos].Add(newPos);
                    cityConnectsToRoad[cityEndPos].Add(newPos);
                   

                    
                      tiles[newPos].setSubBiom(SUBBIOM.Street);
                    // Debug.Log("pos street: " + newPos.x + "," + newPos.y +" -> "+tiles[newPos].getSubBiom());
                   
                       

                    Vector2Int aRoadPos = makeRoad( currentPos,  road,  roadC,  roadD,  roadT);

                   

                    if (aRoadPos != new Vector2Int(-1, -1))
                        roadsToCheck.Add(aRoadPos);

                    if (stop) {

                        aRoadPos = makeRoad(newPos, road, roadC, roadD, roadT);
                        if (aRoadPos != new Vector2Int(-1, -1))
                            roadsToCheck.Add(aRoadPos);
                        break;
                    }
                    //check old possition



                    LastPosition = currentPos;
                    currentPos = newPos;
                    steps++;
                    
                }
                
               // Debug.Log("start " + cityStartPos + " :");

                foreach (Vector2Int v in cityConnectsToRoad[cityStartPos]) {
                //    Debug.Log("V.x=" + v.x + "V.y" + v.y);
                }


                //Debug.Log("end " + cityEndPos + " :" );
                foreach (Vector2Int v in cityConnectsToRoad[cityEndPos])
                {
                  //  Debug.Log("V.x=" + v.x + "V.y" + v.y);
                }
            }
          
        }
       

        foreach ( Vector2Int r in roadsToCheck) {
      
            
            Vector2Int currentPos = r;
            //Debug.Log("Checked " + r.x + " " + r.y);


            //get surroundings 
            MapTile tileUpCurrent;
            tiles.TryGetValue(currentPos + Vector2Int.up, out tileUpCurrent);
            MapTile tileDownCurrent;
            tiles.TryGetValue(currentPos + Vector2Int.down, out tileDownCurrent);
            MapTile tileLeftCurrent;
            tiles.TryGetValue(currentPos + Vector2Int.left, out tileLeftCurrent);
            MapTile tileRightCurrent;
            tiles.TryGetValue(currentPos + Vector2Int.right, out tileRightCurrent);
        
            if (tileUpCurrent != null && tileUpCurrent.getSubBiom() == SUBBIOM.Street) {
              //  Debug.Log((currentPos + Vector2Int.up).x + " - " + (currentPos + Vector2Int.up).y+"subbiom = "+ tileUpCurrent.getSubBiom());
                makeRoad( currentPos + Vector2Int.up, road, roadC, roadD, roadT);
            }


            if (tileDownCurrent != null  && tileDownCurrent.getSubBiom() == SUBBIOM.Street ) {
               // Debug.Log((currentPos + Vector2Int.down).x + " - " + (currentPos + Vector2Int.down).y + "..subbiom = " + tileDownCurrent.getSubBiom());
                makeRoad( currentPos + Vector2Int.down, road, roadC, roadD, roadT);
            }


            if (tileLeftCurrent != null && tileLeftCurrent.getSubBiom() == SUBBIOM.Street) {
                makeRoad( currentPos + Vector2Int.left, road, roadC, roadD, roadT);
            }


            if (tileRightCurrent != null &&  tileRightCurrent.getSubBiom() == SUBBIOM.Street ) {
                makeRoad( currentPos + Vector2Int.right, road, roadC, roadD, roadT);
            }
            

        }
        //check and redraw roads

    }
    private Vector2Int makeRoad(Vector2Int currentPos,Material road, Material roadC, Material roadD, Material roadT) {

        //**check new pos**//
        MapTile tileUp;
        tiles.TryGetValue(currentPos + Vector2Int.up, out tileUp);
        MapTile tileDown;
        tiles.TryGetValue(currentPos + Vector2Int.down, out tileDown);
        MapTile tileLeft;
        tiles.TryGetValue(currentPos + Vector2Int.left, out tileLeft);
        MapTile tileRight;
        tiles.TryGetValue(currentPos + Vector2Int.right, out tileRight);
      

        
       
        GameObject currentTile = tiles[currentPos].getTile();
    


        bool up = tileUp != null && (tileUp.getBiom() == BIOM.Town || tileUp.getSubBiom() == SUBBIOM.Street);
        bool down = tileDown != null && (tileDown.getBiom() == BIOM.Town || tileDown.getSubBiom() == SUBBIOM.Street);
        bool left = tileLeft != null && (tileLeft.getBiom() == BIOM.Town || tileLeft.getSubBiom() == SUBBIOM.Street);
        bool right = tileRight != null && (tileRight.getBiom() == BIOM.Town || tileRight.getSubBiom() == SUBBIOM.Street);

        int count = 0;
        count += up ? 1 : 0;
        count += down ? 1 : 0;
        count += left ? 1 : 0;
        count += right ? 1 : 0;
        Vector2Int ret = new  Vector2Int(-1, -1);

        currentTile.transform.rotation = Quaternion.identity;
        if (tiles[currentPos].getBiom() != BIOM.Town)
            switch (count)
            {
                case 2://normal Road

                    if (up && down)// turn road
                    {
                        currentTile.GetComponent<MeshRenderer>().material = road;
                        currentTile.transform.Rotate(Vector3.up * 90);
                    }
                    else if (left && right)
                    {
                        currentTile.GetComponent<MeshRenderer>().material = road;
                    }
                    else
                    {
                        currentTile.GetComponent<MeshRenderer>().material = roadD;

                        if (up && right)
                        {
                            currentTile.transform.Rotate(Vector3.up * 270);
                        }
                        else if (left && up)
                        {
                            currentTile.transform.Rotate(Vector3.up * 180);
                        }
                        else if (down && left)
                        {
                            currentTile.transform.Rotate(Vector3.up * 90);
                        }
                        else if (right && down)
                        {

                        }
                    }

                   // Debug.Log("Road");
                    break;
                case 3://t
                    ret = currentPos;
                    currentTile.GetComponent<MeshRenderer>().material = roadT; //when left right and down 
                    if (up && right && down)
                    {
                        currentTile.transform.Rotate(Vector3.up * 90);
                       
                    }
                    else if (left && down && right)
                    {
                        currentTile.transform.Rotate(Vector3.up * 180);
                      
                    }
                    else if (up && left && down)
                    {
                        currentTile.transform.Rotate(Vector3.up * 270);
                       
                    }
                   // Debug.Log("t");
                    break;

                case 4://kreuzung
                    currentTile.GetComponent<MeshRenderer>().material = roadC; //when left right and down 
                    ret = currentPos;
                   // Debug.Log("c");
                    break;
                default:
                    Debug.Log(count);
                    Debug.Log(currentPos.x + " <-> " + currentPos.y);
                    throw new System.Exception("something wong");
            }
     

        return ret;
    }
    private void makeShoreLine(bool shoreNord=false,bool shoreEast=false,bool shoreSouth= false ,bool shoreWest=false) {
    


        if (shoreSouth && !shoreNord && !shoreEast &&  !shoreWest) {
            int x = 0;
            int z = 0;

            MapTile positon = getTile(new Vector2Int(x, z));
            while (positon.getBiom() == BIOM.Water) {
                positon = getTile(new Vector2Int(x, ++z));
                //Debug.Log(positon.getBiom());

            }
            //Debug.Log("troll");
            
        }

    }
    private void getShore(MapTile position,int x,int y) {
    /*    last
        Vector2int orientation = Vector2Int(Vector2.down);
        position();*/
    }

	public void printAllBiomAndSubBiom()
    {
        foreach (KeyValuePair<Vector2Int, MapTile> t in tiles) {
            Debug.Log(t.Key);
            Debug.Log("BIOM:" + t.Value.getBiom());
            Debug.Log("SubBIOM:" + t.Value.getSubBiom());
        }
    }
}
