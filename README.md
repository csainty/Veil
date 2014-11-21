# Veil

Veil is a .NET template renderer / view engine. It is designed to support many syntax parsers which sit on top of a single IL-emitting compiler.  
See [Veil.SuperSimple](https://github.com/csainty/Veil/tree/master/Src/Veil.SuperSimple) and [Veil.Handlebars](https://github.com/csainty/Veil/tree/master/Src/Veil.Handlebars) for examples of supported syntax.

### Design Goals

* Easy to install, configure and use in any project
* Fastest view engine available
* Support for explicit flush to aid in browser performance
* Mono support

### Not supported
Unlike Razor, Veil templates are not compiled with the full .NET compilers. This is part of what makes Veil so much easier to integrate and work with. The cost of this approach is that arbitrary code blocks are not supported.  
A purist may argue this is actually a good thing :) 


### Getting Started
You have two options for using Veil :-

1. If you are using [Nancy](https://github.com/NancyFx/Nancy) then install the [Nancy.ViewEngines.Veil](http://www.nuget.org/packages/Nancy.ViewEngines.Veil) package and your preferred syntax parsers e.g. [Veil.Handlebars](http://www.nuget.org/packages/Veil.Handlebars) or [Veil.SuperSimple](http://www.nuget.org/packages/Veil.SuperSimple)
2. Alternatively you can install and use any Veil syntax parser directly in any application. E.g.

````
Install-Package Veil.Handlebars

// Compile your template once with the chosen parser
var template = "Hello {{ Name }}";
var compiledTemplate = new VeilEngine().Compile<ViewModel>("handlebars", new StringReader(template));

--

// Execute your template as needed
using (var writer = new StringWriter()) {
    compiledTemplate(writer, new ViewModel { Name = "Bob" });
}
```` 

### Further Information

* [Try the Veil parsers in your browser](http://tryveil.com)
* [Getting Started with Nancy](http://blog.csainty.com/2014/06/veil-getting-started-nancy.html)
* [Getting Started Standalone](http://blog.csainty.com/2014/07/veil-getting-started-standalone.html)


### Builds
[![Build status](https://ci.appveyor.com/api/projects/status/cad383bewb58svi1/branch/master?svg=true)](https://ci.appveyor.com/project/csainty/veil/branch/master)

Pre-built binaries of the latest commit are always available at [https://ci.appveyor.com/project/csainty/veil/build/artifacts](https://ci.appveyor.com/project/csainty/veil/build/artifacts)
