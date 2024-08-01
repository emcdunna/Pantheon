using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BattleDeck
{
    [System.Serializable]
    public class UnitAnimation
    {
        public static float MINIMUM_ANIMATION_DISTANCE = 1f;

        public enum AnimationPhase
        {
            MoralePhase,
            RangedPhase,
            ShockPhase,
            MeleePhase
        }

        // Animation order is the order that actions are performed in the animation
        public static readonly List<AnimationPhase> ANIMATION_ORDER = new List<AnimationPhase>() {
                AnimationPhase.MoralePhase,
                AnimationPhase.RangedPhase,
                AnimationPhase.ShockPhase,
                AnimationPhase.MeleePhase
            };


        public readonly Vector3 moveDestination;
        public readonly Action action;
        public readonly _MB_UnitUI unit;
        public readonly float restDuration;
        private float waitFinishTime;
        private bool isResting = false;
        private bool hasActed = false;


        public UnitAnimation(_MB_UnitUI unit, Vector3 moveDestination, Action action, float restDuration)
        {
            this.moveDestination = moveDestination;
            this.action = action;
            this.unit = unit;
            this.restDuration = restDuration;
        }


        // Find out if the animation has finished yet
        public bool IsFinished()
        {
            if (!isResting)
            {
                float dist = Vector3.Distance(unit.transform.position, moveDestination);
                if (dist <= MINIMUM_ANIMATION_DISTANCE)
                {
                    isResting = true;
                    waitFinishTime = Time.time + restDuration;
                }
            }
            else if (Time.time >= waitFinishTime)
            {
                isResting = false;
                return true;
            }
            return false;
        }

        public bool HasActed()
        {
            return hasActed;
        }

        public void SetHasActed(bool value)
        {
            hasActed = value;
        }

        public override string ToString()
        {
            return "Animation of " + action.ToString();
        }

        public static AnimationPhase GetPhaseForSymbol(__Engine.CardSymbol symbol)
        {
            switch (symbol)
            {
                case __Engine.CardSymbol.arrow:
                    return AnimationPhase.RangedPhase;
                case __Engine.CardSymbol.shield:
                    return AnimationPhase.RangedPhase;
                case __Engine.CardSymbol.sword:
                    return AnimationPhase.MeleePhase;
                case __Engine.CardSymbol.evasion:
                    return AnimationPhase.MeleePhase;
                case __Engine.CardSymbol.shock:
                    return AnimationPhase.ShockPhase;
                case __Engine.CardSymbol.spear:
                    return AnimationPhase.ShockPhase;
                default:
                    return AnimationPhase.MoralePhase;
            }
        }


        // Create an animation for the action given the specifics of the game/units onscreen
        public static UnitAnimation GenerateAnimation(_MB_UnitUI unit, Action action)
        {
            Vector3 destination = unit.transform.position;
            float restTime = 0.4f;
            switch (action.type)
            {
                case Action.Type.MoveBack:
                    destination = unit.GetHomePosition();
                    break;
                case Action.Type.MoveUp:
                    destination = unit.GetFrontlinesPosition();
                    break;
                case Action.Type.Attack:
                    restTime = 0.4f;
                    break;
                case Action.Type.Defense:
                    restTime = 0.7f;
                    break;
                case Action.Type.Wait:
                    restTime = 1;
                    break;
                default:
                    break;
            }

            UnitAnimation anim = new UnitAnimation(unit, destination, action, restTime);

            return anim;
        }
    }

}