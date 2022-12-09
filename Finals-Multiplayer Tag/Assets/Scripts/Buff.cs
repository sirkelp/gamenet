using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public float rotationSpeed = 20f;
    public int buffCode = 1;

    void Update()
    {
        RotateIndefinitely();
    }

    void RotateIndefinitely()
    {
        float tic = 0;
        tic += Time.deltaTime * rotationSpeed;
        gameObject.transform.eulerAngles += new Vector3(0, tic, 0);
    } 
}
