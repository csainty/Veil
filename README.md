# Veil

Veil is a .NET template renderer / view engine. It is designed to support many syntax parsers which sit on top of a single IL-emitting compiler.  
See [Veil.SuperSimple](https://github.com/csainty/Veil/tree/master/Src/Veil.SuperSimple) and [Veil.Handlebars](https://github.com/csainty/Veil/tree/master/Src/Veil.Handlebars) for examples of syntax.

### Design Goals

* Easy to install, configure and use in any project
* Fastest view engine available
* Support for explicit flush to aid in browser performance
* Mono support **(not available yet)**

### Not supported
Unlike Razor, Veil templates are not compiled to full .NET assemblies. This is part of what makes Veil so much easier to integrate and work with. The cost of this approach is that arbitrary code blocks are not supported.  
A purist may argue this is actually a good thing :) 


### Getting Started
Until nuget packages are released, you can grab pre-compiled binaries from [https://ci.appveyor.com/project/csainty/veil/build/artifacts](https://ci.appveyor.com/project/csainty/veil/build/artifacts)  
You have two options for using Veil :-

1. Install a ViewEngine module in to your existing web framework. e.g. [Nancy.ViewEngines.Veils.SuperSimple](https://github.com/csainty/Veil/tree/master/Src/Nancy.ViewEngines.Veil.SuperSimple)
2. Use [VeilEngine](https://github.com/csainty/Veil/blob/master/Src/Veil/IVeilEngine.cs) directly. E.g.

````
// Register your parsers once at startup
VeilEngine.RegisterParser("handlebars", new HandlebarsParser());

--

// Compile each template once with the chosen parser
var template = "Hello {{ Name }}";
var compiledTemplate = new VeilEngine().Compile<ViewModel>("handlebars", new StringReader(template));

--

// Execute your template as needed
using (var writer = new StringWriter()) {
    compiledTemplate(writer, new ViewModel { Name = "Bob" });
}
```` 

### Build Status


Windows - [![Build Status](http://builds.nullreferenceexception.se/app/rest/builds/buildType:id:Veil_Continuos/statusIcon)](http://builds.nullreferenceexception.se/viewType.html?buildTypeId=Veil_Continuos&guest=1)

Mono - [![Build Status](http://builds.nullreferenceexception.se/app/rest/builds/buildType:id:Veil_Continuous_Mono/statusIcon)](http://builds.nullreferenceexception.se/viewType.html?buildTypeId=Veil_Continuous_Mono&guest=1)