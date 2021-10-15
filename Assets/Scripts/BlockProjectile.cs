using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockProjectile : MonoBehaviour
{
    public GameObject cube;
    private bool alreadyHit;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyHit) return;
        alreadyHit = true;
        Destroy(gameObject);
        Instantiate(this.cube, getSpawnPosition(other), other.transform.rotation);
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