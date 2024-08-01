using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class old_Command
{
    // how close must a unit be to the destination to consider the command finished
    public static float minimum_distance_threshold = 0.05f;

    // The desired relative destination to move towards, attack, etc.
    public Vector3 destination;
    public string name = "Hold";
    public bool should_run = false;
    public bool should_hold_fire = false;
    public bool should_fall_back = false;
    public old_Battalion battalion;
    public old_Command NextCommand = null;
    public bool complete = false;

    public enum Behavior { HoldGround, Engage, Disengage };
    public Behavior behavior = Behavior.HoldGround;

    // Construct a new command for what the unit is supposed to do.
    public old_Command(string name, Vector3 destination, bool should_run,
        bool should_hold_fire, bool should_fall_back, old_Battalion unit)
    {
        this.destination = destination;
        this.name = name;
        this.should_hold_fire = should_hold_fire;
        this.should_run = should_run;
        this.battalion = unit;
        this.should_fall_back = should_fall_back;
    }

    // Once a unit reached the destination, the command is "complete" meaning it can proceed
    // to the next command.
    public bool IsComplete()
    {
        // If the command is brand new, assume it needs 1 frame at least to run
        if (false)
        {
            return false;
        }
        if (complete == true)
        {
            return true;
        }
        else if (battalion.command == this)
        {
            float distance = Vector3.Distance(battalion.transform.position, this.destination);
            
            if (distance <= minimum_distance_threshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            // if the unit does not still have this command set as active, it must be done already
            return true;
        }

    }

    public static old_Command GetDefaultCommand(old_Battalion unit)
    {
        bool should_hold_fire = false;
        if (unit.command != null)
        {
            should_hold_fire = unit.command.should_hold_fire;
        }
        return new old_Command("Hold", unit.transform.position, false, should_hold_fire, false, unit);
    }

    public static old_Command GetFallbackCommand(old_Battalion unit)
    {

        Vector3 destination = unit.transform.position + -10f * unit.transform.up;
        return new old_Command("Fallback", destination, true, false, true, unit);
    }

    // Returns True if this unit is currently trying to move to a destination, False if its just idle
    public bool OnTheMove()
    {
        if (this.name == "Hold")
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public Vector3 GetMovementVector()
    {
        if (name == "Hold")
        {
            return Vector3.zero;
        }
        else
        {
            return (destination - battalion.transform.position).normalized;
        }
    }

    public void Enqueue(old_Command command)
    {

        if(NextCommand == null)
        {
            NextCommand = command;
        }
        else
        {
            NextCommand.Enqueue(command);
        }
    }
}