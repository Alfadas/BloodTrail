using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {
    [SerializeField] RollEncounter rollEncounter;
    [SerializeField] BuildEncounter buildEncounter;
    [SerializeField] CharacterManager characterManager;
	[SerializeField] CharacterButtonManager characterButtonManager;
	[SerializeField] SoundManager soundManager;
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

        playerGroup = characterManager.getCharacters();
        enemies = buildEncounter.GetEnemies();
        participants.AddRange(enemies);
        participants.AddRange(playerGroup);
        participantCount = participants.Count;

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
            }
        }
    }
    public void Attack()
    {
		currentCharacter.setDefenseStance(false);

		soundManager.playSFX("attack");
        int damage = Mathf.RoundToInt(currentCharacter.getStat(1) * 0.4f) + currentCharacter.getWeaponDamage();
        if (selectedCharacter == null || (playerGroup.Contains(selectedCharacter) && !aiTurn))
        {
            SetSelected(enemies[0]);
        }
		else if(selectedCharacter.isDefenseStance()) // that was not implemented before? well, now I need it for SFX
		{
			soundManager.playSFX("parry");
			damage = Mathf.RoundToInt(damage * 0.5f);
		}

        if (selectedCharacter.hurt(damage))
        {
			// Debug.Log(enemies);
			// Debug.Log("sel: " + selectedCharacter);
			if(!enemies.Remove(selectedCharacter) && !playerGroup.Remove(selectedCharacter))
				{
				Debug.Log("Unknown casualty " + selectedCharacter);
				}
        }
        StartCoroutine(AttackAnimation());
    }
	public void BrainAttack() // makeshift, rewrite without redundant code
		{
		currentCharacter.setDefenseStance(false);

		soundManager.playSFX("attack");
        int damage = Mathf.RoundToInt(currentCharacter.getStat(Character.STAT_STRENGTH) * 0.1f + currentCharacter.getStat(Character.STAT_INTELLIGENCE) * 0.3f) + currentCharacter.getWeaponDamage();
        if (selectedCharacter == null || (playerGroup.Contains(selectedCharacter) && !aiTurn))
        {
            SetSelected(enemies[0]);
        }
		else if(selectedCharacter.isDefenseStance()) // that was not implemented before? well, now I need it for SFX
		{
			soundManager.playSFX("parry");
			damage = Mathf.RoundToInt(damage * 0.5f);
		}

        if (selectedCharacter.hurt(damage))
        {
			// Debug.Log(enemies);
			// Debug.Log("sel: " + selectedCharacter);
			if(!enemies.Remove(selectedCharacter) && !playerGroup.Remove(selectedCharacter))
				{
				Debug.Log("Unknown casualty " + selectedCharacter);
				}
        }
        StartCoroutine(AttackAnimation());
		}
    public void Defens()
    {
        currentCharacter.setDefenseStance(true);
        StartCoroutine(AttackAnimation());
    }

    void EndTurn()
    {
		characterButtonManager.highlightCharacter(null);
        currentCharacter.unmarkCharacter();
        participantsQueue.Enqueue(currentCharacter);
        currentCharacter = null;
        NextTurn();
    }

    public void SetSelected(Character selected)
    {
        if (selectedCharacter != null)
        {
            selectedCharacter.unmarkCharacter();
        }
        selectedCharacter = selected;
        selectedCharacter.markCharacter(true);
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
        SetSelected(playerGroup[rollCharacter]);
        if (rollAction == 1)
        {
            Attack();
        }
    }

    IEnumerator AttackAnimation()
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
