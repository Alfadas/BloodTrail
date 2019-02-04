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
    [SerializeField] float distractFailMulti = 0.9f;
    [SerializeField] float distractSuccessMulti = 0.65f;
    [SerializeField] float distractCritSuccessMulti = 0.35f;
    [SerializeField] int additionalIntToCrit = 20;

    //attacks
    public void SimpleAttack()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        int damage = Mathf.RoundToInt(currentCharacter.getStat(1) * simpleAttackStrMulti) + currentCharacter.getWeaponDamage() + currentCharacter.GetEncourage();
        combatManager.Attack(damage, simpleAttackArmorMulti, COMBAT_ACTION.SimpleAttack);
    }
    public void HeavyAttack()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        int damage = Mathf.RoundToInt(currentCharacter.getStat(Character.STAT_STRENGTH) * heavyAttackStrMulti) + currentCharacter.getWeaponDamage() + currentCharacter.GetEncourage();
        combatManager.Attack(damage, heavyAttackArmorMulti, COMBAT_ACTION.HeavyAttack);
    }
    public void SwiftAttack()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        int damage = Mathf.RoundToInt(currentCharacter.getStat(Character.STAT_AGILITY) * swiftAttackAgiMulti) + currentCharacter.getWeaponDamage() + currentCharacter.GetEncourage();
        combatManager.Attack(damage, swiftAttackArmorMulti, COMBAT_ACTION.SwiftAttack);
    }
    public void WeakpointAttack()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        int damage = Mathf.RoundToInt(currentCharacter.getStat(Character.STAT_STRENGTH) * weakpointAttackStrMulti) + currentCharacter.getWeaponDamage() + currentCharacter.GetEncourage();
        combatManager.Attack(damage, weakpointAttackArmorMulti, COMBAT_ACTION.WeakpointAttack);
    }
    public void Kick()
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
    public void CounterAttackStance()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        currentCharacter.SetCounterAttacking(true);
        combatManager.SCoroutine();
    }
    public void Encourage()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        currentCharacter.SetEncouraging(true);
        combatManager.EncourageTeam(currentCharacter.getStat(Character.STAT_INTELLIGENCE)/10);
        combatManager.SCoroutine();
    }
    public void TryToDefend()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        currentCharacter.SetDamageReduction(tryToDefendDamageReduction);
        combatManager.SCoroutine();
    }
    public void DefensiveStance()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        currentCharacter.SetDamageReduction(defensiveStanceDamageReduction);
        combatManager.SCoroutine();
    }
    public void ProvokeAndDefend()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        currentCharacter.SetProvoking(true);
        currentCharacter.SetDamageReduction(tryToDefendDamageReduction);
        combatManager.SCoroutine();
    }
    public void ProvokeAndCounter()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        currentCharacter.SetCounterAttacking(true);
        currentCharacter.SetDamageReduction(tryToDefendDamageReduction);
        combatManager.SCoroutine();
    }
    public void Distract()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        ControllGroupeActives(currentCharacter);
        currentCharacter.ResetActiveActions();

        currentCharacter.SetDistracting(true);
        combatManager.SCoroutine();
    }
    public void Dodge()
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
            combatManager.EncourageTeam(0);
        }
        if (currentCharacter.IsProvoking())
        {
            combatManager.ProvokingTeam(false);
        }
        if (currentCharacter.IsDistracting())
        {
            combatManager.DistractingTeam(false);
        }
    }
    public List<Button> BuildActionButtonList(List<Button> combatActionButtons, Character currentCharacter, COMBAT_BEHAVIOUR combatBehaviour)
    {
        int agility = currentCharacter.getStat(Character.STAT_AGILITY);
        int charisma = currentCharacter.getStat(Character.STAT_CHARISMA);
        int endurance = currentCharacter.getStat(Character.STAT_ENDURANCE);
        int intelligence = currentCharacter.getStat(Character.STAT_INTELLIGENCE);
        int strength = currentCharacter.getStat(Character.STAT_STRENGTH);

        if (agility < CombatActions.EASY_STAT_MIN && strength < CombatActions.EASY_STAT_MIN)
        {
            if (agility > strength)
            {
                if (combatBehaviour== COMBAT_BEHAVIOUR.Player)
                {
                    combatActionButtons.Add(combatButtonManager.GetSwiftAttackBt());
                }
                else
                {

                }
            }
            else
            {
                combatActionButtons.Add(combatButtonManager.GetSimpleAttackBt());
            }
        }
        if (strength >= CombatActions.EASY_STAT_MIN)
        {
            combatActionButtons.Add(combatButtonManager.GetSimpleAttackBt());
        }
        if (agility >= CombatActions.EASY_STAT_MIN)
        {
            combatActionButtons.Add(combatButtonManager.GetSwiftAttackBt());
        }
        if (strength >= CombatActions.MEDIUM_STAT_MIN)
        {
            combatActionButtons.Add(combatButtonManager.GetHeavyAttackBt());
        }
        if (intelligence >= CombatActions.MEDIUM_STAT_MIN && strength >= CombatActions.EASY_STAT_MIN)
        {
            combatActionButtons.Add(combatButtonManager.GetWeackpointAttackBt());
        }
        if (strength >= CombatActions.MEDIUM_STAT_MIN && agility >= CombatActions.MEDIUM_STAT_MIN)
        {
            combatActionButtons.Add(combatButtonManager.GetKickBt());
        }
        if (agility >= CombatActions.MEDIUM_STAT_MIN)
        {
            combatActionButtons.Add(combatButtonManager.GetCounterAttackStanceBt());
        }
        if (endurance >= CombatActions.MEDIUM_STAT_MIN)
        {
            combatActionButtons.Add(combatButtonManager.GetDefendBt());
        }
        else
        {
            combatActionButtons.Add(combatButtonManager.GetTryToDefendBt());
        }
        if (charisma >= CombatActions.MEDIUM_STAT_MIN)
        {
            if (endurance >= CombatActions.MEDIUM_STAT_MIN)
            {
                combatActionButtons.Add(combatButtonManager.GetProvokeAndDefendBt());
            }
            if (agility >= CombatActions.MEDIUM_STAT_MIN)
            {
                combatActionButtons.Add(combatButtonManager.GetProvokeAndCounterBt());
            }
        }
        if (agility >= CombatActions.MEDIUM_STAT_MIN && intelligence >= CombatActions.MEDIUM_STAT_MIN)
        {
            combatActionButtons.Add(combatButtonManager.GetDodgeBt());
        }
        if (charisma >= CombatActions.MEDIUM_STAT_MIN && intelligence >= CombatActions.MEDIUM_STAT_MIN)
        {
            combatActionButtons.Add(combatButtonManager.GetEncourageBt());
        }
        if (intelligence >= CombatActions.EASY_STAT_MIN)
        {
            combatActionButtons.Add(combatButtonManager.GetDistractBt());
        }
        return combatActionButtons;
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
}