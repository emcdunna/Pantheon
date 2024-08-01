                using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mount
{
    public string name = "default";
    public float max_speed, mass = 0f;
    public Weapon ImpactWeapon;
    public int discipline_penalty = 0;

    public Mount(string name, float max_speed, float mass, int discipline_penalty, int impactDamage)
    {
        this.name = name;
        this.max_speed = max_speed;
        this.mass = mass;
        this.discipline_penalty = discipline_penalty;
        this.ImpactWeapon = new Weapon("Impact", impactDamage, 0, 0, 0, 0, 0, 0, Weapon.Type.Impact);
    }

    public override string ToString()
    {
        return this.name;
    }
}
