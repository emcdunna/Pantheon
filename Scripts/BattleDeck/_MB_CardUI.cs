using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.BattleDeck
{
    public class _MB_CardUI : MonoBehaviour
    {
        private static string[] hotkeytext = new string[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" };

        public GameObject imagePrefab;

        public ObjectRearranger topRow, bottomRow;
        public Card card;
        public Text CardNameText;
        public Text HotKeyText;
        bool setup = false;
        int index;


        public void UpdateCardUI(int index, Card newCard)
        {
            this.index = index;
            bool cardChanged = (card.name != newCard.name);
            if (!setup)
            {
                topRow = new ObjectRearranger(60, 20, gameObject);
                bottomRow = new ObjectRearranger(60, -40, gameObject);
                setup = true;
            }
            
            card = newCard;

            HotKeyText.text = hotkeytext[index];
            if (card == null)
            {
                CardNameText.text = "";
                topRow.Clear();
                bottomRow.Clear();

            }
            else if (cardChanged)
            {
                CardNameText.text = card.name;
                topRow.Clear();
                bottomRow.Clear();

                int total_symbols = card.arrows + card.swords + card.shock + card.spears + card.shields
                    + card.evasion + card.banners + card.fear + card.scoutting;

                int symbols_so_far = 0;

                symbols_so_far += AddCardSymbol(__Engine.CardSymbol.arrow, card.arrows, symbols_so_far);
                symbols_so_far += AddCardSymbol(__Engine.CardSymbol.sword, card.swords, symbols_so_far);
                symbols_so_far += AddCardSymbol(__Engine.CardSymbol.shock, card.shock, symbols_so_far);
                symbols_so_far += AddCardSymbol(__Engine.CardSymbol.spear, card.spears, symbols_so_far);
                symbols_so_far += AddCardSymbol(__Engine.CardSymbol.shield, card.shields, symbols_so_far);
                symbols_so_far += AddCardSymbol(__Engine.CardSymbol.evasion, card.evasion, symbols_so_far);
                symbols_so_far += AddCardSymbol(__Engine.CardSymbol.banner, card.banners, symbols_so_far);
                symbols_so_far += AddCardSymbol(__Engine.CardSymbol.fear, card.fear, symbols_so_far);
                symbols_so_far += AddCardSymbol(__Engine.CardSymbol.scoutting, card.scoutting, symbols_so_far);

                if (card.cross_attack)
                {
                    symbols_so_far += AddCardSymbol(__Engine.CardSymbol.star, 1, symbols_so_far);
                }
                if (card.strike_back)
                {
                    symbols_so_far += AddCardSymbol(__Engine.CardSymbol.trap, 1, symbols_so_far);
                }

            }

        }


        // Adds the card Sprite
        int AddCardSymbol(__Engine.CardSymbol symbol, int times, int symbolCount)
        {
            if (times == 0)
            {
                return 0;
            }
            
            Sprite newsprite = null;

            _MB_BattlefieldManager battlefield = _MB_BattlefieldManager.Battlefield();

            switch (symbol)
            {
                case __Engine.CardSymbol.arrow:
                    newsprite = battlefield.arrow;
                    break;
                case __Engine.CardSymbol.sword:
                    newsprite = battlefield.sword;
                    break;
                case __Engine.CardSymbol.shock:
                    newsprite = battlefield.shock;
                    break;
                case __Engine.CardSymbol.spear:
                    newsprite = battlefield.spear;
                    break;
                case __Engine.CardSymbol.shield:
                    newsprite = battlefield.shield;
                    break;
                case __Engine.CardSymbol.evasion:
                    newsprite = battlefield.evasion;
                    break;
                case __Engine.CardSymbol.banner:
                    newsprite = battlefield.banner;
                    break;
                case __Engine.CardSymbol.fear:
                    newsprite = battlefield.fear;
                    break;
                case __Engine.CardSymbol.scoutting:
                    newsprite = battlefield.scoutting;
                    break;
                case __Engine.CardSymbol.armor:
                    newsprite = battlefield.armor;
                    break;
                case __Engine.CardSymbol.star:
                    newsprite = battlefield.star;
                    break;
                case __Engine.CardSymbol.trap:
                    newsprite = battlefield.trap;
                    break;
                default:
                    break;
            }
            
            
            for (int i = 0; i < times; i++)
            {
                symbolCount += 1;

                GameObject newSymbol = Object.Instantiate(imagePrefab);

                Image sr = newSymbol.GetComponent<Image>();
                
                sr.sprite = newsprite;
                sr.color = Color.gray;
                if (symbolCount <= 2)
                {
                    topRow.AddObject(newSymbol);
                } else
                {
                    bottomRow.AddObject(newSymbol);
                }
            }
                
            return times;

        }



        // Update is called once per frame
        void Update()
        {
            
        }
    }
}