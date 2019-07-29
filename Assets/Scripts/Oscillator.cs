using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f,10f,10f);

    [Range(0, 1)] float movementFactor;

    [SerializeField] float period = 2f;

    Vector3 startingPos;

    void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {
        if (period <= Mathf.Epsilon) return;

            float cycles = Time.time / period; //grows continually from 0

            const float tau = Mathf.PI * 2; //about 6.28
            float rawSinWave = Mathf.Sin(cycles * tau); //goes from -1 to +1

            movementFactor = rawSinWave / 2f + 0.5f;
            Vector3 offset = movementFactor * movementVector;
            transform.position = startingPos + offset;    
    }
}
