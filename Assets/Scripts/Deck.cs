using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Sprite> cardSprites; // All card sprites imported.
    private List<Card> deck = new List<Card>(); // Current deck
    private List<Card> originalDeck = new List<Card>(); // Original deck to refill from

    void Awake() // Use Awake instead of Start for initializing the deck
    {
        InitializeDeck();
        ShuffleDeck();
        Debug.Log("Deck initialized with " + deck.Count + " cards in Awake.");
    }

    void InitializeDeck()
    {
        string[] suits = { "Hearts", "Diamonds", "Spades", "Clubs" };
        int[] values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 }; // Aces to Kings

        int spriteIndex = 0;
        foreach (string suit in suits)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (spriteIndex >= cardSprites.Count)  // Safety check to avoid out-of-range errors
                {
                    Debug.LogError("Not enough sprites in cardSprites list for the number of cards being initialized.");
                    return;
                }

                Card newCard = new Card(suit, values[i], cardSprites[spriteIndex]);
                originalDeck.Add(newCard); // Populate the original deck
                spriteIndex++;
            }
        }

        ResetDeck(); // Copy original deck into current deck
        //Debug.Log("Deck successfully initialized with " + deck.Count + " cards.");
    }

    void ResetDeck()
    {
        // Clear current deck and copy from the original deck
        deck.Clear();
        deck.AddRange(originalDeck);
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

    }

    public Card DealCard()
    {
        if (deck.Count == 0)
        {
            //Debug.Log("Deck is empty, reshuffling and refilling.");
            ResetDeck();  // Reset and refill the deck from the original deck
            ShuffleDeck();
        }

        Card dealtCard = deck[0];
        deck.RemoveAt(0);
        return dealtCard;
    }

    public int DeckSize()
    {
        return deck.Count;
    }

    public void ShuffleForNewRound()
    {
        if (deck.Count == 0)
        {
            ResetDeck();  // Refill the deck if it's empty
        }
        ShuffleDeck(); // Shuffle remaining or refilled cards
    }
}
