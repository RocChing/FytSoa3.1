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
                console.log(model);
                var list = model.musicList;
                var songList = [];
                if (list && list.length > 0) {
                    for (var i = 0; i < list.length; i++) {
                        songList.push(convertMusicObj(list[i]));
                    }
                }
                updateUI(model.listName, model.number);
            }
            initMusic(songList);
        });

        connection.on("AddSong", function (list) {
            if (list && list.length > 0) {
                for (var i = 0; i < list.length; i++) {
                    var m = convertMusicObj(list[i]);
                    music.addSong(m);
                }
                console.log("playIndex:" + music.playIndex);
                updateUI(null, music.playList.length);
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

    function convertMusicObj(m) {
        return {
            title: m.name,
            singer: m.artists,
            thumbnail: m.converUrl,
            words: m.lrcInfo.words,
            audio: m.musicUrl
        };
    }

    function updateUI(title, number) {
        if (title) {
            $('#list_title').text(title);
        }
        if (number) {
            $('#list_number').text(number);
        }
    }
   
    init();
});
