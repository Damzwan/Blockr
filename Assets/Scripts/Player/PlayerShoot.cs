using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public float shootVelocity = 10f;
    public List<GameObject> projectiles;

    private CinemachineFreeLook cineCam; //TODO somewhere else

    private float angle = 45f;
    private Transform t;
    private Camera cam;
    private float timeStep = 0.05f;
    private LineRenderer lr;
    private bool cameraFrozen;

    private int selectedProjectileIndex;

    private KeyCode[] keyCodes =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
    };

    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Transform>();
        cam = Camera.main;
        lr = GetComponent<LineRenderer>();
        cineCam = GameObject.Find("Third Person Camera").GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        handleInput();
        handleShooting();
    }

    void handleInput()
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
                selectedProjectileIndex = i > projectiles.Count ? projectiles.Count - 1 : i;
        }
    }

    void handleShooting()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (cameraFrozen) freezeCamera();
            cameraFrozen = true;
            var pos = t.position;
            Plane plane = new Plane(Vector3.up, 0);
            Vector3 worldPosition = new Vector3();

            angle = Mathf.Clamp(angle - Input.mouseScrollDelta.y, 5, 90);

            float distance;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance)) worldPosition = ray.GetPoint(distance);

            var forward = worldPosition - pos;
            forward.y = 0;

            t.rotation = Quaternion.LookRotation(forward, Vector3.up);

            var trajectoryPoints = calculateTrajectory();
            lr.positionCount = trajectoryPoints.Count;
            lr.SetPositions(trajectoryPoints.ToArray());

            if (Input.GetKeyDown(KeyCode.Mouse0)) shoot();
        }
        else
        {
            lr.positionCount = 0;
            if (cameraFrozen) unFreezeCamera();
            cameraFrozen = false;
        }
    }

    List<Vector3> calculateTrajectory()
    {
        var startPos = t.position;
        var velocity = calculateVelocity();
        var gravity = new Vector3(0, -20f, 0); //TODO use gravity of physics engine

        var points = new List<Vector3>();
        var time = 0f;

        var currTrajectoryPoint = startPos;

        while (currTrajectoryPoint.y > 0)
        {
            time += timeStep;
            currTrajectoryPoint = startPos + velocity * time + 0.5f * gravity * Mathf.Pow(time, 2);
            points.Add(currTrajectoryPoint);
        }

        return points;
    }

    void shoot()
    {
        var obj = Instantiate(projectiles[selectedProjectileIndex], t.position, Quaternion.identity);
        obj.GetComponent<Rigidbody>().velocity = calculateVelocity();
        ;
    }

    Vector3 calculateVelocity()
    {
        var dir = transform.forward;
        return new Vector3(dir.x * shootVelocity * Mathf.Cos(Mathf.Deg2Rad * angle),
            Mathf.Sin(Mathf.Deg2Rad * angle) * shootVelocity, dir.z * shootVelocity * Mathf.Cos(Mathf.Deg2Rad * angle));
    }

    //TODO place somewhere else
    void freezeCamera()
    {
        cineCam.m_XAxis.m_InputAxisName = "";
        cineCam.m_XAxis.m_InputAxisValue = 0;

        cineCam.m_YAxis.m_InputAxisName = "";
        cineCam.m_YAxis.m_InputAxisValue = 0;
    }

    void unFreezeCamera()
    {
        cineCam.m_XAxis.m_InputAxisName = "Mouse X";
        cineCam.m_YAxis.m_InputAxisName = "Mouse Y";
    }
}