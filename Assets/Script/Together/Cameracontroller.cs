using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Cameracontroller : MonoBehaviour
{
	public CinemachineVirtualCamera MainCam;
	public CinemachineVirtualCamera FlyCam;
	public CinemachineVirtualCamera FallingCam;
	public CinemachineVirtualCamera EndingCam;
	//flying ¿œ∂ß ªÏ¬¶ ¡‹æ∆øÙ, Falling ¿œ∂ß ¡‹¿Œ, Ongroundø°º≠ ø¯∑° ªÛ≈¬∑Œ.
	public void SetCameraMain()	
	{
		MainCam.Priority = 10;
		FlyCam.Priority = 0;
		FallingCam.Priority = 0;
		//MainCam.gameObject.SetActive(false);
		//FlyCam.gameObject.SetActive(false);
		//FallingCam.gameObject.SetActive(false);
	}
	public void SetCameraFly()
	{
		MainCam.Priority = 0;
		FlyCam.Priority = 10;
		FallingCam.Priority = 0;
		//MainCam.gameObject.SetActive(false);
		//FlyCam.gameObject.SetActive(true);
		//FallingCam.gameObject.SetActive(false);
	}
	public void SetCameraFalling()
	{
		MainCam.Priority = 0;
		FlyCam.Priority = 0;
		FallingCam.Priority = 10;
		//MainCam.gameObject.SetActive(false);
		//FlyCam.gameObject.SetActive(false);
		//FallingCam.gameObject.SetActive(true);
	}
	public void EndingCamera()
	{
		MainCam.Priority = 0;
		FlyCam.Priority = 0;
		FallingCam.Priority = 0;
		EndingCam.Priority = 10;
	}
}
