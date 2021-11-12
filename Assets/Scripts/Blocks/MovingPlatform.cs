using System;
using System.Linq;
using UnityEngine;

namespace Blocks
{
    //TODO fix better name...
    public class MovingPlatform : MonoBehaviour, IBlock
    {
        // This collider is used to make the player stay on the platform
        // If a new block gets added to the moving platform we want to add the same collider to this new block
        private BoxCollider triggerColliderBounds;

        private void Start()
        {
            triggerColliderBounds = GetComponents<BoxCollider>().ToList().Find(col => col.isTrigger);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            other.GetComponent<PlayerMovement>().incrementMovingPlatformAmount();
            other.transform.parent = transform;
        }


        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerMovement>();
            player.decrementMovingPlatformAmount();
            if (player.getMovingPlatformAmount() == 0) other.transform.parent = null;
        }

        public void onBlockSpawned(GameObject block)
        {
            var col = block.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.center = triggerColliderBounds.center;
            col.size = triggerColliderBounds.size;
            block.AddComponent<MovingPlatform>();
        }
    }
}