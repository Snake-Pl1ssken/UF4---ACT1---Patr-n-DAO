using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public bool usesAutopilot = true;

    public float maxSpeed;
    public float maxTurnSpeed;

    public float acceleration;
    public float turnAcceleration;



    public float backAccelerationFront = 0.1f;
    public float backAccelerationSides = 0.1f;
    public float turnBackAcceleration = 0.1f;

    public Transform sensorsOrigin;
    public float sensorsRangeSides;
    public float sensorsSideDeadZone = 2;

    float speed;
    float turnSpeed;

    float angle;

    Rigidbody rigidbody;

    // Control flags

    bool accelerate;
    bool reverse;
    bool turnLeft;
    bool turnRight;

    LevelManager levelManagerC;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        levelManagerC = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!levelManagerC.IsRacing()) { return; }

        if(!usesAutopilot)
        {
            if (Input.GetKey(KeyCode.W)) { accelerate = true; }
            else if (Input.GetKey(KeyCode.S)) { reverse = true; }

            if (Input.GetKey(KeyCode.A)) { turnLeft = true; }
            else if(Input.GetKey(KeyCode.D)) { turnRight = true; }
        }
        
    }

    void FixedUpdate()
    {
        if (!levelManagerC.IsRacing()) { return; }

        if (usesAutopilot)
        {
            float leftSpace = Mathf.Infinity;
            float rightSpace = Mathf.Infinity;

            RaycastHit hit;


            if(Physics.Raycast(sensorsOrigin.position, sensorsOrigin.right, out hit, sensorsRangeSides))
            {
                rightSpace = hit.distance;
            }

            if (Physics.Raycast(sensorsOrigin.position, -sensorsOrigin.right, out hit, sensorsRangeSides))
            {
                leftSpace = hit.distance;
            }

            if (Mathf.Abs(leftSpace - rightSpace) > sensorsSideDeadZone)
            {
                if (leftSpace > rightSpace) { turnLeft = true; }
                else { turnRight = true; }
            }

            accelerate = true;
        }

        Vector3 speed = rigidbody.velocity;

        if (accelerate)
        {
            speed += transform.forward * acceleration * Time.deltaTime;
        }

        else if(reverse)
        {
            speed -= transform.forward * acceleration * Time.deltaTime;
        }

        if (speed.magnitude > maxSpeed) { speed = speed.normalized * maxSpeed; }

        Vector3 speedLocal = transform.InverseTransformVector(speed);

        if(!accelerate)
        {
            if(speedLocal.z > 0)
            {
                speedLocal -= new Vector3(0, 0, backAccelerationFront * Time.deltaTime);
                if (speedLocal.z < 0) { speedLocal = new Vector3(speedLocal.x, speedLocal.y, 0); }
            }

        }

        if(speedLocal.x > 0)
        {
            speedLocal -= new Vector3(backAccelerationSides * Time.deltaTime, 0, 0);
            if (speedLocal.x < 0) { speedLocal = new Vector3(0, speedLocal.y, speedLocal.z); }
        }
        else if (speedLocal.x < 0)
        {
            speedLocal += new Vector3(backAccelerationSides * Time.deltaTime, 0, 0);
            if (speedLocal.x > 0) { speedLocal = new Vector3(0, speedLocal.y, speedLocal.z); }
        }

        speed = transform.TransformVector(speedLocal);


        Vector3 angularSpeed = rigidbody.angularVelocity;

        if(turnLeft)
        {
            angularSpeed -= new Vector3(0, turnAcceleration * Mathf.Deg2Rad * Time.deltaTime, 0);

        }
        else if(turnRight)
        {
            angularSpeed += new Vector3(0, turnAcceleration * Mathf.Deg2Rad * Time.deltaTime, 0);
        }

        if(!turnLeft && !turnRight)
        {
            if(angularSpeed.y > 0)
            {
                angularSpeed -= new Vector3(0, turnBackAcceleration * Mathf.Deg2Rad * Time.deltaTime, 0);
                if(angularSpeed.y < 0) { angularSpeed = new Vector3(angularSpeed.x, 0, angularSpeed.z); }
            }
            else if (angularSpeed.y < 0)
            {
                angularSpeed += new Vector3(0, turnBackAcceleration * Mathf.Deg2Rad * Time.deltaTime, 0);
                if (angularSpeed.y > 0) { angularSpeed = new Vector3(angularSpeed.x, 0, angularSpeed.z); }
            }

        }


        if (angularSpeed.magnitude > maxTurnSpeed * Mathf.Deg2Rad) { angularSpeed = angularSpeed.normalized * maxTurnSpeed * Mathf.Deg2Rad; }

        rigidbody.velocity = speed;
        rigidbody.angularVelocity = angularSpeed;


        accelerate = false;
        reverse = false;
        turnLeft = false;
        turnRight = false;
    }

    public float GetSpeed()
    {
        return rigidbody.velocity.magnitude;
    }


}
