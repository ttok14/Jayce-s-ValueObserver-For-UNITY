using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Linq;

//
//  To Use , Define JAYCE_ANALYSIS_HELPER
// 
public class JayceGUIAnalysisHelper : MonoBehaviour
{
    enum WidthOps
    {
        Tap01,
        Tap02,
        Tap03,
        Value01,
        Value02,
        Value03,
        END
    }

    static JayceGUIAnalysisHelper instance;

    public static JayceGUIAnalysisHelper Instance
    {
        get
        {
            if (instance == null)
            {
                new GameObject("GUIAnalysisHelper", typeof(JayceGUIAnalysisHelper));
            }

            return instance;
        }
    }

    class Value : IComparable<Value>
    {
        public string key;
        public float idleTimeElapsed;
        public string value;
        public string oldValue;

        public Value(string key)
        {
            this.key = key;
            SetDefault();
        }

        public int CompareTo(Value other)
        {
            if (idleTimeElapsed < other.idleTimeElapsed)
            {
                return -1;
            }
            else if (idleTimeElapsed > other.idleTimeElapsed)
            {
                return 1;
            }

            return 0;
        }

        public void SetDefault()
        {
            value = NOT_SET_STRING;
            oldValue = NOT_SET_STRING;
            idleTimeElapsed = 0;
        }
    }

    class DrawProperty
    {
        public int fontSize;

        public float startXratioToScreen;
        public float startYratioToScreen;

        public float[] widths;

        public DrawProperty()
        {
            fontSize = 0;
            startXratioToScreen = 0;
            startYratioToScreen = 0;
            widths = new float[(int)WidthOps.END];
        }
    }

    // channel , value
    Dictionary<string, List<Value>> values = new Dictionary<string, List<Value>>();

    DrawProperty prop = new DrawProperty();

    const string tapKey = "<color=red>Key</color>";
    const string tapIdleTime = "<color=red>IdleTime</color>";
    const string tapValue = "<color=red>Value</color>";
    const string tapOldValue = "<color=red>OldValue</color>";

    const string fontSizeKey = "GUIAnalysisHelper_fontSize_";
    const string startXratioToScreenKey = "GUIAnalysisHelper_startXratio_";
    const string startYratioToScreenKey = "GUIAnalysisHelper_startYratio_";
    string[] widthKeys =
    {
     "GUIAnalysisHelper_tapWidth01_",
     "GUIAnalysisHelper_tapWidth02_",
     "GUIAnalysisHelper_tapWidth03_",
      "GUIAnalysisHelper_valueWidth01_",
      "GUIAnalysisHelper_valueWidth02_",
      "GUIAnalysisHelper_valueWidth03_"
    };

    const string NOT_SET_STRING = "N/S";
    const string showString = "Show";
    const string hideString = "Hide";
    const string editOptionString = "EditOption";
    const string setToDefaultString = "SetToDefault";
    const string fontSizeString = "FontSize";
    const string positionString = "Position";
    const string widthString = "Width";
    const string pauseString = "Pause";
    const string channelListString = "Channels";
    const string noExistingChannelString = "<color=red>No Existing Channel</color>";
    const string adjustTimeScaleString = "AdjustTimeScale";
    const string clearValuesOnChangeChannelString = "ClearOnChannelSwitch";
    const string autoSortString = "AutoSort";

    const string floatFormatter = "0.000";

    int curChannelIndex;
    string curChannel = "";
    List<Value> curChannelValue;
    string[] channels;

    const int default_fontSize = 20;
    const float default_startXratioToScreen = 0.2f;
    const float default_startYratioToScreen = 0.2f;
    readonly float[] default_widths = { 140, 140, 140, 140, 140, 140 };

    Rect area;
    GUILayoutOption[] widths = new GUILayoutOption[(int)WidthOps.END];
    GUILayoutOption generalLayoutWidthOp;
    GUILayoutOption optionChannelListWid, optionChannelListHei;
    GUIStyle style;

    float oldTimeScale;
    float curTimeScale = 1;

    bool show = true;
    bool editOption;
    bool sortValuesByIdleTime = true;
    bool clearValuesOnChangeChannel = false;
    bool adjustTimeScale = false;

    bool sortingValues;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);

        // Array.ForEach(widthKeys, t => t.Concat(Application.productName));

        generalLayoutWidthOp = GUILayout.Width(100);
        optionChannelListWid = GUILayout.Width(150);
        optionChannelListHei = GUILayout.Height(35);

        style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;

        LoadDrawProperty();
        UpdateDrawProperty();
    }

    void UpdateWidthOps()
    {
        for (int i = 0; i < prop.widths.Length; i++)
        {
            widths[i] = GUILayout.Width(prop.widths[i]);
        }
    }

    void SetWidth(WidthOps op, float width)
    {
        prop.widths[(int)op] = width;
    }

    void SetWidths(
        params float[] widths)
    {
        for (int i = 0; i < (int)WidthOps.END; i++)
        {
            prop.widths[i] = widths[i];
        }
    }

    void SetDrawPropertyToDefault()
    {
        prop.fontSize = default_fontSize;
        prop.startXratioToScreen = default_startXratioToScreen;
        prop.startYratioToScreen = default_startYratioToScreen;
        default_widths.CopyTo(prop.widths, 0);

        UpdateDrawProperty();
    }

    /// <summary>
    /// e.g) Red : ff0000 . . . 
    /// </summary>
    /// <param name="color"></param>
    string ColorizeString(string source, string color)
    {
        return "<color=#" + color + ">" + source + "</color>";
    }

    public void LoadDrawProperty()
    {
        prop.fontSize = PlayerPrefs.GetInt(fontSizeKey, default_fontSize);
        prop.startXratioToScreen = PlayerPrefs.GetFloat(startXratioToScreenKey, default_startXratioToScreen);
        prop.startYratioToScreen = PlayerPrefs.GetFloat(startYratioToScreenKey, default_startYratioToScreen);
        LoadWidths();
    }

    void LoadWidths()
    {
        for (int i = 0; i < (int)WidthOps.END; i++)
        {
            prop.widths[i] = PlayerPrefs.GetFloat(widthKeys[i], default_widths[i]);
        }
    }

    void SaveWidthToDisk()
    {
        for (int i = 0; i < (int)WidthOps.END; i++)
        {
            PlayerPrefs.SetFloat(widthKeys[i], prop.widths[i]);
        }
    }

    void SaveDrawPropertyToDisk()
    {
        PlayerPrefs.SetInt(fontSizeKey, prop.fontSize);
        PlayerPrefs.SetFloat(startXratioToScreenKey, prop.startXratioToScreen);
        PlayerPrefs.SetFloat(startYratioToScreenKey, prop.startYratioToScreen);
        SaveWidthToDisk();
    }

    void UpdateDrawProperty()
    {
        style.fontSize = prop.fontSize;
        area.Set(Screen.width * prop.startXratioToScreen, Screen.height * prop.startYratioToScreen, Screen.width, Screen.height);
        UpdateWidthOps();
    }

    public void SetEnable(bool enable)
    {
        show = enable;
    }

    public void RegisterChannel(string channel)
    {
        if (values.ContainsKey(channel))
        {
            return;
        }
        if (string.IsNullOrEmpty(channel))
        {
            PrintErrorMsg("It is not valid string", channel);
            return;
        }

        bool wasEmpty = values.Count == 0;

        values.Add(channel, new List<Value>());

        if (wasEmpty)
        {
            ChangeChannel(channel);
        }
        else
        {
            SetChannelProperty(curChannel, true);
        }
    }

    public void UnregisterChannel(string channel)
    {
        if (values.ContainsKey(channel) == false)
        {
            PrintErrorMsg("Not Found Channel", channel);
            return;
        }

        ReleaseValues(values[channel]);
        values[channel] = null;
        values.Remove(channel);

        bool isCurrentChannel = curChannel.Equals(channel);

        if (isCurrentChannel)
        {
            string switchTo = values.Count > 0 ? values.First().Key : string.Empty;

            ChangeChannel(switchTo);
        }
    }

    public void ClearChannels()
    {
        foreach (var channel in values)
        {
            ReleaseValues(channel.Value);
        }

        values.Clear();

        ChangeChannel(string.Empty);
    }

    public void ChangeChannel(string channel)
    {
        if (curChannel.Equals(channel))
        {
            return;
        }

        SetChannelProperty(channel, true);

        OnChangeChannel();
    }

    public void AddKey(string channel, params string[] key)
    {
        if (key == null)
        {
            PrintErrorMsg("Invalid Key", string.Empty);
            return;
        }

        if (values.ContainsKey(channel) == false)
        {
            RegisterChannel(channel);
        }

        var targetChannel = values[channel];

        foreach (var val in targetChannel)
        {
            foreach (var _key in key)
            {
                if (val.key.Equals(_key))
                {
                    //    PrintErrorMsg("Duplicate Key", _key);
                    return;
                }
            }
        }

        AddKey(targetChannel, key);
    }

    public void AddKey(string channel, string key, string defaultValue)
    {
        if (string.IsNullOrEmpty(key))
        {
            PrintErrorMsg("Invalid Key", string.Empty);
            return;
        }
        
        if (values.ContainsKey(channel) == false)
        {
            RegisterChannel(channel);
        }
       
        var targetChannel = values[channel];
        var duplicate = targetChannel.Find(t => t.Equals(key));

        if(duplicate == null)
        {
            AddKey(targetChannel, key);
            targetChannel.Last().value = defaultValue;
        }
    }

    void AddKey(List<Value> list, params string[] keys)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            list.Add(new Value(keys[i]));
        }
    }

    public void RemoveKey(string channel, string key)
    {
        var targetValues = values[channel];
        var target = targetValues.Find(t => t.key.Equals(key));

        if (target != null)
        {
            targetValues.Remove(target);
        }
    }

    public void SetValue(string channel, string key, int value)
    {
        SetValue(channel, key, value.ToString());
    }

    public void SetValue(string channel, string key, float value)
    {
        SetValue(channel, key, value.ToString(floatFormatter));
    }

    public void SetValue(string channel, string key, string value)
    {
        if (values.ContainsKey(channel) == false)
        {
            RegisterChannel(channel);
        }

        var targetValues = values[channel];

        var target = targetValues.Find(t => t.key.Equals(key));

        if (target == null)
        {
            AddKey(targetValues, key);
            target = targetValues.Last();
        }

        target.idleTimeElapsed = 0;

        if (target.value.Equals(value) == false)
        {
            string superOldValue = target.oldValue;
            target.oldValue = target.value;
            target.value = value;

            if (sortValuesByIdleTime)
            {
                sortingValues = true;
            }
        }
    }

    public void AdjustValue(string channel, string key, int amount)
    {
        if (values.ContainsKey(channel) == false)
        {
            RegisterChannel(channel);
        }

        var target = values[channel].Find(t => t.key.Equals(key));

        if (target == null)
        {
            AddKey(channel, key, "0");
        }
        else
        {
            int curValue = -1;

            if (int.TryParse(target.value, out curValue))
            {
                SetValue(channel, key, curValue + amount);
            }
        }
    }

    public void AdjustValue(string channel, string key, float amount)
    {
        if (values.ContainsKey(channel) == false)
        {
            RegisterChannel(channel);
        }

        var target = values[channel].Find(t => t.key.Equals(key));

        if (target == null)
        {
            AddKey(channel, key, "0.0");
        }
        else
        {
            float curValue = -1;

            if (float.TryParse(target.value, out curValue))
            {
                SetValue(channel, key, curValue + amount);
            }
        }
    }

    public string GetValue(string channel, string key)
    {
        if (values.ContainsKey(channel) == false)
            return string.Empty;

        var target = values[channel].Find(t => t.key.Equals(key));
        string result = string.Empty;

        if(target != null)
        {
            result = target.value;
        }

        return result;
    }

    void SetChannelProperty(string channel, bool updateChannelStringCollection)
    {
        curChannel = channel;

        if (string.IsNullOrEmpty(channel))
        {
            curChannelValue = null;
        }
        else
        {
            curChannelValue = values[channel];
        }

        curChannelIndex = -1;

        int i = 0;

        if (values.Count > 0)
        {
            if (updateChannelStringCollection)
            {
                channels = new string[values.Count];
                i = 0;

                foreach (var c in values)
                {
                    channels[i] = c.Key;
                    i++;
                }
            }

            i = 0;

            foreach (var c in values)
            {
                if (c.Key.Equals(curChannel))
                {
                    curChannelIndex = i;
                    break;
                }

                i++;
            }
        }
        else
        {
            if (updateChannelStringCollection)
            {
                channels = null;
            }

            curChannelIndex = -1;
        }
    }

    void ReleaseValues(List<Value> values)
    {
        for (int i = 0; i < values.Count; i++)
        {
            values[i] = null;
        }

        values.Clear();
    }

    void OnChangeChannel()
    {
        if (curChannelValue != null)
        {
            foreach (var v in curChannelValue)
            {
                if (clearValuesOnChangeChannel)
                {
                    v.SetDefault();
                }
            }
        }
    }

    void PrintErrorMsg(string statementMsg, string colorizedMsg)
    {
        UnityEngine.Debug.LogError("<" + statementMsg + ">\t<<color=red>" + colorizedMsg + "</color>>");
    }

    private void Update()
    {
#if JAYCE_ANALYSIS_HELPER
        foreach (var channel in values)
        {
            foreach (var value in channel.Value)
            {
                value.idleTimeElapsed += Time.deltaTime;
            }
        }

        if (sortingValues && curChannelValue != null)
        {
            curChannelValue.Sort();
            sortingValues = false;
        }
#endif
    }

    private void OnGUI()
    {
#if JAYCE_ANALYSIS_HELPER
        if (GUILayout.Button(show ? hideString : showString, generalLayoutWidthOp))
        {
            SetEnable(!show);
        }

        if (show)
        {
            if (GUILayout.Button(pauseString, generalLayoutWidthOp))
            {
                Debug.Break();
            }

            if (GUILayout.Button(editOptionString, generalLayoutWidthOp))
            {
                editOption = !editOption;
            }

            if (editOption)
            {
                DrawEditOption();
            }

            DrawTable();
        }
#endif
    }

    private void DrawTable()
    {
        GUILayout.BeginArea(area);

        GUILayout.BeginHorizontal();

        GUILayout.Label(tapKey, style, widths[(int)WidthOps.Tap01]);
        GUILayout.Label(tapIdleTime, style, widths[(int)WidthOps.Tap02]);
        GUILayout.Label(tapValue, style, widths[(int)WidthOps.Tap03]);
        GUILayout.Label(tapOldValue, style, null);

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (curChannelValue != null)
        {
            foreach (var v in curChannelValue)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(v.key, style, GUILayout.Width(prop.widths[(int)WidthOps.Value01])); //, widOp, heiOp);

                string timeElapsed = v.idleTimeElapsed.ToString(floatFormatter);

                GUILayout.Label(timeElapsed, style, GUILayout.Width(prop.widths[(int)WidthOps.Value02]));
                GUILayout.Label(v.value, style, GUILayout.Width(prop.widths[(int)WidthOps.Value03]));
                GUILayout.Label(v.oldValue, style, null);

                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndArea();
    }

    private void DrawEditOption()
    {
        if (GUILayout.Button(setToDefaultString, generalLayoutWidthOp))
        {
            SetDrawPropertyToDefault();
        }

        bool oldAdjustTimeScale = adjustTimeScale;

        adjustTimeScale = GUILayout.Toggle(adjustTimeScale, adjustTimeScaleString);

        if (oldAdjustTimeScale != adjustTimeScale)
        {
            if (adjustTimeScale)
            {
                oldTimeScale = Time.timeScale;
                //    TimeManager.instance.timeScale = curTimeScale;
                Time.timeScale = curTimeScale;
            }
            else
            {
                Time.timeScale = oldTimeScale;
                //      TimeManager.instance.timeScale = oldTimeScale;
            }
        }

        if (adjustTimeScale)
        {
            float oldScale = curTimeScale;
            curTimeScale = GUILayout.HorizontalSlider(curTimeScale, 0f, 4f, generalLayoutWidthOp);
            if (oldScale != curTimeScale)
            {
                //     TimeManager.instance.timeScale = curTimeScale;
                Time.timeScale = curTimeScale;
            }
        }

        bool oldSortValues = sortValuesByIdleTime;

        sortValuesByIdleTime = GUILayout.Toggle(sortValuesByIdleTime, autoSortString);

        if (oldSortValues != sortValuesByIdleTime &&
            sortValuesByIdleTime)
        {
            sortingValues = true;
        }

        clearValuesOnChangeChannel = GUILayout.Toggle(clearValuesOnChangeChannel, clearValuesOnChangeChannelString);

        GUILayout.Label(fontSizeString);
        prop.fontSize = (int)(GUILayout.HorizontalSlider(prop.fontSize, 0, 100, generalLayoutWidthOp));

        GUILayout.Label(positionString);
        prop.startXratioToScreen = GUILayout.HorizontalSlider(prop.startXratioToScreen, 0f, 1f, generalLayoutWidthOp);
        prop.startYratioToScreen = GUILayout.VerticalSlider(prop.startYratioToScreen, 0f, 1f, generalLayoutWidthOp);

        GUILayout.Label(widthString);

        for (int i = 0; i < (int)WidthOps.END; i++)
        {
            SetWidth((WidthOps)i, GUILayout.HorizontalSlider(prop.widths[i], 0, Screen.width, generalLayoutWidthOp));
        }

        GUILayout.Space(5);

        if (values.Count > 0)
        {
            GUILayout.Label(channelListString);

            int oldChannel = curChannelIndex;

            curChannelIndex = GUILayout.Toolbar(curChannelIndex, channels, optionChannelListWid, optionChannelListHei);

            if (oldChannel != curChannelIndex)
            {
                ChangeChannel(channels[curChannelIndex]);
            }
        }
        else
        {
            GUILayout.Label(noExistingChannelString);
        }

        UpdateDrawProperty();
    }

    private void OnDestroy()
    {
#if JAYCE_ANALYSIS_HELPER
        SaveDrawPropertyToDisk();
#endif
    }
}