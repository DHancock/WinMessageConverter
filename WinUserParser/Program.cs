using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace WinUserParser;

internal class Program
{
    const int error_success = 0;
    const int error_fail = 1;

    static int Main()
    {
        try
        {
            App app = new App();
            return app.Run();
        }
        catch (Exception ex)
        {
            Trace.Fail(ex.ToString());
        }

        return error_fail;
    }

    private class App
    {
        private readonly string winUserPath;
        private readonly string templatePath;
        private readonly string outputCsPath;
        private readonly string outputCsWin32Path;

        // the two generated source files
        private const string cOutputCsFile = @"..\..\..\..\..\WinMessageConverter\LazyProvider.cs";
        private const string cOutputCsWin32File = @"..\..\..\..\..\WinMessageConverter\NativeMethods.txt";
        
        public App()
        {
            string? executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            winUserPath = Path.Join(executingDir, "WinUser.h");

            if (!File.Exists(winUserPath))
                throw new FileNotFoundException($"Failed to find {winUserPath}");

            templatePath = Path.Join(executingDir, "Template.txt");

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Failed to find {templatePath}");

            outputCsPath = Path.GetFullPath(cOutputCsFile, winUserPath);
            outputCsWin32Path = Path.GetFullPath(cOutputCsWin32File, winUserPath);
        }

        public int Run()
        {
            HashSet<string> messages = ParseWinUserHeaderFile();

            List<string> srcLines = GenerateMessageDictionarySource(messages);

            WriteAllLines(outputCsPath, srcLines);
            WriteAllLines(outputCsWin32Path, messages);

            return error_success;
        }

        private HashSet<string> ParseWinUserHeaderFile()
        {
            HashSet<string> messageSet = new HashSet<string>();

            using (FileStream fs = File.OpenRead(winUserPath))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string? line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        const string cDefine = "#define";
                        const string cWMPrefix = "WM_";

                        string[] parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        if ((parts.Length > 2) && (parts[0] == cDefine))
                        {
                            if (parts[1].StartsWith(cWMPrefix))
                            {
                                if (parts[2].StartsWith(cWMPrefix))
                                {
                                    if (!messageSet.Contains(parts[2]))
                                    {
                                        throw new Exception($"Failed to parse message {line}, forward declaration of {parts[2]}");
                                    }
                                }
                                else if (!TryReadNumber(parts[2], out uint _))
                                {
                                    throw new Exception($"Failed to parse message {line}, invalid number {parts[2]}");
                                }

                                messageSet.Add(parts[1]);
                            }
                        }
                    }
                }
            }

            return messageSet;
        }

        private static bool TryReadNumber(string number, out uint value)
        {
            const string cHexPrefix = "0x";

            if (number.StartsWith(cHexPrefix))
                return uint.TryParse(number.AsSpan(cHexPrefix.Length), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out value);

            return uint.TryParse(number, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);
        }

        private List<string> GenerateMessageDictionarySource(HashSet<string> messages)
        {
            string[] lines = File.ReadAllLines(templatePath);
            List<string> output = new List<string>();

            foreach (string line in lines)
            {
                if (line.Contains("<insert messages>"))
                {
                    foreach (string message in messages)
                    {
                        // define message codes indirectly via CsWin32 so that I don't have to write a c preprocessor
                        output.Add($"\t\t\t(PInvoke.{message}, \"{message}\"),");
                    }
                }
                else
                {
                    output.Add(line);
                }
            }

            return output;
        }

        private static void WriteAllLines(string path, IEnumerable<string> lines)
        {
            using (FileStream fs = File.Create(path))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (string line in lines)
                        sw.WriteLine(line);
                }
            }
        }
    }
}

