using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class NewBehaviourScript : MonoBehaviour
{

    GameObject b2;
    Vector3 p;
    Random rnd;
    // Start is called before the first frame update
    void Start()
    {
      b2 = GameObject.Find ("Black_2_Man_Rod");
      b2.transform.position = new Vector3((float)-0.375002, (float)0.6, (float)0.888);
      rnd = new Random();
      Update();
    }

    // Update is called once per frame
    void Update()
    {
        var y = (float)(rnd.NextDouble()*0.43+0.37);
        b2.transform.position = new Vector3(b2.transform.position.x, y, b2.transform.position.z);
    }
}
