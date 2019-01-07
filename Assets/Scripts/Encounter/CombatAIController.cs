using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAIController : MonoBehaviour
{
    [SerializeField] CombatActions combatActions;

    int attack;
    int defend;
    int buff;

    Dictionary<Character, COMBAT_BEHAVIOUR> enemyBehaviour;

    public void GenerateAIBehaviour(List<Character> enemies)
    {
        enemyBehaviour = new Dictionary<Character, COMBAT_BEHAVIOUR>();
        attack = 1;
        defend = 0;
        buff = 0;
        if (enemies.Count == 2)
        {
            int roll = Random.Range(1, 3);
            if (roll == 1)
            {
                attack++;
            }
            if (roll == 2)
            {
                defend++;
            }
        }
        else if (enemies.Count > 2)
        {
            int roll = Random.Range(1, 4);
            if (roll == 1)
            {
                attack++;
            }
            else if (roll == 2)
            {
                defend++;
            }
            else if (roll == 3)
            {
                buff++;
            }
        }

        foreach (Character enemy in enemies)
        {

        }

    }

    public void AITurn(Character currentCharacter ,int enemyCount, int enemyDistracting, int enemyProvoking)
    {
        
    }
}
