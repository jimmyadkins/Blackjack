using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string suit;
    public int value; // Face cards (J, Q, K) will be 10, and Ace can be 1 or 11.
    public Sprite sprite; // The image of the card.
    public bool isFlipped; // is the card face up

    public Card(string suit, int value, Sprite sprite)
    {
        this.suit = suit;
        this.value = value;
        this.sprite = sprite;
        this.isFlipped = false;
    }
}

