using System.Collections.Generic;
using UnityEngine;

public class old_Player : MonoBehaviour
{

    public List<old_Battalion> SelectedUnits = new List<old_Battalion>();
    public float last_rightclick = 0f;
    public float last_leftclick = 0f;
    public static float double_click_time = 0.5f;
    public static float drawBoundingBoxTime = 0.15f;
    public bool StickyFormation = true;

    public BattleGroup SelectedBattleGroup;

    public Vector3 box_start;
    public Vector3 box_end;
    public Vector2 mouseposBoxStart;

    public GameObject SelectionBoxLines;

    public List<old_Battalion> ControlGroup_1 = new List<old_Battalion>();
    public List<old_Battalion> ControlGroup_2 = new List<old_Battalion>();
    public List<old_Battalion> ControlGroup_3 = new List<old_Battalion>();
    public List<old_Battalion> ControlGroup_4 = new List<old_Battalion>();
    public List<old_Battalion> ControlGroup_5 = new List<old_Battalion>();

    public GameObject selectionBox;
    public old_Battalion LeadUnit;
    public int num_selected = 0;
    
    public enum OrderFormation { LockedFormation, SingleLine, BattleLine};
    public OrderFormation orderFormation = OrderFormation.LockedFormation;

    public old_Battalion mouseOverUnit;
    public bool FLAG_DRAWING_BOX = false;
    public bool FLAG_BUTTON_CLICKED = false;

    public UISoundManager uiSoundManager;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // Allows drawing bounding box with mouse
        if (Input.GetKey(KeyCode.Mouse0) || Input.GetMouseButtonUp(0))
        {
            UpdateSelectWithMouse();
        }

        // Right click tell units where to go
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            IssueOrders();
        }

        // Use the R key to toggle whether the unit runs or not.
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleRun();
        }

        // Use the F key to toggle whether the unit holds fire or not.
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleFireAtWill();
        }

        // Escape means deselect units
        if (Input.GetKey(KeyCode.Escape))
        {
            SelectedUnits.Clear();
        }

        // P means pause the game
        if (Input.GetKeyDown(KeyCode.P))
        {
            SelectedBattleGroup.BATTLE_MANAGER.TogglePause();
        }

        // Backspace means Stop orders!
        if (Input.GetKey(KeyCode.Backspace))
        {
            CancelOrders();
        }

        // Set control groups
        UpdateSelectionGroups();

        // Update which units are highlighted
        UpdateHighlighted();

        // set lead unit to the first one in the selection array
        UpdateLeadUnit();

        // reset flags
        FLAG_BUTTON_CLICKED = false;
    }


    // Choose which unit is selected
    public void UpdateSelectedUnit(old_Battalion new_unit)
    {
        // If the unit is not part of the current battle group, or if Shift is not held down,
        // Do not add the unit to a set of selected units. Instead, clear the set. Then add 
        // only this unit. 
        if (new_unit.battlegroup != SelectedBattleGroup ||
            !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            SelectedUnits.Clear();
        }
        SelectedBattleGroup = new_unit.battlegroup;
        if (!SelectedUnits.Contains(new_unit))
        {
            SelectedUnits.Add(new_unit);
        }
    }

    // Updates selected units to the current selection Box
    public void SelectUnitsInBoundingBox()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        box_end = mousePosition;
        float small_x, small_y, big_x, big_y;
        if (box_start.x < box_end.x)
        {
            small_x = box_start.x;
            big_x = box_end.x;
        }
        else
        {
            small_x = box_end.x;
            big_x = box_start.x;
        }
        if (box_start.y < box_end.y)
        {
            small_y = box_start.y;
            big_y = box_end.y;
        }
        else
        {
            small_y = box_end.y;
            big_y = box_start.y;
        }
        SelectedUnits.Clear();
        foreach (old_Battalion unit in SelectedBattleGroup.Units)
        {
            if ((small_x <= unit.transform.position.x && unit.transform.position.x <= big_x) &&
                    (small_y <= unit.transform.position.y && unit.transform.position.y <= big_y))
            {
                SelectedUnits.Add(unit);
            }
        }

        LineRenderer boundingLines = SelectionBoxLines.GetComponent<LineRenderer>();
        boundingLines.SetPosition(0, new Vector3(small_x, small_y, 0));
        boundingLines.SetPosition(1, new Vector3(small_x, big_y, 0));
        boundingLines.SetPosition(2, new Vector3(big_x, big_y, 0));
        boundingLines.SetPosition(3, new Vector3(big_x, small_y, 0));

        // show box
        if (!SelectionBoxLines.activeInHierarchy)
        {
            SelectionBoxLines.SetActive(true);
        }

        if (!SelectedUnits.Contains(mouseOverUnit) && mouseOverUnit != null && SelectedBattleGroup.Units.Contains(mouseOverUnit))
        {
            SelectedUnits.Add(mouseOverUnit);
        }
    }

    // Toggles Firing for all selected units
    public void ToggleFireAtWill()
    {
        bool all_holding_fire = true;
        FLAG_BUTTON_CLICKED = true;
        foreach (old_Battalion selectedUnit in SelectedUnits)
        {
            all_holding_fire = all_holding_fire && selectedUnit.command.should_hold_fire;
        }
        // if not every unit is shooting, or None of them are, then ENABLE shooting
        if (all_holding_fire)
        {
            foreach (old_Battalion selectedUnit in SelectedUnits)
            {
                selectedUnit.command.should_hold_fire = false;
            }
        }
        // only if every unit is already shooting should F disable fire
        else
        {
            foreach (old_Battalion selectedUnit in SelectedUnits)
            {
                selectedUnit.command.should_hold_fire = true;
            }
        }
    }

    // Toggles Firing for all selected units
    public void ToggleRun()
    {
        bool all_running = true;
        FLAG_BUTTON_CLICKED = true;
        foreach (old_Battalion selectedUnit in SelectedUnits)
        {
            all_running = all_running && selectedUnit.command.should_run;
        }
        // if not every unit is running, or None of them, then ENABLE run
        if (!all_running)
        {
            foreach (old_Battalion selectedUnit in SelectedUnits)
            {
                selectedUnit.command.should_run = true;
            }
        }
        // only if every unit is already running should R disable run
        else
        {
            foreach (old_Battalion selectedUnit in SelectedUnits)
            {
                selectedUnit.command.should_run = false;
            }
        }
    }

    // Cancels all orders for all selected units
    public void CancelOrders()
    {
        FLAG_BUTTON_CLICKED = true;
        foreach (old_Battalion selectedUnit in SelectedUnits)
        {
            selectedUnit.command = old_Command.GetDefaultCommand(selectedUnit);
        }
    }

    // Toggles Mount/Dismount for all selected units
    public void ToggleDismount()
    {

    }

    // Enables falling back order for all selected units
    public void OrderFallback()
    {
        FLAG_BUTTON_CLICKED = true;
        foreach (old_Battalion selectedUnit in SelectedUnits)
        {
            selectedUnit.command = old_Command.GetFallbackCommand(selectedUnit);
        }
    }

    // Changes units selected to a control group
    public void SelectGroup1()
    {
        FLAG_BUTTON_CLICKED = true;
        SelectedUnits.Clear();
        foreach (old_Battalion unit in ControlGroup_1)
        {
            SelectedUnits.Add(unit);
        }
    }

    // Changes units selected to a control group
    public void SelectGroup2()
    {
        FLAG_BUTTON_CLICKED = true;
        SelectedUnits.Clear();
        foreach (old_Battalion unit in ControlGroup_2)
        {
            SelectedUnits.Add(unit);
        }
    }

    // Changes units selected to a control group
    public void SelectGroup3()
    {
        FLAG_BUTTON_CLICKED = true;
        SelectedUnits.Clear();
        foreach (old_Battalion unit in ControlGroup_3)
        {
            SelectedUnits.Add(unit);
        }
    }

    // Changes units selected to a control group
    public void SelectGroup4()
    {
        FLAG_BUTTON_CLICKED = true;
        SelectedUnits.Clear();
        foreach (old_Battalion unit in ControlGroup_4)
        {
            SelectedUnits.Add(unit);
        }
    }

    // Changes units selected to a control group
    public void SelectGroup5()
    {
        SelectedUnits.Clear();
        foreach (old_Battalion unit in ControlGroup_5)
        {
            SelectedUnits.Add(unit);
        }
    }

    // Handles Updating control groups
    public void UpdateSelectionGroups()
    {
        ControlGroup_1 = SelectedBattleGroup.GetSubsetOfUnits(old_Battalion.UnitClass.Infantry);
        ControlGroup_2 = SelectedBattleGroup.GetSubsetOfUnits(old_Battalion.UnitClass.Archers);
        ControlGroup_3 = SelectedBattleGroup.GetSubsetOfUnits(old_Battalion.UnitClass.Cavalry);
        ControlGroup_4 = SelectedBattleGroup.GetSubsetOfUnits(old_Battalion.UnitClass.HorseArchers);
        ControlGroup_5 = SelectedBattleGroup.GetSubsetOfUnits(old_Battalion.UnitClass.Artillery);

        // all controls that work when holding down CTRL key
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            // CTRL + A means select all units in this battlegroup
            if (Input.GetKeyDown(KeyCode.A))
            {
                SelectedUnits.Clear();
                foreach (old_Battalion battalion in SelectedBattleGroup.Units)
                {
                    SelectedUnits.Add(battalion);
                }
            }
        }
        // any commands that do not work when holding down control
        else
        {
            // Use number keys to select groups
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectGroup1();
            }
            // Use number keys to select groups
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectGroup2();
            }
            // Use number keys to select groups
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectGroup3();
            }
            // Use number keys to select groups
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SelectGroup4();
            }
            // Use number keys to select groups
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SelectGroup5();
            }
        }
    }

    // Update which ones are Highlighted
    public void UpdateHighlighted()
    {
        num_selected = 0;
        foreach (old_Battalion b in SelectedBattleGroup.Units)
        {

            b.LOSPrimary.SetActive(true);
            b.LOSSecondary.SetActive(true);

            if (SelectedUnits.Contains(b))
            {
                b.unitMesh.HighlightUnit();
                num_selected += 1;
                b.UnitPathMarker.line.enabled = true;
            }
            else
            {
                b.unitMesh.UnHighlightUnit();
                b.UnitPathMarker.line.enabled = false;
            }
        }
    }

    // Sets which unit is the Lead unit of the currently selected
    public void UpdateLeadUnit()
    {
        if (num_selected > 0)
        {
            // TODO is there a more ideal "lead unit"?
            LeadUnit = SelectedUnits[0];
        }
        else
        {
            LeadUnit = null;
        }
    }

    // Updates units selected based on Mouse inputs at the time
    public void UpdateSelectWithMouse()
    {
        float now = Time.time;

        // Click and drag left click bounding box
        if (Input.GetMouseButtonDown(0))
        {
            mouseposBoxStart = Input.mousePosition;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseposBoxStart);
            last_leftclick = Time.time;
            box_start = mousePosition;
            if(mouseOverUnit != null && SelectedBattleGroup.Units.Contains(mouseOverUnit))
            {
                UpdateSelectedUnit(mouseOverUnit);
            }
        }
        // If mouse has been held down at least this long, draw bounding box
        else if (now - last_leftclick > drawBoundingBoxTime)
        {
            FLAG_DRAWING_BOX = true;
            SelectUnitsInBoundingBox();
            if (Input.GetMouseButtonUp(0))
            {
                FLAG_DRAWING_BOX = false;
                // hide box
                if (SelectionBoxLines.activeInHierarchy)
                {
                    SelectionBoxLines.SetActive(false);
                }
            }
        }
        // if you click somewhere with no unit, and it wasnt a button, and it wasn't held down for too long,
        // then deselect everything
        else if (Input.GetKeyUp(KeyCode.Mouse0) && mouseOverUnit == null && !FLAG_BUTTON_CLICKED)
        {
            SelectedUnits.Clear();
        }
    }

    // Processes unit orders given status and Right Click location
    public void IssueOrders()
    {
        if(num_selected == 0)
        {
            return;
        }
        bool should_run = false;
        bool should_fallback = false;
        bool sidestep = false;
        string name = "Advance";
        bool rotateOnly = false;

        // Otherwise, the first right click just tells units to Advance towards a point
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Double right click means to run to the destination
        if (Time.time - last_rightclick <= double_click_time)
        {
            //should_run = true;
        }

        // Holding Alt while right clicking means to fall back to this point.
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            name = "Rotate";
            rotateOnly = true;
        }

        // Holding CTRL while right clicking means to shift Laterally to new position
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            name = "Redeploy";
            sidestep = true;
        }

        // Set for all units
        Vector3 centerPoint = old_BattleEngine.FindCenterOfGroup(SelectedUnits);
        Vector3 destination = new Vector3(mousePosition.x, mousePosition.y, 0);
        //Direction new_facing = Direction.ClosestDirection(destination - centerPoint);

        // keeps the same facing as currently
        if (sidestep)
        {
            
        }

        // Set up formation offsets depending on movement type
        Dictionary<old_Battalion, Vector3> Offsets = new Dictionary<old_Battalion, Vector3>();
        switch (orderFormation)
        {
            case OrderFormation.LockedFormation:
                foreach (old_Battalion SelectedUnit in SelectedUnits)
                {
                    Offsets[SelectedUnit] = SelectedUnit.transform.position - centerPoint;
                }
                break;
            case OrderFormation.SingleLine:
                //Offsets = CalculateFormationOffset(SelectedUnits, new_facing, centerPoint); ;
                break;
            case OrderFormation.BattleLine:
                //Offsets = CalculateBattleLineOffsets(SelectedUnits, new_facing, centerPoint);
                break;
            default:
                break;
        }

        // issue the appropriate order for each of the selected units
        foreach (old_Battalion SelectedUnit in SelectedUnits)
        {
            should_run = SelectedUnit.command.should_run;
            Vector3 solo_destination = destination;
            // Don't move, only rotate to face this direction
            if (rotateOnly)
            {
                solo_destination = SelectedUnit.transform.position;
            }
            // Move given the above rules
            else
            {
                Vector3 OffsetFromCenter = Offsets[SelectedUnit];
                solo_destination = destination + OffsetFromCenter;
            }
            
            old_Command moveCommand = new old_Command(name, solo_destination, should_run, 
                SelectedUnit.command.should_hold_fire, should_fallback, SelectedUnit);
            
            // TODO: allow enqueueing commands?
            SelectedUnit.command = moveCommand;
        }
        last_rightclick = Time.time;

        uiSoundManager.OrderUnits();
    }

    // Allows buttons to change desired formation commands
    public void ChangeToLockedFormation()
    {
        orderFormation = OrderFormation.LockedFormation;
        FLAG_BUTTON_CLICKED = true;
    }

    // Allows buttons to change desired formation commands
    public void ChangeToSingleLineFormation()
    {
        orderFormation = OrderFormation.SingleLine;
        FLAG_BUTTON_CLICKED = true;
    }

    // Allows buttons to change desired formation commands
    public void ChangeToBattleLineFormation()
    {
        orderFormation = OrderFormation.BattleLine;
        FLAG_BUTTON_CLICKED = true;
    }



    // Report that this unit is moused over
    public void SuggestMouseOver(old_Battalion btln)
    {
        if(mouseOverUnit == null)
        {
            mouseOverUnit = btln;
        }
    }

    // Report that this unit is no longer being moused over
    public void NotMouseOver(old_Battalion btln)
    {
        if (mouseOverUnit == btln)
        {
            mouseOverUnit = null;
        }
    }
}
