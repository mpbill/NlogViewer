[1]: http://dotnetsolutionsbytomi.blogspot.se/2011/06/creating-awesome-logging-control-with.html
[nuget]: https://nuget.org/packages/NlogViewer/

NlogViewerWithFilterScroll
================
This is my fork of erizet/NlogViewer with the following additions:
* Font and Color scheme for easy diferentiation of LogLevels
* added LogCount DependancyProperty allowing you to set your own MaxLogCount
* Filter option for LogLevels and HasException
* AutoScroll property which if set to true, will scroll new LogItems into view.

NlogViewer
==========

NlogViewer is a simple WPF-control to show NLog-logs. It's heavily inspired by [this blog][1].

##How to use?##

Add a namespace to your Window, like this:

        xmlns:nlog ="clr-namespace:NlogViewer;assembly=NlogViewer"

then add the control.

        <nlog:NlogViewer x:Name="logCtrl" /> 

To setup NlogViewer as a target, add the following to your Nlog.config.

```xml
  <extensions>
    <add assembly="NlogViewer" />
  </extensions>
  <targets>
    <target xsi:type="NlogViewer" name="ctrl" />
  </targets>
  <rules>
    <logger name="*" minlevel="Trace" writeTo="ctrl" />
  </rules>
```

##Nuget##

A NuGet-package is available [here][nuget]. It will try to install the control and a sample Nlog.config.
