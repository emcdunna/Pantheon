using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BattleDeck
{
    public class _MB_UnitUI : MonoBehaviour
    {
        Vector3 retreat_direction;
        bool blinkingUp = false;
        float current_saturation = 0;
        float saturation_limit = 0;
        Battalion battalion;
        SpriteRenderer sprite;
        float smooth_time;
        Vector3 targetPosition;
        float topSpeed, speed;
        Vector3 velocity = Vector3.zero;
        Color TeamColor;
        float breakTime;
        ActionScript script;
        int actionIndex = 0;

        public Vector3 forwardDirection;
        public UnitAnimation current_animation;
        public GameObject highlighter;

        private Vector3 homePosition;
        private Vector3 frontlinesPosition;

        public enum AnimationState
        {
            Animating,
            Fleeing,
            Waiting
        }

        AnimationState state = AnimationState.Waiting;


        public void Break(float timeOffset = 0)
        {
            breakTime = Time.time + Random.Range(0.1f, 0.55f) + timeOffset;
            state = AnimationState.Fleeing;
            targetPosition = transform.position + retreat_direction + new Vector3(UnityEngine.Random.Range(-80, 80), 0, 0);
        }

        public void EnableHighlight()
        {
            highlighter.SetActive(true);
        }

        public void DisableHighlight()
        {
            highlighter.SetActive(false);
        }

        void OnMouseOver()
        {
            _MB_PlayerHUD hud = _MB_BattleRunner.Runner().PlayerHud;
            if ( Input.GetMouseButtonDown(0))
            {
                hud.UpdateSelectedUnit(battalion);
            } else if(Input.GetMouseButtonDown(1))
            {
                hud.SetUnitDescriptor(battalion);
            }

        }



        void UpdateSaturation()
        {

            if (current_saturation >= 0.6f && blinkingUp)
            {
                saturation_limit = 0f;
                blinkingUp = false;
            }
            else if (current_saturation <= 0.2f)
            {
                saturation_limit = 0.8f;
                blinkingUp = true;
            }

            // glide toward desired saturation
            if (current_saturation < 0)
            {
                current_saturation = 0;
            }
            else if (current_saturation > 1)
            {
                current_saturation = 1;
            }
            else
            {
                current_saturation += (saturation_limit - current_saturation) * Time.deltaTime;
            }

            float red = TeamColor.r + (1 - TeamColor.r) * current_saturation;
            float green = TeamColor.g + (1 - TeamColor.g) * current_saturation;
            float blue = TeamColor.b + (1 - TeamColor.b) * current_saturation;

            sprite.color = new Color(red, green, blue);
        }


        public AnimationState GetAnimationState()
        {
            return state;
        }

        public void Setup(Battalion battalion, Vector3 forwardDirection, Vector3 frontlinesPosition)
        {
            this.battalion = battalion;
            this.retreat_direction = -1 * forwardDirection;
            this.forwardDirection = forwardDirection;

            this.frontlinesPosition = frontlinesPosition;

            topSpeed = battalion.mount.movementSpeed;
            smooth_time = battalion.mount.accelerationTime;
            targetPosition = transform.position;
            sprite = GetComponent<SpriteRenderer>();
            TeamColor = sprite.color;
            blinkingUp = true;
            saturation_limit = 0;
            state = AnimationState.Waiting;
            actionIndex = 0;
            homePosition = this.transform.position;
        }

        public Vector3 GetHomePosition()
        {
            return homePosition;
        }

        public Vector3 GetFrontlinesPosition()
        {
            return new Vector3(transform.position.x, frontlinesPosition.y);
        }


        public bool IsNotDoneWithPhase(UnitAnimation.AnimationPhase phase)
        {
            // TODO make this more robust

            return GetAnimationState() == _MB_UnitUI.AnimationState.Animating;
        }

        public Battalion GetBattalion()
        {
            return battalion;
        }


        // Execute the actions in the script by animating each action in series
        public void StartAnimation(ActionScript script)
        {
            state = AnimationState.Animating;
            this.script = script;
            actionIndex = 0;
            CreateAnimation();
        }

        public void CreateAnimation()
        {
            if (actionIndex < script.GetActions().Count)
            {
                Action action = script.GetActions()[actionIndex];
                current_animation = UnitAnimation.GenerateAnimation(this, action);
            }
            else
            {
                current_animation = null;
            }
        }

        public UnitAnimation GetCurrentAnimation()
        {
            // TODO: Make sure this never proceeds to animations that are not in the right phase
            return current_animation;
        }

        public void ContinueToNextAnimation()
        {
            //Debug.Log("Finished " + GetCurrentAnimation().ToString());
            actionIndex += 1;
            speed = 0;
            state = AnimationState.Animating;
            CreateAnimation();
        }

        // Move the unit towards the current desired position
        private void Move()
        {
            if (speed < topSpeed)
            {
                speed = Mathf.Min(speed + (Time.deltaTime * topSpeed / smooth_time), topSpeed);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }


        // Apply the damage to the enemy's morale bar
        public void ProcessAttack(Action action)
        {
            Play play1 = script.GetPlay();
            Scenario scenario = _MB_BattleRunner.Runner().GetScenario();
            Play play2 = scenario.GetOpposingPlayer(play1.player).GetPlayOrDelegate(play1.GetTarget());
            __Engine.ProcessAttack(action, play1, play2);
        }


        // Update is called once per frame
        void Update()
        {
            switch (state)
            {

                case AnimationState.Fleeing:
                    if (Time.time >= breakTime)
                    {
                        Move();
                        UpdateSaturation();
                    }
                    break;

                case AnimationState.Animating:
                    UnitAnimation anim = GetCurrentAnimation();
                    current_animation = anim;
                    if (anim == null || anim.action.type == Action.Type.Wait)
                    {
                        state = AnimationState.Waiting;
                    }
                    else if (anim.IsFinished())
                    {
                        ContinueToNextAnimation();
                    }
                    else if (anim.action.type == Action.Type.Attack)
                    {
                        if (!anim.HasActed())
                        {
                            ProcessAttack(anim.action);
                            anim.SetHasActed(true);
                        }
                        
                    }
                    else
                    {
                        targetPosition = anim.moveDestination;
                        Move();
                    }

                    break;

                case AnimationState.Waiting:
                    homePosition = transform.position;
                    break;

                default:
                    break;
            }

        }

    }
}