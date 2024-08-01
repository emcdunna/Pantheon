using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MedievalUnits
{

    // Weapons
    public static Weapon WarBow = new Weapon("War Bow", 12, 120, 5, 3, 0, 0, 2.2f, Weapon.Type.Ranged);
    public static Weapon Longbow = new Weapon("Longbow", 11, 120, 3, 3, 0, 0, 2.1f, Weapon.Type.Ranged);
    public static Weapon Bow = new Weapon("Bow", 10, 100, 1, 2, 0, 0, 2f, Weapon.Type.Ranged);
    public static Weapon CompositeBow = new Weapon("Composite Bow", 11, 100, 2, 2, 0, 0, 1.8f, Weapon.Type.Ranged);
    public static Weapon Crossbow = new Weapon("Crossbow", 15, 90, 4, 5, 0, 0, 3.6f, Weapon.Type.Ranged);
    public static Weapon HeavyCrossbow = new Weapon("Heavy Crossbow", 16, 90, 6, 7, 0, 0, 4.4f, Weapon.Type.Ranged);
    public static Weapon Javelin = new Weapon("Javelin", 20, 45, 2, 5, 0, 0, 2.5f, Weapon.Type.Ranged);
    public static Weapon ShortSword = new Weapon("Short Sword", 8, 2, 2, 2, 3, 0, 1f, Weapon.Type.Melee);
    public static Weapon Sword = new Weapon("Sword", 10, 3, 3, 2, 3, 0, 1f, Weapon.Type.Melee);
    public static Weapon HandAxe = new Weapon("Hand Axe", 14, 3, 0, 3, 0, 0, 1f, Weapon.Type.Melee);
    public static Weapon LongAxe = new Weapon("Long Axe", 18, 4, 1, 4, 0, 0, 1f, Weapon.Type.Melee);
    public static Weapon LongSword = new Weapon("Long Sword", 12, 4, 3, 3, 4, 0, 1f, Weapon.Type.Melee);
    public static Weapon Spear = new Weapon("Spear", 10, 6, 4, 3, 0, 0, 1f, Weapon.Type.Melee);
    public static Weapon Lance = new Weapon("Lance", 11, 6, 4, 4, 0, 0, 1f, Weapon.Type.Melee);
    public static Weapon ShortPike = new Weapon("Short Pike", 11, 8, 4, 5, 0, 0, 1f, Weapon.Type.Melee);
    public static Weapon LongPike = new Weapon("Pike", 12, 10, 4, 7, 0, 0, 1f, Weapon.Type.Melee);
    public static Weapon Halberd = new Weapon("Halberd", 13, 7, 4, 6, 0, 0, 1f, Weapon.Type.Melee);
    public static Weapon Poleaxe = new Weapon("Pole Axe", 15, 5, 5, 6, 0, 0, 1f, Weapon.Type.Melee);
    public static Weapon Mace = new Weapon("Mace", 12, 3, 7, 3, 0, 0, 1f, Weapon.Type.Melee);

    // Mounts
    public static Mount Destrier = new Mount("Destrier", 1.8f, 16, 2, 8);
    public static Mount HeavyHorse = new Mount("Heavy Horse", 1.8f, 13, 3, 6);
    public static Mount LightHorse = new Mount("Light Horse", 2f, 11, 4, 4);
    public static Mount Horse = new Mount("Horse", 1.7f, 10, 5, 3);
    public static Mount NoMount = null;

    // Units
    public static UnitStats Longbowmen = new UnitStats("Longbowmen", 150, 3, 2, 9, 30, 10, 1f, old_Battalion.UnitClass.Archers, Longbow, Sword, NoMount);
    public static UnitStats YeomenArchers = new UnitStats("Yeomen Archers", 150, 4, 3, 10, 30, 12, 1f, old_Battalion.UnitClass.Archers, WarBow, Sword, NoMount);
    public static UnitStats LevyArchers = new UnitStats("Levy Archers", 150, 1, 0, 6, 25, 3, 1f, old_Battalion.UnitClass.Archers, Bow, ShortSword, NoMount);
    public static UnitStats LevySpearmen = new UnitStats("Levy Spearmen", 200, 1, 0, 6, 0, 9, 1f, old_Battalion.UnitClass.Infantry, Spear, ShortSword, NoMount);
    public static UnitStats SerjeantHalberdiers = new UnitStats("Serjeant Halberdiers", 200, 2, 1, 11, 0, 13, 1f, old_Battalion.UnitClass.Infantry, Halberd, ShortSword, NoMount);
    public static UnitStats SerjeantPikemen = new UnitStats("Serjeant Pikemen", 200, 2, 1, 11, 0, 13, 1f, old_Battalion.UnitClass.Infantry, ShortPike, ShortSword, NoMount);
    public static UnitStats SerjeantCrossbowmen = new UnitStats("Serjeant Crossbowmen", 150, 2, 1, 9, 25, 12, 1f, old_Battalion.UnitClass.Archers, Crossbow, ShortSword, NoMount);
    public static UnitStats GenoeseCrossbowmen = new UnitStats("Genoese Crossbowmen", 150, 3, 1, 11, 28, 14, 1f, old_Battalion.UnitClass.Archers, HeavyCrossbow, Sword, NoMount);
    public static UnitStats Men_at_arms = new UnitStats("Men-at-arms", 100, 2, 1, 11, 0, 14, 1f, old_Battalion.UnitClass.Cavalry, Lance, Sword, HeavyHorse);
    public static UnitStats Knights = new UnitStats("Knights", 100, 3, 2, 12, 0, 16, 1f, old_Battalion.UnitClass.Cavalry, Lance, LongSword, Destrier);
    public static UnitStats Hobilars = new UnitStats("Hobilars", 100, 1, 1, 9, 0, 9, 1f, old_Battalion.UnitClass.Cavalry, Spear, ShortSword, LightHorse);
    public static UnitStats Jinites = new UnitStats("Jinites", 100, 2, 2, 11, 10, 12, 1f, old_Battalion.UnitClass.HorseArchers, Javelin, Sword, LightHorse);
    public static UnitStats FootKnights = new UnitStats("Foot Knights", 200, 3, 2, 12, 0, 16, 1f, old_Battalion.UnitClass.Infantry, Poleaxe, LongSword, NoMount);


    public enum UnitID { Longbowmen, YeomenArchers, LevyArchers, LevySpearmen, SerjeantHalberdiers,
        SerjeantPikemen, SerjeantCrossbowmen, GenoeseCrossbowmen, Men_at_arms, Knights,
        Hobilars, Jinites, FootKnights
    }

    public static UnitStats GetUnitStats(UnitID unitID)
    {
        switch (unitID)
        {
            case UnitID.Longbowmen:
                return Longbowmen;
            case UnitID.YeomenArchers:
                return YeomenArchers;
            case UnitID.LevyArchers:
                return LevyArchers;
            case UnitID.LevySpearmen:
                return LevySpearmen;
            case UnitID.SerjeantHalberdiers:
                return SerjeantHalberdiers;
            case UnitID.SerjeantPikemen:
                return SerjeantPikemen;
            case UnitID.SerjeantCrossbowmen:
                return SerjeantCrossbowmen;
            case UnitID.GenoeseCrossbowmen:
                return GenoeseCrossbowmen;
            case UnitID.Men_at_arms:
                return Men_at_arms;
            case UnitID.Knights:
                return Knights;
            case UnitID.Hobilars:
                return Hobilars;
            case UnitID.Jinites:
                return Jinites;
            case UnitID.FootKnights:
                return FootKnights;
            default:
                return LevySpearmen;
        }
    }
}
