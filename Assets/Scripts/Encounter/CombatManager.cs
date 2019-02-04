using System.Collections;
using System.Collections.Generic;
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
    List<Character> enemies;
    List<Character> playerGroup;
    List<Character> participants;
    int participantCount;

    Queue<Character> participantsQueue;
    Character firstCharacter;
    Character currentCharacter;
    Character selectedCharacter;
    [SerializeField] GameObject CombatButtons;

    int combatActionCount = 1;
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
        playerGroup = new List<Character>(); // Wasted processing power, See line 29
        participants = new List<Character>();
        enemiesDistracting = new List<Character>();
        playerDistracting = new List<Character>();
        enemiesProvoking = new List<Character>();
        playerProvoking = new List<Character>();

        playerGroup = characterManager.getCharacters();
        enemies = buildEncounter.GetEnemies();
        participants.AddRange(enemies);
        participants.AddRange(playerGroup);
        participantCount = participants.Count;

        combatAIController.GenerateAIBehaviour(enemies, playerGroup);

        for(int i = 1; i <= participantCount; i++) // while(participants.Count > 0)? More readable and could cut participantCount variable
        {
            int highestSpeed = 0; // See below, same principle, set to first characters speed
            Character fastestParticipant = null; // Stinks after NullPointerExceptions, could set it to first participant instead of null https://en.wikipedia.org/wiki/Defensive_programming
            foreach (Character participant in participants)
            {
                if (participant.getStat(2) > highestSpeed)
                {
                    highestSpeed = participant.getStat(2);
                    fastestParticipant = participant;
                }
            }
            participantsQueue.Enqueue(fastestParticipant);
            participants.Remove(fastestParticipant);
        }
        StartCoroutine(WaitForSecToStart(2));
    }

    void NextTurn()
    {
		if (enemies.Count == 0 || playerGroup.Count == 0)
        {
            EndFight();
        }
        else
        {
            do
            {
                currentCharacter = participantsQueue.Dequeue();
                if (!enemies.Contains(currentCharacter) && !playerGroup.Contains(currentCharacter))
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
                if (enemies.Contains(currentCharacter))
                {
                    characterButtonManager.highlightCharacter(null);
                    aiTurn = true;
                    currentCharacter.markCharacter(false);
                    EnemyTurn();
                }
                else
                {
                    soundManager.playSFX("turn");
                    characterButtonManager.highlightCharacter(currentCharacter);
                    aiTurn = false;
                    currentCharacter.markCharacter(false);
                    CombatButtons.SetActive(true);
                    List<Button> combatActionButtons = currentCharacter.GetCombatActionButtons();
                    if (combatActionButtons.Count == 0)
                    {
                        combatActionButtons = combatActions.BuildActionButtonList(combatActionButtons, currentCharacter);
                        currentCharacter.SetCombatActionButtons(combatActionButtons);
                    }
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
        if (playerProvoking.Count>0 && !fromPlayer)
        {
            selectedCharacter = playerProvoking[Random.Range(0, playerProvoking.Count)];
        }
        else if (enemiesProvoking.Count > 0 && fromPlayer)
        {
            selectedCharacter = enemiesProvoking[Random.Range(0, enemiesProvoking.Count)];
        }
        else
        {
            selectedCharacter = selected;
        }
        selectedCharacter.markCharacter(true);
    }

    public void Attack(int damage, float defensModifier, COMBAT_ACTION action)
    {
        bool denied = false;
        bool countered = false;
        soundManager.playSFX("attack");
        if (selectedCharacter == null || (playerGroup.Contains(selectedCharacter) && !aiTurn))
        {
            SetSelected(enemies[0],true);
        }

        if (aiTurn)
        {
            foreach (Character playerCharacter in playerDistracting)
            {
                if (playerCharacter.getStat(Character.STAT_INTELLIGENCE) > currentCharacter.getStat(Character.STAT_INTELLIGENCE))
                {
                    if (playerCharacter.getStat(Character.STAT_INTELLIGENCE) > currentCharacter.getStat(Character.STAT_INTELLIGENCE) + combatActions.GetAdditionalIntToCrit())
                    {
                        damage = Mathf.RoundToInt(damage * combatActions.GetDistractCritSuccessMulti());
                    }
                    else
                    {
                        damage = Mathf.RoundToInt(damage * combatActions.GetDistractSuccessMulti());
                    }
                }
                else
                {
                    damage = Mathf.RoundToInt(damage * combatActions.GetDistractFailMulti());
                }
            }
        }
        else if (!aiTurn)
        {
            foreach (Character enemy in enemiesDistracting)
            {
                if (enemy.getStat(Character.STAT_INTELLIGENCE) > currentCharacter.getStat(Character.STAT_INTELLIGENCE))
                {
                    if (enemy.getStat(Character.STAT_INTELLIGENCE) > currentCharacter.getStat(Character.STAT_INTELLIGENCE) + combatActions.GetAdditionalIntToCrit())
                    {
                        damage = Mathf.RoundToInt(damage * combatActions.GetDistractCritSuccessMulti());
                    }
                    else
                    {
                        damage = Mathf.RoundToInt(damage * combatActions.GetDistractSuccessMulti());
                    }
                }
                else
                {
                    damage = Mathf.RoundToInt(damage * combatActions.GetDistractFailMulti());
                }
            }
        }
        if ((selectedCharacter.IsDodging() || selectedCharacter.IscounterAttacking())&& action != COMBAT_ACTION.SwiftAttack)
        {
            if (currentCharacter.getStat(Character.STAT_AGILITY) > selectedCharacter.getStat(Character.STAT_AGILITY))
            {
                denied = true;
                if (selectedCharacter.IscounterAttacking())
                {
                    countered = true;
                }
            }
        }
        else if (countered)
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
            if (selectedCharacter.GetDamageReduction() > 0) // that was not implemented before? well, now I need it for SFX
            {
                soundManager.playSFX("parry");
                damage = Mathf.RoundToInt(damage * selectedCharacter.GetDamageReduction());
            }
            if (action == COMBAT_ACTION.Kick)
            {
                selectedCharacter.SetStuned(true);
            }
            if (selectedCharacter.hurt(damage))
            {
                if (playerGroup.Contains(selectedCharacter))
                {
                    foreach (Character enemy in enemies)
                    {
                        combatAIController.DeleteEnemy(enemy, selectedCharacter);
                    }
                }
                if (!enemies.Remove(selectedCharacter) && !playerGroup.Remove(selectedCharacter))
                {
                    Debug.Log("Unknown casualty " + selectedCharacter);
                }
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
    public void CounterAttack(int damage, float defensModifier)
    {
        if (currentCharacter.hurt(damage))
        {
            if (playerGroup.Contains(currentCharacter))
            {
                foreach(Character enemy in enemies)
                {
                    combatAIController.DeleteEnemy(enemy, currentCharacter);
                }
            }
            if (!enemies.Remove(currentCharacter) && !playerGroup.Remove(selectedCharacter))
            {
                Debug.Log("Unknown casualty " + selectedCharacter);
            }
            currentCharacter = null;
        }
        StartCoroutine(AttackAnimation()); // TODO: move to CombatActions
    }
    public void EncourageTeam(int damageBuff)
    {
        if (enemies.Contains(currentCharacter))
        {
            foreach (Character enemy in enemies)
            {
                enemy.SetEncourage(damageBuff);
            }
        }
        else if (playerGroup.Contains(currentCharacter))
        {
            foreach (Character playerCharacter in playerGroup)
            {
                playerCharacter.SetEncourage(damageBuff);
            }
        }
    }
    public void ProvokingTeam(bool active)
    {
        if (enemies.Contains(currentCharacter))
        {
            if (active)
            {
                enemiesProvoking.Remove(currentCharacter);
            }
            else
            {
                enemiesProvoking.Add(currentCharacter);
            }
        }
        else if (playerGroup.Contains(currentCharacter))
        {
            if (active)
            {
                playerProvoking.Remove(currentCharacter);
            }
            else
            {
                playerProvoking.Add(currentCharacter);
            }
        }
    }
    public void DistractingTeam(bool active)
    {
        if (enemies.Contains(currentCharacter))
        {
            if (active)
            {
                enemiesDistracting.Remove(currentCharacter);
            }
            else
            {
                enemiesDistracting.Add(currentCharacter);
            }
        }
        else if (playerGroup.Contains(currentCharacter))
        {
            if (active)
            {
                playerDistracting.Remove(currentCharacter);
            }
            else
            {
                playerDistracting.Add(currentCharacter);
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
    void EnemyTurn()
    {
        int rollAction;
        int rollCharacter;
        rollAction = Random.Range(1, combatActionCount + 1);
        rollCharacter = Random.Range(0, playerGroup.Count);
        SetSelected(playerGroup[rollCharacter],false);
        if (rollAction == 1)
        {
            combatActions.SimpleAttack();
        }
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
}
