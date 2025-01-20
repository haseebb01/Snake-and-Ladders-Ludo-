using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Analytics;
public class GameManager : MonoBehaviour
{
    
    public Button buy;
    public static GameManager instance;
    public Sprite[] enemyAvatar;
    public Image[] playerGameLeaveDots;
    public GameObject gameEndPanel;
    public GameObject MainMenuScreen;
    public GameObject SettingsScreen;
    public GameObject Gameplay;
    public GameObject QuitPanel1;
    public GameObject QuitPanel2;
    public Image gameLooseTxt;
    public Image gameWinTxt;

    public Text firstPlayerNameTxt;
    public Text secondPlayerNameTxt;

    public Text secondPlayerStatusTxt;
    public Text enemyNameTxtGameplay;
    public Image[] volumeOnImages;
    public Image[] volumeOffImages;
    public RectTransform[] points;
    
    public GameObject playerPrefab;
    private GameObject currentPlayer;
    int characterIndex;
    public GameObject ShopPanel; // Reference to the shop panel


    public int noOfPlayers;
    public Sprite[] enemySprites;
    public Sprite[] playerSprites;
    public GameObject prop;
    public GameObject playerProp;
    //public Text ShopPanelCoinsTxt;
    public Text mainMenuCoinsTxt; // Coins text in Main Menu panel
    public Text playerPointsTxt; // Reference to display the player's points in the gameend panel
    public Text enemyPointsTxt;  // Reference to display the enemy's points in the gameend panel
    
    private int playerPoints = 0; // Initial points for the player
    private int enemyPoints = 0;  // Initial points for the enemy


    public Image enemyAvatarGameplay;


    public Image firstAvatarGameEnd;
    public Image secondAvatarGameEnd;
    public List<Sprite> avatarSprites; // List of avatar options


    public Slider musicSlider; // Reference to the music slider in the inspector
    public Button saveButton;  // Reference to the save button in the inspector

    [HideInInspector] public bool isGamePlaying;
    [HideInInspector] public bool isGameEnded = false;
    //  public RectTransform enemyPosition;


    private void Awake()
    {
        instance = this;
        characterIndex = PlayerPrefs.GetInt("SelectedPawn", 0);
        PlayerPrefs.Save();
        //GameObject player = Instantiate(playerPrefab[characterIndex]);
        // Load player and enemy points from PlayerPrefs or set to 0 by default
        playerPoints = PlayerPrefs.HasKey("PlayerPoints") ? PlayerPrefs.GetInt("PlayerPoints") : 0;
        enemyPoints = PlayerPrefs.HasKey("EnemyPoints") ? PlayerPrefs.GetInt("EnemyPoints") : 0;

      

        // Update the UI
        UpdateMainMenuCoinsUI();
        UpdatePointsUI();

        Time.timeScale = 1;
        InstantiatePlayer();
        CheckMusicPreference();

        // Add Save Button Listener
        saveButton.onClick.AddListener(SaveVolumeAndReturnToMenu);
    }

    void InstantiatePlayer()
    {
        List<int> availableEnemyIndices = new List<int>();

        // Populate the list with all indices except the player's selected index
        for (int i = 0; i < enemySprites.Length; i++)
        {
            if (i != characterIndex) // Exclude the player's sprite index
            {
                availableEnemyIndices.Add(i);
            }
        }

        for (int i = 0; i < noOfPlayers; i++)
        {
            if (i == 0) // For the main player
            {
                if (currentPlayer == null)
                {
                    currentPlayer = Instantiate(playerPrefab, transform.TransformPoint(points[0].position), Quaternion.identity);

                    // Set the selected character sprite for the player
                    UpdatePawnSprite(characterIndex, currentPlayer);

                    // Assign the selected player sprite to the player prop
                    AssignSpriteToPlayerProp(characterIndex);
                }
            }
            else // For enemies
            {
                GameObject enemy = Instantiate(playerPrefab, transform.TransformPoint(points[0].position), Quaternion.identity);

                // Adjust position slightly to prevent overlap
                float xPos = enemy.transform.position.x;
                xPos += i * 0.5f; // Offset enemies
                enemy.transform.position = new Vector3(xPos, enemy.transform.position.y);

                enemy.tag = "Enemy";
                enemy.name = $"Enemy_{i}";

                // Randomly select an enemy sprite index from available indices
                int randomIndex = availableEnemyIndices[Random.Range(0, availableEnemyIndices.Count)];

                // Remove the chosen index to ensure uniqueness (optional, depending on your requirement)
                availableEnemyIndices.Remove(randomIndex);

                // Update the enemy sprite
                UpdatePawnSprite(randomIndex, enemy);

                // Assign the enemy sprite to the enemy prop
                if (i == 1 && prop != null)
                {
                    AssignSpriteToEnemyProp(randomIndex);
                }

                Debug.Log($"Enemy {enemy.name} assigned random sprite index: {randomIndex}");
            }
        }
    }

    public void UpdateSelectedPawn(int newSelectedPawn)
    {
        // Update the character index
        characterIndex = newSelectedPawn;
        PlayerPrefs.SetInt("SelectedPawn", characterIndex); // Save the new selected pawn
        PlayerPrefs.Save();

        // Update the active sprite of the current player
        UpdatePawnSprite(characterIndex, currentPlayer);

        // Assign the selected sprite to the player prop
        AssignSpriteToPlayerProp(characterIndex);

        Debug.Log($"Player pawn updated to index: {newSelectedPawn}");
    }

    
    private void UpdatePawnSprite(int index, GameObject targetPlayer)
    {
        if (targetPlayer == null)
        {
            Debug.LogError("Target player object is null.");
            return;
        }

        // Get all child sprites of the target player prefab
        SpriteRenderer[] spriteRenderers = targetPlayer.GetComponentsInChildren<SpriteRenderer>(true);

        // Loop through all child sprites and activate the correct one
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            // Ensure the child object is tagged properly
            if (spriteRenderers[i].CompareTag("PawnSprite"))
            {
                // Activate the sprite at the matching index
                spriteRenderers[i].gameObject.SetActive(i == index);

                // Update the sprite image if it belongs to an enemy
                if (targetPlayer.CompareTag("Enemy") && i == index)
                {
                    spriteRenderers[i].sprite = enemySprites[index];
                }
            }
        }
    }

    /// <summary>
    /// Assigns the selected player sprite to the player prop UI object.
    /// </summary>
    /// <param name="index">The index of the player sprite to assign.</param>
    private void AssignSpriteToPlayerProp(int index)
    {
        // Ensure the player prop object and its Image component are valid
        if (playerProp == null)
        {
            Debug.LogError("Player prop object is not assigned.");
            return;
        }

        Image playerPropImage = playerProp.GetComponent<Image>();
        if (playerPropImage == null)
        {
            Debug.LogError("Player prop object does not have an Image component.");
            return;
        }

        // Assign the sprite from the player's sprite array
        if (index >= 0 && index < playerSprites.Length)
        {
            playerPropImage.sprite = playerSprites[index];
            Debug.Log($"Player prop image updated to player sprite index: {index}");
        }
        else
        {
            Debug.LogError($"Invalid player sprite index: {index}");
        }
    }

   
    private void AssignSpriteToEnemyProp(int index)
    {
        // Ensure the prop object and its Image component are valid
        if (prop == null)
        {
            Debug.LogError("Enemy prop object is not assigned.");
            return;
        }

        Image propImage = prop.GetComponent<Image>();
        if (propImage == null)
        {
            Debug.LogError("Enemy prop object does not have an Image component.");
            return;
        }

        // Assign the sprite from the enemySprites array
        if (index >= 0 && index < enemySprites.Length)
        {
            propImage.sprite = enemySprites[index];
            Debug.Log($"Enemy prop image updated to enemy sprite index: {index}");
        }
        else
        {
            Debug.LogError($"Invalid enemy sprite index: {index}");
        }
    }

   
    void CheckMusicPreference()
    {
        // Load volume from PlayerPrefs if available, otherwise set default
        float savedVolume = PlayerPrefs.HasKey("Volume") ? PlayerPrefs.GetFloat("Volume") : 1.0f;
        AudioListener.volume = savedVolume;

        // Update the slider value to reflect the saved volume
        if (musicSlider != null)
        {
            musicSlider.value = savedVolume;
        }

        UpdateVolumeUI(savedVolume > 0);
    }
    public void AdjustVolume(float value)
    {
        // Adjust volume in real-time as the slider moves
        AudioListener.volume = value;
        UpdateVolumeUI(value > 0);
    }

    void UpdateVolumeUI(bool isVolumeOn)
    {
        for (int i = 0; i < volumeOffImages.Length; i++)
        {
            volumeOffImages[i].gameObject.SetActive(!isVolumeOn);
            volumeOnImages[i].gameObject.SetActive(isVolumeOn);
        }
    }

    public void SaveVolumeAndReturnToMenu()
    {
        // Save the current volume value to PlayerPrefs
        PlayerPrefs.SetFloat("Volume", musicSlider.value);
        PlayerPrefs.Save();

        // Return to the main menu (or desired panel)
        MainMenuScreen.SetActive(true);

        // Enable the Matchmaking screen
        SettingsScreen.SetActive(false);
    }


    public void TurnONOffMusic(GameObject volumeBtn)
    {
        //if (!PlayerPrefs.HasKey("Volume") || PlayerPrefs.GetInt("Volume") == 0)
        if (AudioListener.volume == 0)
        {
            //turn on Music
            AudioListener.volume = 1;
            //PlayerPrefs.SetInt("Volume", 1);
            volumeBtn.transform.GetChild(0).gameObject.SetActive(true);
            volumeBtn.transform.GetChild(1).gameObject.SetActive(false);

        }
        else
        {
            //Turn Off Music
            AudioListener.volume = 0;
            //PlayerPrefs.SetInt("Volume", 0);
            volumeBtn.transform.GetChild(0).gameObject.SetActive(false);
            volumeBtn.transform.GetChild(1).gameObject.SetActive(true);

        }

    }

    public void ShowGameWin()
    {
        SoundsManager.instance.GameWinSound();
        gameEndPanel.SetActive(true);
        gameWinTxt.gameObject.SetActive(true);
        gameLooseTxt.gameObject.SetActive(false);
        secondPlayerNameTxt.text = PlayerPrefs.GetString("EnemyName");
        isGameEnded = true;
        secondAvatarGameEnd.sprite = enemyAvatarGameplay.sprite;
        secondPlayerStatusTxt.text = "Second";
        Time.timeScale = 0;
        isGamePlaying = false;


        // Display a fixed winning score in the UI
        playerPointsTxt.text = "500"; // Show only 500 points for the win
        enemyPointsTxt.text = "0";

        // Update the cumulative points stored in PlayerPrefs
        playerPoints = PlayerPrefs.HasKey("PlayerPoints") ? PlayerPrefs.GetInt("PlayerPoints") : 0;
        playerPoints += 500;  // Add 500 points for the win
        PlayerPrefs.SetInt("PlayerPoints", playerPoints);
        PlayerPrefs.Save();

        // Save updated points
        //SavePoints();
        // Update the UI
        //UpdatePointsUI();
        //UpdateMainMenuCoinsUI();

        PlayFabManager.Instance.SendGameResult(1);
        PlayerPrefs.DeleteKey("EnemyName");
        PlayerPrefs.DeleteKey("enemyPoints");
        LoadingGame.GameResume = false;
        //FirebaseAnalytics.LogEvent("game_result", "outcome", "win");
    }

    public void ShowGameLoose(string statusTxt)
    {
        SoundsManager.instance.GameLoseSound();
        gameEndPanel.SetActive(true);
        gameLooseTxt.gameObject.SetActive(true);
        gameWinTxt.gameObject.SetActive(false);
        
        // Retrieve the player's name from PlayerPrefs and assign it to the correct UI elements
        secondPlayerNameTxt.text = PlayerPrefs.HasKey("PlayerName")
       ? PlayerPrefs.GetString("PlayerName")
       : "Guest";

        // Display the enemy's name
        firstPlayerNameTxt.text = PlayerPrefs.GetString("EnemyName");
        isGameEnded = true;

        int playerAvatarIndex = PlayerPrefs.GetInt("PlayerAvatarIndex", 0); // Default to 0 if not set
        Sprite playerAvatarSprite = avatarSprites[playerAvatarIndex];
        // Assign the avatars correctly
        secondAvatarGameEnd.sprite = playerAvatarSprite; // Player's selected avatar
        firstAvatarGameEnd.sprite = enemyAvatarGameplay.sprite;

        // Highlight losing player's section
        secondPlayerNameTxt.transform.parent.gameObject.GetComponent<Image>().color = Color.white;
        firstPlayerNameTxt.transform.parent.gameObject.GetComponent<Image>().color = Color.grey;
        secondPlayerStatusTxt.text = statusTxt;
        // Update points
        enemyPoints += 500; // Enemy gains points on win
        playerPoints = 0; // Player loses points on loss

        // Ensure neither player's points go negative
        playerPoints = Mathf.Max(playerPoints, 0);
        enemyPoints = Mathf.Max(enemyPoints, 0);

        // Update the points display in the correct UI elements
        playerPointsTxt.text = enemyPoints.ToString();
        enemyPointsTxt.text = playerPoints.ToString();

        // Save updated points
        //SavePoints();

        // Update points UI with swapping enabled
        UpdatePointsUI(true);    //
        //UpdateMainMenuCoinsUI();
        PlayerPrefs.SetString("GameFailed", "Yes");
        Time.timeScale = 0;
        isGamePlaying = false;
        LoadingGame.GameResume = false;
        // Send game result to PlayFab
        PlayFabManager.Instance.SendGameResult(0);
        // Clear enemy name from PlayerPrefs
        PlayerPrefs.DeleteKey("EnemyName");

    }
    private void SavePoints()
    {
        PlayerPrefs.SetInt("PlayerPoints", playerPoints);
        PlayerPrefs.SetInt("EnemyPoints", enemyPoints);
        PlayerPrefs.Save();
    }

    private void UpdatePointsUI(bool isGameLost = false)  //
    {
        if (playerPointsTxt != null)
        {
            // Swap points if the game is lost
            playerPointsTxt.text = isGameLost ? enemyPoints.ToString() : playerPoints.ToString();
        }
        if (enemyPointsTxt != null)
        {
            // Swap points if the game is lost
            enemyPointsTxt.text = isGameLost ? playerPoints.ToString() : enemyPoints.ToString();
        }
    }


    // Updates player points by adding or deducting the specified amount
    public void UpdatePlayerPoints(int amount)
{
    playerPoints += amount;

    // Ensure points do not go negative
    playerPoints = Mathf.Max(playerPoints, 0);

    // Save the updated points in PlayerPrefs
    SavePoints();

    // Update the UI to reflect the changes
    UpdatePointsUI();
    UpdateMainMenuCoinsUI();
}


    // Method to update the Main Menu coins UI
    public void UpdateMainMenuCoinsUI()
    {
        if (mainMenuCoinsTxt != null)
        {
            mainMenuCoinsTxt.text = "" + playerPoints;
        }
    }

    //private void SavePoint()
    //{
    //    PlayerPrefs.SetInt("PlayerPoints", playerPoints);
    //    //PlayerPrefs.SetInt("EnemyPoints", enemyPoints);
    //    PlayerPrefs.Save();
    //}

    public void ResetPoints()
    {
        playerPoints = 0;
        enemyPoints = 0;
        SavePoints();
        UpdatePointsUI();
        UpdateMainMenuCoinsUI();

    }


    public void LoadMainMenuScene()
    {
        // Delete specific keys instead of all PlayerPrefs
        PlayerPrefs.DeleteKey("EnemyName");
        //PlayerPrefs.DeleteKey("GameFailed");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        //FirebaseAnalytics.LogEvent("button_click", "button_name", "mainmenu");


    }

    public void OpenShop()
    {
        ShopPanel.SetActive(true); // Activate the shop panel
        ShopManager shopManager = ShopPanel.GetComponent<ShopManager>();
        shopManager.InitializeShop(); // Initialize the shop
        PlayerPrefs.Save();
    }


    public void quitpanel1()
    {
        // Disable the Main Menu screen
        MainMenuScreen.SetActive(false);

        // Enable the quit panel screen
        QuitPanel1.SetActive(true);
    }

    public void quitpanel2()
    {
        // Disable the screen
        Gameplay.SetActive(false);

        // Enable the quit panel screen
        QuitPanel2.SetActive(true);

        // Pause the game
        Time.timeScale = 0; // Pauses the game
    }

    public void backtogameplay()
    {
        // Disable the Main Menu screen
        QuitPanel2.SetActive(false);

        // Enable the quit panel screen
        Gameplay.SetActive(true);
        Time.timeScale = 1; // Resumes the game

    }

    public void Quit()
    {
        // Check if the application is running in a web player
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // Quit the game in web player
            Application.OpenURL("about:blank");
        }
        else
        {
            // Quit the game in other platforms
            Application.Quit();
        }
    }


}
