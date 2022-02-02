# Jayce-s-ValueObserver

When you want to watch some values by certain group ? 


Let's say you are interested in

<b>int m_n;</b>

You can simply write

<b>JayceGUIAnalysisHelper.Instance.SetValue("Channel01", "Value01", m_n);</b>

It will update the value by specified by the channel and key internally and display it.

when you want to count the number of executions .

<b>JayceGUIAnalysisHelper.Instance.AdjustValue("CountChannel", "Int01", 1);</b>
