# Fuzzy Match

A [Sublime Text](https://www.sublimetext.com/) inspired fuzzy match algorithm

## Version History

(2016-3-25)  Version 0.1.0  First release

## Usage

Both C++ and JS implementation contain two functions. A simple version and a ranked version.

###### C++
```c++
bool fuzzy_match(const char * pattern, const char * str);
bool fuzzy_match(const char * pattern, const char * str, int &score);
```

###### JavaScript
```javascript
fuzzy_match_simple(pattern, str) -> bool matched
fuzzy_match(pattern, str) -> [bool matched, int score, string formattedString]
```

The simple version returns true if each character in the pattern appears in the test string in order.

The scored version does the same but also provides an integer score. The JavaScript function with score also provides a formatted string. If there is a macth it is the input string with each matched character markeded up with a \<b\> tag.

The numerical value of score value is abstract in nature. It has no meaning other than higher is better. Scores ranges depend on the search pattern. Longer search patterns have higher theoretical max scores. Therefore scores can only be compared when they came from the same pattern.

## Examples

```javascript
fuzzy_match("ftw", "ForrestTheWoods") = true
fuzzy_match("fwt", "ForrestTheWoods") = false
fuzzy_match("gh", "GitHub") = true

fuzzy_match("otw", "Power of the Wild", score) = true, score = 14
fuzzy_match("otw", "Druid of the Claw", score) = true, score = -3
fuzzy_match("otw", "Frostwolf Grunt", score) = true, score = -13
```

## FAQ

### Why write this?

Because fuzzy matching makes the vast majority of search fields better. Not just a little better either. A lot better. So much better that once you get used to fuzzy matching anything NOT fuzzy is infuriating.

### API Considerations

I chose to provide a bare bones API. There are two functions that operate one pattern on one string and that's it. How to best make use of this inner loop function depends highly on your environment. 

### Why C++?

I know how to write performant C++ code. I wanted to write something usably fast. I feel my straight forward implementation achieved that. (More later.)

### Why JavaScript?

Because I want to make the world a better place. And most of the world is now in JavaScript on the web.

I don't actually know a damn thing about JavaScript. Like at all. I'm a video game programmer. Not a webdev. So the JavaScript version could probably be made a lot faster. I would love pull requests of this nature.

### How is the score calculated?

Carefully. It's a balance between different factors. I don't believe there is a "correct" answer. It depends on your problem space. SublimeText's fuzzy algorithm is used primarily for filenames and function/class names. My goal was achieve comparable results in this scenario.

Each letter is worth some number of points. Non-matching letters lose points. Matching letters get bonus points if certain critera are met. Sequential matches get a bonus. Matches appearing after a space or _ get a bonus. Matching letters which are uppercase and preceded by a lowercase letter receive a "camel case" bonus. This is especially useful for filenames and function names.

### Special Sauce

Ok that's not a question. But if there is any special sauce to this library it's this.

There is some extra cleverness in that multiple letters may match a pattern character. Consider fuzzy_match("ftw", "ForrestTheWoods", score). The 't' in 'ftw' could match either the 't' at the end of 'Forrest' or the 'T' from 'The'. Based on our scoring critera the 'T' receives a camel case bonus so it's worth more points than 't'. 

This makes a huge difference when sorting results. It's also what makes the algorithm not achievable by regular expressions. 

### C++ Performance

[Grep is fast](https://lists.freebsd.org/pipermail/freebsd-current/2010-August/019310.html). Grep avoids looking at most bytes. Fuzzy matching by it's very nature requires looking at every byte. So it's going to be a lot slower.

Under tests I have provided fts_fuzzy_match_test.cpp. It is a relatively naive C++11 console application. One of the data files is a 4mb txt file containing 355,000 words. This words are stored in a std::vector<std::string> container.
On my Core i5-4670 @ 3.4Ghz Windows 10 PC it takes ~30 milliseconds to perform a scored fuzzy match against all three hundred and fifty five thousand strings. Based on that I consider my single function fast enough for now.

The code could be trimmed a little. I've decided to leave it slightly more verbose for clarity.

### JavaScript Performance

I don't actually know anything about JavaScript. I'm a video game developer. Not a webdev. I'm probably doing some really stupid things. Performance related pull requests would be great.

Suffice to say the JavaScript version is a lot slower. The actual fuzzy_match function seems to be about 20x to 30x slower in JS than C++. It's not nearly as fast as I would like. In my demo I capped the number of displayed results at 200 because I don't know how to update the DOM in an efficient way.

### Language Support

I know nothing about supporting multiple languages. The C++ code operates on char const *. It makes use of functions such as tolower and toupper. That probably doesn't make sense in other languages. The JavaScript version isn't much different.

I do not intend to provide better language support in C++. If you're using the C++ version then you probably have your own internal string class and you should probably port my function to make use of that. Replace tolower and toupper calls with whatever makes sense for your project and your class.

For JavaScript I would consider pull requests that improve language support. It's so far outside my realm of expertise I can barely comment.

### Why is there a simple version?

I included the simple version because it's an easy drop-in replacement. If your project has user input sub-string matching then simple fuzzy match could be a one line change. If users find that to be an improvement then they may be more likely to take the time to implement the more advanced version.

### Future Work

It depends on if anyone uses this. Highest priority would be JavaScript performance. After that I'm not sure. If anyone uses this I'm open to suggestions. If there are use cases where it breaks down I'd love to hear that as well. The current score calculation is more art than science.