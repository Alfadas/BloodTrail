using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile {


    private BIOM biom;
    private SUBBIOM subBiom;

    public MapTile(GameObject tile,BIOM biom, SUBBIOM subBiom ) {
        if ((int)biom != (int)subBiom / 10) {
           throw new System.Exception("Invalid Subbiom '"+subBiom+"' for biom '"+biom+"'");
        }
        this.biom = biom;
        this.subBiom = subBiom;
       // Debug.Log(biom);
        //Debug.Log(subBiom);
    }

    public BIOM getBiom()
    {
        return biom;
    }
    public SUBBIOM getSubBiom() {
        return subBiom;
    }
}
