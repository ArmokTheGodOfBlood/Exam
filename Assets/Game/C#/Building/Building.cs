using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Building_SO building_SO;

    public void Initiate(Building_SO _building_SO)
    {
        building_SO = _building_SO;

        name = building_SO.buildingName;
        GetComponent<SpriteRenderer>().sprite = building_SO.sprite;
    }
}
