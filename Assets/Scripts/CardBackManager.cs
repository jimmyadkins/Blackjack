using UnityEngine;
using UnityEngine.UI;

public class CardBackManager : MonoBehaviour
{
    public static CardBackManager instance; // Singleton to access the selected card back globally

    public Sprite defaultCardBack; // Default card back sprite
    private Sprite selectedCardBack;

    public Image deckImage1;
    public Image deckImage2;
    public Image deckImage3;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one instance
        }

        selectedCardBack = defaultCardBack; // Set the initial card back
        UpdateDeckAppearance();
    }

    // Function to update the card back sprite
    public void SetCardBack(Sprite newCardBack)
    {
        Debug.Log("Updating Card Image");
        selectedCardBack = newCardBack;
        UpdateDeckAppearance();
    }

    // Function to get the current card back sprite
    public Sprite GetCardBack()
    {
        return selectedCardBack;
    }

    private void UpdateDeckAppearance()
    {
        if (deckImage1 != null)
        {
            deckImage1.sprite = selectedCardBack; // Apply the card back to the deck object
        }
        if (deckImage2 != null)
        {
            deckImage2.sprite = selectedCardBack; // Apply the card back to the deck object
        }
        if (deckImage3 != null)
        {
            deckImage3.sprite = selectedCardBack; // Apply the card back to the deck object
        }
    }
}
