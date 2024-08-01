using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.BattleDeck
{
    [System.Serializable]
    // Static class with rules for how actions work in battle
    public static class __Engine
    {

        public enum CardSymbol
        {
            sword,
            arrow,
            spear,
            shield,
            evasion,
            banner,
            fear,
            shock,
            armor,
            scoutting,
            star,
            trap,
            hourglass
        }

        public static bool IsAttackingSymbol(CardSymbol symbol)
        {
            if(symbol == CardSymbol.sword || symbol == CardSymbol.shock || symbol == CardSymbol.arrow)
            {
                return true;
            }
            return false;
        }

        public static bool IsDefenseSymbol(CardSymbol symbol)
        {
            if (symbol == CardSymbol.spear || symbol == CardSymbol.evasion || symbol == CardSymbol.shield)
            {
                return true;
            }
            return false;
        }


        public static void ProcessAttack(Action action, Play attackerPlay, Play counterPlay)
        {

            Battalion attacker = action.GetBattalion();
            CardSymbol attackSymbol = action.GetSymbol();
            Sector defender = counterPlay.battleSector;

            // Calculate Effective Attacks
            int effectiveAttacks = 0;
            CardSymbol defenseSymbol = CardSymbol.armor;
            __UnitLoadout.Weapon weapon = attacker.melee_weapon;
            switch (attackSymbol)
            {
                case CardSymbol.sword:
                    effectiveAttacks = attacker.GetSwords();
                    defenseSymbol = CardSymbol.evasion;
                    break;
                case CardSymbol.shock:
                    effectiveAttacks = attacker.GetShock();
                    defenseSymbol = CardSymbol.spear;
                    break;
                case CardSymbol.arrow:
                    effectiveAttacks = attacker.GetArrows();
                    defenseSymbol = CardSymbol.shield;
                    weapon = attacker.ranged_weapon;
                    break;
                default:
                    Debug.Log("Cannot attack with symbol " + attackSymbol.ToString());
                    return;
            }
            Debug.Log(attacker.name + " is attacking with " + weapon.ToString());

            int remainingBlocks = counterPlay.GetRemainingBlocks(defenseSymbol);

            Debug.Log("Effective attacks: " + effectiveAttacks.ToString());
            Debug.Log("Remaining blocks: " + remainingBlocks.ToString());
            
            // Calculate Net Damage & Calculate Effective Blocks
            int effectiveBlocks = 0;
            int netDamage = 0;
            if (remainingBlocks >= effectiveAttacks)
            {
                effectiveBlocks = effectiveAttacks;
            }
            else
            {
                netDamage = effectiveAttacks - remainingBlocks;
                effectiveBlocks = remainingBlocks;
            }

            // Perform strikeback damage
            if (counterPlay.card.strike_back)
            {
                Debug.Log("Attacker takes " + effectiveBlocks.ToString() + " strike back damage");
                attackerPlay.battleSector.moraleModifier -= effectiveBlocks;
            }

            // Remove the used blocks from the play for further use
            Debug.Log("Effective blocks: " + effectiveBlocks.ToString());
            counterPlay.ReduceRemainingBlocks(defenseSymbol, effectiveBlocks);

            // Lower damage from armor
            //Debug.Log("Total damage before armor: " + netDamage.ToString());
            float totalDamage = CalculateArmorDefense(netDamage, defender.GetAverageArmor());
            //Debug.Log("Total damage after armor: " + totalDamage.ToString());

            
            if(totalDamage > 0)
            {
                // Apply Armor penetration
                if (weapon.IsArmorPiercing())
                {
                    Debug.Log("Penetrating defender's armor");
                    defender.PenetrateArmor(1);
                }

                // Apply Shield breaking
                if (weapon.IsShieldBreaking())
                {
                    Debug.Log("Breaking defender's shields");
                    defender.BreakShields(1);
                }
            }
            
            // Defender takes the final net damage
            defender.moraleModifier -= totalDamage;
            Debug.Log("Defender takes " + totalDamage.ToString() + " damage.");
            
        }


        public static void ProcessCardEffects(Play attackerPlay, Play defenderPlay)
        {
            // Apply fear
            Debug.Log("Attacker inflicts " + attackerPlay.card.fear.ToString() + " fear damage.");
            defenderPlay.battleSector.moraleModifier -= attackerPlay.card.fear;

            // Apply effects to the attacker
            Debug.Log("Attacker gains " + attackerPlay.card.banners.ToString() + " morale from banners.");
            attackerPlay.battleSector.moraleModifier += attackerPlay.card.banners;
            
        }

        public static int CalculateEffective(int cardValue, int battleGroupValue)
        {
            return battleGroupValue * cardValue;
        }

        public static int CalculateNetDamage(int effectiveAttacks, int effectiveBlocks)
        {
            if (effectiveBlocks >= effectiveAttacks)
            {
                return 0;
            }
            else
            {
                return effectiveAttacks - effectiveBlocks;
            }
        }

        public static float CalculateArmorDefense(int netDamage, float averageArmor)
        {
            return (netDamage * (1 - (averageArmor / 10f)));
        }




    }
}