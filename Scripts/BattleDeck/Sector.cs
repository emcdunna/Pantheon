using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Assets.Scripts.BattleDeck
{
    [System.Serializable]
    // The group of units in a sector which face off against the enemy battlegroup in the sector 
    public class Sector
    {
        public enum Type { LeftWing, Left, Center, Right, RightWing, Rear, None };
        public Type sector = Type.Center;

        public List<Battalion> units = new List<Battalion>();

        public int morale = 0;
        public float moraleModifier = 0;
        public int armorModifier = 0;
        public int shieldModifier = 0;
        public int fatigueModifier = 0;

        public Army army;
        bool HasBroken = false;


        public Sector(Type Sector)
        {
            sector = Sector;
        }

        public Stats GetMaximumStats()
        {
            Stats stats = new Stats(
                                    Swords: GetSwords(),
                                    Spears: GetSpears(),
                                    Arrows: GetArrows(),
                                    Armor: Mathf.CeilToInt(10 * GetAverageArmor()),
                                    Shock: GetShock(),
                                    Evasion: GetEvasion(),
                                    Shields:GetShields()
                                    );
            
            return stats;
        }

        public void DeployUnit(Battalion Battalion)
        {
            units.Add(Battalion);
            UpdateMorale();
        }

        public bool Contains(Battalion battalion)
        {
            return units.Contains(battalion);
        }

        public void RemoveUnit(Battalion battalion)
        {
            units.Remove(battalion);
            UpdateMorale();
        }

        // when new units join or leave the sector the baseline morale will be changed
        public void UpdateMorale()
        {
            int total = 0;
            foreach (Battalion bt in units)
            {
                total += bt.GetMorale();
            }
            morale = total;
        }

        public float GetMorale()
        {
            if (HasBroken)
            {
                return 0;
            }
            if (moraleModifier > 0)
            {
                moraleModifier = 0;
            }
            return morale + moraleModifier;
        }

        public int GetSwords()
        {
            int total = 0;
            if (HasBroken)
            {
                return 0;
            }
            foreach (Battalion bt in units)
            {
                total += bt.GetSwords();
            }
            return total;
        }

        public int GetShock()
        {
            int total = 0;
            if (HasBroken)
            {
                return 0;
            }
            foreach (Battalion bt in units)
            {
                total += bt.GetShock();
            }
            return total;
        }

        public int GetArrows()
        {
            int total = 0;
            if (HasBroken)
            {
                return 0;
            }
            foreach (Battalion bt in units)
            {
                total += bt.GetArrows();
            }
            return total;
        }

        public int GetSpears()
        {
            int total = 0;
            if (HasBroken)
            {
                return 0;
            }
            foreach (Battalion bt in units)
            {
                total += bt.GetSpears();
            }
            return total;
            
        }

        public int GetShields()
        {
            int total = 0;
            if (HasBroken)
            {
                return 0;
            }
            foreach (Battalion bt in units)
            {
                total += bt.GetShields();
            }
            total += shieldModifier;
            if (total <= 0 || units.Count == 0)
            {
                return 0;
            }
            else
            {
                return total;
            }
        }

        public int GetEvasion()
        {
            int total = 0;
            if (HasBroken)
            {
                return 0;
            }
            foreach (Battalion bt in units)
            {
                total += bt.GetEvasion();
            }
            return total;
            
        }

        public float GetAverageArmor()
        {
            int total = 0;
            if (HasBroken)
            {
                return 0;
            }
            foreach (Battalion bt in units)
            {
                total += bt.GetArmor();
            }
            total += armorModifier;
            if (total <= 0 || units.Count == 0)
            {
                return 0;
            }
            else
            {
                return total/units.Count;
            }

        }

        public bool HasAlreadyBroken()
        {

            return HasBroken;
        }

        public void BreakSector()
        {
            HasBroken = true;
        }

        public override string ToString()
        {
            return this.sector.ToString();
        }

        public void PenetrateArmor(int amount)
        {
            armorModifier -= amount;
        }

        public void BreakShields(int amount)
        {
            shieldModifier -= amount;
        }

        // Deploy a unit to a sector (without needing to check if for duplicates)
        public void Deploy(Battalion battalion)
        {
            army.DeployUnit(sector, battalion);
        }

        public class Stats
        {
            public readonly int swords, shock, arrows, evasion, spears, shields;
            public readonly float armor = 0;

            public Stats(int Swords = 0, int Shock = 0, int Arrows = 0, int Evasion = 0, int Spears = 0, int Shields = 0, float Armor = 0)
            {
                swords = Swords;
                shock = Shock;
                arrows = Arrows;
                evasion = Evasion;
                spears = Spears;
                shields = Shields;
                armor = Armor;
            }
        }

    }
}