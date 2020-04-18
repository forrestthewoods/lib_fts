// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.
//
// VERSION
//   0.1.0  (2016-03-28)  Initial release
//
// AUTHOR
//   Forrest Smith
//
// CONTRIBUTORS
//   J�rgen Tjern� - async helper
//   Anurag Awasthi - updated to 0.2.0

const SEQUENTIAL_BONUS = 15; // bonus for adjacent matches
const SEPARATOR_BONUS = 30; // bonus if match occurs after a separator
const CAMEL_BONUS = 30; // bonus if match is uppercase and prev is lower
const FIRST_LETTER_BONUS = 15; // bonus if the first letter is matched

const LEADING_LETTER_PENALTY = -5; // penalty applied for every letter in str before the first match
const MAX_LEADING_LETTER_PENALTY = -15; // maximum penalty for leading letters
const UNMATCHED_LETTER_PENALTY = -1;

/**
 * Returns true if each character in pattern is found sequentially within str
 * @param {*} pattern string
 * @param {*} str string
 */
function fuzzyMatchSimple(pattern, str) {
  let patternIdx = 0;
  let strIdx = 0;
  const patternLength = pattern.length;
  const strLength = str.length;

  while (patternIdx != patternLength && strIdx != strLength) {
    const patternChar = pattern.charAt(patternIdx).toLowerCase();
    const strChar = str.charAt(strIdx).toLowerCase();
    if (patternChar == strChar) ++patternIdx;
    ++strIdx;
  }

  return patternLength != 0 && strLength != 0 && patternIdx == patternLength
    ? true
    : false;
}

/**
 * Does a fuzzy search to find pattern inside a string.
 * @param {*} pattern string        pattern to search for
 * @param {*} str     string        string which is being searched
 * @returns [boolean, number]       a boolean which tells if pattern was
 *                                  found or not and a search score
 */
function fuzzyMatch(pattern, str) {
  const recursionCount = 0;
  const recursionLimit = 10;
  const matches = [];
  const maxMatches = 256;

  return fuzzyMatchRecursive(
    pattern,
    str,
    0 /* patternCurIndex */,
    0 /* strCurrIndex */,
    null /* srcMatces */,
    matches,
    maxMatches,
    0 /* nextMatch */,
    recursionCount,
    recursionLimit
  );
}

function fuzzyMatchRecursive(
  pattern,
  str,
  patternCurIndex,
  strCurrIndex,
  srcMatces,
  matches,
  maxMatches,
  nextMatch,
  recursionCount,
  recursionLimit
) {
  let outScore = 0;

  // Return if recursion limit is reached.
  if (++recursionCount >= recursionLimit) {
    return [false, outScore];
  }

  // Return if we reached ends of strings.
  if (patternCurIndex === pattern.length || strCurrIndex === str.length) {
    return [false, outScore];
  }

  // Recursion params
  let recursiveMatch = false;
  let bestRecursiveMatches = [];
  let bestRecursiveScore = 0;

  // Loop through pattern and str looking for a match.
  let firstMatch = true;
  while (patternCurIndex < pattern.length && strCurrIndex < str.length) {
    // Match found.
    if (
      pattern[patternCurIndex].toLowerCase() === str[strCurrIndex].toLowerCase()
    ) {
      if (nextMatch >= maxMatches) {
        return [false, outScore];
      }

      if (firstMatch && srcMatces) {
        matches = [...srcMatces];
        firstMatch = false;
      }

      const recursiveMatches = [];
      const [matched, recursiveScore] = fuzzyMatchRecursive(
        pattern,
        str,
        patternCurIndex,
        strCurrIndex + 1,
        matches,
        recursiveMatches,
        maxMatches,
        nextMatch,
        recursionCount,
        recursionLimit
      );

      if (matched) {
        // Pick best recursive score.
        if (!recursiveMatch || recursiveScore > bestRecursiveScore) {
          bestRecursiveMatches = [...recursiveMatches];
          bestRecursiveScore = recursiveScore;
        }
        recursiveMatch = true;
      }

      matches[nextMatch++] = strCurrIndex;
      ++patternCurIndex;
    }
    ++strCurrIndex;
  }

  const matched = patternCurIndex === pattern.length;

  if (matched) {
    outScore = 100;

    // Apply leading letter penalty
    let penalty = LEADING_LETTER_PENALTY * matches[0];
    penalty =
      penalty < MAX_LEADING_LETTER_PENALTY
        ? MAX_LEADING_LETTER_PENALTY
        : penalty;
    outScore += penalty;

    //Apply unmatched penalty
    const unmatched = str.length - nextMatch;
    outScore += UNMATCHED_LETTER_PENALTY * unmatched;

    // Apply ordering bonuses
    for (let i = 0; i < nextMatch; i++) {
      const currIdx = matches[i];

      if (i > 0) {
        const prevIdx = matches[i - 1];
        if (currIdx == prevIdx + 1) {
          outScore += SEQUENTIAL_BONUS;
        }
      }

      // Check for bonuses based on neighbor character value.
      if (currIdx > 0) {
        // Camel case
        const neighbor = str[currIdx - 1];
        const curr = str[currIdx];
        if (
          neighbor === neighbor.toLowerCase() &&
          curr === curr.toUpperCase()
        ) {
          outScore += CAMEL_BONUS;
        }
        const isNeighbourSeparator = neighbor == "_" || neighbor == " ";
        if (isNeighbourSeparator) {
          outScore += SEPARATOR_BONUS;
        }
      } else {
        // First letter
        outScore += FIRST_LETTER_BONUS;
      }
    }

    // Return best result
    if (recursiveMatch && (!matched || bestRecursiveScore > outScore)) {
      // Recursive score is better than "this"
      matches = [...bestRecursiveMatches];
      outScore = bestRecursiveScore;
      return [true, outScore];
    } else if (matched) {
      // "this" score is better than recursive
      return [true, outScore];
    } else {
      return [false, outScore];
    }
  }
  return [false, outScore];
}

/**
 * Strictly optional utility to help make using fts_fuzzy_match easier for large data sets
 * Uses setTimeout to process matches before a maximum amount of time before sleeping
 *
 * To use:
 *  const asyncMatcher = new ftsFuzzyMatchAsync(fuzzyMatch, "fts", "ForrestTheWoods",
 *                                              function(results) { console.log(results); });
 *  asyncMatcher.start();
 *
 * @param {*} matchFn   function      Matching function - fuzzyMatchSimple or fuzzyMatch.
 * @param {*} pattern   string        Pattern to search for.
 * @param {*} dataSet   array         Array of string in which pattern is searched.
 * @param {*} onComplete function     Callback function which is called after search is complete.
 */
function ftsFuzzyMatchAsync(matchFn, pattern, dataSet, onComplete) {
  const ITEMS_PER_CHECK = 1000; // performance.now can be very slow depending on platform
  const results = [];
  const max_ms_per_frame = 1000.0 / 30.0; // 30FPS
  let dataIndex = 0;
  let resumeTimeout = null;

  // Perform matches for at most max_ms
  function step() {
    clearTimeout(resumeTimeout);
    resumeTimeout = null;

    var stopTime = performance.now() + max_ms_per_frame;

    for (; dataIndex < dataSet.length; ++dataIndex) {
      if (dataIndex % ITEMS_PER_CHECK == 0) {
        if (performance.now() > stopTime) {
          resumeTimeout = setTimeout(step, 1);
          return;
        }
      }

      var str = dataSet[dataIndex];
      var result = matchFn(pattern, str);

      // A little gross because fuzzy_match_simple and fuzzy_match return different things
      if (matchFn == fuzzyMatchSimple && result == true) results.push(str);
      else if (matchFn == fuzzyMatch && result[0] == true) results.push(result);
    }

    onComplete(results);
    return null;
  }

  // Abort current process
  this.cancel = function() {
    if (resumeTimeout !== null) clearTimeout(resumeTimeout);
  };

  // Must be called to start matching.
  // I tried to make asyncMatcher auto-start via "var resumeTimeout = step();"
  // However setTimout behaving in an unexpected fashion as onComplete insisted on triggering twice.
  this.start = function() {
    step();
  };

  // Process full list. Blocks script execution until complete
  this.flush = function() {
    max_ms_per_frame = Infinity;
    step();
  };
}
