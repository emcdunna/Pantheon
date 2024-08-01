using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHudManager : MonoBehaviour
{

    public RawImage VictoryScreen;
    public RawImage DefeatScreen;
    public BattleManager BATTLE_MANAGER;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(BATTLE_MANAGER.victoryStatus == BattleManager.VictoryStatus.Team0Wins)
        {
            VictoryScreen.gameObject.SetActive(true);
            DefeatScreen.gameObject.SetActive(false);
        }
        else if (BATTLE_MANAGER.victoryStatus == BattleManager.VictoryStatus.Team1Wins)
        {
            DefeatScreen.gameObject.SetActive(true);
            VictoryScreen.gameObject.SetActive(false);
        }
        else
        {
            DefeatScreen.gameObject.SetActive(false);
            VictoryScreen.gameObject.SetActive(false);
        }
    }
}
