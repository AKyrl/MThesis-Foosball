using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Unity.Barracuda;
using System;

public class Pass_Agent : Agent
{
    public GameObject Ball;
    BallController BallController;

    Rewards_FP Rewards;

    public GameObject StartPos;
    StartPosition StartPosition;
    //public GameObject Trigger;
    //TargetPosition TriggerPosition;

    Settings.ActionTypes ActionType;
    BehaviorParameters Behaviors;

    public GameObject RodEnemy;
    RodController EnemyRodController;

    RodController thisRodController;

    
    // Start is called before the first frame update
    void Start()
    {
        BallController = Ball.GetComponent<BallController>();
        Rewards = Ball.GetComponent<Rewards_FP>();

        StartPosition = StartPos.GetComponent<StartPosition>();
        //TriggerPosition = Trigger.GetComponent<TargetPosition>();
        thisRodController = this.GetComponent<RodController>();

        EnemyRodController = RodEnemy.GetComponent<RodController>();


    }

    public override void OnEpisodeBegin()
    {
        thisRodController.SetPosition(0.8f);
        thisRodController.SetRotation(0.2f);

        Rewards.EpisodeReset(StartPosition);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // this rod observations
        sensor.AddObservation(thisRodController.GetPosition_norm());
        sensor.AddObservation(thisRodController.GetRotation());
        // Enemy rod observations
        sensor.AddObservation(EnemyRodController.GetPosition_norm());
        sensor.AddObservation(EnemyRodController.GetRotation());
        // Target rod observations
        //sensor.AddObservation(TargetRodController.GetPosition_norm());
        //sensor.AddObservation(TargetRodController.GetRotation());

        // Add the normalized Ball Position (x,y)
        //sensor.AddObservation(BallController.GetNormalizedPosition(this.transform.position.x));
        sensor.AddObservation(BallController.GetNormalizedPosition());
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float trans, rot;

        switch (ActionType)
        {
            case Settings.ActionTypes.Continuous:
                {
                    trans = actions.ContinuousActions[0];
                    rot = actions.ContinuousActions[1];

                    thisRodController.Move(trans, rot);
                    break;
                }
            case Settings.ActionTypes.DCCM:
                {
                    trans = actions.ContinuousActions[0];
                    rot = actions.ContinuousActions[1];

                    int stop = actions.DiscreteActions[0];

                    if (stop == 0)
                    {
                        thisRodController.Move(trans, rot);
                        if (BallController.zone != BallController.Zones.Zone3) // ball is not in the zone and we move 
                            AddReward(-0.001f);                        // negative reward
                        else                                           // ball is in the zone and we move
                            AddReward(0.0002f);                         // possitive reward
                    }
                    else
                    {
                        thisRodController.Move(0f, 0f);
                        if (BallController.zone != BallController.Zones.Zone3) // ball is not in the zone and we don't move
                            AddReward(0.001f);                         // possitive reward 
                        //else                                           // ball is in the zone and we don't move
                        //    AddReward(-0.003f);                        // negative reward
                    }

                    break;
                }
            case Settings.ActionTypes.DCCS:
                {
                    // 0 = stop, 1 = forward, 2 = backward
                    // Convert to 0, 1, -1
                    trans = actions.DiscreteActions[0] <= 1 ? actions.DiscreteActions[0] : -1;
                    rot = actions.DiscreteActions[1] <= 1 ? actions.DiscreteActions[1] : -1;

                    float speedtrans = actions.ContinuousActions[0];
                    float speedrot = actions.ContinuousActions[1];
                    speedtrans = Mathf.Clamp(speedtrans, 0f, 1f);
                    speedrot = Mathf.Clamp(speedrot, 0f, 1f);

                    thisRodController.Move(trans * speedtrans, rot * speedrot);
                    break;
                }
            case Settings.ActionTypes.Discrete:
                {
                    // 0 = stop, 1 = forward, 2 = backward
                    // Convert to 0, 1, -1
                    trans = actions.DiscreteActions[0] <= 1 ? actions.DiscreteActions[0] : -1;
                    rot = actions.DiscreteActions[1] <= 1 ? actions.DiscreteActions[1] : -1;
                    thisRodController.Move(trans, rot);
                    break;
                }
            default:
                break;
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        switch (ActionType)
        {
            case Settings.ActionTypes.Continuous:
                {
                    continuousActions[0] = Input.GetAxisRaw("Vertical");
                    continuousActions[1] = Input.GetAxisRaw("Horizontal");
                    break;
                }
            case Settings.ActionTypes.DCCM:
                {
                    continuousActions[0] = Input.GetAxisRaw("Vertical");
                    continuousActions[1] = Input.GetAxisRaw("Horizontal");
                    discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
                    break;
                }
            case Settings.ActionTypes.DCCS:
                {
                    int vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
                    int horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));

                    // from -1, 0, 1 
                    // convert to 0 = stop, 1 = forward, 2 = backward
                    discreteActions[0] = vertical >= 0 ? vertical : 2;
                    discreteActions[1] = horizontal >= 0 ? horizontal : 2;

                    continuousActions[0] = 1; // max speed
                    continuousActions[1] = 1;

                    break;
                }
            case Settings.ActionTypes.Discrete:
                {
                    int vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
                    int horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));

                    // from -1, 0, 1 
                    // convert to 0 = stop, 1 = forward, 2 = backward
                    discreteActions[0] = vertical >= 0 ? vertical : 2;
                    discreteActions[1] = horizontal >= 0 ? horizontal : 2;
                    break;
                }
            default:
                break;
        }
    }

    public void SetBehaviorParameters(Settings.ActionTypes type)
    {
        Behaviors = this.GetComponent<BehaviorParameters>();
        ActionType = type;
        
        switch (type)
        {
            case Settings.ActionTypes.Continuous:
                {
                    Behaviors.BehaviorName = "FP_Continuous";
                    Behaviors.BrainParameters.ActionSpec = new ActionSpec(2, null);
                    break;
                }
            case Settings.ActionTypes.DCCM:
                {
                    int[] discrete = { 2 };
                    Behaviors.BehaviorName = "FP_DCCM";
                    Behaviors.BrainParameters.ActionSpec = new ActionSpec(2, discrete);
                    break;
                }
            case Settings.ActionTypes.DCCS:
                {
                    int[] discrete = { 3, 3 };
                    Behaviors.BehaviorName = "FP_DCCS";
                    Behaviors.BrainParameters.ActionSpec = new ActionSpec(2, discrete);
                    break;
                }
            case Settings.ActionTypes.Discrete:
                {
                    int[] discrete = { 3, 3 };
                    Behaviors.BehaviorName = "FP_Discrete";
                    Behaviors.BrainParameters.ActionSpec = new ActionSpec(0, discrete);
                    break;
                }
            default:
                break;
        }
    }

    internal void SetBehaviorType(BehaviorType behaviorType)
    {
        Behaviors = this.GetComponent<BehaviorParameters>();

        Behaviors.BehaviorType = behaviorType;
    }
}
