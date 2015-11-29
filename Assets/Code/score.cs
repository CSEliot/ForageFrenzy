using UnityEngine;
using System.Collections.Generic;

public class score : MonoBehaviour
{
    public scoreDisplay myScore;
    public int scoreGoalNumber;//this is what team you're on  -kit
    private int playerScore = 0;
    public int scoreToWin;//This isn't given a value by anything I've coded -Kit

    void OnTriggerEnter (Collider col)
    {
        if(col.gameObject.name.Contains("TV"))//when there's a collision -kit
        {
            Destroy(col.gameObject);
            playerScore += 1;
            myScore.ScoreText();
            if (playerScore >= scoreToWin)//Might be better as == scoreToWin idk -kit
            {
                myScore.Winner();
                Debug.Log("you win!!!!!" +scoreGoalNumber);
            }
        }
    }

    public int GetScore()
    {
        return playerScore;
    }
}