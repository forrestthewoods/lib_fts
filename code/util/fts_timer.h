// LICENSE
//
//   This software is in the public domain. Where that dedication is not
//   recognized, you are granted a perpetual, irrevocable license to copy,
//   distribute, and modify this file as you see fit.

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