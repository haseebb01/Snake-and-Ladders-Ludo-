using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject[] PawnModels;
    public int SelectedPawn;
    public Character[] characters;
    public Text ShopPanelCoinsTxt;
    private int playerPoints = 0; // Initial points for the player

    public GameObject shoppanel;
    public GameObject notenoughcoinsPanel;
    public GameObject unlockItFirstPanel; // Panel for "Unlock it first to continue"

    public Button unlockButton;
    public Button selectSkinButton; // Reference to the Select Skin button


    private void Awake()
    {
        // Load the selected pawn from PlayerPrefs
        SelectedPawn = PlayerPrefs.GetInt("SelectedPawn", 0);

        // Deactivate all pawn models first
        foreach (GameObject Pawn in PawnModels)
            Pawn.SetActive(false);

        // Activate the previously selected pawn
        PawnModels[SelectedPawn].SetActive(true);

        // Initialize character unlock statuses
        foreach (Character c in characters)
        {
            if (c.price == 0)
                c.isUnlocked = true;
            else
            {
                c.isUnlocked = PlayerPrefs.GetInt(c.name, 0) == 0 ? false : true;
            }
        }

        UpdateShopPanelCoinsUI();
        UpdateSelectSkinButtonState();
    }
    public void InitializeShop()
    {
        // Load the selected pawn from PlayerPrefs
        SelectedPawn = PlayerPrefs.GetInt("SelectedPawn", 0);

        // Deactivate all pawn models
        foreach (GameObject Pawn in PawnModels)
            Pawn.SetActive(false);

        // Activate the selected pawn
        PawnModels[SelectedPawn].SetActive(true);

        // Update UI
        UpdateShopPanelCoinsUI();
        UpdateSelectSkinButtonState();
    }

    public void ChangeNext()
    {
        PawnModels[SelectedPawn].SetActive(false);
        SelectedPawn++;
        if (SelectedPawn == PawnModels.Length)
            SelectedPawn = 0;

        PawnModels[SelectedPawn].SetActive(true);

        if (characters[SelectedPawn].isUnlocked)
            PlayerPrefs.SetInt("SelectedPawn", SelectedPawn);
        UpdateShopPanelCoinsUI();
    }

    public void ChangePrevious()
    {
        PawnModels[SelectedPawn].SetActive(false);
        SelectedPawn--;
        if (SelectedPawn == -1)
            SelectedPawn = PawnModels.Length - 1;

        PawnModels[SelectedPawn].SetActive(true);

        if (characters[SelectedPawn].isUnlocked)
            PlayerPrefs.SetInt("SelectedPawn", SelectedPawn);
        UpdateShopPanelCoinsUI();

    }

    public void UpdateShopPanelCoinsUI()
    {
        if (ShopPanelCoinsTxt != null)
        {
            ShopPanelCoinsTxt.text = "Coins: " + PlayerPrefs.GetInt("PlayerPoints");
        }

        //ShopPanelCoinsTxt.text = "Coins: " + PlayerPrefs.GetInt("PlayerPoints");
        //nameText.text = characters[selectedPawn].name;
        if (characters[SelectedPawn].isUnlocked == true)
            unlockButton.gameObject.SetActive(false);
        else
        {
            unlockButton.GetComponentInChildren<Text>().text = "Price:" + characters[SelectedPawn].price;
            if (PlayerPrefs.GetInt("PlayerPoints") < characters[SelectedPawn].price)
            {
                unlockButton.gameObject.SetActive(true);
                unlockButton.interactable = false;
            }
            else
            {
                unlockButton.gameObject.SetActive(true);
                unlockButton.interactable = true;
            }
        }
    }

    public void Unlock()
    {
        int points = PlayerPrefs.GetInt("PlayerPoints");
        int price = characters[SelectedPawn].price;

        if (points >= price) // Ensure the player has enough points
        {
            GameManager.instance.UpdatePlayerPoints(-price); // Deduct points
            PlayerPrefs.SetInt(characters[SelectedPawn].name, 1); // Mark the character as unlocked
            PlayerPrefs.SetInt("SelectedPawn", SelectedPawn); // Save the selected pawn index
            characters[SelectedPawn].isUnlocked = true; // Update the character state
            UpdateShopPanelCoinsUI(); // Refresh UI to reflect the changes
        }
        else
        {
            Debug.Log("Not enough points to unlock this pawn.");
        }
        // Update the state of the Select Skin button
        UpdateSelectSkinButtonState();
    }


    public void SelectSkin()
    {
        // Check if the character is unlocked before allowing selection
        if (!characters[SelectedPawn].isUnlocked)
        {
            int playerCoins = PlayerPrefs.GetInt("PlayerPoints");
            int pawnPrice = characters[SelectedPawn].price;

            if (playerCoins >= pawnPrice)
            {
                // Activate the "Unlock it first to continue" panel
                unlockItFirstPanel.SetActive(true);
                StartCoroutine(DeactivatePanelAfterDelay(unlockItFirstPanel, 3f)); // 3 seconds delay
            }
            else
            {
                // Activate the "Not Enough Points" panel
                notenoughcoinsPanel.SetActive(true);
                StartCoroutine(DeactivatePanelAfterDelay(notenoughcoinsPanel, 3f)); // 3 seconds delay
            }
            return;
        }

        // Save the selected pawn to PlayerPrefs
        PlayerPrefs.SetInt("SelectedPawn", SelectedPawn);
        PlayerPrefs.Save();

        // Notify GameManager to update the active pawn immediately
        GameManager.instance.UpdateSelectedPawn(SelectedPawn);

        // Immediately update the active pawn model
        foreach (GameObject pawn in PawnModels)
        {
            pawn.SetActive(false);
        }
        PawnModels[SelectedPawn].SetActive(true);

        // Immediately update the UI state to reflect the change
        UpdateShopPanelCoinsUI();
        UpdateSelectSkinButtonState();

        Debug.Log($"Skin {SelectedPawn} selected and saved instantly.");

        // disable the shop panel screen
        shoppanel.SetActive(false);

    }
    private IEnumerator DeactivatePanelAfterDelay(GameObject panel, float delay)
{
    //// Deactivate the selected locked pawn
    //if (!characters[SelectedPawn].isUnlocked)
    //{
    //    PawnModels[SelectedPawn].SetActive(false);
    //}

    // Activate the "Not Enough Points" panel
    panel.SetActive(true);

    // Wait for the specified delay
    yield return new WaitForSeconds(delay);

    // Deactivate the panel
    if (panel != null)
    {
        panel.SetActive(false);
    }

    //// Reactivate the selected locked pawn
    //if (!characters[SelectedPawn].isUnlocked)
    //{
    //    PawnModels[SelectedPawn].SetActive(true);
    //}
}

     public void ChangePawn(int index)
    {
        // Update the selected pawn index
        SelectedPawn = index;

        // Update the button state dynamically
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        // Enable or disable the Select Skin button based on the unlock status
        if (characters[SelectedPawn].isUnlocked)
        {
            selectSkinButton.interactable = true;
        }
        else
        {
            selectSkinButton.interactable = false;
            //notenoughcoinsPanel.SetActive(true);
        }
    }

    private void UpdateSelectSkinButtonState()
    {
        // Enable or disable the Select Skin button based on whether the pawn is unlocked
        if (selectSkinButton != null)
        {
            selectSkinButton.interactable = characters[SelectedPawn].isUnlocked;
        }
    }
}
