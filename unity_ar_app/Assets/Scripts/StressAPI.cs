using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[System.Serializable]
public class StressData
{
    public float probability;
    public string state;
}

public class StressAPI : MonoBehaviour
{
    public TextMeshProUGUI stressText;
    string url = "http://10.1.58.154:5000/predict_live";
    
    void Start()
    {
        InvokeRepeating("GetStress", 1f, 2f);
    }

    void GetStress()
    {
        StartCoroutine(Request());
    }

    IEnumerator Request()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            StressData data = JsonUtility.FromJson<StressData>(request.downloadHandler.text);

            // Display
            stressText.text = data.state + "\nProb: " + data.probability.ToString("F2");

            // Use STATE (not probability)
            switch (data.state)
            {
                case "HIGH STRESS":
                    stressText.color = Color.red;
                    break;

                case "MODERATE":
                    stressText.color = Color.yellow;
                    break;

                case "CALM":
                    stressText.color = Color.green;
                    break;
            }
        }
        else
        {
            Debug.LogError("API Error: " + request.error);
        }
    }
}