# BananaCake

## Syntax

### Operators

| Operator          | Description              |
|-------------------|--------------------------|
| `return`          | Return                   |
| `=`               | Assignment               |
| `> >= < <= == !=` | Comparison               |
| `+ -`             | Arithmetic summation     |
| `* /`             | Arithmetic factorization |
| `as`              | Explicit cast            |
| `new`             | Instantiation            |
| `(`               | Function call            |
| `.`               | Access                   |

Operators are evaluated top down in the order of this list, so `.` binds the strongest and `return` the weakest.

#### return

Returns the result of a following expression from the nearest function (just what you'd expect).

#### Assignment `=`

Assigns the value of an expression on the right hand side to a LValue on the left hand side.

#### Comparison `> >= < <= == !=`

Compares two values for relative equality. To see how comparison operators work for each primitive datatype, go to the individual documentation (see [Datatypes: Primitive](#datatypes-primitive)).

#### Arithmetic summation `+ -`

Do just what you'd expect. Some special behaviours exist, look at the individual documentation for each [primitive datatype](#datatypes-primitive).

#### Arithmetic factorization `* /`

Do just what you'd expect. Some special behaviours exist, look at the individual documentation for each [primitive datatype](#datatypes-primitive).

#### Explicit cast `as`

You can convert symbols to other types by using the `as` operator, e.g.:

```
println(3 as string);
```

You can also cast class types with the `as` operator, however you need to explicitly define how the cast should be done. See [Explicit caster functions](#Explicit-caster-functions).

#### Instantiation `new`

Instantiates a given type by calling its constructor. E.g.
```
var human1 = new Human("Mary", 34);
```

#### Function call `(`

Calls a function with the given parameters.

#### Access `.`

Accesses the property of the result of the left hand side expression.

### Expressions

#### rValue

An expression that evaluates to a value, e.g. `5 + 6 * 7`.

#### lValue

An expression that evaluates to an object that has an assignment operation, e.g. `person.name`.

### Variables

Variables can have a type and contain a value of only this type.
```
int age;
bool correct;
```

Assigning a different type will cause an error:
```
age = true;
correct = 0;
```

### Functions

Functions can either be members of a class, a namespace or just be global. They are defined like this:
```
[<access>] <return-type> <name> ([<parameters>]) {
    // ...
}
```

Access defaults to public and is obsolete when the function is declared globally.
Private functions inside a namespace can only be accessed from within the same namespace.
Private functions inside classes can only be accessed from within the same class.

### Classes

BCake features classes. A class can be declared like this:
```
<access> class <classname> {
    // ...
}
```
e.g.
```
public class Test {}
```

Access levels define which other modules will be able to access a symbol. There are the following access levels:
- `public` - anyone can access the symbol
- `protected` - only submodules of the symbol or it's derivatives can access it.
- `private` - only submodules of the symbol can access it.

#### Constructor

Currently, a class can only have one constructor. It is defined like this:
```
<access> <classname> ([<parameters>]) {
    // ...
}
```

A parameter can be an implicit initializer.
```
private class Human {
    private int age;
    public string name;

    public Human(this.age, this.name) {}
}
```
is equivalent to
```
private class Human {
    private int age;
    public string name;

    public Human(int _age, string _name) {
        age = _age;
        name = _name;
    }
}
```

#### Members

Classes can also have members, e.g.
```
public class Test {
    public int a;
    private bool b;
}
```

The can also have the access levels as seen above.

#### Explicit caster functions

Classes can define how they can be casted to different datatypes by defining a caster function:

```
public class Human {
    public string firstName;
    public string lastName;

    public Human(this.firstName, this.lastName) {}

    public cast string() {
        return firstName + " " + lastName;
    }
}
```

Caster functions must not define parameters. They can have the same access levels all other members can have, that means that they can only be used in the correct scope.

In general:

```
[<access-level>] cast <datatype>() {
    // ...
}
```

### Namespaces

Namespaces encapsulate classes, enums and other types. They can be used to add structure to your code and make it more readable. A namespace is defined as followed:

```
<access> namespace ns {
    // ...
}
```

Every symbol defined inside the namespace can only be accessed by prefixing the namespace before the symbols name, separated by a dot, e.g. `ns.DemoClass`. There is no importing yet, this is subject to change.

If an access level is provided, all contained symbols will default to the specified access level, unless stated differently. Default is `public`.

Classes and functions can be defined outside of a namespace, variables can't.

### Datatypes

#### Primitive
<a name="datatypes-primitive"></a>

| Name | Symbol | Value range |
|-|-|-|
| [Integer](docs/datatypes/INT.md) | `int` | `-2^16 - 1` to `2^16` |
| [Boolean](docs/datatypes/BOOL.md) | `bool` | `true`, `false` |
| [String](docs/datatypes/STRING.md) | `string` | `"strings of variable length"` |