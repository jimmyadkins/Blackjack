using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DealerHand : PlayerHand
{
    private Deck deck; // Reference to the deck

    void Start()
    {
        // Initialize deck reference by finding it in the scene
        deck = FindObjectOfType<Deck>();
    }

    // Dealer plays by hitting until they have 17 or higher.
    public IEnumerator DealerPlayCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        while (handValue < 17)
        {
            AddCardToHand(deck.DealCard());

            // Wait until the card is dealt
            while (isDealing)
            {
                yield return null;
            }

            // Recalculate the hand value after dealing a card
            RecalculateHandValue();

            // Debugging information
            Debug.Log("Dealer's hand value: " + handValue);
        }
    }



    // Show dealer's cards, but hide the first one if revealHiddenCard is false
    public void ShowHand(bool revealHiddenCard)
    {
        if (transform.childCount == 0)
        {
            Debug.LogError("No cards have been dealt to the dealer.");
            return;
        }

        // Only show or flip the first card
        GameObject firstCardObject = transform.GetChild(0).gameObject;  // Access the first card
        Image firstCardImage = firstCardObject.GetComponent<Image>();

        if (!revealHiddenCard)
        {
            // First card stays facedown
            firstCardImage.sprite = cardBackSprite;
        }
        else
        {
            // Reveal the first card
            RectTransform cardRect = firstCardObject.GetComponent<RectTransform>();
            Card firstCard = firstCardObject.GetComponent<CardBehavior>().cardData;
            Debug.Log("First card data: " + firstCard.suit + firstCard.value);
            StartCoroutine(FlipCard(cardRect, firstCard));
        }

        FindObjectOfType<BlackjackGameManager>().UpdateHandValueTexts();
    }



    // Coroutine to deal cards sequentially
    protected override IEnumerator DealCardsSequentially(bool dealerIsPlaying)
    {
        //Debug.Log("Coroutine DealCardsSequentially started for dealer");
        isDealing = true;

        while (cardsToDeal.Count > 0)
        {
            Card card = cardsToDeal.Dequeue();

            // Instantiate the card prefab as a child of DealerHand
            GameObject cardObject = Instantiate(cardPrefab, transform);
            RectTransform cardRect = cardObject.GetComponent<RectTransform>();
            CardBehavior cardBehavior = cardObject.GetComponent<CardBehavior>();
            cardBehavior.cardData = card;
            // Set the card's initial position to the deck's position
            cardRect.position = deckTransform.position;

            float cardWidth = 100f;
            float xOffset = (hand.Count - 1) * cardWidth;  // Calculate position based on hand count
            Vector3 finalPosition = new Vector3(xOffset, 0, 0);
            cardObject.GetComponent<Image>().sprite = CardBackManager.instance.GetCardBack();
            if (hand.Count == 1)
            {
                yield return StartCoroutine(MoveCardToPosition(cardRect, finalPosition));
            }
            else
            {
                yield return StartCoroutine(MoveCardToPositionAndFlip(cardRect, finalPosition, card));
            }
        }

        isDealing = false;
        //Debug.Log("Coroutine DealCardsSequentially finished for dealer");
    }


    //// Calculate the final position of each card in the dealer's hand
    //private Vector3 CalculateCardPosition()
    //{
    //    float cardWidth = 100f;  // Width of each card
    //    int cardIndex = transform.childCount - 1;  // Use the child index to calculate position
    //    float xOffset = cardIndex * cardWidth;  // Space cards horizontally
    //    return new Vector3(xOffset, 0, 0);  // Return the final position
    //}
}
