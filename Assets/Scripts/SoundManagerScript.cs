using System;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private void Start()
    {
        DeliveryManager.OnAnyRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.OnAnyRecipeFail += DeliveryManager_OnRecipeFail;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.Instance.OnPickedSomething += Player_OnPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.trash, (sender as BaseCounter).transform.position);
    }

    private void BaseCounter_OnAnyObjectPlacedHere(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.objectDrop, (sender as BaseCounter).transform.position);
    }

    private void Player_OnPickedSomething(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.objectPickup, (sender as Player).transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.deliverySucces, (sender as CuttingCounter).transform.position);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, DeliveryManager.DeliveryManagerEventArgs e)
    {
        if (e.DeliveryCounter == null)
        {
            PlaySound(audioClipRefsSO.deliverySucces, Camera.main.transform.position);
        }
        PlaySound(audioClipRefsSO.deliverySucces, e.DeliveryCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeFail(object sender, DeliveryManager.DeliveryManagerEventArgs e)
    {
        if(e.DeliveryCounter == null)
        {
            PlaySound(audioClipRefsSO.deliveryFail, Camera.main.transform.position);
        }
        PlaySound(audioClipRefsSO.deliveryFail, e.DeliveryCounter.transform.position);
    }

    private void PlaySound(AudioClip[] audioClips, Vector3 pos, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClips[UnityEngine.Random.Range(0, audioClips.Length)], pos, volume);
    }

    private void PlaySound(AudioClip audioClip, Vector3 pos, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, pos, volume);
    }

    public void PlayFootstepsSound(Vector3 pos, float volume = 1f)
    {
        PlaySound(audioClipRefsSO.footstep, pos, volume);
    }
}
 