using System.Xml;
using System.Collections.Generic;

class Answer
{
    string answer;
    int chance;
    List<int> outcome;
    XmlNode questions;

    public Answer (string answer, int chance, List<int> outcome, XmlNode questions)
    {
        this.answer = answer;
        this.chance = chance;
        this.outcome = outcome;
        this.questions = questions;
    }
    public string GetAnswer()
    {
        return answer;
    }
    public int GetChance()
    {
        return chance;
    }
    public List<int> GetOutcome()
    {
        return outcome;
    }
    public XmlNode GetQuestionsNode()
    {
        return questions;
    }
}