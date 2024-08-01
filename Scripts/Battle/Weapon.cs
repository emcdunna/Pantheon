using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon
{
    public int damage, penetration, weight, melee_block, ranged_block = 0;
    public float range, attack_delay = 1.0f;
    public int base_shots = 10;
    public enum Type { Melee, Ranged, Impact };
    public Type type = Type.Melee;
    public string name = "Default Weapon";
    public float recoil = 1f;
    public GameObject Missile_Prefab = null;


    public Weapon(string name, int damage, float range, int penetration,
                  int weight, int melee_block, int ranged_block, float attack_delay, Type type)
    {
        this.name = name;
        this.damage = damage;
        this.range = range;
        this.penetration = penetration;
        this.weight = weight;
        this.melee_block = melee_block;
        this.ranged_block = ranged_block;
        this.type = type;
        this.attack_delay = attack_delay;
    }

    public override string ToString()
    {
        return this.name;
    }


}
