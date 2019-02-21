# BananaCake

## Syntax

### Statements

A statement can be
- an assignment, e.g. `a = 5`
  - The left hand side of the assignment must be an lValue
  - The right hand side of the assignment must be an rValue
- a declaration, e.g. `int a`
- a declaring assignment, e.g. `int a = 5`
  - Same rules as apply as for assignments

#### rValue

An expression that evaluates to a value, e.g. `5 + 6 * 7`.

#### lValue

An expression that evaluates to an object that has an assignment operation, e.g. `person.name`.

### Access levels

Access levels define which other modules will be able to access a symbol. There are the following access levels:
- `public` - anyone can access the symbol
- `protected` - only submodules of the symbol or it's derivatives can access it.
- `private` - only submodules of the symbol can access it.

### Namespaces

Namespaces encapsulate classes, enums and other types. They can be used to add structure to your code and make it more readable. A namespace is defined as followed:

```
[access] namespace ns {
    // ...
}
```

Every symbol defined inside the namespace can only be accessed by either
- prefixing the namespace before the symbols name, separated by a dot, e.g. `ns.DemoClass`
- importing the namespace at the beginning of the file: `use ns` and then accessing the type as if it was not inside of a namespace. Note that this can lead to ambiguity if two types with equal names are defined in two different imported namespaces. In this case, you will have to use the previous method never the less.

If an access level is provided, all contained symbols will default to the specified access level, unless stated differently. Default is `public`.

### Classes

Classes work just like in any other OO language and can be defined as follows:

```
[access] class DemoClass {
    
}
```

By providing an access level, it can be restricted who may use or instantiate the class.