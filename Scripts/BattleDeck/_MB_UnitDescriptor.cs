using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Assets.Scripts.BattleDeck
{
    public class _MB_UnitDescriptor : MonoBehaviour
    {

        public Text UnitName;
        public Text UnitType, Melee, Ranged, Shield, Experience, Recruitment, Cards;
        Battalion battalion = null;
        public GameObject swords, arrows, shock, evade, shields, spears, armor, morale;


        public void SetBattalion(Battalion battalion)
        {
            this.battalion = battalion;
            UnitName.text = battalion.name;
            UnitType.text = battalion.GetUnitType();
            Melee.text = battalion.melee_weapon.name;
            Ranged.text = battalion.ranged_weapon.name;
            Shield.text = battalion.shield_weapon.name;
            Experience.text = battalion.experience.name;
            Recruitment.text = battalion.recruitment.name;

            string cardtext = "";
            int i = 0;
            List<Card> cards = battalion.GetCards();
            foreach (Card card in cards)
            {
                i++;
                cardtext += card.name;
                if (i != cards.Count)
                {
                    cardtext += ", ";
                }
                
            }
            Cards.text = cardtext;
            UpdateText(swords, battalion.GetSwords());
            UpdateText(arrows, battalion.GetArrows());
            UpdateText(shock, battalion.GetShock());
            UpdateText(evade, battalion.GetEvasion());
            UpdateText(shields, battalion.GetShields());
            UpdateText(spears, battalion.GetSpears());
            UpdateText(armor, battalion.GetArmor());
            UpdateText(morale, battalion.GetMorale());
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(battalion != null)
            {
                
            }
            
        }

        void UpdateText(GameObject go, object newText)
        {
            
            Text text = go.GetComponentInChildren<Text>();
            text.text = newText.ToString();

            if ((int)newText == 0)
            {
                go.SetActive(false);
            }
            else
            {
                go.SetActive(true);
            }

        }
    }
}