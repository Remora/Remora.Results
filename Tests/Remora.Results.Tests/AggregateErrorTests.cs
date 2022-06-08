//
//  AggregateErrorTests.cs
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
/// Tests the <see cref="AggregateError"/> record.
/// </summary>
public class AggregateErrorTests
{
    /// <summary>
    /// Tests the <see cref="AggregateError.ToString"/> method.
    /// </summary>
    public new class ToString
    {
        /// <summary>
        /// Tests whether the method prints all inner errors.
        /// </summary>
        [Fact]
        public void PrintsAllInnerErrors()
        {
            Result error1 = new InvalidOperationError();

            // ReSharper disable once NotResolvedInText
            Result error2 = new ArgumentInvalidError("myArgument", "You can't cut back on errors! You'll regret this!");
            Result error3 = new NotFoundError();
            Result error4;
            try
            {
                throw new NotSupportedException("Rude!");
            }
            catch (Exception e)
            {
                error4 = e;
            }

            var aggregate = new AggregateError(error1, error2, error3, error4);
            var errorString = aggregate.ToString();
            Assert.Equal($"AggregateError: One or more errors occurred.\n[0]: \tInvalidOperationError {{ Message = The requested operation is invalid. }}\n[1]: \tArgumentInvalidError {{ Message = Error in argument myArgument: You can't cut back on errors! You'll regret this!, Name = myArgument }}\n[2]: \tNotFoundError {{ Message = The searched-for entity was not found. }}\n[3]: \tExceptionError {{ Message = Rude!, Exception = System.NotSupportedException: Rude!\n\t{((ExceptionError)error4.Error!).Exception.StackTrace} }}\n", errorString);
        }
    }
}
