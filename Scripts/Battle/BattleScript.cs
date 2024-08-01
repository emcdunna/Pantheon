using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A series of events that are scheduled to occur in the battle to guide the flow
public class BattleScript : MonoBehaviour
{
    public BattleManager BATTLE_MANAGER;
    public List<BattleEvent> Events = new List<BattleEvent>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(BattleEvent ent in Events)
        {
            if (!ent.hasTriggered && Time.time >= ent.triggerTime)
            {
                BATTLE_MANAGER.EnqueueEvent(ent);
                ent.hasTriggered = true;
            }
        }
    }
}
