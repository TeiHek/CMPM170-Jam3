using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCharControl : MonoBehaviour
{
    public List<GameObject> Units;

    [Tooltip("Enable the little tile on the ground to make u see the location")]
    [SerializeField] bool enableTileRendering;

    private void Awake()
    {
        if (!enableTileRendering)
        {
            for(int i = 0; i < Units.Count; i++)
            {
                int childrenCount = Units[i].transform.childCount;
                for(int n = 0; n < childrenCount; n++)
                {
                    Units[i].transform.GetChild(n).GetComponent<Renderer>().enabled = false;
                }
            }
        }
    }

}
