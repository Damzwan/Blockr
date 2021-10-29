using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisBlock : MonoBehaviour, IInteractableBlock
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void interact(GameObject player)
    {
        var playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.freeze();
    }
}