using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlateCounter : BaseCounter, IHasProgress
{
    public event EventHandler OnPlateSpawn;
    public event EventHandler OnPlateDespawn;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    private const float plateSpawnTimeThreshold = 5f;
    private const uint platesMaxCount = 4;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float plateSpawnTimer;
    private uint platesCount;

    /*
        public override void Interact_MultiplePlates(Player player) {
            if (player.HasKitchenObject()) {
                if (plates.Count > platesStorageMax) {
                    Debug.Log(name + " is at full capacity.");
                    return;
                }

                if (player.GetKitchenObject().GetComponent<Plate>() is null) {
                    if (HasKitchenObject()) {
                        Debug.Log(name + " already has a KitchenObject.");
                        return;
                    }

                    if (plates.Count > 0) {
                        Debug.Log(name + " has already plates.");
                        return;
                    }
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    return;
                }

                player.GetKitchenObject().SetKitchenObjectParent(this);
                plates.Push((Plate)GetKitchenObject());
                Debug.Log(name + " store a plate.");
            } else {
                if (!HasKitchenObject()) {
                    Debug.Log(name + " has no KitchenObject.");
                    return;
                }

                if (GetKitchenObject().GetComponent<Plate>() is null) {
                    GetKitchenObject().SetKitchenObjectParent(player);
                    return;
                }

                if (plates.Count == 0) {
                    Debug.Log(name + " has no plates.");
                    return;
                }

                GetKitchenObject().SetKitchenObjectParent(player);
                plates.Pop();
                Debug.Log(player.name + " pick up a plate.");
            }
            return;
        }
    */

    //public override void SecondTry_Didnotwork_again_Interact(Player player)
    //{
    //    PlateKitchenObject plateKitchenObject;

    //    if (HasKitchenObject())
    //    {
    //        if (player.HasKitchenObject())
    //        {
    //            if (player.GetKitchenObject() is PlateKitchenObject)
    //            {
    //                Debug.Log(name + " already has a KitchenObject.");
    //                return;
    //            }

    //            if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
    //            {
    //                plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO());
    //            }
    //        }

    //        GetKitchenObject().SetKitchenObjectParent(player);
    //        return;
    //    }

    //    if (platesCount == 0)
    //    {
    //        if (!player.HasKitchenObject())
    //        {
    //            Debug.Log(player + " has nothing to interact with.");
    //            return;
    //        }

    //        player.GetKitchenObject().SetKitchenObjectParent(this);
    //        ResetPlateSpawnTimer();
    //        return;
    //    }

    //    if (!player.HasKitchenObject())
    //    {
    //        RemovePlate();
    //        SpawnPlateToPlayer(player);
    //        ResetPlateSpawnTimer();
    //        return;
    //    }

    //    if (player.GetKitchenObject().TryGetPlate(out plateKitchenObject))
    //    {
    //        if (platesCount >= platesMaxCount)
    //        {
    //            Debug.Log(name + " is at full capacity.");
    //            return;
    //        }

    //        AddPlate();
    //        DestroyPlateFromPlayer(player);
    //        ResetPlateSpawnTimer();
    //    }
    //    else
    //    {
    //        var kitchenObject = player.GetKitchenObject();
    //        player.ClearKitchenObject();

    //        RemovePlate();
    //        SpawnPlateToPlayer(player, kitchenObject.GetKitchenObjectSO());
    //        ResetPlateSpawnTimer();

    //        kitchenObject.DestroySelf();

    //        player.GetKitchenObject().SetKitchenObjectParent(this);
    //        ResetPlateSpawnTimer();
    //    }
    //}


    private void Update()
    {
        if (HasKitchenObject())
        {
            return;
        }

        if (platesCount >= platesMaxCount)
        {
            return;
        }

        plateSpawnTimer += Time.deltaTime;

        if (plateSpawnTimer >= plateSpawnTimeThreshold)
        {
            AddPlate();
            UpdateProgressUI(ProgressBarUI.PROGRESS_MAX);
            ResetPlateSpawnTimer();
        }

        if (platesCount == 0)
        {
            UpdateProgressUI(plateSpawnTimer / plateSpawnTimeThreshold);
        }
    }

    private void UpdateProgressUI(float progressNormalized)
    {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = progressNormalized
        });
    }

    private void SpawnPlateToPlayer(Player player, KitchenObjectSO kitchenObjectSO = null)
    {
        var plate = (PlateKitchenObject) KitchenObject.SpawnObject(plateKitchenObjectSO, player);
        plate.TryAddIngredient(kitchenObjectSO);
    }

    private void DestroyPlateFromPlayer(Player player)
    {
        player.GetKitchenObject().DestroySelf();
    }

    private void AddPlate()
    {
        platesCount++;
        OnPlateSpawn?.Invoke(this, EventArgs.Empty);
    }

    private void RemovePlate()
    {
        platesCount--;
        OnPlateDespawn?.Invoke(this, EventArgs.Empty);
    }

    public void InteractV1Backup(Player player)
    {
        if (HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject() is PlateKitchenObject)
                {
                    Debug.Log(name + " already has a KitchenObject.");
                    return;
                }

                if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO());
                }
            }

            GetKitchenObject().SetKitchenObjectParent(player);
            return;
        }

        if (platesCount > 0)
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject() is not PlateKitchenObject)
                {
                    Debug.Log(name + " already has KitchenObject.");
                    return;
                }
                if (platesCount >= platesMaxCount)
                {
                    Debug.Log(name + " is at full capacity.");
                }
                else
                {
                    AddPlate();
                    DestroyPlateFromPlayer(player);
                    ResetPlateSpawnTimer();
                }
            }
            else
            {
                Debug.LogWarning("Can't do that yet. In development.");
                return;

                RemovePlate();
                SpawnPlateToPlayer(player);
                ResetPlateSpawnTimer();
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject() is not PlateKitchenObject)
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    ResetPlateSpawnTimer();
                    return;
                }
                AddPlate();
                DestroyPlateFromPlayer(player);
                ResetPlateSpawnTimer();
            }
            else
            {
                Debug.Log(player + " has nothing to interact with.");
            }
        }
    }

    public override void Interact(Player player)
    {
        if (HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject() is PlateKitchenObject)
                {
                    Debug.Log(name + " already has a KitchenObject.");
                    return;
                }

                if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO());
                }
            }

            GetKitchenObject().SetKitchenObjectParent(player);
            return;
        }

        if (platesCount > 0)
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject() is not PlateKitchenObject)
                {
                    Debug.Log(name + " already has KitchenObject.");
                    return;
                }
                if (platesCount >= platesMaxCount)
                {
                    Debug.Log(name + " is at full capacity.");
                }
                else
                {
                    // Accept only empty plates
                    Debug.LogWarning("Can't do that yet. In development.");
                    return;

                    AddPlate();
                    DestroyPlateFromPlayer(player);
                    ResetPlateSpawnTimer();
                }
            }
            else
            {
                //Debug.LogWarning("Can't do that yet. In development.");
                //return;

                RemovePlate();
                SpawnPlateToPlayer(player);
                ResetPlateSpawnTimer();
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject() is not PlateKitchenObject)
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    ResetPlateSpawnTimer();
                    return;
                }

                // Ingredients stuck on player, should be an ingredient in a plate
                Debug.LogWarning("Can't do that yet. In development.");
                return;

                AddPlate();
                DestroyPlateFromPlayer(player);
                ResetPlateSpawnTimer();
            }
            else
            {
                Debug.Log(player + " has nothing to interact with.");
            }
        }
    }


    private void ResetPlateSpawnTimer()
    {
        plateSpawnTimer = 0f;
        UpdateProgressUI(ProgressBarUI.PROGRESS_MIN);
    }

    public override void InteractAlternate(Player player)
    {
        Debug.Log(name + " has no alternate interact action.");
    }
}
