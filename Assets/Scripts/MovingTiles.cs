using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovingTiles : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;      
    public float minY = 0f;       
    public float maxY = 5f;       

    private bool movingUp = true;

    void Update()
    {
        Vector3 pos = transform.position;

        if (movingUp)
        {
            pos.y += speed * Time.deltaTime;
            if (pos.y >= maxY)
            {
                pos.y = maxY;
                movingUp = false;
            }
        }
        else
        {
            pos.y -= speed * Time.deltaTime;
            if (pos.y <= minY)
            {
                pos.y = minY;
                movingUp = true;
            }
        }

        transform.position = pos;
    }
}
