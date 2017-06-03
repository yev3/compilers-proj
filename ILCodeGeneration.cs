using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Utilities;
using NLog;
using Proj3Semantics.AST;
using Proj3Semantics.Visitors;

namespace Proj3Semantics
{
    public class ILCodeGeneration
    {
        private static NLog.Logger Log = LogManager.GetCurrentClassLogger();
        private readonly string _assemblyName;
        private readonly string _ilFullPath;
        private readonly string _exeFullPath;
        private readonly Node _rootNode;

        public ILCodeGeneration(Node rootNode, string assemblyName)
        {
            _rootNode = rootNode;
            _assemblyName = assemblyName;
            _ilFullPath = Path.Combine(Directory.GetCurrentDirectory(), assemblyName + ".il");
            _exeFullPath = Path.Combine(Directory.GetCurrentDirectory(), assemblyName + ".exe");
        }
        public ILCodeGeneration(Node rootNode, string assemblyName, string ilFullPath, string exeFullPath)
        {
            _rootNode = rootNode;
            _assemblyName = assemblyName;
            _ilFullPath = ilFullPath;
            _exeFullPath = exeFullPath;
        }

        public void GenerateIL()
        {
            Log.Trace("Starting to generate IL: ");
            Log.Trace(_ilFullPath);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(_ilFullPath))
            {
                CodeGenVisitor codeGenVisitor = new CodeGenVisitor(file, _assemblyName);
                codeGenVisitor.Generate(_rootNode);
            }
            Log.Trace("Finished generating IL code.");

        }

        public int CompileExe()
        {
            string ilasmExePath = Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFrameworkFile("ilasm.exe", TargetDotNetFrameworkVersion.VersionLatest);
            string ilasmArgs = "\"" + _ilFullPath + "\" /out=\"" + _exeFullPath + "\"";

            Log.Trace("Compiling IL to EXE: ");
            Log.Trace(ilasmExePath);
            Log.Trace(ilasmArgs);

            Process ilProcess = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = ilasmExePath,
                    Arguments = ilasmArgs
                }
            };
            ilProcess.Start();

            string output = ilProcess.StandardOutput.ReadToEnd();
            ilProcess.WaitForExit();
            using (OutColor.Gray)
            {
                Console.WriteLine(output);
            }

            Log.Trace("Finished compiling IL to EXE: ");
            return ilProcess.ExitCode;
        }

        public void RunExeAndWait()
        {
            Log.Trace("Executing compiled program and waiting for it to exit:");
            Log.Trace(_exeFullPath);

            string execArgs = "/c \"" + _exeFullPath + "\" &pause";
            Process userProcess = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Arguments = execArgs,
                },
                EnableRaisingEvents = true,
            };

            userProcess.Start();
            userProcess.WaitForExit();
            Log.Trace("Finished executing program.");
        }

        public void GenerateCompileAndRun()
        {
            GenerateIL();
            if (CompileExe() != 0)
                Log.Error("ILASM exited with an error.");
            else
                RunExeAndWait();
        }


    }
}
