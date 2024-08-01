using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.BattleDeck
{
    public static class __UnitLoadout
    {   
        public enum UnitTypeRestriction
        {
            Any,
            Infantry,
            Mounted,
            Elephant
        }

        public static UnitTypeRestriction loadUnitTypeRestriction(string text)
        {
            foreach (UnitTypeRestriction t in Enum.GetValues(typeof(UnitTypeRestriction)))
            {
                if (text == t.ToString())
                {
                    return t;
                }
            }
            return UnitTypeRestriction.Any;
        }

        // One or more weapons equipped by units of any type
        [System.Serializable]
        public class Weapon
        {
            public enum Type
            {
                Melee,
                Ranged,
                Shield,
                None
            }

            public static Type loadType(string text)
            {
                foreach (Type t in Enum.GetValues(typeof(Type)))
                {
                    if (text == t.ToString())
                    {
                        return t;
                    }
                }
                return Type.Melee;
            }

            public readonly Type type = Type.Melee;
            public readonly string name;
            public readonly int sword = 0, arrow = 0, shock = 0, evasion = 0, shield = 0, spear = 0, hands = 1;

            public readonly UnitTypeRestriction unitTypeRestriction = UnitTypeRestriction.Any;
            public Card weaponCard;

            public List<SpecialRule> specialRules = new List<SpecialRule>();
            
            public Weapon(string[] tsv_data, Card card)
            {
                name = tsv_data[0];
                sword = int.Parse(tsv_data[1]);
                arrow = int.Parse(tsv_data[2]);
                shock = int.Parse(tsv_data[3]);
                evasion = int.Parse(tsv_data[4]);
                shield = int.Parse(tsv_data[5]);
                spear = int.Parse(tsv_data[6]);
                hands = int.Parse(tsv_data[7]);
                type = loadType(tsv_data[8]);
                unitTypeRestriction = loadUnitTypeRestriction(tsv_data[9]);
                weaponCard = card;
            }

            public override string ToString()
            {
                return name;
            }

            public void AddSpecialRule(SpecialRule rule)
            {
                specialRules.Add(rule);
            }

            public void SetWeaponCard(Card card)
            {
                weaponCard = card;
            }

            public bool IsNone()
            {
                return name == "None";
            }

            public bool IsArmorPiercing()
            {
                if (specialRules.Contains(__Database.Database().GetSpecialRule("Armor Piercing")))
                {
                    return true;
                }
                return false;
            }

            public bool IsShieldBreaking()
            {
                if (specialRules.Contains(__Database.Database().GetSpecialRule("Shield Breaker")))
                {
                    return true;
                }
                return false;
            }

            public bool EnablesParry()
            {
                if (specialRules.Contains(__Database.Database().GetSpecialRule("Parry")))
                {
                    return true;
                }
                return false;
            }

            public bool IsBreakable()
            {
                if (specialRules.Contains(__Database.Database().GetSpecialRule("Breakable")))
                {
                    return true;
                }
                return false;
            }
        }

        // One or more armors that can be equipped on a unit
        [System.Serializable]
        public class Armor
        {
            public enum Type
            {
                Body,
                Helmet
            }

            public static Type loadType(string text)
            {
                foreach (Type t in Enum.GetValues(typeof(Type)))
                {
                    if (text == t.ToString())
                    {
                        return t;
                    }
                }
                return Type.Body;
            }

            public readonly Type type = Type.Body;
            public readonly string name;
            public readonly int armorBonus = 0, evasion = 0, iron_cost = 0, textiles_cost = 0, heat = 0;

            public Armor(string[] tsv_data)
            {
                name = tsv_data[0];
                armorBonus = int.Parse(tsv_data[1]);
                type = loadType(tsv_data[2]);
                evasion = int.Parse(tsv_data[3]);
                iron_cost = int.Parse(tsv_data[4]);
                textiles_cost = int.Parse(tsv_data[5]);
                heat = int.Parse(tsv_data[6]);
            }

            public override string ToString()
            {
                return name;
            }

            public bool IsNone()
            {
                return name == "None";
            }
        }

        // One (or zero) mounts equipped by a unit
        [System.Serializable]
        public class Mount
        {

            public readonly string name;
            public readonly int evasion, shock_bonus, heat_penalty, terrain_penalty;
            public readonly float movementSpeed, accelerationTime;

            public readonly SpecialRule special_rule;

            public Mount(string[] tsv_data, SpecialRule specialRule)
            {
                name = tsv_data[0];
                evasion = int.Parse(tsv_data[1]);
                shock_bonus = int.Parse(tsv_data[2]);
                heat_penalty = int.Parse(tsv_data[3]);
                terrain_penalty = int.Parse(tsv_data[4]);
                movementSpeed = float.Parse(tsv_data[6]);
                accelerationTime = float.Parse(tsv_data[7]);
                special_rule = specialRule;
            }

            public override string ToString()
            {
                return name;
            }

            public bool IsNone()
            {
                return name == "None";
            }
        }


        // Special rules apply modifiers to a unit on top of its other equipment
        [System.Serializable]
        public class SpecialRule
        {
            public enum Type
            {
                Experience,
                Recruitment,
                Environment,
                Behavior,
                Weapon,
                Culture,
                Difficulty
            }

            public static Type loadType(string text)
            {
                foreach (Type t in Enum.GetValues(typeof(Type)))
                {
                    if (text == t.ToString())
                    {
                        return t;
                    }
                }
                return Type.Experience;
            }

            public readonly Type type;
            public readonly string name;
            public readonly int morale_bonus, damage_bonus, heat_bonus;

            public SpecialRule(string[] tsv_data)
            {
                name = tsv_data[0];
                type = loadType(tsv_data[1]);
                morale_bonus = int.Parse(tsv_data[2]);
                damage_bonus = int.Parse(tsv_data[3]);
                heat_bonus = int.Parse(tsv_data[4]);
            }

            public override string ToString()
            {
                return name;
            }

            public bool IsNone()
            {
                return name == "None";
            }
        }
    }
}