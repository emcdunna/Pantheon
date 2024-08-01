using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.BattleDeck
{

    // The collection of battle cards the general has access to.
    [System.Serializable]
    public class Deck
    {

        public List<Card> cards = new List<Card>();
        public List<Card> deck = new List<Card>();
        public List<Card> discard_pile = new List<Card>();

        public Deck()
        {

        }

        public Stats GetCardProfile()
        {
            int swords = 0, shock = 0, arrows = 0, evasion = 0, spears = 0, shields = 0, fear = 0, banners = 0;

            foreach (Card card in cards)
            {
                swords += card.swords;
                shock += card.shock;
                arrows += card.arrows;
                evasion += card.evasion;
                spears += card.spears;
                shields += card.shields;
                fear += card.fear;
                banners += card.banners;
            }
            Stats stats = new Stats(Swords: swords,
                                    Shock: shock,
                                    Arrows: arrows,
                                    Spears: spears,
                                    Shields: shields,
                                    Fear: fear,
                                    Banners: banners,
                                    Evasion: evasion
                                    );
            return stats;
        }

        public void Shuffle()
        {
            foreach (Card card in discard_pile)
            {
                deck.Add(card);
            }
            discard_pile = new List<Card>();

            // Loop array
            for (int i = deck.Count - 1; i > 0; i--)
            {
                // Randomize a number between 0 and i (so that the range decreases each time)
                int rnd = UnityEngine.Random.Range(0, i);

                // Save the value of the current i, otherwise it'll overwrite when we swap the values
                Card temp = deck[i];

                // Swap the new and old values
                deck[i] = deck[rnd];
                deck[rnd] = temp;
            }
        }

        // Removes card from deck (to place into a hand) which means we lose track of the card
        public Card Draw()
        {

            if (deck.Count == 0)
            {
                Shuffle();
            }

            Card card = deck[0];
            deck.RemoveAt(0);
            return card;
            
        }

        // Takes a card from elsewhere 
        public void Discard(Card card)
        {
            discard_pile.Add(card);
        }

        public void AddCards(List<Card> new_cards)
        {
            //Debug.Log("Adding " + new_cards.Count + " new cards");
            foreach (Card card in new_cards)
            {
                cards.Add(card);
                Discard(card);
            }
            Shuffle();
        }

        public class Stats
        {
            public readonly int swords, shock, arrows, evasion, spears, shields, fear, banners;

            public Stats(int Swords = 0, int Shock = 0, int Arrows = 0, int Evasion = 0, int Spears = 0, int Shields = 0, int Fear = 0,
                int Banners = 0)
            {
                swords = Swords;
                shock = Shock;
                arrows = Arrows;
                evasion = Evasion;
                spears = Spears;
                shields = Shields;
                fear = Fear;
                banners = Banners;
            }
        }
    }
}
