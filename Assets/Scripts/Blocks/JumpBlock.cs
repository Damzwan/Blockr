using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBlock : MonoBehaviour, IInteractableBlock
{
    public float jumpForce = 10f;

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
        var dir = (player.transform.position - transform.position).normalized;
        playerMovement.addImpact(getHitDirection(dir), jumpForce);
    }

    Vector3 getHitDirection(Vector3 dir)
    {
        Vector3 hitDir;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y) && Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
            hitDir = new Vector3(dir.x / Mathf.Abs(dir.x), 0, 0);
        else if (Mathf.Abs(dir.y) > Mathf.Abs(dir.z)) hitDir = new Vector3(0, dir.y / Mathf.Abs(dir.y), 0);
        else hitDir = new Vector3(0, 0, dir.z / Mathf.Abs(dir.z));

        return hitDir;
    }
}