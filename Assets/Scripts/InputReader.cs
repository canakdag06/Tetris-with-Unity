using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance { get; private set; }

    public Vector2 MoveInput { get; private set; }
    public bool RotateCW { get; private set; }
    public bool RotateCCW { get; private set; }
    public bool HardDrop { get; private set; }
    public bool Hold { get; private set; }
    public GameInput InputActions => input;
    private GameInput input;




    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        input = new GameInput();
        input?.Gameplay.Enable();
        input.Gameplay.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        input.Gameplay.Move.canceled += ctx => MoveInput = Vector2.zero;

        input.Gameplay.RotateCW.performed += ctx => RotateCW = true;

        input.Gameplay.RotateCCW.performed += ctx => RotateCCW = true;

        input.Gameplay.HardDrop.performed += ctx => HardDrop = true;

        input.Gameplay.Hold.performed += ctx => Hold = true;

        SceneManager.activeSceneChanged += OnSceneChanged; // listen scene changes & load bindings
        InitializeBindings();
    }

    private void OnDisable() => input?.Gameplay.Disable();

    public void ResetInputs()
    {
        RotateCW = false;
        RotateCCW = false;
        HardDrop = false;
        Hold = false;
    }

    public void InitializeBindings()
    {
        foreach (var action in input.asset)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                string key = action.name + "_binding_" + i;
                if (PlayerPrefs.HasKey(key))
                {
                    string overridePath = PlayerPrefs.GetString(key);
                    action.ApplyBindingOverride(i, overridePath);
                }
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene _, Scene __)
    {
        InitializeBindings();
    }
}
