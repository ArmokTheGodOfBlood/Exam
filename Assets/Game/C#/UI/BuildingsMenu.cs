using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public class BuildingsMenu : MonoBehaviour
{
    [SerializeField] Transform buttonParent;
    [SerializeField] Button buttonPrefab;    
    private List<Building_SO> buildings; 

    private void Awake()
    {
        LoadBuildings();
        GenerateButtons();
    }

    void LoadBuildings()
    {
        buildings = Resources.LoadAll<Building_SO>("Buildings").ToList();
    }

    void GenerateButtons()
    {
        foreach (var buildingData in buildings)
        {
            Button newButton = Instantiate(buttonPrefab, buttonParent);
            newButton.GetComponent<Image>().sprite = buildingData.thumbnail;
            BuildingButton btnData = newButton.gameObject.AddComponent<BuildingButton>();
            btnData.building_SO = buildingData;
        }
    }
}
