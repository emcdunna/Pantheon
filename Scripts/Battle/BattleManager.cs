using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public bool isPaused = false;
    public List<BattleGroup> Factions = new List<BattleGroup>();
    public GameObject FactionPrefab;
    public old_Player player;
    public UI_Bar LeftMoraleBar;
    public UI_Bar RightMoraleBar;
    public enum VictoryStatus { Undecided, Team0Wins, Team1Wins, Draw };
    public VictoryStatus victoryStatus = VictoryStatus.Undecided;
    public Text ObjectiveText;
    public UISoundManager soundManager;
    public List<BattleEvent> EventQueue = new List<BattleEvent>();
    public GameObject unitPrefab;
    public GameObject FogOfWarCanvas;

    public Sprite ArcherSprite;
    public Sprite HorseArcherSprite;
    public Sprite ElephantSprite;
    public Sprite HalberdierSprite;
    public Sprite KnightsSprite;
    public Sprite CrossbowmenSprite;
    public Sprite SpearmanSprite;

    public GameObject arrowPrefab;
    public GameObject axePrefab;
    public GameObject swordPrefab;
    public GameObject spearPrefab;
    public GameObject javelinPrefab;
    public GameObject halberdPrefab;


    // Start is called before the first frame update
    void Start()
    {
        FogOfWarCanvas.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Check event queue and trigger events
        CheckEvents();

        // Determines if anyone has won the battle yet
        UpdateVictoryStatus();

        // Specifies the Objectives text box
        UpdateUI();
    }

    public void CheckEvents()
    {
        if(EventQueue.ToArray().Length > 0)
        {
            TriggerEvent(EventQueue[0]);
        }
    }

    public void EnqueueEvent(BattleEvent be)
    {
        EventQueue.Add(be);
    }

    public void TriggerEvent(BattleEvent be)
    {
        UnitStats unitStats = MedievalUnits.GetUnitStats(be.unitID);
        SpawnUnit(be.battleGroup, be.location1, be.location2, unitStats);
        EventQueue.Remove(be);
    }

    // Creates a new unit with given settings, adds it to this battlegroup
    public void SpawnUnit(BattleGroup battlegroup, Vector3 position, Vector3 destination, UnitStats unitStats)
    {
        // Position, rotation, adding to the army
        GameObject newUnit = Instantiate(unitPrefab);
        newUnit.transform.position = position;
        old_Battalion b = newUnit.GetComponent<old_Battalion>();
        b.command = old_Command.GetDefaultCommand(b);
        b.command.destination = destination;
        b.command.name = "Advance";
        //b.transform.rotation = Quaternion.Euler(0, 0, b.facing.GetRotation());
        b.battlegroup = battlegroup;
        unitStats.ApplyToBattalion(b);
        battlegroup.AddUnit(b);
        b.BATTLE_MANAGER = this;
        if(battlegroup.player == player)
        {
            b.LOSPrimary.SetActive(true);
            b.LOSSecondary.SetActive(true);
        }
        UpdateUnitSprite(b);
        UpdateWeaponPrefabs(b.primary_weapon);
        UpdateWeaponPrefabs(b.side_arm);
    }

    public void UpdateUnitSprite(old_Battalion unit)
    {
        Sprite newSprite = SpearmanSprite;
        if (unit.UnitName.Contains("Crossbowmen"))
        {
            newSprite = CrossbowmenSprite;
        }
        else if (unit.UnitName.Contains("Elephant"))
        {
            newSprite = ElephantSprite;
        }
        else if (unit.unitclass == old_Battalion.UnitClass.Cavalry)
        {
            newSprite = KnightsSprite;
        }
        else if (unit.unitclass == old_Battalion.UnitClass.HorseArchers)
        {
            newSprite = HorseArcherSprite;
        }
        else if(unit.unitclass == old_Battalion.UnitClass.Archers)
        {
            newSprite = ArcherSprite;
        }
        else if (unit.UnitName.Contains("Halberdier"))
        {
            newSprite = HalberdierSprite;
        }

        SpriteRenderer sr = unit.GetComponent<SpriteRenderer>();
        sr.sprite = newSprite;
    }

    public void UpdateWeaponPrefabs(Weapon weapon)
    {
        if (weapon == null)
        {
            return;
        }
        else if(weapon.name == "Javelin")
        {
            weapon.Missile_Prefab = javelinPrefab;
        }
        else if (weapon.name.Contains("Axe"))
        {
            weapon.Missile_Prefab = axePrefab;
        }
        else if (weapon.name.Contains("Halberd"))
        {
            weapon.Missile_Prefab = halberdPrefab;
        }
        else if (weapon.name.Contains("Spear") || weapon.name.Contains("Pike") || weapon.name.Contains("Lance"))
        {
            weapon.Missile_Prefab = spearPrefab;
        }
        else if(weapon.name.Contains("Sword"))
        {
            weapon.Missile_Prefab = swordPrefab;
        }
        else
        {
            weapon.Missile_Prefab = arrowPrefab;
        }
    }

    public void UpdateUI()
    {
        if (ObjectiveText != null)
        {
            if (victoryStatus == VictoryStatus.Team0Wins)
            {
                ObjectiveText.text = Factions[0].name + " wins the battle!";
            }
            else if (victoryStatus == VictoryStatus.Team1Wins)
            {
                ObjectiveText.text = Factions[1].name + " wins the battle!";
            }
            else
            {
                ObjectiveText.text = "Objective: Rout the enemy army";
            }
        }
    }

    // Check if either team has won the battle
    public void UpdateVictoryStatus()
    {
        foreach (BattleGroup f in Factions)
        {
            if (f.TeamMorale <= 0)
            {
                if (f.team == 0)
                {
                    victoryStatus = VictoryStatus.Team1Wins;

                }
                else if (f.team == 1)
                {
                    victoryStatus = VictoryStatus.Team0Wins;
                }
            }

        }
    }

    // Toggles the pause game state
    public void TogglePause()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
        isPaused = !isPaused;
        // TODO show pause screen

    }


    // Spawns a random sized battle made up of random units
    public void QuickBattle()
    {
        if (Factions.ToArray().Length < 2)
        {
            AddFaction("England", Color.red, 0);
            AddFaction("France", Color.blue, 1);
        }

    }


    // Adds a faction to the game, with colors and morale bars
    public void AddFaction(string name, Color factionColor, int team_index)
    {
        GameObject bg_go = Instantiate(FactionPrefab);
        bg_go.name = name;
        BattleGroup bg = bg_go.GetComponent<BattleGroup>();
        bg.team = team_index;
        bg.TeamColor = factionColor;
        // Human player team is team 0
        if (team_index == 0)
        {
            bg.player = player;
            bg.MoraleBar = LeftMoraleBar;
        }
        else
        {
            bg.MoraleBar = RightMoraleBar;
        }
        bg.BATTLE_MANAGER = this;
        Factions.Add(bg);
    }


    // TODO a way to spawn a battlefield
    public void CreateBattlefield()
    {

    }

    // spawns a projectile, fires it at the enemy
    public void DrawAttack(Weapon weapon, old_Unit attacker, float delay, 
                            Vector3 offset_transform, int kills, int cohesionDamage)
    {
        Vector3 offset = offset_transform;
        Vector3 position = attacker.transform.position + offset;
        GameObject newProjectile = Instantiate(weapon.Missile_Prefab, position, attacker.transform.rotation);
        Renderer r = newProjectile.GetComponent<Renderer>();
        r.enabled = false;
        Projectile projectile = newProjectile.GetComponent<Projectile>();
        projectile.offset = offset;
        r.sortingOrder = 9;
        projectile.target = attacker.Target;
        projectile.SetupSound(soundManager);
        projectile.fire_delay = Time.time + delay;
        projectile.weapon = weapon;
        projectile.shooter = attacker;
        projectile.killDamage = kills;
        projectile.cohesionDamage = cohesionDamage;
        projectile.Shoot();
    }

    public void MakeMarchingSFX(Vector3 position)
    {

    }


    public void Attack(old_Unit attacker, old_Unit target, Weapon.Type attack_type)
    {
        AttackCard attack = attacker.deck.Draw();
        Weapon attacking_weapon = null;

        float block_rate = 0;
        float flanking_bonus = old_BattleEngine.CalculateFlankingBonus(attacker.GetFacingVector(),
                                                                   target.GetFacingVector());
        int shots = Mathf.RoundToInt(attack.damage_factor / 0.5f);

        switch (attack_type)
        {
            case Weapon.Type.Melee:
                attacking_weapon = attacker.battalion.GetMeleeWeapon();
                block_rate = old_BattleEngine.CalculateMeleeBlockRate(target, attacking_weapon, flanking_bonus);

                // Update momentum based on attack card
                attacker.melee_push += attack.impact;
                break;
            case Weapon.Type.Ranged:
                attacking_weapon = attacker.battalion.GetRangedWeapon();
                block_rate = old_BattleEngine.CalculateRangedBlockRate(target, attacking_weapon, flanking_bonus);
                attacker.ammunition -= 1;
                shots = shots * 3;
                break;
            case Weapon.Type.Impact:
                attacking_weapon = attacker.battalion.GetImpactWeapon();
                block_rate = old_BattleEngine.CalculateImpactBlockRate(target, attacking_weapon, flanking_bonus);
                break;
            default:
                break;
        }
        // Attacker suffers fatigue loss for attacking
        float fatigueLost = old_BattleEngine.base_FatigueLostPerAttack + attacking_weapon.weight * old_BattleEngine.fatigueLostPerWeaponWeight;
        attacker.battalion.fatigue -= fatigueLost/attacker.unitMesh.size;

        int total_cohesion_damage = 0;
        int total_damage = 0;

        // if the weapon is not found, attack does nothing
        if (attacking_weapon == null)
        {
            return;
        }
        if (attack_type == Weapon.Type.Impact)
        {
            // Impact damage only causes casualties, no cohesion delay.
            float raw_damage = attacking_weapon.damage * attacker.momentum * attack.damage_factor;
            total_damage = old_BattleEngine.RandomRound(raw_damage * (1 - block_rate));
            target.battalion.men -= total_damage;
            attacker.battalion.kills += total_damage;

            // No attack delay is caused, but momentum is set to 0.
            attacker.momentum = 0;
        }
        else
        {
            // Calculate real casualties, subtract men from the target unit
            float raw_damage = attacking_weapon.damage * attack.damage_factor;
            total_damage = old_BattleEngine.RandomRound(raw_damage * (1 - block_rate));

            // Calculate impact of damage on unit cohesion
            float cohesion_damage = (raw_damage + total_damage) / old_BattleEngine.cohesion_loss_dividend;
            if (attacking_weapon.type == Weapon.Type.Ranged)
            {
                cohesion_damage = cohesion_damage * 1.33f;
            }
            float cohesion_block_rate = old_BattleEngine.CalculateCohesionBlockRate(target.battalion);
            total_cohesion_damage = old_BattleEngine.RandomRound(cohesion_damage * (1 - cohesion_block_rate));

            // update timer for next attack
            float delay = attacking_weapon.attack_delay + Random.Range(0, 0.15f * attacking_weapon.attack_delay);
            if (attacker.battalion.falling_back)
            {
                delay = delay * 2;  // attack half as frequently if falling back
            }
            attacker.nextAttack = Time.time + delay;

            float wait = 0;
            Vector3 offset = Vector3.zero;

            // Draw the attacks as projectiles            
            this.DrawAttack(attacking_weapon, attacker, wait, offset, total_damage, total_cohesion_damage);
        }


    }

}
