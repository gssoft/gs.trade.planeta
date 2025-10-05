EventLog = function(domOutputElementId, boolOnOrOff) {
    var isOn = boolOnOrOff;
    //  domElement = document.getElementById(domOutputElementId);
    
    var domElement = $(domOutputElementId);
    if (typeof domElement === 'undefined')
        alert("Invalid LogDomOutputElementId= " + domOutputElementId);

    var addLogItem = function(logStr) {
        if (!isOn)
            return;
        var dt = new Date();
        // $('#Output').prepend('<p>' + dt.toLocaleTimeString() + ' ' + logStr + '</p>');
        //$(domOutputElementId).prepend('<p>' + dt.toLocaleTimeString() + ' ' + logStr + '</p>');        
        domElement.append('<p><strong>' + dt.toLocaleTimeString() + ' ' + logStr + '</strong></p>');
    },
        preaddLogItem = function (logStr) {
            if (!isOn)
                return;
            var dt = new Date();
            domElement.prepend('<p>' + dt.toLocaleTimeString() + ' ' + logStr + '</p>');
        };
    var setOn = function() {
        isOn = true;
    };
    var setOff = function() {
        isOn = false;
    };

    return {
        addLogItem: addLogItem,
        preaddLogItem : preaddLogItem,
        setOn: setOn,
        setOff: setOff
    };
};

var Requester = function(request1, onSuccess, onError, request2, onSuccess2, onError2) {
    var getData = function(request) {
        var r = checkRequestStr(request);
        if (r === 'undefined')
            return;
        $.ajax({
            type: 'GET',
            url: r,
            dataType: 'json',
            success: function(data) {
                onSuccess(data);
            },
            error: function() {
                onError();
            }
        });
    },
        getDataT = function(request, timeInterval) {
            var toContinue = 0;
            if (timeInterval < 1000)
                timeInterval = 5000;
            var r = checkRequestStr(request);
            if (r === 'undefined')
                return;
            $.ajax({
                type: 'GET',
                url: r,
                dataType: 'json',
                success: function(data) {
                    toContinue = onSuccess(data);
                    if (toContinue > 0)
                        setTimeout(getDataT, timeInterval, r, timeInterval);
                },
                error: function() {
                    toContinue = onError();
                    if (toContinue > 0)
                        setTimeout(getDataT, timeInterval, r, timeInterval);
                }
            });
        },
        getDataT2 = function(req1, req2, timeInterval) {
            var toContinue = 0;

            if (timeInterval < 1000)
                timeInterval = 5000;

            var r1 = checkRequestStr(req1, request1);
            if (r1 === 'undefined')
                return;

            $.ajax({
                type: 'GET',
                url: r1,
                dataType: 'json',
                success: function(data) {
                    toContinue = onSuccess(data);
                    if (toContinue > 0) {
                        var r2 = checkRequestStr(req2, request2);
                        if (r2 === 'undefined')
                            return;
                        $.ajax({
                            type: 'GET',
                            url: r2,
                            dataType: 'json',
                            success: function(data2) {
                                toContinue = onSuccess2(data2);
                                if (toContinue > 0) {
                                    setTimeout(getDataT2, timeInterval, r1, r2, timeInterval);
                                }
                            },
                            error: function() {
                                toContinue = onError2();
                                if (toContinue > 0)
                                    setTimeout(getDataT2, timeInterval, req1, req2, timeInterval);
                            }
                        });
                    }
                },
                error: function() {
                    toContinue = onError();
                    if (toContinue > 0)
                        setTimeout(getDataT2, timeInterval, req1, req2, timeInterval);
                }
            });
        },
        checkRequestStr = function(r1, r2) {
            var r =
                typeof r1 !== 'undefined'
                    ? r1
                    : typeof r2 !== 'undefined'
                        ? r2
                        : 'undefined';

        //    if (r === 'undefined')
        //        evl.preaddLogItem('<p>Error - Request String is undefined</p>');
            return r;
        };
    return {
        getData: getData,
        getDataT: getDataT,
        getDataT2: getDataT2
    };
};

var Requester2 = function (request, onSuccess, onError, request1, onSuccess1, onError1, request2, onSuccess2, onError2) {
    var getData = function (req) {
        var r = checkRequestStr(req, request);
        if (r === 'undefined')
            return;
        $.ajax({
            type: 'GET',
            url: r,
            dataType: 'json',
            success: function (data) {
                onSuccess(data);
            },
            error: function () {
                onError();
            }
        });
    },
        getDataT = function (req, timeInterval) {
            var toContinue = 0;
            if (timeInterval < 1000)
                timeInterval = 5000;
            var r = checkRequestStr(req, request);
            if (r === 'undefined')
                return;
            $.ajax({
                type: 'GET',
                url: r,
                dataType: 'json',
                success: function (data) {
                    toContinue = onSuccess(data);
                    if (toContinue > 0)
                        setTimeout(getDataT, timeInterval, r, timeInterval);
                },
                error: function () {
                    toContinue = onError();
                    if (toContinue > 0)
                        setTimeout(getDataT, timeInterval, r, timeInterval);
                }
            });
        },
        getDataT2 = function (req1, req2, timeInterval) {
            var toContinue = 0;

            if (timeInterval < 1000)
                timeInterval = 5000;

            var r1 = checkRequestStr(req1, request1);
            if (r1 === 'undefined')
                return;

            $.ajax({
                type: 'GET',
                url: r1,
                dataType: 'json',
                success: function (data) {
                    toContinue = onSuccess1(data);
                    if (toContinue > 0) {
                        var r2 = checkRequestStr(req2, request2);
                        if (r2 === 'undefined')
                            return;
                        $.ajax({
                            type: 'GET',
                            url: r2,
                            dataType: 'json',
                            success: function (data2) {
                                toContinue = onSuccess2(data2);
                                if (toContinue > 0) {
                                    setTimeout(getDataT2, timeInterval, r1, r2, timeInterval);
                                }
                            },
                            error: function () {
                                toContinue = onError2();
                                if (toContinue > 0)
                                    setTimeout(getDataT2, timeInterval, req1, req2, timeInterval);
                            }
                        });
                    }
                },
                error: function () {
                    toContinue = onError1();
                    if (toContinue > 0)
                        setTimeout(getDataT2, timeInterval, req1, req2, timeInterval);
                }
            });
        },
        checkRequestStr = function (r1, r2) {
            var r =
                typeof r1 !== 'undefined'
                    ? r1
                    : typeof r2 !== 'undefined'
                        ? r2
                        : 'undefined';

            //    if (r === 'undefined')
            //        evl.preaddLogItem('<p>Error - Request String is undefined</p>');
            return r;
        };
    return {
        getData: getData,
        getDataT: getDataT,
        getDataT2: getDataT2
    };
};
var Requester3 = function (request, request1, request2) {
    
    var getData = function (req, onSuccess, onError) {
        var r = checkRequestStr(req, request);
        if (r === 'undefined')
            return;
        $.ajax({
            type: 'GET',
            url: r,
            dataType: 'json',
            success: function (data) {
                onSuccess(data);
            },
            error: function () {
                onError();
            }
        });
    },
        getDataT = function ( timeInterval, req, onSuccess, onError) {
            var toContinue = 0;
            if (timeInterval < 1000)
                timeInterval = 5000;
            var r = checkRequestStr(req, request);
            if (r === 'undefined')
                return;
            $.ajax({
                type: 'GET',
                url: r,
                dataType: 'json',
                success: function (data) {
                    toContinue = onSuccess(data);
                    if (toContinue > 0)
                        setTimeout(getDataT, timeInterval, r, timeInterval);
                },
                error: function () {
                    toContinue = onError();
                    if (toContinue > 0)
                        setTimeout(getDataT, timeInterval, r, timeInterval);
                }
            });
        },
        getDataT2 = function (timeInterval, req1, onSuccess1, onError1, req2, onSuccess2, onError2) {
            var toContinue = 0;

            if (timeInterval < 1000)
                timeInterval = 5000;

            var r1 = checkRequestStr(req1, request1);
            if (r1 === 'undefined')
                return;

            $.ajax({
                type: 'GET',
                url: r1,
                dataType: 'json',
                success: function (data) {
                    toContinue = onSuccess1(data);
                    if (toContinue > 0) {
                        var r2 = checkRequestStr(req2, request2);
                        if (r2 === 'undefined')
                            return;
                        $.ajax({
                            type: 'GET',
                            url: r2,
                            dataType: 'json',
                            success: function (data2) {
                                toContinue = onSuccess2(data2);
                                if (toContinue > 0) {
                                    setTimeout(getDataT2, timeInterval, r1, r2, timeInterval);
                                }
                            },
                            error: function () {
                                toContinue = onError2();
                                if (toContinue > 0)
                                    setTimeout(getDataT2, timeInterval, req1, req2, timeInterval);
                            }
                        });
                    }
                },
                error: function () {
                    toContinue = onError1();
                    if (toContinue > 0)
                        setTimeout(getDataT2, timeInterval, req1, req2, timeInterval);
                }
            });
        },
        checkRequestStr = function (r1, r2) {
            var r =
                typeof r1 !== 'undefined'
                    ? r1
                    : typeof r2 !== 'undefined'
                        ? r2
                        : 'undefined';

            //    if (r === 'undefined')
            //        evl.preaddLogItem('<p>Error - Request String is undefined</p>');
            return r;
        };
    return {
        getData: getData,
        getDataT: getDataT,
        getDataT2: getDataT2
    };
};
