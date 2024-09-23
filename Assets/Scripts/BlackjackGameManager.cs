using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlackjackGameManager : MonoBehaviour
{
    public PlayerHand playerHand;
    public DealerHand dealerHand; 
    private Deck deck;

    public Button hitButton;
    public Button standButton;
    public Button restartButton;

    public TMP_Text resultText;
    public TMP_Text playerHandValueText;
    public TMP_Text dealerHandValueText;

    public CanvasGroup playerPointsCanvasGroup; 
    public CanvasGroup dealerPointsCanvasGroup;

    public BettingSystem bettingSystem;
    public Player player;

    public bool canOpenMenu = true;

    void Awake()
    {
        deck = FindObjectOfType<Deck>();
    }

    private void Start()
    {
        canOpenMenu = true;
        playerPointsCanvasGroup.alpha = 0f;
        dealerPointsCanvasGroup.alpha = 0f;
        restartButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
        //deck.ShuffleForNewRound();
        //playerHand.ResetHand();
        //dealerHand.ResetHand();
        //StartRound();
    }

    public void StartRound()
    {
        StartCoroutine(bettingSystem.FadeInText(1f));
        canOpenMenu = true;
        restartButton.interactable = true;
        Debug.Log("Please place your bet to start the round.");
        int playerBalance = bettingSystem.player.balance;
        ResetGame();

        

        if (playerBalance <= 0)
        {
            // Display the result message and return to the main menu
            resultText.gameObject.SetActive(true);
            resultText.text = "You're out of money! Returning to the main menu...";

            // Disable betting and gameplay
            hitButton.interactable = false;
            standButton.interactable = false;
            restartButton.interactable = false;

            // Optionally, wait for a few seconds before going back to the menu
            StartCoroutine(ReturnToMainMenuAfterDelay(2f)); // Wait 2 seconds
        }
        else
        {
            // Proceed with starting the round as usual
            bettingSystem.ClearBet();
            bettingSystem.UpdateUI(); // Update balance UI before the new round

            // Fade in the betting panel to allow the player to place a bet
            StartCoroutine(bettingSystem.FadeInBettingPanel());

            // Disable the hit/stand buttons until the bet is placed
            hitButton.interactable = false;
            standButton.interactable = false;
        }
    }

    private IEnumerator ReturnToMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        resultText.gameObject.SetActive(false);
        bettingSystem.betTextCanvasGroup.alpha = 0f;
        bettingSystem.balanceTextCanvasGroup.alpha = 0f;
        FindObjectOfType<MenuManager>().ShowMainMenu();  // Go back to the main menu
        bettingSystem.player.ResetBalance();
    }

    public void OnBetPlaced()
    {
        if (bettingSystem.currentBet > 0)
        {
            DealCards(); // Start the game once a bet is placed
        }
    }

    private Sprite GetCardSprite(string cardName)
    {
        // Assuming you have a way to retrieve card sprites by their name
        return deck.cardSprites.Find(sprite => sprite.name == cardName);
    }
    private void DealCards()
    {
        canOpenMenu = false;
        playerPointsCanvasGroup.alpha = 0f;
        dealerPointsCanvasGroup.alpha = 0f;
        //StopAllCoroutines();
        deck.ShuffleForNewRound();

        restartButton.gameObject.SetActive(false);
        playerHand.ResetHand();
        dealerHand.ResetHand();

        //// Test case: Give the player a Blackjack (Ace and 10-value card)
        //Card aceOfSpades = new Card("Spades", 1, GetCardSprite("spades__a"));
        //Card tenOfHearts = new Card("Hearts", 10, GetCardSprite("hearts__10"));

        //playerHand.AddCardToHand(aceOfSpades);
        //playerHand.AddCardToHand(tenOfHearts);
        //dealerHand.AddCardToHand(aceOfSpades);
        //dealerHand.AddCardToHand(tenOfHearts);

        playerHand.AddCardToHand(deck.DealCard());
        dealerHand.AddCardToHand(deck.DealCard());
        StartCoroutine(FadeIn(playerPointsCanvasGroup, 1f));
        StartCoroutine(FadeIn(dealerPointsCanvasGroup, 1f));
        playerHand.AddCardToHand(deck.DealCard());
        dealerHand.AddCardToHand(deck.DealCard());

        hitButton.interactable = false;
        standButton.interactable = false;
        hitButton.gameObject.SetActive(true);
        standButton.gameObject.SetActive(true);


        resultText.gameObject.SetActive(false); 
        UpdateHandValueTexts();

        StartCoroutine(EnableMenuAfterDealing());

        if (playerHand.HasBlackjack())
        {
            StartCoroutine(PlayerStandAfterDealing());
        }
        else
        {
            StartCoroutine(EnableHitButtonAfterDealing());
        }
    }

    private IEnumerator EnableMenuAfterDealing()
    {
        // Wait for all cards to be dealt
        while (playerHand.isDealing || dealerHand.isDealing)
        {
            yield return null;
        }

        // Allow the menu to be accessed once all cards are dealt
        canOpenMenu = true;
    }

    private IEnumerator EnableHitButtonAfterDealing()
    {
        //canOpenMenu = false;
        while (playerHand.isDealing)
        {
            yield return null;
        }

        hitButton.interactable = true;
        standButton.interactable = true;
        //canOpenMenu = true;
    }

    private IEnumerator PlayerStandAfterDealing()
    {
        //canOpenMenu = false;
        while (playerHand.isDealing)
        {
            yield return null;
        }

        hitButton.interactable = false;
        standButton.interactable = false;
        PlayerStand();
        //canOpenMenu = true;
    }

    public void UpdateHandValueTexts()
    {
        playerHandValueText.text = playerHand.handValue.ToString();
        dealerHandValueText.text = dealerHand.handValue.ToString();
    }

    public void PlayerHit()
    {
        canOpenMenu = false;
        hitButton.interactable = false;
        standButton.interactable = false;
        StartCoroutine(DealPlayerCard());
    }

    private IEnumerator DealPlayerCard()
    {
        playerHand.AddCardToHand(deck.DealCard());

        while (playerHand.isDealing)
        {
            yield return null;
        }

        hitButton.interactable = true;
        standButton.interactable = true;
        canOpenMenu = true;

        if (playerHand.IsBusted())
        {
            dealerHand.ShowHand(true);

            EndRound("Player Busts! Dealer Wins!");
        }

        if (playerHand.Is21())
        {
            PlayerStand();
        }
    }

    public void PlayerStand()
    {

        hitButton.interactable = false;
        standButton.interactable = false;

        dealerHand.ShowHand(true);

        StartCoroutine(DealerPlaySequence());
    }

    private IEnumerator DealerPlaySequence()
    {
        yield return StartCoroutine(dealerHand.DealerPlayCoroutine());

        if (dealerHand.IsBusted())
        {
            EndRound("Dealer Busts! Player Wins!");
        }
        else
        {
            DetermineWinner();
        }
    }

    void DetermineWinner()
    {
        if (playerHand.handValue > dealerHand.handValue)
        {
            EndRound("Player Wins!");
        }
        else if (playerHand.handValue < dealerHand.handValue)
        {
            EndRound("Dealer Wins!");
        }
        else
        {
            EndRound("It's a Tie!");
        }
    }

    void EndRound(string result)
    {
        UpdateHandValueTexts();
        Debug.Log("End of Round: " + result);

        resultText.gameObject.SetActive(true);
        resultText.text = result;

        hitButton.interactable = false;
        standButton.interactable = false;

        bool playerWon = result == "Player Wins!" || 
            result == "Dealer Busts! Player Wins!";

        bool playerTie = result == "It's a Tie!";

        bettingSystem.Payout(playerWon, playerTie);
        restartButton.gameObject.SetActive(true);

    }

    public void RestartGame()
    {
        StartRound(); 
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float elapsed = 0f;
        canvasGroup.alpha = 0f;  // Start invisible

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);  // Smoothly transition alpha from 0 to 1
            yield return null;
        }

        canvasGroup.alpha = 1f;  // Ensure alpha is fully 1 (visible)
    }

    public void ResetGame()
    {
        // Reset the hands and game state
        playerHand.ResetHand();
        dealerHand.ResetHand();

        // Reset UI elements
        playerPointsCanvasGroup.alpha = 0f;  // Hide the score UI
        dealerPointsCanvasGroup.alpha = 0f;  // Hide the score UI

        resultText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        // Disable hit/stand buttons
        hitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);

        bettingSystem.bettingPanelCanvasGroup.alpha = 0f;

    }

    public void OpenMenu()
    {
        if (!canOpenMenu)
        {
            Debug.Log("Menu access is not allowed until the cards are fully dealt.");
            return;  // Do nothing if menu access is not allowed yet
        }

        // Add your menu logic here, such as pausing or going back to the main menu
        ResetGame();

        bettingSystem.balanceTextCanvasGroup.alpha = 0f;
        bettingSystem.player.ResetBalance();
        bettingSystem.betTextCanvasGroup.alpha = 0f;
        FindObjectOfType<MenuManager>().ShowMainMenu();
    }

}
