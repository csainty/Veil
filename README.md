# Veil

----------
Veil is a prototype .NET template renderer / view engine. It is designed as a tool chain that can allow multiple syntax parsers to sit on top of the compiler.

### Design Goals

* Easy to install, configure and use in any project
* Cycle and Memory efficiency
* Mono support
* Support for explicit flush to aid in browser perf.
* Support multiple syntax parsers. Drop-in replacement for existing engines


### Target View Features
* Generic Models
* Automatic property conversion to string
* Support for encoding or escaping output
* Master pages
* Partials / Includes
* Conditional Logic
* Loops / Iterators

### Target Performance Features
* Compiles to MSIL with focus on compile once execute many
* Inline partials and master pages
* Handle literal based conditionals and iterations at compile time


### Not supported or not prioritised at this time
* Arbitrary code in templates
* Multi-region master pages
