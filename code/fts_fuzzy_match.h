// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

// VERSION 0.1.0

#ifndef FTS_FUZZY_MATCH_H
#define FTS_FUZZY_MATCH_H

#include <ctype.h> // ::tolower, ::toupper

namespace fts {
    
    // Returns true if each character in pattern is found sequentially within str
    static bool fuzzy_match(char const * pattern, char const * str) 
    {
        while (*pattern != '\0' && *str != '\0')  {
            if (tolower(*pattern) == tolower(*str))
                ++pattern;
            ++str;
        }

        return *pattern == '\0' ? true : false;
    }

    // Returns true if each character in pattern is found sequentially within str
    // iff found then outScore is also set. Score value has no intrinsic meaning. Range varies with pattern. 
    // Can only compare scores with same search pattern.
    static bool fuzzy_match(char const * pattern, char const * str, int & outScore) 
    {
        // Score consts
        const int adjacency_bonus = 5;              // bonus for adjacent matches
        const int separator_bonus = 10;             // bonus if match occurs after a separator
        const int camel_bonus = 10;                 // bonus if match is uppercase and prev is lower
        
        const int leading_letter_penalty = -3;      // penalty applied for every letter in str before the first match
        const int max_leading_letter_penalty = -9;  // maximum penalty for leading letters
        const int unmatched_letter_penalty = -1;    // penalty for every letter that doesn't matter


        // Loop variables
        int score = 0;
        char const * patternIter = pattern;
        char const * strIter = str;
        bool prevMatched = false;
        bool prevLower = false;
        bool prevSeparator = true;                  // true so if first letter match gets separator bonus

        // Use "best" matched letter if multiple string letters match the pattern
        char const * bestLetter = NULL;
        int bestLetterScore = 0;

        // Loop over strings
        while (*strIter != '\0') 
        {
            const char patternLetter = *patternIter;
            const char strLetter = *strIter;

            bool nextMatch = *patternIter != '\0' && tolower(patternLetter) == tolower(strLetter);
            bool rematch = bestLetter && tolower(*bestLetter) == tolower(strLetter);

            bool advanced = nextMatch && bestLetter;
            bool patternRepeat = bestLetter && patternIter != '\0' && tolower(*bestLetter) == tolower(patternLetter);

            if (advanced || patternRepeat) 
            {
                score += bestLetterScore;
                bestLetter = NULL;
                bestLetterScore = 0;
            }

            if (nextMatch || rematch)
            {
                int newScore = 0;

                // Apply penalty for each letter before the first pattern match
                // Note: std::max because penalties are negative values. So max is smallest penalty.
                if (patternIter == pattern)
                {
                    int count = int(strIter - str);
                    int penalty = leading_letter_penalty * count;
                    if (penalty < max_leading_letter_penalty)
                        penalty = max_leading_letter_penalty;

                    score += penalty;
                } 

                // Apply bonus for consecutive bonuses
                if (prevMatched)
                    newScore += adjacency_bonus;

                // Apply bonus for matches after a separator
                if (prevSeparator)
                    newScore += separator_bonus;

                // Apply bonus across camel case boundaries
                if (prevLower && isupper(strLetter))
                    newScore += camel_bonus;

                // Update pattern iter IFF the next pattern letter was matched
                if (nextMatch)
                    ++patternIter;

                // Update best letter in str which may be for a "next" letter or a rematch
                if (newScore >= bestLetterScore) 
                {
                    // Apply penalty for now skipped letter
                    if (bestLetter != NULL)
                        score += unmatched_letter_penalty;

                    bestLetter = strIter;
                    bestLetterScore = newScore;
                }

                prevMatched = true;
            }
            else
            {
                score += unmatched_letter_penalty;
                prevMatched = false;
            }

            // Separators should be more easily defined
            prevLower = islower(strLetter) != 0;
            prevSeparator = strLetter == '_' || strLetter == ' ';

            ++strIter;
        }

        // Apply score for last match
        if (bestLetter) 
            score += bestLetterScore;

        // Did not match full pattern
        if (*patternIter != '\0')
            return false;

        outScore = score;
        return true;
    }

} // namespace fts

#endif // FTS_FUZZY_MATCH_H