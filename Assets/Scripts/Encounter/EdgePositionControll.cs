using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgePositionControll : MonoBehaviour {
    [SerializeField] bool isEdge = true;
    BuildEncounter buildEncounter;
    GameObject encounter;

    int maxTrys = 20;
    int buildNumber = 0;
    public int placeTry = 0;

    private void OnTriggerStay(Collider other)
    {
        if (isEdge && other.tag != "Plague") // Bitte nachbessern
        {
            encounter = GameObject.FindGameObjectWithTag("EncounterController");
            buildEncounter = encounter.GetComponent<BuildEncounter>();
            EdgePositionControll otherEdgePositionControll = other.gameObject.GetComponent<EdgePositionControll>();
            if (otherEdgePositionControll.GetBuildNumber() < buildNumber)
            {
                if (placeTry >= maxTrys)
                {
                    gameObject.transform.localPosition = Vector3.zero;
                }
                else
                {
                    if (gameObject.transform.position.x < 0)
                    {
                        buildEncounter.RollEdgePosition("l", gameObject);
                    }
                    else
                    {
                        buildEncounter.RollEdgePosition("r", gameObject);
                    }
                    placeTry++;
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
