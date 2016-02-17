﻿namespace Il2Native.Logic.DOM.Synthesized
{
    using System.Collections.Generic;
    using System.Linq;
    using DOM2;

    using Il2Native.Logic.DOM.Implementations;

    using Microsoft.CodeAnalysis;

    public class CCodeInterfaceMethodAdapterDefinition : CCodeMethodDefinition
    {
        private IList<Statement> typeDefs = new List<Statement>();

        private ITypeSymbol type;
        private IMethodSymbol classMethod;

        public CCodeInterfaceMethodAdapterDefinition(ITypeSymbol type, IMethodSymbol interfaceMethod, IMethodSymbol classMethod)
            : base(interfaceMethod)
        {
            this.type = type;
            this.classMethod = classMethod;

            var receiver = type == classMethod.ContainingType
                               ? (Expression)new ThisReference { Type = classMethod.ContainingType }
                               : (Expression)new BaseReference { Type = classMethod.ContainingType, ExplicitType = true };

            var call = new Call { ReceiverOpt = receiver, Method = classMethod };
            foreach (var argument in interfaceMethod.Parameters.Select(parameterSymbol => new Parameter { ParameterSymbol = parameterSymbol }))
            {
                call.Arguments.Add(argument);
            }

            var body = !interfaceMethod.ReturnsVoid ? (Statement)new ReturnStatement { ExpressionOpt = call } : (Statement)new ExpressionStatement { Expression = call };

            MethodBodyOpt = new MethodBody(Method);

            if (classMethod.IsGenericMethod)
            {
                // set generic types
                foreach (var typeArgument in interfaceMethod.TypeArguments)
                {
                    this.typeDefs.Add(
                        new TypeDef { TypeExpression = new TypeExpression { Type = typeArgument.GetFirstConstraintType() ?? new TypeImpl { SpecialType = SpecialType.System_Object } }, Identifier = new TypeExpression { Type = typeArgument } });
                }

                // set generic types
                foreach (var typeArgument in classMethod.TypeArguments)
                {
                    MethodBodyOpt.Statements.Add(
                        new TypeDef { TypeExpression = new TypeExpression { Type = typeArgument.GetFirstConstraintType() ?? new TypeImpl { SpecialType = SpecialType.System_Object } }, Identifier = new TypeExpression { Type = typeArgument } });
                }
            }

            MethodBodyOpt.Statements.Add(body);
        }

        public override bool IsGeneric
        {
            get
            {
                return this.classMethod.ContainingType.IsGenericType;
            }
        }

        public override void WriteTo(CCodeWriterBase c)
        {
            c.NewLine();

            c.TextSpan(string.Format("// adapter: {0}", this.Method));
            c.NewLine();

            foreach (var statement in typeDefs)
            {
                statement.WriteTo(c);
            }

            var namedTypeSymbol = (INamedTypeSymbol)this.type;
            if (namedTypeSymbol.IsGenericType)
            {
                c.WriteTemplateDeclaration(namedTypeSymbol);
                c.NewLine();
            }

            c.WriteMethodReturn(this.Method, true);
            c.WriteMethodNamespace(namedTypeSymbol);
            c.WriteMethodName(this.Method, false);
            c.WriteMethodPatameters(this.Method, true, this.MethodBodyOpt != null);

            if (this.MethodBodyOpt == null)
            {
                c.EndStatement();
            }
            else
            {
                this.MethodBodyOpt.WriteTo(c);
            }
        }
    }
}
