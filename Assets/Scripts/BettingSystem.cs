using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BettingSystem : MonoBehaviour
{
    public Player player = new Player(); // Reference to the player
    public TMP_Text balanceText; // Display player's balance
    public TMP_Text currentBetText; // Display current bet
    public BlackjackGameManager blackjackGameManager;

    public int currentBet = 0; // Track the current bet amount
    public CanvasGroup bettingPanelCanvasGroup; // The CanvasGroup of the betting panel for fading

    private float fadeDuration = 0.5f; // Duration of fade in/out

    public CanvasGroup betTextCanvasGroup;
    public CanvasGroup balanceTextCanvasGroup;

    void Start()
    {
        UpdateUI();
        bettingPanelCanvasGroup.alpha = 0f;
        balanceTextCanvasGroup.alpha = 0f;
        betTextCanvasGroup.alpha = 0f;
    }

    // Method to update the displayed balance and bet
    public void UpdateUI()
    {
        balanceText.text = "Balance: $" + player.balance;
        currentBetText.text = "Current Bet: $" + currentBet;
    }

    // Method to handle chip betting
    public void PlaceChipBet(int chipValue)
    {
        if (player.PlaceBet(chipValue))
        {
            currentBet += chipValue;
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough balance!");
        }
    }

    // Method to confirm the bet and start the round
    public void ConfirmBet()
    {
        if (currentBet > 0)
        {
            Debug.Log("Bet confirmed: $" + currentBet);
            blackjackGameManager.OnBetPlaced();  // Notify the game that betting is done
            StartCoroutine(FadeOutBettingPanel()); // Fade out the betting panel
        }
        else
        {
            Debug.Log("No bet placed. Please place a bet before confirming.");
        }
    }

    // Method to clear the bet (reset after a round)
    public void ClearBet()
    {
        currentBet = 0;
        UpdateUI();
    }

    public void CancelBet()
    {
        player.AddWinnings(currentBet);
        currentBet = 0;
        UpdateUI();
    }

    // Fade in the betting panel
    public IEnumerator FadeInBettingPanel()
    {
        blackjackGameManager.canOpenMenu = false;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            bettingPanelCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            bettingPanelCanvasGroup.interactable = true; // Enable interaction while fading in
            bettingPanelCanvasGroup.blocksRaycasts = true; // Allow input to pass through
            yield return null;
        }
        bettingPanelCanvasGroup.alpha = 1f;
        blackjackGameManager.canOpenMenu = true;
    }

    // Fade out the betting panel
    public IEnumerator FadeOutBettingPanel()
    {
        blackjackGameManager.canOpenMenu = false;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            bettingPanelCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            bettingPanelCanvasGroup.interactable = false; // Disable interaction while fading out
            bettingPanelCanvasGroup.blocksRaycasts = false; // Block input while fading out
            yield return null;
        }
        bettingPanelCanvasGroup.alpha = 0f;
        blackjackGameManager.canOpenMenu = true;
    }

    // Method to payout the winnings
    public void Payout(bool playerWon)
    {
        if (playerWon)
        {
            player.AddWinnings(currentBet * 2);  // Double the bet if the player wins
        }
        ClearBet();  // Clear the bet after payout
        UpdateUI();

        // Fade in the betting panel again after the round is over
        //StartCoroutine(FadeInBettingPanel());
    }

    public IEnumerator FadeInText(float duration)
    {
        if (balanceTextCanvasGroup.alpha != 1 || betTextCanvasGroup.alpha != 1)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                balanceTextCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                betTextCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                yield return null;
            }
            balanceTextCanvasGroup.alpha = 1f;
            betTextCanvasGroup.alpha = 1f;
        }
    }
}
