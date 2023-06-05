using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System.Linq;
using static UnityEngine.Networking.UnityWebRequest;

public class Rewards_FP : MonoBehaviour
{
    
    public GameObject PassAgent;
    Pass_Agent passAg;

    public GameObject ReceiveAgent;
    Receive_Agent recAg;

    public GameObject EnemyRod;
    Enemy_Rod enemyRod;

    BallController BallController;

    public GameObject PlayerPass;
    public GameObject PlayerRec1;
    public GameObject PlayerRec2;
    bool start = true;
    bool trigger = true;
    bool target = true;
    bool end = false;
    int count = 0;
    public GameObject Trigger;
    public GameObject Target;
    public GameObject Lane;

    BallController.Zones currentZone;
    readonly BallController.Zones minZone = BallController.Zones.Zone3;
    readonly BallController.Zones maxZone = BallController.Zones.Zone5;

    float temp_pen;

    string results = @"C:\Users\Alex\Downloads\ml-agents-release_19\results\Results_FP_Discrete_2.txt";
    public bool testing;
    public int Maxcounter;
    bool success = false;
    int counter = 0;
    int successful = 0;
    float time = 0;
    List<float> successfulltiem = new(); 

    Settings settings;
    void Start() {
        start = true;
        trigger = true;
        target = true;
        end = false;
        count = 0;
        BallController = this.GetComponent<BallController>();
        passAg = PassAgent.GetComponent<Pass_Agent>(); 
        recAg = ReceiveAgent.GetComponent<Receive_Agent>();
        enemyRod = EnemyRod.GetComponent<Enemy_Rod>();
        settings = GameObject.Find("Settings").GetComponent<Settings>();

        temp_pen = (passAg.MaxStep == 0) ? 0 : (-20f / passAg.MaxStep);
    }

    // Update is called once per frame
    void Update()
    {
        currentZone = BallController.zone;
        if (currentZone < minZone)
        {
            passAg.AddReward(-20f);
            passAg.EndEpisode();
            recAg.EndEpisode();
        }
        if (currentZone > maxZone)
        {
            passAg.EndEpisode();
            recAg.AddReward(-40f);
            recAg.EndEpisode();
        }

        passAg.AddReward(temp_pen);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (start)
        {
            if (collision.collider.name == PlayerPass.name)
            {
                passAg.AddReward(5f);
                //Debug.Log("Collided with player 1");
                start = false;
            }
        }

        if (!end) 
        { 

            if (collision.collider.name == PlayerRec1.name)
            {
                passAg.AddReward(20f);
                recAg.AddReward(15f);
                //Debug.Log("Collided with player 2");
                end = true;
                success = true;
            }
            if (collision.collider.name == PlayerRec2.name)
            {
                recAg.AddReward(10f);
                //Debug.Log("Collided with player 2");
                end = true;
                success = true;
            }
        }

        
        //check if colision with any of the children of the enemy rod
        if( enemyRod.GetComponentsInChildren<Collider>().Contains(collision.collider) )
        {
            passAg.AddReward(-10f);
            passAg.EndEpisode();
            recAg.EndEpisode();
            //Debug.Log("Collided with enemy");
        }
        
    }

    //print on trigger enter with another object
    private void OnTriggerEnter(Collider other)
    {
        if (trigger)
        {
            if (other.gameObject.name == Trigger.name)
            {
                passAg.AddReward(10f);
                //Debug.Log("Passed through trigger");
                trigger = false;
            }
        }
        else
        {
            if (other.gameObject.name == Trigger.name)
            {
                recAg.AddReward(-30f);
                passAg.EndEpisode();
                recAg.EndEpisode();
                //Debug.Log("Passed through trigger");
            }
        }

        if (target)
        {
            if (other.gameObject.name == Target.name)
            {
                passAg.AddReward(5f);
                success = true;
                //Debug.Log("Passed through Goal");
            }
        }


        if(other.gameObject.name == Lane.name)
        {
            passAg.AddReward(-10f);
            recAg.AddReward(-5f);
            passAg.EndEpisode();
            recAg.EndEpisode();
            //Debug.Log("Passed through Lane");
        }


    }

    private void OnTriggerStay(Collider other)
    {

        if (end)
        {
            if (other.gameObject.name == Target.name)
            {
                recAg.AddReward(0.02f);
                count++;
            }
            if (count > 300)
            {
                recAg.AddReward(20f);
                success = true;
                passAg.EndEpisode();
                recAg.EndEpisode();
            }
        }
        
    }
     public void EpisodeReset(StartPosition StartPosition)
    {
        BallController.BallReset(StartPosition);
        enemyRod.SetEnemy((Enemy_Rod.EnemyType)Academy.Instance.EnvironmentParameters.GetWithDefault("enemy", (int)settings.EnemyType));
        start = true;
        trigger = true;
        target = true;
        end = false;
        count = 0;

        if (testing)
        {
            if (counter % 5 == 0)
            {
                Debug.Log(Maxcounter - counter);
            }
            float diff = Time.time - time;
            time = Time.time;
            if (success)
            {
                success = false;
                successful++;
                successfulltiem.Add(diff);

            }
            if (counter > Maxcounter)
            {
                Debug.Log("Done with enemy" + settings.EnemyType);
                Debug.Log("Successful: " + successful + " out of " + Maxcounter);
                Debug.Log("Average Time: " + successfulltiem.Average());
                
                System.IO.File.AppendAllText(results, "Done with enemy" + settings.EnemyType + "\n");
                System.IO.File.AppendAllText(results, "Successful: " + successful + " out of " + Maxcounter + "\n");
                System.IO.File.AppendAllText(results, "Average Time: " + successfulltiem.Average() + "\n\n");
                counter = 0;
                successful = 0;
                successfulltiem.Clear();
                settings.EnemyType++;
            }
            counter++;
        }
    }
}