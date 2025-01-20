using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SavePlayerName : MonoBehaviour
{
    public InputField nameInputField;
    public Dropdown avatarDropdown; // Dropdown for selecting avatars
    public Text matchmakingNameText;
    public Image matchmakingProfilePicture;
    public Text gameplayNameText;
    public Image gameplayProfilePicture;
    public Text gameEndNameText;
    public Image gameEndProfilePicture;

    public List<Sprite> avatarSprites; // List of avatar options

    private void Start()
    {
        // Load and display the saved player name on start
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            string savedName = PlayerPrefs.GetString("PlayerName");
            nameInputField.text = savedName;
            UpdateMatchmakingName(savedName);
            UpdateGameplayName(savedName);
            UpdateGameEndName(savedName);
        }

        // populate the dropdown with avatar options
        PopulateAvatarDropdown();

        // load and display the saved avatar on start
        if (PlayerPrefs.HasKey("PlayerAvatarIndex"))
        {
            int savedAvatarIndex = PlayerPrefs.GetInt("PlayerAvatarIndex");
            avatarDropdown.value = savedAvatarIndex;
            UpdateAllProfilePictures(savedAvatarIndex);
        }

        // Add a listener for avatar dropdown changes
        avatarDropdown.onValueChanged.AddListener(OnAvatarSelected);
    }

    public void OnNameConfirm()
    {
        string playerName = nameInputField.text;
        if (string.IsNullOrWhiteSpace(playerName))
        {
            Debug.LogWarning("Player name is empty. Please enter a name.");
            return;
        }

        // save the player's name in PlayerPrefs
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        UpdateMatchmakingName(playerName);
        UpdateGameplayName(playerName);
        UpdateGameEndName(playerName);

        // save the selected avatar
        int selectedAvatarIndex = avatarDropdown.value;
        PlayerPrefs.SetInt("PlayerAvatarIndex", selectedAvatarIndex);
        PlayerPrefs.Save();

        UpdateAllProfilePictures(selectedAvatarIndex);
    }

    private void PopulateAvatarDropdown()
    {
        avatarDropdown.ClearOptions();

        // create a list of dropdown.OptionData to include both sprite and text
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i = 0; i < avatarSprites.Count; i++)
        {
            // Create a new Dropdown.OptionData with both text and sprite
            Dropdown.OptionData option = new Dropdown.OptionData
            {
                text = "Avatar " + (i + 1), // Add the name
                image = avatarSprites[i]   // Add the corresponding sprite
            };
            options.Add(option);
        }

        // Assign the options to the dropdown
        avatarDropdown.options = options;

        // Set the initial value (if any)
        if (PlayerPrefs.HasKey("PlayerAvatarIndex"))
        {
            avatarDropdown.value = PlayerPrefs.GetInt("PlayerAvatarIndex");
        }
    }


    private void UpdateMatchmakingName(string playerName)
    {
        if (matchmakingNameText != null)
        {
            matchmakingNameText.text = playerName;
        }
    }

    private void UpdateGameplayName(string playerName)
    {
        if (gameplayNameText != null)
        {
            gameplayNameText.text = playerName;
        }
    }

    private void UpdateGameEndName(string playerName)
    {
        if (gameEndNameText != null)
        {
            gameEndNameText.text = playerName;
        }
    }

    private void UpdateAllProfilePictures(int avatarIndex)
    {
        Sprite selectedAvatar = avatarSprites[avatarIndex];

        if (matchmakingProfilePicture != null)
        {
            matchmakingProfilePicture.sprite = selectedAvatar;
        }

        if (gameplayProfilePicture != null)
        {
            gameplayProfilePicture.sprite = selectedAvatar;
        }

        if (gameEndProfilePicture != null)
        {
            gameEndProfilePicture.sprite = selectedAvatar;
        }
    }
    private void OnAvatarSelected(int avatarIndex)
    {
        // Save the selected avatar index in PlayerPrefs
        PlayerPrefs.SetInt("PlayerAvatarIndex", avatarIndex);
        PlayerPrefs.Save();

        // Update profile pictures immediately
        UpdateAllProfilePictures(avatarIndex);
    }
}
