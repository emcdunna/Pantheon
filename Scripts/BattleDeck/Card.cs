using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.BattleDeck
{
    // Battle cards are played in each sector as the tactic used in this round.
    [System.Serializable]
    public class Card
    {
        public int swords, shock, arrows, evasion, spears, shields, fear, banners, scoutting;
        public string flavorText, name, rulesText;
        public string card_id;
        public bool cross_attack, strike_back;
        public Sprite sprite;

        public enum CardType
        {
            Base,
            Strategy,
            Tactic,
            Psychology,
            Strength,
            Stats,
            Delegate,
            Ability
        }

        public static CardType LoadCardType(string text)
        {
            foreach (CardType t in Enum.GetValues(typeof(CardType)))
            {
                if (text == t.ToString())
                {
                    return t;
                }
            }
            return CardType.Base;
        }

        public CardType type;

        public Card(string Name, CardType cardType, int Swords = 0, int Shock = 0, int Arrows = 0, int Evasion = 0, int Spears = 0, int Shields = 0, int Fear = 0, int Banners = 0, bool CrossAttack = false, bool StrikeBack = false)
        {
            type = cardType;
            name = Name;
            swords = Swords;
            shock = Shock;
            arrows = Arrows;
            evasion = Evasion;
            spears = Spears;
            shields = Shields;
            fear = Fear;
            banners = Banners;
            cross_attack = CrossAttack;
            strike_back = StrikeBack;
        }

        public Card(string[] tsv_data)
        {
            card_id = tsv_data[0];
            name = tsv_data[1];
            type = LoadCardType(tsv_data[2]);
            swords = int.Parse(tsv_data[3]);
            shock = int.Parse(tsv_data[4]);
            arrows = int.Parse(tsv_data[5]);
            evasion = int.Parse(tsv_data[6]);
            spears = int.Parse(tsv_data[7]);
            shields = int.Parse(tsv_data[8]);
            fear = int.Parse(tsv_data[9]);
            banners = int.Parse(tsv_data[10]);
            cross_attack = bool.Parse(tsv_data[11]);
            strike_back = bool.Parse(tsv_data[12]);
        }

        public Card Clone()
        {
            Card clone = new Card(name, type, swords, shock, arrows, evasion, spears, shields, fear, banners, cross_attack, strike_back);
            clone.flavorText = flavorText;
            clone.rulesText = rulesText;
            return clone;
        }

        public override string ToString()
        {
            return name;
        }


        public List<__Engine.CardSymbol> GetCardSymbols()
        {
            List<__Engine.CardSymbol> symbols = new List<__Engine.CardSymbol>();

            for(int i = 0; i < swords; i++)
            {
                symbols.Add(__Engine.CardSymbol.sword);
            }
            for (int i = 0; i < arrows; i++)
            {
                symbols.Add(__Engine.CardSymbol.arrow);
            }
            for (int i = 0; i < shock; i++)
            {
                symbols.Add(__Engine.CardSymbol.shock);
            }
            for (int i = 0; i < evasion; i++)
            {
                symbols.Add(__Engine.CardSymbol.evasion);
            }
            for (int i = 0; i < shields; i++)
            {
                symbols.Add(__Engine.CardSymbol.shield);
            }
            for (int i = 0; i < spears; i++)
            {
                symbols.Add(__Engine.CardSymbol.spear);
            }
            if (strike_back)
            {
                //symbols.Add(__BattleEngine.CardSymbol.trap);
            }
            if (cross_attack)
            {
                //symbols.Add(__BattleEngine.CardSymbol.star);
            }
            return symbols;
        }
    }
}