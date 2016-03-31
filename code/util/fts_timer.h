// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

#ifndef FTS_TIMER_H
#define FTS_TIMER_H

#include <chrono>   // C++11, high resolution clock

namespace fts {

    // Stopwatch
    //   A basic utility for timing things
    class Stopwatch 
    {
      public:
        Stopwatch();
        void Reset();

        int64_t elapsedNanoseconds() const;
        int64_t elapsedMicroseconds() const;
        double elapsedMilliseconds() const;
        double elapsedSeconds() const;

        int64_t elapsedNanosecondsAndReset();
        int64_t elapsedMicrosecondsAndReset();
        double elapsedMillisecondsAndReset();
        double elapsedSecondsAndReset();

      private:
        std::chrono::high_resolution_clock::time_point now() const;

        std::chrono::high_resolution_clock::time_point start;
    };


    // Timer
    //   A basic utility for setting a timer and querying if it's finished
    class Timer 
    {
      public:
        explicit Timer(double durationInSeconds);

        void Reset();
        void Reset(double durationInSeconds);
        
        bool Finished() const;
    
      private:
        Stopwatch stopwatch;
        double duration;
    };



    // Stopwatch Implementation
    inline Stopwatch::Stopwatch() {
        Reset();
    }

    inline void Stopwatch::Reset() {
        start = now();
    }

    inline int64_t Stopwatch::elapsedNanoseconds() const {
        std::chrono::nanoseconds ns = now() - start;
        return ns.count();
    }

    inline int64_t Stopwatch::elapsedMicroseconds() const {
        auto us = std::chrono::duration_cast<std::chrono::duration<int64_t, std::micro>>(now() - start);
        return us.count();
    }

    inline double Stopwatch::elapsedMilliseconds() const {
        std::chrono::duration<double, std::milli> ms = now() - start;
        return ms.count();
    }

    inline double Stopwatch::elapsedSeconds() const {
        std::chrono::duration<double, std::ratio<1,1>> s = now() - start;
        return s.count();
    }

    inline int64_t Stopwatch::elapsedNanosecondsAndReset() {
        int64_t ns = elapsedNanoseconds();
        Reset();
        return ns;
    }

    inline int64_t Stopwatch::elapsedMicrosecondsAndReset() {
        int64_t ms = elapsedMicroseconds();
        Reset();
        return ms;
    }

    inline double Stopwatch::elapsedMillisecondsAndReset() {
        double ms = elapsedMilliseconds();
        Reset();
        return ms;
    }

    inline double Stopwatch::elapsedSecondsAndReset() {
        double s = elapsedSeconds();
        Reset();
        return s;
    }

    inline std::chrono::high_resolution_clock::time_point Stopwatch::now() const {
        return std::chrono::high_resolution_clock::now();
    }



    // Timer implementation
    inline Timer::Timer(double durationInSeconds)
        : duration(durationInSeconds)
    {
    }

    inline void Timer::Reset() {
        stopwatch.Reset();
    }

    inline void Timer::Reset(double durationInSeconds) {
        duration = durationInSeconds;
        Reset();
    }

    inline bool Timer::Finished() const {
        return stopwatch.elapsedSeconds() > duration;
    }

} // namespace fts

#endif FTS_TIMER_H