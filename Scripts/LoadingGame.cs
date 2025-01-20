using UnityEngine;

using UnityEngine.SceneManagement;
public class LoadingGame : MonoBehaviour
{
    public static bool GameResume = false;
   


    private static bool isMarkedDontDestroyOnLoad = false;

    private void Awake()
    {
        // Check if the GameObject is already marked as DontDestroyOnLoad
        if (!isMarkedDontDestroyOnLoad)
        {
            // Ensure that this GameObject is not destroyed when loading a new scene
            DontDestroyOnLoad(gameObject);
            isMarkedDontDestroyOnLoad = true;
        }
        else
        {
            // If the GameObject is already marked as DontDestroyOnLoad, destroy the duplicate
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        if(PlayerPrefs.HasKey("QuitTimeSave"))
        {
            GameResume = true;
        }
        else
        {
            GameResume = false;
        }

        SceneManager.LoadScene(1);

      
    }

   
}
