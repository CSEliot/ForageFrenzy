using UnityEngine;
using System.Collections.Generic;

public class score : MonoBehaviour
{
    public scoreDisplay myScore;
    public int scoreGoalNumber;//this is what team you're on  -kit
    public int[] ScoreForYou = new int[4];//4 players, hardcoded -kit
    public int scoreToWin;//This isn't given a value by anything I've coded -Kit

    void OnTriggerEnter (Collider col)
    {
        if(col.gameObject.name.Contains("TV"))//when there's a collision -kit
        {
            Destroy(col.gameObject);
            ScoreForYou[scoreGoalNumber] += 1;
            myScore.ScoreText();
            if (ScoreForYou[scoreGoalNumber] >= scoreToWin)//Might be better as == scoreToWin idk -kit
            {
                myScore.Winner();
                Debug.Log("you win!!!!!" +scoreGoalNumber);
            }
        }
    }
}