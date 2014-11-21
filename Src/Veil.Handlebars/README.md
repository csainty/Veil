# Veil.Handlebars

A [Handlebars](http://handlebarsjs.com/) inspired syntax parser for [Veil](https://github.com/csainty/Veil).

Get it on nuget `Install-Package Veil.Handlebars`

The Handlebars parser is registered under the keys `handlebars` and `hbs`.

You can experiment with the syntax online - [http://tryveil.com](http://tryveil.com)

### Syntax
Expressions in Handlebars are wrapped in {{ and }}.

### Access your model - {{ }}

Access to your model is implicit in any handlebars expression.

````
var model = new {
	Name = "Bob",
	Department = new {
		Name = "Sector 7G"
	}
}
````  
`Hello {{ Name }}` - Hello Bob  
`From {{ Department.Name }}` - From Sector 7G 

Handlebars supports referencing the model of the parent scope with the `../` expression.

````
var model = new {
	Name = "Bob",
	Roles = new [] { "Admin", "User" }
}
````  
`{{#each Roles}}Hello {{../Name}} the {{this}}! {{/each}}` - Hello Bob the Admin! Hello Bob the User! 

### Disable HTML Escape - {{{ }}}

All handlebars expressions are HTML-escape by default. To disable this functionality you should wrap your expression with three braces instead.   
````
var model = new {
	Content = "Safe <b>Markup</b>"
};
````  
`See my {{{ Content }}}` - See my Safe <b>Markup</b>

### Conditionals - {{#if}} {{else}} {{/if}}
Conditionals are handled with the `{{#if}}` expression. E.g.  
````
var model = new {
	Name = "Bob",
	IsAdmin = false
};
````  
`@Model.Name is {{#if IsAdmin }}an Admin{{ else }}a User{{/if}}` - Bob is a User  
In additional to boolean values, conditionals also support null-checking reference types. 

### Conditionals - {{#unless}} {{/unless}}
An alternate form of conditional exists that renders its content when the expression evaluates false.  

````
var model = new {
	IsAdmin = false
};
````  
`{{#unless IsAdmin }}Please Login{{/unless}}` - Please login  


### Iteration - {{#each}} {{else}} {{/each}}
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

If an `{{#each}}` block contains an `{{else}}` then that content will be rendered when there are not items in the collection being iterated.

````
var model = new {
	Items = new string[0]
};
````  
`{{#each Items}}{{this}}{{else}}NoItems{{/each}}` - NoItems  

### Scope - {{#with}}
You can scope a block in Handlebars with the `{{#with Name}} {{/with}}` expression. Any expressions within this scope block will use the object referenced by the block as their model.
````  
var model = new {
	User = new {
		Name = "Joe",
		Id = 1
	}
}; 
````  
`{{#with User}} {{Id}}: {{Name}}{{/with}}` - 1: Joe

### Response Flush - {{#flush}}
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

### Master pages - {{< masterName}} / {{body}}
Handlebars supports single-sectioned master pages using the `{{< masterName}}` expression. The named template will be loaded and the rest of the content from the original template will be inserted into the master template in place of the `{{body}}` expression.  

````
var model = new {
	Name = "Joe"
};
var master = "Hello {{body}}, Have Fun!"; 
````  
`{{< master}}{{Name}} - Hello Joe, Have Fun!`


### Comments {{! ... }}
You can add comments to your template with the `{{! your comment here }}` expression.  
These are simply ignored and removed during compilation.

### Whitespace control - {{~Foo~}}
Handlebars supports selectively trimming templates whitespace by adding `~` markers in your expressions.

````  
var model = { Name = "Joe" };

<p>
	{{~Name~}}
</p>

<p>Joe</p>
````

Placing a `~` at the start of the block trim the whitespace preceeding the block. Placing a `~` at the end of a block trims whitespace following the block.  
Use the features to generate smaller markup for sending over the wire.