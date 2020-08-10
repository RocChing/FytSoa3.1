var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

/*
 * mo v0.0.1
 * author : smohan
 * The MIT License (MIT)
 * Copyright (c) 2016 https://smohan.net
 */
(function () {

    'use strict';

    var ArrayProto = Array.prototype;

    /**
     * 过滤样式，返回样式数组
     * (classA classB ...) | (classA, classB ...) => [classA, classB]
     * @param className
     * @returns {Array}
     */
    var filterClassName = function filterClassName(className) {
        return !className ? [] : className.trim().replace(/\s+/, ',').split(',');
    };

    /**
     * 获取元素计算后的样式
     * @param  {HTMLElement} element
     * @return {Object}
     */
    var getComputedStyles = function getComputedStyles(element) {
        return element.ownerDocument.defaultView.getComputedStyle(element, null);
    };

    /**
     * CLASSES 样式操作
     * @type {Object}
     */
    var CLASSES = {
        contains: function contains(element, className) {
            if (element.classList) {
                return element.classList.contains(className);
            } else {
                return new RegExp('(^| )' + className + '( |$)', 'gi').test(element.className);
            }
        },
        add: function add(element, className) {
            filterClassName(className).forEach(function (name) {
                name = name.trim();
                if (name && !CLASSES.contains(element, name)) {
                    if (element.classList) {
                        element.classList.add(name);
                    } else {
                        element.className += ' ' + name;
                        element.className = element.className.trim();
                    }
                }
            });
        },
        remove: function remove(element, className) {
            filterClassName(className).forEach(function (name) {
                name = name.trim();
                if (name && CLASSES.contains(element, name)) {
                    if (element.classList) {
                        element.classList.remove(name);
                    } else {
                        element.className = element.className.replace(new RegExp('(^|\\b)' + name.split(' ').join('|') + '(\\b|$)', 'gi'), ' ');
                        element.className = element.className.trim();
                    }
                }
            });
        },
        toggle: function toggle(element, className) {
            filterClassName(className).forEach(function (name) {
                if (element.classList) {
                    return element.classList.toggle(name);
                } else {
                    return CLASSES.contains(element, name) ? CLASSES.remove(element, name) : CLASSES.add(element, name);
                }
            });
        }
    };

    /**
     * 拷贝源对象到目标对象
     * Polyfill https://developer.mozilla.org/zh-CN/docs/Web/JavaScript/Reference/Global_Objects/Object/assign
     * @param target
     * @returns {{}}
     */
    var assign = function assign() {
        var target = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};

        target = Object(target);
        for (var i = 1, len = arguments.length; i < len; i++) {
            var source = arguments[i];
            if (source != null) {
                for (var key in source) {
                    if (Object.prototype.hasOwnProperty.call(source, key)) {
                        target[key] = source[key];
                    }
                }
            }
        }
        return target;
    };

    var SelectorRegs = {
        id: /^#([\w-]+)$/,
        className: /^\.([\w-]+)$/,
        tagName: /^[\w-]+$/

        /**
         * 获取元素对象集合
         * @param selector 选择器
         * @param context 父级上下文
         * @returns {*}
         */
    };var qsa = function qsa() {
        var selector = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : '*';
        var context = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : document;

        if (typeof selector === "string") {
            selector = selector.trim();
            var dom = [];
            if (SelectorRegs.id.test(selector)) {
                dom = document.getElementById(RegExp.$1);
                dom = dom ? [dom] : [];
            } else if (SelectorRegs.className.test(selector)) {
                dom = context.getElementsByClassName(RegExp.$1);
            } else if (SelectorRegs.tagName.test(selector)) {
                dom = context.getElementsByTagName(selector);
            } else {
                dom = context.querySelectorAll(selector);
            }
            return ArrayProto.slice.call(dom);
        }
        return [];
    };

    /**
     * matchSelector兼容模式
     * @param element
     * @param selector
     * @returns {*}
     */
    var matchSelector = function () {
        var prop = Element.prototype;
        var matchesSelector = prop.matches || prop.matchesSelector || prop.mozMatchesSelector || prop.msMatchesSelector || prop.oMatchesSelector || prop.webkitMatchesSelector;
        return function (element, selector) {
            return matchesSelector.call(element, selector);
        };
    }();

    /**
     * 对象遍历
     * @param object
     * @param callback
     */
    var each = function each(object, callback) {
        if ((typeof object === 'undefined' ? 'undefined' : _typeof(object)) === "object" && typeof callback === "function") {
            if (Array.isArray(object)) {
                for (var i = 0, len = object.length; i < len; i++) {
                    if (callback.call(object[i], i, object[i]) === false) {
                        break;
                    }
                }
            } else if ('length' in object && typeof object.length === "number") {
                //这地方不太严谨，谨慎使用
                for (var k in object) {
                    if (callback.call(object[k], k, object[k]) === false) {
                        break;
                    }
                }
            }
        }
    };

    /**
     * 事件绑定，支持简单事件代理
     * @param element
     * @param eventType
     * @param selector
     * @param callback
     */
    var bind = function bind(element, eventType, selector, callback) {
        var sel = void 0,
            handler = void 0;
        if (typeof selector === "function") {
            handler = selector;
        } else if (typeof selector === "string" && typeof callback === "function") {
            sel = selector;
        } else {
            return;
        }
        if (sel) {
            //事件代理
            handler = function handler(e) {
                //todo, 多选择器支持
                var nodes = qsa(sel, element);
                var matched = false;
                for (var i = 0, len = nodes.length; i < len; i++) {
                    var node = nodes[i];
                    if (node === e.target || node.contains(e.target)) {
                        matched = node;
                        break;
                    }
                }
                if (matched) {
                    callback.apply(matched, ArrayProto.slice.call(arguments));
                }
            };
        }

        element.addEventListener(eventType, handler, false);
    };

    /**
     * 事件解绑
     * @param element
     * @param eventType
     * @param callback
     */
    var unbind = function unbind(element, eventType, callback) {
        return element.removeEventListener(eventType, callback, false);
    };

    /**
     * 事件触发
     * @param element
     * @param eventName
     */
    var trigger = function trigger(element, eventName) {
        var event = document.createEvent('HTMLEvents');
        event.initEvent(eventName, true, false);
        element.dispatchEvent(event);
    };

    /**
     * 将字面量对象转为URL字符串
     * @param data
     * @returns {string}
     */
    var paramStringify = function paramStringify(data) {
        var arr = [];
        var key = void 0,
            value = void 0;
        for (key in data) {
            value = data[key];
            if (value) {
                arr.push(key + '=' + value.toString());
            }
        }
        return arr.join('&');
    };

    /**
     * http请求
     * @param options
     */
    var http = function http() {
        var options = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};

        var url = options.url || window.location.href,
            done = options.done || null,
            fail = options.fail || null,
            data = options.data,
            dataString = paramStringify(data),
            method = options.type && /^(get|post)$/i.test(options.type) ? options.type.toUpperCase() : 'GET';
        var xhr = new XMLHttpRequest(),
            headers = options.headers || {};
        headers.accept = "application/json, text/javascript";
        if (method === 'POST') {
            headers['Content-Type'] = 'application/x-www-form-urlencoded; charset=UTF-8';
        } else {
            headers['Content-Type'] = 'application/json; charset=UTF-8';
            url = url.indexOf('?') > -1 ? url + dataString : url;
            dataString = undefined;
        }
        xhr.open(method, url, true);
        for (var i in headers) {
            xhr.setRequestHeader(i, headers[i]);
        }
        xhr.onload = function () {
            if (xhr.status >= 200 && xhr.status < 400) {
                var response = xhr.responseText;
                try {
                    response = JSON.parse(response);
                } catch (e) {}
                done && done.call(null, response, xhr);
            } else {
                fail && fail.call(xhr.status, 'error', xhr);
            }
        };
        xhr.send(dataString);
    };

    var utils = {
        $: function $(selector, context) {
            return qsa(selector, context)[0];
        },
        $$: qsa,
        matchSelector: matchSelector,
        addClass: CLASSES.add,
        removeClass: CLASSES.remove,
        hasClass: CLASSES.contains,
        toggleClass: CLASSES.toggle,
        getStyles: getComputedStyles,
        assign: Object.assign || assign,
        each: each,
        bind: bind,
        unbind: unbind,
        trigger: trigger,
        http: http
    };

    window.MoUtils = utils;
})();
//# sourceMappingURL=mo.js.map
