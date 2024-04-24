//
//  UnusedResultAnalyzerTests.cs
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
/// Tests the <see cref="REM0003UnusedResultAnalyzer"/> analyzer.
/// </summary>
public class UnusedResultAnalyzerTests : ResultAnalyzerTests<REM0003UnusedResultAnalyzer>
{
    /// <summary>
    /// Tests that the analyzer raises a warning when a Result of T is unused.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForUnusedResultOfT()
    {
        this.TestCode =
            @"
            using System;
            using Remora.Results;

            public class Program
            {
                public Result<int> MyMethod() => 1;

                public void Main()
                {
                    MyMethod();
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM0003")
            .WithSpan(11, 21, 11, 31)
            .WithArguments("MyMethod"));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer raises a warning when a Result of T is unused.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForUnusedResult()
    {
        this.TestCode =
            @"
            using System;
            using Remora.Results;

            public class Program
            {
                public Result MyMethod() => Result.FromSuccess();

                public void Main()
                {
                    MyMethod();
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM0003")
            .WithSpan(11, 21, 11, 31)
            .WithArguments("MyMethod"));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer doesn't raise a warning when Result of T is used.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task IgnoresUsedResultOfTWithArrowSyntax()
    {
        this.TestCode =
            @"
            using System;
            using Remora.Results;

            public class Program
            {
                public Result<int> MyMethod() => 1;

                public void Main()
                {
                    var result = MyMethod();
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer doesn't raise a warning when Result of T is used.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task IgnoresUsedResultOfT()
    {
        this.TestCode =
            @"
            using System;
            using Remora.Results;

            public class Program
            {
                public Result<int> MyMethod()
                {
                    return 1;
                }

                public void Main()
                {
                    var result = MyMethod();
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer doesn't raise a warning when Result is used.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task IgnoresUsedResultWithArrowSyntax()
    {
        this.TestCode =
            @"
            using System;
            using Remora.Results;

            public class Program
            {
                public Result MyMethod() => Result.FromSuccess();

                public void Main()
                {
                    var result = MyMethod();
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer doesn't raise a warning when Result is used.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task IgnoresUsedResult()
    {
        this.TestCode =
            @"
            using System;
            using Remora.Results;

            public class Program
            {
                public Result MyMethod()
                {
                    return Result.FromSuccess();
                }

                public void Main()
                {
                    var result = MyMethod();
                }
            }
        ";

        this.ExpectedDiagnostics.Clear();

        await RunAsync();
    }
}
