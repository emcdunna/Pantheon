using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// Keyboard mouse and other controls used for a human player
namespace Assets.Scripts.BattleDeck
{
    public class _MB_Controller : MonoBehaviour
    {
        
        private Player player = null;
        bool QUICK_PLAY = true;


        // Start is called before the first frame update
        void Start()
        {
            

            
        }

        public void ToggleQuickPlay()
        {
            QUICK_PLAY = !QUICK_PLAY;
        }

        // Update is called once per frame
        void Update()
        {
            _MB_BattleRunner runner = _MB_BattleRunner.Runner();

            if (player == null)
            {
                player = _MB_BattleRunner.Runner().GetScenario().player1;
            }

            int unplayedSectors = player.CountUnplayedSectors();
            float currentTime = Time.time;

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (runner.IsPaused())
                {
                    ContinueGame();
                }
                else
                {
                    PauseGame();
                }
            }
            if (!runner.IsPaused())
            {
                // Letters to select cards
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    KeyPressWithIndex(0);
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    KeyPressWithIndex(1);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    KeyPressWithIndex(2);
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    KeyPressWithIndex(3);
                }
                else if (Input.GetKeyDown(KeyCode.T))
                {
                    KeyPressWithIndex(4);
                }
                else if (Input.GetKeyDown(KeyCode.Y))
                {
                    KeyPressWithIndex(5);
                }
                else if (Input.GetKeyDown(KeyCode.U))
                {
                    KeyPressWithIndex(6);
                }
                else if (Input.GetKeyDown(KeyCode.I))
                {
                    KeyPressWithIndex(7);
                }
                else if (Input.GetKeyDown(KeyCode.O))
                {
                    KeyPressWithIndex(8);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    DiscardCard();
                }

                // Numbers to select Sectors
                else if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    runner.ClickSector(Sector.Type.Left);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    runner.ClickSector(Sector.Type.Center);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    runner.ClickSector(Sector.Type.Right);
                }

                // Interactions with gameplay runner
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _MB_PlayerHUD hud = _MB_BattleRunner.Runner().PlayerHud;
                    hud.UpdateSelectedUnit(null);
                    player.CancelPlays();
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    runner.Execute();
                }

                
            }

        }


        void KeyPressWithIndex(int index)
        {
            _MB_BattleRunner runner = _MB_BattleRunner.Runner();
            if (index >= player.hand.Count)
            {
                return;
            }
            if (player.GetSelectedCard() == player.hand[index])
            {
                Sector.Type sector = runner.GetNextUnplayedSector(player);
                runner.ClickSector(sector);
            }
            else
            {
                player.SetSelectedCard(index);
            }
        }


        public void QuitToMainMenu()
        {
            GameObject current_manager = GameObject.Find("BattleDeckSceneManager");
            if (current_manager != null)
            {
                Object.DestroyImmediate(current_manager);
            }
            SceneManager.LoadScene(0);

        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void PauseGame()
        {
            _MB_BattleRunner runner = _MB_BattleRunner.Runner();
            Time.timeScale = 0;
            runner.TogglePause();
        }

        public void ContinueGame()
        {
            _MB_BattleRunner runner = _MB_BattleRunner.Runner();
            Time.timeScale = 1;
            runner.TogglePause();
        }


        public void DiscardCard()
        {
            player.ManualDiscard();
        }

    }

}