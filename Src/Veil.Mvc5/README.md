# Veil.Mvc5

A ViewEngine for [ASP.NET MVC](http://asp.net/) which detects installed Veil parsers and uses them to renders views with matching extensions.

Get it on nuget `Install-Package Veil.Mvc5`

Register it in `Global.asax`

````
protected void Application_Start() {
	// Register routes etc

	ViewEngines.Engines.Add(new VeilViewEngine());
}
````


**Note:**  
In addition to installing this package, you need to install one or more syntax parsers for Veil.

* [Veil.Handlebars](https://github.com/csainty/Veil/tree/master/Src/Veil.Handlebars)
* [Veil.SuperSimple](https://github.com/csainty/Veil/tree/master/Src/Veil.SuperSimple)