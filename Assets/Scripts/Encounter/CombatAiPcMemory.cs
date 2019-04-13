using System.Collections.Generic;
using UnityEngine;

public class CombatAiPcMemory
{
    private List<int> minStats;
    private List<int> maxStats;
    public bool WasDistracting { get; set; }
    public bool WasCountering { get; set; }
    public bool WasDodgeing { get; set; }
    public bool IsDistracting { get; set; }
    public bool IsCountering { get; set; }
    public bool IsSupporting { get; set; }
    public bool Supportknown { get; set; }
    public bool IsDodgeing { get; set; }
    public Character PlayerCharacter { get; private set; }
    public int PlayerDamage {get; set; }
    public int PlayerBuffCount { get; set; }

    readonly int maxEstStat = 100;

    public CombatAiPcMemory(Character playerCharacter)
    {
        minStats = new List<int>() { 0, 0, 0, 0, 0 };
        maxStats = new List<int>() { 0, 0, 0, 0, 0 };
        PlayerCharacter = playerCharacter;
        for (int I = Character.STAT_ENDURANCE; I <= Character.STAT_CHARISMA; ++I)
        {
            minStats[I] = 0;
            if (I == Character.STAT_ENDURANCE)
            {
                minStats[I] = Mathf.RoundToInt(playerCharacter.getHealth() / playerCharacter.EndMulti);
            }
            maxStats[I] = maxEstStat;
        }
    }

    public void TrySetMaxStat(int stat,int newEstStat)
    {
        if (newEstStat < maxStats[stat])
        {
            maxStats[stat] = newEstStat;
            if (maxStats[stat] < minStats[stat])
            {
                minStats[stat] = maxStats[stat];
            }
        }
    }
    public void TrySetMinStat(int stat, int newEstStat)
    {
        if (newEstStat > maxStats[stat])
        {
            maxStats[stat] = newEstStat;
            if (maxStats[stat] < minStats[stat])
            {
                maxStats[stat] = minStats[stat];
            }
        }
    }
    public int GetMinStat (int stat)
    {
        return minStats[stat];
    }
    public int GetMaxStat(int stat)
    {
        return maxStats[stat];
    }
}

