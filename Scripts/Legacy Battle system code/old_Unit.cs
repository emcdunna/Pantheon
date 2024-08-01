using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class old_Unit : MonoBehaviour
{
    public Vector3 desiredPosition;
    private Rigidbody rb;

    public float maxVelocity = 1f;
    //public float max_acceleration = 1;

    private float maxSqrVelocity = 0;
    private SpriteRenderer spriteRenderer;
    public Vector2 formationOffset;
    public Vector3 spreadDirection;
    public UnitMesh unitMesh;
    public old_Battalion battalion;
    public GameObject highlight;
    public float orderDelay = 0; // time at which they have to wait until it can start moving
    
    public enum MovementMode { Idle, Walking, Running, Fighting, Pursuing, Fleeing };
    public MovementMode movementMode = MovementMode.Idle;

    public old_Unit Target;
    public float nextAttack = 0;

    private float current_speed = 0f;
    public old_Terrain current_terrain = null;

    public int Advantages = old_BattleEngine.base_Advantages;
    public int Disadvantages = old_BattleEngine.base_Disadvantages;
    public AttackDeck deck;

    public Weapon.Type fighting_mode = Weapon.Type.Melee;

    private BattleManager BATTLE_MANAGER;

    private float previous_speed = 0;
    public float momentum = 0;
    public int ammunition = 0;

    public int melee_push = 0;

    // Use this for initialization
    void Start()
    {
        deck = old_BattleEngine.GenerateAttackDeck();
        desiredPosition = transform.position;
        BATTLE_MANAGER = battalion.BATTLE_MANAGER;
        rb = this.GetComponent<Rigidbody>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        maxSqrVelocity = maxVelocity * maxVelocity;
        spreadDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
    }

    // Update is called once per frame
    void Update()
    {

        // Update momentum, for charge impact and bonus damage
        UpdateMomentum();


        // Calculate Advantages/Disadvantages
        UpdateAdvantages();

        // Attack, if possible
        TryAttack();

        // Update Target, sets ranged/melee attack type
        SetTarget();

        // Move troops
        Move();

        // Play SFX regarding current actions
        PlaySFX();



    }

    public void UpdateSortingOrder(int newOrder)
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = newOrder;
    }

    private void Move()
    {

        // terrain reduces current speed
        if (current_terrain != null)
        {
            current_speed = maxVelocity * (1f - current_terrain.speedPenalty);
        }
        else
        {
            current_speed = maxVelocity;
        }

        // fatigue also reduces current speed
        current_speed = current_speed * ((battalion.fatigue + 200) / 300f);

        Vector3 direction = desiredPosition - transform.position;

        

        float remainingDistanceSqr = direction.sqrMagnitude;
        if(remainingDistanceSqr > 0.01f)
        {
            rb.AddForce(direction.normalized * current_speed * Time.deltaTime);
            //rb.velocity = direction.normalized * Time.deltaTime * current_speed;
        }
        else
        {

        }

        
    }


    // Makes the appropriate attack if possible
    public void TryAttack()
    {
        // Check if the unit can currently make an attack
        if (CanAttack())
        {
            // Only do not attack if using a ranged weapon AND commanded to hold fire
            if (fighting_mode != Weapon.Type.Ranged || !battalion.command.should_hold_fire)
            {
                battalion.BATTLE_MANAGER.Attack(this, Target, fighting_mode);
            }
        }
    }

    // returns true if it has been long enough to allow attacks
    bool CanAttack()
    {
        if (Time.time >= nextAttack && Target != null)
        {
            return true;
        }
        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject target = other.gameObject;
        old_Unit hitUnit = target.GetComponent<old_Unit>();
        if (hitUnit != null && !hitUnit.battalion.is_dead)
        {
            if (!battalion.falling_back)
            {
                if (hitUnit.battalion.battlegroup.team != battalion.battlegroup.team)
                {
                    // impact special attack
                    print(this.ToString() + " making impact attack on " + hitUnit.ToString());
                    BATTLE_MANAGER.Attack(this, hitUnit, Weapon.Type.Impact);
                    previous_speed = 0;
                }
            }
        }
    }

    // Sets the units current Momentum stat
    public void UpdateMomentum()
    {
        switch (movementMode)
        {
            // if standing still, momentum is 0
            case MovementMode.Idle:
                momentum = 0f;
                break;
            // if pushing into an enemy unit, momentum drops to 0 after impact
            case MovementMode.Fighting:
                break;
            // if moving in any other way, momentum is speed * mass
            default:
                momentum = Mathf.Abs(previous_speed) * battalion.mass;
                break;
        }
        if (momentum < 0)
        {
            momentum = 0f;
        }
    }

    public Vector3 GetFacingVector()
    {
        return unitMesh.transform.up;
    }


    // Updates Advantages/Disadvantages deck
    void UpdateAdvantages()
    {
        // Reset to base value first
        Advantages = old_BattleEngine.base_Advantages;
        Disadvantages = old_BattleEngine.base_Disadvantages;

        // TODO high ground advantage
        if (current_terrain != null)
        {
            Advantages += current_terrain.elevationBonus;
        }

        // Prowess grants additional advantages in combat
        Advantages += battalion.Prowess;

        // high morale advantage
        if (battalion.morale >= 15)
        {
            Advantages += 3;
        }
        else if (battalion.morale >= 10)
        {
            Advantages += 2;
        }
        else if (battalion.morale >= 5)
        {
            Advantages += 1;
        }

        // low cohesion disadvantage
        //int lost_cohesion = 5 - Mathf.CeilToInt(cohesion / 20);
        //Disadvantages += lost_cohesion; // add one disadvantage per every 20% cohesion lost

        // reach disadvantage
        if (Target != null && fighting_mode == Weapon.Type.Melee)
        {
            Weapon enemy_weapon = Target.battalion.GetMeleeWeapon();
            Weapon our_weapon = this.battalion.GetMeleeWeapon();
            if (enemy_weapon.range > our_weapon.range)
            {
                int reach_difference = Mathf.CeilToInt(enemy_weapon.range - our_weapon.range);
                Disadvantages += reach_difference; // must add one disadvantage for each point of reach the enemy has over us
            }
        }

        // Update Deck if needed
        deck.Update(this.Advantages, this.Disadvantages);
    }

    public void OnMouseEnter()
    {
        BATTLE_MANAGER.player.SuggestMouseOver(this.battalion);

    }

    public void OnMouseExit()
    {
        BATTLE_MANAGER.player.NotMouseOver(this.battalion);
    }

    public void PlaySFX()
    {
        switch (battalion.movementMode)
        {
            case old_Battalion.MovementMode.Idle:
                return;
            case old_Battalion.MovementMode.Walking:
                //PlayMarchingSound(1, 0.45f);
                return;
            case old_Battalion.MovementMode.Fighting:
                return;
            default:
                //PlayMarchingSound(1, 0.3f);
                break;
        }

    }

    // sets variable Target and fighting_mode
    void SetTarget()
    {
        List<old_Battalion> possible_melee_targets = new List<old_Battalion>();
        int total = 0;
        /*if (in_melee)
        {
            this.fighting_mode = Weapon.Type.Melee;
            foreach (Battalion b in touching_units)
            {
                if (!b.is_dead && b.battlegroup.team != battlegroup.team)
                {
                    possible_melee_targets.Add(b);
                    total += 1;
                }
            }
            if (total > 0)
            {
                int unit_index = Random.Range(0, total);
                Target = possible_melee_targets[unit_index];
            }
            else
            {
                Target = null;
            }

        }
        else if ((primary_weapon.type == Weapon.Type.Ranged) && ammunition > 0)
        {
            Battalion primary_target = BattleEngine.GetTarget(this, primary_weapon);
            this.fighting_mode = Weapon.Type.Ranged;
            this.Target = primary_target;
        }
        else**/
        if(true)
        {
            this.fighting_mode = Weapon.Type.Melee;
            Target = null;
        }
    }

    private void FixedUpdate()
    {
        
    }
}