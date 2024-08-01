using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.BattleDeck
{

    // Singleton
    public sealed class _MB_BattlefieldManager : MonoBehaviour
    {
        private static _MB_BattlefieldManager battlefield = null;

        private _MB_BattlefieldManager()
        {

        }

        public static _MB_BattlefieldManager Battlefield()
        {
            return battlefield;
        }


        // Start class
        public GameObject BattleSectorPrefab;
        Dictionary<Sector, _MB_BattleSectorManager> bsm_array = new Dictionary<Sector, _MB_BattleSectorManager>();
        public GameObject trianglePrefab;
        public GameObject squarePrefab;
        public GameObject ovalPrefab;
        ObjectRearranger bottom_sectors, top_sectors;

        public GameObject iconPrefabObject;
        public Sprite sword, arrow, shock, evasion, armor, shield, spear, banner, fear, star, trap, scoutting, hourglass;

        public GameObject GetUnitSprite()
        {
            return squarePrefab;
        }

        // Go through all enemy sectors and reveal what cards they played
        public void RevealPlays(List<Play> plays)
        {
            foreach (Play p in plays)
            {
                if (p.player == _MB_BattleRunner.Runner().GetScenario().player2)
                {
                    SetPlaySprites(p);
                }
            }
        }

        public void CreateSector(Sector bg, bool top_sector = false)
        {
            GameObject go = Object.Instantiate(BattleSectorPrefab);

            _MB_BattleSectorManager bsm = go.GetComponent<_MB_BattleSectorManager>();

            go.transform.SetParent(transform);

            bsm.battlegroup = bg;
            if (bsm_array.ContainsKey(bg))
            {
                bsm_array[bg] = bsm;
            }
            else
            {
                bsm_array.Add(bg, bsm);
            }

            bsm.sector = bg.sector;
            bsm.morale_bar = bsm.GetComponentInChildren<UI_Bar>();

            if (top_sector)
            {
                bsm.inverse_sector = true;
                go.transform.Rotate(new Vector3(0, 0, 180));
                Image bar_image = bsm.morale_bar.GetComponent<Image>();
                bar_image.fillOrigin = (int)Image.OriginHorizontal.Right;
                top_sectors.AddObject(go);

                bsm.retreat_direction = new Vector3(0, 200, 0);
            }
            else
            {
                bottom_sectors.AddObject(go);
                bsm.retreat_direction = new Vector3(0, -200, 0);
            }


        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetPlaySprites(Play play)
        {
            _MB_BattleSectorManager bsm = bsm_array[play.battleSector];

            bsm.SetPlaySprites(play);
        }

        public void RemoveAllPlaySprites()
        {
            foreach (_MB_BattleSectorManager bsm in bsm_array.Values)
            {
                bsm.ClearPlaySprites();
            }
        }

        public void RemovePlaySprites(Play play)
        {
            _MB_BattleSectorManager bsm = bsm_array[play.battleSector];

            bsm.ClearPlaySprites();
        }

        public _MB_BattleSectorManager GetSectorManager(Sector bs)
        {
            if (bsm_array.ContainsKey(bs))
            {
                return bsm_array[bs];
            }
            else
            {
                return null;
            }
        }

        public void Setup(Scenario scenario)
        {
            battlefield = this;
            top_sectors = new ObjectRearranger(145, 57, gameObject);
            bottom_sectors = new ObjectRearranger(145, -20, gameObject);

            // Create Battle Sectors
            foreach (Sector.Type sector in scenario.activeSectors)
            {
                CreateSector(scenario.army1.GetBattleSector(sector), false);
                CreateSector(scenario.army2.GetBattleSector(sector), true);
            }
        }



    }
}