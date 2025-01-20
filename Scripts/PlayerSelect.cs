using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelect : MonoBehaviour
{
    public int currentPawnIndex;
    public GameObject[] Pawns;

    void Start()
    {
        currentPawnIndex = PlayerPrefs.GetInt("Se1ectedPawn", 0);
        foreach (GameObject Pawn in Pawns)
            Pawn.SetActive(false);

        Pawns[currentPawnIndex].SetActive(true);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
