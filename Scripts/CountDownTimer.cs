using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownTimer : MonoBehaviour
{
    public Timer timerScript;
    public Text counterTxt;
    public GameObject counterPanel;
    float counterTime = 3;
    bool counterStart = false; // Start only after Play button is clicked
    bool runOnce;

    private void Update()
    {
        // Check if matchmaking is done and countdown needs to start
        if (counterStart && timerScript.ismatchMakingStart == false)
        {
            if (!runOnce)
            {
                counterPanel.SetActive(true);
                runOnce = true;
            }

            if (counterTime > 1)
            {
                counterTime -= Time.deltaTime;
                counterTxt.text = counterTime.ToString("f0");
            }
            else
            {
                counterTime = 0;
                counterTxt.text = counterTime.ToString();
                counterTxt.gameObject.SetActive(false);
                counterPanel.SetActive(false);
                counterStart = false;

                // Trigger game start
                PlayerRemaningTimer.instance.isTimerStart = true;
                timerScript.startTxtImg.gameObject.SetActive(true);
            }
        }
    }

    // Method to start the countdown
    public void StartCountdown()
    {
        counterTime = 3; // Reset timer
        counterStart = true; // Enable countdown
        runOnce = false; // Ensure the counter panel activates
    }
}
