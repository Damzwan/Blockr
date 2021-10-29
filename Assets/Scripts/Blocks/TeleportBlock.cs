using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBlock : MonoBehaviour
{
    void Start()
    {
        var player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        player.setTeleportBlock(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }
}