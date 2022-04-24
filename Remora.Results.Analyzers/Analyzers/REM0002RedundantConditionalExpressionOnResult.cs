//
//  REM0002RedundantConditionalExpressionOnResult.cs
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
/// Detects and flags redundant ternary expressions on results.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class REM0002RedundantConditionalExpressionOnResult : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Descriptors.REM0002RedundantConditionalExpressionOnResult);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis
        (
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics
        );

        context.RegisterSyntaxNodeAction(action: AnalyzeConditionalExpression, SyntaxKind.ConditionalExpression);
    }

    private void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ConditionalExpressionSyntax conditional)
        {
            return;
        }

        var isFirstArmTheSuccessCase = true;
        MemberAccessExpressionSyntax conditionAccess;

        switch (conditional.Condition)
        {
            case PrefixUnaryExpressionSyntax prefixExpression
                when prefixExpression.IsKind(SyntaxKind.LogicalNotExpression):
            {
                if (prefixExpression.Operand is not MemberAccessExpressionSyntax innerAccess)
                {
                    return;
                }

                isFirstArmTheSuccessCase = false;
                conditionAccess = innerAccess;
                break;
            }
            case MemberAccessExpressionSyntax innerAccess:
            {
                conditionAccess = innerAccess;
                break;
            }
            default:
            {
                return;
            }
        }

        // check type & property of the condition
        var semanticModel = context.SemanticModel;
        var conditionalTypeInfo = semanticModel.GetTypeInfo(conditionAccess.Expression);
        if (conditionalTypeInfo.Type is null || conditionalTypeInfo.Type.Name != "Result")
        {
            return;
        }

        if (conditionAccess.Name.Identifier.Text != "IsSuccess")
        {
            return;
        }

        // Okay, we're looking at some sort of IsSuccess call...
        var successArm = isFirstArmTheSuccessCase ? conditional.WhenTrue : conditional.WhenFalse;
        var errorArm = isFirstArmTheSuccessCase ? conditional.WhenFalse : conditional.WhenTrue;

        if (successArm is not InvocationExpressionSyntax successInvocation)
        {
            return;
        }

        if (successInvocation.ArgumentList.Arguments.Count != 0)
        {
            return;
        }

        if (successInvocation.Expression is not MemberAccessExpressionSyntax successAccess)
        {
            return;
        }

        var successTypeInfo = semanticModel.GetTypeInfo(successAccess.Expression);
        if (successTypeInfo.Type is null || successTypeInfo.Type.Name != "Result")
        {
            return;
        }

        if (successAccess.Name.Identifier.Text != "FromSuccess")
        {
            return;
        }

        if (errorArm is not IdentifierNameSyntax errorIdentifier)
        {
            return;
        }

        if (conditionAccess.Expression is not IdentifierNameSyntax conditionIdentifier)
        {
            return;
        }

        if (errorIdentifier.Identifier.Text != conditionIdentifier.Identifier.Text)
        {
            return;
        }

        // Bad!
        context.ReportDiagnostic
        (
            Diagnostic.Create
            (
                descriptor: Descriptors.REM0002RedundantConditionalExpressionOnResult,
                conditional.GetLocation(),
                messageArgs: errorIdentifier.Identifier.Text
            )
        );
    }
}
