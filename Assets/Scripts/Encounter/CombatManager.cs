using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour {
    [SerializeField] RollEncounter rollEncounter;
    [SerializeField] BuildEncounter buildEncounter;
    [SerializeField] CharacterManager characterManager;
	[SerializeField] CharacterButtonManager characterButtonManager;
    [SerializeField] CombatButtonManager combatButtonManager;
    [SerializeField] CombatAIController combatAIController;
	[SerializeField] SoundManager soundManager;
    [SerializeField] CombatActions combatActions;
	[SerializeField] GameObject fightTutorial;
    List<NpcCharacter> enemies;
    List<Character> playerGroup;
    List<Character> participants;

    Queue<Character> participantsQueue;
    Character firstCharacter;
    Character currentCharacter;
    Character selectedCharacter;
    [SerializeField] GameObject CombatButtons;

    int turnCounter = 0;
    public bool aiTurn = false;
	private bool tutorialShown = false;
    List<Character> enemiesDistracting;
    List<Character> playerDistracting;
    List<Character> enemiesProvoking;
    List<Character> playerProvoking;

    public void StartFight()
    {
		if(!tutorialShown)
			{
			fightTutorial.SetActive(true);
			tutorialShown = true;
			}

        participantsQueue = new Queue<Character>();
        participants = new List<Character>();
        enemiesDistracting = new List<Character>();
        playerDistracting = new List<Character>();
        enemiesProvoking = new List<Character>();
        playerProvoking = new List<Character>();

        playerGroup = characterManager.getCharacters();
        enemies = buildEncounter.GetEnemies();
        foreach(NpcCharacter npcCharacter in enemies)
        {
            participants.Add(npcCharacter);
        }
        participants.AddRange(playerGroup);

        combatAIController.InitializeAI(enemies, playerGroup);

        participants = participants.OrderByDescending(o => o.getStat(Character.STAT_AGILITY)).ToList();
        foreach (Character participant in participants)
        {
            participantsQueue.Enqueue(participant);
        }
        participants.Clear();
        foreach (Character playerCharacter in playerGroup)
        {
            combatActions.BuildActionButtonList(playerCharacter, true);
        }
        StartCoroutine(WaitForSecToStart(2));
    }

    void NextTurn()
    {
        turnCounter++;
        BattleLog("turn: " + turnCounter.ToString() + " -----------");
		if (enemies.Count == 0 || playerGroup.Count == 0)
        {
            EndFight();
        }
        else
        {
            do
            {
                currentCharacter = participantsQueue.Dequeue();
                if (currentCharacter.GetType() == typeof(NpcCharacter))
                {
                    var npcCharacter = (NpcCharacter)currentCharacter;
                    if (!enemies.Contains(npcCharacter))
                    {
                        currentCharacter = null;
                    }
                }
                else if (!playerGroup.Contains(currentCharacter))
                {
                    currentCharacter = null;
                }
            }
            while (currentCharacter == null);

            if (currentCharacter.IsStuned())
            {
                currentCharacter.SetStuned(false);
                EndTurn();
            }
            else
            {
                if (currentCharacter.GetType() == typeof(NpcCharacter))
                {
                    characterButtonManager.highlightCharacter(null);
                    aiTurn = true;
                    currentCharacter.markCharacter(false);
                    combatAIController.AITurn((NpcCharacter)currentCharacter, playerGroup, enemies, playerProvoking);
                }
                else
                {
                    soundManager.playSFX("turn");
                    characterButtonManager.highlightCharacter(currentCharacter);
                    aiTurn = false;
                    currentCharacter.markCharacter(false);
                    CombatButtons.SetActive(true);
                    List<Button> combatActionButtons = currentCharacter.GetCombatActionButtons();
                    combatButtonManager.ActivateButtons(currentCharacter, combatActionButtons);
                }
            }
        }
    }

    void EndTurn()
    {
		characterButtonManager.highlightCharacter(null);
        currentCharacter.unmarkCharacter();
        if (currentCharacter != null)
        {
            participantsQueue.Enqueue(currentCharacter);
            currentCharacter = null;
        }
        NextTurn();
    }

    public Character GetCurrentCharecter()
    {
        return currentCharacter;
    }

    public Character GetSelectedCharecter()
    {
        return selectedCharacter;
    }

    public void SetSelected(Character selected, bool fromPlayer)
    {
        if (selectedCharacter != null)
        {
            selectedCharacter.unmarkCharacter();
        }
        selectedCharacter = selected;
        if (playerProvoking.Count>0 && !fromPlayer)
        {
            foreach (Character provoking in playerProvoking)
            {
                if (CompareStats(Character.STAT_CHARISMA, Character.STAT_INTELLIGENCE, provoking, currentCharacter))
                {
                    selectedCharacter = provoking;
                }
            }
        }
        else if (enemiesProvoking.Count > 0 && fromPlayer)
        {
            foreach (Character provoking in enemiesProvoking)
            {
                if (CompareStats(Character.STAT_CHARISMA, Character.STAT_INTELLIGENCE, provoking, currentCharacter))
                {
                    selectedCharacter = provoking;
                }
            }
        }
        selectedCharacter.markCharacter(true);
    }

    public void Attack(int damage, float defensModifier, COMBAT_ACTION action)
    {
        float damageReduction = 0;
        BattleLog(action.ToString());
        bool denied = false;
        bool countered = false;
        soundManager.playSFX("attack");
        if (selectedCharacter == null || (playerGroup.Contains(selectedCharacter) && !aiTurn))
        {
            SetSelected(enemies[0],true);
        }

        if (aiTurn)
        {
            damageReduction = ApplyDistract(damageReduction, playerDistracting, currentCharacter);
            if (selectedCharacter.IscounterAttacking())
            {
                CombatAiPcMemory combatAiPcMemory = combatAIController.GetCombatAiPcMemory(selectedCharacter);
                combatAiPcMemory.WasCountering = true;
                combatAiPcMemory.IsCountering = true;
            }
        }
        else if (!aiTurn)
        {
            damageReduction = ApplyDistract(damageReduction, enemiesDistracting, currentCharacter);
        }
        if ((selectedCharacter.IsDodging() || selectedCharacter.IscounterAttacking())&& action != COMBAT_ACTION.SwiftAttack)
        {
            if(CompareStats(Character.STAT_AGILITY, Character.STAT_AGILITY, currentCharacter, selectedCharacter))
            {
                if (selectedCharacter.IscounterAttacking())
                {
                    denied = true;
                    countered = true;
                    BattleLog("Countered");
                }
                else if (action == COMBAT_ACTION.Kick)
                {
                    selectedCharacter.SetStuned(true);
                    BattleLog("Target stuned");
                }
                else
                {
                    denied = true;
                    BattleLog("dodged");
                }
            }
        }

        if (countered)
        {
            if (action == COMBAT_ACTION.HeavyAttack)
            {
                combatActions.CounterAttack(2);
            }
            else
            {
                combatActions.CounterAttack(1);
            }
        }
        else if (!denied)
        {
            damageReduction += selectedCharacter.GetDamageReduction();
            if (action == COMBAT_ACTION.WeakpointAttack)
            {
                damageReduction *= combatActions.GetDamageReductionReductin();
            }
            if (damageReduction > 0.9f)
            {
                damageReduction = 0.9f;
            }
            if (selectedCharacter.GetDamageReduction() > 0) 
            {
                soundManager.playSFX("parry");
                damage -= Mathf.RoundToInt(damage * damageReduction);
                BattleLog("Target defended");
            }
            else
            {
                damage -= Mathf.RoundToInt(damage * damageReduction);
            }
            BattleLog("Target hit with " + damage + " damage");
            if (selectedCharacter.hurt(damage))
            {
                KillCharacter(selectedCharacter);
                selectedCharacter = null;
            }
        }
        else
        {
            Debug.LogWarning("Missing Attack condition");
        }

        if (!countered)
        {
            StartCoroutine(AttackAnimation()); // TODO: move to CombatActions
        }
    }

    private float ApplyDistract(float damageReduction, List<Character> distracting, Character distracted)
    {
        foreach (Character character in distracting)
        {
            if (aiTurn)
            {
                CombatAiPcMemory combatAiPcMemory = combatAIController.GetCombatAiPcMemory(character);
                combatAiPcMemory.WasDistracting = true;
                combatAiPcMemory.IsDistracting = true;
            }
            if (CompareStats(Character.STAT_INTELLIGENCE, Character.STAT_INTELLIGENCE, character, distracted))
            {
                if (CompareStats(Character.STAT_INTELLIGENCE, Character.STAT_INTELLIGENCE, character, distracted, combatActions.GetAdditionalIntToCrit()))
                {
                    damageReduction += combatActions.GetDistractCritSuccessMulti();
                    BattleLog("critDistract");
                }
                else
                {
                    damageReduction += combatActions.GetDistractSuccessMulti();
                    BattleLog("Distract");
                }
            }
            else
            {
                damageReduction += combatActions.GetDistractFailMulti();
                BattleLog("distract Failed");
            }
        }

        return damageReduction;
    }
    private bool CompareStats(int stat1, int stat2, Character character1, Character character2)
    {
        if (character1.getStat(stat1) > character2.getStat(stat2))
        {
            if (character1.GetType() == typeof(NpcCharacter))
            {
                combatAIController.GetCombatAiPcMemory(character2).TrySetMaxStat(stat2, character1.getStat(stat1));
            }
            else
            {
                combatAIController.GetCombatAiPcMemory(character1).TrySetMinStat(stat1, character2.getStat(stat2));
            }
            return true;
        }
        else
        {
            if (character1.GetType() == typeof(NpcCharacter))
            {
                combatAIController.GetCombatAiPcMemory(character2).TrySetMinStat(stat2, character1.getStat(stat1));
            }
            else
            {
                combatAIController.GetCombatAiPcMemory(character1).TrySetMaxStat(stat1, character2.getStat(stat2));
            }
            return false;
        }
    }
    private bool CompareStats(int stat1, int stat2, Character character1, Character character2, int critDif)
    {
        if (character1.getStat(stat1) > character2.getStat(stat2) + critDif)
        {
            if (character1.GetType() == typeof(NpcCharacter))
            {
                combatAIController.GetCombatAiPcMemory(character2).TrySetMaxStat(stat2, character1.getStat(stat1) - critDif);
            }
            else
            {
                combatAIController.GetCombatAiPcMemory(character1).TrySetMinStat(stat1, character2.getStat(stat2) + critDif);
            }
            return true;
        }
        else
        {
            if (character1.GetType() == typeof(NpcCharacter))
            {
                combatAIController.GetCombatAiPcMemory(character2).TrySetMinStat(stat2, character1.getStat(stat1) - critDif);
            }
            else
            {
                combatAIController.GetCombatAiPcMemory(character1).TrySetMaxStat(stat1, character2.getStat(stat2) + critDif);
            }
            return false;
        }
    }

    public void CounterAttack(int damage, float defensModifier)
    {
        BattleLog("attacker hit with " + damage + " damage");
        if (currentCharacter.hurt(damage))
        {
            KillCharacter(currentCharacter);
            currentCharacter = null;
        }
        StartCoroutine(AttackAnimation()); // TODO: move to CombatActions
    }

    public void SetSupporting(bool isActive)
    {
        if (!aiTurn)
        {
            CombatAiPcMemory combatAiPcMemory = combatAIController.GetCombatAiPcMemory(currentCharacter);
            combatAiPcMemory.IsSupporting = isActive;
        }
    }

    private void KillCharacter(Character character)
    {
        if (playerGroup.Contains(character))
        {
            BattleLog("PC died");
            combatAIController.RemoveCombatAiPcMemory(character);
            if (!playerGroup.Remove(character))
            {
                Debug.Log("Unknown Player casualty " + character);
            }
        }
        else if (character.GetType() == typeof(NpcCharacter))
        {
            BattleLog("NPC died");
            var npcCharacter = (NpcCharacter)character;
            if (!enemies.Remove(npcCharacter))
            {
                Debug.Log("Unknown Enemy casualty " + character);
            }
        }
    }

    public void EncourageTeam(int damageBuff)
    {
        if (aiTurn)
        {
            if (damageBuff > 0)
            {
                BattleLog("Enemies Supported");
            }
            foreach (NpcCharacter enemy in enemies)
            {
                enemy.SetEncourage(damageBuff);
            }
        }
        else
        {
            if (damageBuff < 0)
            {
                combatAIController.GetCombatAiPcMemory(currentCharacter).IsSupporting = false;
            }
            else
            {
                combatAIController.GetCombatAiPcMemory(currentCharacter).IsSupporting = true;
            }
            BattleLog("Characters buffed: + " + damageBuff + " damage");
            foreach (Character playerCharacter in playerGroup)
            {
                playerCharacter.SetEncourage(damageBuff);
            }
        }
    }
    public void ProvokingTeam(bool isActive)
    {
        if (aiTurn)
        {
            if (isActive)
            {
                enemiesProvoking.Remove(currentCharacter);
            }
            else
            {
                BattleLog("Enemie Provoking");
                enemiesProvoking.Add(currentCharacter);
            }
        }
        else
        {
            if (isActive)
            {
                playerProvoking.Remove(currentCharacter);
            }
            else
            {
                BattleLog("Enemies Provoked");
                playerProvoking.Add(currentCharacter);
            }
        }
    }
    public void DistractingTeam(bool active)
    {
        if (aiTurn)
        {
            if (active)
            {
                enemiesDistracting.Remove(currentCharacter);

            }
            else
            {
                enemiesDistracting.Add(currentCharacter);
                BattleLog("Enemies Supported");
            }
        }
        else
        {
            if (active)
            {
                playerDistracting.Remove(currentCharacter);
                combatAIController.GetCombatAiPcMemory(currentCharacter).IsSupporting = false;
                combatAIController.GetCombatAiPcMemory(currentCharacter).IsDistracting = false;
            }
            else
            {
                combatAIController.GetCombatAiPcMemory(currentCharacter).IsSupporting = true;
                playerDistracting.Add(currentCharacter);
                BattleLog("Enemies Distracted");
            }
        }
    }

    public void SCoroutine() //temp
    {
        StartCoroutine(AttackAnimation());
    }

    void EndFight()
    {
        CombatButtons.SetActive(false);
        // int reward = 5; //TODO add reward // TODO: reward only if player is victorious, EndFight() is also called, when playerGroup is empty
        rollEncounter.EndEncounter(0);
    }

    IEnumerator AttackAnimation() // TODO: move to CombatAnimator
    {
        CombatButtons.SetActive(false);
        yield return new WaitForSeconds(2);
        EndTurn();
    }
    IEnumerator WaitForSecToStart(int sec)
    {
        yield return new WaitForSeconds(sec);
        NextTurn();
    }
    public void BattleLog(string text)
    {
        Debug.Log(text);
    }
}
