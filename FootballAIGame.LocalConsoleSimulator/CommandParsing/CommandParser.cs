using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors;
using FootballAIGame.LocalConsoleSimulator.Commands;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing
{
    /// <summary>
    /// Provides methods for parsing the string input and convert it to the <see cref="ICommand"/>.
    /// </summary>
    static class CommandParser
    {
        /// <summary>
        /// Tries to parse the input line.
        /// </summary>
        /// <param name="line">The input line.</param>
        /// <returns>The <see cref="ParseResult"/> instance. </returns>
        public static ParseResult TryParse(string line)
        {
            if (line == null)
            {
                return new ParseResult() { Error = new NullInput() };
            }

            line = line.Trim();

            if (line.Length == 0)
                return new ParseResult() { Error = new MissingCommand() };

            if (Regex.IsMatch(line, @"^simulate(\s|\s*$)"))
                return TryParseSimulate(line);


            return new ParseResult() { Error = new InvalidCommandName(line.Split()[0]) };
        }

        /// <summary>
        /// Tries to parse the input line and convert it to the <see cref="SimulateMatchesCommand"/>.
        /// Expects the input line to start with the simulate word.
        /// </summary>
        /// <param name="line">The input line.</param>
        /// <returns></returns>
        private static ParseResult TryParseSimulate(string line)
        {
            Debug.Assert(line != null && Regex.IsMatch(line, @"^simulate(\s|\s*$)"));

            var regex = new Regex(@"^(?i)simulate(?-i)\s*\[(?<options>[^\]]*)\](?<body>.*)$");

            var match = regex.Match(line);
            if (!match.Success)
            {
                return new ParseResult() { Error = new MissingOptions() };
            }

            var options = Regex.Split(match.Groups["options"].Value, @"\s*,\s*").Where(o => o.Length > 0);
            var ais = Regex.Split(match.Groups["body"].Value, @"\s+").Where(ai => ai.Length > 0).ToList();

            var command = new SimulateMatchesCommand();

            // parse options
            var saveToDirectoryRegex = new Regex(@"^sd\((?<directory>.*?)\)$|^sd$");
            var saveToFilesRegex = new Regex(@"^sf\((?<files>.*?)\)$|^sf$");

            foreach (var option in options)
            {
                if (option == "e")
                {
                    command.ExtendedResultOn = true;
                }
                else if ((match = saveToDirectoryRegex.Match(option)).Success)
                {
                    if (!match.Groups["directory"].Success)
                        return new ParseResult() { Error = new UnspecifiedSavePath() };

                    var directoryPath = match.Groups["directory"].Value;
                    DirectoryInfo directoryInfo;

                    var parsingError = TryParseSaveDirectory(directoryPath, out directoryInfo);
                    if (parsingError != null)
                        return new ParseResult { Error = parsingError };

                    command.SavingDirectory = directoryInfo;
                    command.SavingOn = true;
                }
                else if ((match = saveToFilesRegex.Match(option)).Success)
                {
                    if (!match.Groups["files"].Success)
                        return new ParseResult() { Error = new UnspecifiedSavePath() };

                    var filePaths = Regex.Split(match.Groups["files"].Value, @"\s*;\s*");

                    var files = new FileInfo[filePaths.Length];

                    for (var i = 0; i < filePaths.Length; i++)
                    {
                        FileInfo fileInfo;

                        var parsingError = TryParseSaveFile(filePaths[i], out fileInfo);
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

            return new ParseResult() { Command = command, IsSuccessful = true };
        }

        /// <summary>
        /// Tries to parse the directory path to the directory specified by the save to directory command option.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="directoryInfo">The parsed directory information.</param>
        /// <returns>Null if the parsing was successful and the parsed <see cref="DirectoryInfo"/> is saved to the 
        /// <paramref name="directoryInfo"/> output parameter; otherwise, returns the <see cref="IParsingError"/> instance describing the error that happened. </returns>
        private static IParsingError TryParseSaveDirectory(string directoryPath, out DirectoryInfo directoryInfo)
        {
            directoryInfo = null;

            Debug.Assert(directoryPath != null);

            if (directoryPath.Length == 0)
                return new EmptySavePath();

            try
            {
                directoryInfo = new DirectoryInfo(directoryPath);

                if (!directoryInfo.Exists)
                    directoryInfo.Create();

            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
            {
                return new UnauthorizedAccess(directoryPath);
            }
            catch (PathTooLongException)
            {
                return new PathTooLong(directoryPath);
            }
            catch (Exception) // rest
            {
                return new InvalidPath(directoryPath);
            }


            return null;
        }

        /// <summary>
        /// Tries to parse the file path specified by the save to files command option.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileInfo">The parsed file information.</param>
        /// <returns>Null if the parsing was successful and the parsed <see cref="FileInfo"/> is saved to the 
        /// <paramref name="fileInfo"/> output parameter; otherwise, returns the <see cref="IParsingError"/> instance describing the error that happened. </returns>
        private static IParsingError TryParseSaveFile(string filePath, out FileInfo fileInfo)
        {
            fileInfo = null;

            Debug.Assert(filePath != null);

            if (filePath.Length == 0)
                return new EmptySavePath();

            try
            {
                fileInfo = new FileInfo(filePath);

            }
            catch (Exception ex) when (ex is SecurityException || ex is UnauthorizedAccessException)
            {
                return new UnauthorizedAccess(filePath);
            }
            catch (PathTooLongException)
            {
                return new PathTooLong(filePath);
            }
            catch (Exception) // rest
            {
                return new InvalidPath(filePath);
            }


            return null;
        }

    }
}
