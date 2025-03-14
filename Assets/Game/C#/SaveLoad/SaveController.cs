using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    public float AutosaveInterval = 5f;
    public void Start()
    {
        BuildController.instance.OnBuildingDataChanged += SaveBuildings;
        StartCoroutine(AutoSaveRoutine());
        Application.quitting += () => SaveBuildings(BuildController.instance.GetBuildings());


        var list = LoadBuildings();
        foreach (var loadedBuilding in list)
        {
            BuildController.instance.PlaceBuilding(
                new Vector3(loadedBuilding.position.x, loadedBuilding.position.y, 0),
                loadedBuilding.buildingSO);
        }
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(AutosaveInterval);
            SaveBuildings(BuildController.instance.GetBuildings());
        }
    }
    private string path = Application.dataPath + "/save.json";

    public void SaveBuildings(Dictionary<Vector2Int, Building> buildings)
    {
        List<BuildingSaveData> buildingList = new List<BuildingSaveData>();
        HashSet<Building> AlreadySavedBuildings = new HashSet<Building>(); 

        foreach (var entry in buildings)
        {
            Building building = entry.Value;


            if (!AlreadySavedBuildings.Contains(building))
            {
                buildingList.Add(new BuildingSaveData
                {
                    position = entry.Key, 
                    buildingSO = building.building_SO
                });

                AlreadySavedBuildings.Add(building); 
            }
        }

        string json = JsonUtility.ToJson(new BuildingSaveWrapper(buildingList));
        File.WriteAllText(path, json);
    }

    public List<BuildingSaveData> LoadBuildings()
    {
        if (!File.Exists(path)) return new List<BuildingSaveData>();

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<BuildingSaveWrapper>(json).buildings;
    }
}

[System.Serializable]
public class BuildingSaveData
{
    public Vector2Int position;
    public Building_SO buildingSO;
}

[System.Serializable]
public class BuildingSaveWrapper
{
    public List<BuildingSaveData> buildings;

    public BuildingSaveWrapper(List<BuildingSaveData> buildings)
    {
        this.buildings = buildings;
    }
}
