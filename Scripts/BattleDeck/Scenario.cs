using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BattleDeck
{
    [System.Serializable]
    public class Scenario
    {
        public string name;

        public int battleSize;
        public General general1, general2;
        public Faction faction_1, faction_2;
        
        public Player player1, player2;

        public Army army1, army2;
        public List<Sector.Type> activeSectors = new List<Sector.Type>();

        // date

        // location

        // weather


        public Scenario(string Name, General General1, General General2, Faction Faction1, Faction Faction2,
                        Army Army1, Army Army2, int BattleSize)
        {
            name = Name;
            general1 = General1;
            general2 = General2;
            faction_1 = Faction1;
            faction_2 = Faction2;
            army1 = Army1;
            army2 = Army2;
            battleSize = BattleSize;

            player1 = new Player("Human player", army1, general1, faction_1);
            player2 = new Player("CPU player", army2, general2, faction_2);
        }

        public void Setup()
        {
            Debug.Log("Setup " + name);


            // Add sectors to map
            if (battleSize >= 4)
            {
                activeSectors.Add(Sector.Type.LeftWing);
            }
            if (battleSize >= 2)
            {
                activeSectors.Add(Sector.Type.Left);
            }

            activeSectors.Add(Sector.Type.Center);
            
            if (battleSize >= 3)
            {
                activeSectors.Add(Sector.Type.Right);
            }
            if (battleSize >= 5)
            {
                activeSectors.Add(Sector.Type.RightWing);
            }

            // setup armies
            army1.SetupBattleSectors(battleSize);
            army2.SetupBattleSectors(battleSize);



            // Setup terrain
        }


        public static Scenario BuildScenario()
        {
            Scenario scenario = new Scenario(
                    Name:"Zama",
                    General1: new General("Hannibal"),
                    General2: new General("Scipio"),
                    Faction1: new Faction("Carthage", new Color(0.28f, 0.48f, 0.77f), Color.gray, null, new List<Card>()
                        {
                            __Database.Database().GetCard("schiltrom")
                        }),
                    Faction2: new Faction("Rome", new Color(0.6f,0.3f,0.3f), Color.yellow, null, new List<Card>()
                        {
                            __Database.Database().GetCard("hold_the_line")
                        }),
                    Army1: new Army(new List<Battalion>()
                        {
                            __Database.Database().GetUnit("Alyrian Cavalry"),
                            __Database.Database().GetUnit("Royal Arani Guard"),
                            __Database.Database().GetUnit("Aethunding Pikemen"),
                            __Database.Database().GetUnit("Aethunding Noble Swordsmen"),
                            __Database.Database().GetUnit("Aethunding Shieldmaidens"),
                            __Database.Database().GetUnit("Militia Crossbowmen"),
                            __Database.Database().GetUnit("Arani Cataphracts"),
                            __Database.Database().GetUnit("Mearhan Horse Archers")
                        }),
                    Army2: new Army(new List<Battalion>()
                        {
                            __Database.Database().GetUnit("Nordic Thegns"),
                            __Database.Database().GetUnit("Brythonic Guard"),
                            __Database.Database().GetUnit("Levy Halberdiers"),
                            __Database.Database().GetUnit("Men at arms"),
                            __Database.Database().GetUnit("Findaric Knights"),
                            __Database.Database().GetUnit("Haelan Crossbowmen"),
                            __Database.Database().GetUnit("Rodan Huscarls"),
                            __Database.Database().GetUnit("Gothic Longbowmen"),
                        }),
                    BattleSize: 3
                );

            return scenario;
        }

        public Player GetOpposingPlayer(Player thisPlayer)
        {
            if(thisPlayer == player1)
            {
                return player2;
            }
            else
            {
                return player1;
            }
        }
    }

}