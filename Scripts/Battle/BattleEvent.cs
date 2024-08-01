using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A single event with a set trigger (like time passed)
[System.Serializable]
public class BattleEvent 
{
    public float triggerTime = 0;
    public Vector3 location1, location2;
    public MedievalUnits.UnitID unitID;
    public UnitStats unitStats;
    public BattleGroup battleGroup;
    public bool hasTriggered = false;

    // Tells battle to spawn a specific unit at a location and at a certain time.
    public BattleEvent(float triggerTime, Vector3 location1, Vector3 location2, MedievalUnits.UnitID unitID,
        BattleGroup battleGroup)
    {
        this.triggerTime = triggerTime;
        this.location1 = location1;
        this.location2 = location2;
        this.unitID = unitID;
        this.battleGroup = battleGroup;
    }

}
