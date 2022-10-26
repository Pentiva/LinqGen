﻿// LinqGen.Generator, Maxwell Keonwoo Kang <code.athei@gmail.com>, 2022

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Cathei.LinqGen.Generator
{
    using static SyntaxFactory;
    using static CodeGenUtils;

    public static class CompiledGenerationTemplate
    {
        private static readonly SyntaxTree TemplateSyntaxTree = CSharpSyntaxTree.ParseText(@"// DO NOT EDIT
// Generated by LinqGen.Generator

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cathei.LinqGen;
using Cathei.LinqGen.Hidden;

namespace Cathei.LinqGen
{
    // Extensions needs to be internal to prevent ambiguous resolution
    internal static class _Extensions_
    {

    }
}
");

        private class Rewriter : CSharpSyntaxRewriter
        {
            private readonly CompiledGeneration _instruction;

            public Rewriter(CompiledGeneration instruction)
            {
                _instruction = instruction;
            }

            public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                switch (node.Identifier.ValueText)
                {
                    case "_Extensions_":
                        node = RewriteExtensionClass(node);
                        break;
                }

                return base.VisitClassDeclaration(node);
            }

            private ClassDeclarationSyntax RewriteExtensionClass(ClassDeclarationSyntax node)
            {
                return node.WithIdentifier(
                        Identifier($"LinqGenExtensions_{_instruction.IdentifierName.Identifier.ValueText}"))
                    .WithMembers(new SyntaxList<MemberDeclarationSyntax>(GetExtensionMethods()));
            }

            private IEnumerable<MemberDeclarationSyntax> GetExtensionMethods()
            {
                if (_instruction.Evaluations == null)
                {
                    // nothing to evaluate
                    yield break;
                }

                // evaluation can use specialization, so it should be extension method
                foreach (var evaluation in _instruction.Evaluations)
                {
                    yield return MethodDeclaration(new(AggressiveInliningAttributeList),
                        PublicStaticTokenList, evaluation.ReturnType, default,
                        evaluation.MethodName.Identifier, evaluation.GetTypeParameters(),
                        ParameterList(evaluation.GetParameters()), evaluation.GetGenericConstraints(),
                        evaluation.RenderMethodBody(), default, default);
                }
            }
        }

        public static SourceText Render(CompiledGeneration instruction)
        {
            var root = TemplateSyntaxTree.GetRoot();

            var rewriter = new Rewriter(instruction);
            root = rewriter.Visit(root);

            return root.NormalizeWhitespace().GetText(Encoding.UTF8);
        }
    }
}