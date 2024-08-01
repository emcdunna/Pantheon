using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BattleDeck
{
    [System.Serializable]
    public class ActionScript
    {

        public enum Priority
        {
            MeleeFirst,
            ShockFirst,
            RangedFirst,
            ShieldFirst,
            SpearFirst,
            EvadeFirst,
            Default
        }


        // Priority determines which action is taken if the unit can perform 2 and has to choose 1
        public static readonly List<__Engine.CardSymbol> meleePriority = new List<__Engine.CardSymbol>() {
            __Engine.CardSymbol.arrow,
            __Engine.CardSymbol.shock,
            __Engine.CardSymbol.sword,
            __Engine.CardSymbol.shield,
            __Engine.CardSymbol.spear,
            __Engine.CardSymbol.evasion
        };
        public static readonly List<__Engine.CardSymbol> shockPriority = new List<__Engine.CardSymbol>() {
            __Engine.CardSymbol.shock,
            __Engine.CardSymbol.sword,
            __Engine.CardSymbol.arrow,
            __Engine.CardSymbol.shield,
            __Engine.CardSymbol.spear,
            __Engine.CardSymbol.evasion
        };
        public static readonly List<__Engine.CardSymbol> rangedPriority = new List<__Engine.CardSymbol>() {
            __Engine.CardSymbol.arrow,
            __Engine.CardSymbol.shock,
            __Engine.CardSymbol.sword,
            __Engine.CardSymbol.shield,
            __Engine.CardSymbol.spear,
            __Engine.CardSymbol.evasion
        };
        public static readonly List<__Engine.CardSymbol> shieldPriority = new List<__Engine.CardSymbol>() {
            __Engine.CardSymbol.shield,
            __Engine.CardSymbol.arrow,
            __Engine.CardSymbol.shock,
            __Engine.CardSymbol.sword,
            __Engine.CardSymbol.spear,
            __Engine.CardSymbol.evasion
        };
        public static readonly List<__Engine.CardSymbol> spearPriority = new List<__Engine.CardSymbol>() {
            __Engine.CardSymbol.spear,
            __Engine.CardSymbol.arrow,
            __Engine.CardSymbol.shock,
            __Engine.CardSymbol.sword,
            __Engine.CardSymbol.shield,
            __Engine.CardSymbol.evasion
        };
        public static readonly List<__Engine.CardSymbol> evadePriority = new List<__Engine.CardSymbol>() {
            __Engine.CardSymbol.evasion,
            __Engine.CardSymbol.arrow,
            __Engine.CardSymbol.shock,
            __Engine.CardSymbol.sword,
            __Engine.CardSymbol.shield,
            __Engine.CardSymbol.spear
        };



        public static readonly int ACTION_LIMIT = 3;
        public static readonly int UNIQUE_ATTACK_LIMIT = 1;
        public static readonly int UNIQUE_DEFENSE_LIMIT = 1;


        // Start class definition
        private int actions_remaining, attacks_remaining, defenses_remaining;
        private Battalion battalion;
        private Priority priority;
        private List<Action> actions;
        private Play play;

        public ActionScript(Play play, Battalion battalion, Priority priority)
        {
            actions = new List<Action>();
            this.battalion = battalion;
            this.priority = priority;
            this.play = play;
            actions_remaining = ACTION_LIMIT;
            attacks_remaining = UNIQUE_ATTACK_LIMIT;
            defenses_remaining = UNIQUE_DEFENSE_LIMIT;
        }

        public void Add(Action action)
        {
            actions.Add(action);
        }

        public Play GetPlay()
        {
            return play;
        }

        public List<Action> GetActions()
        {
            return actions;
        }

        public List<__Engine.CardSymbol> GetSymbolPriority()
        {
            switch (priority)
            {
                case Priority.MeleeFirst:
                    return meleePriority;
                case Priority.ShockFirst:
                    return shockPriority;
                case Priority.ShieldFirst:
                    return shieldPriority;
                case Priority.SpearFirst:
                    return spearPriority;
                case Priority.EvadeFirst:
                    return evadePriority;
                case Priority.RangedFirst:
                    return rangedPriority;
                default:
                    return battalion.GetPriority();
            }
        }


        public static List<Action> GetActionsForSymbol(__Engine.CardSymbol symbol, Play play,
                                                       Battalion battalion, ActionScript script)
        {

            List<Action> actions = new List<Action>();

            int play_stat = play.GetStatByCardSymbol(symbol);
            int b_stat = battalion.GetStatByCardSymbol(symbol);
            bool can_perform = false;

            // Determine which action type it is
            Action.Type type = Action.Type.Defense;
            if (__Engine.IsAttackingSymbol(symbol))
            {
                type = Action.Type.Attack;
                can_perform = (script.attacks_remaining > 0 && script.actions_remaining > 0);
            }
            if (__Engine.IsDefenseSymbol(symbol))
            {
                type = Action.Type.Defense;
                can_perform = (script.defenses_remaining > 0 && script.actions_remaining > 0);
            }

            // If the unit can still perform this action, then let them add this series of actions
            if (b_stat > 0 && play_stat > 0 && can_perform)
            {
                // Always move up before the attack
                actions.Add(new Action(Action.Type.MoveUp, battalion, symbol));

                // Allow many attacks in a row as long as the unit has not reached its limit
                for (int i = 0; i < play_stat; i++)
                {
                    Action action = new Action(type, battalion, symbol);
                    if (script.actions_remaining > 0)
                    {
                        actions.Add(action);
                        script.actions_remaining -= 1;
                    }
                    else
                    {
                        //Debug.Log(battalion + " is out of actions so it cannot perform " + action);
                    }

                    if (type == Action.Type.Attack)
                    {
                        script.attacks_remaining -= 1;
                    }
                    else
                    {
                        script.defenses_remaining -= 1;
                    }

                }
                // Always move back after the attack
                actions.Add(new Action(Action.Type.MoveBack, battalion, symbol));

            }

            // Always add a "WAIT" action for each symbol type
            actions.Add(new Action(Action.Type.Wait, battalion, symbol));
            return actions;
        }

        public static ActionScript GenerateScript(Play play, Battalion battalion)
        {
            Dictionary<UnitAnimation.AnimationPhase, List<Action>> anim_phases = new Dictionary<UnitAnimation.AnimationPhase, List<Action>>();
            ActionScript script = new ActionScript(play, battalion, Priority.Default);

            // Iterate over every card symbol
            foreach (__Engine.CardSymbol symbol in script.GetSymbolPriority())
            {
                UnitAnimation.AnimationPhase phase = UnitAnimation.GetPhaseForSymbol(symbol);

                if (anim_phases.ContainsKey(phase))
                {
                    // Only needed if we allow a unit to both fight and block in the same phase
                    //actions = anim_phases[phase];
                    //Debug.Log(battalion + " already has actions for phase " + phase.ToString() + "\nNot adding " + symbol.ToString() + " actions.");
                }
                else
                {
                    // Get the animations needed for this symbol
                    anim_phases.Add(phase, GetActionsForSymbol(symbol, play, battalion, script));
                }
            }

            // Add each anim phase in the right order to the final script list
            foreach (UnitAnimation.AnimationPhase phase in UnitAnimation.ANIMATION_ORDER)
            {
                if (anim_phases.ContainsKey(phase))
                {
                    foreach (Action action in anim_phases[phase])
                    {
                        script.Add(action);
                    }
                }
                else
                {
                    script.Add(new Action(Action.Type.Wait, battalion, __Engine.CardSymbol.hourglass));
                }

            }
            return script;
        }

        public override string ToString()
        {
            string full = battalion.ToString() + " performing actions: ";

            int i = 0;
            foreach (Action action in this.actions)
            {
                i += 1;
                full += "\n   " + i.ToString() + ") " + action.ToString();
            }
            return full;
        }
    }
}