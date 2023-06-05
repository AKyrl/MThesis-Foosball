using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Receive_Agent : Agent
{
    public GameObject Ball;
    BallController BallController;

    RodController thisRodController;

    // Start is called before the first frame update
    void Start()
    {
        BallController = Ball.GetComponent<BallController>();
        thisRodController = this.GetComponent<RodController>();
    }

    public override void OnEpisodeBegin()
    {
        thisRodController.SetPosition(Random.Range(0f, 1f));
        thisRodController.SetRotation(Random.Range(-0.2f, -0.1f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // this rod observations
        sensor.AddObservation(thisRodController.GetPosition_norm());
        sensor.AddObservation(thisRodController.GetRotation());
        //// Enemy rod observations
        //sensor.AddObservation(EnemyRodController.GetPosition());
        //sensor.AddObservation(EnemyRodController.GetRotation());
        // Target rod observations
        //sensor.AddObservation(PassRodController.GetPosition_norm());
        //sensor.AddObservation(PassRodController.GetRotation());

        // Add the normalized Ball Position (x,y)
        //sensor.AddObservation(BallController.GetNormalizedPosition(this.transform.position.x));
        sensor.AddObservation(BallController.GetNormalizedPosition());
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float trans, rot;

        // 0 = stop, 1 = forward, 2 = backward
        // Convert to 0, 1, -1
        trans = actions.DiscreteActions[0] <= 1 ? actions.DiscreteActions[0] : -1;
        rot = actions.DiscreteActions[1] <= 1 ? actions.DiscreteActions[1] : -1;
        thisRodController.Move(trans, rot);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        int vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        int horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));

        // from -1, 0, 1 
        // convert to 0 = stop, 1 = forward, 2 = backward
        discreteActions[0] = vertical >= 0 ? vertical : 2;
        discreteActions[1] = horizontal >= 0 ? horizontal : 2;

    }

}
