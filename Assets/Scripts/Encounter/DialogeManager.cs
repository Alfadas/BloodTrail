using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogeManager : MonoBehaviour {
    [SerializeField] RollEncounter rollEncounter;
    [SerializeField] GameObject dialogeButtons;
    [SerializeField] Text text;
    [SerializeField] Button button1;
    [SerializeField] Text button1Text;
    [SerializeField] Button button2;
    [SerializeField] Text button2Text;
    [SerializeField] Button button3;
    [SerializeField] Text button3Text;
    [SerializeField] Button button4;
    [SerializeField] Text button4Text;

    [SerializeField] string ok;
    [SerializeField] string nok;
    [SerializeField] string join;
    [SerializeField] string dialogeNewMemberAns1;
    [SerializeField] string dialogeNewMemberAns2;
    [SerializeField] string dialogeNewMemberAns3;


    [SerializeField] string dialogeFarmer1Start;
    [SerializeField] string dialogeFarmer1APos;
    [SerializeField] string dialogeFarmer1BPos;
    [SerializeField] string dialogeFarmer1CPos;
    [SerializeField] string dialogeFarmer1DPos;
    [SerializeField] string dialogeFarmer1AAns;
    [SerializeField] string dialogeFarmer1BAns;
    [SerializeField] string dialogeFarmer1CAns;

    [SerializeField] string dialogeFarmer1BAnsAPos;
    [SerializeField] string dialogeFarmer1BAnsBPos;
    [SerializeField] string dialogeFarmer1BAnsCPos;
    [SerializeField] string dialogeFarmer1BAnsDPos;
    [SerializeField] string dialogeFarmer1CAnsAPos;
    [SerializeField] string dialogeFarmer1CAnsBPos;
    [SerializeField] string dialogeFarmer1BAnsAns;
    [SerializeField] string dialogeFarmer1CAnsAAns1;
    [SerializeField] string dialogeFarmer1CAnsAAns2;
    [SerializeField] string dialogeFarmer1CAnsBAns;

    [SerializeField] string dialogeDoctor1Start;
    [SerializeField] string dialogeDoctor1APos;
    [SerializeField] string dialogeDoctor1BPos;
    [SerializeField] string dialogeDoctor1CPos;
    [SerializeField] string dialogeDoctor1AAns;
    [SerializeField] string dialogeDoctor1BAns1;
    [SerializeField] string dialogeDoctor1BAns2;

    [SerializeField] string dialogeRefugee1Start;
    [SerializeField] string dialogeRefugee1APos;
    [SerializeField] string dialogeRefugee1BPos;
    [SerializeField] string dialogeRefugee1AAns;
    [SerializeField] string dialogeRefugee1BAns;


    public void StartDialoge(string dialoge)
    {
        dialogeButtons.SetActive(true);
        if (dialoge == "farmer1")
        {
            text.text = dialogeFarmer1Start;
            button1Text.text = dialogeFarmer1APos;
            button2Text.text = dialogeFarmer1BPos;
            button3Text.text = dialogeFarmer1CPos;
            button4Text.text = dialogeFarmer1DPos;
        }
        else if(dialoge == "doctor1")
        {
            text.text = dialogeDoctor1Start;
            button1Text.text = dialogeDoctor1APos;
            button2Text.text = dialogeDoctor1BPos;
            button3Text.text = dialogeDoctor1CPos;
            button4Text.text = "";
        }
        else if (dialoge == "refugee1")
        {
            text.text = dialogeRefugee1Start;
            button1Text.text = dialogeRefugee1APos;
            button2Text.text = dialogeRefugee1BPos;
            button3Text.text = "";
            button4Text.text = "";
        }
        else
        {
            Debug.LogWarning("No Dialoge " + dialoge);
            EndDialoge(0);
        }
    }
    public void Button1()
    {
        if (button1Text.text == dialogeFarmer1APos)
        {
            text.text = dialogeFarmer1AAns;
            button1Text.text = ok;
            button2Text.text = "";
            button3Text.text = "";
            button4Text.text = "";
        }
        else if (button1Text.text == dialogeFarmer1BAnsAPos)
        {
            text.text = dialogeFarmer1BAnsAns;
            End(ok);
        }
        else if (button1Text.text == ok)
        {
            EndDialoge(0);
        }
        else if (button1Text.text == nok)
        {
            EndDialoge(1);
        }
        else if (button1Text.text == join)
        {
            EndDialoge(2);
        }
        else if (button1Text.text == dialogeFarmer1CAnsAPos)
        {
            int roll = Random.Range(1,100);
            if (roll < 60)
            {
                text.text = dialogeFarmer1CAnsAAns1;
                End(nok);
            }
            else
            {
                text.text = dialogeFarmer1CAnsAAns2;
                button1Text.text = ok;
                button2Text.text = "";
                button3Text.text = "";
                button4Text.text = "";
            }
        }
        else if (button1Text.text == dialogeDoctor1APos)
        {
            text.text = dialogeDoctor1AAns;
            End(ok);
        }
        else if (button1Text.text == dialogeRefugee1APos)
        {
            text.text = dialogeRefugee1AAns;
            End(nok);
        }
    }

    public void Button2()
    {
        if (button2Text.text == dialogeFarmer1BPos)
        {
            text.text = dialogeFarmer1BAns;
            button1Text.text = dialogeFarmer1BAnsAPos;
            button2Text.text = dialogeFarmer1BAnsBPos;
            button3Text.text = dialogeFarmer1BAnsCPos;
            button4Text.text = dialogeFarmer1BAnsDPos;
        }
        else if (button2Text.text == dialogeFarmer1BAnsBPos)
        {
            text.text = dialogeFarmer1BAnsAns;
            End(ok);
        }
        else if (button2Text.text == dialogeFarmer1CAnsBPos)
        {
            text.text = dialogeFarmer1CAnsAAns1;
            End(nok);
        }
        else if (button2Text.text == dialogeDoctor1BPos)
        {
            int roll = Random.Range(1, 100);
            if (roll < 60)
            {
                text.text = dialogeDoctor1BAns1;
                End(nok);
            }
            else
            {
                text.text = dialogeDoctor1BAns2;
                End(ok);
            }
        }
        else if (button2Text.text == dialogeRefugee1BPos)
        {
            text.text = dialogeRefugee1BAns;
            End(ok);
        }
    }
    public void Button3()
    {
        if (button3Text.text == dialogeFarmer1CPos)
        {
            text.text = dialogeFarmer1CAns;
            button1Text.text = dialogeFarmer1CAnsAPos;
            button2Text.text = dialogeFarmer1CAnsBPos;
            button3Text.text = "";
            button4Text.text = "";
        }
        else if (button3Text.text == dialogeFarmer1BAnsCPos)
        {
            text.text = dialogeFarmer1BAnsAns;
            End(ok);
        }
        else if (button3Text.text == dialogeDoctor1CPos)
        {
            NewMember();
        }
    }

    public void Button4()
    {
        if (button4Text.text == dialogeFarmer1DPos)
        {
            NewMember();
        }
        else if (button4Text.text == dialogeFarmer1BAnsDPos)
        {
            text.text = dialogeFarmer1BAnsAns;
            End(ok);
        }
    }

    void EndDialoge(int proceed)
    {
        dialogeButtons.SetActive(false);
        rollEncounter.EndEncounter(proceed);
    }

    private void NewMember()
    {
        int roll = Random.Range(1, 100);
         /*if (roll < 2)
         {
             text.text = dialogeNewMemberAns1;
             End(join);
         }*/
         if (roll > 52)
         {
             text.text = dialogeNewMemberAns2;
             End(nok);
         }
         else
         {
             text.text = dialogeNewMemberAns3;
             End(ok);
         }
    }

    private void End(string isok)
    {
        button1Text.text = isok;
        button2Text.text = "";
        button3Text.text = "";
        button4Text.text = "";
    }




}
