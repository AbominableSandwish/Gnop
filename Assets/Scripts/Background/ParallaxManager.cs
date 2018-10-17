using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour {
    public float backgroundSize;
    public float parallaxSpeed;

    private Transform cameraTransform;
    private Transform[] layers;
    private float viewZone = 10;
    private int leftIndex;
    private int rightIndex;
    private float lastCameraX;

    private float lastCameraY;

    float deltaX = 0.02f;
    // Use this for initialization
    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraX = cameraTransform.position.x;
        lastCameraY = cameraTransform.position.y;
        layers = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
        }
        leftIndex = 0;
        rightIndex = layers.Length - 1;

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
        transform.position += Vector3.right * (deltaX * parallaxSpeed);
        float i = 0;
        foreach (Transform layer in layers)
        {
            i += 0.2f;
            layer.position += Vector3.right * (deltaX * parallaxSpeed) * 1 / i;
        }
        lastCameraX = cameraTransform.position.x;

        //Y
        float deltaY = cameraTransform.position.y - lastCameraY;
        transform.position += Vector3.up * (deltaY * parallaxSpeed);
        float j = 0;
        foreach (Transform layer in layers)
        {
            j += 0.3f;
            layer.position += Vector3.up * (deltaY * parallaxSpeed) * 0.3f / j;
        }
        lastCameraY = cameraTransform.position.y;


        if (cameraTransform.position.x < (layers[leftIndex].transform.position.x + viewZone))
        {
            //   ScrollLeft();
        }

        if (cameraTransform.position.x < (layers[rightIndex].transform.position.x - viewZone))
        {
            //     ScroolRight();
        }
    }

    private void ScrollLeft()
    {
        int lastRight = rightIndex;
        layers[rightIndex].position = Vector3.right * (layers[leftIndex].position.x - backgroundSize);
        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0)
            rightIndex = layers.Length - 1;

    }

    private void ScroolRight()
    {
        int lastRight = leftIndex;
        layers[leftIndex].position = Vector3.right * (layers[rightIndex].position.x - ++backgroundSize);
        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == layers.Length)
            leftIndex = 0;
    }
}
