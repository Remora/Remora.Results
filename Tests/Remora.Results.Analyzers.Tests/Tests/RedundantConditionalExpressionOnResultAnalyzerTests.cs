//
//  RedundantConditionalExpressionOnResultAnalyzerTests.cs
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

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Remora.Results.Analyzers.Tests.TestBases;
using Xunit;

namespace Remora.Results.Analyzers.Tests;

/// <summary>
/// Tests the <see cref="REM0002RedundantConditionalExpressionOnResultAnalyzer"/> analyzer.
/// </summary>
public class RedundantConditionalExpressionOnResultAnalyzerTests : ResultAnalyzerTests<REM0002RedundantConditionalExpressionOnResultAnalyzer>
{
    /// <summary>
    /// Tests that the analyzer flags happy-path code.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForRedundantCode()
    {
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

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM0002").WithSpan(10, 34, 12, 28).WithArguments("b"));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags happy-path code.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForInvertedRedundantCode()
    {
        this.TestCode =
            @"
            using Remora.Results;

            public class Program
            {
                public static void Main()
                {
                    Result b = default;

                    var result = !b.IsSuccess
                        ? b
                        : Result.FromSuccess();
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM0002").WithSpan(10, 34, 12, 47).WithArguments("b"));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer ignores code that does not contain a conditional expression.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task IgnoresCodeWithNoConditional()
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
    /// Tests that the analyzer does not flag sad-path code.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task IgnoresCodeWithMoreComplexCondition()
    {
        this.TestCode =
        @"
            using Remora.Results;

            public class Program
            {
                public static void Main()
                {
                    Result a = default;
                    Result b = default;

                    var result = a.IsSuccess || b.IsSuccess
                        ? Result.FromSuccess()
                        : b;
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer does not flag sad-path code.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task IgnoresCodeWithNonResultErrorArm()
    {
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
                        : new InvalidOperationError();
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer does not flag sad-path code.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task IgnoresCodeWithDifferentResultInErrorArm()
    {
        this.TestCode =
        @"
            using Remora.Results;

            public class Program
            {
                public static void Main()
                {
                    Result a = default;
                    Result b = default;

                    var result = a.IsSuccess
                        ? Result.FromSuccess()
                        : b;
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();

        await RunAsync();
    }
}
