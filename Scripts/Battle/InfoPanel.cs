using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// A class to control printing a series of lines of text on a GUI image
public class InfoPanel : MonoBehaviour
{

    public GameObject TextPrefab;
    public old_Player HumanPlayer;
    public old_Battalion UnitShown;

    public enum PanelType { UnitStatus, UnitInfo, FactionInfo, Tooltip };
    public PanelType type = PanelType.UnitStatus;

    private int boxCount = 0;

    public List<GameObject> TextBoxes = new List<GameObject>();
    public Image image;
    private float mouseOverTime = 0;
    private float minMouseOverTime = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        
        switch (type)
        {
            case PanelType.UnitStatus:
                MakeUnitStatusPanel();
                break;
            case PanelType.UnitInfo:
                MakeUnitInfoPanel();
                break;
            case PanelType.Tooltip:
                break;
            default:
                break;
        }
    }

    public void MakeUnitStatusPanel()
    {
        AddTextBox("Name Box", "Name");
        AddTextBox("Combat Box", "Combat Bonuses");
        AddTextBox("Kills Box", "Kills");
        AddTextBox("Men Box", "Men");
        AddTextBox("Morale Box", "Morale");
        AddTextBox("Cohesion Box", "Cohesion");
        AddTextBox("Fatigue Box", "Fatigue");
        AddTextBox("Ammunition Box", "Ammunition");
        AddTextBox("Formation Box", "Formation");
        AddTextBox("Impact Box", "Impact Weapon");
    }

    public void MakeUnitInfoPanel()
    {
        AddTextBox("Name Box", "Name");
        AddTextBox("Discipline Box", "Discipline");
        AddTextBox("Armor Box", "Armor");
        AddTextBox("Prowess Box", "Prowess");
        AddTextBox("Confidence Box", "Confidence");
        AddTextBox("Speed Box", "Speed");
        AddTextBox("Weight Box", "Weight");
        AddTextBox("PrimaryWeapon Box", "Primary Weapon");
        AddTextBox("Sidearm Box", "Sidearm Weapon");
        AddTextBox("Impact Weapon Box", "Impact Weapon");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUnitShown();

        UpdateUI();
    }

    // Decides which unit is to be displayed in the info panel
    public void UpdateUnitShown()
    {
        UnitShown = HumanPlayer.LeadUnit;
    }

    // Updates/Redraws unit panel for the current unit
    public void UpdateUI()
    {
        // decide whether or not to hide the panel
        if (type == PanelType.UnitInfo)
        {
            if (HumanPlayer.mouseOverUnit == null)
            {
                UnitShown = null;
                HidePanel();
                mouseOverTime = Mathf.Infinity;
            }
            else if(Time.time > mouseOverTime + minMouseOverTime)
            {
                UnitShown = HumanPlayer.mouseOverUnit;
                ShowPanel();
                transform.position = Input.mousePosition + new Vector3(200,-75,0);
            }
            else if (mouseOverTime == Mathf.Infinity)
            {
                mouseOverTime = Time.time;
            }
        }

        // rewrite text boxes
        foreach (GameObject text in TextBoxes)
        {
            string new_string = "";
            if (UnitShown != null)
            {
                if (text.name == "Name Box")
                {
                    new_string = UnitShown.UnitName;
                }
                else if (text.name == "Combat Box")
                {
                    //new_string = "ADV: " + UnitShown.Advantages.ToString() + ", DIS: " + UnitShown.Disadvantages.ToString();
                }
                else if (text.name == "Fatigue Box")
                {
                    new_string = "Fatigue: " + Mathf.RoundToInt(UnitShown.fatigue).ToString() + "%";
                }
                else if (text.name == "Kills Box")
                {
                    new_string = "Kills: " + UnitShown.kills.ToString();
                }
                else if (text.name == "Men Box")
                {
                    new_string = "Troops: " + UnitShown.men.ToString() + " / " + UnitShown.starting_men.ToString();
                }
                else if (text.name == "Weight Box")
                {
                    new_string = "Weight: " + UnitShown.GetUnitWeight().ToString();
                }
                else if (text.name == "Morale Box")
                {
                    new_string = "Morale: " + UnitShown.morale.ToString();
                }
                else if (text.name == "Formation Box")
                {
                    new_string = "Formation: " + UnitShown.formationStrength.ToString();
                }
                else if (text.name == "Cohesion Box")
                {
                    new_string = "Cohesion: " + Mathf.RoundToInt(UnitShown.cohesion).ToString() + "%";
                }
                else if (text.name == "Discipline Box")
                {
                    new_string = "Discipline: " + UnitShown.discipline.ToString();
                }
                else if (text.name == "Armor Box")
                {
                    new_string = "Armor: " + UnitShown.armor.ToString();
                }
                else if (text.name == "Speed Box")
                {
                    new_string = "Speed: " + UnitShown.max_movement_speed.ToString();
                }
                else if (text.name == "Confidence Box")
                {
                    new_string = "Confidence: " + UnitShown.Confidence.ToString();
                }
                else if (text.name == "Prowess Box")
                {
                    new_string = "Prowess: " + UnitShown.Prowess.ToString();
                }
                else if (text.name == "PrimaryWeapon Box")
                {
                    new_string = GetWeaponText(UnitShown.primary_weapon);
                }
                else if (text.name == "Sidearm Box" && UnitShown.side_arm != null)
                {
                    new_string = GetWeaponText(UnitShown.side_arm);
                }
                else if (text.name == "Impact Box")
                {
                    new_string = GetImpact(UnitShown.impact_weapon);
                }
                else if (text.name == "Ammunition Box")
                {
                    new_string = "Ammunition: " + UnitShown.ammunition.ToString();
                }
                else if (text.name == "Ammunition Box")
                {
                    new_string = "Impact Weapon Box: " + GetWeaponText(UnitShown.GetImpactWeapon());
                }
                UpdateTextbox(text, new_string);
            }
        }
        
        
    }

    // Create new text box, add it to the UI.
    private void AddTextBox(string box_name, string new_text)
    {
        //int y_offset = Mathf.RoundToInt(TotalHeight - BoxHeight * (boxCount + 1));
        GameObject tb = Instantiate(TextPrefab);
        tb.name = box_name;
        UpdateTextbox(tb, new_text);
        tb.transform.SetParent(transform);
        RectTransform rt = tb.GetComponent<RectTransform>();
        //rt.position = new Vector3(x_offset, y_offset, 0);
        TextBoxes.Add(tb);
        boxCount += 1;
    }

    // Removes a text box from system
    private void DeleteTextBox(GameObject tb)
    {
        TextBoxes.Remove(tb);
        GameObject.Destroy(tb, 0.01f);
        boxCount -= 1;
        // Todo: would have to redraw all boxes to make that work
    }

    // Changes the text of a text box to match a new string
    private void UpdateTextbox(GameObject tb, string new_text)
    {
        Text txt = tb.GetComponent<Text>();
        txt.text = new_text;
    }

    public string GetImpact(Weapon weapon)
    {
        if(weapon.type == Weapon.Type.Impact)
        {
            // Mathf.RoundToInt(weapon.damage * UnitShown.momentum).ToString()
            return "? Impact Damage";
        }
        else
        {
            return "No impact weapon.";
        }
    }

    public string GetWeaponText(Weapon weapon)
    {
        if (weapon == null)
        {
            return "No weapon";
        }
        switch (weapon.type)
        {
            case Weapon.Type.Impact:
                return weapon.name + ": DMG " + weapon.damage.ToString() + ", PEN " +
                    weapon.penetration.ToString() + ", RNG Impact";
            case Weapon.Type.Melee:
                return weapon.name + ": DMG " + weapon.damage.ToString() + ", RATE " + Mathf.RoundToInt(60f / weapon.attack_delay).ToString() + ", AP " +
                    weapon.penetration.ToString() + ", RNG Melee";
            default:
                return weapon.name + ": DMG " + weapon.damage.ToString() + ", RATE " + Mathf.RoundToInt(60f/weapon.attack_delay).ToString() + ", AP " +
                    weapon.penetration.ToString() + ", RNG " + weapon.range.ToString();
        }

    }

    public void HidePanel()
    {
        image.enabled = false;
        foreach (GameObject tb in TextBoxes)
        {
            tb.SetActive(false);
        }
    }

    public void ShowPanel()
    {
        image.enabled = true;
        foreach (GameObject tb in TextBoxes)
        {
            tb.SetActive(true);
        }
    }
}
