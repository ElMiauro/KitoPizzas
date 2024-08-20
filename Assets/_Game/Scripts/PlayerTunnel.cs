using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTunnel : MonoBehaviour
{
    public float coolDown = 1f;
    public float upTime = 0.5f;
    public float downTime = 1f;

    public GameObject pivot;
    public float maxSideRotation = 90f;

    private float currTimer = 0f;

    public enum State
    {
        idle,
        goingUp,
        up,
        goingDown
    }

    private State state = State.idle;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    void Update()
    {
        HandleInput();
        UpdateState();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Keypad6) && state != State.goingUp)
        {
            StartGoingUp(-maxSideRotation);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4) && state != State.goingUp)
        {
            StartGoingUp(maxSideRotation);
        }
    }

    private void StartGoingUp(float targetRot)
    {
        state = State.goingUp;
        currTimer = upTime;
        initialRotation = pivot.transform.rotation;
        targetRotation = Quaternion.Euler(0, 0, targetRot);
    }

    private void UpdateState()
    {
        currTimer -= Time.deltaTime;

        switch (state)
        {
            case State.goingUp:
                RotatePlayer(upTime, State.up);
                break;
            case State.up:
                StartCooldown();
                break;
            case State.idle:
                HandleCooldown();
                break;
            case State.goingDown:
                RotatePlayer(downTime, State.idle);
                break;
        }
    }

    void StartCooldown()
    {
        currTimer = coolDown;
        state = State.idle;
    }

    void HandleCooldown()
    {
        if (pivot.transform.eulerAngles.z == 0) return;
        if (currTimer > 0) return;

        state = State.goingDown;
        currTimer = downTime;
        initialRotation = pivot.transform.rotation;
        targetRotation = Quaternion.Euler(0, 0, 0);
    }

    private void RotatePlayer(float transitionTime, State nextState)
    {
        if (currTimer > 0)
        {
            float perc = 1 - (currTimer / transitionTime);
            pivot.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, perc);
        }
        else
        {
            pivot.transform.rotation = targetRotation;
            state = nextState;
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle < 0) angle += 360;
        return angle;
    }
}
