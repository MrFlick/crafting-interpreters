using System.Collections.Generic;

namespace crafting_interpreters
{
    internal class LoxInstance
    {
        private LoxClass Klass;
        private readonly Dictionary<string, object> Fields = new Dictionary<string, object>();

        public LoxInstance(LoxClass klass)
        {
            Klass = klass;
        }

        public object get(Token name) {
            object value;
            if (Fields.TryGetValue(name.Lexeme, out value)) {
                return value;
            }

            LoxFunction method = Klass.findMethod(name.Lexeme);
            if (method != null) return method.bind(this);

            throw new RuntimeError(name, 
                $"Undefined property '{name.Lexeme}'");
        }

        public void set(Token name, object value) {
            Fields[name.Lexeme] = value;
        }

        public override string ToString() {
            return $"{Klass.Name} instance";
        }
    }
}