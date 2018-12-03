using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {
    [SerializeField] BuildEncounter buildEncounter;
    [SerializeField] CaracterArrayHolder caracterArrayHolder;
    List<Character> enemies;
    List<Character> playerGroup;
    List<Character> participants;
    List<Character> deadParticipants;
    Character[] playerCharacter;
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
        playerGroup.AddRange(caracterArrayHolder.playerGroup);
        deadParticipants = new List<Character>();
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
        NextTurn();
    }

    void NextTurn()
    {
        while (currentCharacter == null)
        {
            currentCharacter = participantsQueue.Dequeue();
            if (deadParticipants.Contains(currentCharacter) && enemies.Contains(currentCharacter))
            {
                enemies.Remove(currentCharacter);
                if (enemies.Count == 0)
                {
                    EndFight();
                }
                currentCharacter = null;
            }
            else if(deadParticipants.Contains(currentCharacter) && playerGroup.Contains(currentCharacter))
            {
                playerGroup.Remove(currentCharacter);
                if (playerGroup.Count == 0)
                {
                    EndFight();
                }
                currentCharacter = null;
            }
        }
        if (enemies.Contains(currentCharacter))
        {
            aiTurn = true;
        }
        CombatButtons.SetActive(true);
    }
    public void Attack()
    {
        int damage = 0;//TODO add damage
        if (selectedCharacter.hurt(damage))
        {
            deadParticipants.Add(selectedCharacter);
        }
        StartCoroutine("AttackAnimation");
    }

    void EndTurn()
    {
        CombatButtons.SetActive(false);
        participantsQueue.Enqueue(currentCharacter);
        currentCharacter = null;
        NextTurn();
    }

    void SetSelected(Character selected)
    {
        selectedCharacter = selected;
    }

    void EndFight()
    {
        int reward = 5; //TODO add reward
    }

    void GameLost()
    {
        Application.Quit();//TODO add bad ending
    }
    void EnemyTurn()
    {
        int rollAction;
        int rollCharacter;
        rollAction = Random.Range(1, combatActionCount + 1);
        rollCharacter = Random.Range(1, playerGroup.Count);
        playerCharacter = playerGroup.ToArray();
        SetSelected(playerCharacter[rollCharacter]);
        if (rollAction == 1)
        {
            Attack();
        }
    }

    IEnumerable AttackAnimation()
    {
        yield return new WaitForSeconds(2);
        EndTurn();
    }
}
