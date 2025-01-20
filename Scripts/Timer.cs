using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Analytics;
using UnityEngine.SocialPlatforms.Impl;
public class Timer : MonoBehaviour
{
    //public Sprite[] avatarSprites;

    public Image enemyAvatarImg;
    public Image startTxtImg;
    public Image movingImgMatchMaking;
    public Animator movingMatchMakingAnimator;

    public List<string> enemyNames;
    public Text enemyNameTxt;

    public GameObject matchMakingScreen;
    public GameObject MainMenuScreen;
    public GameObject SettingsScreen;
    public GameObject LeadboardScreen;
    public GameObject QuitPanel;
    public GameObject GameModePanel; // Panel for game modes
    public Button EasyModeButton, MediumModeButton, HardModeButton; // Buttons for modes
    public Text SelectedGameModeText; // Text to display the selected game mode

    //public Button SaveModeButton; // Save button for selected mode
    //public Text SelectedModeText; // Text to display selected mode

    public AudioSource backgroundMusic; // Add reference to the background music AudioSource
    public float lowVolumeLevel = 0.2f; // Define the volume level for the background music
    public float volumeDelay = 1.0f; // Delay before lowering the volume

    float timeRemaining;
    [HideInInspector]public bool ismatchMakingStart = false;
    public Text timeText;
    public AudioSource matchMakingAudio;
    public AudioClip matchMakingClip;
    public AudioClip playerJoinedClip;

    private string selectedGameMode = "Easy"; // Default mode
    public GameObject[] snakes; // Array to store all snakes in the scene

    private void Start()
    {
        // Load the saved game mode or set to default
        selectedGameMode = PlayerPrefs.GetString("GameMode", "Easy");
        UpdateSelectedGameModeText();

        // Add listeners for game mode buttons
        EasyModeButton.onClick.AddListener(() => SelectGameMode("Easy"));
        MediumModeButton.onClick.AddListener(() => SelectGameMode("Medium"));
        HardModeButton.onClick.AddListener(() => SelectGameMode("Hard"));

        //// Add listener for the save button
        //SaveModeButton.onClick.AddListener(SaveSelectedGameMode);

    }
    public void GameModesButton()
    {
        //MainMenuScreen.SetActive(false);
        GameModePanel.SetActive(true);
    }

    public void SelectGameMode(string mode)
    {
        selectedGameMode = mode; // Set selected mode
        PlayerPrefs.SetString("GameMode", mode); // Save the mode to PlayerPrefs
        UpdateSelectedGameModeText(); // Update the displayed text

        //SelectedModeText.text = "Selected Mode: " + mode; // Update UI text
        GameModePanel.SetActive(false); // Close the Game Modes panel
        MainMenuScreen.SetActive(true); // Return to Main Menu
    }
    private void UpdateSelectedGameModeText()
    {
        SelectedGameModeText.text = "" + selectedGameMode;
    }

    public void PlayButton()
    {
        // Disable the Main Menu screen
        MainMenuScreen.SetActive(false);

        // Enable the Matchmaking screen
        matchMakingScreen.SetActive(true);

        // Activating match making and starting the timer
        //matchMakingScreen.SetActive(true);
        ismatchMakingStart = true; // Timer starts when Play button is clicked
        timeRemaining = 5f; // Resetting timer in case it's reused
        if (LoadingGame.GameResume == false)
        {
            StartCoroutine(StartMatchMakingSound());
        }

        // Start coroutine to lower the background music volume after a delay
        if (backgroundMusic != null)
        {
            StartCoroutine(LowerVolumeAfterDelay());
        }

        // Adjust snakes based on game mode
        string mode = PlayerPrefs.GetString("GameMode", "Easy");
        AdjustSnakesBasedOnMode(mode);

        //FirebaseAnalytics.LogEvent("game_start");
        //Debug.Log("Event logged: game_start");
    }
    private IEnumerator LowerVolumeAfterDelay()
    {
        yield return new WaitForSeconds(volumeDelay);
        backgroundMusic.volume = lowVolumeLevel;
    }

    void AdjustSnakesBasedOnMode(string mode)
    {
        int snakeLimit = 0;

        switch (mode)
        {
            case "Easy":
                snakeLimit = 2; // Fewer snakes
                break;
            case "Medium":
                snakeLimit = 5; // Medium number of snakes
                break;
            case "Hard":
                snakeLimit = 9; // Many snakes
                break;
        }

        for (int i = 0; i < snakes.Length; i++)
        {
            snakes[i].SetActive(i < snakeLimit);
        }
    }

    public void settingsbutton()
    {
        //MainMenuScreen.SetActive(false);

        SettingsScreen.SetActive(true);

    }

    public void leadboardbutton()
    {
        //MainMenuScreen.SetActive(false);

        LeadboardScreen.SetActive(true);
    }

    public void Backbutton()
    {
        SettingsScreen.SetActive(false);
        LeadboardScreen.SetActive(false);
        QuitPanel.SetActive(false);

        MainMenuScreen.SetActive(true);

    }

    void Update()
    {
        if (ismatchMakingStart && matchMakingScreen.activeInHierarchy)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Match Start!");
                timeRemaining = 0;
                ismatchMakingStart = false;
                matchMakingScreen.SetActive(false);

                // Start countdown after matchmaking
                GameObject.FindObjectOfType<CountDownTimer>().StartCountdown();

                GameManager.instance.isGamePlaying = true;
            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public IEnumerator StartMatchMakingSound()
    {
        yield return new WaitForSeconds(1f);
        matchMakingAudio.clip = matchMakingClip;
        matchMakingAudio.Play();
        StartCoroutine(PlayPlayerJoinedSound());
        StartMovingImg();
            
       
    }

    public IEnumerator PlayPlayerJoinedSound()
    {
        float duration = matchMakingClip.length;
        yield return new WaitForSeconds(duration);
        matchMakingAudio.PlayOneShot(playerJoinedClip);
        
        // Randomly select a name from the list
        if (enemyNames != null && enemyNames.Count > 0)
        {
            enemyNameTxt.text = enemyNames[Random.Range(0, enemyNames.Count)];
        }
        else
        {
            enemyNameTxt.text = "Guest" + Random.Range(1000, 9000); // Fallback name
        }

        PlayerPrefs.SetString("EnemyName", enemyNameTxt.text);
        GameManager.instance.enemyNameTxtGameplay.text = PlayerPrefs.GetString("EnemyName");
        movingImgMatchMaking.gameObject.SetActive(false);
        int randValue = Random.Range(0,GameManager.instance.enemyAvatar.Length);
        enemyAvatarImg.sprite = GameManager.instance.enemyAvatar[randValue];
        PlayerPrefs.SetInt("EnemyAvatarIndex", randValue);
       GameManager.instance.enemyAvatarGameplay.sprite = enemyAvatarImg.sprite;
    }
  


  public void StartMovingImg()
  {
    movingImgMatchMaking.gameObject.SetActive(true);
    movingMatchMakingAnimator.SetTrigger("ImgMoveStart");
  }
}