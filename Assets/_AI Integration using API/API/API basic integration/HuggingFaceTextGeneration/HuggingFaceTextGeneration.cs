using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <summary>
/// This class handles text generation using the Hugging Face AI API (specifically the Gemma 2B model)
/// It takes user input and generates AI responses
/// </summary>
public class HuggingFaceTextGeneration : MonoBehaviour
{
    /* Valid Models 
    https://huggingface.co/mistralai/Mistral-7B-Instruct-v0.3
    https://huggingface.co/google/gemma-2-2b-it
    */
    private const string BaseApiUrl = "https://api-inference.huggingface.co/models/";

    [SerializeField] private InputField _inputField;        // Where the user types their prompt
    [SerializeField] private Text _outputText;             // Where we'll show the AI's response
    [SerializeField] private Button _generateButton;        // Button to trigger the generation
    [SerializeField] private HuggingFaceResponseWrapper _responsesWrapper;    // Holds the API response data

    [Header("Model Settings")]
    [SerializeField] private string _modelName = "mistralai/Mistral-7B-Instruct-v0.3";
    [SerializeField] private string _authToken = "";

    // Flag to prevent multiple simultaneous generations
    private bool _isGenerating;
    private string ApiUrl => $"{BaseApiUrl}{_modelName}";
    private string AuthToken => $"Bearer {_authToken}";

    private void Start()
    {
        // Validate settings
        if (string.IsNullOrEmpty(_modelName) || string.IsNullOrEmpty(_authToken))
        {
            Debug.LogError("Model name or auth token not set!");
            _generateButton.interactable = false;
            return;
        }

        // Connect the button click to our generation method
        _generateButton.onClick.AddListener(OnButtonClick_GenerateResponse);
    }

    /// <summary>
    /// Called when the Generate button is clicked
    /// Checks if we can generate a response and starts the generation process
    /// </summary>
    private void OnButtonClick_GenerateResponse()
    {
        // Don't generate if we're already generating or if the input is empty
        if (_isGenerating || string.IsNullOrEmpty(_inputField.text))
            return;

        GenerateResponseAsync(_inputField.text);
    }

    /// <summary>
    /// Manages the process of getting a response from the AI
    /// Updates UI elements to show the generation status
    /// </summary>
    private async void GenerateResponseAsync(string prompt)
    {
        // Set UI state to "generating"
        _isGenerating = true;
        _outputText.text = "Generating...";
        _generateButton.interactable = false;

        try
        {
            // Try to get a response from the API
            string response = await SendRequestAsync(prompt);
            _outputText.text = response;
        }
        catch (Exception e)
        {
            // If something goes wrong, show the error
            _outputText.text = $"Error: {e.Message}";
            Debug.LogError($"Error generating response: {e}");
        }

        // Reset UI state after generation (whether successful or not)
        _isGenerating = false;
        _generateButton.interactable = true;
    }

    /// <summary>
    /// Sends the actual HTTP request to the Hugging Face API
    /// This is where the network communication happens
    /// </summary>
    private async Task<string> SendRequestAsync(string prompt)
    {
        #region STEP 1: Prepare the data for the API
        // Convert our prompt into a format the API expects (JSON)
        var requestData = new HuggingFaceRequest { inputs = prompt };
        string jsonData = JsonConvert.SerializeObject(requestData);
        #endregion

        #region STEP 2: Create and configure the HTTP request
        // Create a new POST request to the API URL
        using UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        #endregion

        #region STEP 3: Convert our JSON data into bytes
        // The API needs the data as bytes, not a string
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        #endregion

        #region STEP 4: Set up handlers for sending and receiving data
        // uploadHandler: Responsible for sending our prompt to the API
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        // downloadHandler: Responsible for receiving the API's response
        request.downloadHandler = new DownloadHandlerBuffer();
        #endregion

        #region STEP 5: Add required headers
        // Tell the API we're sending JSON data
        request.SetRequestHeader("Content-Type", "application/json");
        // Authenticate our request with our API key
        request.SetRequestHeader("Authorization", AuthToken);
        #endregion

        #region STEP 6: Send the request and wait for response
        var operation = request.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();  // Wait without freezing the game
        #endregion

        #region STEP 7: Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"API Response Code: {request.responseCode}");
            Debug.LogError($"Raw Error: {request.error}");
            Debug.LogError($"Response Body: {request.downloadHandler?.text}");
            throw new Exception($"Request failed: {request.error}");
        }
        #endregion

        #region STEP 8: Process the response
        string responseText = request.downloadHandler.text;
        Debug.Log($"Raw response: {responseText}");

        HuggingFaceResponse[] responses = JsonConvert.DeserializeObject<HuggingFaceResponse[]>(responseText);
        _responsesWrapper.responses = responses;

        return responses[0].generated_text;
        #endregion
    }
}

// Classes to help with JSON serialization/deserialization

/// <summary>
/// Wraps the array of responses from the API
/// </summary>
[Serializable]
public class HuggingFaceResponseWrapper
{
    public HuggingFaceResponse[] responses;
}

/// <summary>
/// Represents a single response from the API
/// </summary>
[Serializable]
public class HuggingFaceResponse
{
    public string generated_text;
}

/// <summary>
/// Represents the data we send to the API
/// </summary>
[Serializable]
public class HuggingFaceRequest
{
    public string inputs;
}
