using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool RotateCW { get; private set; }
    public bool RotateCCW { get; private set; }
    public bool HardDrop { get; private set; }
    public bool Hold { get; private set; }

    private GameInput input;



    private void Awake()
    {
        input = new GameInput();

        input.Gameplay.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        input.Gameplay.Move.canceled += ctx => MoveInput = Vector2.zero;

        input.Gameplay.RotateCW.performed += ctx => RotateCW = true;

        input.Gameplay.RotateCCW.performed += ctx => RotateCCW = true;

        input.Gameplay.HardDrop.performed += ctx => HardDrop = true;

        input.Gameplay.Hold.performed += ctx => Hold = true;

    }

    private void OnEnable() => input.Gameplay.Enable();
    private void OnDisable() => input.Gameplay.Disable();


    // Update is called once per frame
    void Update()
    {
        RotateCW = false;
        RotateCCW = false;
        HardDrop = false;
        Hold = false;
    }


}
