using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class heroAttackController : MonoBehaviour
{
    public Animator playerAnimator;
    private int step = 0;

    private bool canAcceptInput = false;
    private bool haveInput = false;

    private NewHeroController _heroController;
    private void Start()
    {
        _heroController = GetComponent<NewHeroController>();
        if (!_heroController)
        {
            Debug.LogError("hero controller not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_heroController.view != null && !_heroController.view.IsMine)
        {
            return;
        }
        // if (_heroController.currentState != HeroState.Attack)
        // {
        //     resetFlags();
        // }
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            string tempname = playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (!tempname.StartsWith("attack"))
            {
                resetFlags();
            }
            if (step!=0 && !canAcceptInput)
            {
                return;
            }
            haveInput = true;
            
            //TODO: check if the player is idle/run
            if (step == 0)
            {
                _heroController.enterAttackState();
                // if the player is idle or run, attack instantly. Otherwise play the attack when this animation is finished.
                this.nextStep();
            }
        }
    }

    public void nextStep()
    {
        if (step!=0 && !this.haveInput)
        {
            resetFlags();
            return;
        }
        
        this.canAcceptInput = false;
        this.haveInput = false;
        switch (step)
        {
            case 0:
                playerAnimator.Play("attack1");
                break;
            case 1:
                playerAnimator.Play("attack2");
                break;
            case 2:
                playerAnimator.Play("attack3");
                break;
            default:
                break;
        }
        step += 1;
    }

    public void startGetInput()
    {
        this.canAcceptInput = true;
        this.haveInput = false;
    }
    
    void resetFlags()
    {
        this.step = 0;
        this.canAcceptInput = false;
        this.haveInput = false;
    }
}
