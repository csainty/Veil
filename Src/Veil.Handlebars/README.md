# Veil.Handlebars

A [Handlebars](http://handlebarsjs.com/) inspired syntax parser for [Veil](https://github.com/csainty/Veil).

### Syntax
Expressions in Handlebars are wrapped in {{ and }}.

### Accessing your model

Access to your model is implicit in any handlebars expression.

````
var model = new {
	Name = "Bob",
	Department = new {
		Name = "Sector 7G"
	}
}
````  
Hello `{{ Name }}` - Hello Bob  
From `{{ Department.Name }}` - From Sector 7G 

Handlebars supports referencing the model of the parent scope with the `../` expression.

````
var model = new {
	Name = "Bob",
	Roles = new [] { "Admin", "User" }
}
````  
`{{#each Roles}}`Hello `{{../Name}}` the `{{this}}`! `{{/each}}` - Hello Bob the Admin! Hello Bob the User! 

All handlebars expressions are HTML-escape by default. To disable this functionality you should wrap your expression with three braces instead.   
````
var model = new {
	Content = "Safe <b>Markup</b>"
};
````  
See my `{{{ Content }}}` - See my Safe <b>Markup</b>

### {{#if}} {{else}} {{/if}}
Conditionals are handled with the `{{#if}}` expression. E.g.  
````
var model = new {
	Name = "Bob",
	IsAdmin = false
};
````  
`@Model.Name` is `{{#if IsAdmin }}`an Admin`{{ else }}`a User`{{/if}}` - Bob is a User  
In additional to boolean values, conditionals also support null-checking reference types. 

### {{#each}} {{/each}}
Iteration is handled with the `{{#each}}` expression. Access to the current item in the iteration is provided through the `{{this}}` expression. E.g.  
````
var model = new {
	Items = new [] { "Cat", "Dog" },
	Users = new [] {
		new User { Name = "Jim" },
		new User { Name = "Bob" )
	}
};
````  
`{{#each Items}}{{this}}{{/each}}` - CatDog  
`{{#each Users}}{{this.Name}}{{/each}}` - JimBob

### {{#flush}}
Veil supports early flushing rendered content. Doing this allows the browser to start loading external assets such as CSS, JavaScript and images before the full page is loaded. You can trigger this anywhere in your templates with the `{{#flush}}` expression.

### Partials - {{> partialName }}
Including another template is handled through the `{{> }}` expression. Partials are loaded through the [IVeilContext](https://github.com/csainty/Veil/blob/master/Src/Veil/IVeilContext.cs) provdided to your [VeilEngine](https://github.com/csainty/Veil/blob/master/Src/Veil/VeilEngine.cs) instance. If you are executing without an `IVeilContext` then attempts to load partials will throw an exception.

````
var model = new {
	User = new {
		Name = "Bob"
	},
	Department = new {
		Name = "Sector 7G"
	}
};
var userTemplate = "Hello {{ User.Name }}";
var deptTemplate = "From {{ Department.Name }}"; 
````  
`{{> userTemplate }} {{> deptTemplate }} - Hello Bob From Sector 7G`

Partials always inherit the current model context.

### {{! comments }}
You can add comments to your template with the `{{! your comment here }}` expression.  
These are simply ignored and removed during compilation.