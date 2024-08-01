using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class old_Battalion : MonoBehaviour
{
    public string UnitName = "Default unit";
    public enum UnitClass { Infantry, Archers, Cavalry, HorseArchers, Artillery, Elephants };
    public UnitClass unitclass = UnitClass.Infantry;
    public old_Command command = null;
    public float current_speed = 1f;
    public float max_movement_speed = 1f;
    public float mass = old_BattleEngine.base_InfantryMass;

    public bool falling_back = false;
    public BattleGroup battlegroup;
    public float melee_push = 0f; // number of points towards pushing enemy back
    
    public bool ParthianShot = false;
    public int ammunition = 0;

    public int Prowess = 1;
    public int Confidence = 0;
    public enum MovementMode { Idle, Walking, Running, Fighting, Pursuing, Fleeing };
    public MovementMode movementMode = MovementMode.Idle;

    public int MinimumUnitSize = old_BattleEngine.MinimumUnitSize; // base
    public bool is_dead = false;
    public int men = 100;
    public int starting_men = 100;
    public int morale = 10;
    public float cohesion = 100f;
    public float maxCohesion = 100f;
    public float fatigue = 100f;
    public int discipline = 10; // controls the resistance to losing cohesion
    public int armor = 0;
    public int kills = 0;
    public int formationStrength = 0;

    public Mount mount = null;


    public UnitPathScript UnitPathMarker;

    public Weapon primary_weapon; // main weapon of the unit, can be ranged weapons, polearms, or 2 handed weapons 
    public Weapon side_arm; // a one handed, sheathed weapon used as a backup
    public Weapon impact_weapon; // weapon represents the physical impact of the unit as it charges into an enemy. 
    

    public BattleManager BATTLE_MANAGER;



    public GameObject LOSPrimary;
    public GameObject LOSSecondary;
    
    private float current_saturation = 0f;
    private float saturation_limit = 0f;
    private bool blinkingUp = true;

    private float fallingBackTime = 0;

    public UnitMesh unitMesh;

    // Start is called before the first frame update
    void Start()
    {
        command = old_Command.GetDefaultCommand(this);
        starting_men = men;

        // check if gamestate reference exists
        if (BATTLE_MANAGER == null)
        {
            BATTLE_MANAGER = gameObject.GetComponentInParent<BattleManager>();
        }

        if (impact_weapon == null)
        {
            impact_weapon = new Weapon("Default impact", 1, 0, 0, 0, 0, 0, 0, Weapon.Type.Impact);
        }
        if(mount != null && mount.max_speed > 0)
        {
            this.SetMount(mount);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Use default command if none is set yet.
        if (command == null)
        {
            command = old_Command.GetDefaultCommand(this);
        }

        // first, check if the unit is now dead and should get removed.
        is_dead = CheckDeath();
        if (is_dead)
        {
            Kill();
        }
        else
        {
            // good for icon over units
            Vector3 newPos = old_BattleEngine.FindCenterOfGroup(unitMesh.units);
            transform.position = Vector3.Lerp(transform.position, newPos, 1f);

            // Update morale status
            UpdateMorale();

            // Update Fatigue stat
            UpdateFatigue();

            // Update UI on screen
            UpdateUI();
            
        }
        
    }
    
    public override string ToString()
    {
        return UnitName;
    }

    // Change stats for having a mount
    public void SetMount(Mount mnt)
    {
        this.mount = mnt;
        this.mass = mnt.mass + old_BattleEngine.base_InfantryMass;
        this.max_movement_speed = mnt.max_speed;
        this.discipline = discipline - mnt.discipline_penalty;
        if (discipline < 0)
        {
            discipline = 0;
        }
    }

    // Change stats for dismounting
    public void Dismount()
    {
        this.discipline = discipline + mount.discipline_penalty;
        this.mount = null;
        this.mass = old_BattleEngine.base_InfantryMass;
        this.max_movement_speed = old_BattleEngine.base_infantrySpeed;
        // TODO: set UI to change
        
    }

    

    // update the graphical representation of the unit (UI)
    public void UpdateUI()
    {
        //TODO is it possible to change Fill direciton depending on facing?
        // Update HP bar
        float hp_ratio = men * 1.0f / starting_men;
        //health_bar.SetHealthBarValue(hp_ratio);

        // set cohesion bar
        float coh_ratio = cohesion / 100f;
        //cohesion_bar.SetHealthBarValue(coh_ratio);

        float saturation_factor = current_saturation;
        if (falling_back)
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
            else
            {
            }
        }
        else
        {
            blinkingUp = true;
            saturation_limit = 0;
        }

        // glide toward desired saturation
        if(current_saturation < 0)
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

        float red = battlegroup.TeamColor.r + (1-battlegroup.TeamColor.r)*current_saturation;
        float green = battlegroup.TeamColor.g + (1 - battlegroup.TeamColor.g) * current_saturation;
        float blue = battlegroup.TeamColor.b + (1 - battlegroup.TeamColor.b) * current_saturation;

        //sprite.color = new Color(red, green, blue);
    }

    // Check if the unit is now dead, and should be removed (returns true) or not (returns false)
    public bool CheckDeath()
    {
        
        if (men < MinimumUnitSize)
        {
            print(this.ToString() + " died from having too few men remaining. ");
            return true;
        }
        else if (falling_back && cohesion < 10f && Time.time > fallingBackTime + old_BattleEngine.minimumFallbackTime*1.5)
        {
            print(this.ToString() + " died from being scattered. ");
            return true;
        }
        else if (falling_back && Time.time > fallingBackTime + old_BattleEngine.minimumFallbackTime * 4)
        {
            print(this.ToString() + " died from leaving the combat area. ");
            return true;

        }
        else
        {
            return false;
        }
    }

    public float CalculateFatigueLossRate()
    {
        float loss_rate = 0f;
        int unitWeight = GetUnitWeight();
        float weightFactor = 1 + unitWeight * old_BattleEngine.fatigueLostPerUnitWeight;
        switch (movementMode)
        {
            case MovementMode.Fighting:
                loss_rate = 0.1f * weightFactor;
                break;
            case MovementMode.Running:
                loss_rate = 0.25f * weightFactor;
                break;
            case MovementMode.Pursuing:
                loss_rate = 0.25f * weightFactor;
                break;
            case MovementMode.Walking:
                loss_rate = 0.05f * weightFactor;
                break;
            case MovementMode.Fleeing:
                loss_rate = 0.4f * weightFactor;
                break;
            default:
                loss_rate = -0.05f;
                break;
        }
        return loss_rate;
    }

    // Update fatigue over time
    public void UpdateFatigue()
    {
        float loss_rate = CalculateFatigueLossRate();
        fatigue -= loss_rate * Time.deltaTime;
        if(fatigue < 0)
        {
            fatigue = 0f;
        }
        if(fatigue > 100)
        {
            fatigue = 100f;
        }
    }

    // Find the total weight of all equipment carried by the unit
    public int GetUnitWeight()
    {
        int wgt = 0;
        wgt += armor; // TODO: For now assume each point of armor = 1 pt of weight
        wgt += primary_weapon.weight;
        if(side_arm != null)
            wgt += side_arm.weight;
        return wgt;
    }

    

    // Get the melee weapon this unit can fight with
    public Weapon GetMeleeWeapon()
    {
        if (primary_weapon.type == Weapon.Type.Ranged)
        {
            return side_arm; // use sidearm if this is close combat
        }
        else
        {
            return primary_weapon;
        }
    }

    // Get the ranged weapon this unit has, if any. Otherwise returns null
    public Weapon GetRangedWeapon()
    {

        if (primary_weapon.type == Weapon.Type.Ranged)
        {
            return primary_weapon;
        }
        else
        {
            return null;
        }
    }

    // Gets the impact weapon this unit has, if any. Otherwise returns null
    public Weapon GetImpactWeapon()
    {
        if(mount != null)
        {
            return mount.ImpactWeapon;
        }
        return impact_weapon;
    }

    // Updates morale variable given current battle context
    void UpdateMorale()
    {
        int tmp_morale = old_BattleEngine.base_UnitMorale;

        // Gain morale for formation strength
        formationStrength = old_BattleEngine.CalculateFormationStrength(this);
        tmp_morale += formationStrength;

        // Taken casualties lowers morale
        float hp_ratio = men / (starting_men * 1.0f);
        int lost_chunks = 10 - Mathf.CeilToInt(hp_ratio / 0.1f);
        tmp_morale -= lost_chunks; // lose 1 point of morale per 10% casualties

        // Gain default morale from confidence level
        tmp_morale += Confidence;

        // Retreat is impossible
        if (false)
        {
            tmp_morale += 5;
        }

        // Gaining or losing ground
        if (melee_push >= 1)
        {
            tmp_morale += 1;
        }
        else if (melee_push < 0)
        {
            tmp_morale += -1;
        }

        // Our general's honor

        // Enemy general's dread

        // If team morale is 0, our morale must now be 0
        if(battlegroup.TeamMorale <= 0)
        {
            this.morale = 0;
            falling_back = true;
            fallingBackTime = Time.time;
        }
        else
        {
            // Update morale to reflect this status. Check if it is now fleeing.
            // if morale is 0 or less, the unit is now fleeing out of control
            if (tmp_morale <= 0)
            {
                if (!falling_back)
                {
                    fallingBackTime = Time.time;
                }
                falling_back = true;
            }
            // if the command is ordering the unit to fall back, then do so.
            else if (command.should_fall_back && !this.command.IsComplete())
            {
                if (!falling_back)
                {
                    fallingBackTime = Time.time;
                }
                falling_back = true;
            }
            // if the command is not to fall back, and the unit has positive morale, then stop falling back.
            else if (falling_back && tmp_morale > 0 && Time.time > fallingBackTime + old_BattleEngine.minimumFallbackTime)
            {
                falling_back = false;
            }
            // in this case, nothing has changed so leave the boolean value the same
            else
            {
                // pass: falling_back doesn't change
                // I think this only happens when a unit is not falling back AND morale is fine. 
                // So that'd be the same as just setting it to false.
            }

            this.morale = tmp_morale;
        }
    }

    // Remove self from the game
    public void Kill()
    {
        gameObject.SetActive(false);
    }

    

    
}
