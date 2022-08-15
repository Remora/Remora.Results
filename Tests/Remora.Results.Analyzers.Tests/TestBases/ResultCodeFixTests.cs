//
//  ResultCodeFixTests.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System.IO;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Remora.Results.Analyzers.Tests.TestBases;

/// <summary>
/// Serves as a base class for result-targeted analyzer tests.
/// </summary>
/// <typeparam name="TAnalyzer">The analyzer under test.</typeparam>
/// <typeparam name="TCodeFix">The code fix under test.</typeparam>
public abstract class ResultCodeFixTests<TAnalyzer, TCodeFix> : CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultCodeFixTests{TAnalyzer, TCodeFix}"/> class.
    /// </summary>
    protected ResultCodeFixTests()
    {
        #if NET6_0
        this.ReferenceAssemblies = new ReferenceAssemblies
        (
            "net6.0",
            new PackageIdentity("Microsoft.NETCore.App.Ref", "6.0.0"),
            Path.Combine("ref", "net6.0")
        );
        #else
        this.ReferenceAssemblies = ReferenceAssemblies.Net.Net50;
        #endif
        this.TestState.AdditionalReferences.Add(typeof(Result).Assembly);
    }
}
