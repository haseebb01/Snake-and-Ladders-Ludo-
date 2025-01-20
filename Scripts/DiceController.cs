using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DiceController : MonoBehaviour
{
    //Static Variables
    public static DiceController Instance;
    public AudioSource diceRollAudio;
    [Header("Dice Ui Elements")]
    public Image diceValueImg;
    public DiceRollComplete diceRollImg;
    public Image arrowImg;
    public Button diceBtn;
    [Space]
    [Header("Public Variables")]
    public Sprite[] diveValueSprites;
    public Sprite diceDefaultSprite;


    //Private Variables
    Animator diceRollAnimator;

    [HideInInspector] public PropsMovement playerMovementScript;
    [HideInInspector] public PropsMovement enemyMovementScript;
    [HideInInspector] public bool turnVal;
    [HideInInspector] public bool playerPlayed;

    private NetworkManager networkManager => NetworkManager.Instance();
    private void Awake()
    {
        Instance = this;
        //diceRollAnimator = diceRollImg.gameObject.GetComponent<Animator>();
        diceValueImg.sprite = diceDefaultSprite;
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PropsMovement>();
        enemyMovementScript = GameObject.FindGameObjectWithTag("Enemy").GetComponent<PropsMovement>();
        // isPlayerMovement = true;
    }

    private void Start()
    {
        arrowImg.gameObject.SetActive(true);
    }

    //Call this on Btn Click
    public void RollDice()
    {
        arrowImg.gameObject.SetActive(false);

        int _Value = UnityEngine.Random.Range(0, 6);
        diceRollAudio.Play();
        diceBtn.interactable = false;
        diceValueImg.gameObject.SetActive(false);

        diceRollImg.gameObject.SetActive(true);
        diceRollImg.StartCoroutine(diceRollImg.RollDice(_Value));
        this.gameObject.transform.parent.GetChild(0).GetComponent<Image>().color = Color.green;
        SoundsManager.instance.TickingTimeStop();
        PlayerRemaningTimer.instance.playOnce = false;
        diceRollImg.gameObject.SetActive(true);
        //diceRollAnimator.SetTrigger("Roll");
        PlayerRemaningTimer.instance.isTimerStart = false;

        /// Check For Network Instances and Sync the Dice Roll
        if (networkManager.ActiveMultiplayer && turnVal)
        {
            // Enemy
            enemyMovementScript.TryGetComponent<NetworkPlayer>(out NetworkPlayer networkPlayer);
            if (networkPlayer != null && networkPlayer.PlayerPhotonView.IsMine)
            {
                string _SyncRoll = nameof(networkPlayer.RollDice);
                networkPlayer.PlayerPhotonView.RPC(_SyncRoll, Photon.Pun.RpcTarget.All, _Value);
            }
        }
        if (networkManager.ActiveMultiplayer && turnVal == false)
        {
            // Player
            playerMovementScript.TryGetComponent<NetworkPlayer>(out NetworkPlayer networkPlayer);
            if (networkPlayer != null && networkPlayer.PlayerPhotonView.IsMine)
            {
                string _SyncRoll = nameof(networkPlayer.RollDice);
                networkPlayer.PlayerPhotonView.RPC(_SyncRoll, Photon.Pun.RpcTarget.All, _Value);
            }
        }
    }
    public void SyncRoll(int value)
    {
        Debug.Log("Pun: Rolling Dice On Network Side");
        diceValueImg.gameObject.SetActive(false);
        diceRollImg.gameObject.SetActive(true);
        diceRollImg.StartCoroutine(diceRollImg.RollDiceOnNetwork(value));
    }

    public IEnumerator SkipPlayerTurn()
    {
        Debug.Log("Skip Player Turn");
        diceBtn.interactable = false;
        gameObject.transform.parent.GetChild(0).GetComponent<Image>().color = Color.green;
        SoundsManager.instance.TickingTimeStop();
        PlayerRemaningTimer.instance.playOnce = false;

        PlayerRemaningTimer.instance.isTimerStart = false;
        PlayerRemaningTimer.instance.playerTimerImg.fillAmount = 1;

        arrowImg.gameObject.SetActive(false);
        PlayerRemaningTimer.instance.ResetTime();
        PlayerRemaningTimer.instance.AssignEnemyTimerImg();
        PlayerRemaningTimer.instance.isTimerStart = true;

        float randWait = UnityEngine.Random.Range(1, 12);
        yield return new WaitForSeconds(randWait);
        turnVal = true;
        RollDice();
    }

    public void SkipEnemyTurn()
    {
        PlayerRemaningTimer.instance.ResetTime();
        PlayerRemaningTimer.instance.AssignPlayerTimerImg();

        PlayerRemaningTimer.instance.isTimerStart = true;
        arrowImg.gameObject.SetActive(true);

        // SoundsManager.instance.PlayerTurnSound();
        turnVal = false;

        //Turn On Dice for player If Movement Completed
        diceBtn.interactable = true;
        diceValueImg.sprite = DiceController.Instance.diceDefaultSprite;
    }



    //Call this at animation completed
    public void RollAnimComplete(int value)
    {
        diceRollImg.gameObject.SetActive(false);

        int randValue = value;//UnityEngine.Random.Range(0, 6);

        diceValueImg.sprite = diveValueSprites[randValue];
        diceValueImg.gameObject.SetActive(true);

        if (turnVal == false)
        {
            //Player Turn 
            if (GameManager.instance.isGameEnded == false)
                LeanTween.scale(playerMovementScript.transform.GetChild(0).gameObject, new Vector2(0.2f, 0.2f), 1f);
            StartCoroutine(playerMovementScript.MovePlayer(randValue + 1, true));
            playerPlayed = true;
        }
        else
        {
            //Enemy Turn 
            if (GameManager.instance.isGameEnded == false)
                LeanTween.scale(enemyMovementScript.transform.GetChild(0).gameObject, new Vector2(0.2f, 0.2f), 1f);
            StartCoroutine(enemyMovementScript.MovePlayer(randValue + 1, true));
            playerPlayed = false;
        }

        //This data is saving we can get when we player leave game.
        SavePlayerAndEnemyData();
    }

    public void DiceValueOnNetwork(int value)
    {
        diceRollImg.gameObject.SetActive(false);
        int randValue = value;
        diceValueImg.sprite = diveValueSprites[randValue];
        diceValueImg.gameObject.SetActive(true);
    }


    public void SavePlayerAndEnemyData()
    {
        PlayerPrefs.SetInt("PlayerCurrentIndex", playerMovementScript.currentIndex);
        PlayerPrefs.SetInt("EnemyCurrentIndex", enemyMovementScript.currentIndex);
        PlayerPrefs.GetInt("PlayerRemainingChance", PlayerRemaningTimer.instance.playerLeaveCounter);
    }


}
