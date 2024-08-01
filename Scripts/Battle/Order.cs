using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{

    // formation

    // All orders are relative to a specific direction. This is usually the same as
    // the facing of the units in question.
    public Direction direction;

    // The desired relative destination to move towards, attack, etc.
    public Vector3 destination;


    // Make a new Order, which can be delivered to one or more units
    public Order()
    {
        

    }
}
