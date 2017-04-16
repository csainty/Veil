# Veil.SuperSimple

A [SuperSimpleViewEngine](https://github.com/grumpydev/SuperSimpleViewEngine) inspired syntax parser for [Veil](https://github.com/csainty/Veil).

Get it on nuget `Install-Package Veil.SuperSimple`

The SuperSimple parser is registered under the keys `supersimple`, `sshtml` and `vsshtml`.

You can experiment with the syntax online - [http://tryveil.com](http://tryveil.com)


### Syntax
Expressions in SuperSimple syntax all start with an `@` and are optionally terminated with a `;`. E.g. `@Model.Name` and `@Model.Name;` are equivalent. If your expressions has a natural separator such as a space, then the terminator is not needed.

### @Model.*

Access to your model is provided by the `@Model` expression. E.g.  
````
var model = new {
	Name = "Bob",
	Department = new {
		Name = "Sector 7G"
	}
}
````  
Hello `@Model.Name` - Hello Bob  
From `@Model.Department.Name` - From Sector 7G 

An expression of simply `@Model` references the model itself. E.g.  
````
var model = "World";
````  
Hello `@Model` - Hello World

To HTML-escape an expression, prefix it with a `!`  
````
var model = new {
	Content = "Dangerous <script>"
};
````  
See my `@!Model.Content` - See my Dangerous &amp;lt;script&amp;gt;

### @If.* / @IfNot.*
Conditionals are handled with the `@If` and `@IfNot` expressions. E.g.  
````
var model = new {
	Name = "Bob",
	IsAdmin = false
};
````  
`@Model.Name` is `@If.IsAdmin;`an Admin`@EndIf;``@IfNot.IsAdmin`a User`@EndIf` - Bob is a User  
In additional to boolean values, conditionals also support null-checking reference types. 

### @IfNull.* / @IfNotNull.*
Aliases exist for the `@IfNull` and `@IfNotNull` expressions. These are mapped to regular Veil conditionals which support null checking for non-booleans.

### @Each.* / @Current.*
Iteration is handled with the `@Each` expressions. Access to the current item in the iteration is provided through the `@Current` expression which supports the same syntax as `@Model` E.g.  
````
var model = new {
	Items = new [] { "Cat", "Dog" },
	Users = new [] {
		new User { Name = "Jim" },
		new User { Name = "Bob" )
	}
};
````  
`@Each.Items;@Current;@EndEach;` - CatDog  
`@Each.Users;@Current.Name;@EndEach;` - JimBob

### @Partial
Including another template is handled through the `@Partial` expression. Partials are loaded through the [IVeilContext](https://github.com/csainty/Veil/blob/master/Src/Veil/IVeilContext.cs) provdided to your [VeilEngine](https://github.com/csainty/Veil/blob/master/Src/Veil/VeilEngine.cs) instance. If you are executing without an `IVeilContext` then attempts to load partials will throw an exception.

````
var model = new {
	User = new {
		Name = "Bob"
	},
	Department = new {
		Name = "Sector 7G"
	}
};
var userTemplate = "Hello @Model.Name";
var deptTemplate = "From @Model.Name"; 
````  
`@Partial['userTemplate', User] @Partial['deptTemplate', Department]` - Hello Bob From Sector 7G

If you do not provided a second value `@Partial['Name']` then the whole model is passed to the partial.

### @Master and @Section
Master pages are handled through the `@Master` and `@Section` expressions.  
A master page is simply a template that has one or more names sections in it. E.g.  
````
// MyMasterPage.vsshtml
My header
@Section['Content']
My Footer
````  
To use this master page, you need to create a template where the first expression indicates to use this master page and then define each section.  
````
@Master['MyMasterPage']

@Section['Content']
My Content
@EndSection
````

The model you pass to your template is also passed to the MasterPage and can be accessed, You can also nest master pages many levels deep.  
You can not reuse section names in nested templates, if you wish to "inherit" a section you need to rename it in your intermediary master page.

### @Flush
Veil supports early flushing rendered content. Doing this allows the browser to start loading external assets such as CSS, JavaScript and images before the full page is loaded. You can trigger this anywhere in your templates with the `@Flush` expression.