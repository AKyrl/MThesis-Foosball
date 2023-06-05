using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPosition : MonoBehaviour
{
    float minX = -5.81f;
    float maxX = 5.81f;
    float minY = -3.54f;
    float maxY = 3.08f;
    
    public void SetPosition(float x, float y) {
        float realX = x * (maxX - minX) + minX;
        float realY = y * (maxY - minY) + minY;
        transform.localPosition = new Vector3(realX, realY, transform.localPosition.z);
    }
}
