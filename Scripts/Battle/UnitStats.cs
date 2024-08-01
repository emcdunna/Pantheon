using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats
{
    public int size, prowess, confidence, discipline, ammunition, armor = 0;
    public float maxSpeed = 0;
    public old_Battalion.UnitClass unitClass = old_Battalion.UnitClass.Infantry;
    public Weapon primary, sidearm = null;
    public Mount mount = null;
    string name = "Default unit";

    public UnitStats(string name, int size, int prowess, int confidence, int discipline, int ammunition, int armor, 
                        float maxSpeed, old_Battalion.UnitClass unitClass, Weapon primary, Weapon sidearm,
                        Mount mount)
    {
        this.name = name;
        this.size = size;
        this.prowess = prowess;
        this.confidence = confidence;
        this.discipline = discipline;
        this.ammunition = ammunition;
        this.armor = armor;
        this.maxSpeed = maxSpeed;
        this.unitClass = unitClass;
        this.primary = primary;
        this.sidearm = sidearm;
        this.mount = mount;
    }

    public void ApplyToBattalion(old_Battalion b)
    {
        b.UnitName = name;
        b.men = size;
        b.starting_men = size;
        b.Confidence = confidence;
        b.Prowess = prowess;
        b.discipline = discipline;
        b.ammunition = ammunition;
        b.armor = armor;
        b.max_movement_speed = maxSpeed;
        b.unitclass = unitClass;
        b.primary_weapon = primary;
        b.side_arm = sidearm;
        if(mount != null)
        {
            b.SetMount(mount);
        }

        // TODO change image on screen to match
    }
}
