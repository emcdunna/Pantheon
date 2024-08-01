using System.Collections;
using UnityEngine;

namespace Assets.Scripts.BattleDeck
{
    public class Terrain
    {
        public bool impassible = false;
        public string name;
        public int swords, shock, arrows, evasion, spears, shields, morale;


        // TODO: How to make terrain help with one side vs the other?
        public Terrain(string[] tsv_data)
        {
            name = tsv_data[0];
            swords = int.Parse(tsv_data[1]);
            shock = int.Parse(tsv_data[2]);
            arrows = int.Parse(tsv_data[3]);
            evasion = int.Parse(tsv_data[4]);
            spears = int.Parse(tsv_data[5]);
            shields = int.Parse(tsv_data[6]);
            morale = int.Parse(tsv_data[7]);
            impassible = bool.Parse(tsv_data[8]);
        }

        public Terrain(string Name, bool Impassible=false, int Swords = 0, int Shock = 0, int Arrows = 0,
            int Evasion = 0, int Spears = 0, int Shields = 0, int Morale=0)
        {
            impassible = Impassible;
            name = Name;
            swords = Swords;
            shock = Shock;
            arrows = Arrows;
            evasion = Evasion;
            spears = Spears;
            shields = Shields;
            morale = Morale;
        }

        public override string ToString()
        {
            return name;
        }

    }
}