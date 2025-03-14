using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public Building_SO building_SO;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        BuildController.instance.SetBuilding(building_SO);
    }
}
