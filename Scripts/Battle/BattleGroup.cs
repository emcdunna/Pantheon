using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGroup : MonoBehaviour
{
    public List<old_Battalion> Units = new List<old_Battalion>();
    public old_Player player;
    public int team = 0;
    public Color TeamColor;

    public float TeamMorale = 100f;
    public UI_Bar MoraleBar;
    public BattleManager BATTLE_MANAGER;
    public List<old_Building> Buildings = new List<old_Building>();

    public int startingMen = 0;
    public int currentMen = 0;
    public int kills = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTeamMorale();

        UpdateUI();
    }

    public void AddUnit(old_Battalion unit)
    {
        unit.battlegroup = this;
        Units.Add(unit);
    }

    public void RemoveUnit(old_Battalion unit)
    {
        unit.battlegroup = null;
        Units.Remove(unit);
    }

    public void UnitClicked(old_Battalion unit)
    {
        if (player != null)
        {
            player.UpdateSelectedUnit(unit);
        }
    }

    // Updates current morale level of the battle group
    public void UpdateTeamMorale()
    {
        float ml = 0;
        int baseMorale = Units.ToArray().Length;
        foreach (old_Battalion unit in Units)
        {
            if (unit != null)
            {

                if (unit.is_dead)
                {
                    ml += -1f;
                }
                else if (!unit.falling_back)
                {
                    ml += 1f;
                }
                else
                {
                    ml += -0.25f;
                } 

            }
        }
        foreach(old_Building bld in Buildings)
        {
            baseMorale += bld.teamMoraleBonus;
            if (!bld.is_dead)
            {
                if(bld.GetHPRatio() <= 0.5f)
                {
                    ml += 0;
                }
                else
                {
                    ml += bld.teamMoraleBonus;
                }
            }
            else
            {
                ml -= 1f * bld.teamMoraleBonus;
            }
        }
        float new_morale = (ml / baseMorale);
        TeamMorale = Mathf.Lerp(TeamMorale, new_morale, 0.2f);
        
    }

    public void UpdateUI()
    {
        // set Morale bar
        float coh_ratio = TeamMorale;
        MoraleBar.SetHealthBarValue(coh_ratio);
        MoraleBar.SetHealthBarColor(TeamColor);

        int tmp_men = 0;
        int tmp_kills = 0;
        int tmp_men_start = 0;
        foreach (old_Battalion u in Units)
        {
            tmp_men += u.men;
            tmp_men_start += u.starting_men;
            tmp_kills += u.kills;
        }
        currentMen = tmp_men;
        startingMen = tmp_men_start;
        kills = tmp_kills;
    }

    public List<old_Battalion> GetSubsetOfUnits(old_Battalion.UnitClass unitclass)
    {
        List<old_Battalion> units = new List<old_Battalion>();

        foreach(old_Battalion u in Units)
        {
            if(u.unitclass == unitclass)
            {
                units.Add(u);
            }
        }
        return units;
    }

    
}
