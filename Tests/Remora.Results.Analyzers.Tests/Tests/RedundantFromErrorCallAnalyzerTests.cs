//
//  RedundantFromErrorCallAnalyzerTests.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace Remora.Results.Analyzers.Tests;

/// <summary>
/// Tests the <see cref="REM0001RedundantFromErrorCallAnalyzer"/> analyzer.
/// </summary>
public class RedundantFromErrorCallAnalyzerTests : CSharpAnalyzerTest<REM0001RedundantFromErrorCallAnalyzer, XUnitVerifier>
{
    /// <summary>
    /// Tests that the analyzer ignores code where nothing that looks like a FromError call exists.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task IgnoresCodeWithNoCallsToFromError()
    {
        this.TestCode =
        @"
            using System;

            public class Program
            {
                public static void Main()
                {
                    Console.WriteLine(""Hello world!"");
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer ignores code where nothing that looks like a FromError call exists.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task IgnoresCodeWithCallToNonStaticFromError()
    {
        this.TestCode =
        @"
            public class Program
            {
                public static void Main()
                {
                    Result a;
                    Result b;
                    a.FromError(b);
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0246").WithLocation(6, 21));
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0246").WithLocation(7, 21));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer ignores code where nothing that looks like a FromError call exists.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task IgnoresCodeWithCallToWrongMethod()
    {
        this.TestCode =
        @"
            public class Program
            {
                public static void Main()
                {
                    Result b;
                    Result.FromSomethingElse(b);
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0246").WithLocation(6, 21));
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0103").WithLocation(7, 21));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer ignores code where nothing that looks like a FromError call exists.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForRedundantCode()
    {
        this.TestCode =
        @"
            struct Result { public static void FromError(object obj) { } public object Error; }

            public class Program
            {
                public static void Main()
                {
                    Result b;
                    Result.FromError(b.Error);
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM0001").WithSpan(9, 21, 9, 46).WithArguments("b"));
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0170").WithLocation(9, 38));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer ignores code where nothing that looks like a FromError call exists.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForGenericRedundantCode()
    {
        this.TestCode =
        @"
            struct Result<T> { public static void FromError(object obj) { } public object Error; }

            public class Program
            {
                public static void Main()
                {
                    Result<int> b;
                    Result<int>.FromError(b.Error);
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM0001").WithSpan(9, 21, 9, 51).WithArguments("b"));
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0170").WithLocation(9, 43));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer ignores code where nothing that looks like a FromError call exists.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task IgnoresCodeWithDifferentGenericParameters()
    {
        this.TestCode =
        @"
            struct Result<T> { public static void FromError(object obj) { } public object Error; }

            public class Program
            {
                public static void Main()
                {
                    Result<int> b;
                    Result<string>.FromError(b.Error);
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0170").WithLocation(9, 46));

        await RunAsync();
    }
}
