using UnityEngine;

namespace Projectiles
{
    public class TeleportProjectile : BlockProjectile
    {
        void Start()
        {
            //TODO expensive method I think
            var player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            player.setTeleportBlock(gameObject);
        }
    }
}