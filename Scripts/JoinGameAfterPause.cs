
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class JoinGameAfterPause : MonoBehaviour
{
    public static JoinGameAfterPause instance;
    private DateTime quitTime;


    private void Awake()
    {
        instance = this;
    }

    void OnApplicationFocus(bool focus)
    {
        if (focus == false && (GameManager.instance.isGamePlaying))

        {
            // Save current quit time to PlayerPrefs
            PlayerPrefs.SetString("PauseTimeSave", DateTime.Now.ToString());
            PlayerPrefs.SetString("QuitTimeSave", DateTime.Now.ToString());

        }
        else if(PlayerPrefs.HasKey("PauseTimeSave"))
        {
            string quitTimeString = PlayerPrefs.GetString("PauseTimeSave");
            quitTime = DateTime.Parse(quitTimeString);
            TimeSpan timeDifference = DateTime.Now - quitTime;
          //  Debug.Log("Time since last quit: " + timeDifference.TotalSeconds + " seconds");

            JoinedAfterHowManySeconds((float)timeDifference.TotalSeconds,DiceController.Instance.turnVal);

            PlayerPrefs.DeleteKey("PauseTimeSave");
            PlayerPrefs.DeleteKey("QuitTimeSave");
        }
    }


    public void JoinedAfterHowManySeconds(float seconds,bool whooseTurn)
    {

        if(seconds >= 3 && seconds < 40)
        {
            //Remove 1 Chance
            PlayerRemaningTimer.instance.ReduceChance();

            Debug.Log("Remove 1 Chance");
            //For Player
            PositionUpdateAfterPause(DiceController.Instance.playerMovementScript, 1, 6);

            //For Enemy
           // PositionUpdateAfterPause(DiceController.Instance.enemyMovementScript, 1, 6);

            ReduceSizeOfBothProps(DiceController.Instance.playerMovementScript, DiceController.Instance.enemyMovementScript);

            if(whooseTurn == false)
            {
                StartCoroutine(DiceController.Instance.SkipPlayerTurn());

            }
            else
            {
                DiceController.Instance.SkipEnemyTurn();
            }


        }
        else if(seconds >= 40 && seconds < 60)
        {
            //Remove 2 chance
            for (int i = 0; i < 2; i++)
            {
                PlayerRemaningTimer.instance.ReduceChance();

            }

            Debug.Log("Remove 2 Chance");
            //For Player
            PositionUpdateAfterPause(DiceController.Instance.playerMovementScript, 6, 12);

            //For Enemy
           // PositionUpdateAfterPause(DiceController.Instance.enemyMovementScript, 6, 12);
            if (whooseTurn == false)
            {
                StartCoroutine(DiceController.Instance.SkipPlayerTurn());

            }
            else
            {
                DiceController.Instance.SkipEnemyTurn();
            }

            ReduceSizeOfBothProps(DiceController.Instance.playerMovementScript, DiceController.Instance.enemyMovementScript);


        }
        else if (seconds >= 60 && seconds < 80)
        {
            //Remove 3 chance
            for (int i = 0; i < 3; i++)
            {
                PlayerRemaningTimer.instance.ReduceChance();

            }
            Debug.Log("Remove 3 Chance");
            //For Player
            PositionUpdateAfterPause(DiceController.Instance.playerMovementScript, 12, 18);

            //For Enemy
            //PositionUpdateAfterPause(DiceController.Instance.enemyMovementScript, 12, 18);
            if (whooseTurn == false)
            {
                StartCoroutine(DiceController.Instance.SkipPlayerTurn());

            }
            else
            {
                DiceController.Instance.SkipEnemyTurn();
            }
            ReduceSizeOfBothProps(DiceController.Instance.playerMovementScript, DiceController.Instance.enemyMovementScript);


        }
        else if (seconds >= 80 && seconds < 100)
        {
            //Remove 4 chance
            for (int i = 0; i < 3; i++)
            {
                PlayerRemaningTimer.instance.ReduceChance();

            }
            Debug.Log("Remove 4 Chance");
            //For Player
            PositionUpdateAfterPause(DiceController.Instance.playerMovementScript, 18, 24);

            //For Enemy
           // PositionUpdateAfterPause(DiceController.Instance.enemyMovementScript, 18, 24);
            if (whooseTurn == false)
            {
                StartCoroutine(DiceController.Instance.SkipPlayerTurn());

            }
            else
            {
                DiceController.Instance.SkipEnemyTurn();
            }
            ReduceSizeOfBothProps(DiceController.Instance.playerMovementScript, DiceController.Instance.enemyMovementScript);


        }
        else if (seconds >= 100)
        {
            //Do Game Over for Player
            GameManager.instance.ShowGameLoose("Auto Exit");
            Debug.Log("Remove All Chance");

        }
       
    }


    public void PositionUpdateAfterPause(PropsMovement propMoveScript,int startIndx,int endIndx)
    {
        int remainingIndx = 100 - propMoveScript.currentIndex;
       int randIndx = UnityEngine.Random.Range(startIndx, endIndx);

        if(randIndx <= remainingIndx)
        {
            propMoveScript.currentIndex += randIndx;
            propMoveScript.singleMoveIndx += randIndx;
            propMoveScript.transform.position = GameManager.instance.points[propMoveScript.currentIndex].position;
        }
        else
        {
            // Debug.Log("I'm not be to calculate position");

            int remainder  = randIndx - remainingIndx;

            randIndx -= (remainder - 2);
            propMoveScript.currentIndex += randIndx;
            propMoveScript.singleMoveIndx += randIndx;
            propMoveScript.transform.position = GameManager.instance.points[propMoveScript.currentIndex].position;
        }

    }


    public void ReduceSizeOfBothProps(PropsMovement playerPropScript, PropsMovement enemyPropScript)
    {
        if(playerPropScript.currentIndex == enemyPropScript.currentIndex)
        {
            LeanTween.scale(playerPropScript.transform.GetChild(0).gameObject, new Vector2(0.2f, 0.2f), 1f);
            LeanTween.scale(enemyPropScript.transform.GetChild(0).gameObject, new Vector2(0.2f, 0.2f), 1f);


        }

    }


}
