// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using fts;

namespace FuzzyMatch
{
	internal class Program
	{
		private static string[] _dictionary;

		private static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("No file specified.");
				return;
			}
			var path = args[0];
			Console.WriteLine($"Reading [{path}]");

			// Open and read file
			var watch = Stopwatch.StartNew();
			_dictionary = File.ReadAllLines(path);
			watch.Stop();

			var time = watch.Elapsed.TotalMilliseconds;
			Console.WriteLine($"Read [{_dictionary.Length}] entries in {time} ms.");

			// Input Loop
			while (true)
			{
				Console.WriteLine("1. Count Matches");
				Console.WriteLine("2. Print Matches (Alphabetical)");
				Console.WriteLine("3. Print Matches (By Score)");
				Console.WriteLine("4. Exit");
				Console.WriteLine();
				Console.Write("> ");
				var option = Console.ReadLine();
				Console.WriteLine();

				if (option == "1" || option == "2" || option == "3")
				{
					// Read pattern
					Console.WriteLine("Enter search pattern");
					Console.WriteLine();
					Console.Write("> ");
					var pattern = Console.ReadLine();
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine($"Searching for [{pattern}] ...");
					Console.WriteLine();
					if (option == "1")
					{
						// Count Matches
						watch.Reset();
						watch.Start();
						var matches = CountMatches(pattern);
						time = watch.Elapsed.TotalMilliseconds;

						Console.WriteLine($"Found {matches} matches in {time} ms.");
						Console.WriteLine();
						Console.WriteLine();

					}
					else if (option == "2")
					{
						// Print Matches (Alphabetical)
						watch.Reset();
						watch.Start();
						var results = AlphabeticalMatches(pattern);
						time = watch.Elapsed.TotalMilliseconds;

						foreach (var result in results)
							Console.WriteLine(result);

						Console.WriteLine();
						Console.WriteLine($"Found {results.Count} matches in {time} ms.");
						Console.WriteLine();
						Console.WriteLine();

					}
					else if (option == "3")
					{
						// Print Matches (By Score)
						watch.Reset();
						watch.Start();
						var results = ScoredMatches(pattern);
						time = watch.Elapsed.TotalMilliseconds;

						foreach (var result in results)
							Console.WriteLine($"{result.Key} - {result.Value}");

						Console.WriteLine();
						Console.WriteLine($"Found {results.Count} matches in {time} ms.");
						Console.WriteLine();
						Console.WriteLine();
					}
				}
				else if (option == "4")
				{
					// Quit
					break;
				}
			}
		}

		private static int CountMatches(string pattern)
		{
			var matches = 0;
			foreach (var entry in _dictionary)
				if (FuzzyMatcher.FuzzyMatch(pattern, entry))
					++matches;

			return matches;
		}

		private static List<string> AlphabeticalMatches(string pattern)
		{
			var matches = new List<string>();
			foreach (var entry in _dictionary)
				if (FuzzyMatcher.FuzzyMatch(pattern, entry))
					matches.Add(entry);

			return matches;
		}

		private static Dictionary<string, int> ScoredMatches(string pattern)
		{
			var matches = new List<KeyValuePair<string, int>>();
			int score;
			foreach (var entry in _dictionary)
				if (FuzzyMatcher.FuzzyMatch(pattern, entry, out score))
					matches.Add(new KeyValuePair<string, int>(entry, score));

			return matches.OrderBy(item => -item.Value).ToDictionary(item => item.Key, item => item.Value);
		} 
	}
}
