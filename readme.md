lib_fts
===

single-file public domain libraries

lib | lang | doc | desc
---- | --- | --- | ---
fts_ballistic_projectile | [C# (Unity)](https://github.com/forrestthewoods/lib_fts/blob/master/code/fts_ballistic_trajectory.cs) | [blog](https://blog.forrestthewoods.com/solving-ballistic-trajectories-b0165523348c#.krla7uaz8), [demo](https://dl.dropboxusercontent.com/u/2152526/fts_ballistic_trajectory_web/index.html) | Solve ballistic trajectory firing angles
fts_fuzzy_match | [C++](https://github.com/forrestthewoods/lib_fts/blob/master/code/fts_fuzzy_match.h), [JavaScript](https://github.com/forrestthewoods/lib_fts/blob/master/code/fts_fuzzy_match.js) | [readme](https://github.com/forrestthewoods/lib_fts/blob/master/docs/fuzzy_match.md), [blog](https://blog.forrestthewoods.com/reverse-engineering-sublime-text-s-fuzzy-match-4cffeed33fdb#.m9cmfqknc), [demo](https://s3-us-west-2.amazonaws.com/forrestthewoods.staticweb/lib_fts/tests/fuzzy_match/fts_fuzzy_match_test.html) | Fuzzy string matching inspired by Sublime Text

## Ports

These are externally hosted, user written ports. I can not guarantee they are correct, up to date, or non-hostile. User beware!

lib | lang
---| ---
fts_fuzzy_match | [C#](https://gist.github.com/CDillinger/2aa02128f840bdca90340ce08ee71bc2), [F#](https://github.com/xavierzwirtz/lib_fts/blob/939fc8730334a97156ca1e0791ae11250154a1f4/code/fts_fuzzy_match.fsx), [Lua](https://gist.github.com/blake-mealey/f7752f95aed71fe23428abb0ffba2c96), [PHP](https://github.com/detectiveYarmas/lib_fts/blob/master/code/fts_fuzzy_match.php), [Python](https://gist.github.com/menzenski/f0f846a254d269bd567e2160485f4b89)


FAQ
===

#### What is lib_fts?
My personal take on [STB](https://github.com/nothings/stb) style file libraries. Small librabries in one or two files that can be trivially drag'n'dropped into an existing project.

No convoluted build systems. No dependency rabbit holes. Easy to read, easy to use.

#### What license?
All source code is embedded with the following license:

> This software is dual-licensed to the public domain and under the following license: you are granted a perpetual, irrevocable license to copy, modify, publish, and distribute this file as you see fit.

#### What is FTS?
My initials; Forrest Thomas Smith.

#### What language is used?
Several! C++ is my baseline. Different or multiple languages are used where appropriate.

#### Why C++ and not C?
Because "proper" open source C code is full of gross macros and defines. I'd rather write simple code in a C++ namespace and call it a day.

Open source, cross-platform C often include a dozen defines and macros to let users turn knobs on or off resulting in a million permutations. An alternative solution is to provide the simplest possible implementation that can trivially modified by users to meet their specific needs. I prefer both writing and using the second.

There are some cases where user modification is expected. For example, a string processing algorithm or anything involving 3d math. Users can and should use their existing string or vector/matrix libraries. In such cases I prefer to provide basic implementations that be easily modified by the user. I believe that to be better than crufty, inscrutible macro schemes.

#### Is C++11 used?
Core libs linked on this page are written in C++ but very near to C. 

Test code is full of C++11, STL, and all kinds of icky things. Tread with caution!

Files in code/util are fully independent but not significant enough to warrant root status. They may be C++ oriented and may use C++11/14 features.

