using System.Collections.Generic;

namespace crafting_interpreters
{
    interface LoxCallable {
        int arity();
        object call(Interpreter interpreter, List<object> args);
    }
}
