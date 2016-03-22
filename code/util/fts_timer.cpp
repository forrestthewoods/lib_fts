// LICENSE
//
//   This software is in the public domain. Where that dedication is not
//   recognized, you are granted a perpetual, irrevocable license to copy,
//   distribute, and modify this file as you see fit.

#include "fts_timer.h"

namespace fts {

    Stopwatch::Stopwatch() {
        Reset();
    }

    void Stopwatch::Reset() {
        start = std::chrono::high_resolution_clock::now();
    }

    float Stopwatch::elapsedMilliseconds() const {
        std::chrono::duration<float, std::milli> ms = std::chrono::high_resolution_clock::now() - start;
        return ms.count();
    }

    float Stopwatch::elapsedMillisecondsAndReset() {
        auto now = std::chrono::high_resolution_clock::now();
        std::chrono::duration<float, std::milli> ms = now - start;
        start = now;
        return ms.count();
    }

} // namespace fts