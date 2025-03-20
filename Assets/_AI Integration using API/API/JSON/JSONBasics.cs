using System;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

// This class demonstrates different ways to work with JSON data in Unity
public class JSONBasics : MonoBehaviour
{ 
    // UI Text component to display our JSON data
    [SerializeField] private Text _jsonViewText;
    
    // Reference to a JSON file asset
    [SerializeField] private UnityEngine.Object _jsonFile;

    // Displays raw JSON data as a string
    public void DisplayJsonData()
    {
        _jsonViewText.text = _jsonFile.ToString();
    }

    // Demonstrates how to parse and access specific values from JSON
    // In this example, we're accessing: { "user": { "firstName": "value" } }
    public void DisplayJsonValue()
    {
        JObject json = JObject.Parse(_jsonFile.ToString());
        string firstName = json["recentActivities"]["type"].ToString();
        _jsonViewText.text = firstName;  
    }

    // Shows how to convert a C# object to JSON string using Newtonsoft.Json
    // Newtonsoft.Json is more feature-rich compared to Unity's JsonUtility
    public void ConvertToJsonNetonsoft()
    {
        UserData userData = new UserData();
        string jsonString = JsonConvert.SerializeObject(userData, Formatting.Indented);
        _jsonViewText.text = jsonString;
    }

    // Shows how to convert a C# object to JSON string using Unity's built-in JsonUtility
    // JsonUtility is simpler but has limitations (doesn't support dictionaries, etc.)
    public void ConvertToJsonUnity()
    {
        UserData userData = new UserData();
        string jsonString = JsonUtility.ToJson(userData);
        _jsonViewText.text = jsonString;
    }
}

[Serializable]
public class UserData
{
    // SerializeField allows private fields to be serialized
    [SerializeField] private string firstName = "FirstName";
    
    // This private field won't be included in JSON because it lacks SerializeField
    private string lastName = "LastName";
    
    // Public fields are automatically serialized
    public string email = "defaultEmail";
}