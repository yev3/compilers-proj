.assembly extern mscorlib {}  
.assembly hello {}  

.class public hello
{
  .method static void main() 
  {
    .entrypoint
    .maxstack 1


    ldstr        "Hello, World"
    call         void [mscorlib]System.Console::WriteLine(string)

    ret          
  } // end of method hello::Main

} // end of class hello
