using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.BattleDeck
{
    public class _MB_PlayerHUD : MonoBehaviour
    {
        public GameObject DeckDescriptorPanel;
        public GameObject UnitDescriptorPanel;
        public GameObject VictoryScreen;
        public GameObject DefeatScreen;
        public GameObject PauseScreen;
        public GameObject MissionBriefing;
        public GameObject HelpWindow;
        public GameObject MenuOverlay;
        public GameObject SectorDescriptor;
        public GameObject RunnerStateText;
        public GameObject GameStateUI;
        public GameObject PlayerHand;
        public GameObject ScreenPrompts;
        public Image factionBanner1, factionBanner2;
        public Image portrait1, portrait2;
        bool set_sprites = false;
        private Battalion selectedBattalion;
        private Player player;


        // Start is called before the first frame update
        void Start()
        {

        }


        public void Setup()
        {
            _MB_HandManager handManager = GetComponentInChildren<_MB_HandManager>();
            player = _MB_BattleRunner.Runner().GetScenario().player1;
            handManager.player = player;

            
        }

        public void HidePlayerHand()
        {
            PlayerHand.SetActive(false);
        }

        public void ShowPlayerHand()
        {
            PlayerHand.SetActive(true);
        }

        public void ToggleDeckDescriptorPanel_player1()
        {
            
            DeckDescriptorPanel.SetActive(true);
            DeckDescriptorPanel.GetComponent<_MB_DeckDescriptor>().SetDeck(_MB_BattleRunner.Runner().GetScenario().player1.deck);
            
        }

        public void CloseDeckDescriptorPanel()
        {
            DeckDescriptorPanel.SetActive(false);
        }

        public void CloseUnitDescriptorPanel()
        {
            UnitDescriptorPanel.SetActive(false);
        }

        public void CloseSectorDescriptorPanel()
        {
            SectorDescriptor.SetActive(false);
        }

        public void OpenSectorDescriptorPanel()
        {
            SectorDescriptor.SetActive(true);
        }

        public void SetSectorDescriptorPanel(Sector sector)
        {
            SectorDescriptor.GetComponent<_MB_SectorDescriptor>().SetBattleSector(sector);
        }

        public void SetUnitDescriptor(Battalion battalion)
        {
            UnitDescriptorPanel.SetActive(true);
            UnitDescriptorPanel.GetComponent<_MB_UnitDescriptor>().SetBattalion(battalion);
        }

        public void UpdateSelectedUnit(Battalion battalion)
        {
            _MB_BattleRunner runner = _MB_BattleRunner.Runner();

            if (selectedBattalion != null)
            {
                selectedBattalion.UnitUI.DisableHighlight();
            }

            if(runner.GetState() != _MB_BattleRunner.STATE.DEPLOYMENT)
            {
                Debug.Log("Cannot select units at this time");
                return;
            }

            if (!runner.GetScenario().army1.all_battalions.Contains(battalion))
            {
                Debug.Log("Not allowed to select enemy unit " + battalion);
                return;
            }

            selectedBattalion = battalion;

            if(battalion != null)
            {
                selectedBattalion.UnitUI.EnableHighlight();
            }
        }

        public Battalion GetSelectedUnit()
        {
            return selectedBattalion;
        }


        public void ToggleDeckDescriptorPanel_player2()
        {
            DeckDescriptorPanel.SetActive(true);
            DeckDescriptorPanel.GetComponent<_MB_DeckDescriptor>().SetDeck(_MB_BattleRunner.Runner().GetScenario().player2.deck);
        }

        // Update is called once per frame
        void Update()
        {
            _MB_BattleRunner runner = _MB_BattleRunner.Runner();
            if(runner.GetState() == _MB_BattleRunner.STATE.DEPLOYMENT)
            {
                HidePlayerHand();
                ScreenPrompts.SetActive(true);
            } else
            {
                ShowPlayerHand();
                ScreenPrompts.SetActive(false);
            }

            RunnerStateText.GetComponent<Text>().text = runner.GetState().ToString();
            if (runner.DidPlayerWin(_MB_BattleRunner.Runner().GetScenario().player1))
            {
                MenuOverlay.SetActive(true);
                VictoryScreen.SetActive(true);
                DefeatScreen.SetActive(false);
            }
            else if (runner.DidPlayerWin(_MB_BattleRunner.Runner().GetScenario().player2))
            {
                MenuOverlay.SetActive(true);
                DefeatScreen.SetActive(true);
                VictoryScreen.SetActive(false);
            }
            else if (runner.IsPaused())
            {
                MenuOverlay.SetActive(true);
                PauseScreen.SetActive(true);
            }
            else
            {
                MenuOverlay.SetActive(false);
                PauseScreen.SetActive(false);
                HelpWindow.SetActive(false);
                MissionBriefing.SetActive(false);
            }

            if (false) // TODO !set_sprites)
            {
                Scenario scenario = runner.GetScenario();
                if (scenario != null)
                {
                    factionBanner1.sprite = runner.GetScenario().faction_1.GetBannerSprite();
                    factionBanner2.sprite = runner.GetScenario().faction_2.GetBannerSprite();
                    portrait1.sprite = runner.GetScenario().general1.portrait;
                    portrait2.sprite = runner.GetScenario().general2.portrait;
                    set_sprites = true;
                }
                else
                {
                    set_sprites = false;
                }

            }
        }

        public void OpenHelpSceen()
        {
            HelpWindow.SetActive(true);
        }

        public void OpenMissionBriefing()
        {
            MissionBriefing.SetActive(true);
        }
    }
}