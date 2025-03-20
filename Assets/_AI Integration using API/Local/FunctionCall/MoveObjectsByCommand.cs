using System.Collections.Generic;
using System.Reflection;
using LLMUnity;
using UnityEngine;
using UnityEngine.UI;

// Main class that handles the movement of objects based on text commands
public class MoveObjectsByCommand : MonoBehaviour
{
    // References to required components
    public LLMCharacter llmCharacter;        // Reference to the AI language model
    public InputField playerText;            // Input field where player types commands
    
    // References to the UI objects that can be moved
    public RectTransform blueSquare;
    public RectTransform redSquare;

    void Start()
    {
        // Set up the input field to listen for submissions
        playerText.onSubmit.AddListener(onInputFieldSubmit);
        playerText.Select();  // Automatically focus the input field
    }

    // Helper method to get all function names from a class using reflection
    string[] GetFunctionNames<T>()
    {
        List<string> functionNames = new List<string>();
        foreach (var function in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)) 
            functionNames.Add(function.Name);
        return functionNames.ToArray();
    }

    // Constructs the prompt for the AI to understand direction commands
    string ConstructDirectionPrompt(string message)
    {
        string prompt = "From the input, which direction is mentioned? Choose from the following options:\n\n";
        prompt += "Input:" + message + "\n\n";
        prompt += "Choices:\n";
        foreach (string functionName in GetFunctionNames<DirectionFunctions>()) 
            prompt += $"- {functionName}\n";
        prompt += "\nAnswer directly with the choice, focusing only on direction";
        return prompt;
    }

    // Constructs the prompt for the AI to understand color commands
    string ConstructColorPrompt(string message)
    {
        string prompt = "From the input, which color is mentioned? Choose from the following options:\n\n";
        prompt += "Input:" + message + "\n\n";
        prompt += "Choices:\n";
        foreach (string functionName in GetFunctionNames<ColorFunctions>()) 
            prompt += $"- {functionName}\n";
        prompt += "\nAnswer directly with the choice, focusing only on color";
        return prompt;
    }

    // Called when the player submits text in the input field
    async void onInputFieldSubmit(string message)
    {
        // Disable input while processing
        playerText.interactable = false;

        // Ask the AI to interpret the direction and color from the input
        string getDirection = await llmCharacter.Chat(ConstructDirectionPrompt(message));
        string getColor = await llmCharacter.Chat(ConstructColorPrompt(message));

        // Convert the AI's responses into actual Color and Vector3 values using reflection
        Color color = (Color)typeof(ColorFunctions).GetMethod(getColor).Invoke(null, null);
        Vector3 direction = (Vector3)typeof(DirectionFunctions).GetMethod(getDirection).Invoke(null, null);

        // Log the results for debugging
        Debug.Log($"Direction function called: {getDirection}, returned: {direction}");
        Debug.Log($"Color function called: {getColor}, returned: {color}");

        // Get the object to move based on the color
        RectTransform selectedObject = GetObjectByColor(color);
        if (selectedObject != null)
        {
            Debug.Log($"Selected object: {selectedObject.name}");
            // Move the object in the specified direction (multiplied by 100 for visible movement)
            selectedObject.anchoredPosition += (Vector2)direction * 100f;
        }
        else
        {
            Debug.Log("No object selected (NoneColor returned)");
        }

        // Re-enable input
        playerText.interactable = true;
    }

    // Helper method to get the correct square based on color
    private RectTransform GetObjectByColor(Color color)
    {
        if (color == Color.blue)
        {
            return blueSquare;
        }
        else if (color == Color.red)
        {
            return redSquare;
        }
        else
        {
            return null;
        }
    }

    // Cancels any pending AI requests
    public void CancelRequests()
    {
        llmCharacter.CancelRequests();
    }

    // Quits the application (called by UI button)
    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }

    // Editor-only validation to ensure the AI model is properly set up
    bool onValidateWarning = true;
    void OnValidate()
    {
        if (onValidateWarning && !llmCharacter.remote && llmCharacter.llm != null && llmCharacter.llm.model == "")
        {
            Debug.LogWarning($"Please select a model in the {llmCharacter.llm.gameObject.name} GameObject!");
            onValidateWarning = false;
        }
    }
}
