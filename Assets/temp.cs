using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To Enable the featuer Define JAYCE_ANALYSIS_HELPER
public class temp : MonoBehaviour
{
    // Values you are interested in for observing.
    int int01;
    float float01;
    string string01;
    
    void Start()
    {
        // Create a channel , 
        // Think of it as a group . 
        // You might want to observe more than one value but there are too many , 
        // You can devide by channeling.

        // So i registered a channel which its identifier is "0"
        JayceGUIAnalysisHelper.Instance.RegisterChannel("0");

        // A key needed for a value you wanna observe because the 
        // helper class needs to know which one is which.
        JayceGUIAnalysisHelper.Instance.AddKey("0", "int01");
        JayceGUIAnalysisHelper.Instance.AddKey("0", "float01");
        JayceGUIAnalysisHelper.Instance.AddKey("0", "string01");
    }

    void Update()
    {
        // Generate Random Values 
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetInt(Random.Range(0, 100));
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SetFloat(Random.Range(0f, 1f) * Random.Range(1, 100));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SetString("whole lot of cash" + Random.Range(0, 100).ToString());
        }

        // OR you can just skip registering channel and key , 
        // It will just automatically register by the requested channel and key.
        if (Input.GetKeyDown(KeyCode.R))
        {
            JayceGUIAnalysisHelper.Instance.SetValue("Auto", "Hello", 1234);
        }

        // It will count ascending . Also automatical registration.
        if(Input.GetKeyDown(KeyCode.T))
        {
            JayceGUIAnalysisHelper.Instance.AdjustValue("Count", "Int", 1);
        }
    }

    // Update values to the helper class 

    void SetInt(int i)
    {
        int01 = i;

        // Channel, Key, New Value
        // So the class updates the value matches the key and
        // show it to you on the screen by GUI api 
        JayceGUIAnalysisHelper.Instance.SetValue("0", "int01", i);
    }

    void SetFloat(float f)
    {
        float01 = f;

        JayceGUIAnalysisHelper.Instance.SetValue("0", "float01", f);
    }

    void SetString(string str)
    {
        string01 = str;

        JayceGUIAnalysisHelper.Instance.SetValue("0", "string01", str);
    }
}
