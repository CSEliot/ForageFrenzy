using UnityEngine;
using System.Collections;

public class sfx : MonoBehaviour {

    public AudioSource myPlayer;
    public AudioClip[] musicList;

	// Use this for initialization
	void Start () {
        PlaySong(0);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("p"))
        {
            PlaySong(0);
        }
        if (Input.GetKeyDown("o"))
        {
            PlaySong(1);
        }
	}

    public void PlaySong(int song)
    {
        myPlayer.pitch = Random.Range(0.9f, 1.1f);
        myPlayer.Stop();
        myPlayer.clip = musicList[song];
        myPlayer.Play();
    }
}
