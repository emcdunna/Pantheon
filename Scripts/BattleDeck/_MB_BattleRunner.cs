using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.BattleDeck
{

    // Singleton
    public sealed class _MB_BattleRunner : MonoBehaviour
    {
        private static _MB_BattleRunner runner = null;

        private _MB_BattleRunner()
        {

        }

        public static _MB_BattleRunner Runner()
        {
            return runner;
        }


        // Start class
        
        // Unity object references
        public _MB_BattlefieldManager BattlefieldManager;
        public _MB_Controller Player1Controller;
        public _MB_CPU_Controller Player2Controller;
        public _MB_PlayerHUD PlayerHud;

        // Local variables
        Scenario scenario = null;
        Player winner = null;
        bool paused = false;
        int roundNumber = 0;
        float phaseStartTime = 0;
        float minimumPhaseTime = 1;
        
        public enum STATE
        {
            SETUP,
            DEPLOYMENT,
            WAITING,
            EXECUTE,
            ANIMATION,
            ENDOFROUND,
            BATTLEOVER,
            NEWROUND
        }
        STATE state = STATE.SETUP;

        UnitAnimation.AnimationPhase animationPhase = UnitAnimation.AnimationPhase.RangedPhase;
        int animationIndex = 0;


        public Scenario GetScenario()
        {
            return scenario;
        }

        public void LoadScenario(string scenario_name)
        {
            scenario = __Database.Database().GetScenario(scenario_name);
        }

        public STATE GetState()
        {
            return state;
        }

        public bool DidPlayerWin(Player player)
        {
            if (winner == null)
            {
                return false;
            }
            else
            {
                if (player == winner)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        void Setup()
        {
            state = STATE.SETUP;
            scenario.Setup();
            
            BattlefieldManager.gameObject.SetActive(true);
            Player1Controller.gameObject.SetActive(true);
            Player2Controller.gameObject.SetActive(true);
            PlayerHud.gameObject.SetActive(true);

            BattlefieldManager.Setup(scenario);
            PlayerHud.Setup();
            AutoDeploy(scenario.player1);
            AutoDeploy(scenario.player2);

            state = STATE.DEPLOYMENT;
            ResetPlays();

            

        }

        // Start is called before the first frame update
        void Start()
        {
            runner = this;
            scenario = Scenario.BuildScenario();
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {

                // Setup the scenario for the first time
                case (STATE.SETUP):
                    if (scenario != null)
                    {
                        Setup();
                    }
                    break;

                // Allow players to deploy their units
                case (STATE.DEPLOYMENT):

                    break;

                // Start of a new round
                case (STATE.NEWROUND):

                    BattlefieldManager.RemoveAllPlaySprites();
                    ResetPlays();
                    roundNumber += 1;

                    // fill hands with new cards
                    scenario.player1.FillHand();
                    scenario.player2.FillHand();

                    state = STATE.WAITING;
                    break;

                // Waiting for Player 1 to decide which card to play
                case (STATE.WAITING):

                    // TODO: reveal any known information about the enemy plays

                    break;

                // Once all play is ready, execute orders directly.
                case (STATE.EXECUTE):
                    BattlefieldManager.RevealPlays(scenario.player2.GetPlays());
                    RunPlays();
                    StartAnimations();
                    state = STATE.ANIMATION;
                    animationIndex = 0;
                    Debug.Log("Starting animation phase " + UnitAnimation.ANIMATION_ORDER[animationIndex]);
                    break;

                // Animate the effects.
                case (STATE.ANIMATION):
                    animationPhase = UnitAnimation.ANIMATION_ORDER[animationIndex];
                    
                    if (IsPhaseFinished(animationPhase))
                    {
                        animationIndex++;
                        if(animationIndex >= UnitAnimation.ANIMATION_ORDER.Count)
                        {
                            Debug.Log("Finished all animation phases");
                            state = STATE.ENDOFROUND;
                        }
                        else
                        {
                            Debug.Log("Starting animation phase " + UnitAnimation.ANIMATION_ORDER[animationIndex]);
                            ContinueToNextAnimationPhase();
                        }

                    }
                    else
                    {
                        
                    }
                    break;

                case (STATE.ENDOFROUND):
                    
                    bool gameover = CheckForEndOfGame();
                    if (gameover)
                    {
                        state = STATE.BATTLEOVER;
                    }
                    else
                    {
                        state = STATE.NEWROUND;
                    }
                    break;

                // Battle is over, present post battle screen.
                case (STATE.BATTLEOVER):
                    break;


                default:
                    break;
            }

        }

        public UnitAnimation.AnimationPhase GetAnimationPhase()
        {
            return animationPhase;
        }

        // Loads the scenario by resetting everything and applying the scenario
        public void LoadScenario(Scenario newScenario)
        {
            scenario = newScenario;
            state = STATE.SETUP;
        }

        

        // Selected when the player is done playing cards to sectors
        public void Execute()
        {
            if (state == STATE.WAITING)
            {
                scenario.player1.SetSelectedCard(null);
                state = STATE.EXECUTE;

            } else if(state == STATE.DEPLOYMENT)
            {
                EndDeployment();
            }
        }


        // Start animations for all units
        void StartAnimations()
        {
            phaseStartTime = Time.time;

            Army a1 = scenario.army1;
            Army a2 = scenario.army2;

            foreach(Battalion battalion in a1.all_battalions)
            {
                battalion.StartAnimation();
            }

            foreach (Battalion battalion in a2.all_battalions)
            {
                battalion.StartAnimation();
            }
        }

        // Determine if a phase is finished by checking that all units are now done animating
        private bool IsPhaseFinished(UnitAnimation.AnimationPhase phase)
        {
            if(Time.time < phaseStartTime + minimumPhaseTime)
            {
                return false;
            }
            Army a1 = scenario.army1;
            Army a2 = scenario.army2;

            foreach (Battalion battalion in a1.all_battalions)
            {
                if (battalion.UnitUI.IsNotDoneWithPhase(animationPhase))
                {
                    return false;
                }
            }

            foreach (Battalion battalion in a2.all_battalions)
            {
                if (battalion.UnitUI.IsNotDoneWithPhase(animationPhase))
                {
                    return false;
                }
            }

            return true;
        }

        // Once all units are finished animating, start the next phase
        private void ContinueToNextAnimationPhase()
        {
            phaseStartTime = Time.time;

            Army a1 = scenario.army1;
            Army a2 = scenario.army2;

            foreach (Battalion battalion in a1.all_battalions)
            {
                battalion.UnitUI.ContinueToNextAnimation();
            }

            foreach (Battalion battalion in a2.all_battalions)
            {
                battalion.UnitUI.ContinueToNextAnimation();
            }
        }

        // Runs the selected plays
        void RunPlays()
        {

            foreach (Sector.Type sector in scenario.activeSectors)
            {
                // If this is removed, player1's delegates will not work
                Play p1 = scenario.player1.GetPlayOrDelegate(sector);
                Play p2 = scenario.player2.GetPlayOrDelegate(sector);

                p1.CreateActionScripts();
                p2.CreateActionScripts();

                __Engine.ProcessCardEffects(p1, p2);
                __Engine.ProcessCardEffects(p2, p1);

                if (p1.card != null && p1.card.type != Card.CardType.Delegate)
                {
                    scenario.player1.hand.Remove(p1.card);
                    scenario.player1.deck.Discard(p1.card);
                }
                if (p2.card != null && p2.card.type != Card.CardType.Delegate)
                {
                    scenario.player2.hand.Remove(p2.card);
                    scenario.player2.deck.Discard(p2.card);
                }
                Debug.Log("-------------");
            }

            Debug.Log("####################");

        }

        // Check if any sector has broken by the end of the round of plays
        bool CheckForEndOfGame()
        {
            int p1_broken_count = 0;
            int p2_broken_count = 0;
            foreach (Sector.Type sector in scenario.activeSectors)
            {
                Sector bs1 = scenario.army1.GetBattleSector(sector);
                Sector bs2 = scenario.army2.GetBattleSector(sector);

                bool p1_broken = CheckSector(bs1);
                bool p2_broken = CheckSector(bs2);

                if (p1_broken)
                {
                    p1_broken_count += 1;
                    if (sector == Sector.Type.Center)
                    {
                        winner = scenario.player2;
                        BreakRemainingSectors(scenario.player1);
                        return true;
                    }
                }
                if (p2_broken)
                {
                    p2_broken_count += 1;
                    if (sector == Sector.Type.Center)
                    {
                        winner = scenario.player1;
                        BreakRemainingSectors(scenario.player2);
                        return true;
                    }
                }
            }
            if (p1_broken_count >= 2)
            {
                winner = scenario.player2;
                BreakRemainingSectors(scenario.player1);
                return true;
            }
            else if (p2_broken_count >= 2)
            {
                winner = scenario.player1;
                BreakRemainingSectors(scenario.player2);
                return true;
            }
            else
            {
                return false;
            }
        }

        // returns True if the sector is defeated
        bool CheckSector(Sector bs)
        {
            if (!bs.HasAlreadyBroken())
            {
                float current_morale = bs.GetMorale();
                if (current_morale <= 0)
                {
                    Debug.Log("Sector " + bs.ToString() + " has broken");

                    _MB_BattleSectorManager bsm = BattlefieldManager.GetSectorManager(bs);
                    bsm.BreakSector();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        void BreakRemainingSectors(Player player)
        {
            foreach (Sector.Type sector in scenario.activeSectors)
            {
                Sector bs = player.GetArmy().GetBattleSector(sector);
                if (!bs.HasAlreadyBroken())
                {

                    Debug.Log("Sector " + bs.ToString() + " has broken");

                    _MB_BattleSectorManager bsm = BattlefieldManager.GetSectorManager(bs);
                    bsm.BreakSector(Random.Range(0.5f, 1.25f));

                }
            }
        }

        // Delete all plays before the next round starts
        void ResetPlays()
        {
            scenario.player1.ClearPlays();
            scenario.player2.ClearPlays();
        }

        public Sector.Type GetNextUnplayedSector(Player player)
        {
            List<Sector.Type> unplayed = player.GetUnplayedSectors();
            if (unplayed.Count > 0)
            {
                return unplayed[0];
            }
            else
            {
                return scenario.activeSectors[0];
            }

        }

        public void ClickSector(Sector.Type sector)
        {
            if (!paused)
            {
                if (state == STATE.WAITING)
                {
                    if (scenario.army1.GetBattleSector(sector).HasAlreadyBroken())
                    {
                        Debug.Log("Can't play card to defeated sector");
                        return;
                    }
                    Play existingPlay = scenario.player1.GetPlay(sector);
                    if (existingPlay != null)
                    {
                        scenario.player1.CancelPlay(existingPlay);
                    }
                    if (scenario.player1.GetSelectedCard() != null)
                    {
                        scenario.player1.NewPlay(scenario.player1.GetSelectedCard(), sector);
                        scenario.player1.SetSelectedCard(null);
                    }
                }
                else
                {
                    // not allowed
                    Debug.Log("Not allowed to click a sector right now.");
                }
            }

        }

        public void TogglePause()
        {
            paused = !paused;
        }

        public bool IsPaused()
        {
            return paused;
        }

        public void EndDeployment()
        {
            if (state == STATE.DEPLOYMENT)
            {
                state = STATE.NEWROUND;
            }
        }

        public void AutoDeploy(Player player)
        {
            int i = 0;
            int total_sectors = scenario.activeSectors.Count;
            foreach (Battalion battalion in player.army.all_battalions)
            {
                int index = i % total_sectors;
                Sector.Type sector = scenario.activeSectors[index];
                player.Deploy(sector, battalion);
                i++;
            }
        }
    }
}