using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeSelected : MonoBehaviour {

     CombatManager combatManager;

    Character character;

    private void Start()
    {
        combatManager = FindObjectOfType<CombatManager>();
        character = gameObject.GetComponent<Character>();
    }

    private void OnMouseDown()
    {
        if (!combatManager.aiTurn)
        {
            combatManager.SetSelected(character, true);
        }
    }
}
