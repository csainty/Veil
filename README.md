# Veil

----------
Veil is a .NET template renderer / view engine. It is designed as a tool chain that can allow multiple syntax parsers to sit on top of the compiler.

### Design Goals

* Fastest view engine available
* Easy to install, configure and use in any project
* Mono support
* Support for explicit flush to aid in browser perf.
* Support multiple syntax parsers.

### Not supported
* Arbitrary code in templates


### Getting Started
There a no nuget packages yet, but these will come.  
You have two options for using Veil.

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