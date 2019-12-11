using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpTargets : MonoBehaviour
{

    PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void OnBecameVisible()
    {
        if (!player.screenTargets.Contains(transform))
            player.screenTargets.Add(transform);
    }

    private void OnBecameInvisible()
    {
        if (player.screenTargets.Contains(transform))
            player.screenTargets.Remove(transform);
    }
}