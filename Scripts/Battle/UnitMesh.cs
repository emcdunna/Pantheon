using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMesh : MonoBehaviour
{

    public int formationSpread = 1; // % that units are randomly spreadout
    public List<old_Unit> units = new List<old_Unit>();
    public float formationSpacing = 0.3f; // base amount of distance between units
    public int formationDrift = 0; // % amount formation is drifting to becoming bigger
    public int formationWidth = 2; // number of men wide
    public int formationDepth = 1; // Descriptive of how many ranks deep the unit is

    public float fixedDelayTime = 0f; // amount of time until the troops get the order
    public float widthOverDepth = 1f; // ratio of width to depth in spacing
    public int size = 0;
    public int maxSize = 0;
    public float momentum = 0f;

    public float nextAttack = 0f;

    public float max_acceleration = 0.5f; // max change in speed per second
    public GameObject unitPrefab;

    private old_Command command;


    private SpriteRenderer sprite; // for unit ICON on battlefield

    public static Vector3 SortingDirectionVector = new Vector3(0.4f, -1f, 0);
    public old_Battalion battalion;

    public static int MAX_SPREAD = 33;
    public static int MAX_DRIFT = 75;



    public float rotationStrength = 0.75f;


    public Direction facing = new Direction(Direction.Type.North);


    public float current_speed = 1f;
    public float max_movement_speed = 1f;


    // Start is called before the first frame update
    void Start()
    {
        

        if (size != 0 && unitPrefab != null)
        {
            SetUnits(size, unitPrefab);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        command = battalion.command;

        // Update Facing variable
        UpdateFacing();

        // Update rotation as needed
        UpdateRotation();

        // Move to command destination
        Move();

        // Update settings based on battalion status
        UpdateFormation();

        // Reset unit positions
        UpdatePositions();
    }

    private void UpdateFormation()
    {
        float cohRatio = battalion.cohesion / 100f;

        formationDrift = Mathf.RoundToInt((1 - cohRatio) * MAX_DRIFT);

        formationSpread = Mathf.RoundToInt((1 - cohRatio) * MAX_SPREAD);

        float HpRatio = battalion.men * 1.0f / battalion.starting_men;

        int newSize = Mathf.CeilToInt(HpRatio * maxSize);

        if(newSize < size)
        {
            RemoveUnit(units[0]);
        }

    }

    // instantiate a series of units to be drawn in the unit mesh
    public void SetUnits(int size, GameObject unitPrefab)
    {
        foreach(old_Unit u in units)
        {
            RemoveUnit(u);
            
        }
        for (int i = 0; i < size; i++)
        {
            GameObject unit = Instantiate(unitPrefab);
            old_Unit u = unit.GetComponent<old_Unit>();
            u.unitMesh = this;
            u.battalion = this.battalion;
            units.Add(u);
        }
        this.size = size;
        this.unitPrefab = unitPrefab;

        // resort positions
        UpdatePositions();

        // Teleport units to the desired position
        foreach(old_Unit u in units)
        {
            u.transform.position = u.desiredPosition;
        }
        maxSize = size;
    }

    public void RemoveUnit(old_Unit unit)
    {
        GameObject.Destroy(unit.gameObject);
        units.Remove(unit);
        size -= 1;
    }

    private void UpdatePositions()
    {
        Vector3 rightShift = transform.right;
        Vector3 backShift = -1 * transform.up;
        
        float totalFrontage = formationWidth * formationSpacing;
        this.formationDepth = Mathf.CeilToInt(size / formationWidth);
        int remainingUnits = size;
        for(int rank = 0; rank <= formationDepth; rank ++)
        {
            float spacing = formationSpacing * (1 + formationDrift / 100f);
            int files = 0;
            float frontage = 0;
            if(remainingUnits >= formationWidth)
            {
                files = formationWidth;
                frontage = totalFrontage;
            }
            else
            {
                files = remainingUnits;
                frontage = spacing * files;
            }
            //print("Rank " + rank.ToString() + " - sp: " + spacing.ToString() + " files: " + files.ToString());
            for(int file = 0; file < files; file++)
            {
                // needs to be at time of commands not here
                //float orderDelay = Time.time + fixedDelayTime + Random.Range(0f, 0.1f);

                // Shift in the horizontal direction
                Vector3 pos = rightShift * ((file + 0.5f) * spacing + frontage * -0.5f);

                // Add in the current position of the mesh
                pos = pos + transform.position;

                // Add in the current vertical displacement
                pos = pos + backShift * rank * spacing * widthOverDepth;
                
                // Choose the appropriate unit
                old_Unit unit = units[file + rank * formationWidth];

                if(unit != null)
                {
                    unit.formationOffset = new Vector2(file, rank);

                    // Random spread of the unit given its current spread
                    pos = pos + unit.spreadDirection * formationSpacing * (formationSpread / 100f);

                    // Set the unit's final position
                    unit.desiredPosition = pos;

                    //print("File " + file.ToString() + " - pos: " + pos.ToString() + " ru: " + remainingUnits.ToString());

                    // Reduce counter
                    remainingUnits -= 1;

                    int sortingOrder = Mathf.RoundToInt(Vector3.Dot(pos, SortingDirectionVector) * 100f);
                    //print("SO: " + sortingOrder.ToString());

                    //unit.UpdateSortingOrder(10 + file + rank * formationWidth);
                    unit.UpdateSortingOrder(100 + sortingOrder);
                }
            }
        }
    }
    
    // sets underlying selection marker active
    public void HighlightUnit()
    {
        foreach (old_Unit u in units)
        {
            u.highlight.SetActive(true);
        }
    }

    // sets underlying selection marker active
    public void UnHighlightUnit()
    {
        foreach (old_Unit u in units)
        {
            u.highlight.SetActive(false);
        }
    }

    // Updates the facing variable to represent the discrete facing parameter
    public void UpdateFacing()
    {
        facing = Direction.ClosestDirection(transform.up);
    }

    // return True/False for if the unit moved
    public void Move()
    {
        // If our command is finished, we can proceed to the next one.
        if (command.IsComplete())
        {
            command.complete = true;
            if (command.NextCommand != null)
            {
                command = command.NextCommand;
            }
            else
            {
                command = old_Command.GetDefaultCommand(battalion);
            }
        }

        transform.position = Vector3.Lerp(transform.position, command.destination, 1);

        float dist = Vector3.Distance(transform.position, command.destination);

        if(dist > 1)
        {
            //transform.position = Vector3.Lerp(transform.position, transform.position + transform.up * Time.deltaTime, 1);
        }
        else
        {
            
        }
        
        
    }

    public void UpdateRotation()
    {
        Vector3 direction = command.destination - transform.position;
        if (direction.sqrMagnitude > 0.05f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, command.destination - transform.position);
            float str = Mathf.Min(rotationStrength * Time.deltaTime, 1);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
        }
    }

    
}
