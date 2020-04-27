using System;
using System.Collections.Generic;

namespace crafting_interpreters
{
     class LoxFunction : LoxCallable {
        private readonly Stmt.Function Declaration;
        private readonly Envir Closure;
        private readonly bool IsInitializer;

        public LoxFunction(Stmt.Function declaration, Envir closure,
            bool isInitializer) {
            IsInitializer = isInitializer;
            Closure = closure;
            Declaration = declaration;
        }

        public int arity()
        {
            return Declaration.Parameters.Count;
        }

        public object call(Interpreter interpreter, List<object> args)
        {
            Envir envir = new Envir(Closure);
            for(int i = 0; i< Declaration.Parameters.Count; i++) {
                envir.define(Declaration.Parameters[i].Lexeme, args[i]);
            }
            try {
                interpreter.executeBlock(Declaration.Body, envir);
            } catch (LoxReturn returnValue) {
                if (IsInitializer) return Closure.getAt(0, "this");
                return returnValue.Value;
            }
            if(IsInitializer) return Closure.getAt(0, "this");
            return null;
        }

        internal LoxFunction bind(LoxInstance instance)
        {
            Envir envir = new Envir(Closure);
            envir.define("this", instance);
            return new LoxFunction(Declaration, envir, IsInitializer);
        }

        public override string ToString() {
            return $"<fn {Declaration.Name.Lexeme}>";
        }
    }

    class LoxReturn : System.Exception {
        public readonly object Value;

        public LoxReturn(object value) : base() {
            Value = value;
        }
    }
}
