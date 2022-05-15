//
//  RedundantFromErrorCallCodeFixTests.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Remora.Results.Analyzers.CodeFixes;
using Xunit;

namespace Remora.Results.Analyzers.Tests;

/// <summary>
/// Tests the code fix for redundant FromError calls.
/// </summary>
public class RedundantFromErrorCallCodeFixTests : CSharpCodeFixTest<REM0001RedundantFromErrorCallAnalyzer, REM0001RedundantFromErrorCallCodeFix, XUnitVerifier>
{
    /// <summary>
    /// Tests whether the code fix replaces the entire invocation with the name of the result in the argument.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplacesCallWithArgument()
    {
        this.CompilerDiagnostics = CompilerDiagnostics.None;

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

        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM0001").WithSpan(9, 21, 9, 46).WithArguments("b"));

        this.FixedCode =
        @"
            struct Result { public static void FromError(object obj) { } public object Error; }

            public class Program
            {
                public static void Main()
                {
                    Result b;
                    b;
                }
            }
        ";

        await RunAsync();
    }
}
