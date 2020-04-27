using System;
using System.Collections.Generic;

namespace crafting_interpreters
{
    internal class LoxClass : LoxCallable
    {
        public readonly string Name;
        public readonly Dictionary<string, LoxFunction> Methods;

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            Methods = methods;
        }

        public int arity()
        {
            LoxFunction initializer = findMethod("init");
            if(initializer == null) return 0;
            return initializer.arity();
        }

        public object call(Interpreter interpreter, List<object> args)
        {
            LoxInstance instance = new LoxInstance(this);
            LoxFunction initializer = findMethod("init");
            if (initializer != null) {
                initializer.bind(instance).call(interpreter, args);
            }
            return instance;
        }

        internal LoxFunction findMethod(string name)
        {
            LoxFunction method;
            if(Methods.TryGetValue(name, out method)) {
                return method;
            }

            return null;
        }

        public override string ToString() {
            return Name;
        }
    }
}