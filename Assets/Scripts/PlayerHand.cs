using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerHand : MonoBehaviour
{
    public List<Card> hand = new List<Card>();
    public int handValue = 0;
    public GameObject cardPrefab;
    public Transform deckTransform;
    public Sprite cardBackSprite; 

    public bool isDealing = false;
    protected Queue<Card> cardsToDeal = new Queue<Card>();


    public bool HasBlackjack()
    {
        if (hand.Count == 2)
        {
            bool hasAce = false;
            bool hasTenValueCard = false;

            foreach (Card card in hand)
            {
                if (card.value == 1)
                {
                    hasAce = true;
                }
                else if (card.value == 10)
                {
                    hasTenValueCard = true;
                }
            }

            return hasAce && hasTenValueCard;
        }

        return false;
    }


    public void RecalculateHandValue()
    {
        handValue = 0;
        int aces = 0;

        foreach (Card card in hand)
        {
            if (card.isFlipped)
            {
                handValue += card.value;

                if (card.value == 1)
                {
                    aces++;
                }
            }
        }

        while (aces > 0 && handValue <= 11) 
        {
            handValue += 10; 
            aces--;
        }
    }


    public void AddCardToHand(Card card)
    {
        hand.Add(card);

        RecalculateHandValue();

        cardsToDeal.Enqueue(card);

        if (!isDealing)
        {
            StartCoroutine(DealCardsSequentially(false));
        }

        FindObjectOfType<BlackjackGameManager>().UpdateHandValueTexts();
    }

    protected virtual IEnumerator DealCardsSequentially(bool dealerIsPlaying)
    {
        //Debug.Log("Coroutine DealCardsSequentially started for player");
        isDealing = true;

        int cardIndex = hand.Count-1;  // Track the index of the card being dealt
        while (cardsToDeal.Count > 0)
        {
            Card card = cardsToDeal.Dequeue();

            // Instantiate the card object
            GameObject cardObject = Instantiate(cardPrefab, transform);
            RectTransform cardRect = cardObject.GetComponent<RectTransform>();

            // Set the card data on the CardBehavior component
            CardBehavior cardBehavior = cardObject.GetComponent<CardBehavior>();
            cardBehavior.cardData = card;  

            // Position the card
            cardRect.position = deckTransform.position;
            float cardWidth = 100f;
            float xOffset = cardIndex * cardWidth;
            Vector3 finalPosition = new Vector3(xOffset, 0, 0);
            cardObject.GetComponent<Image>().sprite = CardBackManager.instance.GetCardBack();

            yield return StartCoroutine(MoveCardToPositionAndFlip(cardRect, finalPosition, card));

            cardIndex++;  
        }
        
        isDealing = false;
        //Debug.Log("Coroutine DealCardsSequentially finished for player");
    }


    protected Vector3 CalculateCardPosition()
    {
        float cardWidth = 100f;
        float xOffset = hand.Count * cardWidth; 
        return new Vector3(xOffset, 0, 0); 
    }

    protected IEnumerator MoveCardToPosition(RectTransform cardRect, Vector3 targetPosition)
    {
        //Debug.Log("Coroutine MoveCardToPosition started on: " + gameObject.name);

        Vector3 startPosition = cardRect.localPosition;
        float duration = 1f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            cardRect.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        cardRect.localPosition = targetPosition;
        //Debug.Log("Coroutine MoveCardToPosition finished on: " + gameObject.name);
    }

    protected IEnumerator MoveCardToPositionAndFlip(RectTransform cardRect, Vector3 targetPosition, Card card)
    {
        yield return StartCoroutine(MoveCardToPosition(cardRect, targetPosition));
        yield return StartCoroutine(FlipCard(cardRect, card));
    }

    protected IEnumerator FlipCard(RectTransform cardRect, Card card)
    {
        Image cardImage = cardRect.GetComponent<Image>();
        float flipDuration = 0.4f;
        float elapsed = 0f;

        float initialScaleY = cardRect.localScale.y;
        float initialScaleZ = cardRect.localScale.z;

        // Flip halfway (scale X to 0)
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / (flipDuration / 2));
            cardRect.localScale = new Vector3(1 - t, initialScaleY, initialScaleZ);
            yield return null;
        }

        cardImage.sprite = card.sprite;

        elapsed = 0f;

        // Flip the other half (scale X back to 3)
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / (flipDuration / 2));
            cardRect.localScale = new Vector3(t * 3, initialScaleY, initialScaleZ);
            yield return null;
        }

        card.isFlipped = true;
        RecalculateHandValue();
        FindObjectOfType<BlackjackGameManager>().UpdateHandValueTexts();
    }

    public void ResetHand()
    {
        //StopAllCoroutines();
        //Debug.Log("Stopped coroutines");
        hand.Clear();
        handValue = 0;

        foreach (Transform child in transform)
        {
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public bool IsBusted()
    {
        return handValue > 21;
    }

    public bool Is21()
    {
        return handValue == 21;
    }
}
