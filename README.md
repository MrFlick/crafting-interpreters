# Following along with Crafting Interpreters

This was just a project to following along with the 
examples in the book Crafting Interpreters by Bob Nystrom

https://craftinginterpreters.com/

The code is broken up into two projects

## tree-walker (ilox)

This is an implementation of the tree-walk interpreter
described in part 2 of the book written using C# 
and dotnetcore to build and run inside VS Code

### virtual machine (clox)

This is an implementation of the bytecode virtual
machine that follows the C code written in part 3
of the book.


### Installing

For the tree-walker, you'll need to install dotnetcore from

https://dotnet.microsoft.com/download

and works well the with C# extension

https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp

For the virtual-machine, I installed MinGW at C:\Mingw-w64\mingw32\bin in order to compile the code

https://sourceforge.net/projects/mingw-w64/

And used the C/C++ VS code extension for editor support 

https://marketplace.visualstudio.com/items?itemName=ms-vscode.cpptools


## Running the projects

You can open both projects in VS code using the 
`ci.code-workspace` file. That should preserve
the build and launch tasks across projects.
Otherwise you'll want to open either the
`tree-walker` or `virtual-machine` folder 
as your workspace folder.