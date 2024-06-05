using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlateCounterVisual : MonoBehaviour
{
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;
    [SerializeField] private PlateCounter plateCounter;

    private List<GameObject> plateVisualGameObjectList;
    private const float OFFSET_Y = .1f;

    private void Awake() {
        plateVisualGameObjectList= new List<GameObject>();
    }

    private void Start() {
        plateCounter.OnPlateSpawn += PlateCounter_OnPlateSpawn;
        plateCounter.OnPlateDespawn += PlateCounter_OnPlateDespawn;
    }

    private void PlateCounter_OnPlateSpawn(object sender, System.EventArgs e) {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

        float plateOffsetY = plateVisualGameObjectList.Count * OFFSET_Y;
        plateVisualTransform.localPosition = new Vector3(0, plateOffsetY, 0);

        plateVisualGameObjectList.Add(plateVisualTransform.gameObject);
    }
    private void PlateCounter_OnPlateDespawn(object sender, System.EventArgs e) {
        GameObject plateGameObject = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];

        plateVisualGameObjectList.Remove(plateGameObject);
        Destroy(plateGameObject);
    }
}
