using System.Collections.Generic;
using System.Reflection;
using LLMUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Main class that handles the movement of objects based on text commands
public class MoveObjectsByCommand : MonoBehaviour
{
    // References to required components
    public LLMCharacter llmCharacter;        // Reference to the AI language model
    public InputField playerText;            // Input field where player types commands
    [SerializeField] Button sendButton;
    [SerializeField] private TextMeshProUGUI _responseText; // Text field to display the response
    [SerializeField, TextArea(3, 10)] private string _systemInstructions; // Instructions for the system

    // References to the UI objects that can be moved
    public RectTransform blueSquare;
    public RectTransform redSquare;

    void Start()
    {
        // Set up the input field to listen for submissions
        playerText.onSubmit.AddListener(onInputFieldSubmit);
        sendButton.onClick.AddListener(() => onInputFieldSubmit(playerText.text));
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
    string ConstructDamagePrompt(string message)
    {
        string prompt = "From the input, which Skill is mentioned? Choose from the following options:\n\n";
        prompt += "Input:" + message + "\n\n";
        prompt += "Choices:\n";
        foreach (string functionName in GetFunctionNames<AttackFunctions>()) 
            prompt += $"- {functionName}\n";
        prompt += "\nAnswer directly with the choice, focusing only on Skill";
        return prompt;
    }

    // Constructs the prompt for the AI to understand color commands
    string ConstructHeroPrompt(string message)
    {
        string prompt = "From the input, which Hero is mentioned? Choose from the following options:\n\n";
        prompt += "Input:" + message + "\n\n";
        prompt += "Choices:\n";
        foreach (string functionName in GetFunctionNames<HeroFunctions>()) 
            prompt += $"- {functionName}\n";
        prompt += "\nAnswer directly with the choice, focusing only on Hero";
        return prompt;
    }

    string ConstructResponsePrompt(string message)
    {
        string prompt = "Answer the input using the system instructions\n\n";
        prompt += "Input:" + message + "\n\n";
        prompt += "SystemInstructions:" + _systemInstructions + "\n\n";
        return prompt;
    }

    // Called when the player submits text in the input field
    async void onInputFieldSubmit(string message)
    {
        // Disable input while processing
        playerText.interactable = false;

        // Ask the AI to interpret the direction and color from the input
        string getDamage = await llmCharacter.Chat(ConstructDamagePrompt(message));
        string getHero = await llmCharacter.Chat(ConstructHeroPrompt(message));
        string getResponse = await llmCharacter.Chat(ConstructResponsePrompt(message));

        // Convert the AI's responses into actual Color and Vector3 values using reflection
        Color hero = (Color)typeof(HeroFunctions).GetMethod(getHero).Invoke(null, null);
        int damage = (int)typeof(AttackFunctions).GetMethod(getDamage).Invoke(null, null);

        // Log the results for debugging
        Debug.Log($"Damage function called: {getDamage}, returned: {damage}");
        Debug.Log($"Color function called: {getHero}, returned: {hero}");

        // Get the object to move based on the color
        RectTransform selectedObject = GetObjectByColor(hero);
        if (selectedObject != null)
        {
            Debug.Log($"Selected object: {selectedObject.name}");
            selectedObject.GetComponent<Enemy>().TakeDamage(damage);
        }
        else
        {
            _responseText.text = getResponse;
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
