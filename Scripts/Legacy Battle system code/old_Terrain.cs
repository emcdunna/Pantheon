using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class old_Terrain : MonoBehaviour
{

    public enum Type { Woods, Farms, Swamp, Hills, Cliffs, RockyGround };
    public Type type = Type.Woods;
    public float speedPenalty = 0f; // % reduction in top speed
    public float cohesionPenalty = 0f; // % reduction in max cohesion
    public float coverBonus = 0f; // % bonus armor vs ranged attacks
    public int elevationBonus = 0; // # of advantages gained for high ground
    public float stealthBonus = 0f; 
    public List<old_Unit> touching_units = new List<old_Unit>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject target = other.gameObject;
        old_Unit unit = target.GetComponent<old_Unit>();
        if (unit != null && !unit.battalion.is_dead)
        {
            unit.current_terrain = this;
        }
        else if (unit != null && unit.battalion.is_dead)
        {
            touching_units.Remove(unit);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        GameObject target = other.gameObject;
        old_Unit unit = target.GetComponent<old_Unit>();
        if (unit != null && !unit.battalion.is_dead)
        {
            print(unit.ToString() + " entered " + type.ToString());
            touching_units.Add(unit);
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject target = other.gameObject;
        old_Unit unit = target.GetComponent<old_Unit>();
        if (unit != null && !unit.battalion.is_dead)
        {
            touching_units.Remove(unit);
            if(unit.current_terrain == this)
            {
                unit.current_terrain = null;
            }
        }
    }

    
}
