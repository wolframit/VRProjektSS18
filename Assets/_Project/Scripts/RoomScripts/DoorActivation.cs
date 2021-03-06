﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class DoorActivation : MonoBehaviour {

    public string AnimationName;

    //public Collider BlockingCollider;

    public bool Ready = false;


    private bool _canAnimate = false;
    private Animator _animator;

    public delegate void DoorOpen();
    public DoorOpen doorOpening;

    public delegate void RoomFinished();
    public RoomFinished RoomHasFinished;

    private bool _isOpen = false;
    private bool _hasFinished;

    public bool OpenDoor = false;

	// Use this for initialization
	void Start () {

        _animator = GetComponentInParent<Animator>();

	}

    void Update()
    {
        if (OpenDoor)
        {
            OpenDoor = false;
            OpenDoorFromButton();
        }
    }

	public void Open()
	{
        if (!Ready)
            return;
		if (!_canAnimate)
			return;
		if (_isOpen)
			return;

        if (doorOpening != null)
        {
            doorOpening();
        }

        _canAnimate = false;
		_isOpen = true;

		_animator.SetTrigger("Open");
        //DisableBlockingCollider();

	}

    public void OpenDoorFromButton()
    {
        if (Ready)
            return;
        Ready = true;
        _canAnimate = true;
        Open();
    }

	public void Close()
	{
        if (!Ready)
            return;
        if (!_canAnimate)
            return;
        if (!_isOpen)
			return;

		_isOpen = false;
		_animator.SetTrigger("Close");

		_canAnimate = true;

	}


    private void OnTriggerEnter(Collider other)
    {
        if (_hasFinished)
            return;

        if (other.tag != "Player")
            return;

        if(RoomHasFinished != null)
            RoomHasFinished();

        _hasFinished = true;
    }

    private void OnTriggerExit(Collider other)
    {

			//Close ();

    }

    //public void DisableBlockingCollider()
    //{
    //    BlockingCollider.enabled = false;
    //}
}
