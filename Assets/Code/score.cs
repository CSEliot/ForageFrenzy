using UnityEngine;
using System.Collections.Generic;

public class score : MonoBehaviour
{

    public Camera[] winnerCameras;
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
            GameObject.FindGameObjectWithTag("sfx").GetComponent<sfx>().PlaySong(0);
            if (playerScore >= scoreToWin)//Might be better as == scoreToWin idk -kit
            {
                myScore.Winner();
                Debug.Log("you win!!!!! \n" + (scoreGoalNumber + 1));
                winnerCameras[scoreGoalNumber].rect = new Rect(0f, 0f, 1f, 1f);
                winnerCameras[scoreGoalNumber].depth = 100f;
            }
        }
    }

    public int GetScore()
    {
        return playerScore;
    }
}