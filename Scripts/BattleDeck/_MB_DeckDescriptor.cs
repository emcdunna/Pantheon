using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BattleDeck
{
    public class _MB_DeckDescriptor : MonoBehaviour
    {
        public UI_Bar swords, arrows, shock, evason, shields, spears, special;
        Deck deck_to_show;
        Deck.Stats deck_stats;
        int deck_size = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetDeck(Deck deck)
        {
            deck_to_show = deck;
            deck_stats = deck.GetCardProfile();
            deck_size = deck.cards.Count;

            swords.SetHealthBarImmediate(CalculateBarStatus(__Engine.CardSymbol.sword));
            arrows.SetHealthBarImmediate(CalculateBarStatus(__Engine.CardSymbol.arrow));
            shock.SetHealthBarImmediate(CalculateBarStatus(__Engine.CardSymbol.shock));
            evason.SetHealthBarImmediate(CalculateBarStatus(__Engine.CardSymbol.evasion));
            shields.SetHealthBarImmediate(CalculateBarStatus(__Engine.CardSymbol.shield));
            spears.SetHealthBarImmediate(CalculateBarStatus(__Engine.CardSymbol.spear));
            special.SetHealthBarImmediate(
                CalculateBarStatus(__Engine.CardSymbol.star) + CalculateBarStatus(__Engine.CardSymbol.trap));
        }

        float CalculateBarStatus(__Engine.CardSymbol symbol)
        {
            int factor = 0;
            switch (symbol)
            {
                case __Engine.CardSymbol.sword:
                    factor = deck_stats.swords;
                    break;
                case __Engine.CardSymbol.arrow:
                    factor = deck_stats.arrows;
                    break;
                case __Engine.CardSymbol.shock:
                    factor = deck_stats.shock;
                    break;
                case __Engine.CardSymbol.evasion:
                    factor = deck_stats.evasion;
                    break;
                case __Engine.CardSymbol.shield:
                    factor = deck_stats.shields;
                    break;
                case __Engine.CardSymbol.spear:
                    factor = deck_stats.spears;
                    break;
                case __Engine.CardSymbol.armor:
                    factor = 0;
                    break;
                default:
                    break;
            }
            float ratio = (1.33f * (float)factor) / (float)deck_size;
            return ratio;
        }
    }
}