using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BattleDeck
{

    // Battalions represent units that fight in the battle
    [System.Serializable]
    public class Battalion
    {
        
        public enum Type
        {
            Infantry,
            Cavalry,
            Artillery,
            Elephants,
            Chariots
        }

        public enum Weight
        {
            Light,
            Medium,
            Heavy
        }

        public enum Role
        {
            Melee,
            Ranged,
            Shock,
            Support
        }

        public readonly string name;

        public readonly Type type = Type.Infantry;
        public readonly Weight weight = Weight.Medium;
        public readonly Role role = Role.Melee;

        public readonly __UnitLoadout.Armor body_armor;
        public readonly __UnitLoadout.Armor helmet_armor;

        public readonly __UnitLoadout.Weapon melee_weapon;
        public readonly __UnitLoadout.Weapon ranged_weapon;
        public readonly __UnitLoadout.Weapon shield_weapon;

        public readonly __UnitLoadout.Mount mount;
        public readonly __UnitLoadout.SpecialRule experience, recruitment;

        public readonly Card trainingCard = null;

        private ActionScript actionScript;

        public _MB_UnitUI UnitUI;


        public Battalion(string Name, __UnitLoadout.Weapon MeleeWeapon,
                         __UnitLoadout.Weapon RangedWeapon, __UnitLoadout.Weapon Shield, 
                         __UnitLoadout.Armor BodyArmor, __UnitLoadout.Armor HelmetArmor,  
                         __UnitLoadout.Mount Mount, __UnitLoadout.SpecialRule Experience,
                         __UnitLoadout.SpecialRule Recruitment, Card TrainingCard)
        {
            name = Name;
            type = Type.Infantry;
            
            body_armor = BodyArmor;
            helmet_armor = HelmetArmor;
            melee_weapon = MeleeWeapon;
            ranged_weapon = RangedWeapon;
            shield_weapon = Shield;
            mount = Mount;
            experience = Experience;
            recruitment = Recruitment;
            trainingCard = TrainingCard;


            if (mount.IsNone())
            {
                type = Type.Infantry;
            } else
            {
                if (mount.name.Contains("Elephant"))
                {
                    type = Type.Elephants;
                } else if (mount.name.Contains("Chariot"))
                {
                    type = Type.Chariots;
                } else
                {
                    type = Type.Cavalry;
                }
            }

            weight = GetWeight();

            role = GetRole();

        }

        public void SetScript(ActionScript script)
        {
            this.actionScript = script;
        }

        public void StartAnimation()
        {
            UnitUI.StartAnimation(actionScript);
        }

        public ActionScript GetScript()
        {
            return actionScript;
        }

        public int CalculateWeight()
        {
            int w = 0;
            w += GetArmor();
            w += melee_weapon.hands;
            w += ranged_weapon.hands;
            w += shield_weapon.hands;

            return w;
        }

        public string GetUnitType()
        {
            string ut = weight.ToString();
            ut += " " + role.ToString();
            ut += " " + type.ToString();
            return ut;
        }

        public Weight GetWeight()
        {
            int w = CalculateWeight();
            Weight unitWeight;
            if (w <= 4)
            {
                unitWeight = Weight.Light;
            }
            else if (w <= 6)
            {
                unitWeight = Weight.Medium;
            }
            else
            {
                unitWeight = Weight.Heavy;
            }
            return unitWeight;
        }

        public List<Card> GetCards()
        {
            List<Card> cards = new List<Card>();

            if (trainingCard != null)
            {
                cards.Add(trainingCard);
            }
            
            if (!(melee_weapon.weaponCard == null))
            {
                cards.Add(melee_weapon.weaponCard);
            }
            if (!(ranged_weapon.weaponCard == null))
            {
                cards.Add(ranged_weapon.weaponCard);
            }
            if (!(shield_weapon.weaponCard == null))
            {
                cards.Add(shield_weapon.weaponCard);
            }
            
            return cards;
        }

        public Role GetRole()
        {
            int swords, shock, arrows;
            Role newrole = Role.Support;
            swords = GetSwords();
            shock = GetShock();
            arrows = GetArrows();

            int maxScore = 0;


            if (arrows > maxScore)
            {
                newrole = Role.Ranged;
                maxScore = arrows;
            }
            if (shock > maxScore)
            {
                newrole = Role.Shock;
                maxScore = shock;
            }
            if (swords > maxScore)
            {
                newrole = Role.Melee;
                maxScore = swords;
            }

            return newrole;
        }

        public int GetDamageBonus()
        {
            return experience.damage_bonus + recruitment.damage_bonus;
        }

        public int GetArmor()
        {
            int total = body_armor.armorBonus + helmet_armor.armorBonus; 
            if (total <= 0)
            {
                return 0;
            }
            return total;
        }

        public int GetSwords()
        {
            if (melee_weapon.IsNone() || !mount.IsNone())
            {
                return 0;
            }
            int total = melee_weapon.sword + ranged_weapon.sword + shield_weapon.sword + GetDamageBonus();
            if (total <= 0)
            {
                return 0;
            }
            return total;
        }

        public int GetShock()
        {
            if (mount.IsNone() || melee_weapon.IsNone())
            {
                return 0;
            }
            int total = melee_weapon.shock + ranged_weapon.shock + shield_weapon.shock + mount.shock_bonus + GetDamageBonus();
            if (total <= 0)
            {
                return 0;
            }
            return total;
        }

        public int GetArrows()
        {
            if (ranged_weapon.IsNone())
            {
                return 0;
            }
            int total = melee_weapon.arrow + ranged_weapon.arrow + shield_weapon.arrow + GetDamageBonus();
            if (total <= 0)
            {
                return 0;
            }
            return total;
        }

        public int GetEvasion()
        {
            
            int total = melee_weapon.evasion + ranged_weapon.evasion + shield_weapon.evasion
                + body_armor.evasion + helmet_armor.evasion + mount.evasion;

            Weight w = GetWeight();
            if (w == Weight.Light)
            {
                total += 1;
            } else if (w == Weight.Heavy)
            {
                total -= 1;
            }
            if (total <= 0)
            {
                return 0;
            }
            return total;
        }


        public int GetParryBonus()
        {
            int total = 0;
            if (melee_weapon.EnablesParry())
            {
                total += 1;
            } 
            if (shield_weapon.EnablesParry())
            {
                total += 1;
            }
            return total;
        }


        public int GetSpears()
        {
            int total = melee_weapon.spear + ranged_weapon.spear + shield_weapon.spear;
            if (!mount.IsNone())
            {
                return 0;
            }
            if (total <= 0)
            {
                return 0;
            }
            return total;
        }

        public int GetShields()
        {
            int total = melee_weapon.shield + ranged_weapon.shield + shield_weapon.shield;
            if (total <= 0)
            {
                return 0;
            }
            return total;
        }

        public int GetHeatPenalty()
        {
            int total = body_armor.heat + helmet_armor.heat + mount.heat_penalty;
            if (total <= 0)
            {
                return 0;
            }
            return total;
        }

        public Battalion Clone()
        {
            Battalion clone = new Battalion(name, melee_weapon, ranged_weapon, shield_weapon, 
                                            body_armor, helmet_armor, mount, experience,
                                            recruitment, trainingCard.Clone());
            return clone;
        }

        public override string ToString()
        {
            return name;
        }

        public int GetMorale()
        {
            return 15 + experience.morale_bonus + recruitment.morale_bonus;
        }

        // Get the priority list based on the stats of this unit
        public List<__Engine.CardSymbol> GetPriority()
        {
            List<__Engine.CardSymbol> cardSymbols = new List<__Engine.CardSymbol>() { 
                __Engine.CardSymbol.sword,
                __Engine.CardSymbol.arrow,
                __Engine.CardSymbol.shock,
                __Engine.CardSymbol.evasion,
                __Engine.CardSymbol.shield,
                __Engine.CardSymbol.spear
            };

            cardSymbols.Sort(delegate (__Engine.CardSymbol a, __Engine.CardSymbol b) 
                            { return GetStatByCardSymbol(a).CompareTo(GetStatByCardSymbol(b)); });

            cardSymbols.Reverse();
            string log = this.ToString() + " has priority ";
            foreach(__Engine.CardSymbol s in cardSymbols)
            {
                log += s.ToString() + ", ";
            }
            //Debug.Log(log);
            return cardSymbols;
        }


        public int GetStatByCardSymbol(__Engine.CardSymbol symbol)
        {
            switch (symbol) {
                case __Engine.CardSymbol.sword:
                    return GetSwords();
                case __Engine.CardSymbol.arrow:
                    return GetArrows();
                case __Engine.CardSymbol.shock:
                    return GetShock();
                case __Engine.CardSymbol.shield:
                    return GetShields();
                case __Engine.CardSymbol.spear:
                    return GetSpears();
                case __Engine.CardSymbol.evasion:
                    return GetEvasion();
                default:
                    return 0;

            }
        }
    }
}