using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CheckGameToContinue : MonoBehaviour
{
    public static CheckGameToContinue instance;
    public GameObject matchMakingCanvas;
    private DateTime quitTime;

    private void Awake()
    {
        instance = this;

        if(LoadingGame.GameResume == true)
        {
            string quitTimeString = PlayerPrefs.GetString("QuitTimeSave");
            quitTime = DateTime.Parse(quitTimeString);
            TimeSpan timeDifference = DateTime.Now - quitTime;
            Debug.Log("Run Game After Quit");
            StartCoroutine(ContinueGame((float)timeDifference.TotalSeconds));

            PlayerPrefs.DeleteKey("QuitTimeSave");
        }
    }


 

    public IEnumerator ContinueGame(float timeAfterJoin)
    {
      // Game Continuing after quit
        GameManager.instance.enemyAvatarGameplay.sprite = GameManager.instance.avatarSprites[PlayerPrefs.GetInt("EnemyAvatarIndex")];
        matchMakingCanvas.GetComponent<Timer>().enabled = false;
        matchMakingCanvas.GetComponent<CountDownTimer>().enabled = false;
        matchMakingCanvas.transform.GetChild(0).gameObject.SetActive(false);//Turn off matchmaking screen

        yield return new WaitForSeconds(0.01f);//Delaying because instance not going to be null
       
        DiceController.Instance.playerMovementScript.currentIndex = PlayerPrefs.GetInt("PlayerCurrentIndex");
        DiceController.Instance.playerMovementScript.singleMoveIndx = PlayerPrefs.GetInt("PlayerCurrentIndex");
        PlayerRemaningTimer.instance.playerLeaveCounter = PlayerPrefs.GetInt("PlayerRemainingChance");
        

        DiceController.Instance.enemyMovementScript.currentIndex = PlayerPrefs.GetInt("EnemyCurrentIndex");
        DiceController.Instance.enemyMovementScript.singleMoveIndx = PlayerPrefs.GetInt("EnemyCurrentIndex");

        GameManager.instance.enemyNameTxtGameplay.text = PlayerPrefs.GetString("EnemyName");


        JoinGameAfterPause.instance.JoinedAfterHowManySeconds(timeAfterJoin, DiceController.Instance.turnVal);
        //Delete All PlayerPref Keys
        PlayerPrefs.DeleteKey("PlayerCurrentIndex");
        PlayerPrefs.DeleteKey("PlayerRemainingChance");
        PlayerPrefs.DeleteKey("EnemyCurrentIndex");

        Debug.Log("Continue Game here");
    }

}
