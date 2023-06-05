using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Unity.Barracuda;


public class Enemy_Rod : MonoBehaviour
{
    public enum EnemyType
    {
        Non_factor,
        Corner,
        Static,
        Rythmic,
        Random
    }

    EnemyType enemy;
    bool rythmic = false;
    bool random = false;

    RodController EnemyRodController;
    Settings settings;
    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        EnemyRodController = GetComponent<RodController>();
        settings = GameObject.Find("Settings").GetComponent<Settings>();
        SetEnemy((EnemyType)Academy.Instance.EnvironmentParameters.GetWithDefault("enemy", (int)settings.EnemyType));
    }
    // Update is called once per frame
    void Update()
    {
            if (rythmic)
            {
                EnemyRodController.SetPosition(Mathf.PingPong(Time.time*0.7f, 1f));
            }
            if (random)
            {
                count++;
                if (count > 20)
                {
                    EnemyRodController.Translate(Random.Range(-0.1f, 0.1f));
                    count = 0;
                }
                
            }

    }

    public void SetEnemy(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Non_factor:
                {
                    EnemyRodController.SetPosition(Random.Range(0f, 1f));
                    EnemyRodController.SetRotation(Random.Range(0.25f, 0.75f));
                    rythmic = false;
                    random = false;
                    break;
                }
            case EnemyType.Corner:
                {
                //    EnemyRodController.SetPosition(Random.Range(0.9f, 1f));
                //    EnemyRodController.SetRotation(Random.Range(-0.1f, 0.1f));
                    EnemyRodController.SetPosition(Random.Range(0.7f, 1f));
                    EnemyRodController.SetRotation(Random.Range(-0.25f, 0.25f));
                    rythmic = false;
                    random = false;
                    break;
                }
            case EnemyType.Static:
                {
                    EnemyRodController.SetPosition(Random.Range(0f, 1f));
                    EnemyRodController.SetRotation(Random.Range(-0.2f, 0.2f));
                    rythmic = false;
                    random = false;
                    break;
                }
            case EnemyType.Rythmic:
                {
                    EnemyRodController.SetPosition(Random.Range(0f, 1f));
                    EnemyRodController.SetRotation(0f);
                    rythmic = true;
                    random = false;
                    break;
                }
            case EnemyType.Random:
                {
                    EnemyRodController.SetPosition(Random.Range(0f, 1f));
                    EnemyRodController.SetRotation(0f);
                    rythmic = false;
                    random = true;
                    break;
                }
            default:
                {
                    EnemyRodController.SetPosition(Random.Range(0f, 1f));
                    EnemyRodController.SetRotation(0f);
                    rythmic = false;
                    random = true;
                    break;
                }

        }
    }
}
