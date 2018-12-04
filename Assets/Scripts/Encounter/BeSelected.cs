using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeSelected : MonoBehaviour {

    [SerializeField] CombatManager combatManager;

    Character character;

    private void Start()
    {
        character = gameObject.GetComponent<Character>();
    }

    private void OnMouseDown()
    {
        if (!combatManager.aiTurn)
        {
            combatManager.SetSelected(character);
        }
    }
}
