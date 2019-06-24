# Jayce-s-ValueObserver
When you want to go tracking some values , This will help you.

Let's say you want to track down this value

int i01;

Then you first register a channel so we can distinguish groups of
values we are interested in.

<b>JayceGUIAnalysisHelper.Instance.RegisterChannel("C1");</b>

And you register a key for the helper class to know which one is which.

<b>JayceGUIAnalysisHelper.Instance.AddKey("C1", "IntValue01");</b>


And you will be able to see the value ,

but when the value is updated you better let the class know that.

So let's register the new value . 

<b>JayceGUIAnalysisHelper.Instance.SetValue("C1", "IntValue01", i01);</b>




You do not have to deal with extra options with coding.

It will appear on the screen as a toggle or slider just enjoy !