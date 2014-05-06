# Veil

----------
Veil is a .NET template renderer / view engine. It is designed as a tool chain that can allow multiple syntax parsers to sit on top of a single IL-emitting compiler.

### Design Goals

* Fastest view engine available
* Easy to install, configure and use in any project
* Mono support
* Support for explicit flush to aid in browser performance
* Support multiple template parsers e.g. SuperSimple, Handlebars and more to come

### Not supported
Unlike Razor, Veil templates are not compiled to full .NET assemblies. This is part of what makes Veil so much easier to integrate and work with. The cost of this approach is that arbitrary code blocks are not supported.  
A purist may argue this is actually a goog thing :) 


### Getting Started
Until nuget packages are released, you can grab pre-compiled binaries from [https://ci.appveyor.com/project/csainty/veil/build/artifacts](https://ci.appveyor.com/project/csainty/veil/build/artifacts)  
You have two options for using Veil :-

1. Install a ViewEngine module in to your existing web framework. e.g. `Nancy.ViewEngines.Veils.SuperSimple`
2. Use `VeilEngine` directly

````
// Register your parsers
VeilEngine.RegisterParser("handlebars", new HandlebarsParser());

--

// Compile your template once with the chosen parser
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