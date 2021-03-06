﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public bool isFiring = false;
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public Transform raycastOrigin;
    public Transform raycastDest;

    public Transform Hook;
    private CharacterLocomotion aiming;
    public Transform hookOrigin;

    private GameObject hookedObj;
    Ray ray;
    public RaycastHit hitInfo;
    

    private void Start()
    {
        Hook.position = hookOrigin.position;
        aiming = GameObject.Find("Player").GetComponent<CharacterLocomotion>();
    }
    public void StartFiring()
    {
        isFiring = true;
        muzzleFlash.Emit(1);

        ray.origin = raycastOrigin.position ;
        ray.direction = raycastDest.position-raycastOrigin.position;

        if (Physics.Raycast(ray, out hitInfo))
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);
            Hook.transform.position = hitInfo.point;
            hookedObj = hitInfo.transform.gameObject;
            Hook.transform.parent = hookedObj.transform;
            aiming.SetHooked(true);
            isFiring = false;
        }
            
    }

    public void StopFiring()
    {
        aiming.SetHooked(false);
    }

    public void ReturnHook()
    {
        aiming.SetHooked(false);
        Hook.position = hookOrigin.position;
        Hook.transform.parent = gameObject.transform;
    }
}
