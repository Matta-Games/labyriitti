using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class Taskmaster : MonoBehaviour
{
    [System.Serializable]
    public class Objective
    {
        public string text;
        [HideInInspector] public bool completed = false;
    }

    [SerializeField] private TMP_Text uiText;
    [SerializeField] private float typeSpeed = 0.04f;
    [SerializeField] private float completedDelay = 0.5f;
    [SerializeField] private List<Objective> objectives = new List<Objective>();

    private int index = 0;

    void Start()
    {
        StartCoroutine(RunTutorial());
    }

    void Update()
    {
        if (index >= objectives.Count) return;

        var gamepad = Gamepad.current;
        if (gamepad == null) return; // only accept input from a connected gamepad

        // B on Xbox controllers maps to buttonEast in the Input System.
        // Pressing B completes the current objective.
        if (gamepad.buttonEast.wasPressedThisFrame)
        {
            Objective obj = objectives[index];
            if (!obj.completed)
            {
                obj.completed = true;
                // update the UI immediately so player sees feedback
                uiText.text = GenerateDisplayText(obj);
            }
        }
    }

    IEnumerator RunTutorial()
    {
        while (index < objectives.Count)
        {
            Objective obj = objectives[index];
            obj.completed = false;

            // Typewriter effect for the objective text
            uiText.text = "";
            foreach (char c in obj.text)
            {
                uiText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }

            // Show instruction to press B
            uiText.text = GenerateDisplayText(obj);

            // Wait until objective is completed (B pressed)
            yield return new WaitUntil(() => obj.completed);

            // Strike-through when completed
            uiText.text = $"<s>{obj.text}</s>";
            yield return new WaitForSeconds(completedDelay);

            index++;
        }
    }

    // Generates the display text with an instruction to press B
    private string GenerateDisplayText(Objective obj)
    {
        // Use simple palette: pressed B will immediately change to strike-through in RunTutorial
        return $"{obj.text}\n\n<color=#FFE8D6>Press <color=#FB8B24>B</color> to continue</color>";
    }
}
