using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatAIController : MonoBehaviour
{
    [SerializeField] CombatActions combatActions;
    [SerializeField] CombatManager combatManager;

    readonly int smallChange = 10;
    readonly int midChange = 25;
    readonly int bigChange = 50;

    int pcLowHealth = 30;

    int attack;
    int defend;
    int buff;

    Dictionary<Character, CombatAiPcMemory> combatAiPcMemorys;

    public void InitializeAI(List<NpcCharacter> enemies, List<Character> playerCharacters)
    {
        List<Character> done = new List<Character>();
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
        foreach (NpcCharacter npcCharacter in enemies)
        {
            combatActions.BuildActionButtonList(npcCharacter, false);
            if (npcCharacter.DeffensiveCombatActions.Contains(COMBAT_ACTION.ProvokeAndCounter) || npcCharacter.DeffensiveCombatActions.Contains(COMBAT_ACTION.ProvokeAndDefend))
            {
                npcCharacter.CanProvoke = true;
            }
            else
            {
                npcCharacter.CanProvoke = false;
            }
        }
        List<NpcCharacter> sortedOffensiveList = enemies.OrderByDescending(o => o.getStat(Character.STAT_AGILITY) + o.getStat(Character.STAT_STRENGTH)).ToList();
        List<NpcCharacter> sortedDefesiveList = enemies.OrderByDescending(o => o.getStat(Character.STAT_ENDURANCE) + o.getStat(Character.STAT_CHARISMA)).ToList();
        List<NpcCharacter> sortedSupportList = enemies.OrderByDescending(o => o.getStat(Character.STAT_INTELLIGENCE) + o.getStat(Character.STAT_CHARISMA)).ToList();

        for (int i = 0; i < buff; i++)
        {
            if (sortedSupportList[i].SupportCombatActions.Count() > 0)
            {
                sortedSupportList[i].CombatBehaviour = COMBAT_BEHAVIOUR.Support;
                done.Add(sortedSupportList[i]);
            }
            else
            {
                int remaining = buff - (i + 1);
                for (int j = 0; j < remaining; j++)
                {
                    int roll = Random.Range(1, 3);
                    if (roll == 1)
                    {
                        attack++;
                    }
                    else if (roll == 2)
                    {
                        defend++;
                    }
                }
                break;
            }
        }
        for (int i = 0; i < defend; i++)
        {
            if (!done.Contains(sortedDefesiveList[i]))
            {
                if (sortedDefesiveList[i].getStat(Character.STAT_ENDURANCE) >= CombatActions.EASY_STAT_MIN)
                {
                    sortedDefesiveList[i].CombatBehaviour = COMBAT_BEHAVIOUR.Defend;
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
                defend++;
            }
        }
        for (int i = 0; i < attack; i++)
        {
            if (!done.Contains(sortedOffensiveList[i]))
            {
                sortedOffensiveList[i].CombatBehaviour = COMBAT_BEHAVIOUR.Attack;
                done.Add(sortedOffensiveList[i]);
            }
            else
            {
                attack++;
            }
        }

        combatAiPcMemorys = new Dictionary<Character, CombatAiPcMemory>();
        foreach (Character character in playerCharacters)
        {
            CombatAiPcMemory newCombatAiEnemyBrain = new CombatAiPcMemory(character);
            combatAiPcMemorys.Add(character, newCombatAiEnemyBrain);
        }
    }

    public void AITurn(NpcCharacter currentCharacter, List<Character> playerCharacters, List<NpcCharacter> npcCharacters, List<Character> PcProvoking)
    {
        COMBAT_BEHAVIOUR turnBehaviour = GenerateTurnBehaviour(currentCharacter, playerCharacters, npcCharacters, PcProvoking);
        bool provoked = false;
        int gesAttackValue = 0;
        Character selectedCharacter = playerCharacters[0];
        COMBAT_ACTION action = COMBAT_ACTION.SimpleAttack;
        if (turnBehaviour == COMBAT_BEHAVIOUR.Attack)
        {
            SelectTarget(currentCharacter, playerCharacters, ref provoked, ref gesAttackValue, ref selectedCharacter);
            action = SelectAttack(currentCharacter, playerCharacters, selectedCharacter);
            combatManager.SetSelected(selectedCharacter, false);
        }
        else
        {
            if (turnBehaviour == COMBAT_BEHAVIOUR.Defend)
            {
                action = GetDefendAction(currentCharacter, playerCharacters);
            }
            else if (turnBehaviour == COMBAT_BEHAVIOUR.Provoke)
            {
                action = GetProvokeAction(currentCharacter, playerCharacters);
            }
            else
            {
                action = GetSupportAction(currentCharacter, playerCharacters, npcCharacters);
            }
        }
        combatActions.EnumToAction(action);
    }

    private COMBAT_ACTION GetSupportAction(NpcCharacter currentCharacter, List<Character> playerCharacters, List<NpcCharacter> npcCharacters)
    {
        COMBAT_ACTION action;
        List<COMBAT_ACTION> selectableActions = currentCharacter.SupportCombatActions;
        if (selectableActions.Contains(COMBAT_ACTION.Distract))
        {
            foreach (Character character in playerCharacters)
            {
                if (GetCombatAiPcMemory(character).GetMaxStat(Character.STAT_INTELLIGENCE) < currentCharacter.getStat(Character.STAT_INTELLIGENCE))
                {
                    selectableActions.Add(COMBAT_ACTION.Distract);
                }
            }
        }
        if (selectableActions.Contains(COMBAT_ACTION.Encourage))
        {
            foreach (NpcCharacter character in npcCharacters)
            {
                if (character.getStat(Character.STAT_STRENGTH) > CombatActions.MEDIUM_STAT_MIN)
                {
                    selectableActions.Add(COMBAT_ACTION.Encourage);
                }
            }
        }
        int newRoll = Random.Range(0, selectableActions.Count());
        action = selectableActions[newRoll];
        return action;
    }

    private COMBAT_ACTION GetProvokeAction(NpcCharacter currentCharacter, List<Character> playerCharacters)
    {
        COMBAT_ACTION action;
        List<COMBAT_ACTION> selectableActions = new List<COMBAT_ACTION>();
        if (currentCharacter.DeffensiveCombatActions.Contains(COMBAT_ACTION.ProvokeAndCounter))
        {
            selectableActions.Add(COMBAT_ACTION.ProvokeAndCounter);
            foreach (Character character in playerCharacters)
            {
                if (GetCombatAiPcMemory(character).GetMaxStat(Character.STAT_AGILITY) < currentCharacter.getStat(Character.STAT_AGILITY))
                {
                    selectableActions.Add(COMBAT_ACTION.ProvokeAndCounter);
                }
            }
        }
        if (currentCharacter.DeffensiveCombatActions.Contains(COMBAT_ACTION.ProvokeAndDefend))
        {
            selectableActions.Add(COMBAT_ACTION.ProvokeAndDefend);
        }
        int newRoll = Random.Range(0, selectableActions.Count());
        action = selectableActions[newRoll];
        return action;
    }

    private COMBAT_ACTION GetDefendAction(NpcCharacter currentCharacter, List<Character> playerCharacters)
    {
        COMBAT_ACTION action;
        List<COMBAT_ACTION> selectableActions = currentCharacter.DeffensiveCombatActions;
        if (selectableActions.Contains(COMBAT_ACTION.ProvokeAndCounter))
        {
            selectableActions.Remove(COMBAT_ACTION.ProvokeAndCounter);
        }
        if (selectableActions.Contains(COMBAT_ACTION.ProvokeAndDefend))
        {
            selectableActions.Remove(COMBAT_ACTION.ProvokeAndDefend);
        }
        if (selectableActions.Contains(COMBAT_ACTION.CounterAttack))
        {
            foreach (Character character in playerCharacters)
            {
                if (GetCombatAiPcMemory(character).GetMaxStat(Character.STAT_AGILITY) < currentCharacter.getStat(Character.STAT_AGILITY))
                {
                    selectableActions.Add(COMBAT_ACTION.CounterAttack);
                }
            }
        }
        if (selectableActions.Contains(COMBAT_ACTION.Dodge))
        {
            foreach (Character character in playerCharacters)
            {
                if (GetCombatAiPcMemory(character).GetMaxStat(Character.STAT_AGILITY) < currentCharacter.getStat(Character.STAT_AGILITY))
                {
                    selectableActions.Add(COMBAT_ACTION.Dodge);
                }
            }
        }
        if (selectableActions.Contains(COMBAT_ACTION.DefensiveStance))
        {
            selectableActions.Remove(COMBAT_ACTION.TryToDefend);
        }
        int newRoll = Random.Range(0, selectableActions.Count());
        action = selectableActions[newRoll];
        return action;
    }

    private COMBAT_ACTION SelectAttack(NpcCharacter currentCharacter, List<Character> playerCharacters, Character selectedCharacter)
    {
        COMBAT_ACTION action;
        CombatAiPcMemory currentCombatAiPcMemory = GetCombatAiPcMemory(selectedCharacter);
        List<COMBAT_ACTION> selectableActions = currentCharacter.OffensiveCombatActions;
        if (currentCombatAiPcMemory.IsSupporting)
        {
            if (currentCombatAiPcMemory.WasCountering || currentCombatAiPcMemory.WasDodgeing && selectableActions.Contains(COMBAT_ACTION.SwiftAttack))
            {
                selectableActions.Add(COMBAT_ACTION.SwiftAttack);
            }
            if (selectableActions.Contains(COMBAT_ACTION.HeavyAttack))
            {
                if (currentCombatAiPcMemory.IsCountering)
                {
                    selectableActions.Remove(COMBAT_ACTION.HeavyAttack);
                }
                else if (currentCombatAiPcMemory.WasCountering)
                {
                    int roll = Random.Range(0, 100);
                    if (roll < 50)
                    {
                        selectableActions.Remove(COMBAT_ACTION.HeavyAttack);
                    }
                }
            }
        }
        if (selectedCharacter.IsProvoking())
        {
            int roll = Random.Range(0, 10);
            if (roll < 5 && selectableActions.Contains(COMBAT_ACTION.HeavyAttack))
            {
                selectableActions.Remove(COMBAT_ACTION.HeavyAttack);
            }
            roll = Random.Range(0, 10);
            if (roll < 5 && selectableActions.Contains(COMBAT_ACTION.SwiftAttack))
            {
                selectableActions.Add(COMBAT_ACTION.SwiftAttack);
            }
        }
        if (selectableActions.Contains(COMBAT_ACTION.WeakpointAttack))
        {
            foreach (Character character in playerCharacters)
            {
                if (GetCombatAiPcMemory(character).IsDistracting)
                {
                    selectableActions.Add(COMBAT_ACTION.WeakpointAttack);
                }
            }
        }
        if (selectableActions.Contains(COMBAT_ACTION.HeavyAttack) && !selectedCharacter.IsProvoking() && !currentCombatAiPcMemory.IsSupporting)
        {
            selectableActions.Add(COMBAT_ACTION.HeavyAttack);
        }

        int newRoll = Random.Range(0, selectableActions.Count());
        action = selectableActions[newRoll];
        return action;
    }

    private void SelectTarget(NpcCharacter currentCharacter, List<Character> playerCharacters, ref bool provoked, ref int gesAttackValue, ref Character selectedCharacter)
    {
        List<Character> CanAttack = new List<Character>();
        Dictionary<Character, int> CanAttackValue = new Dictionary<Character, int>();
        foreach (Character character in playerCharacters)
        {
            CombatAiPcMemory currentCombatAiPcMemory = GetCombatAiPcMemory(character);
            int attackValue = 0;
            if (character.IsProvoking())
            {
                if (currentCombatAiPcMemory.GetMinStat(Character.STAT_CHARISMA) > currentCharacter.getStat(Character.STAT_INTELLIGENCE))
                {
                    selectedCharacter = character;
                    provoked = true;
                    break;
                }
                attackValue -= bigChange;
            }
            if (currentCombatAiPcMemory.IsSupporting)
            {
                if (currentCombatAiPcMemory.Supportknown)
                {
                    if ((currentCombatAiPcMemory.IsCountering || currentCombatAiPcMemory.IsDodgeing)
                        && !currentCharacter.OffensiveCombatActions.Contains(COMBAT_ACTION.SwiftAttack)
                        && currentCombatAiPcMemory.GetMinStat(Character.STAT_AGILITY) > currentCharacter.getStat(Character.STAT_AGILITY))
                    {
                        continue;
                    }
                    else if (currentCombatAiPcMemory.IsDistracting)
                    {
                        attackValue += bigChange;
                    }
                    else
                    {
                        attackValue += midChange;
                    }

                }
                else if (currentCombatAiPcMemory.WasCountering || currentCombatAiPcMemory.WasDodgeing)
                {
                    if (!currentCharacter.OffensiveCombatActions.Contains(COMBAT_ACTION.SwiftAttack)
                        && currentCombatAiPcMemory.GetMinStat(Character.STAT_AGILITY) > currentCharacter.getStat(Character.STAT_AGILITY))
                    {
                        continue;
                    }
                    else
                    {
                        attackValue += midChange;
                    }
                }
                else
                {
                    attackValue += bigChange;
                }
            }
            else
            {
                attackValue += smallChange;
            }

            if (character.getHealth() < pcLowHealth)
            {
                attackValue += bigChange;
            }

            if (!character.IsProvoking() && character.GetDamageReduction() > 0 && !currentCharacter.OffensiveCombatActions.Contains(COMBAT_ACTION.WeakpointAttack))
            {
                attackValue -= midChange;
            }
            else
            {
                attackValue += midChange;
            }
            if (attackValue > 0)
            {
                CanAttack.Add(character);
                CanAttackValue.Add(character, attackValue);
                gesAttackValue += attackValue;
            }
        }
        if (!provoked)
        {
            int roll = Random.Range(0, gesAttackValue);
            int gesValue = 0;
            foreach (Character character in CanAttack)
            {
                int value;
                CanAttackValue.TryGetValue(character, out value);
                gesValue += value;
                if (roll < gesValue)
                {
                    selectedCharacter = character;
                    break;
                }
            }
        }
    }

    private COMBAT_BEHAVIOUR GenerateTurnBehaviour(NpcCharacter currentCharacter, List<Character> playerCharacters, List<NpcCharacter> npcCharacters, List<Character> PcProvoking)
    {
        if (npcCharacters.Count > 1)
        {
            COMBAT_BEHAVIOUR aiBehaviour = currentCharacter.CombatBehaviour;
            int supportModi = 0;
            int defendModi = 0;
            int provokeModi = 0;
            int attackModi = 0;

            // behaviour
            if (aiBehaviour == COMBAT_BEHAVIOUR.Support)
            {
                supportModi += bigChange;
            }
            else if (aiBehaviour == COMBAT_BEHAVIOUR.Defend)
            {
                defendModi += midChange;
                provokeModi += bigChange;
            }
            else
            {
                attackModi += bigChange;
            }

            CalcGeneralModi(currentCharacter, playerCharacters, PcProvoking, ref defendModi, ref attackModi);

            // AiGroup
            provokeModi *= npcCharacters.Count();
            supportModi *= npcCharacters.Count();
            foreach (NpcCharacter npc in npcCharacters)
            {
                if (npc != currentCharacter)
                {
                    if (currentCharacter.getHealth() < currentCharacter.getMaxHealth() / 2)
                    {
                        provokeModi += bigChange;
                        supportModi += bigChange;
                    }
                }
            }
            if (currentCharacter.SupportCombatActions.Count() == 0)
            {
                supportModi = 0;
            }
            if (!currentCharacter.CanProvoke)
            {
                provokeModi = 0;
            }

            foreach (Character character in playerCharacters)
            {
                if (character.getHealth() < pcLowHealth &&!GetCombatAiPcMemory(character).IsSupporting)
                {
                    attackModi += midChange;
                }
            }

            return CreateRandomBehaviour(attackModi, defendModi, provokeModi, supportModi);
        }
        else
        {
            int defendModi = 0;
            int attackModi = bigChange;

            CalcGeneralModi(currentCharacter, playerCharacters, PcProvoking, ref defendModi, ref attackModi);

            return CreateRandomBehaviour(attackModi, defendModi);
        }
    }

    private static COMBAT_BEHAVIOUR CreateRandomBehaviour(int attackModi, int defendModi, int provokeModi = 0, int supportModi = 0)
    {
        int gesModi = supportModi + attackModi + defendModi + provokeModi;
        int roll = Random.Range(0, gesModi);
        if (roll < attackModi)
        {
            return COMBAT_BEHAVIOUR.Attack;
        }
        else if (roll < attackModi + defendModi)
        {
            return COMBAT_BEHAVIOUR.Defend;
        }
        else if (roll < attackModi + defendModi + supportModi)
        {
            return COMBAT_BEHAVIOUR.Support;
        }
        else
        {
            return COMBAT_BEHAVIOUR.Provoke;
        }
    }

    private void CalcGeneralModi(NpcCharacter currentCharacter, List<Character> playerCharacters, List<Character> PcProvoking, ref int defendModi, ref int attackModi)
    {
        // health
        if (currentCharacter.getHealth() < currentCharacter.getMaxHealth() / 5)
        {
            defendModi += bigChange;
        }
        else if (currentCharacter.getHealth() < currentCharacter.getMaxHealth() / 2)
        {
            defendModi += midChange;
        }
        else if (currentCharacter.getHealth() == currentCharacter.getMaxHealth())
        {
            defendModi -= midChange;
        }
        else
        {
            defendModi -= smallChange;
        }

        // Provoking
        if (PcProvoking.Count > 0)
        {
            int couldProvoke = 0;
            int willProvoke = 0;
            foreach (Character provoking in PcProvoking)
            {
                if (GetCombatAiPcMemory(provoking).GetMinStat(Character.STAT_CHARISMA) > currentCharacter.getStat(Character.STAT_INTELLIGENCE))
                {
                    attackModi -= midChange;
                }
                else if (GetCombatAiPcMemory(provoking).GetMaxStat(Character.STAT_CHARISMA) > currentCharacter.getStat(Character.STAT_INTELLIGENCE))
                {
                    attackModi -= smallChange;
                }
            }
            if (couldProvoke == 0 && willProvoke == 0)
            {
                attackModi += smallChange;
            }
        }

        // PcGroup
        foreach (Character character in playerCharacters)
        {
            if (character.getHealth() < pcLowHealth)
            {
                attackModi += bigChange;
            }
            if (GetCombatAiPcMemory(character).IsDistracting)
            {
                attackModi -= smallChange;
                if (GetCombatAiPcMemory(character).GetMinStat(Character.STAT_INTELLIGENCE) > currentCharacter.getStat(Character.STAT_INTELLIGENCE))
                {
                    attackModi -= midChange;
                }
            }
        }
    }

    public CombatAiPcMemory GetCombatAiPcMemory(Character character)
    {
        CombatAiPcMemory combatAiPcMemory;
        if(!combatAiPcMemorys.TryGetValue(character, out combatAiPcMemory))
        {
            Debug.LogError("No character " + character + " in combatAiEnemyBrains");
        }
        return combatAiPcMemory;
    }
    public void RemoveCombatAiPcMemory(Character playerCharacter)
    {
        combatAiPcMemorys.Remove(playerCharacter);
    }
}
