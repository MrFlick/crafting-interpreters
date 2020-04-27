using System.Collections.Generic;

namespace crafting_interpreters
{
    class Envir {
        readonly Envir Enclosing;
        private readonly Dictionary<string, object> Values;

        public Envir()
        {
            Enclosing = null;
            Values = new Dictionary<string, object>();
        }
        public Envir(Envir enclosing)
        {
            Enclosing = enclosing;
            Values = new Dictionary<string, object>();
        }

        public void define(string name, object value) {
            Values[name] = value;
        }

        Envir ancestor(int distance) {
            Envir envir = this;
            for(int i =0; i<distance; i++) {
                envir = envir.Enclosing;
            }
            return envir;
        }

        public object getAt(int distance, string name) {
            return ancestor(distance).Values[name];
        }

        public object get(Token name) {
            if (Values.ContainsKey(name.Lexeme)) {
                return Values.GetValueOrDefault(name.Lexeme);
            }

            if (Enclosing != null) return Enclosing.get(name);

            throw new RuntimeError(name,
            $"undefined variable '{name.Lexeme}'.");
        }

        public void assign(Token name, object value) {
            if (Values.ContainsKey(name.Lexeme)) {
                Values[name.Lexeme] = value;
                return;
            }

            if (Enclosing != null) {
                Enclosing.assign(name, value);
                return;
            }

            throw new RuntimeError(name,
                $"Undefined variable '{name.Lexeme}'.");
        }

    }
}
