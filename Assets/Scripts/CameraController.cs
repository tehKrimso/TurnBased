using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private float moveSpeed = 10f;
	
	[SerializeField]
	private float rotationSpeed = 100f;
	
	[SerializeField]
	private float zoomAmount = 1f;
	
	[SerializeField]
	private float zoomSpeed = 150f;
	
	private Vector3 followOffset;
	
	private const float MIN_FOLLOW_Y_OFFSET = 2f;
	private const float MAX_FOLLOW_Y_OFFSET = 12f;
	
	[SerializeField]
	private CinemachineVirtualCamera cinemachineVirtualCamera;
	private CinemachineTransposer cinemachineTransposer;
	
	private void Start()
	{
		cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
		followOffset = cinemachineTransposer.m_FollowOffset;
	}
	
	private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 inputMoveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.z = 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.z = -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = 1f;
        }

        Vector3 moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector.y = 1f;
        }

        if (Input.GetKey(KeyCode.E))
        {
            rotationVector.y = -1f;
        }

        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset.y -= zoomAmount;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset.y += zoomAmount;
        }

        followOffset.y = Mathf.Clamp(followOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, followOffset, zoomSpeed * Time.deltaTime);
    }
}
