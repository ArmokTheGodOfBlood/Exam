using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building_SO", menuName = "Buildings/Building_SO")]
public class Building_SO : ScriptableObject
{
    public string buildingName;
    public Vector2Int size;

    public Sprite thumbnail;
    public Sprite sprite;
    public GameObject prefab;
}
