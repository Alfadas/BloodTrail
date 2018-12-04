using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {
    [SerializeField] RollEncounter rollEncounter;
    [SerializeField] BuildEncounter buildEncounter;
    [SerializeField] CharacterManager characterManager;
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

    public void StartFight()
    {
        participantsQueue = new Queue<Character>();
        playerGroup = new List<Character>();
        participants = new List<Character>();

        playerGroup = characterManager.getCharacters();
        enemies = buildEncounter.GetEnemies();
        participants.AddRange(enemies);
        participants.AddRange(playerGroup);
        participantCount = participants.Count;

        for(int i = 1; i <= participantCount; i++)
        {
            int highestSpeed = 0;
            Character fastestParticipant = null;
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

		do
		{
			currentCharacter = participantsQueue.Dequeue();
		}
		while(currentCharacter == null);
  
        if (enemies.Contains(currentCharacter))
        {
            aiTurn = true;
            currentCharacter.markCharacter(false);
            EnemyTurn();
        }
        else
        {
            currentCharacter.markCharacter(false);
            aiTurn = false;
            CombatButtons.SetActive(true);
        }
    }
    public void Attack()
    {
		currentCharacter.setDefenseStance(false);

        int damage = Mathf.RoundToInt(currentCharacter.getStat(1) * 0.5f);
        if (selectedCharacter == null || (playerGroup.Contains(selectedCharacter) && !aiTurn))
        {
            SetSelected(enemies[0]);
        }
        if (selectedCharacter.hurt(damage))
        {
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
        int reward = 5; //TODO add reward // TODO: reward only if player is victorious, EndFight() is also called, when playerGroup is empty
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
