using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;
using FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors;
using FootballAIGame.LocalConsoleSimulator.Commands;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing
{
    static class CommandParser
    {
        public static ParseResult TryParse(string line)
        {
            if (line == null)
            {
                return new ParseResult() { Error = new NullInput() };
            }

            line = line.Trim();

            if (line.Length == 0)
            {
                return new ParseResult() { Error = new MissingCommand() };
            }

            if (line.StartsWith("simulate", StringComparison.InvariantCultureIgnoreCase))
                return TryParseSimulate(line);


            return new ParseResult() { Error = new InvalidCommandName(line.Split()[0]) };
        }

        private static ParseResult TryParseSimulate(string line)
        {
            Debug.Assert(line != null && line.StartsWith("simulate"));

            var regex = new Regex($@"^(?i)simulate(?-i)\s*\[(?<options>[^\]]*)\](?<body>.*)$");

            var match = regex.Match(line);
            if (!match.Success)
            {
                return new ParseResult() { Error = new MissingOptions() };
            }

            var options = Regex.Split(match.Groups["options"].Value, @"\s*,\s*").Where(o => o.Length > 0);
            var ais = Regex.Split(match.Groups["body"].Value, @"\s+").Where(ai => ai.Length > 0).ToList();

            var command = new SimulateMatchesCommand();

            // parse options
            var saveToDirectoryRegex = new Regex(@"^sd\((?<directory>.?)$");
            var saveToFilesRegex = new Regex(@"^sf\((?<files>)\)");

            foreach (var option in options)
            {
                if (option == "e")
                {
                    command.ExtendedResultOn = true;
                }
                else if ((match = saveToDirectoryRegex.Match(option)).Success)
                {
                    var directoryPath = match.Groups["directory"].Value;
                    DirectoryInfo directoryInfo;

                    var parsingError = TryParseDirectory(directoryPath, out directoryInfo);
                    if (parsingError != null)
                        return new ParseResult { Error = parsingError };

                    command.SavingDirectory = directoryInfo;
                    command.SavingOn = true;
                }
                else if ((match = saveToFilesRegex.Match(option)).Success)
                {
                    var filePaths = Regex.Split(match.Groups["files"].Value, @"\s*,\s*");

                    var files = new FileInfo[filePaths.Count()];

                    for (var i = 0; i < filePaths.Count(); i++)
                    {
                        FileInfo fileInfo;

                        var parsingError = TryParseFile(filePaths[i], out fileInfo);
                        if (parsingError != null)
                            return new ParseResult { Error = parsingError };

                        files[i] = fileInfo;
                    }

                    command.SavingFiles = files;
                    command.SavingOn = true;
                }
                else
                {
                    return new ParseResult { Error = new UnknownOption(option) };
                }
            }

            // parse AIs
            if (ais.Count == 0)
                return new ParseResult() { Error = new MissingAIs() };
            if (ais.Count % 2 != 0)
                return new ParseResult() { Error = new InvalidNumberOfAi() };

            command.Opponents = new List<Tuple<string, string>>();

            string first = null;
            foreach (var ai in ais)
            {
                if (first == null)
                    first = ai;
                else
                {
                    command.Opponents.Add(new Tuple<string, string>(first, ai));
                    first = null;
                }
            }

            return new ParseResult() { Command = command, IsSuccessfull = true };
        }

        private static IParsingError TryParseDirectory(string directoryPath, out DirectoryInfo directoryInfo)
        {
            directoryInfo = null;

            Debug.Assert(directoryPath != null);

            try
            {
                directoryInfo = new DirectoryInfo(directoryPath);
            }
            catch (SecurityException)
            {
                return new PermissionDenied(directoryPath);
            }
            catch (ArgumentException)
            {
                return new InvalidPath(directoryPath);
            }
            catch (PathTooLongException)
            {
                return new PathTooLong(directoryPath);
            }

            if (!directoryInfo.Exists)
                directoryInfo.Create();

            return null;
        }

        private static IParsingError TryParseFile(string filePath, out FileInfo fileInfo)
        {
            fileInfo = null;

            try
            {
                fileInfo = new FileInfo(filePath);
            }
            catch (SecurityException)
            {
                return new PermissionDenied(filePath);
            }
            catch (ArgumentException)
            {
                return new InvalidPath(filePath);
            }
            catch (PathTooLongException)
            {
                return new PathTooLong(filePath);
            }


            return null;
        }

    }
}
