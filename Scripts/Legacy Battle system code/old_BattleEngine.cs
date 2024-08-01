using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class old_BattleEngine
{

    public static int base_InfantryMass = 3;
    public static int base_Advantages = 0;
    public static int base_Disadvantages = 0;
    public static int base_AttackCards = 6;
    public static float meters_to_game_units = 0.05f;
    public static float base_AttackRange = 0.33f;
    public static float base_AttackOffset = 0.4f;
    public static int MinimumUnitSize = 30;
    public static int base_UnitMorale = 3;
    public static float base_ReformRate = 0.5f;
    public static float base_OverlapCohesionLoss = 2f;
    public static float base_FleeingCohesionLoss = 2f;
    public static float walkingSpeed = 0.3f;
    public static float base_infantrySpeed = 0.75f;
    public static float battleLaneWidth = 1.75f;
    public static float saturation_blinking_rate = 0.5f;
    public static float cohesion_loss_dividend = 2f;
    public static float chainrout_morale_threshold = 0f; // if less than this % of the army still fights, the battle is over
    public static float formationRange = 2.5f;
    public static float base_FatigueLostPerAttack = 2f;
    public static float fatigueLostPerWeaponWeight = 0.1f; // Multiplier on extra fatigue lost from attacking 
    public static float fatigueLostPerUnitWeight = 0.05f; // Multiplier on extra fatigue lost from movement 
    public static float fleeingMovementSpeedScalar = -1.15f; // how quickly does a unit move when fleeing
    public static float minimumFallbackTime = 5f;
    public static float impededAttackPenalty = 0.5f;

    public static AttackDeck GenerateAttackDeck()
    {
        return new AttackDeck(base_AttackCards);
    }

    public static float GetClosestBattleLane(float x_position)
    {
        return battleLaneWidth * (Mathf.RoundToInt(x_position / battleLaneWidth));

    }

    // randomly round a float to an int
    // ex: 1.65 is rounded to 2.0 65% of the time, or 1.0 35% of the time
    public static int RandomRound(float input)
    {
        float rand = Random.value;

        int round_down = Mathf.FloorToInt(input);
        if (round_down + rand >= input)
        {
            return round_down;
        }
        else
        {
            return round_down + 1;
        }
    }

    public static float CalculateMeleeBlockRate(old_Unit target, Weapon weapon, float flanking_bonus)
    {
        float rate = 0.05f * (target.battalion.armor + target.battalion.primary_weapon.melee_block - weapon.penetration - flanking_bonus * 3);
        if (target.battalion.falling_back)
        {
            rate = 0.25f * rate;
        }
        if (rate < 0)
        {
            return 0;  // cannot have block rate below 0%
        }
        else if (rate >= 0.9f)
        {
            return 0.9f;  // cannot have block rate above 90%
        }
        else
        {
            return rate;
        }
    }

    public static float CalculateCohesionBlockRate(old_Battalion target)
    {
        float rate = 0.05f * (target.discipline) * (target.fatigue / 100);
        if (target.falling_back)
        {
            rate = 0.5f * rate;
        }
        if (rate < 0)
        {
            return 0;  // cannot have block rate below 0%
        }
        else if (rate >= 0.9f)
        {
            return 0.9f;  // cannot have block rate above 90%
        }
        else
        {
            return rate;
        }
    }

    public static float CalculateImpactBlockRate(old_Unit target, Weapon weapon, float flanking_bonus)
    {
        float rate =  ((target.battalion.cohesion * 0.01f) - ((weapon.penetration + flanking_bonus * 3) * 0.05f));
        if (target.battalion.falling_back)
        {
            rate = 0.25f * rate;
        }
        if (rate < 0)
        {
            return 0;  // cannot have block rate below 0%
        }
        else if (rate >= 0.90f)
        {
            return 0.90f;  // cannot have block rate above 90%
        }
        else
        {
            return rate;
        }
    }

    public static float CalculateRangedBlockRate(old_Unit target, Weapon weapon, float flanking_bonus)
    {
        float rate = 0.05f * (target.battalion.armor + - weapon.penetration - flanking_bonus * 2);
        if (target.current_terrain != null)
        {
            rate += target.current_terrain.coverBonus;
        }

        if (target.battalion.falling_back)
        {
            rate = 0.5f * rate;
        }
        if (rate < 0)
        {
            return 0;  // cannot have block rate below 0%
        }
        else if (rate >= 0.95f)
        {
            return 0.95f;  // cannot have block rate above 95%
        }
        else
        {
            return rate;
        }
    }

    public static old_Unit GetTarget(old_Unit attacker, Weapon weapon)
    {
        Vector3 start = attacker.transform.position;
        Vector3 direction = attacker.GetFacingVector();
        float scalar = (weapon.range * meters_to_game_units + base_AttackRange);

        // return closest first
        old_Unit center_target =  FindClosestEnemyInLine(attacker, start, direction, scalar);
        Vector3 rightShift = Vector3.Cross(attacker.GetFacingVector(), attacker.transform.forward).normalized;
        Vector3 leftShift = Vector3.Cross(attacker.GetFacingVector(), -1 * attacker.transform.forward).normalized;
        float[] Angles = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };
        if (center_target == null)
        {
            foreach(float subAngle in Angles)
            {
                Vector3 left_angle = (direction + leftShift * subAngle).normalized;
                Vector3 right_angle = (direction + rightShift * subAngle).normalized;

                old_Unit left_target = FindClosestEnemyInLine(attacker, start, left_angle, scalar);
                old_Unit right_target = FindClosestEnemyInLine(attacker, start, right_angle, scalar);

                if (left_target != null)
                {
                    return left_target;
                }
                else if(right_target != null)
                {
                    return right_target;
                }
            }
            
        }
        return center_target;
    }

    // Searches radially around a direction for the closest unit in that direction
    public static old_Battalion FindClosestUnitRadialDirection(old_Battalion origin, Vector3 direction, float range, float angleDelta)
    {
        Vector3 start = origin.transform.position;

        // return closest first
        old_Battalion bestTarget = FindClosestUnitInLine(origin, start, direction, range);
        Vector3 rightShift = Vector3.Cross(direction, origin.transform.forward).normalized;
        Vector3 leftShift = Vector3.Cross(direction, -1 * origin.transform.forward).normalized;
        float[] Angles = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };

        float shortest_distance = Mathf.Infinity; 
        if (bestTarget != null)
        {
            shortest_distance = Vector3.Distance(start, bestTarget.transform.position);
        }

        float subAngle = 0;
        while(subAngle < angleDelta)
        {
            subAngle += 0.05f;

            Vector3 left_angle = (direction + leftShift * subAngle).normalized;
            Vector3 right_angle = (direction + rightShift * subAngle).normalized;

            old_Battalion left_target = FindClosestUnitInLine(origin, start, left_angle, range);
            old_Battalion right_target = FindClosestUnitInLine(origin, start, right_angle, range);

            if (left_target != null)
            {
                float left_distance = Vector3.Distance(start, left_target.transform.position);
                if (left_distance < shortest_distance)
                {
                    shortest_distance = left_distance;
                    bestTarget = left_target;
                }
            }
            else if (right_target != null)
            {
                float right_distance = Vector3.Distance(start, right_target.transform.position);
                if (right_distance < shortest_distance)
                {
                    shortest_distance = right_distance;
                    bestTarget = right_target;
                }
            }
        }
        return bestTarget;
        
    }

    public static old_Battalion FindClosestEnemyInLine(old_Battalion attacker, Vector3 start, Vector3 direction, float scalar)
    {
        RaycastHit[] allHits = Physics.RaycastAll(start, direction, scalar);

        float closest_range = Mathf.Infinity;
        old_Battalion closest_battalion = null;

        // return closest first
        foreach (RaycastHit hit in allHits)
        {
            GameObject go = hit.collider.gameObject;
            old_Battalion bt = null;
            if (go != null)
            {
                bt = go.GetComponent<old_Battalion>();
            }
            if (bt != null && bt.battlegroup.team != attacker.battlegroup.team && !bt.is_dead)
            {
                float distance = Vector3.Distance(attacker.transform.position, go.transform.position);
                if (distance < closest_range)
                {
                    closest_range = distance;
                    closest_battalion = bt;
                }
            }
        }
        return closest_battalion;
    }

    public static old_Unit FindClosestEnemyInLine(old_Unit attacker, Vector3 start, Vector3 direction, float scalar)
    {
        RaycastHit[] allHits = Physics.RaycastAll(start, direction, scalar);

        float closest_range = Mathf.Infinity;
        old_Unit closestUnit = null;

        // return closest first
        foreach (RaycastHit hit in allHits)
        {
            GameObject go = hit.collider.gameObject;
            old_Unit bt = null;
            if (go != null)
            {
                bt = go.GetComponent<old_Unit>();
            }
            if (bt != null && bt.battalion.battlegroup.team != attacker.battalion.battlegroup.team && !bt.battalion.is_dead)
            {
                float distance = Vector3.Distance(attacker.transform.position, go.transform.position);
                if (distance < closest_range)
                {
                    closest_range = distance;
                    closestUnit = bt;
                }
            }
        }
        return closestUnit;
    }


    // starts rotation to a specific position
    public static void SlerpRotation(Transform transform, Vector3 direction)
    {
        // handles when trying to look at nothing, vector length 0
        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction, transform.TransformDirection(-Vector3.forward));
            rotation = new Quaternion(0, 0, rotation.z, rotation.w);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
        }
    }

    // finds the central point of various Battalions
    public static Vector3 FindCenterOfGroup(List<GameObject> list)
    {
        int count = 0;
        Vector3 center = Vector3.zero;
        foreach(GameObject b in list)
        {
            center += b.transform.position;
            count += 1;
        }
        if (count == 0)
        {
            return center;
        }
        else
        {
            return center / count;
        }
        
    }

    // finds the central point of various Battalions
    public static Vector3 FindCenterOfGroup(List<old_Battalion> list)
    {
        int count = 0;
        Vector3 center = Vector3.zero;
        foreach (old_Battalion b in list)
        {
            center += b.transform.position;
            count += 1;
        }
        if (count == 0)
        {
            return center;
        }
        else
        {
            return center / count;
        }

    }

    // finds the central point of various Battalions
    public static Vector3 FindCenterOfGroup(List<old_Unit> list)
    {
        int count = 0;
        Vector3 center = Vector3.zero;
        foreach (old_Unit b in list)
        {
            center += b.transform.position;
            count += 1;
        }
        if (count == 0)
        {
            return center;
        }
        else
        {
            return center / count;
        }

    }

    public static old_Battalion FindClosestUnitInLine(old_Battalion origin, Vector3 start, Vector3 direction, float scalar)
    {
        RaycastHit[] allHits = Physics.RaycastAll(start, direction, scalar);

        float closest_range = Mathf.Infinity;
        old_Battalion closest_battalion = null;

        // return closest first
        foreach (RaycastHit hit in allHits)
        {
            GameObject go = hit.collider.gameObject;
            old_Battalion bt = null;
            if (go != null)
            {
                bt = go.GetComponent<old_Battalion>();
            }
            if (bt != origin && bt != null && !bt.is_dead)
            {
                float distance = Vector3.Distance(start, go.transform.position);
                if (distance < closest_range)
                {
                    closest_range = distance;
                    closest_battalion = bt;
                }
            }
        }
        return closest_battalion;
    }

    // calculates how many contiguous friendly units are close formation with this unit
    public static int CalculateFormationStrength(old_Battalion unit)
    {
        int strength = 0;
        List<old_Battalion> lst = new List<old_Battalion>();

        Vector3[] directions = { unit.transform.up, -1 * unit.transform.up, unit.transform.right, -1 * unit.transform.right };
        foreach(Vector3 d in directions)
        {
            old_Battalion dir_unit = FindClosestUnitRadialDirection(unit, d, formationRange, 0.95f);
            if(dir_unit != null)
            {
                if (!lst.Contains(dir_unit))
                {
                    if(!dir_unit.is_dead && !dir_unit.falling_back)
                    {
                        if (dir_unit.battlegroup.team == unit.battlegroup.team)
                        {

                            strength += 1;
                        }
                        else
                        {
                            strength -= 1;
                        }
                        lst.Add(dir_unit);
                    }
                }
            }
        }

        return strength;
    }



    public static float CalculateFlankingBonus(Vector3 attackerFacing, Vector3 targetFacing)
    {
        float Angle = Vector3.Angle(attackerFacing, targetFacing);

        // Attacking from the rear
        if (Angle < 60)
        {
            return 2f;
        }
        // Attacking from the flank
        else if (Angle < 120)
        {
            return 1f;
        }
        // Attacking from the front
        else
        {
            return 0;
        }

    }



    // for a group of units reorganizing into formation, reorder the destination points per unit
    public static Dictionary<old_Battalion, Vector3> CalculateFormationOffset(List<old_Battalion> units,
                                                                Direction new_facing, Vector3 center)
    {
        Vector3 rightShift = Vector3.Cross(new_facing.GetVector(), new Vector3(0, 0, 1)).normalized;

        // Find all destinations to move toward
        List<Vector3> destinations = new List<Vector3>();
        int num_units = units.ToArray().Length;
        for (int i = 0; i < num_units; i++)
        {
            float d = 0;
            d = (i - ((num_units - 1) / 2f)) * old_BattleEngine.battleLaneWidth;

            destinations.Add(rightShift * d);
        }

        // Create every valid combination of units moving to each position
        List<Dictionary<old_Battalion, Vector3>> options = new List<Dictionary<old_Battalion, Vector3>>();

        int size = units.ToArray().Length;
        for (int i = 0; i < size; i++)
        {
            //print("Combo " + i.ToString());
            Dictionary<old_Battalion, Vector3> Offsets = new Dictionary<old_Battalion, Vector3>();
            for (int j = 0; j < size; j++)
            {
                int real_index = (j + i) % size;
                old_Battalion unit = units[j];
                Vector3 dest = destinations[real_index];
                Offsets[unit] = dest;
                //print(real_index.ToString() + "-" + unit.ToString() + "," + dest.ToString());
            }
            options.Add(Offsets);
        }

        //print(options.ToArray().Length.ToString() + " options");

        // Get the option with the lowest cost
        float lowest_cost = Mathf.Infinity;
        Dictionary<old_Battalion, Vector3> bestOffsets = new Dictionary<old_Battalion, Vector3>();
        foreach (Dictionary<old_Battalion, Vector3> Offsets in options)
        {
            float cost = 0;
            foreach (old_Battalion b in units)
            {
                // TODO: calculate cost by time to destination instead of total distance?
                float thisCost = Vector3.Distance(b.transform.position, Offsets[b]);
                cost += thisCost;
            }
            if (cost < lowest_cost)
            {
                bestOffsets = Offsets;
                lowest_cost = cost;
            }
            //print("Cost " + cost.ToString());
        }
        return bestOffsets;
    }

    // Returns the new ordered positions of all units into 5 lines
    public static Dictionary<old_Battalion, Vector3> CalculateBattleLineOffsets(List<old_Battalion> units, Direction new_facing, Vector3 center)
    {
        int line_index = 0;
        Dictionary<old_Battalion, Vector3> Offsets = new Dictionary<old_Battalion, Vector3>();

        old_Battalion.UnitClass[] UnitClasses = { old_Battalion.UnitClass.Archers, old_Battalion.UnitClass.Infantry, old_Battalion.UnitClass.Cavalry,
                                              old_Battalion.UnitClass.HorseArchers, old_Battalion.UnitClass.Elephants, old_Battalion.UnitClass.Artillery };
        foreach (old_Battalion.UnitClass cls in UnitClasses)
        {
            List<old_Battalion> line_units = new List<old_Battalion>();
            int count = 0;
            foreach (old_Battalion u in units)
            {
                if (u.unitclass == cls)
                {
                    line_units.Add(u);
                    count += 1;
                }
            }
            ;
            if (count > 0)
            {
                Offsets = AddOffsetDictionary(Offsets, line_units, line_index, new_facing, center);
                line_index += 1;
            }
        }

        return Offsets;
    }

    // Adds offsets of 1 dictionary (a single row in the battle line) to the master list
    public static Dictionary<old_Battalion, Vector3> AddOffsetDictionary(Dictionary<old_Battalion, Vector3> Offsets,
                                                            List<old_Battalion> unit_group, int line_index,
                                                               Direction new_facing, Vector3 center)
    {
        Vector3 lineDestination = line_index * -1 * old_BattleEngine.battleLaneWidth * 0.5f * new_facing.GetVector();
        Dictionary<old_Battalion, Vector3> offsets_1 = CalculateFormationOffset(unit_group, new_facing, center);
        foreach (old_Battalion b in unit_group)
        {
            Offsets[b] = offsets_1[b] + lineDestination;
        }
        return Offsets;
    }
}
