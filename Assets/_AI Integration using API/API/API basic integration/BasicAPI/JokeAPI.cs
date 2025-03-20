using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// This class represents the structure of the JSON response we get from the Chuck Norris API.
/// Each property matches the JSON fields in the API response.
/// </summary>
[Serializable]
public class ChuckNorrisJoke
{
    public string icon_url;  // URL of the joke's icon
    public string id;        // Unique identifier for the joke
    public string url;       // Web URL where the joke can be found
    public string value;     // The actual joke text
}

/// <summary>
/// This class handles fetching random jokes from the Chuck Norris API and displaying them in the UI.
/// It demonstrates basic API integration in Unity using UnityWebRequest.
/// </summary>
public class JokeAPI : MonoBehaviour
{
    // UI elements that we'll connect in the Unity Inspector
    [SerializeField] private Text _jokeText;        // Text component to display the joke
    [SerializeField] private Button _fetchJokeButton; // Button to trigger new joke fetch
    
    // API endpoint we'll be calling to get random jokes
    private const string API_URL = "https://api.chucknorris.io/jokes/random";

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Sets up the button click listener.
    /// </summary>
    private void Awake()
    {
        _fetchJokeButton.onClick.AddListener(OnButtonClick_FetchNewJoke);
    }

    /// <summary>
    /// Called when the script instance is being destroyed.
    /// Cleans up the button click listener to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        _fetchJokeButton.onClick.RemoveListener(OnButtonClick_FetchNewJoke);
    }

    /// <summary>
    /// Asynchronously fetches a random joke from the API.
    /// Shows how to:
    /// 1. Make an HTTP GET request
    /// 2. Handle the response
    /// 3. Parse JSON data
    /// 4. Update UI elements
    /// 5. Handle errors
    /// </summary>
    public async Task FetchRandomJoke()
    {
        try
        {
            // Disable the button while fetching to prevent multiple requests
            _fetchJokeButton.interactable = false;

            // Create and send the web request
            using UnityWebRequest request = UnityWebRequest.Get(API_URL);
            var operation = request.SendWebRequest();

            // Wait for the request to complete
            while (!operation.isDone)
                await Task.Yield();

            // Check if the request was successful
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
                return;
            }

            // Parse the JSON response into our ChuckNorrisJoke class
            string jsonResponse = request.downloadHandler.text;
            ChuckNorrisJoke joke = JsonUtility.FromJson<ChuckNorrisJoke>(jsonResponse);
            
            // Display the joke text in the UI
            _jokeText.text = joke.value;
        }
        catch (Exception e)
        {
            // Handle any errors that occurred during the process
            Debug.LogError($"Error fetching joke: {e.Message}");
            _jokeText.text = "Failed to fetch joke";
        }
        finally
        {
            // Re-enable the button whether the request succeeded or failed
            _fetchJokeButton.interactable = true;
        }
    }

    /// <summary>
    /// Button click handler that initiates fetching a new joke.
    /// The underscore (_) discards the Task return value since we don't need to await it here.
    /// </summary>
    public void OnButtonClick_FetchNewJoke()
    {
        _ = FetchRandomJoke();
    }
}
