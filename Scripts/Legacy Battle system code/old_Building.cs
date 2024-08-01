using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class old_Building : MonoBehaviour
{

    public int hp = 500;
    private int starting_hp;
    public int teamMoraleBonus = 3; // how much is the team's morale improved by this building

    // Determines which kinds of weapons are capable of damaging the building
    public enum FortificationLevel { Weak, FortifiedWood, SimpleStone, ReinforcedStone };
    public FortificationLevel fortlevel = FortificationLevel.Weak;
    public bool is_dead = false;
    public BattleGroup battlegroup;
    public List<old_Battalion> touching_units = new List<old_Battalion>();
    public UI_Bar health_bar;
    public bool underAttack = false;

    // Start is called before the first frame update
    void Start()
    {
        starting_hp = hp;
    }

    public float GetHPRatio()
    {
        return hp * 1.0f / starting_hp;
    }

    // returns whether the weapon is capable of damaging the building
    public bool CanAttack(Weapon weapon)
    {
        switch (fortlevel)
        {
            case FortificationLevel.Weak:
                return true;
            default:
                return false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // first, check if the unit is now dead and should get removed.
        is_dead = CheckDeath();
        if (is_dead)
        {
            Kill();
        }
        // Update UI on screen
        UpdateUI();
    }

    // update the graphical representation of the unit (UI)
    public void UpdateUI()
    {
        // Update HP bar
        float hp_ratio = hp * 1.0f / starting_hp;
        health_bar.SetHealthBarValue(hp_ratio);
    }

    // Check if the unit is now dead, and should be removed (returns true) or not (returns false)
    public bool CheckDeath()
    {
        if (hp <= 0)
        {
            print(this.ToString() + " was destroyed. ");
            return true;
        }
        else
        {
            return false;
        }
    }

    // Remove self from the game
    public void Kill()
    {
        //GameObject.Destroy(gameObject, 0.01f);
        gameObject.SetActive(false);
    }

    public void OnTriggerStay(Collider other)
    {
        GameObject target = other.gameObject;
        old_Battalion t_btl = target.GetComponent<old_Battalion>();
        if (t_btl != null && !t_btl.is_dead)
        {
            // When two units on the same team overlap
            if (t_btl.battlegroup.team != battlegroup.team)
            {
                underAttack = true;
                Weapon pillageWeapon = t_btl.GetMeleeWeapon();
                int damage = old_BattleEngine.RandomRound(pillageWeapon.damage * Time.deltaTime / pillageWeapon.attack_delay);
                hp -= damage;
                print(this.name + " being pillaged for " + damage.ToString() + " damage.");
            }

        }
        else if(t_btl != null && t_btl.is_dead)
        {

        }
    }

}
