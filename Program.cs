using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auto_unrar
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Log("******* Starting execution with args[0]: " + (args.Length > 0 ? args[0] : "null"));

                // Grab the path to the unrar.exe binary.
                var unrarPath = ConfigurationSettings.AppSettings["unrar-path"];

                if (!File.Exists(unrarPath))
                    throw new ArgumentException("Unable to locate unrar utility at: " + unrarPath);

                // There should be a single argument passed in.
                if (args == null || args.Length != 1)
                    throw new ArgumentException("Expecting exactly one argument; the path in which to search for a RAR to extract.");

                var searchPath = args[0];

                Log("Search Path: " + searchPath);

                var rarFilePath = "";
                var destinationPath = "";

                if (searchPath.EndsWith(".rar") && File.Exists(searchPath))
                {
                    Log("Using case 1 - is RAR file.");
                    rarFilePath = searchPath;
                    destinationPath = Path.GetDirectoryName(rarFilePath);
                }
                else if (IsDirectory(searchPath))
                {
                    Log("Using case 2 - is directory.");

                    if (!searchPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        searchPath += Path.DirectorySeparatorChar;

                    var files = Directory.GetFiles(searchPath, "*.rar", SearchOption.AllDirectories);

                    if (files == null || files.Length == 0)
                        throw new Exception("Unable to locate *.rar in the search path.");

                    if (files.Length > 1)
                        Log("Found multiple RAR files in search path; using the first one.");

                    rarFilePath = files[0];
                    destinationPath = Path.GetDirectoryName(rarFilePath);
                }
                else
                {
                    throw new Exception("Unhandled case encountered.");
                }

                Log("RAR File: " + rarFilePath);
                Log("Destination Path: " + destinationPath);

                var arguments = String.Format("x -y \"{0}\" \"{1}\"", rarFilePath, destinationPath);

                Log("Unrar arguments: " + arguments);

                Log("Waiting 30 seconds for RAR to be flushed to disk...");
                System.Threading.Thread.Sleep(30000);

                var p = new Process();

                // This will hold the stdout/stderr of the unrar execution.
                var sb = new StringBuilder();
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.OutputDataReceived += (sender, eventArgs) => sb.AppendLine(eventArgs.Data);
                p.ErrorDataReceived += (sender, eventArgs) => sb.AppendLine(eventArgs.Data);

                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = unrarPath;
                p.StartInfo.Arguments = arguments;

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                // Block until the unrar process has finished.
                p.WaitForExit();

                Log("Unrar process output: " + sb.ToString());

                Log("auto-unrar process exited without error.");
            }
            catch (Exception ex)
            {
                Log("Unhandled Exception: " + ex.Message);
            }

            Log("******* Ending execution");
        }

        private static bool IsDirectory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);

            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }

        private static void Log(string message)
        {
            var path = ConfigurationSettings.AppSettings["log-path"];

            if (String.IsNullOrEmpty(path))
                path = "C:\\auto-unrar.txt";

            File.AppendAllText(path, "[" + DateTime.Now.ToString() + "] " + message + Environment.NewLine);
        }
    }
}
