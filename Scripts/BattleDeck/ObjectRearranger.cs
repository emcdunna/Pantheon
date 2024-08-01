using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.BattleDeck
{
    // Object rearranger that spreads objects out equally in the X axis
    public class ObjectRearranger
    {

        public List<GameObject> objects = new List<GameObject>();
        int spread = 5;
        int y_offset = 0;
        GameObject origin;

        public ObjectRearranger(int Spread = 0, int Y_offset = 0, GameObject Origin = null)
        {
            spread = Spread;
            y_offset = Y_offset;
            origin = Origin;
        }

        public void UpdateSpread()
        {
            Vector3 offset = new Vector3(0, y_offset, -3);
            float x_offset = 0;
            int i = 0;
            foreach (GameObject obj in objects)
            {
                x_offset = i * spread - ((objects.Count / 2.0f - 0.5f) * spread);


                offset.x = x_offset;
                obj.transform.position = origin.transform.position + offset;
                i += 1;
            }

        }

        public void AddObject(GameObject gameObject)
        {
            objects.Add(gameObject);
            if (origin != null)
            {
                gameObject.transform.SetParent(origin.transform);
            }
            UpdateSpread();
        }

        public void RemoveObject(GameObject gameObject)
        {
            objects.Remove(gameObject);
            Transform.Destroy(gameObject);
            UpdateSpread();
        }

        public void Clear()
        {
            List<GameObject> temp = new List<GameObject>();
            foreach (GameObject go in objects)
            {
                temp.Add(go);
            }
            foreach (GameObject go in temp)
            {
                RemoveObject(go);
            }
        }



    }
}

