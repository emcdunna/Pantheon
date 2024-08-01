using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPanel : MonoBehaviour
{
    public GameObject ButtonPrefab;
    public BattleGroup battlegroup;

    public float y_offset = 100;
    // Todo: better way to know this automatically
    public float TotalWidth = 600;
    public float ButtonHeight = 60;
    public float ButtonWidth = 100;

    private int buttonCount = 0;

    public List<GameObject> Buttons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SetupDefaultButtons();
    }

    public void SetupDefaultButtons()
    {
        GameObject tb1 = AddButton("Select1", "Infantry");
        UpdateButtonFunction(tb1, battlegroup.player.SelectGroup1);
        GameObject tb2 = AddButton("Select2", "Archers");
        UpdateButtonFunction(tb2, battlegroup.player.SelectGroup2);
        GameObject tb3 = AddButton("Select3", "Melee Cavalry");
        UpdateButtonFunction(tb3, battlegroup.player.SelectGroup3);
        GameObject tb4 = AddButton("Select4", "Horse Archers");
        UpdateButtonFunction(tb4, battlegroup.player.SelectGroup4);
        GameObject tb5 = AddButton("Select5", "Artillery");
        UpdateButtonFunction(tb5, battlegroup.player.SelectGroup5);

    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }
    
    // Updates/Redraws unit panel for the current unit
    public void UpdateUI()
    {

    }

    // Create new text box, add it to the UI.
    private GameObject AddButton(string box_name, string new_text)
    {
        int x_offset = Mathf.RoundToInt(transform.position.x - 0.5f*TotalWidth + ButtonWidth * (buttonCount + 0.5f));
        GameObject tb = Instantiate(ButtonPrefab);
        tb.name = box_name;
        UpdateButton(tb, new_text);
        tb.transform.SetParent(transform);
        RectTransform rt = tb.GetComponent<RectTransform>();
        rt.position = new Vector3(x_offset, y_offset, 0);
        Buttons.Add(tb);
        buttonCount += 1;
        rt.sizeDelta = new Vector2(Mathf.Abs(ButtonWidth), Mathf.Abs(ButtonHeight));
        return tb;
    }

    // Changes the text of a text box to match a new string
    private void UpdateButton(GameObject tb, string new_text)
    {
        Text txt = tb.GetComponentInChildren<Text>();
        txt.text = new_text;
    }

    // updates the On Click to a specific function
    private void UpdateButtonFunction(GameObject tb, UnityEngine.Events.UnityAction function)
    {

        Button bt = tb.GetComponent<Button>();
        bt.onClick.RemoveAllListeners();
        bt.onClick.AddListener(function);
    }
}
