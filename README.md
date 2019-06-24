# Jayce-s-ValueObserver

When you want to watch some values by certain group ? 


Let's say you are interested in

<fontsize=30><color=#bb00bb><b>int m_n;</b></color></fontsize>

You can simply write

<fontsize=30><color=#bb00bb><b>JayceGUIAnalysisHelper.Instance.SetValue("Channel01", "Value01", m_n);</b></color></fontsize>

It will update the value by specified by the channel and key internally and display it.

when you want to count the number of executions .

<fontsize=30><color=#bb00bb><b>JayceGUIAnalysisHelper.Instance.AdjustValue("CountChannel", "Int01", 1);</b></color></fontsize>
