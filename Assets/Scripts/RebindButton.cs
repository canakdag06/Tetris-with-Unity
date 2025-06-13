using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindButton : MonoBehaviour
{
    public InputActionReference actionReference;
    public int bindingIndex = 0;
    public TMP_Text bindingText;

    private void OnEnable()
    {
        UpdateBindingDisplay();
    }

    public void StartRebinding()
    {
        bindingText.text = "Press a key";

        actionReference.action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnComplete(operation =>
            {
                operation.Dispose();
                UpdateBindingDisplay();
                PlayerPrefs.SetString(actionReference.action.name + "_binding_" + bindingIndex,
                    actionReference.action.bindings[bindingIndex].effectivePath);
                PlayerPrefs.Save();
            }).Start();
    }

    private void UpdateBindingDisplay()
    {
        bindingText.text = InputControlPath.ToHumanReadableString(
            actionReference.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
}
