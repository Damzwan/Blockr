using System;
using System.Collections;
using System.Collections.Generic;
using Blocks;
using UnityEngine;

public class BlockProjectile : MonoBehaviour
{
    public GameObject blockToSpawn;

    private Vector3 originalPos;
    private float minShootDistance = 1f;
    private bool alreadyHit;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyHit) return;
        if (Vector3.Distance(getSpawnPosition(other), originalPos) < minShootDistance) return;
        alreadyHit = true;
        Destroy(gameObject);

        if (blockToSpawn == null) Destroy(other.gameObject);
        else
        {
            var block = Instantiate(blockToSpawn, getSpawnPosition(other), other.transform.rotation);
            block.transform.parent = other.transform;
            other.GetComponent<IBlock>()?.onBlockSpawned(block);
        }
    }

    Vector3 getSpawnPosition(Collider other)
    {
        var dir = (other.transform.position - transform.position).normalized;
        Vector3 offset;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y) && Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
            offset = new Vector3(dir.x / Mathf.Abs(dir.x), 0, 0);
        else if (Mathf.Abs(dir.y) > Mathf.Abs(dir.z)) offset = new Vector3(0, dir.y / Mathf.Abs(dir.y), 0);
        else offset = new Vector3(0, 0, dir.z / Mathf.Abs(dir.z));

        return other.transform.position - offset;
    }
}