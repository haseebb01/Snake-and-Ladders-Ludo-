using ExitGames.Client.Photon.StructWrapping;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PropsMovement : MonoBehaviour
{
    public static event Action OnTurnEnd;
    public static PropsMovement instance;


    //Private Variables 
    public bool PlayerAutoMove;
    public bool EnemyAutoMove;
    public int currentIndex = 0;
    public int singleMoveIndx;
    //bool isCurrentIndxThanHundered;
    int extraPointFromhundred;


    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        // NetworkManager.Instance().OnNetworkProcessComplete += OnProcessComplete;
    }
    private void OnDisable()
    {
        try
        {
            // NetworkManager.Instance().OnNetworkProcessComplete -= OnProcessComplete;
        }
        catch (System.Exception)
        {
        }
    }

    private void OnProcessComplete()
    {
        CheckTurns();
    }

    private void SyncMovement()
    {
        TryGetComponent<NetworkPlayer>(out NetworkPlayer networkPlayer);
        if (networkPlayer != null)
        {
            networkPlayer.Move(currentIndex, singleMoveIndx);
        }
        else
        {
            if (gameObject.tag == "Enemy")
            {
                Data data = MatchManager.Instance().GetPlayerByTag("Enemy");
                data.tokenIndex = gameObject.GetComponent<PropsMovement>().currentIndex;
                MatchManager.Instance().UpdateDataPlayerAB(data);
            }
            if (gameObject.tag == "Player")
            {
                Data data = MatchManager.Instance().GetPlayerByTag("Player");
                data.tokenIndex = this.GetComponent<PropsMovement>().currentIndex;
                MatchManager.Instance().UpdateDataPlayerAB(data);
            }
        }
    }
    public IEnumerator MovePlayer(int moveIndex, bool playerPlayed)
    {
        if (playerPlayed)
        {
            int remaining = 100 - currentIndex;
            if (moveIndex <= remaining)
            {
                currentIndex += moveIndex;
                SyncMovement();
            }
        }
        yield return new WaitForSeconds(0.15f);
        MoveByOne();
    }

    private void MoveByOne()
    {
        if (singleMoveIndx <= currentIndex)
        {
            if (singleMoveIndx < currentIndex)
            {
                singleMoveIndx++;
                SoundsManager.instance.MovementSound();

            }
            LeanTween.move(this.gameObject, GameManager.instance.points[singleMoveIndx].transform.TransformPoint(GameManager.instance.points[singleMoveIndx].position), 0.1f).setOnComplete(onComplete =>
            {
                if (singleMoveIndx < currentIndex)
                {
                    //Move Player until Player Reached current Index
                    StartCoroutine(MovePlayer(singleMoveIndx, false));
                }
                if (singleMoveIndx == currentIndex)
                {
                    //Movement on Stairs
                    PropsCatergory pointCategoryScript = GameManager.instance.points[singleMoveIndx].GetComponent<PropsCatergory>();
                    if (pointCategoryScript != null && pointCategoryScript.currentPointCategory == PointCategories.Stairs)
                    {
                        SoundsManager.instance.PlayStairSound();
                        LeanTween.move(this.gameObject, pointCategoryScript.endPoint.transform.TransformPoint(pointCategoryScript.endPoint.transform.position), 0.5f);
                        currentIndex = int.Parse(pointCategoryScript.endPoint.name);
                        singleMoveIndx = currentIndex;

                    }
                    else if (pointCategoryScript != null && pointCategoryScript.currentPointCategory == PointCategories.Snake) //Movement on Snake
                    {
                        SoundsManager.instance.PlaySnakeEatSound();

                        pointCategoryScript.snakeAnimator.SetTrigger("Eat");
                        transform.GetChild(0).gameObject.SetActive(false);
                        LeanTween.move(this.gameObject, pointCategoryScript.endPoint.transform.TransformPoint(pointCategoryScript.endPoint.transform.position), 0.2f).setOnComplete(onComplete =>
                        {
                            currentIndex = int.Parse(pointCategoryScript.endPoint.name);
                            singleMoveIndx = currentIndex;
                        });
                    }
                    //if (isCurrentIndxThanHundered)
                    //{
                    //    //Reverse all point which is greater than hundred
                    //    extraPointFromhundred = currentIndex;
                    //    extraPointFromhundred -= 100;
                    //    currentIndex = 100;
                    //    currentIndex -= extraPointFromhundred;
                    //    singleMoveIndx = currentIndex;
                    //    isCurrentIndxThanHundered = false;
                    //}
                    CheckTurns();

                    //Game Win Logic
                    if (currentIndex == 100)
                    {
                        WhoseWin();
                    }
                    // DiceController.Instance.isPlayerMovement = false;
                    CheckPlayersOnSamePoint();

                }
            });
        }
    }

    public void DirectMove(int pointIndex)
    {
        currentIndex = pointIndex;
        singleMoveIndx = pointIndex;
        LeanTween.move(this.gameObject, GameManager.instance.points[pointIndex].TransformPoint(GameManager.instance.points[pointIndex].position), 0.5f);

        if (NetworkManager.Instance().ActiveMultiplayer)
        {
            this.TryGetComponent<NetworkPlayer>(out NetworkPlayer NP);
            NP.currentIndex = currentIndex;
            NP.singleMoveIndex = singleMoveIndx;
            SyncMovement();
        }

    }
    public void CheckTurns()
    {
        StartCoroutine(WhoseTurn());
    }

    private IEnumerator WhoseTurn()
    {
        if (GameManager.instance.isGameEnded == false)
        {
            if (gameObject.tag == "Player")
            {
                GameObject Enemy = GameObject.FindGameObjectWithTag("Enemy");
                bool CheckRealPlayer = Enemy.TryGetComponent<NetworkPlayer>(out NetworkPlayer networkPlayer);
                // Do things for  Enemy
                yield return new WaitForSeconds(0.5f);


                if (CheckRealPlayer && networkPlayer.PlayerPhotonView.IsMine == false)
                {
                    Debug.Log("Pun: You are not right enemy to move");
                    DiceController.Instance.arrowImg.gameObject.SetActive(false);
                    PlayerRemaningTimer.instance.ResetTime();
                    PlayerRemaningTimer.instance.AssignEnemyTimerImg();
                    PlayerRemaningTimer.instance.isTimerStart = true;
                    DiceController.Instance.turnVal = true;

                    yield break;
                }
                if (CheckRealPlayer && networkPlayer.PlayerPhotonView.IsMine)
                {
                    DiceController.Instance.arrowImg.gameObject.SetActive(true);
                    PlayerRemaningTimer.instance.ResetTime();
                    PlayerRemaningTimer.instance.AssignEnemyTimerImg();
                    PlayerRemaningTimer.instance.isTimerStart = true;
                    DiceController.Instance.turnVal = true;
                    SetDiceForPlayer();
                    yield break;
                }


                if (EnemyAutoMove)
                {
                    DiceController.Instance.arrowImg.gameObject.SetActive(false);
                    PlayerRemaningTimer.instance.ResetTime();
                    PlayerRemaningTimer.instance.AssignEnemyTimerImg();
                    PlayerRemaningTimer.instance.isTimerStart = true;
                    //SoundsManager.instance.OpponentTurnSound();


                    ////Give Random Wait For Enemy Movement
                    float randWait = UnityEngine.Random.Range(1, 3);     //
                    // Set a fixed wait time for the enemy
                    //float fixedWaitTime = 2.5f;
                    //yield return new WaitForSeconds(fixedWaitTime);
                    yield return new WaitForSeconds(randWait);           //
                    PlayerRemaningTimer.instance.playOnce = false;       //
                    //PlayerRemaningTimer.instance.timerImg.color = Color.green;


                    DiceController.Instance.turnVal = true;
                    DiceController.Instance.RollDice();


                    Debug.Log("Roll Dice For Enemy");
                }
                else
                {
                    DiceController.Instance.arrowImg.gameObject.SetActive(true);
                    PlayerRemaningTimer.instance.ResetTime();
                    PlayerRemaningTimer.instance.AssignEnemyTimerImg();
                    PlayerRemaningTimer.instance.isTimerStart = true;
                    DiceController.Instance.turnVal = true;
                    SetDiceForPlayer();
                }

            }
            else if (gameObject.tag == "Enemy")
            {
                GameObject Player = GameObject.FindGameObjectWithTag("Player");
                bool CheckRealPlayer = Player.TryGetComponent<NetworkPlayer>(out NetworkPlayer networkPlayer);
                // Do things for  Player
                yield return new WaitForSeconds(0.5f);

                if (CheckRealPlayer && networkPlayer.PlayerPhotonView.IsMine == false)
                {
                    Debug.Log("Pun: You are not right player to move");
                    PlayerRemaningTimer.instance.ResetTime();
                    PlayerRemaningTimer.instance.AssignPlayerTimerImg();
                    PlayerRemaningTimer.instance.isTimerStart = true;
                    DiceController.Instance.arrowImg.gameObject.SetActive(false);
                    DiceController.Instance.turnVal = false;
                    yield break;
                }
                if (PlayerAutoMove)
                {

                    PlayerRemaningTimer.instance.ResetTime();
                    PlayerRemaningTimer.instance.AssignPlayerTimerImg();
                    PlayerRemaningTimer.instance.isTimerStart = true;
                    DiceController.Instance.arrowImg.gameObject.SetActive(false);

                    float randWait = UnityEngine.Random.Range(1, 12);
                    yield return new WaitForSeconds(randWait);
                    DiceController.Instance.turnVal = false;
                    DiceController.Instance.RollDice();
                    yield break;
                }

                PlayerRemaningTimer.instance.ResetTime();
                PlayerRemaningTimer.instance.AssignPlayerTimerImg();

                PlayerRemaningTimer.instance.isTimerStart = true;
                DiceController.Instance.arrowImg.gameObject.SetActive(true);

                // SoundsManager.instance.PlayerTurnSound();
                DiceController.Instance.turnVal = false;

                //Turn On Dice for player If Movement Completed
                SetDiceForPlayer();
                Debug.Log("Roll Dice For PLayer");
            }
            OnTurnEnd?.Invoke();

        }
    }
    private void SetDiceForPlayer()
    {
        DiceController.Instance.diceBtn.interactable = true;
        DiceController.Instance.diceValueImg.sprite = DiceController.Instance.diceDefaultSprite;
    }

    public void CallWhenSnakeEatComplete()
    {

        transform.GetChild(0).gameObject.SetActive(true);

    }

    public void WhoseWin()
    {
        if (gameObject.tag == "Player")
        {
            GameManager.instance.ShowGameWin();

        }
        else
        {
            GameManager.instance.ShowGameLoose("Second");
        }

    }

    public void CheckPlayersOnSamePoint()
    {

        if (DiceController.Instance.playerMovementScript.currentIndex == DiceController.Instance.enemyMovementScript.currentIndex)
        {
            //Decrease Size Of Player
            GameObject playerSpriteObj = DiceController.Instance.playerMovementScript.gameObject.transform.GetChild(0).gameObject;
            //LeanTween.scale(playerSpriteObj, new Vector2(0.15f, 0.15f), 1f);
            DiceController.Instance.playerMovementScript.gameObject.transform.position = new Vector2(DiceController.Instance.playerMovementScript.gameObject.transform.position.x + 0.15f, DiceController.Instance.playerMovementScript.gameObject.transform.position.y);



            //Decrease Size Of Enemy
            GameObject enemySpriteObj = DiceController.Instance.enemyMovementScript.gameObject.transform.GetChild(0).gameObject;
            //LeanTween.scale(enemySpriteObj, new Vector2(0.15f, 0.15f), 1f);
            DiceController.Instance.enemyMovementScript.gameObject.transform.position = new Vector2(DiceController.Instance.enemyMovementScript.gameObject.transform.position.x - 0.15f, DiceController.Instance.enemyMovementScript.gameObject.transform.position.y);
        }

    }
}
