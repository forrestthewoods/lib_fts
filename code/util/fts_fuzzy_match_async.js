// LICENSE
//
//   This software is in the public domain. Where that dedication is not
//   recognized, you are granted a perpetual, irrevocable license to copy,
//   distribute, and modify this file as you see fit.

var FTSAsyncMatcher = function (matchFn, pattern, dataSet, onComplete) {
    var MAX_MS_PER_FRAME = 1000/30.0; // 30FPS
    var ITEMS_PER_CHECK = 1000;

    var dataIndex = 0;
    var results = [];

    var resumeTimeout = null;

    var step = function() {
        var stopTime = performance.now() + MAX_MS_PER_FRAME;

        resumeTimeout = null;

        for (; dataIndex < dataSet.length; ++dataIndex)
        {
            if ((dataIndex % ITEMS_PER_CHECK) === 0)
            {
                if (performance.now() > stopTime)
                {
                    resumeTimeout = setTimeout(step, 1);
                    return;
                }
            }

            var str = dataSet[dataIndex];
            var result = matchFn(pattern, str);
            if (result !== null)
                results.push(result);
        }

        resumeTimeout = setTimeout(function() {
            resumeTimeout = null;
            onComplete(results);
        }, 1);
    };

    this.start = function() { 
        step();
    };

    this.cancel = function() {
        if (resumeTimeout !== null)
            clearTimeout(resumeTimeout);
    };
};