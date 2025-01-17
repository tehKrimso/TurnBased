using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraController : MonoBehaviour
{
	public static CameraController Instance {get; private set;}
	
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
	private const float MAX_FOLLOW_Y_OFFSET = 15f;
	
	[SerializeField]
	private CinemachineVirtualCamera cinemachineVirtualCamera;
	private CinemachineTransposer cinemachineTransposer;
	
	private void Awake()
	{
		if(Instance != null)
		{
			Debug.LogError("There's more than one CameraController! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		
		Instance = this;
	}
	
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
		Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();

		Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
		transform.position += moveVector * moveSpeed * Time.deltaTime;
	}

	private void HandleRotation()
	{
		Vector3 rotationVector = new Vector3(0, 0, 0);

		rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

		transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
	}

	private void HandleZoom()
	{
		followOffset.y += zoomAmount * InputManager.Instance.GetCameraZoomAmount();

		followOffset.y = Mathf.Clamp(followOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
		cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, followOffset, zoomSpeed * Time.deltaTime);
	}
	
	public float GetCameraHeight()
	{
		return followOffset.y;
	}
}
