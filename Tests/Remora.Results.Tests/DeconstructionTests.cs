//
//  DeconstructionTests.cs
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

using Xunit;

namespace Remora.Results.Tests;

/// <summary>
/// Tests various deconstruction methods.
/// </summary>
public class DeconstructionTests
{
    /// <summary>
    /// Tests the 3-parameter deconstruct method.
    /// </summary>
    public class Deconstruct3
    {
        /// <summary>
        /// Tests whether <see cref="IResult{T}.IsSuccess"/> is correctly deconstructed.
        /// </summary>
        [Fact]
        public void DeconstructsIsSuccessCorrectly()
        {
            Result<int> result = 1;

            var (isSuccess, _, _) = result;
            Assert.True(isSuccess);
        }

        /// <summary>
        /// Tests whether <see cref="IResult{T}.Entity"/> is correctly deconstructed.
        /// </summary>
        [Fact]
        public void DeconstructsEntityCorrectly()
        {
            var expected = 1;
            Result<int> result = expected;

            var (_, _, actual) = result;
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Tests whether <see cref="IResult{T}.Entity"/> is correctly deconstructed.
        /// </summary>
        [Fact]
        public void DeconstructsErrorCorrectly()
        {
            var expected = new NotFoundError();
            Result<int> result = expected;

            var (_, actual, _) = result;
            Assert.Equal(expected, actual);
        }
    }

    /// <summary>
    /// Tests the 2-parameter deconstruct method.
    /// </summary>
    public class Deconstruct2
    {
        /// <summary>
        /// Tests whether <see cref="IResult{T}.IsSuccess"/> is correctly deconstructed.
        /// </summary>
        [Fact]
        public void DeconstructsValueIsSuccessCorrectly()
        {
            Result<int> result = 1;

            var (isSuccess, _) = result;
            Assert.True(isSuccess);
        }

        /// <summary>
        /// Tests whether <see cref="IResult{T}.Entity"/> is correctly deconstructed.
        /// </summary>
        [Fact]
        public void DeconstructsValueEntityCorrectly()
        {
            var expected = 1;
            Result<int> result = expected;

            var (_, actual) = result;
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Tests whether <see cref="IResult{T}.IsSuccess"/> is correctly deconstructed.
        /// </summary>
        [Fact]
        public void DeconstructsIsSuccessCorrectly()
        {
            var result = Result.FromSuccess();

            var (isSuccess, _) = result;
            Assert.True(isSuccess);
        }

        /// <summary>
        /// Tests whether <see cref="IResult{T}.Entity"/> is correctly deconstructed.
        /// </summary>
        [Fact]
        public void DeconstructsErrorCorrectly()
        {
            var expected = new NotFoundError();
            Result result = expected;

            var (_, actual) = result;
            Assert.Equal(expected, actual);
        }
    }

    /// <summary>
    /// Tests compatibility with pattern matching.
    /// </summary>
    public class SwitchPatternMatching
    {
        /// <summary>
        /// Tests whether a failed <see cref="Result{T}"/> can be matched with a switch pattern.
        /// </summary>
        [Fact]
        public void CanMatchValueError()
        {
            Result<int> result = new NotFoundError();

            var pass = result switch
            {
                // ReSharper disable once UnusedVariable
                (true, var value) => false,

                // ReSharper disable once UnusedVariable
                (false, _, var error) => true,

                _ => false
            };

            Assert.True(pass);
        }

        /// <summary>
        /// Tests whether a failed <see cref="Result{T}"/> can be matched with a switch pattern.
        /// </summary>
        [Fact]
        public void CanMatchError()
        {
            Result result = new NotFoundError();

            var pass = result switch
            {
                // ReSharper disable once UnusedVariable
                (true, var error) => false,

                // ReSharper disable once UnusedVariable
                (false, var error) => true
            };

            Assert.True(pass);
        }

        /// <summary>
        /// Tests whether a failed <see cref="Result{T}"/> can be matched with a switch pattern.
        /// </summary>
        [Fact]
        public void CanMatchSuccess()
        {
            var result = Result.FromSuccess();

            var pass = result switch
            {
                // ReSharper disable once UnusedVariable
                (true, var error) => true,

                // ReSharper disable once UnusedVariable
                (false, var error) => false
            };

            Assert.True(pass);
        }
    }
}
