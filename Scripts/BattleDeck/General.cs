using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A general represents the abilities and tactics an army will have during battle
namespace Assets.Scripts.BattleDeck
{
    public class General
    {

        List<Card> generalCards = new List<Card>();
        public readonly int PassiveMorale = 1;
        public readonly string name;
        public readonly Sprite portrait;
        public readonly int hand_limit = 5;

        public bool send_scouts, rally, lead_the_charge, delay;

        public General(string Name)
        {
            name = Name;
        }


        public List<Card> GetCards()
        {
            return generalCards;
        }


    }
}
