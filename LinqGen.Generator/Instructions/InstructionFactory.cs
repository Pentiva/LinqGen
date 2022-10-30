// LinqGen.Generator, Maxwell Keonwoo Kang <code.athei@gmail.com>, 2022

using System;
using System.Collections.Generic;
using System.Text;
using Cathei.LinqGen.Hidden;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cathei.LinqGen.Generator
{
    using static CodeGenUtils;

    public static class InstructionFactory
    {
        /// <summary>
        /// The Instruction instance must be unique per signature (per generic arguments combination).
        /// </summary>
        public static Generation? CreateGeneration(StringBuilder logBuilder, in LinqGenExpression expression, int id)
        {
            INamedTypeSymbol? typeSymbol;

            switch (expression.SignatureSymbol!.Name)
            {
                case "Specialize":
                {
                    ITypeSymbol typeArgument = expression.SignatureSymbol!.TypeArguments[0];

                    if (typeArgument is IArrayTypeSymbol arraySymbol)
                    {
                        if (arraySymbol.Rank == 1)
                            return new SpecializeArrayGeneration(expression, id, arraySymbol);
                        return new SpecializeArrayMultiGeneration(expression, id, arraySymbol);
                    }

                    if (typeArgument is INamedTypeSymbol namedTypeSymbol)
                        return new SpecializeGeneration(expression, id, namedTypeSymbol);

                    break;
                }

                case "Select":
                    if (!expression.TryGetNamedParameterType(0, out typeSymbol))
                        break;
                    return new SelectOperation(expression, id, typeSymbol, false, false);

                case "SelectStruct":
                    if (!expression.TryGetNamedParameterType(0, out typeSymbol))
                        break;
                    return new SelectOperation(expression, id, typeSymbol, false, true);

                case "SelectAt":
                    if (!expression.TryGetNamedParameterType(0, out typeSymbol))
                        break;
                    return new SelectOperation(expression, id, typeSymbol, true, false);

                case "SelectAtStruct":
                    if (!expression.TryGetNamedParameterType(0, out typeSymbol))
                        break;
                    return new SelectOperation(expression, id, typeSymbol, true, true);

                case "Where":
                    if (!expression.TryGetNamedParameterType(0, out typeSymbol))
                        break;
                    return new WhereOperation(expression, id, typeSymbol, false, false);

                case "WhereAt":
                    if (!expression.TryGetNamedParameterType(0, out typeSymbol))
                        break;
                    return new WhereOperation(expression, id, typeSymbol, true, false);

                case "WhereStruct":
                    if (!expression.TryGetNamedParameterType(0, out typeSymbol))
                        break;
                    return new WhereOperation(expression, id, typeSymbol, false, true);

                case "WhereAtStruct":
                    if (!expression.TryGetNamedParameterType(0, out typeSymbol))
                        break;
                    return new WhereOperation(expression, id, typeSymbol, true, true);

                case "AsEnumerable":
                    return new AsEnumerableOperation(expression, id);

                case "Cast":
                    return new CastOperation(expression, id, false);

                case "OfType":
                    return new CastOperation(expression, id, true);
            }

            // not yet implemented
            return null;
        }

        public static Evaluation? CreateEvaluation(StringBuilder logBuilder, in LinqGenExpression expression)
        {
            switch (expression.MethodSymbol.Name)
            {
                case "First":
                    return new FirstEvaluation(expression, false);

                case "FirstOrDefault":
                    return new FirstEvaluation(expression, true);

                case "Last":
                    return new LastEvaluation(expression, false);

                case "LastOrDefault":
                    return new LastEvaluation(expression, true);

                case "Single":
                    break;

                case "Sum":
                    return new SumEvaluation(expression);

                case "Count":
                    return new CountEvaluation(expression);
            }

            return null;
        }

    }
}