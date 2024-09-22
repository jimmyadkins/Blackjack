using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject startMenuPanel; // The main start menu panel
    public GameObject rulesPanel;     // The rules panel
    public GameObject deckPanel;      // The deck selection panel
    public CanvasGroup menuCanvasGroup; // Canvas group for fading out the menu

    // Buttons for interacting with the menu
    public Button startButton;
    public Button rulesButton;
    public Button deckButton;
    public Button closeRulesButton;
    public Button closeDeckButton;

    public BlackjackGameManager blackjackGameManager;
    //private BlackjackGameManager blackjackGameManager; // Reference to BlackjackGameManager

    void Start()
    {
        // Ensure only the start menu is shown initially
        startMenuPanel.SetActive(true);
        startButton.interactable = true;
        rulesPanel.SetActive(false);
        deckPanel.SetActive(false);

        // Find the BlackjackGameManager in the scene
        //blackjackGameManager = FindObjectOfType<BlackjackGameManager>();
    }

    // Function to start the game and fade out the menu
    public void StartGame()
    {
        startButton.interactable = false;
        StartCoroutine(FadeOutMenuAndStartGame());
    }

    // Coroutine to fade out the menu and start the game
    private IEnumerator FadeOutMenuAndStartGame()
    {
        blackjackGameManager.canOpenMenu = false;
        float fadeDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            menuCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        menuCanvasGroup.alpha = 0f;
        startMenuPanel.SetActive(false); // Hide the menu after fading

        // Start the game by calling StartRound on the BlackjackGameManager
        //yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(1f); // Extra delay to ensure smooth transition
        startButton.interactable = true;
        blackjackGameManager.StartRound();
        //Debug.Log("Fade out actually done");
    }

    // Show the Rules panel
    public void ShowRules()
    {
        startMenuPanel.SetActive(false);
        rulesPanel.SetActive(true);
    }

    // Close the Rules panel
    public void CloseRules()
    {
        rulesPanel.SetActive(false);
        startMenuPanel.SetActive(true);
    }

    // Show the Deck panel
    public void ShowDeck()
    {
        startMenuPanel.SetActive(false);
        deckPanel.SetActive(true);
    }

    // Close the Deck panel
    public void CloseDeck()
    {
        deckPanel.SetActive(false);
        startMenuPanel.SetActive(true);
    }
    public void ShowMainMenu()
    {
        menuCanvasGroup.alpha = 1f;
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;

        CloseDeck();
        CloseRules();
        startMenuPanel.SetActive(true);

    }
}
