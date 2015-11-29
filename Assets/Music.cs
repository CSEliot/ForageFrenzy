using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

    public AudioSource myPlayer;
    public AudioClip[] musicList;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlaySong(int song)
    {

        myPlayer.clip = musicList[song];
        myPlayer.Play();
    }
}
