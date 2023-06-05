using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Rewards_TT : MonoBehaviour
{
    public GameObject Agent;
    FoosBall_Agent fa;
    BallController controller;
    BallController.Zones currentZone;
    float temp_pen;
    public GameObject Player1;
    public GameObject Player2;
    bool previousPlayer = false;

    public bool testing;
    public int Maxcounter;
    int counter = 0;
    int lost = 0;
    int touches = 0;
    List<int> touchesList = new();
    public enum Dificulty
    {
        none,
        easy,
        medium,
        hard,
        ultra
    }
    public Dificulty difficulty;

    string results = @"C:\Users\Alex\Downloads\ml-agents-release_19\results\Results_TickTack_DCCS.txt";

    void Start() {
        previousPlayer = false;
        controller = this.GetComponent<BallController>();
        fa = Agent.GetComponent<FoosBall_Agent>();

        temp_pen = (fa.MaxStep == 0) ? 0 : (-10f / fa.MaxStep);

        if (testing)
        {
            counter = Maxcounter;
        }
    }

    private void Update()
    {
        if (!(currentZone == controller.zone))
        {
            fa.AddReward(-(5f));
            lost++;
            fa.EndEpisode();
        }
        fa.AddReward(temp_pen);

        if(testing)
        {
            switch(difficulty)
            {
                case Dificulty.none:
                    break; 
                case Dificulty.easy: //forward and back moves easy to keep in controll
                    {
                        controller.speed = 0.2f;
                        if (fa.StepCount == (int)(fa.MaxStep / 4) )
                            controller.AddForce(Vector3.forward);
                        else if(fa.StepCount == (int)(fa.MaxStep / 3) )
                            controller.AddForce(Vector3.back);
                        else if (fa.StepCount == (int)(fa.MaxStep / 2) )
                            controller.AddForce(Vector3.forward);
                        break;
                    }
                case Dificulty.medium: //left and right moves a bit harder 
                    {
                        controller.speed = 0.08f;
                        if (fa.StepCount == (int)(fa.MaxStep / 4))
                            controller.AddForce(Vector3.left);
                        else if (fa.StepCount == (int)(fa.MaxStep / 3))
                            controller.AddForce(Vector3.right);
                        else if (fa.StepCount == (int)(fa.MaxStep / 2))
                            controller.AddForce(Vector3.left);
                        break;
                    }
                case Dificulty.hard: // 10 times random left or right
                    {
                        controller.speed = 0.1f;
                        if (fa.StepCount % (int)(fa.MaxStep / 10) == 0)
                        {
                            int random = Random.Range(0, 2) * 2 - 1;
                            controller.AddForce( new Vector3(random, 0, 0));
                        }
                        break;
                    }
                case Dificulty.ultra: // 20 times random direction
                    {
                        controller.speed = 0.12f;
                        if (fa.StepCount % (int)(fa.MaxStep / 20) == 0)
                        {
                            int randomX = Random.Range(0, 2) * 2 - 1;
                            int randomZ = Random.Range(0, 2) * 2 - 1;
                            controller.AddForce(new Vector3(randomX, 0, randomZ));
                        }
                        break;
                    }
                default:
                    controller.speed = 0.01f;
                    break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (previousPlayer)//If true then it needs to touch player 2
        {
            if(collision.collider.name == Player2.name)
            {
                fa.AddReward(1);
                //Debug.Log("Colided with player 2");
                previousPlayer = false;
                touches++;
            }
        }
        else
        {
            if (collision.collider.name == Player1.name)
            {
                fa.AddReward(1);
                //Debug.Log("Colided with player 1");
                previousPlayer = true;
                touches++;
            }
        }
    }

    public void EpisodeReset(StartPosition StartPosition)
    {
        controller.BallReset(StartPosition);
        currentZone = controller.zone;
        previousPlayer = false;

        if(testing)
        {
            counter--;
            if(counter%5 == 0)
            {
                Debug.Log(counter);
            }
            touchesList.Add(touches);
            touches = 0;
            if (counter == 0)
            {
                Debug.Log("testing done! with difficulty: " + difficulty);
                Debug.Log("Times lost: " + lost);
                Debug.Log("Average Touches: " + touchesList.Average() );
                // print the same in a file
                System.IO.File.AppendAllText(results, "Testing with difficulty: " + difficulty + "\n");
                System.IO.File.AppendAllText(results, "Times lost: " + lost + "\n");
                System.IO.File.AppendAllText(results, "Average Touches: " + touchesList.Average() + "\n\n");
                difficulty++;
                counter = Maxcounter;
                lost = 0;
                touchesList.Clear();
            }
            
        }

    }
}
