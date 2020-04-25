using System.Collections.Generic;

namespace crafting_interpreters
{
     class LoxFunction : LoxCallable {
        private readonly Stmt.Function Declaration;
        private readonly Envir Closure;

        public LoxFunction(Stmt.Function declaration, Envir closure) {
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
                return returnValue.Value;
            }
            return null;
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
