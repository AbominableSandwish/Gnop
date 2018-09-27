using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour {
    public float backgroundSize;
    public float ParallaxSpeed;

    private Transform CameraTransform;
    private Transform[] layers;
    private float viewZone = 10;
    private int LeftIndex;
    private int RightIndex;
    private float LastCameraX;

    private float LastCameraY;

    float deltaX = 0.02f;
    // Use this for initialization
    void Start()
    {
        CameraTransform = Camera.main.transform;
        LastCameraX = CameraTransform.position.x;
        LastCameraY = CameraTransform.position.y;
        layers = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
        }
        LeftIndex = 0;
        RightIndex = layers.Length - 1;

    }

    float move_X;
    private void FixedUpdate()
    {
        //X
       
        if (move_X < -20)
        {
            deltaX = 0.02f;
        }

        if(move_X > 20){
            deltaX = -0.02f;
        }
        move_X += deltaX;
       // Debug.Log(move_X);
        transform.position += Vector3.right * (deltaX * ParallaxSpeed);
        float i = 0;
        foreach (Transform layer in layers)
        {
            i += 0.2f;
            layer.position += Vector3.right * (deltaX * ParallaxSpeed) * 1 / i;
        }
        LastCameraX = CameraTransform.position.x;

        //Y
        float deltaY = CameraTransform.position.y - LastCameraY;
        transform.position += Vector3.up * (deltaY * ParallaxSpeed);
        float j = 0;
        foreach (Transform layer in layers)
        {
            j += 0.3f;
            layer.position += Vector3.up * (deltaY * ParallaxSpeed) * 0.3f / j;
        }
        LastCameraY = CameraTransform.position.y;


        if (CameraTransform.position.x < (layers[LeftIndex].transform.position.x + viewZone))
        {
            //   ScrollLeft();
        }

        if (CameraTransform.position.x < (layers[RightIndex].transform.position.x - viewZone))
        {
            //     ScroolRight();
        }
    }

    private void ScrollLeft()
    {
        int lastRight = RightIndex;
        layers[RightIndex].position = Vector3.right * (layers[LeftIndex].position.x - backgroundSize);
        LeftIndex = RightIndex;
        RightIndex--;
        if (RightIndex < 0)
            RightIndex = layers.Length - 1;

    }

    private void ScroolRight()
    {
        int lastRight = LeftIndex;
        layers[LeftIndex].position = Vector3.right * (layers[RightIndex].position.x - ++backgroundSize);
        RightIndex = LeftIndex;
        LeftIndex++;
        if (LeftIndex == layers.Length)
            LeftIndex = 0;
    }
}
