using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatButtonManager : MonoBehaviour
{
    [Header("Button Config")]
    [SerializeField] int buttonWidth;
    [SerializeField] int space;
    [SerializeField] Button simpleAttackBt;
    [SerializeField] Button heavyAttackBt;
    [SerializeField] Button swiftAttackBt;
    [SerializeField] Button weackpointAttackBt;
    [SerializeField] Button kickBt;
    [SerializeField] Button counterAttackStanceBt;
    [SerializeField] Button tryToDefendBt;
    [SerializeField] Button DefendBt;
    [SerializeField] Button provokeAndDefendBt;
    [SerializeField] Button provokeAndCounterBt;
    [SerializeField] Button dodgeBt;
    [SerializeField] Button encourageBt;
    [SerializeField] Button distractBt;

    public void ActivateButtons(Character currentCharacter)
    {
        simpleAttackBt.gameObject.SetActive(false);
        heavyAttackBt.gameObject.SetActive(false);
        swiftAttackBt.gameObject.SetActive(false);
        weackpointAttackBt.gameObject.SetActive(false);
        kickBt.gameObject.SetActive(false);
        counterAttackStanceBt.gameObject.SetActive(false);
        tryToDefendBt.gameObject.SetActive(false);
        DefendBt.gameObject.SetActive(false);
        provokeAndDefendBt.gameObject.SetActive(false);
        provokeAndCounterBt.gameObject.SetActive(false);
        dodgeBt.gameObject.SetActive(false);
        encourageBt.gameObject.SetActive(false);
        distractBt.gameObject.SetActive(false);

        List<Button> combatActionButtons = currentCharacter.GetCombatActionButtons();
        if (combatActionButtons.Count == 0)
        {
            combatActionButtons = BuildActionButtonList(combatActionButtons, currentCharacter);
            currentCharacter.SetCombatActionButtons(combatActionButtons);
        }
        PositionCombatButtons(combatActionButtons);
    }

    List<Button> BuildActionButtonList(List<Button> combatActionButtons, Character currentCharacter)
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
                combatActionButtons.Add(swiftAttackBt);
            }
            else
            {
                combatActionButtons.Add(simpleAttackBt);
            }
        }
        if (strength >= CombatActions.EASY_STAT_MIN)
        {
            combatActionButtons.Add(simpleAttackBt);
        }
        if (agility >= CombatActions.EASY_STAT_MIN)
        {
            combatActionButtons.Add(swiftAttackBt);
        }
        if (strength >= CombatActions.MEDIUM_STAT_MIN)
        {
            combatActionButtons.Add(heavyAttackBt);
        }
        if (intelligence >= CombatActions.MEDIUM_STAT_MIN && strength >= CombatActions.EASY_STAT_MIN)
        {
            combatActionButtons.Add(weackpointAttackBt);
        }
        if (strength >= CombatActions.MEDIUM_STAT_MIN && agility >= CombatActions.MEDIUM_STAT_MIN)
        {
            combatActionButtons.Add(kickBt);
        }
        if (agility >= CombatActions.MEDIUM_STAT_MIN)
        {
            combatActionButtons.Add(counterAttackStanceBt);
        }
        if (endurance >= CombatActions.MEDIUM_STAT_MIN)
        {
            combatActionButtons.Add(DefendBt);
        }
        else
        {
            combatActionButtons.Add(tryToDefendBt);
        }
        if (charisma >= CombatActions.MEDIUM_STAT_MIN)
        {
            if (endurance >= CombatActions.MEDIUM_STAT_MIN)
            {
                combatActionButtons.Add(provokeAndDefendBt);
            }
            if (agility >= CombatActions.MEDIUM_STAT_MIN)
            {
                combatActionButtons.Add(provokeAndCounterBt);
            }
        }
        if (agility >= CombatActions.MEDIUM_STAT_MIN && intelligence >= CombatActions.MEDIUM_STAT_MIN)
        {
            combatActionButtons.Add(dodgeBt);
        }
        if (charisma >= CombatActions.MEDIUM_STAT_MIN && intelligence >= CombatActions.MEDIUM_STAT_MIN)
        {
            combatActionButtons.Add(encourageBt);
        }
        if (intelligence >= CombatActions.EASY_STAT_MIN)
        {
            combatActionButtons.Add(distractBt);
        }
        return combatActionButtons;
    }

    void PositionCombatButtons(List<Button> combatActionButtons)
    {
        float sideMulti = (combatActionButtons.Count * 0.5f);
        sideMulti = sideMulti - 0.5f;
        int lastXPosition = Mathf.RoundToInt(sideMulti * buttonWidth + sideMulti * space);

        for (int i = combatActionButtons.Count - 1; i >= 0; i--)
        {
            combatActionButtons[i].gameObject.SetActive(true);
            combatActionButtons[i].transform.localPosition = new Vector3(lastXPosition, 0);
            lastXPosition = lastXPosition - (buttonWidth + space);
        }
    }
}
