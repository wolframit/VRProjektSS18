﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using DG.Tweening;

[ExecuteInEditMode]
public class ChangeParticleColor : MonoBehaviour
{
    public ChangeType Type;
    public VRTK_Lever Lever;

    public bool IsActive;

    public Action<ChangeParticleColor,bool> Finished;

    public AnimationCurve ColorCurve;

    public Color TargetColor;

    private ParticleSystem _ps;

    public bool JustColor;

    // these lists are used to contain the particles which match
    // the trigger conditions each frame.
    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

    private Color _color;

    private void Start()
    {
        _ps = GetComponent<ParticleSystem>();

        if (Type == ChangeType.Lever)
        {
            Lever.ValueChanged += Lever_ValueChanged;
            _color = new Color(1, 0, 0,0.6f);
        }
    }

    private void Lever_ValueChanged(object sender, Control3DEventArgs e)
    {
        float value = 1 - e.normalizedValue / 100;

        float animValue = ColorCurve.Evaluate(1 - value);

        //_color = new Color(value, Mathf.Clamp01(animValue), 0, 0.6f);

        _color = Color.Lerp(Color.red, TargetColor, Mathf.Clamp01(animValue));

        var mm = _ps.main;

        

        mm.startColor = _color;

        if (1 - value > 0.90f)
        {
            Finished(this,true);
            IsActive = true;
        }
        else
        {
            Finished(this, false);
            IsActive = false;
        }
    }

    void OnParticleTrigger()
    {
        if (Type == ChangeType.Lever)
            return;

        if(Type == ChangeType.Collider)
        {
            // get the particles which matched the trigger conditions this frame
            int numEnter = _ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

            if(numEnter > 0 && !JustColor)
            {
                if (enter[0].GetCurrentColor(_ps) == TargetColor)
                {
                    Finished(this, true);
                }
                else Finished(this, false);
            }

            // iterate through the particles which exited the trigger and make them green
            for (int i = 0; i < numEnter; i++)
            {
                ParticleSystem.Particle p = enter[i];
                p.startColor = TargetColor;
                enter[i] = p;
            }

            // re-assign the modified particles back into the particle system
            _ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        }     
    }


    public enum ChangeType
    {
        Collider,
        Receiver,
        Lever
    }

}