using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BattleDeck
{
    [System.Serializable]
    public class Army
    {

        // List of all battlegroups the army has
        Dictionary<Sector.Type, Sector> Sectors =
            new Dictionary<Sector.Type, Sector>();


        public readonly List<Battalion> all_battalions;

        public Army(List<Battalion> Battalions)
        {
            all_battalions = Battalions;
        }

        public List<Card> GetCards()
        {
            List<Card> cards = new List<Card>();

            foreach (Battalion battalion in all_battalions)
            {
                foreach (Card card in battalion.GetCards())
                {
                    Card newCard = card.Clone();
                    cards.Add(newCard);
                }
            }
            return cards;
        }

        public void DeployUnit(Sector.Type Sector, Battalion battalion)
        {
            Sector bg = Sectors[Sector];

            foreach(Sector sector in Sectors.Values)
            {
                if (sector.Contains(battalion))
                {
                    sector.RemoveUnit(battalion);
                }
            }
            bg.DeployUnit(battalion);
            
        }

        public void SetupBattleSectors(int BattleSize)
        {
            Sector center = new Sector(Sector.Type.Center);
            Sectors.Add(Sector.Type.Center, center);
            center.army = this;
            if (BattleSize >= 2)
            {
                Sector left = new Sector(Sector.Type.Left);
                Sectors.Add(Sector.Type.Left, left);
                left.army = this;
            }
            if (BattleSize >= 3)
            {
                Sector right = new Sector(Sector.Type.Right);
                Sectors.Add(Sector.Type.Right, right);
                right.army = this;
            }
            if (BattleSize >= 4)
            {
                Sector leftwing = new Sector(Sector.Type.LeftWing);
                Sectors.Add(Sector.Type.LeftWing, leftwing);
                leftwing.army = this;
            }
            if (BattleSize >= 5)
            {
                Sector rightwing = new Sector(Sector.Type.RightWing);
                Sectors.Add(Sector.Type.RightWing, rightwing);
                rightwing.army = this;
            }
            Sector rear = new Sector(Sector.Type.Rear);
            Sectors.Add(Sector.Type.Rear, rear);
            rear.army = this;

        }

        public Sector GetBattleSector(Sector.Type sector)
        {
            if (Sectors.ContainsKey(sector))
            {
                return Sectors[sector];
            }
            else
            {
                return null;
            }
        }

    }
}