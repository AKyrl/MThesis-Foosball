using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using UnityEngine.UIElements;

public class Settings : MonoBehaviour
{

    public ActionTypes FP_ActionType;
    public BehaviorType FP_BehaviorType;
    public float Speed;

    public Enemy_Rod.EnemyType EnemyType; 

    public enum ActionTypes
    {
        Continuous, // Continuous all 
        DCCM, // Discrete Choice Continuous Movement
        DCCS, // Discrete Choice Continuous Speed
        Discrete, // Discrete all
    }

     private void Awake()
    {
        //NNModel Model = Resources.Load<NNModel>("models/TickTack_Discrete-12999956");
         //= this.GetComponent<Unity.Barracuda.NNModel>();

        foreach (GameObject Agent in GameObject.FindGameObjectsWithTag("Agent"))
        {
            Agent.GetComponent<Pass_Agent>().SetBehaviorParameters(FP_ActionType);
            Agent.GetComponent<Pass_Agent>().SetBehaviorType(FP_BehaviorType);

            Agent.GetComponent<RodController>().SetSpeed(Speed);
        }

    }

}
