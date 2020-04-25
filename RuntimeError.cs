namespace crafting_interpreters
{
    class RuntimeError : System.Exception {
        readonly public Token Token;

        public RuntimeError(Token token, string message): base(message)
        {
            Token=token;
        }
    }
}
