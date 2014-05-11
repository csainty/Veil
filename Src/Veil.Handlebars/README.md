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