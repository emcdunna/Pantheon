using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controls showing which cards are in the player's hand, where they are on screen, and card sprites
namespace Assets.Scripts.BattleDeck
{
    public class _MB_HandManager : MonoBehaviour
    {
        public List<GameObject> handcards = new List<GameObject>();

        public int hand_size = 5;
        public Player player;

        public GameObject cardslot_prefab;

        public readonly static int DEFAULT_WIDTH = 115;
        public readonly static int DEFAULT_HEIGHT = 160;
        public readonly static int DEFAULT_SPACING = 5;


        // Update is called once per frame
        void Update()
        {
            
            UpdateCardSlots();
            UpdateCardUIs();
            UpdateHighlightedCard();
        }


        // Update how many card slots are visible
        void UpdateCardSlots()
        {
            
            hand_size = player.hand.Count;

            // Create new slots if necessary
            if (handcards.Count < hand_size)
            {
                int newSlots = hand_size - handcards.Count;
                for (int i = 0; i < newSlots; i++)
                {
                    AddNewSlot();
                }
            }

            // Delete slots if there are too many
            else if (handcards.Count > hand_size)
            {
                int extraSlots = handcards.Count - hand_size;
                for (int i = 0; i < extraSlots; i++)
                {
                    RemoveSlot(handcards.Count - 1);
                }
            }
        }

        void ManualDiscard(int index)
        {

        }

        void CardClicked(int index)
        {
            player.SetSelectedCard(index);
        }

        void UpdateHighlightedCard()
        {
            int index = 0;
            if (player.hand.Count == 0)
            {
                return;
            }

            // When a card is selected, gray out the others
            if (player.GetSelectedCard() != null)
            {
                int selected_index = player.GetSelectedIndex();
                if (selected_index <= handcards.Count - 1)
                {

                    foreach (GameObject go in handcards)
                    {
                        // All non selected cards
                        Card card = player.hand[index];
                        if (card != player.GetSelectedCard())
                        {

                            if (player.HasAlreadyPlayedCard(card))
                            {
                                go.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                            }
                            else
                            {
                                go.GetComponent<Image>().color = Color.gray;
                            }

                        }
                        // Selected card
                        else
                        {
                            go.GetComponent<Image>().color = Color.white;
                        }
                        index += 1;
                    }
                    return;
                }
            }
            // When no card is selected, all are white except already played cards
            foreach (GameObject go in handcards)
            {   
                Card card = player.hand[index];
                if (player.HasAlreadyPlayedCard(card))
                {
                    go.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
                }
                else
                {
                    go.GetComponent<Image>().color = Color.white;
                }

                index += 1;
            }

        }

        // Creates a new cardslot object and adds it to this set
        void AddNewSlot()
        {
            GameObject newSlot = Object.Instantiate(cardslot_prefab);
            int index = handcards.Count;
            handcards.Add(newSlot);
            newSlot.transform.parent = transform;
            Button btn = newSlot.GetComponent<Button>();
            btn.onClick.AddListener(delegate { CardClicked(index); });

        }

        // Removes a cardslot object and deletes the reference
        void RemoveSlot(int index = 0)
        {
            GameObject slot = handcards[index];
            Object.Destroy(slot);
            handcards.RemoveAt(index);
        }

        // Display the most recent set of cards available
        void UpdateCardUIs()
        {
            int i = 0;
            Card card = null;
            foreach (GameObject go in handcards)
            {
                _MB_CardUI cardUI = go.GetComponent<_MB_CardUI>();
                cardUI.UpdateCardUI(i, player.hand[i]);
                i += 1;
            }
        }
    }
}