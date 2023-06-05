using System.Collections;
using System.Collections.Generic;
//using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

// toggling gravity could be used to replicate agent holding rod or not.


public class RodAgent : Agent
{
    public bool keyboardInput = true;
    float lastMousePosX;
    float lastMousePosY;
    //          rod    b2 poss    b5 poss        collider
    float min = 4.37f - (3.635513f + 1.288883f) + 0.2098976f;
    float max = 6.16f + 0.2098976f;
    public float speed = 1f;
    public float angularSpeed = 1f;
    float temp_pen;
    Rigidbody rb;
    public GameObject ball;
    BallController ballContr;

    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    
    
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        ballContr = ball.GetComponent<BallController>();
        // Setting a temporal penalty in case of a maximum number of steps.
        temp_pen = (MaxStep == 0) ? 0 : (-1f / MaxStep);


        lastMousePosX = Input.mousePosition.x;
        lastMousePosY = Input.mousePosition.y;        
    }


    

    public override void OnEpisodeBegin()
    {
        ballContr.SetNormalizedPosition(new Vector2(Random.Range(0.64f,0.74f), Random.Range(0f,1f)));
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        ResetRod();

        lastMousePosX = Input.mousePosition.x;
        lastMousePosY = Input.mousePosition.y;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(GetPlayerPosition(player1));
        sensor.AddObservation(GetPlayerPosition(player2));
        sensor.AddObservation(GetPlayerPosition(player3));
        sensor.AddObservation(GetRotation());
        sensor.AddObservation(ballContr.GetNormalizedPosition());   
        //Debug.Log(GetPlayerPosition(player1));
        //sensor.AddObservation(GetVelocity());
        //sensor.AddObservation(GetAngularVelocity());
        //sensor.AddObservation(ball.GetComponent<Rigidbody>().velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float tran = actions.ContinuousActions[0];
        float rot = actions.ContinuousActions[1];
        Translate(tran);
        Rotate(rot);
        AddReward(temp_pen);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        if (keyboardInput) { 
            continuousActions[0] = Input.GetAxisRaw("Vertical");
            continuousActions[1] = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            continuousActions[0] = mouseDeltaY();
            continuousActions[1] = mouseDeltaX();

            lastMousePosX =  Input.mousePosition.x;
            lastMousePosY =  Input.mousePosition.y;

        }
    }

    void Update() {
        Vector2 pos = ballContr.GetNormalizedPosition();
        float x = pos.x;
        float y = pos.y;
        float d1 = Mathf.Pow((GetPlayerPosition(player1) - y),2);
        float d2 = Mathf.Pow((GetPlayerPosition(player2) - y),2);
        float d3 = Mathf.Pow((GetPlayerPosition(player3) - y),2);
        float dx = Mathf.Pow((x - 0.68f),2);
        float min = Mathf.Min(d1,d2);
        if (x < 0.6f || x > 0.78)
        {
            AddReward(-1f);
            EndEpisode();
        }
        min = Mathf.Min(d3, min);
        AddReward(1/(min+dx));
    }

    float mouseDeltaX()
    {
        return Input.mousePosition.x - lastMousePosX;
    }
    float mouseDeltaY()
    {
        return Input.mousePosition.y - lastMousePosY;
    }

    float GetPlayerPosition(GameObject player)
    {
        return (transform.localPosition.y - (player.transform.position.z - player1.transform.position.z) - min) / (max - min);
    }

    void SetPosition(float y)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, (y * (6.16f - 4.37f) + 4.37f), transform.localPosition.z);
    }

    float GetRotation()
    {
        return (((transform.localEulerAngles.y + 180)/ 360) % 1);
    }

    void SetRotation(float r)
    {
        transform.localEulerAngles.Set(transform.transform.localEulerAngles.x,r*360,transform.transform.localEulerAngles.z);
    }

    void Translate(float t)
    {
        float move = (t * Time.deltaTime * speed) - rb.velocity.z;
        rb.AddForce(new Vector3(0f, 0f, move), ForceMode.VelocityChange);
    }

    void Rotate(float r)
    {
        float rot = (r * Time.deltaTime * angularSpeed);
        rb.AddRelativeTorque(new Vector3(0f, rot, 0f), ForceMode.VelocityChange);
    }

    void ResetRod()
    {
        SetPosition(Random.Range(0.1f, 0.9f));
        SetRotation(Random.Range(0f,1f));
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.collider.name == "Ball")
        {
            //AddReward(1f);
            //EndEpisode();
        }
    }
}
