using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    private float screenHeight;
    private float screenWidth;

    private void Awake()
    {
        var cam = Camera.main;
        var screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
        var screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));
        screenWidth = screenTopRight.x - screenBottomLeft.x;
        screenHeight = screenTopRight.y - screenBottomLeft.y;
    }

    private void Update()
    {
        MovePlayer();
        WrapAround();
    }

    public void MovePlayer()
    {
        if (Input.GetKey(KeyCode.W))
            transform.position += new Vector3(0, 1, 0) * speed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.S))
            transform.position += new Vector3(0, -1, 0) * speed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.D))
            transform.position += new Vector3(1, 0, 0) * speed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.A))
            transform.position += new Vector3(-1, 0, 0) * speed * Time.deltaTime;
    }


    private void WrapAround()
    {
        Vector3 pos = transform.position;

        if (pos.x < -screenWidth / 2)
            pos = new Vector3(pos.x + screenWidth, pos.y, pos.z);
        else if (pos.x > screenWidth / 2)
            pos = new Vector3(pos.x - screenWidth, pos.y, pos.z);
        else if (pos.y < -screenHeight / 2)
            pos = new Vector3(pos.x, pos.y + screenHeight, pos.z);
        else if (pos.y > screenHeight / 2)
            pos = new Vector3(pos.x, pos.y - screenHeight, pos.z);

        transform.position = pos;
    }
}
