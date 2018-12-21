using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatActions : MonoBehaviour
{
    [SerializeField] CombatManager combatManager;
    [SerializeField] SoundManager soundManager;

    public void StrengthAttack()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        currentCharacter.setDefenseStance(false);

        int damage = Mathf.RoundToInt(currentCharacter.getStat(1) * 0.4f) + currentCharacter.getWeaponDamage();
        combatManager.Attack(damage, 0);
    }
    public void BrainAttack() // makeshift, rewrite without redundant code
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        Character selectedCharacter = combatManager.GetSelectedCharecter();
        currentCharacter.setDefenseStance(false);

        int damage = Mathf.RoundToInt(currentCharacter.getStat(Character.STAT_STRENGTH) * 0.1f + currentCharacter.getStat(Character.STAT_INTELLIGENCE) * 0.3f) + currentCharacter.getWeaponDamage();
        combatManager.Attack(damage, 0);
    }
    public void Defens()
    {
        Character currentCharacter = combatManager.GetCurrentCharecter();
        currentCharacter.setDefenseStance(true);
        combatManager.SCoroutine();
    }
}
