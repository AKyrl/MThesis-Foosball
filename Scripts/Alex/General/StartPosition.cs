using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPosition : MonoBehaviour
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

    public (float, float, float, float) GetEdges()
    {
        Vector3 size = this.GetComponent<Renderer>().bounds.size;

        float xMin = NormalizeX(transform.localPosition.x - size.x / 2) ;
        float xMax = NormalizeX(transform.localPosition.x + size.x / 2) ;
        float zMin = NormalizeY(transform.localPosition.y - size.z / 2) ;
        float zMax = NormalizeY(transform.localPosition.y + size.z / 2) ;
        return (xMin, xMax, zMin, zMax);

    }

    public Vector2 GetRandomInStart()
    {
        float xMin, xMax, zMin, zMax;
        (xMin, xMax, zMin, zMax) = GetEdges();

        return new Vector2(Random.Range(xMin, xMax), Random.Range(zMin, zMax));
    }

    float NormalizeX(float num)
    {
        return Mathf.Clamp((num - minX) / (maxX - minX), 0, 1);
    }
    float NormalizeY(float num)
    {
        return Mathf.Clamp((num - minY) / (maxY - minY), 0, 1);
    }
}
