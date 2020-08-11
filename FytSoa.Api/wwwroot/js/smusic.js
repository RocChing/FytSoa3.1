function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

/*
 * smusic v2.1.0
 * author : smohan
 * The MIT License (MIT)
 * Copyright (c) 2016 https://smohan.net.
 * https://smohan.net/lab/smusic
 * https://github.com/S-mohan/smusic
 */

(function () {
    "use strict";

    var doc = document,
        win = window,
        utils = MoUtils,
        isTouch = "ontouchend" in document;
    var version = '2.1.0',
        homepage = '',
        thumbnailPlaceholder = 'https://s-mohan.github.io/demo/static/img/smusic.jpg';
    var _ref = [utils.$, utils.$$, utils.bind, function () { }],
        $ = _ref[0],
        $$ = _ref[1],
        bind = _ref[2],
        noop = _ref[3];


    var EVENT_START = isTouch ? 'touchstart' : 'mousedown',
        EVENT_MOVE = isTouch ? 'touchmove' : 'mousemove',
        EVENT_END = isTouch ? 'touchend' : 'mouseup';

    /**
     * 模拟一个滑块方法
     * @param element 进度条容器
     * @param sliderSelector 滑块选择器
     * @param direction 滑动方向 horizontal | vertical
     * @param callback 移动时回调移动距离
     * @param end 结束滑动时回调
     */
    var Range = function Range(element, sliderSelector) {
        var direction = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : 'horizontal';
        var callback = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : noop;
        var end = arguments.length > 4 && arguments[4] !== undefined ? arguments[4] : noop;
        var _ref2 = [element, $(sliderSelector, element)],
            $bar = _ref2[0],
            $slider = _ref2[1];

        var barSize = {
            width: $bar.offsetWidth,
            height: $bar.offsetHeight
        };
        bind($slider, 'click', function (event) {
            return event.stopPropagation();
        });
        bind($slider, EVENT_START, function (event) {
            event = event || win.event;
            event.stopPropagation();
            var start = void 0,
                offset = void 0;
            start = {
                x: event.clientX || event.touches[0].clientX,
                y: event.clientY || event.touches[0].clientY
            };
            offset = {
                x: this.offsetLeft,
                y: this.offsetTop
            };
            var MAX = direction == "horizontal" ? barSize.width : barSize.height;
            var moveHandle = function moveHandle(event) {
                event = event || win.event;
                event.stopPropagation();
                var thisX = event.clientX || event.touches[0].clientX,
                    thisY = event.clientY || event.touches[0].clientY;
                //x : left to right
                //y : bottom to top
                var range = Math.min(MAX, Math.max(0, direction == "horizontal" ? offset.x + (thisX - start.x) : offset.y + (thisY - start.y)));
                callback && typeof callback == "function" && callback(range);
            };
            bind(doc, EVENT_MOVE, moveHandle);
            bind(doc, EVENT_END, function (event) {
                end && typeof end == "function" && end();
                utils.unbind(doc, EVENT_MOVE, moveHandle);
            });
        });
    };

    /**
     * 默认配置项
     * @type {{container: HTMLElement, playIndex: number, playMode: number, volume: number, autoPlay: boolean}}
     */
    var defaultConfig = {
        //放置Smusic的DOM容器
        container: doc.body,
        //初始化播放索引
        playIndex: 0,
        //初始化播放模式 (1 : 列表循环  2 : 随机播放  3 : 单曲循环)
        playMode: 1,
        //初始化音量 (0 - 1之间)
        volume: .5,
        //自动播放
        autoPlay: true,
        //加载时默认显示面板，['list', 'lyric']
        panel: 'list'
    };

    var uid = 1;

    /**
     * 抛出异常
     * @param message
     */
    var error = function error(message) {
        throw new Error("Smusic Error：" + message);
    };

    /**
     * 输出log
     * @param message
     */
    var log = function log(message) {
        return win.console.log("Smusic Log：" + message);
    };

    /**
     * 格式化时间
     * @param time
     * @returns {string}
     */
    var calcTime = function calcTime(time) {
        var hour = void 0,
            minute = void 0,
            second = void 0,
            times = '';
        hour = String(parseInt(time / 3600, 10));
        minute = String(parseInt(time % 3600 / 60, 10));
        second = String(parseInt(time % 60, 10));
        if (hour != '0') {
            if (hour.length == 1) hour = '0' + hour;
            times += hour + ':';
        }
        if (minute.length == 1) minute = '0' + minute;
        times += minute + ':';
        if (second.length == 1) second = '0' + second;
        times += second;
        return times;
    };

    /**
     * [获取模板]
     * @param  {String} panel [默认显示面板]
     * @return {String} 
     */
    var __getSmusicTpl = function __getSmusicTpl() {
        var panel = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : 'list';
        var isList = panel === 'list';
        var panelClass = isList ? 'show-list' : '';

        return "<main class=\"smusic-main\">\n    <div class=\"smusic-panel\">\n        <div class=\"smusic-music-info\">\n            <div class=\"smusic-music-scroll js-smusic-scroll--title\">\n                <strong class=\"smusic-music--title js-smusic-song--title\">Smusic</strong>\n                <small class=\"smusic-music--singer js-smusic-song--singer\">smohan</small>\n            </div>\n        </div>\n        <figure class=\"smusic-music-thumbnail\">\n            <img src=\"" + thumbnailPlaceholder + "\" class=\"js-smusic-song--thumbnail\" />\n        </figure>\n                <div class=\"smusic-music-ctrl\">\n            <a class=\"smusic-ctrl--prev js-smusic-btn--prev\" title=\"\u4E0A\u4E00\u9996\"><i class=\"smusic-ico-prev\"></i></a>\n            <a class=\"smusic-ctrl--play smusic-music-play js-smusic-btn--play\" title=\"\u6682\u505C\"></a>\n            <a class=\"smusic-ctrl--next js-smusic-btn--next\"><i class=\"smusic-ico-next\" title=\"\u4E0B\u4E00\u9996\"></i></a>\n        </div>\n           </div>\n    <div class=\"smusic-panel\" style=\"width:40%\">\n <div class=\"smusic-panel--scroll js-smusic-scroll--panel\">  <div class=\"smusic-lyric--wrap\">\n                <ul class=\"smusic-lyric--scroll js-smusic-scroll--lyric\"></ul>\n    </div> </div></div>     <div class=\"smusic-panel\">  <div class=\"smusic-panel--scroll js-smusic-scroll--panel\">    <div class=\"smusic-list--wrap\">\n                <ul class=\"smusic-list--scroll js-smusic-scroll--list\"></ul>\n    </div> </div>\n        </div>\n  </main>\n<aside class=\"smusic-aside\">\n    <div class=\"smusic-ctrl smusic-ctrl--left\">\n        <div class=\"smusic-ctrl--volume\">\n            <a class=\"smusic-volume--toggle js-smusic-btn--volume\"></a>\n            <div class=\"smusic-volume--bar js-smusic-volume--bar\">\n                <span class=\"smusic-volume--value js-smusic-volume--value\"></span>\n                <span class=\"smusic-volume--slider js-smusic-volume--slider\"></span>\n            </div>\n        </div>\n        <a class=\"smusic-ctrl--mode smusic-mode--loop js-smusic-btn--mode\" data-play-mode=\"1\"></a>\n    </div>\n    <div class=\"smusic-progress js-smusic-progress\">\n        <span class=\"smusic-progress--buffer js-smusic-progress--buffer\"></span>\n        <span class=\"smusic-progress--value js-smusic-progress--value\"><i class=\"smusic-progress--slider js-smusic-progress--slider\"></i></span>\n    </div>\n    <div class=\"smusic-ctrl smusic-ctrl--right\">\n        <time class=\"smusic-time js-smusic-time\">00:00/00:00</time>\n        <a class=\"smusic-ctrl--lyric js-smusic-btn--lyric js-smusic-panel--tab " + (!isList ? 'active' : '') + "\" data-panel=\"lyric\" title=\"\u6B4C\u8BCD\"><i class=\"smusic-ico-lyric\"></i></a>\n        <a class=\"smusic-ctrl--list js-smusic-btn--list js-smusic-panel--tab " + (isList ? 'active' : '') + "\" data-panel=\"list\" title=\"\u5217\u8868\"><i class=\"smusic-ico-list\"></i></a>\n    </div>\n</aside>";
        //
    };

    //歌词缓存
    var _lyricCache = {};
    var timeReg = /\[\d*:\d*((\.|:)\d*)*]/g;

    /**
     * 解析歌词
     * @param lyric
     * @returns {{}}
     * @private
     */
    var _parseLyric = function _parseLyric(lyric) {
        // 将歌词通过换行符转换为数组，
        // 每一行格式如 [00:00.00] 作曲 : 赵雷
        var lyricRows = lyric.replace(/(\[\d*:\d*((\.|:)\d*)*])/gm, '\n$1').trim() // 草，酷狗的歌词竟然是一坨，没有换行符，这里要专门处理
            .replace(/\\n/gm, '\n').split('\n');
        var lyricData = {}; //时间为key, 歌词做value
        var i = 0,
            content = void 0,
            len = lyricRows.length;
        for (i; i < len; i++) {
            content = decodeURIComponent(lyricRows[i]);
            if (typeof content !== "string") break;
            var timeRegArr = content.match(timeReg);
            if (!timeRegArr) continue;
            for (var _i = 0, _len = timeRegArr.length; _i < _len; _i++) {
                var t = timeRegArr[_i];
                var minute = Number(String(t.match(/\[\d*/i)).slice(1)),
                    second = Number(String(t.match(/\:\d*/i)).slice(1));
                var time = minute * 60 + second;
                lyricData[time] = content.replace(timeReg, ''); //内容区去掉时间
            }
        }
        return lyricData;
    };

    var _parseLyricArray = function _parseLyricArray(list) {
        if (!list) {
            return 0;
        }
        var lyricData = {};
        var i = 0,
            len = list.length;
        for (i; i < len; i++) {
            var word = list[i];
            let time = Math.round(word.time);
            lyricData[time] = word.text;
        }
        return lyricData;
    };

    /**
     * 加载歌词
     * @param playList
     * @param index
     * @param callback
     * @private
     */
    var _getLyric = function _getLyric(playList, index, callback) {
        var song = playList[index];
        var lyricUrl = song['lyric'];
        var cacheName = '_smusic_lyric_' + index;

        if (!_lyricCache[cacheName]) {
            if (!lyricUrl) {
                _lyricCache[cacheName] = _parseLyricArray(song['words']);
                callback(_lyricCache[cacheName]);
            }
            else {
                utils.http({
                    type: 'GET',
                    url: lyricUrl,
                    done: function done(response) {
                        if (response) {
                            response = _parseLyric(response);
                        } else {
                            response = -1; //empty
                        }
                        _lyricCache[cacheName] = response;
                        callback(_lyricCache[cacheName]);
                    },
                    fail: function fail(error) {
                        _lyricCache[cacheName] = -2; //fail
                        callback(_lyricCache[cacheName]);
                    }
                });
            }
        }
        else {
            callback(_lyricCache[cacheName]);
        }
    };

    /**
     * 获取一个不包含当期索引在内的随机索引
     * @param playIndex
     * @param len
     * @returns {*}
     * @private
     */
    var _getRandomIndex = function _getRandomIndex(playIndex, len) {
        var array = [];
        for (var i = 0; i < len; i++) {
            if (i != playIndex) {
                array.push(i);
            }
        }
        var random = parseInt(Math.random() * (len - 1));
        var index = array[random];
        array = null;
        return index;
    };

    /**
     * 创建DOM
     * @param container
     * @returns {{element: Element, id: number}}
     * @private
     */
    var _createDom = function _createDom() {
        var options = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};

        var container = options.container;

        var id = uid++;
        var smusic = doc.createElement('div');
        smusic.id = 'smohan-smusic-' + id;
        smusic.className = 'smusic-container';
        smusic.setAttribute('data-smusic-version', version);
        smusic.setAttribute('data-smusic-homepage', homepage);
        smusic.setAttribute('data-smusic-id', id.toString());
        smusic.innerHTML = __getSmusicTpl(options.panel || 'list') + '<audio muted="muted" class="js-smusic--audio" hidden></audio>';
        container.appendChild(smusic);
        return {
            element: smusic,
            id: id
        };
    };

    /**
     * 缓存Dom
     * @returns {__domCache}
     * @private
     */
    var __domCache = function __domCache() {
        var smusic = this.smusic;
        this.dom = {
            scroll: {
                title: $('.js-smusic-scroll--title', smusic),
                panel: $('.js-smusic-scroll--panel', smusic),
                lyric: $('.js-smusic-scroll--lyric', this.panel),
                list: $('.js-smusic-scroll--list', this.panel)
            },
            song: {
                title: $('.js-smusic-song--title', smusic),
                singer: $('.js-smusic-song--singer', smusic),
                thumbnail: $('.js-smusic-song--thumbnail', smusic)
            },
            btn: {
                prev: $('.js-smusic-btn--prev', smusic),
                play: $('.js-smusic-btn--play', smusic),
                next: $('.js-smusic-btn--next', smusic),
                volume: $('.js-smusic-btn--volume', smusic),
                mode: $('.js-smusic-btn--mode', smusic),
                lyric: $('.js-smusic-btn--lyric', smusic),
                list: $('.js-smusic-btn--list', smusic)
            },
            time: $('.js-smusic-time', smusic),
            progress: {
                bar: $('.js-smusic-progress', smusic),
                buffer: $('.js-smusic-progress--buffer', this.bar),
                value: $('.js-smusic-progress--value', this.bar),
                slider: $('.js-smusic-progress--slider', this.bar)
            },
            volume: {
                bar: $('.js-smusic-volume--bar', smusic),
                value: $('.js-smusic-volume--value', this.bar),
                slider: $('.js-smusic-volume--slider', this.bar)
            }
        };
    };

    /**
     * 渲染列表
     * @private
     */
    var __renderList = function __renderList() {
        var self = this,
            list = self.dom.scroll.list,
            data = self.playList;
        var html = '';
        data.forEach(function (item, index) {
            var active = index === self.playIndex ? ' active' : '';
            html += "<li class=\"js-smusic-song--item" + active + "\" data-song-index=\"" + index + "\">\n<span class=\"song-animate\"><i></i><i></i><i></i></span>\n<span class=\"song-title\" title=\"" + item.title + "\">" + item.title + "</span>\n<span class=\"song-singer\" title=\"" + item.singer + "\">" + item.singer + "</span>\n</li>";
        });
        list.innerHTML = html;
    };

    /**
     * 渲染歌词
     * @private
     */
    var __renderLyric = function __renderLyric() {
        var self = this,
            rowHeight = 50;
        var lyricHeight = self.dom.scroll.lyric.parentNode.offsetHeight;
        var html = '',
            i = 0;
        _getLyric(this.playList, this.playIndex, function (lyric) {
            if (typeof lyric === "number") {
                switch (lyric) {
                    case 0:
                        html += "<li class=\"empty\">\u6682\u65E0\u6B4C\u8BCD</li>";
                        break;
                    case -1:
                        html += "<li class=\"empty\">\u6B4C\u8BCD\u89E3\u6790\u5931\u8D25</li>";
                        break;
                    case -2:
                        html += "<li class=\"empty\">\u6B4C\u8BCD\u52A0\u8F7D\u5931\u8D25</li>";
                        break;
                }
                self.dom.scroll.lyric.innerHTML = html;
            } else {
                for (var time in lyric) {
                    var top = rowHeight * i,
                        toScroll = top >= lyricHeight / 2 - rowHeight;
                    var content = lyric[time] || homepage;
                    if (typeof content !== "string") {
                        content = content.content;
                    }
                    lyric[time] = {
                        index: i,
                        content: content,
                        top: top,
                        scrollTop: toScroll ? top - (lyricHeight / 2 - rowHeight) : 0,
                        toScroll: toScroll
                    };
                    i++;
                    html += "<li>" + content + "</li>";
                }
                self.dom.scroll.lyric.innerHTML = html;
                //self.dom.scroll.lyric.innerHTML = html + ("<li><a href=\"" + homepage + "\" title=\"Smusic\" target=\"_blank\">" + homepage + "</a></li>");
            }
        });
    };

    //记录buffer的Interval
    var _bufferTimer = {};

    /**
     * 设置播放缓冲
     * @private
     */
    var __setBuffer = function __setBuffer() {
        var _ref3 = [this.dom, this.audio, this.smusicId],
            DOM = _ref3[0],
            AUDIO = _ref3[1],
            smusicId = _ref3[2];

        _bufferTimer[smusicId] && clearInterval(_bufferTimer[smusicId]);
        var progressWidth = parseFloat(DOM.progress.bar.offsetWidth);
        if (!isNaN(AUDIO.duration)) {
            var totalTime = calcTime(AUDIO.duration);
            DOM.time.textContent = "00:00/" + totalTime;
        }
        _bufferTimer[smusicId] = setInterval(function () {
            var buffer = AUDIO.buffered.length;
            if (buffer > 0 && AUDIO.buffered != undefined) {
                var bufferWidth = AUDIO.buffered.end(buffer - 1) / AUDIO.duration * progressWidth;
                DOM.progress.buffer.style.width = bufferWidth + 'px';
                if (Math.abs(AUDIO.duration - AUDIO.buffered.end(buffer - 1)) < 1) {
                    DOM.progress.buffer.style.width = progressWidth + 'px';
                    clearInterval(_bufferTimer[smusicId]);
                }
            }
        }, 1e3);
    };

    /**
     * 绑定事件
     * @private
     */
    var __bindAction = function __bindAction() {
        var _ref4 = [this, this.dom, this.audio],
            self = _ref4[0],
            DOM = _ref4[1],
            AUDIO = _ref4[2];
        //进度条宽度

        var progressWidth = Math.round(DOM.progress.bar.offsetWidth);
        var volumeHeight = Math.round(DOM.volume.bar.offsetHeight);

        //列表面板和歌词面板切换
        bind(self.smusic, 'click', '.js-smusic-panel--tab', function (event) {
            event.stopPropagation();
            if (utils.hasClass(this, 'active')) return false;
            var tab = this.getAttribute('data-panel');
            utils.addClass(this, 'active');
            if (tab === 'list') {
                utils.removeClass(DOM.btn.lyric, 'active');
                utils.addClass(DOM.scroll.panel, 'show-list');
            } else {
                utils.removeClass(DOM.btn.list, 'active');
                utils.removeClass(DOM.scroll.panel, 'show-list');
            }
        });

        //Audio timeupdate事件
        bind(AUDIO, 'timeupdate', function () {
            if (!isNaN(AUDIO.duration)) {
                var surplusTime = calcTime(AUDIO.currentTime),
                    totalTime = calcTime(AUDIO.duration),
                    currentProcess = AUDIO.currentTime / AUDIO.duration * progressWidth;
                //当前播放时间/总时间 = 播放百分比
                //播放百分比 * 进度条宽度 = 当前播放进度
                DOM.time.textContent = surplusTime + "/" + totalTime;
                var range = Math.min(currentProcess, progressWidth);
                DOM.progress.value.style.width = range + 'px';
                //歌词
                var cacheName = '_smusic_lyric_' + self.playIndex;
                var lyricData = _lyricCache[cacheName];
                if (!lyricData) return;
                var currentTime = Math.round(AUDIO.currentTime);
                var lyric = lyricData[currentTime];
                if (!lyric || typeof lyric === "number") return; //当前时间点没有对应到歌词，结束并返回当前行歌词;
                var index = lyric['index'],
                    toScroll = lyric['toScroll'],
                    $lyric = $$('li', DOM.scroll.lyric)[index],
                    $activeLyric = $$('li.active', DOM.scroll.lyric);
                $activeLyric.forEach(function (node) {
                    node && utils.removeClass(node, 'active');
                });
                $lyric && utils.addClass($lyric, 'active');
                DOM.scroll.lyric.style.transform = toScroll ? 'translate3d(0, -' + lyric.scrollTop + 'px, 0)' : 'translate3d(0, 0, 0)';
            }
        });

        //播放模式切换
        bind(DOM.btn.mode, 'click', function () {
            var mode = this.getAttribute('data-play-mode');
            var _mode = void 0;
            switch (Number(mode)) {
                case 1:
                    _mode = 2;
                    break;
                case 2:
                    _mode = 3;
                    break;
                case 3:
                    _mode = 1;
                    break;
            }
            self.setMode(_mode);
        });

        //播放暂停
        bind(DOM.btn.play, 'click', function () {
            var isPlay = utils.hasClass(this, 'smusic-music-play');
            if (isPlay) {
                self.pause();
            } else {
                self.play();
            }
        });

        //播放结束后调用ended事件，开始下一首
        bind(AUDIO, 'ended', function () {
            return self.playByMode('ended');
        });
        //上一首
        bind(DOM.btn.prev, 'click', function () {
            return self.prev();
        });

        //下一首
        bind(DOM.btn.next, 'click', function () {
            return self.next();
        });

        //拖动进度条
        Range(DOM.progress.bar, '.js-smusic-progress--slider', 'horizontal', function (range) {
            var progress = Math.max(0, Math.min(range / progressWidth, progressWidth));
            if (AUDIO.currentTime && AUDIO.duration) {
                AUDIO.currentTime = Math.round(progress * AUDIO.duration);
            }
        });

        //点击进度条
        bind(DOM.progress.bar, 'click', function (event) {
            event.stopPropagation();
            var rect = this.getBoundingClientRect();
            var progress = Math.min(progressWidth, Math.abs(event.clientX - rect.left)) / progressWidth;
            if (AUDIO.currentTime && AUDIO.duration) {
                AUDIO.currentTime = Math.round(progress * AUDIO.duration);
                self.play();
            }
        });

        //音量拖动
        Range(DOM.volume.bar, '.js-smusic-volume--slider', 'vertical', function (range) {
            var volume = Number((volumeHeight - range) / volumeHeight);
            self.setVolume(volume);
        });

        //音量静音开关
        bind(DOM.btn.volume, 'click', function (event) {
            if (AUDIO.muted) {
                utils.removeClass(this, 'smusic-volume--mute');
                this.setAttribute('title', '静音');
                AUDIO.muted = false;
            } else {
                utils.addClass(this, 'smusic-volume--mute');
                this.setAttribute('title', '取消静音');
                AUDIO.muted = true;
            }
        });

        //点击列表
        bind(DOM.scroll.list, 'click', '.js-smusic-song--item', function (event) {
            var index = this.getAttribute('data-song-index');
            if (utils.hasClass(this, 'active')) {
                self.play();
            } else {
                __playMusic.call(self, index);
            }
        });
    };

    /**
     * 播放歌曲
     * @param index
     * @param callback
     * @param isInit
     * @private
     */
    var __playMusic = function __playMusic(index, callback, isInit) {
        try {
            var _ref5 = [this.dom, this.audio, this.playList.length],
                DOM = _ref5[0],
                AUDIO = _ref5[1],
                listLength = _ref5[2];
            //index 调整

            index >= listLength - 1 && (index = listLength - 1);
            index <= 0 && (index = 0);
            this.playIndex = index;
            //当前播放歌曲信息
            var song = this.playList[this.playIndex];
            if (!song) {
                log("没有要播放的歌曲");
                return false;
            }
            var tempHandle = function () {
                return __setBuffer.call(this);
            }.bind(this);
            //在canplay事件监听前移除之前的监听
            AUDIO.removeEventListener('canplay', tempHandle, false);
            _bufferTimer[this.smusicId] && clearInterval(_bufferTimer[this.smusicId]);

            //刷新DOM
            DOM.progress.buffer.style.width = '0px';
            DOM.progress.value.style.width = '0px';
            DOM.time.textContent = '00:00/00:00';
            DOM.scroll.lyric.style.transform = 'translate3d(0, 0, 0)';
            DOM.scroll.lyric.innerHTML = "<li class=\"empty\">\u6B63\u5728\u52A0\u8F7D\u6B4C\u8BCD...</li>";

            $$('.js-smusic-song--item.active', DOM.scroll.list).forEach(function (item) {
                item && utils.removeClass(item, 'active, pause');
            });
            var currentSong = $$('.js-smusic-song--item', DOM.scroll.list)[index];
            currentSong && utils.addClass(currentSong, 'active');

            AUDIO.src = song.audio;
            AUDIO.load();
            AUDIO.addEventListener('canplay', tempHandle, false);
            if (isInit) {
                this.config.autoPlay && AUDIO.play();
            } else {
                AUDIO.play();
            }
            var title = song.title || 'Smusic',
                singer = song.singer || 'singer',
                thumbnail = song.thumbnail || thumbnailPlaceholder;

            DOM.song.title.textContent = title;
            DOM.song.singer.textContent = singer;
            DOM.song.thumbnail.src = thumbnail;
            DOM.scroll.title.setAttribute('title', singer + " - " + title);
            //加载歌词
            __renderLyric.call(this);
            callback && callback.call(this, song);
        } catch (e) {
            log(e);
        }
    };

    /**
     * smusic
     * playList 播放列表
     * options 配置
     */

    var SmohanMusic = function () {

        /**
         * 构造方法
         * @param playList
         * @param options
         */
        function SmohanMusic() {
            var playList = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : [];
            var options = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : {};

            _classCallCheck(this, SmohanMusic);

            if (!Array.isArray(playList)) {
                error("播放列表必须是一个数组");
            }
            this.playList = playList;
            this.audio = null;
            this.version = version;
            this.config = utils.assign({}, defaultConfig, options);
            var playIndex = parseInt(this.config.playIndex),
                playMode = parseInt(this.config.playMode),
                volume = parseFloat(this.config.volume);

            if (playIndex < 0) playIndex = 0;
            if (playIndex > this.playList.length - 1) playIndex = this.playList.length - 1;

            if (playMode < 1 || playMode > 3) playMode = 1;

            if (volume < 0 || volume > 1) volume = .5;

            this.playIndex = playIndex;
            this.playMode = playMode;
            this.volume = volume;
        }

        /**
         * 通过模式播放
         * 由播放模式定义播放索引
         * prev,next,ended等方法都应该通过该接口调用
         * 返回一个合理的播放索引
         * @param type
         * @param callback
         * @param isInit
         */
        SmohanMusic.prototype.playByMode = function playByMode(type, callback, isInit) {
            var _ref6 = [Number(this.playMode), this.playIndex, this.playList.length],
                playMode = _ref6[0],
                playIndex = _ref6[1],
                songLength = _ref6[2];

            var index = playIndex;
            //如果没有定义播放类型，以及播放模式非随机，则直接播放当前索引
            if (!type && playMode != 2) {
                //todo 不做任何事
            } else {
                switch (playMode) {
                    case 1:
                        if (type == 'prev') {
                            index = playIndex <= songLength - 1 && playIndex > 0 ? playIndex - 1 : songLength - 1;
                        } else if (type == 'next' || type == 'ended') {
                            index = playIndex >= songLength - 1 ? 0 : playIndex + 1;
                        }
                        break;
                    case 2:
                        index = _getRandomIndex(playIndex, songLength);
                        break;
                    case 3:
                        if (type == 'prev') {
                            index = playIndex <= songLength - 1 && playIndex > 0 ? playIndex - 1 : songLength - 1;
                        } else if (type == 'next') {
                            index = playIndex >= songLength - 1 ? 0 : playIndex + 1;
                        } else {
                            index = playIndex;
                        }
                        break;
                }
            }
            __playMusic.call(this, index, callback, isInit);
        };

        /**
         * 向列表中追加音乐
         * @param music
         * @param callback
         * @returns {Array|*}
         */
        SmohanMusic.prototype.addSong = function addSong() {
            var music = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
            var callback = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : noop;

            if (music && music.audio) {
                this.playList.push({
                    title: music.title || '歌曲名',
                    singer: music.singer || '歌手名',
                    audio: music.audio,
                    thumbnail: music.thumbnail || thumbnailPlaceholder,
                    lyric: music.lyric || '',
                    words: music.words || []
                });
                this.refreshList();
                callback && callback();
            } else {
                callback && callback("添加失败，参数不是一个对象或者未设置audio属性");
            }
            return this.playList;
        };

        //刷新播放列表
        SmohanMusic.prototype.refreshList = function refreshList() {
            __renderList.call(this);
        };

        /**
         * 下一首
         * @param callback
         */
        SmohanMusic.prototype.next = function next(callback) {
            this.playByMode('next', callback);
        };

        /**
         * 上一首
         * @param callback
         */
        SmohanMusic.prototype.prev = function prev(callback) {
            this.playByMode('next', callback);
        };

        /**
         * 播放
         * @param callback
         */
        SmohanMusic.prototype.play = function play(callback) {
            var BTN = this.dom.btn.play;
            utils.removeClass(BTN, 'smusic-music-pause');
            utils.addClass(BTN, 'smusic-music-play');
            BTN.setAttribute('title', '暂停');
            var $currentSong = $('.js-smusic-song--item.active', this.dom.scroll.list);
            utils.removeClass($currentSong, 'pause');
            this.playList.length && this.audio.play();
            callback && callback.call(this, this.playList[this.playIndex]);
        };

        /**
         * 暂停
         * @param callback
         */
        SmohanMusic.prototype.pause = function pause(callback) {
            var BTN = this.dom.btn.play;
            utils.addClass(BTN, 'smusic-music-pause');
            utils.removeClass(BTN, 'smusic-music-play');
            var $currentSong = $('.js-smusic-song--item.active', this.dom.scroll.list);
            utils.addClass($currentSong, 'pause');
            BTN.setAttribute('title', '播放');
            this.playList.length && this.audio.pause();
            callback && callback.call(this, this.playList[this.playIndex]);
        };

        /**
         * 设置音量
         * @param volume
         */
        SmohanMusic.prototype.setVolume = function setVolume() {
            var volume = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : .5;

            volume = Number(volume);
            volume < 0 && (volume = 0);
            volume > 1 && (volume = 1);
            var barHeight = this.dom.volume.bar.offsetHeight;
            var sliderHeight = this.dom.volume.slider.offsetHeight;
            var valueHeight = barHeight * volume;
            var sliderTop = Math.min((1 - volume) * barHeight, barHeight - sliderHeight);
            if (volume === 0) {
                utils.addClass(this.dom.btn.volume, 'smusic-volume--mute');
                this.dom.btn.volume.setAttribute('title', '取消静音');
                this.audio.muted = true;
            } else {
                utils.removeClass(this.dom.btn.volume, 'smusic-volume--mute');
                this.audio.muted = false;
                this.dom.btn.volume.setAttribute('title', '静音');
            }
            this.dom.volume.value.style.height = valueHeight + 'px';
            this.dom.volume.slider.style.top = sliderTop + 'px';
            this.volume = volume;
            this.audio.volume = volume;
        };

        /**
         * 设置播放模式
         * @param mode
         */
        SmohanMusic.prototype.setMode = function setMode() {
            var mode = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : 1;

            mode = Number(mode);
            var title = void 0,
                className = void 0;
            var $Mode = this.dom.btn.mode;
            switch (mode) {
                case 1:
                default:
                    title = '列表循环';
                    className = 'smusic-mode--loop';
                    break;
                case 2:
                    title = '随机播放';
                    className = 'smusic-mode--random';
                    break;
                case 3:
                    title = '单曲循环';
                    className = 'smusic-mode--single';
                    break;
            }
            $Mode.setAttribute('data-play-mode', mode.toString());
            $Mode.setAttribute('title', title);
            $Mode.className = $Mode.className.replace(/smusic-mode--\w+/, className);
            this.playMode = mode;
        };

        //初始化
        SmohanMusic.prototype.init = function init() {
            var _ref7 = [this.config, _createDom(this.config)],
                config = _ref7[0],
                create = _ref7[1];

            this.smusic = create.element;
            this.smusicId = create.id;
            this.audio = this.smusic.getElementsByTagName('audio')[0];
            //缓存DOM
            __domCache.call(this);
            //渲染列表
            __renderList.call(this);
            //绑定事件
            __bindAction.call(this);
            //设置音量
            this.setVolume(this.volume);
            //设置播放模式
            this.setMode(this.playMode);

            if (this.playList.length) {
                this.playByMode(undefined, function () {
                    if (!config.autoPlay) {
                        utils.trigger(this.dom.btn.play, 'click');
                    }
                }, true);
            } else {
                log("歌曲列表为空");
            }
        };

        /**
         * 获取当前播放的歌曲信息
         * @returns {*}
         */
        SmohanMusic.prototype.getCurrentInfo = function getCurrentInfo() {
            if (this.playList.length) {
                var song = this.playList[this.playIndex];
                song.index = this.playIndex;
                song.volume = this.volume;
                song.mode = this.playMode;
                return song;
            }
            return null;
        };

        //析构


        SmohanMusic.prototype.destroy = function destroy() { };

        return SmohanMusic;
    }();

    /**
     * 作为全局对象返回
     * @param playList
     * @param config
     * @constructor
     */


    win.SMusic = function (playList, config) {
        return new SmohanMusic(playList, config);
    };
})();
//# sourceMappingURL=smusic.js.map
