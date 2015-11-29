using UnityEngine;
using System.Collections.Generic;

public class picker : MonoBehaviour {
    public float SpawnFreq;

    int PosMax;

    int RandLoc;
    int RandObj;


    public List<Transform> PosList = new List<Transform>();

    float Timer = 0;

    //public Transform[] PosList;

    public GameObject[] DuplicateList;

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        Timer += Time.deltaTime;
        if (Timer >= SpawnFreq)
        {
            RandLoc = (int)(Random.value * (PosList.Count - 1));
            RandObj = Random.Range(0, DuplicateList.Length);
            Instantiate(DuplicateList[RandObj], PosList[RandLoc].position, Quaternion.Euler(Vector3.zero));
            Timer -= SpawnFreq;
        }
    }
}
