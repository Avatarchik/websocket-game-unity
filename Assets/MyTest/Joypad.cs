using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joypad : MonoBehaviour
{
    public bool testAutoPlay;
    public enum TestAutoPlayState
    {
        None,

        Idle,
        Move,
    }
    TestAutoPlayState testAutoPlayState = TestAutoPlayState.Idle;
    TestAutoPlayState testAutoPlayStateLast = TestAutoPlayState.None;

    bool testAutoPlayTurnLeft = false;
    bool testAutoPlayTurnRight = false;

    float testAutoPlayTimer = 0f;
    float testAutoPlayIdleToMoveWaitSeconds = 0f;
    float testAutoPlayMoveToIdleWaitSeconds = 0f;

    public OnPointerEvent onPointerEventW;
    public OnPointerEvent onPointerEventS;
    public OnPointerEvent onPointerEventA;
    public OnPointerEvent onPointerEventD;
    public OnPointerEvent onPointerEventQ;
    public OnPointerEvent onPointerEventE;

    void UpdateAutoPlayState()
    {
        bool stateDiff = false;
        if (testAutoPlayState != testAutoPlayStateLast)
        {
            stateDiff = true;
            testAutoPlayStateLast = testAutoPlayState;
        }

        switch (testAutoPlayState)
        {
            case TestAutoPlayState.None:
                {
                    testAutoPlayTurnLeft = false;
                    testAutoPlayTurnRight = false;

                    testAutoPlayTimer = 0f;
                    testAutoPlayIdleToMoveWaitSeconds = 0f;
                    testAutoPlayMoveToIdleWaitSeconds = 0f;
                    break;
                }

            case TestAutoPlayState.Idle:
                {
                    if (stateDiff)
                    {
                        testAutoPlayTimer = 0f;
                        testAutoPlayIdleToMoveWaitSeconds = Random.Range(3f, 6f);
                        break;
                    }

                    testAutoPlayTimer += Time.deltaTime;
                    if (testAutoPlayTimer < testAutoPlayIdleToMoveWaitSeconds)
                    {
                        // idle state should stay and do nothing
                        break;
                    }
                    else
                    {
                        testAutoPlayState = TestAutoPlayState.Move;
                        break;
                    }
                }

            case TestAutoPlayState.Move:
                {
                    if (stateDiff)
                    {
                        testAutoPlayTimer = 0f;
                        testAutoPlayMoveToIdleWaitSeconds = Random.Range(3f, 6f);

                        float rndF = 10000f;
                        testAutoPlayTurnLeft = Random.Range(0f, rndF) > rndF * 0.5f ? true : false;
                        testAutoPlayTurnRight = Random.Range(0f, rndF) > rndF * 0.5f ? true : false;

                        if (testAutoPlayTurnLeft && testAutoPlayTurnRight)
                        {
                            testAutoPlayTurnLeft = testAutoPlayTurnRight = false;
                        }
                        break;
                    }

                    testAutoPlayTimer += Time.deltaTime;
                    if (testAutoPlayTimer < testAutoPlayMoveToIdleWaitSeconds)
                    {
                        // move
                        onPointerEventW.OnPointerPress(null);
                        if (testAutoPlayTurnLeft)
                        {
                            onPointerEventQ.OnPointerPress(null);
                        }
                        if (testAutoPlayTurnRight)
                        {
                            onPointerEventE.OnPointerPress(null);
                        }
                        break;
                    }
                    else
                    {
                        testAutoPlayState = TestAutoPlayState.Idle;
                        break;
                    }
                }
        }
    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            onPointerEventW.OnPointerPress(null);
        }
        if (Input.GetKey(KeyCode.S))
        {
            onPointerEventS.OnPointerPress(null);
        }
        if (Input.GetKey(KeyCode.A))
        {
            onPointerEventA.OnPointerPress(null);
        }
        if (Input.GetKey(KeyCode.D))
        {
            onPointerEventD.OnPointerPress(null);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            onPointerEventQ.OnPointerPress(null);
        }
        if (Input.GetKey(KeyCode.E))
        {
            onPointerEventE.OnPointerPress(null);
        }

        if (testAutoPlay)
        {
            UpdateAutoPlayState();
        }
    }
}
