//
//  ExceptionErrorTests.cs
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

using System;
using Xunit;

namespace Remora.Results.Tests;

/// <summary>
/// Tests the <see cref="ExceptionError"/> record.
/// </summary>
public class ExceptionErrorTests
{
    /// <summary>
    /// Tests whether the <see cref="ExceptionError.Exception"/> property returns the correct object.
    /// </summary>
    [Fact]
    public void ExceptionReturnsCorrectObject()
    {
        var exception = new Exception();
        var error = new ExceptionError(exception);

        Assert.Equal(exception, error.Exception);
    }

    /// <summary>
    /// Tests whether the <see cref="ResultError.Message"/> is simply forwarded from the exception by default.
    /// </summary>
    [Fact]
    public void ExceptionErrorForwardsExceptionMessageByDefault()
    {
        var exception = new Exception("Wooga");
        var error = new ExceptionError(exception);

        Assert.Equal(exception.Message, error.Message);
    }

    /// <summary>
    /// Tests whether the <see cref="ResultError.Message"/> is simply forwarded from the exception by default.
    /// </summary>
    [Fact]
    public void ExceptionErrorUsesProvidedMessageIfAvailable()
    {
        var exception = new Exception("Wooga");
        var error = new ExceptionError(exception, "Booga");

        Assert.NotEqual(exception.Message, error.Message);
    }
}
