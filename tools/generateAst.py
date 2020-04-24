import sys
import os.path

class FileOrStdOut:
    def __init__(self, path):
        self.path = path

    def __enter__(self):
        if self.path != "-":
            self.f = open(self.path, "w")
        else:
            self.f = sys.stdout
        return IndentWriter(self.f)

    def __exit__(self, type, value, traceback):
        if self.path != "-":
            self.f.close()

class IndentWriter:
    def __init__(self, f):
        self.indentLevel = 0
        self.f = f

    def indent(self):
        self.indentLevel += 1
    def dedent(self):
        self.indentLevel -= 1
    
    def write(self, text):
        self.f.write("    " * self.indentLevel + text)

    def writeln(self, text):
        self.write(text + "\n")

def defineVisitor(f, baseName, types):
    f.writeln("public interface Visitor<R> {")
    f.indent()
    for etype in types:
        className = etype.split(":")[0].strip()
        f.writeln(f"R visit{className}{baseName}({className} {baseName.lower()});")
    f.dedent()
    f.writeln("}")

def defineType(f, baseName, className, fieldList):
    f.writeln(f"public class {className} : {baseName} {{")
    f.indent()
    fielddefs = [x.strip().split() for x in fieldList.split(",")]

    etypes = [x[0] for x in fielddefs]
    params = [x[1] for x in fielddefs]
    fields = [x.capitalize() for x in params]

    # fields
    for field, etype in zip(fields, etypes):
        f.writeln(f"public readonly {etype} {field};")

    #constructor
    args = ", ".join([f'{x} {y}' for x,y in zip(etypes, params)])
    f.writeln(f"public {className}({args}) {{")
    f.indent()
    for field, param in zip(fields, params):
        f.writeln(f"{field} = {param};")
    f.dedent()
    f.writeln("}")
    f.writeln("public override R accept<R>(Visitor<R> visitor) {")
    f.indent()
    f.writeln(f"return visitor.visit{className}{baseName}(this);")
    f.dedent()
    f.writeln("}")
    f.dedent()
    f.writeln("}")

def defineAst(outputDir, baseName, types):
    if outputDir != "":
        path = os.path.join(outputDir, baseName + ".cs")
    else:
        path = "-"
    with FileOrStdOut(path) as f:
        f.writeln("using System;")
        f.writeln("using System.Collections.Generic;")
        f.writeln("namespace crafting_interpreters {")
        f.indent()
        f.writeln(f"abstract class {baseName} {{")
        f.indent()
        defineVisitor(f, baseName, types)
        f.writeln("public abstract R accept<R>(Visitor<R> visitor);")

        for etype in types:
            className, fields = [x.strip() for x in etype.split(":")]
            defineType(f, baseName, className, fields)

        f.dedent()
        f.writeln("}")



        f.dedent()
        f.writeln("}")



if __name__=="__main__":
    defineAst("../", "Expr", [
        "Binary:    Expr left, Token op, Expr right",
        "Grouping:  Expr expression",
        "Literal:   Object value",
        "Unary:     Token op, Expr right"
    ])