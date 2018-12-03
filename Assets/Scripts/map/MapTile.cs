using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile {


    private BIOM biom;
    private SUBBIOM subBiom;
    private GameObject tile;

    public MapTile(GameObject tile,BIOM biom, SUBBIOM subBiom ) {
        if ((int)biom != (int)subBiom / 10) {
           throw new System.Exception("Invalid Subbiom '"+subBiom+"' for biom '"+biom+"'");
        }
        this.biom = biom;
        this.subBiom = subBiom;
        this.tile = tile;
       // Debug.Log(biom);
        //Debug.Log(subBiom);
    }
    public GameObject getTile() {
        return tile;
    }
    public void setSubBiom(SUBBIOM sub) {
        this.subBiom = sub;
    }
    public void setBiom(BIOM bio)
    {
        this.biom = bio;
    }
    public int getSpeed() {
        if (getSubBiom() == SUBBIOM.Street)
            return 160;
        if (getBiom() == BIOM.Grassland)
            return 100;
        if (getBiom() == BIOM.Mountain)
            return 20;
        if (getBiom() == BIOM.Woods)
            return 60;
        if(getBiom() == BIOM.Farm)
            return 80;
        Debug.Log("getSpeed called on not defined biom!");
        return 100;
    }
    public BIOM getBiom()
    {
        return biom;
    }
    public SUBBIOM getSubBiom() {
        return subBiom;
    }

}
