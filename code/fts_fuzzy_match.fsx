type State =
    { Score : int
      Index : int
      PatternIdx : int
      BestLetter : char option
      BestLower : char option
      BestLetterIdx : int option
      BestLetterScore : int
      MatchedIndices : int list
      PrevMatched : bool
      PrevLower : bool
      PrevSeperator : bool }
with
    static member Empty =
        { Score = 0
          Index = 0
          PatternIdx = 0
          BestLetter = None
          BestLower = None
          BestLetterIdx = None
          BestLetterScore = 0
          MatchedIndices = []
          PrevMatched = false
          PrevLower = false
          PrevSeperator = true }

let fuzzyMatch (pattern : string) (str : string) useLookup =
    // Score consts
    let adjacency_bonus = 5;                // bonus for adjacent matches
    let separator_bonus = 10;               // bonus if match occurs after a separator
    let camel_bonus = 10;                   // bonus if match is uppercase and prev is lower
    let leading_letter_penalty = -3;        // penalty applied for every letter in str before the first match
    let max_leading_letter_penalty = -9;    // maximum penalty for leading letters
    let unmatched_letter_penalty = -1;      // penalty for every letter that doesn't matter

    let patternLength = pattern.Length
    let state =
        str
        |> Seq.fold(fun (state : State) (strChar : char) ->
            let patternChar = 
                if state.PatternIdx <> patternLength then
                    pattern.Chars(state.PatternIdx) |> Some
                else
                    None
            let patternLower = patternChar |> Option.bind(fun x -> System.Char.ToLower(x) |> Some)
            let strLower = System.Char.ToLower(strChar)
            let strUpper = System.Char.ToUpper(strChar)
            let nextMatch = 
                match patternLower with
                | None -> false
                | Some x -> x = strLower
            let rematch =
                match state.BestLetter with
                | None -> false
                | Some x -> x = strLower
            let advanced = nextMatch && state.BestLetter.IsSome
            let patternRepeat = 
                if state.BestLetter.IsSome && patternChar.IsSome then 
                    match state.BestLower with
                    | None -> false
                    | Some x -> Some x = patternLower
                else
                    false
        
            let state =
                if advanced || patternRepeat then
                    { state with
                        Score = state.Score + state.BestLetterScore
                        MatchedIndices = state.MatchedIndices @ [state.BestLetterIdx.Value]
                        BestLetter = None
                        BestLower = None
                        BestLetterIdx = None
                        BestLetterScore = 0 }
                else
                    state
            let state = 
                if nextMatch || rematch then
                    // Apply penalty for each letter before the first pattern match
                    // Note: System.Math because penalties are negative values. So max is smallest penalty.
                    let state = 
                        if state.PatternIdx = 0 then
                            let penalty = System.Math.Max(state.Index * leading_letter_penalty, max_leading_letter_penalty)
                            { state with Score = state.Score + penalty }
                        else
                            state

                    let newScore = 0
                    
                    // Apply bonus for consecutive bonuses
                    let newScore =
                        if state.PrevMatched then
                            newScore + adjacency_bonus
                        else
                            newScore
                    
                    // Apply bonus for matches after a separator
                    let newScore =
                        if state.PrevSeperator then
                            newScore + separator_bonus
                        else
                            newScore

                    // Apply bonus across camel case boundaries. Includes "clever" isLetter check.
                    let newScore =
                        if state.PrevLower && (strChar = strUpper) && strLower <> strUpper then
                            newScore + camel_bonus
                        else
                            newScore

                    // Update pattern index IFF the next pattern letter was matched
                    let state = 
                        if nextMatch then
                            { state with 
                                PatternIdx = state.PatternIdx + 1 }
                        else
                            state 

                    // Update best letter in str which may be for a "next" letter or a "rematch"
                    let state =
                        if newScore >= state.BestLetterScore then
                            
                            // Apply penalty for now skipped letter
                            let score = 
                                match state.BestLetter with
                                | None -> state.Score
                                | Some x -> state.Score + unmatched_letter_penalty

                            { state with
                                Score = score
                                BestLetter = Some strChar
                                BestLower = Some strLower
                                BestLetterIdx = Some state.Index 
                                BestLetterScore = newScore }
                        else
                            state
                    { state with PrevMatched = true }
                else
                    { state with 
                        Score = state.Score + unmatched_letter_penalty
                        PrevMatched = false }
            let state = 
                { state with 
                    PrevLower = strChar = strLower && strLower <> strUpper
                    PrevSeperator = strChar = '_' || strChar = ' '
                    Index = state.Index + 1 }
            state) State.Empty

    let state =
        if state.BestLetter.IsSome then
            { state with 
                Score = state.Score + state.BestLetterScore
                MatchedIndices = state.MatchedIndices @ [state.BestLetterIdx.Value] }
        else
            state
    
    let matched = patternLength = state.PatternIdx
    matched, state.Score, state.MatchedIndices