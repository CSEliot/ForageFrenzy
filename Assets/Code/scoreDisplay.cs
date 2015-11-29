using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class scoreDisplay : MonoBehaviour {

    public Text MyPrintThing;//IMPORTANT!!!!!!! Name the text ui "Text"
    public score Player;

    // Use this for initialization
    void Start () {

    
    }
    
    // Update is called once per frame
    void Update () {
    
    }
    public void ScoreText()
    {
        MyPrintThing.text = "SCORE " + Player.GetScore();
    }
    public void Winner()
    {
        MyPrintThing.text = "PLAYER " + Player.scoreGoalNumber +" WINS";
    }
}