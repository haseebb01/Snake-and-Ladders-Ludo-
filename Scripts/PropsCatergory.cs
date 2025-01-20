using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointCategories
{
    Default,
    Stairs,
    Snake
}

public class PropsCatergory : MonoBehaviour
{
    public PointCategories currentPointCategory; // Current point category (Default, Stairs, Snake)
    public RectTransform endPoint; // The position where the player moves if they land on a special point
    public Animator snakeAnimator; // Animator for the snake

    [Header("Snake Settings")]
    public GameObject snakeObject; // Reference to the snake GameObject (optional)
    public bool isSnakeActive; // Whether the snake is active for this point

    [Header("Manual Settings")]
    public bool allowManualLadderSelection = true; // Allow manual selection for "Stairs" in the inspector

    private void Start()
    {
        UpdateCategoryBasedOnSnake();
    }

    private void Update()
    {
        UpdateCategoryBasedOnSnake(); // Continuously check snake state during gameplay
    }

    private void UpdateCategoryBasedOnSnake()
    {
        // If manually set to "Stairs," don't override with snake logic
        if (allowManualLadderSelection && currentPointCategory == PointCategories.Stairs)
        {
            return;
        }
        // Check if the snake object is assigned
        if (snakeObject != null)
        {
            isSnakeActive = snakeObject.activeInHierarchy; // Dynamically check if the snake is active in the scene
        }
        else
        {
            isSnakeActive = false; // Default to inactive if no snake object is assigned
        }

        // Update category based on snake state
        if (isSnakeActive)
        {
            currentPointCategory = PointCategories.Snake;
        }
        else
        {
            currentPointCategory = PointCategories.Default;
        }
    }

    private void OnValidate()
    {
        // Update the category when changes are made in the editor
        UpdateCategoryBasedOnSnake();
    }
}
