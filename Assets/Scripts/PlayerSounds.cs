using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private SoundManagerScript soundManager;

    private Player player;
    private float footstepTimer;
    private float footstepTimerMax = .1f;
    private float footstepVolume = 1f;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        footstepTimer -= Time.deltaTime;

        if(footstepTimer < 0)
        {
            footstepTimer = footstepTimerMax;

            if(player.IsWalking())
            {
                soundManager.PlayFootstepsSound(player.transform.position, footstepVolume);
            } 
        }
    }
}
