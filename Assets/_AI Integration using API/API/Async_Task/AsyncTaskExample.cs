using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This class demonstrates the difference between Coroutines and async/await Tasks in Unity
/// Both are ways to handle asynchronous operations (operations that take time to complete)
/// </summary>
public class AsyncTaskExample : MonoBehaviour
{
    // UI elements to show status and control the examples
    [SerializeField] private Text _statusText;
    [SerializeField] private Button _startCoroutineButton;
    [SerializeField] private Button _startTaskButton;

    // Flag to prevent multiple operations running at once
    private bool _isRunning;

    private void Start()
    {
        // Set up button click handlers
        _startCoroutineButton.onClick.AddListener(OnButtonClick_StartCoroutineExample);
        _startTaskButton.onClick.AddListener(OnButtonClick_StartTaskExample);
    }

    // Simple examples showing basic usage without return values
    private void OnButtonClick_StartCoroutineExample()
    {
        // Prevent starting if already running
        if (_isRunning) return;

        _statusText.text = "Starting Coroutine...";
        _isRunning = true;
        StartCoroutine(SimpleCoroutine());
        Debug.Log("Coroutine started");
    }

    private async void OnButtonClick_StartTaskExample()
    {
        // Prevent starting if already running
        if (_isRunning) return;

        _statusText.text = "Starting Task...";
        _isRunning = true;
        int result = await SimpleTask();
        Debug.Log("Task completed with result: " + result);
    }

    // Coroutine Example: Uses Unity's built-in system for handling async operations
    // - Works with Unity's time system (WaitForSeconds)
    // - Can be stopped with StopCoroutine
    // - Must be run on the main thread
    private IEnumerator SimpleCoroutine()
    {
        yield return new WaitForSeconds(1f);
        _statusText.text = "Coroutine: Started";

        yield return new WaitForSeconds(2f);
        _statusText.text = "Coroutine: Middle";

        yield return new WaitForSeconds(3f);
        _statusText.text = "Coroutine: Finished";
        _isRunning = false;
    }

    // Task Example: Uses C#'s modern async/await pattern
    // - More flexible than Coroutines
    // - Can run on background threads (careful with Unity API calls!)
    // - Better error handling
    // - Can be canceled with CancellationToken
    private async Task<int> SimpleTask()
    { int value = 0;
        await Task.Delay(1000); // 1 second delay
        _statusText.text = "Task: Started";
        value++;
        await Task.Delay(2000); // 2 second delay
        _statusText.text = "Task: Middle";
        value++;
        await Task.Delay(3000); // 3 second delay
        _statusText.text = "Task: Finished";
        _isRunning = false;

        return value;
    }

    // Clean up event listeners when the object is destroyed
    private void OnDestroy()
    {
        _startCoroutineButton.onClick.RemoveListener(OnButtonClick_StartCoroutineExample);
        _startTaskButton.onClick.RemoveListener(OnButtonClick_StartTaskExample);
    }
}
