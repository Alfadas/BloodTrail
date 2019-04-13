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

    public void ActivateButtons(Character currentCharacter, List<Button> combatActionButtons)
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

        PositionCombatButtons(combatActionButtons);
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

    public Button GetSimpleAttackBt()
    {
        return simpleAttackBt;
    }
    public Button GetHeavyAttackBt()
    {
        return heavyAttackBt;
    }
    public Button GetSwiftAttackBt()
    {
        return swiftAttackBt;
    }
    public Button GetWeackpointAttackBt()
    {
        return weackpointAttackBt;
    }
    public Button GetKickBt()
    {
        return kickBt;
    }
    public Button GetCounterAttackStanceBt()
    {
        return counterAttackStanceBt;
    }
    public Button GetTryToDefendBt()
    {
        return tryToDefendBt;
    }
    public Button GetDefendBt()
    {
        return DefendBt;
    }
    public Button GetProvokeAndDefendBt()
    {
        return provokeAndDefendBt;
    }
    public Button GetProvokeAndCounterBt()
    {
        return provokeAndCounterBt;
    }
    public Button GetDodgeBt()
    {
        return dodgeBt;
    }
    public Button GetEncourageBt()
    {
        return encourageBt;
    }
    public Button GetDistractBt()
    {
        return distractBt;
    }
}
