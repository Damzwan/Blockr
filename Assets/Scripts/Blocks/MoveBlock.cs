using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    public class MoveBlock : MonoBehaviour, IBlock
    {
        public float moveSpeed = 5f;
        public float waitTime; //TODO implement

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
            transform.position = Vector3.MoveTowards(transform.position,
                waypoints[currWaypoint],
                (moveSpeed * Time.deltaTime));

            if (Vector3.Distance(waypoints[currWaypoint], transform.position) <= 0)
                currWaypoint++;


            if (currWaypoint != waypoints.Count) return;
            waypoints.Reverse();
            currWaypoint = 0;
        }
    
        // Update is called once per frame
        void Update()
        {
        }


        public void onBlockSpawned(GameObject block)
        {
            var col = block.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.center = new Vector3(0, 0.6f, 0);
            col.size = new Vector3(1.2f, 0.2f, 1.2f);
            block.AddComponent<MovingPlatform>();

        }
    }
}