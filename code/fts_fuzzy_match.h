// LICENSE
//
//   This software is in the public domain. Where that dedication is not
//   recognized, you are granted a perpetual, irrevocable license to copy,
//   distribute, and modify this file as you see fit.


#ifndef FTS_FUZZY_MATCH_H
#define FTS_FUZZY_MATCH_H

#include <ctype.h> // for tolower

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
    // iff found then outScore is also set
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

        // Loop over strings
        while (*patternIter != '\0' && *strIter != '\0') 
        {
            const char patternLetter = *patternIter;
            const char strLetter = *strIter;

            if (tolower(patternLetter) == tolower(strLetter))
            {
                // Apply penalty for each letter before the first pattern match
                // Note: std::max because penalties are negative values. So max is smallest penalty.
                if (patternIter == pattern)
                {
                    int count = int(strIter - str);
                    int penalty = std::max(leading_letter_penalty * count, max_leading_letter_penalty); 
                    score += penalty;
                }

                // Apply bonus for consecutive bonuses
                if (prevMatched)
                    score += adjacency_bonus;

                // Apply bonus for matches after a separator
                if (prevSeparator)
                    score += separator_bonus;

                // Apply bonus across camel case boundaries
                if (prevLower && isupper(strLetter))
                    score += camel_bonus;

                prevMatched = true;
                ++patternIter;
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

        // Did not match full pattern
        if (*patternIter != '\0')
            return false;

        outScore = score;
        return true;
    }

} // namespace fts

#endif // FTS_FUZZY_MATCH_H