using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        List <Character> done = new List<Character>();
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
        List<Character> sortedOffensiveList = enemies.OrderByDescending(o => o.getStat(Character.STAT_AGILITY)+o.getStat(Character.STAT_STRENGTH)).ToList();
        List<Character> sortedDefesiveList = enemies.OrderByDescending(o => o.getStat(Character.STAT_ENDURANCE) + o.getStat(Character.STAT_CHARISMA)).ToList();
        List<Character> sortedSupportList = enemies.OrderByDescending(o => o.getStat(Character.STAT_INTELLIGENCE) + o.getStat(Character.STAT_CHARISMA)).ToList();

        for (int i = 0; i < buff; i++)
        {
            if (sortedSupportList[i].getStat(Character.STAT_INTELLIGENCE) >= CombatActions.EASY_STAT_MIN)
            {
                enemyBehaviour.Add(sortedSupportList[i], COMBAT_BEHAVIOUR.Buff);
                done.Add(sortedSupportList[i]);
            }
            else
            {
                int remaining = buff - (i + 1);
                for(int j = 0; j < remaining; j++)
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
                break;
            }
        }
        for (int i = 0; i < defend; i++)
        {
            if (done.Contains(sortedDefesiveList[i]))
            {
                if (sortedDefesiveList[i].getStat(Character.STAT_ENDURANCE) >= CombatActions.EASY_STAT_MIN)
                {
                    enemyBehaviour.Add(sortedDefesiveList[i], COMBAT_BEHAVIOUR.Defend);
                    done.Add(sortedDefesiveList[i]);
                }
                else
                {
                    int remaining = buff - (i + 1);
                    attack += remaining;
                    break;
                }
            }
            else
            {
                i--;
            }
        }
        for (int i = 0; i < attack; i++)
        {
            if (done.Contains(sortedOffensiveList[i]))
            {
                enemyBehaviour.Add(sortedOffensiveList[i], COMBAT_BEHAVIOUR.Attack);
                done.Add(sortedOffensiveList[i]);
            }
            else
            {
                i--;
            }
        }
    }

    public void AITurn(Character currentCharacter ,int enemyCount, int enemyDistracting, int enemyProvoking)
    {
        
    }
}
