using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodController : MonoBehaviour
{
    float speed = 5000;
    public float angularSpeed = 1;
    public float min;
    public float max;
    float av; 
    Rigidbody rb;

    float Init_x, Init_y, Init_z;
    float Init_rot_x, Init_rot_y, Init_rot_z;
    public void SetSpeed(float s)
    {
        speed = s;
    }
    void Start() {
        rb = this.GetComponent<Rigidbody>();
        av = (max + min) / 2;
        Init_x = transform.localPosition.x;
        Init_y = transform.localPosition.y;
        Init_z = transform.localPosition.z;
        Init_rot_x = transform.localEulerAngles.x;
        Init_rot_y = transform.localEulerAngles.y;
        Init_rot_z = transform.localEulerAngles.z;
    }

    void Update() {

    }
    public float GetPosition_norm()
    {
        return Mathf.Clamp( (transform.localPosition.y - min) / (max - min), 0f, 1f);
    }

    public float GetPosition() {
        return Mathf.Clamp( 2*(transform.localPosition.y - av) / (max - min), -1f, 1f);
    }

    // Give this function value between 0 and 1
    public void SetPosition(float p) {
        transform.localPosition = new Vector3(Init_x, (p * (max - min)) + min, Init_z);
    }

    public float GetRotation() {
        return (((transform.localEulerAngles.y + 180)/ 360) % 1);
    }

    public void SetRotation(float r) {
        transform.localEulerAngles = new Vector3(Init_rot_x, r*360, Init_rot_z);
    }

    public void Move(float t, float r) {
        Translate(t);
        Rotate(r);
    }

    public void Translate(float t) 
    {
        float move = (t * Time.deltaTime * speed) - rb.velocity.z;
        rb.AddForce(new Vector3(0f, 0f, move), ForceMode.VelocityChange);
        if (t == 0)
        {
            rb.constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    public void Rotate(float r) 
    {
        float rot = (r * Time.deltaTime * angularSpeed);
        transform.Rotate(new Vector3(0f, rot, 0f));

        if (r == 0)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        //rb.AddRelativeTorque(new Vector3(0f, rot, 0f), ForceMode.VelocityChange);
    }

}