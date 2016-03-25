lib_fts
===

single-file public domain libraries

library | documentation | description
--------------------- | -------- | ------------- | --------------------------------
**[fts_fuzzy_match.h](https://github.com/forrestthewoods/lib_fts/blob/master/code/fts_fuzzy_match.h)** | [readme](https://github.com/forrestthewoods/lib_fts/blob/master/docs/fuzzy_match.md) | SublimeText inspired fuzzy string matching in C++
**[fts_fuzzy_match.js](https://github.com/forrestthewoods/lib_fts/blob/master/code/fts_fuzzy_match.js)** | [readme](https://github.com/forrestthewoods/lib_fts/blob/master/docs/fuzzy_match.md)  demo | Fuzzy string matching in JavaScript


FAQ
---

#### What is this?
My personal take on [STB](https://github.com/nothings/stb) style file libraries. Small librabries in one or two files that can be trivially drag'n'dropped into an existing project.

No convoluted build systems. No dependency rabbit holes. Easy to read, easy to use.

#### What license?
All source code is public domain. Do whatever you want.

#### What is FTS?
My initials; Forrest Thomas Smith.

#### What language is used?
Several! C++ is the baseline. Multiple language implementations are provided where appropriate.

#### Why C++ and not C?
Because "proper" open source C code is full of gross macros and defines. I'd rather write simple code in a C++ namespace and call it a day.

My opinion is that portability can be achieved in two ways. One, include a dozen defines and macros to let users turn knobs on or off in a million permutations. Two, provide the simplest possible implementation that can be trivially modified by users to meet their specific needs. I prefer the second.

There are also cases where user modification is to be expected. For example a string processing algorithm or 3d math. Users can and should use their own existing string and vector/matrix libraries. In such cases I prefer to provide basic implementations that can be easily modified by users to fit the mold of their project. I believe that to be better than crufty, hard to read macro schemes.

#### Is C++11 used?
Test code is full of C++11, STL, and all kinds of icky things. Library C++ files are written very near to C.
