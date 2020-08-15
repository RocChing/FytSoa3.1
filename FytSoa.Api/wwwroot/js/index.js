/// <reference path="jquery.js" />
/// <reference path="mo.js" />
/// <reference path="smusic.js" />

var music;
var connection;

$(function () {
    function init() {
        connection = new signalR.HubConnectionBuilder().withUrl("/songHub").build();

        connection.on("OnConnected", function (model) {
            if (model) {
                var songList = getMusicList(model.musicList);
                updateUI(model.listName, model.number);
            }
            initMusic(songList);
        });

        connection.on("AddSong", function (list, fansName) {
            if (list && list.length > 0) {
                var mm = null;
                for (var i = 0; i < list.length; i++) {
                    var m = convertMusicObj(list[i]);
                    music.addSong(m);
                    mm = m;
                }
                updateUI(null, music.playList.length, mm, fansName);
            }
        });

        connection.on('PlaySong', function (id) {
            music.playById(id);
        });

        connection.on('DeleteSong', function (id) {
            music.deleteSong(id);
        });

        connection.on('UpdateSongSortId', function (model) {
            if (model) {
                var songList = getMusicList(model.musicList);
                updateUI(model.listName, model.number);
                music.updateList(songList);
            }
        });

        connection.on('HeartBeat', function (data) {
            console.log(data);
        });

        connection.start().then(function () {
            console.log('ok');
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }

    function initMusic(songList) {
        music = SMusic(songList, {
            container: $('#music')[0],
            panel: 'list',
            volume: 0.5
        });
        music.init();
    }

    function getMusicList(list) {
        var songList = [];
        if (list && list.length > 0) {
            for (var i = 0; i < list.length; i++) {
                songList.push(convertMusicObj(list[i]));
            }
        }
        return songList;
    }

    function convertMusicObj(m) {
        return {
            id: m.id,
            title: m.name,
            singer: m.artists,
            thumbnail: m.converUrl,
            words: m.lrcInfo.words,
            audio: m.musicUrl
        };
    }

    function updateUI(title, number, m, fansName) {
        if (title) {
            $('#list_title').text(title);
        }
        if (number) {
            $('#list_number').text(number);
        }
        if (m) {
            var ad = "[" + fansName + "]点了歌曲[" + m.title + "#" + m.singer + "]";
            $('#ad').text(ad);
        }
    }

    init();
});
