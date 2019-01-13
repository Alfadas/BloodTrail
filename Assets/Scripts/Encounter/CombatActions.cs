using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatActions : MonoBehaviour
{
    public const int EASY_STAT_MIN = 50;
    public const int MEDIUM_STAT_MIN = 65;
    public const int HARD_STAT_MIN = 80;

    [SerializeField] CombatManager combatManager;
    [Header("Simple Attack")]
    [SerializeField] float simpleAttackStrMulti = 0.4f;
    [SerializeField] float simpleAttackArmorMulti = 1f;
    [Header("Heavy Attack")]
    [SerializeField] float heavyAttackStrMulti = 0.5f;
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
    [Header("Encourage")]
    [SerializeField] int encourageDamageBuff = 7;
    [Header("Try To Defend")]
    [SerializeField] float tryToDefendDamageReduction = 0.25f;
    [Header("Defensive Stance")]
    [SerializeField] float defensiveStanceDamageReduction = 0.5f;

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
        combatManager.EncourageTeam(encourageDamageBuff);
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
}