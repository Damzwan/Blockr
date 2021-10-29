using UnityEngine;

namespace Blocks
{
    //TODO fix better name...
    public class MovingPlatform : MonoBehaviour
    {
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
    }
}