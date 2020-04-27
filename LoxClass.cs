using System;
using System.Collections.Generic;

namespace crafting_interpreters
{
    internal class LoxClass : LoxCallable
    {
        public readonly string Name;
        public readonly Dictionary<string, LoxFunction> Methods;
        public readonly LoxClass SuperClass;

        public LoxClass(string name, LoxClass superClass, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            SuperClass = superClass;
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

            if (SuperClass != null) {
                return SuperClass.findMethod(name);
            }

            return null;
        }

        public override string ToString() {
            return Name;
        }
    }
}