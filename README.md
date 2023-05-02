Remora.Results
==============

Remora.Results is an implementation of a simple algebraic data type for C# - that is, a type which encapsulates the
success or failure of a given operation, as well as an explanation of the failure in the case that there is one.

This library provides a set of common use cases for failure-prone operations, and strongly typed results for them.

## Building
The library does not require anything out of the ordinary to compile.

```bash
cd $SOLUTION_DIR
dotnet build
dotnet pack -c Release
```

## Downloading
Get it on [NuGet][1].


[1]: https://www.nuget.org/packages/Remora.Results/
