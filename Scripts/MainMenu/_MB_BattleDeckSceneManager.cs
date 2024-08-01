using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.MainMenu
{
    public class _MB_BattleDeckSceneManager : MonoBehaviour
    {

        public string mainMenuScene = "main_menu";
        Assets.Scripts.BattleDeck._MB_BattleRunner runner;
        public string scenario_to_load = null;

        // Start is called before the first frame update
        void Start()
        {
            Object.DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            Scene current = SceneManager.GetActiveScene();
            if (current.buildIndex == 0)
            {
                //scenario_to_load = null;
            }

            if (scenario_to_load != null)
            {
                GameObject runner_go = GameObject.FindGameObjectWithTag("GameController");
                if (runner_go != null)
                {
                    runner = runner_go.GetComponent<Assets.Scripts.BattleDeck._MB_BattleRunner>();
                    if (runner != null)
                    {
                        if (runner.GetScenario() == null)
                        {
                            runner.LoadScenario(scenario_to_load);
                        }

                    }

                }
            }

        }


        public void LoadTours()
        {
            SceneManager.LoadScene(1);

        }

        public void LoadHastings()
        {
            SceneManager.LoadScene(1);
            scenario_to_load = "Battle of Hastings, 1066 AD";

        }


    }

}
