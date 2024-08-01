using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BattleDeck
{

    [System.Serializable]
    public class Player
    {

        public readonly string name;
        public readonly Army army;
        public readonly General general;
        public readonly Deck deck;
        public readonly Faction faction;

        public List<Card> hand;


        List<Play> plays = new List<Play>();

        Card selected_card = null;
        int selected_index = 0;

        public Player(string Name, Army Army, General General, Faction Faction)
        {
            name = Name;
            deck = new Deck();
            army = Army;
            deck.AddCards(army.GetCards());
            general = General;
            deck.AddCards(general.GetCards());
            faction = Faction;
            deck.AddCards(faction.GetCards());
            hand = new List<Card>();
            
            FillHand();
            
        }

        public override string ToString()
        {
            return name;
        }

        public Army GetArmy()
        {
            return army;
        }

        public int GetSelectedIndex()
        {
            return selected_index;
        }

        public void SetSelectedCard(Card card)
        {
            selected_card = card;

            for (int i = 0; i < hand.Count; i++)
            {
                if (card == hand[i])
                {
                    selected_index = i;
                }
            }
        }

        public void SetSelectedCard(int index)
        {
            if (index <= hand.Count - 1)
            {
                selected_card = hand[index];
                selected_index = index;
            }
            else
            {
                selected_card = null;
                selected_index = 0;
            }

        }

        public void ManualDiscard()
        {
            if(selected_card != null)
            {
                Debug.Log("Discarding " + selected_card + " at index " + selected_index);
                deck.Discard(selected_card);
                hand.RemoveAt(selected_index);
                selected_card = null;
            }
            else
            {
                Debug.Log("Not discarding because no card is selected.");
            }
            
        }

        public Card GetSelectedCard()
        {
            return selected_card;
        }

        // Fills hand with as many cards are needed to reach maximum hand limit
        public void FillHand()
        {
            int new_cards = general.hand_limit - hand.Count;

            for (int i = 0; i < new_cards; i++)
            {
                Card card = deck.Draw();
                hand.Add(card);
            }
        }

        // Allows the player to play a card to a sector
        public bool NewPlay(Card card, Sector.Type sector)
        {
            Sector battlesector = army.GetBattleSector(sector);

            foreach(Play oldPlay in plays)
            {
                if(oldPlay.card == card)
                {
                    Debug.Log("Cancelling old play. ");
                    CancelPlay(oldPlay);
                }
            }

            Play p = new Play(this, card, battlesector);

            if (p.GetIsValid())
            {
                plays.Add(p);

                // TODO: smarter way of knowing when to reveal
                if (this == _MB_BattleRunner.Runner().GetScenario().player1)
                {
                    _MB_BattlefieldManager.Battlefield().SetPlaySprites(p);
                }

                return true;
            }
            else
            {
                return false;
            }

        }


        public List<Play> GetPlays()
        {
            return plays;
        }


        public Play GetPlay(Sector.Type sector)
        {
            Sector battlesector = army.GetBattleSector(sector);

            foreach (Play p in plays)
            {
                if (p.battleSector.sector == sector)
                {
                    return p;
                }
            }
            return null;
        }

        public Play GetPlayOrDelegate(Sector.Type sector)
        {
            Sector battlesector = army.GetBattleSector(sector);
            if (battlesector.HasAlreadyBroken())
            {
                return new Play(this, new Card("Nothing", Card.CardType.Base), battlesector);
            }
            
            Play p = GetPlay(sector);
            if (p == null)
            {
                Play newP = new Play(this, new Card("Delegate", Card.CardType.Delegate), battlesector);
                plays.Add(newP);
                _MB_BattlefieldManager.Battlefield().SetPlaySprites(newP);
                return newP;
            }
            else
            {
                return p;
            }
            
        }


        public void ClearPlays()
        {
            plays.Clear();
        }


        public int CountUnplayedSectors()
        {
            return GetUnplayedSectors().Count;
        }

        public List<Sector.Type> GetUnplayedSectors()
        {
            List<Sector.Type> unplayed = new List<Sector.Type>();
            foreach (Sector.Type sector in _MB_BattleRunner.Runner().GetScenario().activeSectors)
            {
                Sector battlesector = army.GetBattleSector(sector);
                if (battlesector.HasAlreadyBroken())
                {
                    // don't add
                }
                else
                {
                    Play p = GetPlay(sector);
                    if (p == null)
                    {
                        unplayed.Add(sector);
                        //Debug.Log(player.name + " has not yet played " + sector);
                    }
                    else
                    {
                        //Debug.Log(player.name + " has already played " + sector);
                    }

                }

            }
            return unplayed;
        }

        public void CancelPlay(Play play)
        {
            plays.Remove(play);
            _MB_BattlefieldManager.Battlefield().RemovePlaySprites(play);
        }

        // Cancels plays for player1 and returns cards to the hand
        public void CancelPlays()
        {
            List<Play> removed = new List<Play>();
            foreach (Play play in plays)
            {
                if (play != null)
                {
                    removed.Add(play);
                }
            }
            foreach (Play play in removed)
            {
                CancelPlay(play);
            }

        }

        public bool HasAlreadyPlayedCard(Card card)
        {
            foreach(Play play in plays)
            {
                if (play.card == card)
                {
                    return true;
                }
            }
            return false;
        }


        // Deploy a unit to a sector
        public void Deploy(Sector.Type sector, Battalion battalion)
        {

            army.DeployUnit(sector, battalion);
        }


        

    }
}
