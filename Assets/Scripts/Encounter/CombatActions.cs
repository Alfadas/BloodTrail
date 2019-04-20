using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatActions : MonoBehaviour
{
    public const int EASY_STAT_MIN = 50;
    public const int MEDIUM_STAT_MIN = 65;
    public const int HARD_STAT_MIN = 80;

    [SerializeField] CombatManager combatManager;
    [SerializeField] CombatButtonManager combatButtonManager;
    [Header("Simple Attack")]
    [SerializeField] float simpleAttackStrMulti = 0.4f;
    [SerializeField] float simpleAttackArmorMulti = 1f;
    [Header("Heavy Attack")]
    [SerializeField] float heavyAttackStrMulti = 0.55f;
    [SerializeField] float heavyAttackArmorMulti = 1f;
    [Header("Swift Attack")]
    [SerializeField] float swiftAttackAgiMulti = 0.2f;
    [SerializeField] float swiftAttackArmorMulti = 1f;
    [Header("Weakpoint Attack")]
    [SerializeField] float weakpointAttackStrMulti = 0.2f;
    [SerializeField] float weakpointAttackArmorMulti = 1f;
    [SerializeField] float damageReductionReduction = 0.5f;
    [Header("Kick")]
    [SerializeField] float kickStrMulti = 0.2f;
    [SerializeField] float kickArmorMulti = 2f;
    [Header("Counter Attack")]
    [SerializeField] float counterAttackAgiMulti = 0.4f;
    [SerializeField] float counterAttackArmorMulti = 1f;
    [Header("Try To Defend")]
    [SerializeField] float tryToDefendDamageReduction = 0.25f;
    [Header("Defensive Stance")]
    [SerializeField] float defensiveStanceDamageReduction = 0.5f;
    [Header("Distract")]
    [SerializeField] float distractFailMulti = 0.1f;
    [SerializeField] float distractSuccessMulti = 0.35f;
    [SerializeField] float distractCritSuccessMulti = 0.65f;
    [SerializeField] int additionalIntToCrit = 20;

    public void EnumToAction(COMBAT_ACTION action)
    {
        switch (action)
        {
            case COMBAT_ACTION.SimpleAttack:
                SimpleAttack();
                break;
            case COMBAT_ACTION.HeavyAttack:
                HeavyAttack();
                break;
            case COMBAT_ACTION.SwiftAttack:
                SwiftAttack();
                break;
            case COMBAT_ACTION.WeakpointAttack:
                WeakpointAttack();
                break;
            case COMBAT_ACTION.Kick:
                Kick();
                break;
            case COMBAT_ACTION.CounterAttack:
                CounterAttackStance();
                break;
            case COMBAT_ACTION.Encourage:
                Encourage();
                break;
            case COMBAT_ACTION.TryToDefend:
                TryToDefend();
                break;
            case COMBAT_ACTION.DefensiveStance:
                DefensiveStance();
                break;
            case COMBAT_ACTION.ProvokeAndCounter:
                ProvokeAndCounter();
                break;
            case COMBAT_ACTION.ProvokeAndDefend:
                ProvokeAndDefend();
                break;
            case COMBAT_ACTION.Distract:
                Distract();
                break;
            case COMBAT_ACTION.Dodge:
                Dodge();
                break;
            default:
                Debug.LogWarning("missing case " + action);
                break;
        }
    }
    //attacks
    public void SimpleAttack() // normal
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        int damage = Mathf.RoundToInt(currentCharacter.getStat(1) * simpleAttackStrMulti) + currentCharacter.getWeaponDamage() + currentCharacter.GetEncourage();
        combatManager.Attack(damage, simpleAttackArmorMulti, COMBAT_ACTION.SimpleAttack);
    }
    public void HeavyAttack() // much damage
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        int damage = Mathf.RoundToInt(currentCharacter.getStat(Character.STAT_STRENGTH) * heavyAttackStrMulti) + currentCharacter.getWeaponDamage() + currentCharacter.GetEncourage();
        combatManager.Attack(damage, heavyAttackArmorMulti, COMBAT_ACTION.HeavyAttack);
    }
    public void SwiftAttack() // not dodgeable, not counterable, bad agains reduction
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        int damage = Mathf.RoundToInt(currentCharacter.getStat(Character.STAT_AGILITY) * swiftAttackAgiMulti) + currentCharacter.getWeaponDamage() + currentCharacter.GetEncourage();
        combatManager.Attack(damage, swiftAttackArmorMulti, COMBAT_ACTION.SwiftAttack);
    }
    public void WeakpointAttack() // good against damg reduction
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        int damage = Mathf.RoundToInt(currentCharacter.getStat(Character.STAT_STRENGTH) * weakpointAttackStrMulti) + currentCharacter.getWeaponDamage() + currentCharacter.GetEncourage();
        combatManager.Attack(damage, weakpointAttackArmorMulti, COMBAT_ACTION.WeakpointAttack);
    }
    public void Kick() // stuning
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        int damage = Mathf.RoundToInt(currentCharacter.getStat(Character.STAT_STRENGTH) * kickStrMulti) + currentCharacter.getWeaponDamage() + currentCharacter.GetEncourage();
        combatManager.Attack(damage, kickArmorMulti, COMBAT_ACTION.Kick);
    }
    public void CounterAttack(int attackMulti)
    {
        Character selectedCharacter = combatManager.GetSelectedCharecter();

        int damage = Mathf.RoundToInt(selectedCharacter.getStat(Character.STAT_AGILITY) * counterAttackAgiMulti) + selectedCharacter.getWeaponDamage() + selectedCharacter.GetEncourage();
        combatManager.CounterAttack(damage * attackMulti, counterAttackArmorMulti);
    }
    //buffs
    public void CounterAttackStance() // prep to counter
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        currentCharacter.SetCounterAttacking(true);
        combatManager.SetSupporting(true);
        combatManager.SCoroutine();
    }
    public void Encourage() // buff atk for all in team
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        currentCharacter.SetEncouraging(true);
        combatManager.EncourageTeam(currentCharacter.getStat(Character.STAT_INTELLIGENCE)/10);
        combatManager.SCoroutine();
    }
    public void TryToDefend() // dmg red
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        combatManager.BattleLog("Defence Stance");
        currentCharacter.SetDamageReduction(tryToDefendDamageReduction);
        combatManager.SCoroutine();
    }
    public void DefensiveStance() // higher dmg red
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        combatManager.BattleLog("Defence Stance");
        currentCharacter.SetDamageReduction(defensiveStanceDamageReduction);
        combatManager.SCoroutine();
    }
    public void ProvokeAndDefend() // pref target + dmg red
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        currentCharacter.SetProvoking(true);
        currentCharacter.SetDamageReduction(tryToDefendDamageReduction);
        combatManager.SCoroutine();
    }
    public void ProvokeAndCounter() // pref target + counter
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();


        currentCharacter.SetProvoking(true);
        combatManager.ProvokingTeam(false);
        currentCharacter.SetCounterAttacking(true);
        combatManager.SCoroutine();
    }
    public void Distract() // dmg red against all enemys
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        currentCharacter.SetDistracting(true);
        combatManager.DistractingTeam(false);
        combatManager.SCoroutine();
    }
    public void Dodge() // dodge attacks
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        currentCharacter.SetDodging(true);
        combatManager.SCoroutine();
    }
    private void ControllGroupeActives(Character currentCharacter)
    {
        if (currentCharacter.IsEncouraging())
        {
            combatManager.EncourageTeam(-currentCharacter.getStat(Character.STAT_INTELLIGENCE) / 10);
        }
        if (currentCharacter.IsProvoking())
        {
            combatManager.ProvokingTeam(true);
        }
        if (currentCharacter.IsDistracting())
        {
            combatManager.DistractingTeam(true);
        }
        combatManager.SetSupporting(false);
    }
    public void BuildActionButtonList(Character currentCharacter, bool playerCharacter)
    {
        List<Button> combatActionButtons = new List<Button>();
        List<COMBAT_ACTION> OffensiveCombatActions = new List<COMBAT_ACTION>();
        List<COMBAT_ACTION> DeffensiveCombatActions = new List<COMBAT_ACTION>();
        List<COMBAT_ACTION> SupportCombatActions = new List<COMBAT_ACTION>();
        int agility = currentCharacter.getStat(Character.STAT_AGILITY);
        int charisma = currentCharacter.getStat(Character.STAT_CHARISMA);
        int endurance = currentCharacter.getStat(Character.STAT_ENDURANCE);
        int intelligence = currentCharacter.getStat(Character.STAT_INTELLIGENCE);
        int strength = currentCharacter.getStat(Character.STAT_STRENGTH);

        if (agility < CombatActions.EASY_STAT_MIN && strength < CombatActions.EASY_STAT_MIN)
        {
            if (agility > strength)
            {
                if (playerCharacter)
                {
                    combatActionButtons.Add(combatButtonManager.GetSwiftAttackBt());
                }
                else
                {
                    OffensiveCombatActions.Add(COMBAT_ACTION.SwiftAttack);
                }
            }
            else
            {
                if (playerCharacter)
                {
                    combatActionButtons.Add(combatButtonManager.GetSimpleAttackBt());
                }
                else
                {
                    OffensiveCombatActions.Add(COMBAT_ACTION.SimpleAttack);
                }
            }
        }
        if (strength >= CombatActions.EASY_STAT_MIN)
        {
            if (playerCharacter)
            {
                combatActionButtons.Add(combatButtonManager.GetSimpleAttackBt());
            }
            else
            {
                OffensiveCombatActions.Add(COMBAT_ACTION.SimpleAttack);
            }
        }
        if (agility >= CombatActions.EASY_STAT_MIN)
        {
            if (playerCharacter)
            {
                combatActionButtons.Add(combatButtonManager.GetSwiftAttackBt());
            }
            else
            {
                OffensiveCombatActions.Add(COMBAT_ACTION.SwiftAttack);
            }
        }
        if (strength >= CombatActions.MEDIUM_STAT_MIN)
        {
            if (playerCharacter)
            {
                combatActionButtons.Add(combatButtonManager.GetHeavyAttackBt());
            }
            else
            {
                OffensiveCombatActions.Add(COMBAT_ACTION.HeavyAttack);
            }
        }
        if (intelligence >= CombatActions.MEDIUM_STAT_MIN && strength >= CombatActions.EASY_STAT_MIN)
        {
            if (playerCharacter)
            {
                combatActionButtons.Add(combatButtonManager.GetWeackpointAttackBt());
            }
            else
            {
                OffensiveCombatActions.Add(COMBAT_ACTION.WeakpointAttack);
            }
        }
        if (strength >= CombatActions.MEDIUM_STAT_MIN && agility >= CombatActions.MEDIUM_STAT_MIN)
        {
            if (playerCharacter)
            {
                combatActionButtons.Add(combatButtonManager.GetKickBt());
            }
            else
            {
                OffensiveCombatActions.Add(COMBAT_ACTION.Kick);
            }
        }
        if (agility >= CombatActions.MEDIUM_STAT_MIN)
        {
            if (playerCharacter)
            {
                combatActionButtons.Add(combatButtonManager.GetCounterAttackStanceBt());
            }
            else
            {
                DeffensiveCombatActions.Add(COMBAT_ACTION.CounterAttack);
            }
        }
        if (endurance >= CombatActions.MEDIUM_STAT_MIN)
        {
            if (playerCharacter)
            {
                combatActionButtons.Add(combatButtonManager.GetDefendBt());
            }
            else
            {
                DeffensiveCombatActions.Add(COMBAT_ACTION.DefensiveStance);
            }
        }
        else
        {
            if (playerCharacter)
            {
                combatActionButtons.Add(combatButtonManager.GetTryToDefendBt());
            }
            else
            {
                DeffensiveCombatActions.Add(COMBAT_ACTION.TryToDefend);
            }
        }
        if (charisma >= CombatActions.MEDIUM_STAT_MIN)
        {
            if (endurance >= CombatActions.MEDIUM_STAT_MIN)
            {
                if (playerCharacter)
                {
                    combatActionButtons.Add(combatButtonManager.GetProvokeAndDefendBt());
                }
                else
                {
                    DeffensiveCombatActions.Add(COMBAT_ACTION.ProvokeAndDefend);
                }
            }
            if (agility >= CombatActions.MEDIUM_STAT_MIN)
            {
                if (playerCharacter)
                {
                    combatActionButtons.Add(combatButtonManager.GetProvokeAndCounterBt());
                }
                else
                {
                    DeffensiveCombatActions.Add(COMBAT_ACTION.ProvokeAndCounter);
                }
            }
        }
        if (agility >= CombatActions.MEDIUM_STAT_MIN && intelligence >= CombatActions.MEDIUM_STAT_MIN)
        {
            if (playerCharacter)
            {
                combatActionButtons.Add(combatButtonManager.GetDodgeBt());
            }
            else
            {
                DeffensiveCombatActions.Add(COMBAT_ACTION.Dodge);
            }
        }
        if (charisma >= CombatActions.MEDIUM_STAT_MIN && intelligence >= CombatActions.MEDIUM_STAT_MIN)
        {
            if (playerCharacter)
            {
                combatActionButtons.Add(combatButtonManager.GetEncourageBt());
            }
            else
            {
                SupportCombatActions.Add(COMBAT_ACTION.Encourage);
            }
        }
        if (intelligence >= CombatActions.EASY_STAT_MIN)
        {
            if (playerCharacter)
            {
                combatActionButtons.Add(combatButtonManager.GetDistractBt());
            }
            else
            {
                SupportCombatActions.Add(COMBAT_ACTION.Distract);
            }
        }
        if (currentCharacter.GetType() == typeof(NpcCharacter))
        {
            var npcCharacter = (NpcCharacter)currentCharacter;
            npcCharacter.SetCombatActions(OffensiveCombatActions, DeffensiveCombatActions, SupportCombatActions);
        }
        else
        {
            currentCharacter.SetCombatActionButtons(combatActionButtons);
        }
    }

    public float GetDistractFailMulti()
    {
        return distractFailMulti;
    }
    public float GetDistractSuccessMulti()
    {
        return distractSuccessMulti;
    }
    public float GetDistractCritSuccessMulti()
    {
        return distractCritSuccessMulti;
    }
    public int GetAdditionalIntToCrit()
    {
        return additionalIntToCrit;
    }
    public float GetDamageReductionReductin()
    {
        return damageReductionReduction;
    }
}