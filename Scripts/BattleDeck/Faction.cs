using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.BattleDeck
{
    public class Faction
    {

        public readonly Color primary, secondary;
        public readonly string factionName;
        Sprite bannerSprite;
        List<Card> factionCards;

        public Sprite GetBannerSprite()
        {
            return bannerSprite;
        }

        public Faction(string Name, Color PrimaryColor, Color SecondaryColor, Sprite Banner, 
                        List<Card> FactionCards)
        {
            factionName = Name;
            bannerSprite = Banner;
            primary = PrimaryColor;
            secondary = SecondaryColor;
            factionCards = FactionCards;
        }

        public override string ToString()
        {
            return factionName;
        }

        public List<Card> GetCards()
        {
            return factionCards;
        }

    }
}
