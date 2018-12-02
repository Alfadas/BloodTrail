using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {
    [SerializeField] BuildEncounter buildEncounter;
    List<Character> enemies;
    List<Character> playerGroup;
    List<Character> participants;
    List<Character> deadParticipants;
    int participantCount;

    Queue<Character> participantsQueue;
    Character firstCharacter;
    Character currentCharacter;
    Character selectedCharacter;
    [SerializeField] GameObject CombatButtons;

    public void StartFight()
    {
        participantsQueue = new Queue<Character>();
        playerGroup = new List<Character>(); //get Playergroup
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
        CombatButtons.SetActive(true);
    }
    public void Attack()
    {
        int damage = 0;//TODO add damage
        if (selectedCharacter.hurt(damage))
        {
            deadParticipants.Add(selectedCharacter);
        }
        EndTurn();
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
}
