lib_fts
===

single-file public domain libraries

library | language | documentation | description
--------------------- | -------- | ------------- | --------------------------------
fts_fuzzy_match | [C++](https://github.com/forrestthewoods/lib_fts/blob/master/code/fts_fuzzy_match.h), [JavaScript](https://github.com/forrestthewoods/lib_fts/blob/master/code/fts_fuzzy_match.js) | [readme](https://github.com/forrestthewoods/lib_fts/blob/master/docs/fuzzy_match.md), [blog](https://blog.forrestthewoods.com/reverse-engineering-sublime-text-s-fuzzy-match-4cffeed33fdb#.m9cmfqknc), [demo](https://s3-us-west-2.amazonaws.com/forrestthewoods.staticweb/lib_fts/tests/fuzzy_match/fts_fuzzy_match_test.html) | Fuzzy string matching inspired by Sublime Text


FAQ
---

#### What is this?
My personal take on [STB](https://github.com/nothings/stb) style file libraries. Small librabries in one or two files that can be trivially drag'n'dropped into an existing project.

No convoluted build systems. No dependency rabbit holes. Easy to read, easy to use.

#### What license?
All source code is embedded with the following license:

> This software is dual-licensed to the public domain and under the following license: you are granted a perpetual, irrevocable license to copy, modify, publish, and distribute this file as you see fit.

#### What is FTS?
My initials; Forrest Thomas Smith.

#### What language is used?
Several! C++ is the baseline. Multiple language implementations are provided where appropriate.

#### Why C++ and not C?
Because "proper" open source C code is full of gross macros and defines. I'd rather write simple code in a C++ namespace and call it a day.

My opinion is that portability can be achieved in two ways. One, include a dozen defines and macros to let users turn knobs on or off resulting in a million permutations. Two, provide the simplest possible implementation that can be trivially modified by users to meet their specific needs. I prefer writing and using the second.

There are also cases where user modification is to be expected. For example a string processing algorithm or any 3d math. Users can and should use their existing string and vector/matrix libraries. In such cases I prefer to provide basic implementations that can be easily modified by users to fit the mold of their project. I believe that to be better than crufty, inscrutible macro schemes.

#### Is C++11 used?
Core libs linked on this page are written in C++ but very near to C. 

Test code is full of C++11, STL, and all kinds of icky things. Tread with caution!

Files in code/util are fully independent but not significant enough to warrant root status. They may be C++ oriented and may use C++11/14 features.
