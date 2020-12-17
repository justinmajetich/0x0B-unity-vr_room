using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Keyboard : MonoBehaviour
{
    public TextMeshProUGUI inputField;
    public Color32 placeholderColor = Color.grey;
    public Color32 inputColor = Color.white;
    
    [SerializeField] LockPanel lockPanel;
    [SerializeField] DoorBehavior glassDoor;
    [SerializeField] string passcode = "RATS";
    [SerializeField] int inputBufferSize = 14;
    string[] inputBuffer;
    string inputPlaceholder = "Enter Passcode";


    void Start()
    {
        inputBuffer = new string[inputBufferSize];

        // Initialize buffer with empty strings.
        EmptyInputBuffer();

        inputField.color = placeholderColor;
        inputField.text = inputPlaceholder;

        if (!glassDoor) { glassDoor = GameObject.Find("GlassDoor").GetComponent<DoorBehavior>(); }
        if (!lockPanel) { lockPanel = GameObject.Find("LockStatus").GetComponent<LockPanel>(); }
    }

    public void Input(string character) {

        QuestDebug.Instance.Log("In Keyboard.Input()");

        int i = 0;

        // Search input buffer for first empty position to assign new character.
        for (; i < inputBufferSize; i++) {
            if (inputBuffer[i].Length == 0) {
                inputBuffer[i] = character;
                break;
            }
        }

        // If loop ended without assignment, buffer is full. Play error audio.

        // Set updated input buffer to input field.
        inputField.color = inputColor;
        inputField.text = String.Join("", inputBuffer);
    }

    public void Backspace() {

        int i = inputBufferSize - 1;

        // Search backwards through input buffer for last non-empty position and replace character with empty string.
        for (; i >= 0; i--) {
            if (inputBuffer[i].Length != 0) {
                inputBuffer[i] = String.Empty;
                break;
            }
        }

        // If buffer is empty, play error audio.

        // Set updated input buffer to input field.
        inputField.text = String.Join("", inputBuffer);
    }

    private void EmptyInputBuffer() {
        for (int i = 0; i < inputBufferSize; i++) {
            inputBuffer[i] = String.Empty;
        }
    }

    public void Enter() {

        if (inputField.text == passcode) {
            lockPanel.Unlock();
            glassDoor.Open();
        } else {
            // Play error audio.
            inputField.color = placeholderColor;
            inputField.text = "Passcode Error";
            EmptyInputBuffer();
        }
    }
}
