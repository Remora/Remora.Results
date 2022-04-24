//
//  REM0001RedundantFromErrorCallAnalyzer.cs
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

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Remora.Results.Analyzers;

/// <summary>
/// Detects and flags redundant calls to FromError and its various overloads.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class REM0001RedundantFromErrorCallAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Descriptors.REM0001RedundantFromErrorCall);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis
        (
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics
        );

        context.RegisterSyntaxNodeAction(action: AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation || !context.Node.IsKind(SyntaxKind.InvocationExpression))
        {
            return;
        }

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return;
        }

        if (invocation.ArgumentList.Arguments.Count != 1)
        {
            return;
        }

        // Filter out everything except FromError
        if (memberAccess.Name.Identifier.Text != "FromError")
        {
            return;
        }

        // Filter out everything except Result and Result<T>
        if (memberAccess.Expression is not SimpleNameSyntax memberAccessName)
        {
            return;
        }

        if (memberAccessName.Identifier.Text != "Result")
        {
            return;
        }

        // Okay, we're looking at a Result[<>].FromError(something) call...
        var argument = invocation.ArgumentList.Arguments.Single();
        if (argument.Expression is not MemberAccessExpressionSyntax argumentMemberAccess)
        {
            return;
        }

        if (argumentMemberAccess.Name.Identifier.Text != "Error")
        {
            return;
        }

        if (argumentMemberAccess.Expression is not SimpleNameSyntax argumentMemberAccessName)
        {
            return;
        }

        // Suspicious! Let's compare types...
        var argumentTypeInfo = context.SemanticModel.GetTypeInfo(argumentMemberAccessName);
        var expressionTypeInfo = context.SemanticModel.GetTypeInfo(memberAccessName);

        if (!argumentTypeInfo.Equals(expressionTypeInfo))
        {
            return;
        }

        // Bad!
        context.ReportDiagnostic
        (
            Diagnostic.Create
            (
                descriptor: Descriptors.REM0001RedundantFromErrorCall,
                invocation.GetLocation(),
                messageArgs: argumentMemberAccessName.Identifier.Text
            )
        );
    }
}
