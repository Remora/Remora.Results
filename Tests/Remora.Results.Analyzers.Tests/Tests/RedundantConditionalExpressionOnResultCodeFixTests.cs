//
//  RedundantConditionalExpressionOnResultCodeFixTests.cs
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
using Microsoft.CodeAnalysis.Testing;
using Remora.Results.Analyzers.CodeFixes;
using Remora.Results.Analyzers.Tests.TestBases;
using Xunit;

namespace Remora.Results.Analyzers.Tests;

/// <summary>
/// Tests the code fix for redundant conditional expressions on results.
/// </summary>
public class RedundantConditionalExpressionOnResultCodeFixTests : ResultCodeFixTests<REM0002RedundantConditionalExpressionOnResultAnalyzer, REM0002RedundantConditionalExpressionOnResultCodeFix>
{
    /// <summary>
    /// Tests whether the code fix replaces the entire conditional with the name of the result in the error arm.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplacesConditionalWithArgument()
    {
        this.CompilerDiagnostics = CompilerDiagnostics.None;

        this.TestCode =
        @"
            using Remora.Results;

            public class Program
            {
                public static void Main()
                {
                    Result b = default;

                    var result = b.IsSuccess
                        ? Result.FromSuccess()
                        : b;
                }
            }
        ";

        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM0002").WithSpan(10, 34, 12, 28).WithArguments("b"));

        this.FixedCode =
        @"
            using Remora.Results;

            public class Program
            {
                public static void Main()
                {
                    Result b = default;

                    var result = b;
                }
            }
        ";

        await RunAsync();
    }
}
