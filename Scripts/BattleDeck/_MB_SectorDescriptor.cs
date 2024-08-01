using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.BattleDeck
{
    public class _MB_SectorDescriptor : MonoBehaviour
    {

        public Text SectorName;
        public Sector battleSector;
        public GameObject swords, arrows, shock, evade, shields, spears, armor;


        public void SetBattleSector(Sector battleSector)
        {
            this.battleSector = battleSector;
            SectorName.text = battleSector.sector.ToString();
            UpdateText(swords, battleSector.GetSwords());
            UpdateText(arrows, battleSector.GetArrows());
            UpdateText(shock, battleSector.GetShock());
            UpdateText(evade, battleSector.GetEvasion());
            UpdateText(shields, battleSector.GetShields());
            UpdateText(spears, battleSector.GetSpears());
            UpdateText(armor, battleSector.GetAverageArmor());

        }


        // Update is called once per frame
        void Update()
        {

        }

        void UpdateText(GameObject go, object newText)
        {
            Text text = go.GetComponentInChildren<Text>();
            text.text = newText.ToString();
        }
    }
}