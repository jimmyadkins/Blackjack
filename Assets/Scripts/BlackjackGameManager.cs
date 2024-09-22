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
        canOpenMenu = false;
        playerPointsCanvasGroup.alpha = 0f;
        dealerPointsCanvasGroup.alpha = 0f;
        //StopAllCoroutines();
        deck.ShuffleForNewRound();

        restartButton.gameObject.SetActive(false);
        playerHand.ResetHand();
        dealerHand.ResetHand();

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
            hitButton.interactable = false;
            standButton.interactable = false;
            PlayerStand();
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

    public void UpdateHandValueTexts()
    {
        playerHandValueText.text = playerHand.handValue.ToString();
        dealerHandValueText.text = dealerHand.handValue.ToString();
    }

    public void PlayerHit()
    {
        canOpenMenu = false;
        hitButton.interactable = false;
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
        FindObjectOfType<MenuManager>().ShowMainMenu();
    }

}
