using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BattleDeck
{
    public class _MB_CPU_Controller : MonoBehaviour
    {

        private Player player = null;
        private bool has_played = false;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

            if (player == null)
            {
                player = _MB_BattleRunner.Runner().GetScenario().player2;
            }
            if (player.hand.Count == 0)
            {
                Debug.Log("No cards in hand for " + player.ToString());

            }
            else
            {
                
                _MB_BattleRunner runner = _MB_BattleRunner.Runner();
                _MB_BattleRunner.STATE state = runner.GetState();
                switch (state)
                {
                    case _MB_BattleRunner.STATE.WAITING:
                        SelectPlays();
                        break;
                    case _MB_BattleRunner.STATE.ANIMATION:
                        has_played = false;
                        break;
                    default:
                        break;
                }
            }

        }

        void SelectPlays()
        {
            _MB_BattleRunner runner = _MB_BattleRunner.Runner();
            List<Sector.Type> activeSectors = new List<Sector.Type>(runner.GetScenario().activeSectors);

            if (!has_played)
            {

                // use a random order to avoid power bias to one sector
                for (int i = activeSectors.Count - 1; i > 0; i--)
                {
                    // Randomize a number between 0 and i (so that the range decreases each time)
                    int rnd = UnityEngine.Random.Range(0, i);

                    // Save the value of the current i, otherwise it'll overwrite when we swap the values
                    Sector.Type temp = activeSectors[i];

                    // Swap the new and old values
                    activeSectors[i] = activeSectors[rnd];
                    activeSectors[rnd] = temp;

                }


                foreach (Sector.Type sector in activeSectors)
                {
                    Sector battlesector = player.GetArmy().GetBattleSector(sector);
                    if (battlesector.HasAlreadyBroken())
                    {
                        // then don't play to it
                    }
                    else
                    {
                        Card bestCard = GetBestCard(battlesector);
                        if(bestCard != null)
                        {
                            player.NewPlay(bestCard, sector);
                        } else
                        {
                            // delegate should be automatic
                            Debug.Log("CPU will use a delegate for sector " + sector);
                        }
                        
                    }

                }
            }
            has_played = true;
        }

        Card GetBestCard(Sector battlesector)
        {
            Card bestCard = null;
            int best_utility = 0;
            foreach (Card card in player.hand)
            {
                if (player.HasAlreadyPlayedCard(card))
                {
                    // already used
                }
                else
                {
                    Play possiblePlay = new Play(player, card, battlesector);
                    int utility = possiblePlay.CalculateUtility();
                    if (utility > best_utility)
                    {
                        bestCard = card;
                        best_utility = utility;
                    }
                }
            }

            return bestCard;
        }
    }
}