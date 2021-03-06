﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

public class LeverRemote : MonoBehaviour {

    public ControlledObject ControlledObj;
    
    public Action ObjFinishedCallBack;

    private VRTK_Control_UnityEvents controlEvents;

    public float maxLeverAngle = -120;

    private ReferenceManager _refs;

    private void Start()
    {
        if(GetComponent<VRTK_Lever>() == null)
        {
            VRTK_Lever lever = gameObject.AddComponent<VRTK_Lever>();
            lever.direction = VRTK_Lever.LeverDirection.y;
            lever.minAngle = 0;
            lever.maxAngle = maxLeverAngle;
            lever.stepSize = 0.5f;
            lever.grabbedFriction = 40;
            lever.connectedTo = transform.parent.parent.gameObject; 
        }
        GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += LeverRemote_InteractableObjectGrabbed;   ;
        GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += LeverRemote_InteractableObjectUngrabbed;
        GetComponent<VRTK_Lever>().ValueChanged += LeverRemote_ValueChanged;

        _refs = FindObjectOfType<ReferenceManager>();

        Debug.Log("Remote:Start()");
    }

    private void LeverRemote_InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
    {
        if (ControlledObj != null)
        {
            if (e.interactingObject == _refs.RightController.actual.transform.GetChild(0).gameObject)
            {
                ControlledObj.Controller = _refs.RightController;
            }
            else ControlledObj.Controller  = _refs.LeftController;
        }
            
    }

    private void LeverRemote_InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        if (ControlledObj.IsControlledObjectOnTarget())
        {
            GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed -= LeverRemote_InteractableObjectUngrabbed;
            GetComponent<VRTK_Lever>().ValueChanged -= LeverRemote_ValueChanged;
            this.enabled = false;

            ObjFinishedCallBack();
        }
    }

    public void SetNewControlledObj(ControlledObject obj)
    {
        ControlledObj = obj;

        float value = GetComponent<VRTK_Lever>().maxAngle;

        if(((ControlledBlock)obj).Axis == VRTK_Control.Direction.y)
        {
            if(((ControlledBlock)obj).StartPos.y == ((ControlledBlock)obj).Min)
            {
                value = GetComponent<VRTK_Lever>().maxAngle;
            }
        }
        else if (((ControlledBlock)obj).Axis == VRTK_Control.Direction.x)
        {
            if (((ControlledBlock)obj).StartPos.x == ((ControlledBlock)obj).Min)
            {
                value = GetComponent<VRTK_Lever>().maxAngle;
            }
        }


        GetComponent<VRTK_Lever>().SnapToValue(value);

        GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += LeverRemote_InteractableObjectUngrabbed;
        GetComponent<VRTK_Lever>().ValueChanged += LeverRemote_ValueChanged;

    }

    private void LeverRemote_ValueChanged(object sender, Control3DEventArgs e)
    {
        ControlledObj.MoveToNormedPos((e.normalizedValue) / 100);

        
    }

}
