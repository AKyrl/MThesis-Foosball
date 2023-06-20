using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public enum Zones
    {
        Zone1,
        Zone2,
        Zone3,
        Zone4,
        Zone5,
        Zone6
    }

    public float speed = 0.01f;
    float minX = -5.81f;
    float maxX = 5.81f;
    float minY = -3.54f;
    float maxY = 3.08f;
    float avY;
    Rigidbody rb;
    public Zones zone;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        avY = (maxY + minY) / 2;
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            rb.AddForce(Vector3.forward * speed, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            rb.AddForce(Vector3.back * speed, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            rb.AddForce(Vector3.left * speed, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            rb.AddForce(Vector3.right * speed, ForceMode.Impulse);
        }
    }

    void Update()
    {
        CheckZone();
    }

    // Scale position between 0 and 1
    public Vector2 GetNormalizedPosition()
    {
        float x = Mathf.Clamp((transform.localPosition.x - minX) / (maxX - minX), 0f, 1f);
        float y = Mathf.Clamp((transform.localPosition.y - minY) / (maxY - minY), 0f, 1f);
        return new Vector2(x, y);
    }

    public Vector2 GetNormalizedPosition(float rodX)
    {
        float rodNorm = transform.localPosition.x - rodX;
        float x = Mathf.Clamp(rodNorm / 3, -1, 1);
        //float y = Mathf.Clamp((transform.localPosition.y - minY) / (maxY - minY), 0, 1);
        float y = Mathf.Clamp(2*(transform.localPosition.y - avY)/ (maxY - minY), -1f, 1f);
        return new Vector2(x, y);
    }

    public void SetNormalizedPosition(Vector2 pos)
    {
        float x = pos.x * (maxX - minX) + minX;
        float y = pos.y * (maxY - minY) + minY;
        transform.localPosition = new Vector3(x, y, transform.localPosition.z);
    }

    public void SetVelocity(Vector3 v)
    {
        rb.velocity = v;
    }

    public void SetAngularVelocity(Vector3 v)
    {
        rb.angularVelocity = v;
    }

    public void AddForce(Vector3 direction)
    {
        rb.AddForce(direction * speed, ForceMode.Impulse);
    }

    void CheckZone()
    {
        float x = GetNormalizedPosition().x;
        if (x < 0.23)
            zone = Zones.Zone1;
        else if (x < 0.36)
            zone = Zones.Zone2;
        else if (x < 0.5)
            zone = Zones.Zone3;
        else if (x < 0.63)
            zone = Zones.Zone4;
        else if (x < 0.75)
            zone = Zones.Zone5;
        else
            zone = Zones.Zone6;
    }

    public void BallReset(StartPosition StartPosition)
    {
        SetVelocity(Vector3.zero);
        SetAngularVelocity(Vector3.zero);
        SetNormalizedPosition(StartPosition.GetRandomInStart());
        //Vector3 direction = new Vector3(500, 0f, (float)Random.Range(-150, 150));
        //float force = (float)Random.Range(0.15f, 0.85f);
        //rb.AddForce(direction * force);
    }
}
