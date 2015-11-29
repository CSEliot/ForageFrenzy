using UnityEngine;
using System.Collections.Generic;

public class spawnmoreoverloards : MonoBehaviour {

    public picker MyPicker;

	// Use this for initialization
	void Start () {
        MyPicker.PosList.Add(transform);
	}
	
	// Update is called once per frame
	void Update () {
        //bool JustOnce = false;
        //Debug.Log("time " + (int)Time.time);
        /*
        if (((int)Time.time) % SpawnFreq == 0 && JustOnce == false){
            JustOnce = true;
            Instantiate(DuplicateList[0], PosList[0].position, Quaternion.Euler(Vector3.zero));
        }
        if (((int)Time.time) % SpawnFreq != 0){
            JustOnce = false;
        }
        */
    }
}
