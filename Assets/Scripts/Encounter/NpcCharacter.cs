using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class NpcCharacter : Character
{
    [Range(0, 4)] [SerializeField] int weaponClass = 0;
    [Range(0, 4)] [SerializeField] int armorClass = 0;

    public bool CanProvoke { get; set; }
    public COMBAT_BEHAVIOUR CombatBehaviour { get; set; }
    public List<COMBAT_ACTION> OffensiveCombatActions { get; private set; }
    public List<COMBAT_ACTION> DeffensiveCombatActions { get; private set; }
    public List<COMBAT_ACTION> SupportCombatActions { get; private set; }


    public void ReRollCharacter()
    {
        reRollStats();
        rollName();

        nutrition = getMaxNutrition();
        health = getMaxHealth();
        stuned = false;
        ResetActiveActions();
    }
    public void SetCombatActions(List<COMBAT_ACTION> offensive, List<COMBAT_ACTION> deffensive, List<COMBAT_ACTION> support)
    {
        OffensiveCombatActions = offensive;
        DeffensiveCombatActions = deffensive;
        SupportCombatActions = support;
    }
}

