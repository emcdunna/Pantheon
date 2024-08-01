using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.BattleDeck
{
    // Manages a single battle sector so there will be many of these
    public class _MB_BattleSectorManager : MonoBehaviour
    {

        public UI_Bar morale_bar = null;
        public float morale_ratio = 1;
        public Sector battlegroup = null;
        public Sector.Type sector = Sector.Type.Center;
        public bool inverse_sector = false;
        public List<_MB_UnitUI> units = new List<_MB_UnitUI>();
        public Dictionary<int, ObjectRearranger> unit_sets = new Dictionary<int, ObjectRearranger>();
        public ObjectRearranger play_sets;
        public Dictionary<Battalion, GameObject> battlefieldUnits = new Dictionary<Battalion, GameObject>();

        public Vector3 retreat_direction = Vector3.zero;
        public Play activePlay;

        public Text hotkeyText, moraleText;

        bool hasRotated = false;


        // Start is called before the first frame update
        void Start()
        {
            int y_offset = 26;
            if (inverse_sector)
            {
                y_offset = -26;
            }
            play_sets = new ObjectRearranger(20, y_offset, gameObject);
            if (morale_bar == null)
            {
                morale_bar = GetComponentInChildren<UI_Bar>();
                morale_bar.SetHealthBarValue(0);
            }
        }

        // Update is called once per frame
        void Update()
        {

            moraleText.text = Mathf.CeilToInt(battlegroup.GetMorale()).ToString();

            if (battlegroup != null && !battlegroup.HasAlreadyBroken())
            {
                morale_ratio = ((float)battlegroup.GetMorale()) / (float)battlegroup.morale;
                morale_bar.SetHealthBarValue(morale_ratio);

                if (inverse_sector && !hasRotated)
                {
                    moraleText.transform.Rotate(0, 0, 180);
                    hasRotated = true;
                }

                UpdateUnitSprites();

                UpdateHotkeyText();

                RemoveUnnecessaryUnits();
            }

        }

        void RemoveUnnecessaryUnits()
        {
            _MB_PlayerHUD hud = _MB_BattleRunner.Runner().PlayerHud;
            List<_MB_UnitUI> removed = new List<_MB_UnitUI>();
            foreach (_MB_UnitUI unitUI in units)
            {
                Battalion battalion = unitUI.GetBattalion();
                if (!battlegroup.Contains(battalion))
                {
                    Battalion.Type type = battalion.type;
                    int typeNum = -1;

                    if (type == Battalion.Type.Cavalry)
                    {
                        typeNum = -3;
                    }
                    else if (battalion.GetRole() == Battalion.Role.Ranged)
                    {
                        typeNum = 1;
                    }

                    if(hud.GetSelectedUnit() == battalion)
                    {
                        hud.UpdateSelectedUnit(null);
                    }

                    ObjectRearranger or = unit_sets[typeNum];
                    or.RemoveObject(unitUI.gameObject);
                    removed.Add(unitUI);
                    battlefieldUnits.Remove(battalion);
                    Debug.Log("Deleting old unit " + battalion + " in sector " + battlegroup.sector);
                }
            }

            foreach(_MB_UnitUI unitUI in removed)
            {
                units.Remove(unitUI);
            }
            
        }

        void UpdateHotkeyText()
        {
            if (hotkeyText == null)
            {
                return;
            }
            else if (inverse_sector)
            {
                hotkeyText.text = "";
            }
            else
            {
                switch (sector)
                {
                    case Sector.Type.Center:
                        hotkeyText.text = "2";
                        break;
                    case Sector.Type.Left:
                        hotkeyText.text = "1";
                        break;
                    case Sector.Type.Right:
                        hotkeyText.text = "3";
                        break;
                    default:
                        hotkeyText.text = "";
                        break;
                }
            }

        }

        public Sector.Stats GetMaximumStats()
        {
            return battlegroup.GetMaximumStats();
        }

        // Update sprites for the play
        public void SetPlaySprites(Play play)
        {
            activePlay = play;
            play_sets.Clear();

            AddPlaySprite(play, __Engine.CardSymbol.arrow, play.card.arrows);
            AddPlaySprite(play, __Engine.CardSymbol.sword, play.card.swords);
            AddPlaySprite(play, __Engine.CardSymbol.shock, play.card.shock);
            AddPlaySprite(play, __Engine.CardSymbol.spear, play.card.spears);
            AddPlaySprite(play, __Engine.CardSymbol.shield, play.card.shields);
            AddPlaySprite(play, __Engine.CardSymbol.evasion, play.card.evasion);
            AddPlaySprite(play, __Engine.CardSymbol.banner, play.card.banners);
            AddPlaySprite(play, __Engine.CardSymbol.fear, play.card.fear);
            AddPlaySprite(play, __Engine.CardSymbol.scoutting, play.card.scoutting);
            if (play.card.cross_attack)
            {
                AddPlaySprite(play, __Engine.CardSymbol.star, 1);
            }
            if (play.card.strike_back)
            {
                AddPlaySprite(play, __Engine.CardSymbol.trap, 1);
            }

        }

        // Adds the given play sprite
        void AddPlaySprite(Play play, __Engine.CardSymbol symbol, int times = 1)
        {
            if (times == 0)
            {
                return;
            }

            Sprite newsprite = null;
            int total_effective = 0;
            _MB_BattlefieldManager battlefield = _MB_BattlefieldManager.Battlefield();
            switch (symbol)
            {
                case __Engine.CardSymbol.arrow:
                    newsprite = battlefield.arrow;
                    total_effective = play.effectiveArrows;
                    break;
                case __Engine.CardSymbol.sword:
                    newsprite = battlefield.sword;
                    total_effective = play.effectiveSwords;
                    break;
                case __Engine.CardSymbol.shock:
                    newsprite = battlefield.shock;
                    total_effective = play.effectiveShock;
                    break;
                case __Engine.CardSymbol.spear:
                    newsprite = battlefield.spear;
                    total_effective = play.effectiveSpears;
                    break;
                case __Engine.CardSymbol.shield:
                    newsprite = battlefield.shield;
                    total_effective = play.effectiveShields;
                    break;
                case __Engine.CardSymbol.evasion:
                    newsprite = battlefield.evasion;
                    total_effective = play.effectiveEvasion;
                    break;
                case __Engine.CardSymbol.banner:
                    newsprite = battlefield.banner;
                    total_effective = times;
                    break;
                case __Engine.CardSymbol.fear:
                    newsprite = battlefield.fear;
                    total_effective = times;
                    break;
                case __Engine.CardSymbol.scoutting:
                    newsprite = battlefield.scoutting;
                    total_effective = times;
                    break;
                case __Engine.CardSymbol.armor:
                    newsprite = battlefield.armor;
                    total_effective = times;
                    break;
                case __Engine.CardSymbol.star:
                    newsprite = battlefield.star;
                    total_effective = times;
                    break;
                case __Engine.CardSymbol.trap:
                    newsprite = battlefield.trap;
                    total_effective = times;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < times; i++)
            {
                GameObject newPlaySprite = Object.Instantiate(battlefield.iconPrefabObject);

                SpriteRenderer sr = newPlaySprite.GetComponent<SpriteRenderer>();
                sr.sprite = newsprite;

                sr.color = Color.gray;
                Text damageText = newPlaySprite.GetComponentInChildren<Text>();
                damageText.text = (total_effective / times).ToString();
                play_sets.AddObject(newPlaySprite);
            }
            

        }

        // Removes all Play sprites
        public void ClearPlaySprites()
        {
            activePlay = null;
            play_sets.Clear();
        }

        void OnMouseOver()
        {
            _MB_BattleRunner runner = _MB_BattleRunner.Runner();
            _MB_PlayerHUD hud = runner.PlayerHud;
            hud.SetSectorDescriptorPanel(battlegroup);
            hud.OpenSectorDescriptorPanel();

            if (Input.GetMouseButtonDown(0))
            {
                _MB_BattleRunner.Runner().ClickSector(sector);
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (runner.GetState() == _MB_BattleRunner.STATE.DEPLOYMENT)
                {
                    Battalion battalion = hud.GetSelectedUnit();
                    if (battalion != null)
                    {
                        battlegroup.Deploy(battalion);
                    }
                }
                else
                {
                    Debug.Log("Not allowed to deploy units at this time.");
                }
            }

        }

        void OnMouseExit()
        {
            _MB_PlayerHUD hud = _MB_BattleRunner.Runner().PlayerHud;
            hud.CloseSectorDescriptorPanel();
        }

        void UpdateUnitSprites()
        {

            foreach (Battalion battalion in battlegroup.units)
            {
                Battalion.Type type = battalion.type;

                if (!battlefieldUnits.ContainsKey(battalion))
                {
                    CreateUnit(battalion);
                }
            }
        }


        void CreateUnit(Battalion battalion)
        {
            Battalion.Type type = battalion.type;
            int typeNum = -1;

            GameObject prefab = _MB_BattlefieldManager.Battlefield().GetUnitSprite();
            if (type == Battalion.Type.Cavalry)
            {
                prefab = _MB_BattlefieldManager.Battlefield().ovalPrefab;
                typeNum = -3;
            } else if (battalion.GetRole() == Battalion.Role.Ranged)
            {
                prefab = _MB_BattlefieldManager.Battlefield().trianglePrefab;
                typeNum = 1;
            }
            GameObject newunit = Object.Instantiate(prefab);

            battlefieldUnits[battalion] = newunit;

            _MB_UnitUI unitUI = newunit.GetComponent<_MB_UnitUI>();
            battalion.UnitUI = unitUI;
            units.Add(unitUI);

            SpriteRenderer sr = newunit.GetComponent<SpriteRenderer>();

            Scenario scenario = _MB_BattleRunner.Runner().GetScenario();


            Vector3 forwardDirection;
            Vector3 frontlinesPosition;
            if (inverse_sector)
            {
                sr.color = scenario.faction_2.primary;
                forwardDirection = new Vector3(0, -1, 0);
                frontlinesPosition = transform.position + new Vector3(0, -20, 0);
            }
            else
            {
                sr.color = scenario.faction_1.primary;
                forwardDirection = new Vector3(0, 1, 0);
                frontlinesPosition = transform.position + new Vector3(0, 20, 0);
            }
            sr.sortingOrder += 1;

            

            ObjectRearranger or;
            if (!unit_sets.ContainsKey(typeNum))
            {
                int y_offset = typeNum * 9;

                
                if (inverse_sector)
                {
                    y_offset *= -1;
                }
                or = new ObjectRearranger(16, y_offset, gameObject);

                unit_sets.Add(typeNum, or);

            }

            or = unit_sets[typeNum];
            or.AddObject(newunit);
            

            unitUI.Setup(battalion, forwardDirection, frontlinesPosition);
        }


        public void BreakSector(float timeDelay = 0)
        {
            battlegroup.BreakSector();

            Canvas canvas = GetComponentInChildren<Canvas>();
            canvas.gameObject.SetActive(false);

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.enabled = false;

            foreach (ObjectRearranger or in unit_sets.Values)
            {
                foreach (GameObject go in or.objects)
                {
                    _MB_UnitUI unit = go.GetComponent<_MB_UnitUI>();
                    unit.Break(timeDelay);
                }
            }
            ClearPlaySprites();
        }


        

    }
}