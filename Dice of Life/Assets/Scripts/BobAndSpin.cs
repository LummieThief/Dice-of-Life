using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobAndSpin : MonoBehaviour
{
    public float bobHeight = 1f;
    public float bobSpeed;
    public float spinSpeed;
    private float startingY;
    private float t;
	// Update is called once per frame

	void Awake()
	{
        startingY = transform.position.y;
	}
	void Update()
    {
        if (MainCanvas.instance.paused)
            return;
        t += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, startingY + (1 + Mathf.Sin(t * bobSpeed)) * bobHeight, transform.position.z);
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
    }
}
