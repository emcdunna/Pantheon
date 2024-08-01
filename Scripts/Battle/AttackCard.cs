using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackCard
{
    
    public enum Type { Hit, CriticalHit, Miss };
    public Type type = Type.Hit;
    public float damage_factor = 1.0f;
    public int impact = 0;  // extra pushing force for the unit due to the attack
    public string Reason = "Default Card"; // TODO: Keep track of why each +/- card is included

    public AttackCard(Type type)
    {
        switch (type)
        {
            case Type.Hit:
                this.damage_factor = 1f;
                this.impact = 0;
                break;
            case Type.CriticalHit:
                this.damage_factor = 2f;
                this.impact = 1;
                break;
            case Type.Miss:
                this.damage_factor = 0f;
                this.impact = -1;
                break;
            default:
                break;
        }
    }

    override public string ToString()
    {
        return "AttackCard " + this.type.ToString();
    }
}

public class AttackDeck
{
    public static int MAX_CARDS_DRAWN = 5;
    List<AttackCard> Cards = new List<AttackCard>();
    int DrawnCards = 0;
    public int Advantages = 0;
    public int Disadvantages = 0;
    public int BaseCards = 0;

    public AttackDeck(int base_cards=6)
    {
        BaseCards = base_cards;
        // Add regular attacks (HITS)
        AddCards(AttackCard.Type.Hit, base_cards);
    }

    void AddCard(AttackCard card)
    {
        this.Cards.Add(card);
    }

    void AddCards(AttackCard.Type type, int number)
    {
        for (int i = 0; i < number; i++)
        {
            this.AddCard(new AttackCard(type));
        }
    }

    // resets the deck to have a different number of advantages and disadvantages
    public void Update(int advantages, int disadvantages)
    {
        if (advantages != this.Advantages || disadvantages != this.Disadvantages)
        {
            Cards = new List<AttackCard>();
            AddCards(AttackCard.Type.Hit, BaseCards);
            AddCards(AttackCard.Type.CriticalHit, advantages);
            AddCards(AttackCard.Type.Miss, disadvantages);
            this.Advantages = advantages;
            this.Disadvantages = disadvantages;
            DrawnCards = 0;
            this.Shuffle();
        }
    }

    public void Shuffle()
    {
        // Loop array
        for (int i = Cards.Count - 1; i > 0; i--)
        {
            // Randomize a number between 0 and i (so that the range decreases each time)
            int rnd = UnityEngine.Random.Range(0, i);

            // Save the value of the current i, otherwise it'll overwrite when we swap the values
            AttackCard temp = Cards[i];

            // Swap the new and old values
            Cards[i] = Cards[rnd];
            Cards[rnd] = temp;
        }
    }

    public AttackCard Draw()
    {
        AttackCard topCard = this.Cards[0];
        Cards.Remove(topCard); // remove it from the top of the pile
        Cards.Add(topCard); // add it to the bottom of the pile.

        if (DrawnCards >= MAX_CARDS_DRAWN)
        {
            DrawnCards = 0;
            this.Shuffle(); // Every few cards that are drawn, shuffle the deck again.
        }
        return topCard;
    }
}