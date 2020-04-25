using System;
using System.IO;
using System.Collections.Generic;

namespace crafting_interpreters
{
    class Lox
    {
        private static readonly Interpreter Interpreter = new Interpreter();
        static bool hadError = false;
        static bool hadRuntimeError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1) {
                Console.WriteLine("Usage: jlox [script]");
                Environment.Exit(65);
            } else if (args.Length == 1) {
                runFile(args[0]);
            } else {
                runPrompt();
            };
        }

        private static void runFile(String path) {
            string source = File.ReadAllText(path);
            run(source);
        }

        private static void runPrompt() {
            for(;;) {
                Console.Write("> ");
                run(Console.ReadLine());
                hadError = false;
                hadRuntimeError = false;
            }
        }

        private static void run(string source) {
            if (hadError) Environment.Exit(65);
            if (hadRuntimeError) Environment.Exit(70);
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();
            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.parse();

            if (hadError) return;

            Interpreter.interpret(statements);
            
        }

        public static void error(int line, string message) {
            report(line, "", message);
        }

        public static void runtimeError(RuntimeError error) {
            Console.Error.WriteLine(error.Message);
            Console.Error.WriteLine($"[line {error.Token.Line}]");
            hadRuntimeError = true;

        }

        public static void error(Token token, string message) {
            if (token.Type == TokenType.EOF) {
                report(token.Line, "at end", message);
            } else {
                report(token.Line, $" at '{token.Lexeme}'", message);
            }
        }

        private static void report(int line, string where, string message) {
            Console.Error.WriteLine(
                $"[line + {line}] Error {where}: {message}"
            );
            hadError = true;
        }
    }
}
