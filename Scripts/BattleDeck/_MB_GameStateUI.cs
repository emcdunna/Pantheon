using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.BattleDeck
{
    public class _MB_GameStateUI : MonoBehaviour
    {

        public GameObject Playing, Morale, Ranged, Shock, Melee;

        private static Color ActiveColor = new Color(1,1,1,1);
        private static Color InActiveColor = new Color(0.5f,0.5f,0.5f,0.75f);

        private GameObject CurrentlyActive;
        private _MB_BattleRunner runner;

        void Start()
        {
            
            SetAllInactive();
        }

        // Update is called once per frame
        void Update()
        {

            runner = _MB_BattleRunner.Runner();
            switch (runner.GetState())
            {
                case _MB_BattleRunner.STATE.ANIMATION:

                    switch (runner.GetAnimationPhase())
                    {
                        case UnitAnimation.AnimationPhase.MeleePhase:
                            SetActive(Melee);
                            break;

                        case UnitAnimation.AnimationPhase.MoralePhase:
                            SetActive(Morale);
                            break;

                        case UnitAnimation.AnimationPhase.RangedPhase:
                            SetActive(Ranged);
                            break;

                        case UnitAnimation.AnimationPhase.ShockPhase:
                            SetActive(Shock);
                            break;

                        default:
                            SetInactive(CurrentlyActive);
                            break;
                    }
                    break;

                case _MB_BattleRunner.STATE.WAITING:
                    SetActive(Playing);
                    break;

                //case _MB_BattleRunner.STATE.DEPLOYMENT:
                //    break;

                case _MB_BattleRunner.STATE.ENDOFROUND:
                    break;

                default:
                    SetInactive(CurrentlyActive);
                    break;
            }
        }

        private void SetActive(GameObject go)
        {
            if (go == CurrentlyActive)
            {
                return;
            }
            else
            {
                SetInactive(CurrentlyActive);
            }
            Image[] icons = go.GetComponentsInChildren<Image>();
            foreach (Image i in icons)
            {
                i.color = ActiveColor;
            }
            CurrentlyActive = go;
        }

        private void SetInactive(GameObject go)
        {
            if (go == null)
            {
                return;
            }
            Image[] icons = go.GetComponentsInChildren<Image>();
            foreach(Image i in icons)
            {
                i.color = InActiveColor;
            }
        }

        private void SetAllInactive()
        {
            SetInactive(Playing);
            SetInactive(Ranged);
            SetInactive(Melee);
            SetInactive(Shock);
            SetInactive(Morale);
        }
    }
}