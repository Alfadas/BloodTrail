using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class CombatAIBrain
{
    COMBAT_BEHAVIOUR combatBehaviour;
    Dictionary<Character, CombatAiEnemyBrain> combatAiEnemyBrains;

    public CombatAIBrain(COMBAT_BEHAVIOUR combatBehaviour, List<Character> characters)
    {
        this.combatBehaviour = combatBehaviour;
        foreach (Character character in characters)
        {
            CombatAiEnemyBrain newCombatAiEnemyBrain = new CombatAiEnemyBrain(character);
            combatAiEnemyBrains.Add(character, newCombatAiEnemyBrain);
        }
    }

    public COMBAT_BEHAVIOUR getBehaviour()
    {
        return combatBehaviour;
    }

    public void DeleteEnemy(Character character)
    {
        CombatAiEnemyBrain combatAiEnemyBrain;
        combatAiEnemyBrains.TryGetValue(character, out combatAiEnemyBrain);
        combatAiEnemyBrains.Remove(character);
    }
}

