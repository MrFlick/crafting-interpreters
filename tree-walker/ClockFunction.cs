using System.Collections.Generic;

namespace crafting_interpreters
{
    namespace Functions
    {
        class ClockFunction : LoxCallable
        {
            public int arity()
            {
                return 0;
            }

            public object call(Interpreter interpreter, List<object> args)
            {
                return (double)System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }

            public override string ToString()
            {
               return "<native fn>"; 
            }
        }
    }
}
