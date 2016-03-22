// LICENSE
//
//   This software is in the public domain. Where that dedication is not
//   recognized, you are granted a perpetual, irrevocable license to copy,
//   distribute, and modify this file as you see fit.


// Returns true if each character in pattern is found sequentially within str
function fuzzy_match_simple(pattern, str) {

    var patternIdx = 0;
    var strIdx = 0;
    var patternLength = pattern.length;
    var strLength = str.length;

    while (patternIdx != patternLength && strIdx != strLength) {
        var patternChar = pattern.charAt(patternIdx).toLowerCase();
        var strChar = str.charAt(strIdx).toLowerCase();
        if (patternChar == strChar)
            ++patternIdx;
        ++strIdx;
    }

    return patternLength != 0 && strLength != 0 && patternIdx == patternLength ? true : false;
}

// Returns [bool, score, formattedStr]
// bool: true if each character in pattern is found sequentially within str
// score: integer; higher is better match. Value has no intrinsic meaning. Range varies with pattern. 
//        Can only compare scores with same search pattern.
// formattedStr: input str with matched characters marked in <b> tags. Delete if unwanted.
function fuzzy_match(pattern, str) {
   
    // Score consts
    var adjacency_bonus = 5;                // bonus for adjacent matches
    var separator_bonus = 10;               // bonus if match occurs after a separator
    var camel_bonus = 10;                   // bonus if match is uppercase and prev is lower
    var leading_letter_penalty = -3;        // penalty applied for every letter in str before the first match
    var max_leading_letter_penalty = -9;    // maximum penalty for leading letters
    var unmatched_letter_penalty = -1;      // penalty for every letter that doesn't matter

    // Loop variables
    var score = 0;
    var patternIdx = 0;
    var patternLength = pattern.length;
    var strIdx = 0;
    var strLength = str.length;
    var prevMatched = false;
    var prevLower = false;
    var prevSeparator = true;       // true so if first letter match gets separator bonus

    var formattedStr = "";

    // Loop over strings
    while (patternIdx != patternLength && strIdx != strLength) {
        var patternChar = pattern.charAt(patternIdx);
        var strChar = str.charAt(strIdx);

        if (patternChar.toLowerCase() == strChar.toLowerCase()) {
            // Apply penalty for each letter before the first pattern match
            // Note: std::max because penalties are negative values. So max is smallest penalty.
            if (patternIdx == 0) {
                var penalty = Math.max(strIdx * leading_letter_penalty, max_leading_letter_penalty);
                score += penalty;
            }

            // Apply bonus for consecutive bonuses
            if (prevMatched)
                score += adjacency_bonus;

            // Apply bonus for matches after a separator
            if (prevSeparator)
                score += separator_bonus;

            // Apply bonus across camel case boundaries
            if (prevLower && strChar == strChar.toUpperCase())
                score += camel_bonus;

            // Formatted string; wrap matched characters in bold
            formattedStr += "<b>" + strChar + "</b>";

            prevMatched = true;
            ++patternIdx;
        }
        else {
            // Append unmatch characters
            formattedStr += strChar;

            score += unmatched_letter_penalty;
            prevMatched = false;
        }

        prevLower = strChar == strChar.toLowerCase();
        prevSeparator = strChar == '_' || strChar == ' ';

        ++strIdx;
    }

    // Finish out formatted string after last pattern matched
    formattedStr += str.substr(strIdx, strLength - strIdx);

    return [patternIdx == patternLength, score, formattedStr];
}
