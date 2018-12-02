using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgePositionControll : MonoBehaviour {
    [SerializeField] bool isEdge = true;
    BuildEncounter buildEncounter;
    GameObject encounter;

    int buildNumber = 0;

    private void OnTriggerStay(Collider other)
    {
        if (isEdge)
        {
            encounter = GameObject.FindGameObjectWithTag("EncounterController");
            buildEncounter = encounter.GetComponent<BuildEncounter>();
            EdgePositionControll otherEdgePositionControll = other.gameObject.GetComponent<EdgePositionControll>();
            if (otherEdgePositionControll.GetBuildNumber() < buildNumber)
            {
                if (gameObject.transform.position.x < 0)
                {
                    buildEncounter.RollEdgePosition("l", gameObject);
                }
                else
                {
                    buildEncounter.RollEdgePosition("r", gameObject);
                }
            }
        }
    }

    public int GetBuildNumber()
    {
        return buildNumber;
    }
    public void SetBuildNumber(int number)
    {
        buildNumber = number;
    }
}
