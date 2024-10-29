using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
	private int collisionCount = 0;
	private RagdollController ragdoll;

	private bool flyorfall = true; //true is flytoground false is falltoground

	private float expectedtime = 3.5f;
	private float standingtime = 0f;

	private void Start()
	{
		ragdoll = GameManager.Instance.ragdollController;
	}
	private void OnCollisionEnter(Collision collision)
	{
		collisionCount++;
		
		standingtime = 0f;
		if (ragdoll.currentState != RagdollController.PlayerState.Falling)
		{
			//ragdoll.currentState = RagdollController.PlayerState.Onground;
			ragdoll.FlytoGround();
			flyorfall = true;
		}
		else
		{
			flyorfall = false;
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (ragdoll.currentState == RagdollController.PlayerState.Falling) //falling to ground
		{
			standingtime += Time.fixedDeltaTime;
			if (standingtime > expectedtime && !flyorfall)
			{
				ragdoll.OnGroundBehaviour();
				flyorfall = true;
			}
		}
        else
        {
			ragdoll.FlytoGround();
        }
	}

	private void OnCollisionExit(Collision collision)
	{
		collisionCount--;

		if (collisionCount <= 0)
		{
			collisionCount = 0;
			standingtime = 0f;
			if (ragdoll.currentState != RagdollController.PlayerState.Falling)
			{
				ragdoll.currentState = RagdollController.PlayerState.Flying;
				ragdoll.FlyingBehaviour();
			}
		}
	}
}
