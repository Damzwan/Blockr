using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blocks
{
    public class MoveBlock : MonoBehaviour
    {
        public float moveSpeed = 5f;

        public float waitTime; //TODO implement
        private float waitTimer = 0;

        private List<Vector3> waypoints = new List<Vector3>();
        private int currWaypoint = 1;
        private Vector3 startPos;


        // Start is called before the first frame update
        void Start()
        {
            waypoints.Add(transform.position);
            foreach (Transform child in transform) waypoints.Add(child.transform.position);
        }

        private void FixedUpdate()
        {
            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position,
                waypoints[currWaypoint],
                (moveSpeed * Time.deltaTime));

            if (Vector3.Distance(waypoints[currWaypoint], transform.position) <= 0)
            {
                waitTimer = waitTime;
                currWaypoint++;
            }

            if (currWaypoint != waypoints.Count) return;
            waypoints.Reverse();
            currWaypoint = 0;
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}