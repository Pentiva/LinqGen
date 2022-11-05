// LinqGen.Generator, Maxwell Keonwoo Kang <code.athei@gmail.com>, 2022

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Cathei.LinqGen.Generator
{
    using static SyntaxFactory;
    using static CodeGenUtils;

    public sealed class FirstEvaluation : Evaluation
    {
        private bool OrDefault { get; }

        public FirstEvaluation(in LinqGenExpression expression, bool orDefault) : base(expression)
        {
            OrDefault = orDefault;
        }

        public override TypeSyntax ReturnType => Upstream!.OutputElementType;

        private InvocationExpressionSyntax GetEnumeratorExpression()
        {
            if (Upstream!.IsPartition)
            {
                return InvocationExpression(
                    MemberAccessExpression(SourceVar, GetSliceEnumeratorMethod),
                    ArgumentList(LiteralExpression(0), LiteralExpression(1)));
            }

            return InvocationExpression(SourceVar, GetEnumeratorMethod);
        }

        public override BlockSyntax RenderMethodBody()
        {
            return Block(UsingLocalDeclarationStatement(
                    IteratorVar.Identifier, GetEnumeratorExpression()),
                IfStatement(InvocationExpression(IteratorVar, MoveNextMethod),
                    ReturnStatement(MemberAccessExpression(IteratorVar, CurrentProperty))),
                OrDefault ? ReturnDefaultStatement() : ThrowInvalidOperationStatement());
        }
    }
}