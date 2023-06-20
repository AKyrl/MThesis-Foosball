using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Unity.Barracuda;

public class FoosBall_Agent : Agent
{
    
    // Penalty applied each timestep
    Settings.ActionTypes ActionType;

    public GameObject Ball;
    BallController BallController;
    Rewards_TT Rewards;

    public GameObject StartPos;
    StartPosition StartPosition;
    RodController RodController;

    BehaviorParameters Behaviors;

    // Start is called before the first frame update
    void Start()
    {
        
        RodController = this.GetComponent<RodController>();

        BallController = Ball.GetComponent<BallController>();
        Rewards = Ball.GetComponent<Rewards_TT>();

        StartPosition = StartPos.GetComponent<StartPosition>();

        
    }

    public override void OnEpisodeBegin()
    {
        ResetRods();
        Rewards.EpisodeReset(StartPosition);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(RodController.GetPosition_norm());
        sensor.AddObservation(RodController.GetRotation());
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
                    ;
                    RodController.Move(trans, rot);
                    break;
                }
            case Settings.ActionTypes.DCCM:
                {
                    trans = actions.ContinuousActions[0];
                    rot = actions.ContinuousActions[1];

                    int stop = actions.DiscreteActions[0];

                    if (stop == 0)
                    {
                        RodController.Move(trans, rot);
                        if (BallController.zone != BallController.Zones.Zone3) // ball is not in the zone and we move 
                            AddReward(-0.003f);                        // negative reward
                        else                                           // ball is in the zone and we move
                            AddReward(0.003f);                         // possitive reward
                    }
                    else
                    {
                        RodController.Move(0f, 0f);
                        if (BallController.zone != BallController.Zones.Zone3) // ball is not in the zone and we don't move
                            AddReward(0.003f);                         // possitive reward 
                        else                                           // ball is in the zone and we don't move
                            AddReward(-0.003f);                        // negative reward
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

                    RodController.Move(trans * speedtrans, rot * speedrot);
                    break;
                }
            case Settings.ActionTypes.Discrete:
                {
                    // 0 = stop, 1 = forward, 2 = backward
                    // Convert to 0, 1, -1
                    trans = actions.DiscreteActions[0] <= 1 ? actions.DiscreteActions[0] : -1;
                    rot = actions.DiscreteActions[1] <= 1 ? actions.DiscreteActions[1] : -1;
                    RodController.Move(trans, rot);
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

        switch(ActionType)
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

    void ResetRods()
    {
        RodController.SetPosition(0.5f);
        RodController.SetRotation(0.12f);
    }

    public void SetBehaviorParameters(Settings.ActionTypes type)
    {
        Behaviors = this.GetComponent<BehaviorParameters>();
        ActionType = type;

        switch (type)
        {
            case Settings.ActionTypes.Continuous:
                {
                    Behaviors.BehaviorName = "TickTack_Call";
                    Behaviors.BrainParameters.ActionSpec = new ActionSpec(2, null); 
                    break;
                }
            case Settings.ActionTypes.DCCM:
                {
                    int[] discrete = { 2 };
                    Behaviors.BehaviorName = "TickTack_DCCM";
                    Behaviors.BrainParameters.ActionSpec = new ActionSpec(2, discrete);
                    break;
                }
            case Settings.ActionTypes.DCCS:
                {
                    int[] discrete = { 3, 3 };
                    Behaviors.BehaviorName = "TickTack_DCCS";
                    Behaviors.BrainParameters.ActionSpec = new ActionSpec(2, discrete);
                    break;
                }
            case Settings.ActionTypes.Discrete:
                {
                    int[] discrete = { 3, 3 };
                    Behaviors.BehaviorName = "TickTack_Discrete";
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

