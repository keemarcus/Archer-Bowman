using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	public GameObject player;

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	private float cameraZ = 0;
	private float horizMin = -22;
	private float horizMax = 14;
	private float vertMin = -16;
	private float vertMax = 10;


	private Camera mainCam;

	void Start()
	{
		cameraZ = transform.position.z;
		mainCam = GetComponent<Camera>();
	}

	void FixedUpdate()
	{
		if (player)
		{
			Vector3 playerPosition = player.transform.position;
			if(playerPosition.x < horizMin){
				playerPosition.x = horizMin;
			} else if(playerPosition.x > horizMax){
				playerPosition.x = horizMax;
			}
			if(playerPosition.y < vertMin){
				playerPosition.y = vertMin;
			} else if(playerPosition.y > vertMax){
				playerPosition.y = vertMax;
			}
			Vector3 delta = playerPosition - mainCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cameraZ));
			Vector3 destination = transform.position + delta;
			destination.z = cameraZ;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
	}
}
