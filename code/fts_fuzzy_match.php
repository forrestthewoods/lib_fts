<?php
function fuzzy_match($pattern, $str) {
	//score constants
	$ADJACENCY_BONUS = 5; //bonus for adjacent matches
	$SEPERATOR_BOUNUS = 10; //bonus if match occures after a separator
	$CAMEL_BONUS = 10; // bonus if match is uppercase and prev is lower
	$LEADING_LETTER_PENALTY = -3; //penalty applied for evert letter in the $str before the first match
	$MAX_LEADING_LETTER_PENALTY = -9; //max penality for leading letters
	$UNMATCHED_LETTER_PENALTY = -1; // penalty for evert letter that doesn't matter

	//loop variables
	$score = 0;
	$patternIdx = 0;
	$patternLength = strlen($pattern);
	$strIdx = 0;
	$strLength = strlen(str);
	$prevMatched = false;
	$prevLower = false;
	$prevSeparator = true; //true so if the first letter match gets separator bonus

	//use "best" matches letter if multiple string letters match the $pattern
	$bestLetter = null;
	$bestLower = null;
	$bestLetterIdx = null;
	$bestLetterScore = 0;

	$matchedIndecies = [];

	while ($strIdx != $strLength) {
		$patternChar = ($patternIdx != $patternLength) ? $pattern{$strIdx} : null; //check char at current position of index
		$strChar = $str{$strIdx}; //current letter check for entered string
		$patternLower = ($patternChar != null) ? strtolower($patternChar) : null; //converts pattern to lower
		
		//str conversion to upper/lower
		$strLower = strtolower($str);
		$strUpper = strtoupper($str);

		$nextMatch = ($patternChar && $patternLower == $strLower);
		$rematch = ($bestletteer && $patternChar && $bestLower == $strLower);
		$advanced = ($nextMatch && $bestLetter);
		$patternRepeat = ($bestLetter && $patternChar && $bestLower == $patternLower);

		//push to $matchedIndecies if $patternChar and $patternLower are equal to $strLower 
		//		->(so a matched char between pattern and str), 
		//OR pushes to $matchedIndecies if $bestLetter, $patternChar and $bestLower are the same as $patternLower
		//		->(the best letter that matches for lowercase checks)
		if ($advanced || $patternRepeat) {
			$score += $bestLetterScore;
			array_push($matchedIndecies, $bestLetterIdx);
			$bestLetter = null;
			$bestLower= null;
			$bestletterIdx = 0;
			$bestLetterScore = 0;
		}

		if ($nextMatch || $rematch) {
			$newScore = 0;

			//apply penalty for each letter before the first pattern match
			//note: std::max because penalities are negative values. so max is smallest penalty
			if ($pattenIdx == 0) {
				$penalty = max($strIdx * $LEADING_LETTER_PENALTY, $MAX_LEADING_LETTER_PENALTY);
				$score += $penalty;
			}

			//apply bonus to consecutive bonuses
			if($prevMatched) {
				$newScore += $ADJACENCY_BONUS;
			}

			//apply bonus across camel case boundaries. includes "clever" isLetter check
			if ($prevLower && $strChar == $strUpper && $strlower != $strUpper) {
				$newScore += $CAMEL_BONUS;
			}

			//update best letter in str which may be for a "next" letter or a "rematch"
			if ($newScore >= $bestLetterScore) {
				//apply penality for now skipped letter
				if ($bestLetter != null) {
					$score += $UNMATCHED_LETTER_PENALTY;
				}

				$bestletter = $strChar;
				$bestLower = strtolower($bestLetter);
				$bestLetterIdx = $strIdx;
				$bestLetterScore = $newScore;
			}

			$prevMatched = true;

		} else {
			//append unmatch charaters
			$formattedStr += $strChar;

			$score += $UNMATCHED_LETTER_PENALTY;
			$prevMatched = false;
		}

		//includes "clever" isLetter check
		$prevLower = $strChar == $strLower && $strLower != $strUpper;
		$prevSeparator = $strChar == '_' || $strChar == ' ';

		++ $strIdx;
	}

//apply score for last match
	if ($bestLetter) {
		$score += $bestLetterScore;
		array_push($matchedIndecies, $bestLetterIdx);
	}

	//finish out formatter string after last pattern matched
	//build formated string based on matched letters
	$formattedStr = "";
	$lastIdx = 0;
	for ($i = 0; $i < length($matchedIndecies); ++$i) {
		$idx = $matchedIndecies[$i];
		$formattedStr .= substr($str, ($idx - $lastIdx)) . "<b>" . $str{$idx} . "</b>";
		$lastIdx = $idx + 1;
	}
	$formattedStr .= substr($str, $lastIdx, (strlen($str) - $lastIdx));

	$matched = $patternIdx == $patternLength;
	return [$matched, $score, $formattedStr];
}


