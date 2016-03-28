// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

#ifndef FTS_TIMER_H
#define FTS_TIMER_H

#include <chrono>   // C++11, high resolution clock

namespace fts {

    class Stopwatch {

      public:
        Stopwatch();
        void Reset();

        float elapsedMilliseconds() const;
        float elapsedMillisecondsAndReset();

      private:
        std::chrono::high_resolution_clock::time_point start;
    };

} // namespace fts

#endif FTS_TIMER_H