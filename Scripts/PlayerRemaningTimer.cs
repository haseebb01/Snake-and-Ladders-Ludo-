using UnityEngine;
using UnityEngine.UI;

public class PlayerRemaningTimer : MonoBehaviour
{
    public static PlayerRemaningTimer instance;
    [HideInInspector]public Image timerImg;

    public Image playerTimerImg;
    public Image enemyTimerImg;

    public float reductionSpeed = 0.5f; // Speed at which the fill amount reduces
    private float currentFill;
    [HideInInspector] public bool isTimerStart;
    [HideInInspector]public bool playOnce;
    [HideInInspector] public  int playerLeaveCounter;


    private void Awake() {
        instance =this;
        AssignPlayerTimerImg();
    }
    private void Start()
    {
        currentFill = timerImg.fillAmount;
    }

    private void Update()
    {
        if (currentFill > 0 && isTimerStart && GameManager.instance.isGameEnded==false)
        {
            currentFill -= reductionSpeed * Time.deltaTime;
            timerImg.fillAmount = currentFill;

            if(currentFill < 0.25 && playOnce == false)
            {
                SoundsManager.instance.TickingTimeStart();
                timerImg.color = Color.red;
                playOnce =true;
            }
         
            if (currentFill < 0.01)
            {
                DiceController.Instance.RollDice();
                SoundsManager.instance.StopTickingTime();
                timerImg.color = Color.green;
                playOnce =false;
                ReduceChance();
            }
        }
    }

    public void ResetTime()
    {
        isTimerStart = false;
        currentFill = 1;
        timerImg.fillAmount = currentFill;

    }

    public void AssignPlayerTimerImg()
    {
        timerImg = playerTimerImg;
    }


 public void AssignEnemyTimerImg()
    {
        timerImg = enemyTimerImg;
    }


    public void ReduceChance()
    {
        GameManager.instance.playerGameLeaveDots[playerLeaveCounter].gameObject.SetActive(false);
        playerLeaveCounter++;

        if (playerLeaveCounter >= 5)
        {
            GameManager.instance.ShowGameLoose("Auto Exit");
        }

    }

  

}
