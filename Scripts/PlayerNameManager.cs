using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    public static PlayerNameManager instance;

    private string playerName;
    private Sprite playerProfilePicture;

    private void Awake()
    {
        // Ensure there's only one instance of PlayerNameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Set the player's name
    public void SetPlayerName(string name)
    {
        playerName = name;


    }

    // Get the player's name
    public string GetPlayerName()
    {
        return playerName;
    }

    // Set the player's profile picture
    public void SetPlayerProfilePicture(Sprite picture)
    {
        playerProfilePicture = picture;
    }

    // Get the player's profile picture
    public Sprite GetPlayerProfilePicture()
    {
        return playerProfilePicture;
    }
}
