using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BattleDeck
{
    [System.Serializable]
    // A specific play of a card and abilities to a sector in the game
    public class Play
    {
        public Player player;
        public Card card;
        public Sector battleSector;
        public Sector target;
        public readonly int effectiveShock = 0, effectiveSwords = 0, effectiveArrows = 0, 
                   effectiveSpears = 0, effectiveEvasion = 0, effectiveShields = 0;
        int remainingSpears = 0, remainingEvasion = 0, remainingShields = 0;


        public Play(Player Player, Card Card, Sector BattleSector)
        {
            player = Player;
            card = Card;
            battleSector = BattleSector;

            
            if (card == null)
            {
                Debug.Log("Play created with null card by " + player + " in sector " + battleSector);
                GenerateRandomCardSymbol();
            }
            else if (card.type == Card.CardType.Delegate)
            {
                GenerateRandomCardSymbol();
            }

            if (battleSector.HasAlreadyBroken())
            {
                // TODO
            }


            // Count up the effective values
            List<Action> actions = GetActions();

            foreach(Action action in actions)
            {
                switch (action.GetSymbol())
                {
                    case __Engine.CardSymbol.arrow:
                        effectiveArrows += action.GetValue();
                        break;
                    case __Engine.CardSymbol.sword:
                        effectiveSwords += action.GetValue();
                        break;
                    case __Engine.CardSymbol.shock:
                        effectiveShock += action.GetValue();
                        break;
                    case __Engine.CardSymbol.shield: 
                        effectiveShields += action.GetValue();
                        break;
                    case __Engine.CardSymbol.spear:
                        effectiveSpears += action.GetValue();
                        break;
                    case __Engine.CardSymbol.evasion:
                        effectiveEvasion += action.GetValue();
                        break;

                    default:
                        break;
                }
            }


            if (card.swords > 0)
            {
                int total = 0;
                foreach (Battalion battalion in battleSector.units)
                {
                    int x = battalion.GetParryBonus();
                    if (x > 0)
                    {
                        effectiveEvasion += x;
                        total += x;
                    }
                }
            }
            remainingEvasion = effectiveEvasion;
            remainingSpears = effectiveSpears;
            remainingShields = effectiveShields;
        }

        public int GetStatByCardSymbol(__Engine.CardSymbol symbol)
        {
            switch (symbol)
            {
                case __Engine.CardSymbol.sword:
                    return card.swords;
                case __Engine.CardSymbol.arrow:
                    return card.arrows;
                case __Engine.CardSymbol.shock:
                    return card.shock;
                case __Engine.CardSymbol.shield:
                    return card.shields;
                case __Engine.CardSymbol.spear:
                    return card.spears;
                case __Engine.CardSymbol.evasion:
                    return card.evasion;
                default:
                    return 0;

            }
        }

        void GenerateRandomCardSymbol()
        {
            List<Card> delegates = new List<Card>();
            if (battleSector.GetShock() > 0)
            {
                delegates.Add(new Card("Delegate to the Lieutenant", Card.CardType.Delegate, Shock: 1));
            }
            if (battleSector.GetArrows() > 0)
            {
                delegates.Add(new Card("Delegate to the Lieutenant", Card.CardType.Delegate, Arrows: 1));
            }
            if (battleSector.GetEvasion() > 0)
            {
                delegates.Add(new Card("Delegate to the Lieutenant", Card.CardType.Delegate, Evasion: 1));
            }
            if (battleSector.GetSwords() > 0)
            {
                delegates.Add(new Card("Delegate to the Lieutenant", Card.CardType.Delegate, Swords: 1));
            }
            if (battleSector.GetSpears() > 0)
            {
                delegates.Add(new Card("Delegate to the Lieutenant", Card.CardType.Delegate, Spears: 1));
            }
            if (battleSector.GetShields() > 0)
            {
                delegates.Add(new Card("Delegate to the Lieutenant", Card.CardType.Delegate, Shields: 1));
            }

            int index = UnityEngine.Random.Range(0, delegates.Count - 1);
            card = delegates[index];
        }

        public bool GetIsValid()
        {
            return true;
        }


        public Sector.Type GetTarget()
        {
            Scenario scenario = _MB_BattleRunner.Runner().GetScenario();
            Army enemyArmy;

            if(battleSector.army == scenario.army1)
            {
                enemyArmy = scenario.army2;
            }
            else
            {
                enemyArmy = scenario.army1;
            }

            target = enemyArmy.GetBattleSector(battleSector.sector);

            if (target.HasAlreadyBroken() || card.cross_attack)
            {
                target = scenario.army2.GetBattleSector(Sector.Type.Center);
                Debug.Log("Play is targetting center instead.");
            }
            return target.sector;
        }

        public int CalculateUtility()
        {
            int utility = 10 * (effectiveShock + effectiveSwords + effectiveArrows);
            utility += 6 * (effectiveSpears + effectiveEvasion + effectiveShields);
            utility += 15 * (card.fear + card.banners);
            if (battleSector.sector != Sector.Type.Center && card.cross_attack)
            {
                utility += 20;
            }
            if (card.strike_back)
            {
                utility += 8 * (effectiveSpears + effectiveEvasion + effectiveShields);
            }


            return utility;

        }

        public void ReduceRemainingBlocks(__Engine.CardSymbol symbol, int amount)
        {
            switch (symbol)
            {
                case __Engine.CardSymbol.shield:
                    remainingShields -= amount;
                    break;
                case __Engine.CardSymbol.spear:
                    remainingSpears -= amount;
                    break;
                case __Engine.CardSymbol.evasion:
                    remainingEvasion -= amount;
                    break;
                default:
                    break;
            }
        }

        public int GetRemainingBlocks(__Engine.CardSymbol symbol)
        {
            switch (symbol)
            {
                case __Engine.CardSymbol.shield:
                    return remainingShields;
                case __Engine.CardSymbol.spear:
                    return remainingSpears;
                case __Engine.CardSymbol.evasion:
                    return remainingEvasion;
                default:
                    return 0;
            }
        }

        private List<Action> GetActions()
        {
            List<Action> actions = new List<Action>();

            foreach (Battalion battalion in battleSector.units)
            {
                ActionScript script = ActionScript.GenerateScript(this, battalion);
                actions.AddRange(script.GetActions());
            }

            return actions;
        }

        public void CreateActionScripts()
        {
            foreach (Battalion battalion in battleSector.units)
            {
                ActionScript script = ActionScript.GenerateScript(this, battalion);
                battalion.SetScript(script);
            }
        }
    }
}