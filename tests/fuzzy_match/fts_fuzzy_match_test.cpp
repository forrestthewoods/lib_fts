// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <unordered_map>
#include <queue>

#include "..\..\code\fts_fuzzy_match.h"
#include "..\..\code\util\fts_timer.h"


int main(int argc, char *argv[]) {

    // Dictionary
    std::vector<std::string> dictionary;
    
    auto countMatches = [&dictionary](std::string const & pattern) -> int { 
        int matches = 0;
        for (auto && entry : dictionary)
            if (fts::fuzzy_match(pattern.c_str(), entry.c_str()) == 1)
                ++matches;

        return matches; 
    };

    auto alphabeticalMatches = [&dictionary](std::string const & pattern) {
        std::vector<std::string const *> matches;
        for (auto && entry : dictionary)
            if (fts::fuzzy_match(pattern.c_str(), entry.c_str()) == 1)
                matches.push_back(&entry);

        return matches;
    };

    auto scoredMatches = [&dictionary](std::string const & pattern) {
        std::vector<std::pair<int, std::string const*>> matches;
        int score;
        for (auto && entry : dictionary)
            if (fts::fuzzy_match(pattern.c_str(), entry.c_str(), score))
                matches.emplace_back(score, &entry);

        std::sort(matches.begin(), matches.end(), [](auto && a, auto && b) { return a.first > b.first; });
        
        return matches;
    };

    // Open file
    using namespace std::string_literals;
    std::string path = argc > 1 ? argv[1] : "no file specified"s;
    std::cout << "Reading [" << path << "]" << std::endl;
    std::ifstream infile(path);
    if (!infile.good()) {
        std::cout << "Failed to open file." << std::endl;
        return 0;
    }

    // Read file
    fts::Stopwatch stopwatch;
    std::string entry;
    while (std::getline(infile, entry))
        dictionary.push_back(std::move(entry));

    float time = stopwatch.elapsedMilliseconds();
    std::cout << "Read [" << dictionary.size() << "] entries in " << time << "ms" << std::endl << std::endl;


    // Input Loop
    std::string option;
    std::string pattern;

    while (1) {
        std::cout << "1. Count Matches" << std::endl;
        std::cout << "2. Print Matches (Alphabetical)" << std::endl;
        std::cout << "3. Print Matches (By Score)" << std::endl;
        std::cout << "4. Exit" << std::endl << std::endl;
        std::cout << "> ";
        std::getline(std::cin, option);
        std::cout << std::endl;

        if (option == "1" || option == "2" || option == "3") {

            // Read pattern from std::cin
            std::cout << "Enter search pattern" << std::endl << std::endl << "> ";
            std::getline(std::cin, pattern);
            std::cout << std::endl << std::endl << "Searching for [" << pattern << "] ..." << std::endl;

            if (option == "1") {
                // Count Matches
                stopwatch.Reset();
                int matches = countMatches(pattern);
                time = stopwatch.elapsedMilliseconds();

                std::cout << "Found " << matches << " matches in " << time << "ms" << std::endl << std::endl;
            }
            else if (option == "2") {
                // Print Matches (Alphabetical)
                stopwatch.Reset();
                auto results = alphabeticalMatches(pattern);
                time = stopwatch.elapsedMilliseconds();

                for (auto && result : results)
                    std::cout << *result << std::endl;
                std::cout << std::endl << "Found " << results.size() << " matches in " << time << "ms" << std::endl << std::endl;
            }
            else if (option == "3") {
                // Print Matches (By Score)
                stopwatch.Reset();
                auto results = scoredMatches(pattern);
                time = stopwatch.elapsedMilliseconds();

                for (auto && result : results)
                    std::cout << result.first << " - " << *result.second << std::endl;
                std::cout << std::endl << "Found " << results.size() << " matches in " << time << "ms" << std::endl << std::endl;
            }
        }
        else if (option == "4") {
            // Quit
            break;
        }
    }
}