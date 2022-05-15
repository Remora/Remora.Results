//
//  NullabilityTests.cs
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

using Xunit;

namespace Remora.Results.Tests;

/// <summary>
/// Tests various nullability conditions.
/// </summary>
public class NullabilityTests
{
    /// <summary>
    /// Tests whether a value of <value>null</value> can be converted into a successful result.
    /// </summary>
    [Fact]
    public void CreatesSuccessfulResultFromNullIfEntityIsNullableStruct()
    {
        Result<int?> result = (int?)null;
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Tests whether a value of <value>null</value> can be converted into a successful result.
    /// </summary>
    [Fact]
    public void CreatesSuccessfulResultFromNullIfEntityIsNullableClass()
    {
        Result<string?> result = (string?)null;
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Tests whether a value can be converted into a successful result.
    /// </summary>
    [Fact]
    public void CreatesSuccessfulResultFromValueIfEntityIsNullableStruct()
    {
        Result<int?> result = 1;
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Tests whether a value can be converted into a successful result.
    /// </summary>
    [Fact]
    public void CreatesSuccessfulResultFromValueIfEntityIsNullableClass()
    {
        Result<string?> result = "wooga";
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// This method doesn't actually test anything at runtime, but rather serves to expose a variety of nullability
    /// conditions. Consider the test as having failed if nullability warnings prevents the solution from building.
    /// </summary>
    [Fact]
    public void HandlesNullabilityAnnotationsCorrectly()
    {
        // Successful value type results
        Result<int> integerValue = 1;
        Result<int?> nullNullableInteger = (int?)null;
        Result<int?> nonNullNullableInteger = 1;

        // Unsuccessful value type results
        Result<int> failedInteger = new InvalidOperationError();
        Result<int?> failedNullableInteger = new InvalidOperationError();

        // Successful reference type results
        Result<string> stringValue = string.Empty;
        Result<string?> nullNullableString = (string?)null;
        Result<string?> nonNullNullableString = string.Empty;

        // Unsuccessful reference type results
        Result<string> failedString = new InvalidOperationError();
        Result<string?> failedNullableString = new InvalidOperationError();

        // Actual tests

        // Valid access to value types
        if (integerValue.IsSuccess)
        {
            Assert.NotNull(integerValue.Entity.ToString());
        }

        if (nullNullableInteger.IsSuccess)
        {
            Assert.Null(nullNullableInteger.Entity?.ToString());
        }

        if (nonNullNullableInteger.IsSuccess)
        {
            Assert.NotNull(nullNullableInteger.Entity.ToString());
        }

        // Composite access to value types
        if (nonNullNullableInteger is { IsSuccess: true, Entity: not null })
        {
            Assert.NotNull(nonNullNullableInteger.Entity.Value.ToString());
        }

        // Definition access
        if (nullNullableInteger.IsDefined())
        {
            Assert.NotNull(nullNullableInteger.Entity.Value.ToString());
        }
        else
        {
            Assert.Null(nullNullableInteger.Entity?.ToString());
        }

        if (nonNullNullableInteger.IsDefined())
        {
            Assert.NotNull(nonNullNullableInteger.Entity.Value.ToString());
        }
        else
        {
            Assert.Null(nonNullNullableInteger.Entity?.ToString());
        }

        // Invalid access to value types
        Assert.Equal(default, failedInteger.Entity);
        Assert.Equal(default, failedNullableInteger.Entity);

        // Valid access to reference types
        if (stringValue.IsSuccess)
        {
            Assert.NotNull(stringValue.Entity.Trim());
        }

        if (nullNullableString.IsSuccess)
        {
            Assert.Null(nullNullableString.Entity?.Trim());
        }

        if (nonNullNullableString.IsSuccess)
        {
            Assert.NotNull(nonNullNullableString.Entity);
        }

        // Composite access to reference types
        if (nonNullNullableString is { IsSuccess: true, Entity: not null })
        {
            Assert.NotNull(nonNullNullableString.Entity.Trim());
        }

        if (nullNullableString.IsDefined())
        {
            Assert.NotNull(nullNullableString.Entity.Trim());
        }
        else
        {
            Assert.Null(nullNullableString.Entity?.Trim());
        }

        if (nonNullNullableString.IsDefined())
        {
            Assert.NotNull(nonNullNullableString.Entity.Trim());
        }
        else
        {
            Assert.Null(nonNullNullableString.Entity?.Trim());
        }

        // Invalid access to reference types
        Assert.Equal(default, failedString.Entity);
        Assert.Equal(default, failedNullableString.Entity);
    }
}
