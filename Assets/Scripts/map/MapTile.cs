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
    public BIOM getBiom()
    {
        return biom;
    }
    public SUBBIOM getSubBiom() {
        return subBiom;
    }

}
