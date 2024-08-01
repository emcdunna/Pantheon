using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


namespace Assets.Scripts.BattleDeck
{
    [System.Serializable]
    public sealed class __Database
    {

        private static readonly __Database database = new __Database();

        private __Database()
        {
            ReloadData();

        }

        public static __Database Database ()
        {
            return database;
        }

        private string[] LoadTsvData(string line)
        {
            return line.Split('\t');
        }

        Dictionary<string, __UnitLoadout.Armor> Armors;
        Dictionary<string, Card> Cards;
        Dictionary<string, __UnitLoadout.Mount> Mounts;
        Dictionary<string, __UnitLoadout.SpecialRule> SpecialRules;
        Dictionary<string, __UnitLoadout.Weapon> Weapons;
        Dictionary<string, Battalion> BaseUnits;
        Dictionary<string, Scenario> Scenarios;


        public __UnitLoadout.Armor GetArmor(string name)
        {
            if (Armors.ContainsKey(name))
            {
                return Armors[name];
            }
            else
            {
                Debug.LogError("Missing Armor Key: " + name);
            }
            return null;
        }

        public Card GetCard(string name)
        {
            if (Cards.ContainsKey(name))
            {
                return Cards[name].Clone();
            }
            else
            {
                Debug.LogWarning("Missing Card Key: " + name);
            }
            return null;
        }

        public __UnitLoadout.Mount GetMount(string name)
        {
            if (Mounts.ContainsKey(name))
            {
                return Mounts[name];
            }
            else
            {
                Debug.LogError("Missing Mounts Key: " + name);
            }
            return null;
        }

        public __UnitLoadout.Weapon GetWeapon(string name)
        {
            if (Weapons.ContainsKey(name))
            {
                return Weapons[name];
            }
            else
            {
                Debug.LogError("Missing Weapons Key: " + name);
            }
            return null;
        }

        public __UnitLoadout.SpecialRule GetSpecialRule(string name)
        {
            if (SpecialRules.ContainsKey(name))
            {
                return SpecialRules[name];
            }
            else
            {
                Debug.LogWarning("Missing SpecialRules Key: " + name);
            }
            return null;
        }

        public Battalion GetUnit(string name)
        {
            if (BaseUnits.ContainsKey(name))
            {
                return BaseUnits[name].Clone();
            }
            else
            {
                Debug.LogError("Missing BaseUnits Key: " + name);
            }
            return null;
        }

        public Scenario GetScenario(string name)
        {
            if (Scenarios.ContainsKey(name))
            {
                return Scenarios[name];
            }
            else
            {
                Debug.LogError("Missing Scenarios Key: " + name);
            }
            return null;
        }


        public void ReloadData()
        {
            int i = 0;

            Armors = new Dictionary<string, __UnitLoadout.Armor>();
            Cards = new Dictionary<string, Card>();
            Mounts = new Dictionary<string, __UnitLoadout.Mount>();
            SpecialRules = new Dictionary<string, __UnitLoadout.SpecialRule>();
            Weapons = new Dictionary<string, __UnitLoadout.Weapon>();
            BaseUnits = new Dictionary<string, Battalion>();

            // Load special rules
            string[] special_lines = System.IO.File.ReadAllLines(@"C:\Unity\Pantheon\Assets\GameData\BattleDeck\special_rules.tsv");
            i = 0;
            foreach (string line in special_lines)
            {
                if (i != 0)
                {
                    string[] tsv_data = LoadTsvData(line);
                    __UnitLoadout.SpecialRule r = new __UnitLoadout.SpecialRule(tsv_data);
                    SpecialRules[tsv_data[0]] = r;
                }
                i++;
            }


            // Load armors
            string[] resource_lines = System.IO.File.ReadAllLines(@"C:\Unity\Pantheon\Assets\GameData\BattleDeck\armor.tsv");
            i = 0;
            foreach (string line in resource_lines)
            {
                if (i != 0)
                {
                    string[] tsv_data = LoadTsvData(line);
                    __UnitLoadout.Armor r = new __UnitLoadout.Armor(tsv_data);
                    Armors[tsv_data[0]] = r;
                }
                i++;
            }


            // Load cards
            string[] card_lines = System.IO.File.ReadAllLines(@"C:\Unity\Pantheon\Assets\GameData\BattleDeck\cards.tsv");
            i = 0;
            foreach (string line in card_lines)
            {
                if (i != 0)
                {
                    string[] tsv_data = LoadTsvData(line);
                    Card c = new Card(tsv_data);
                    Cards[tsv_data[0]] = c;
                }
                i++;
            }


            // Load mounts
            string[] mounts_lines = System.IO.File.ReadAllLines(@"C:\Unity\Pantheon\Assets\GameData\BattleDeck\mounts.tsv");
            i = 0;
            foreach (string line in mounts_lines)
            {
                if (i != 0)
                {
                    string[] tsv_data = LoadTsvData(line);
                    __UnitLoadout.Mount r = new __UnitLoadout.Mount(tsv_data, GetSpecialRule(tsv_data[5]));
                    Mounts[tsv_data[0]] = r;
                }
                i++;
            }


            // Load weapons
            string[] weapons_lines = System.IO.File.ReadAllLines(@"C:\Unity\Pantheon\Assets\GameData\BattleDeck\weapons.tsv");
            i = 0;
            foreach (string line in weapons_lines)
            {
                if (i != 0)
                {
                    string[] tsv_data = LoadTsvData(line);
                    __UnitLoadout.Weapon r = new __UnitLoadout.Weapon(tsv_data, GetCard(tsv_data[10]));
                    Weapons[tsv_data[0]] = r;

                    foreach (String name in tsv_data[11].Split(','))
                    {
                        r.AddSpecialRule(GetSpecialRule(name));
                    }
                }
                i++;
            }


            // Load units
            string[] unit_lines = System.IO.File.ReadAllLines(@"C:\Unity\Pantheon\Assets\GameData\BattleDeck\base_units.tsv");
            i = 0;
            foreach (string line in unit_lines)
            {
                if (i != 0)
                {
                    string[] tsv_data = LoadTsvData(line);
                    Battalion b = new Battalion(
                        tsv_data[0],
                        GetWeapon(tsv_data[1]),
                        GetWeapon(tsv_data[2]),
                        GetWeapon(tsv_data[3]),
                        GetArmor(tsv_data[4]),
                        GetArmor(tsv_data[5]),
                        GetMount(tsv_data[6]),
                        GetSpecialRule(tsv_data[7]),
                        GetSpecialRule(tsv_data[8]),
                        GetCard(tsv_data[9])
                    );

                    BaseUnits[tsv_data[0]] = b;
                }
                i++;
            }
        }

    }
}