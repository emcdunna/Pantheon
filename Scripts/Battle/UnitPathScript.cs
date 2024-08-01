using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPathScript : MonoBehaviour
{

    public LineRenderer line;
    public old_Battalion unit;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(unit != null)
        {
            line.SetPosition(0, unit.transform.position);
            line.SetPosition(1, unit.command.destination);
        }
        
    }
}
