// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

#ifndef FTS_HASHUTIL_H
#define FTS_HASHUTIL_H

#include <utility>  // std::pair

// std namespace so STL containers automatically use std::hash specializations
namespace std {

    // 'Generic hash function for stl containers' (via boost)
    // http://www.boost.org/doc/libs/1_33_1/boost/functional/hash/hash.hpp
    // http://stackoverflow.com/questions/6899392/generic-hash-function-for-all-stl-containers
    template <class T>
    inline void hash_combine(std::size_t & seed, const T & v) {
        std::hash<T> hasher;
        seed ^= hasher(v) + 0x9e3779b9 + (seed << 6) + (seed >> 2);
    }

    // std::hash specialization for std::pair<T1,T2>. Relies on std::hash<T1> and std::hash<T2>.
    template <typename T1, typename T2> 
    struct std::hash< std::pair<T1, T2> > {
        inline size_t operator()(std::pair<T1, T2> const & pair) const {
            size_t result = std::hash<T1>()(pair.first);
            std::hash_combine(result, pair.second);
            return result;
        }
    };
}

#endif // FTS_HASHUTIL_H