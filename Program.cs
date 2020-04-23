using System;
using System.IO;
using System.Collections.Generic;

namespace crafting_interpreters
{

    class Lox
    {
        static Boolean hadError = false;

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
            }
        }

        private static void run(string source) {
            if (hadError) Environment.Exit(65);
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();

            foreach (Token token in tokens) {
                Console.WriteLine(token);
            }
        }

        public static void error(int line, string message) {
            report(line, "", message);
        }

        private static void report(int line, string where, string message) {
            Console.Error.WriteLine(
                $"[line + {line}] Error {where}: {message}"
            );
            hadError = true;
        }
    }
}
