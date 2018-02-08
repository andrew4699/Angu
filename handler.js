var tabID = -1;
var tabType = -1;

var YOUTUBE = 1;
var SPOTIFY = 2;
var SPOTIFY_NEW = 3;
var SOUNDCLOUD = 4;

var YOUTUBE_PLAYLIST = "Dank Beats";

var port;

document.addEventListener('DOMContentLoaded', function ()
{
	connect();
});

//connect();

function connect()
{
	port = chrome.runtime.connectNative("com.angu.angu");
	port.onMessage.addListener(onNativeMessage);

	port.postMessage({text: "hello boys"});

	setInterval(scrollToSong, 10000);
}

function runCode(tab, code)
{
	chrome.tabs.executeScript(tab, {code: code});
}

function processMessage(msg)
{
	if(msg == "skip")
	{
		if(tabType == YOUTUBE) runCode(tabID, "document.getElementsByClassName('ytp-next-button')[0].click()");
		else if(tabType == SPOTIFY) runCode(tabID, "document.querySelector('.spoticon-skip-forward-24').click()");
		else if(tabType == SPOTIFY_NEW) runCode(tabID, "document.querySelector('.spoticon-skip-forward-16').click()");
		else if(tabType == SOUNDCLOUD) runCode(tabID, "document.getElementsByClassName('skipControl__next')[0].click()");
    }
    else if(msg == "add")
    {
    	if(tabType == YOUTUBE)
    	{
    		//runCode(tabID, "document.querySelector('iron-icon[alt=\"Add to\"]').click()");
    		runCode(tabID, "document.querySelector('#info #top-level-buttons').children[2].click()");

    		setTimeout(function()
    		{
    			runCode(tabID, "var lists = document.querySelectorAll('paper-checkbox.ytd-playlist-add-to-option-renderer'); \
						for(var i = 0; i < lists.length; i++) \
							if(lists[i].innerText.trim() == '" + YOUTUBE_PLAYLIST + "') \
						    	if(lists[i].getAttribute('aria-checked') == 'false') \
									lists[i].click();");
    		}, 1000);
    	}
    	else if(tabType == SPOTIFY) runCode(tabID, "document.getElementById('app-player').contentDocument.getElementById('track-add').click()");
    	else if(tabType == SPOTIFY_NEW) runCode(tabID, "document.querySelector('.spoticon-add-16').click()");
    	else if(tabType == SOUNDCLOUD) runCode(tabID, "document.getElementsByClassName('playbackSoundBadge__like')[0].click()");
    }
    else if(msg == "delete")
    {
    	if(tabType == YOUTUBE)
    	{
    		//runCode(tabID, "document.querySelector('iron-icon[alt=\"Add to\"]').click()");
    		runCode(tabID, "document.querySelector('#info #top-level-buttons').children[2].click()");

    		setTimeout(function()
    		{
    			runCode(tabID, "var lists = document.querySelectorAll('paper-checkbox.ytd-playlist-add-to-option-renderer'); \
						for(var i = 0; i < lists.length; i++) \
							if(lists[i].innerText.trim() == '" + YOUTUBE_PLAYLIST + "') \
						    	if(lists[i].getAttribute('aria-checked') == 'true') \
									lists[i].click();");

    			setTimeout(function()
    			{
    				processMessage("skip");
    			}, 1000);
    		}, 1000);
    	}
    	else if(tabType == SPOTIFY) runCode(tabID, "document.getElementById('app-player').contentDocument.getElementById('track-add').click()");
    	else if(tabType == SPOTIFY_NEW) runCode(tabID, "document.querySelector('.spoticon-added-16').click()");
    	else if(tabType == SOUNDCLOUD) runCode(tabID, "document.getElementsByClassName('playbackSoundBadge__like')[0].click()");
    }
    else if(msg == "pause")
    {
    	if(tabType == YOUTUBE) runCode(tabID, "document.getElementsByClassName('ytp-play-button')[0].click()");
 		else if(tabType == SPOTIFY) runCode(tabID, "document.querySelector('.spoticon-play-32, .spoticon-pause-32').click()");
 		else if(tabType == SPOTIFY_NEW) runCode(tabID, "document.querySelector('.spoticon-pause-16, .spoticon-play-16').click()");
 		else if(tabType == SOUNDCLOUD) runCode(tabID, "document.getElementsByClassName('playControl')[0].click()");
    }
    else if(msg.startsWith("vol"))
    {
    	var vol = msg.substring(3);

    	runCode(tabID, "function mouseEvent(type, sx, sy, cx, cy) { \
						  var evt; \
						  var e = { \
						    bubbles: true, \
						    cancelable: (type != 'mousemove'), \
						    view: window, \
						    detail: 0, \
						    screenX: sx, \
						    screenY: sy, \
						    clientX: cx, \
						    clientY: cy, \
						    ctrlKey: false, \
						    altKey: false, \
						    shiftKey: false, \
						    metaKey: false, \
						    button: 0, \
						    relatedTarget: undefined \
						  }; \
						  if (typeof( document.createEvent ) == 'function') { \
						    evt = document.createEvent('MouseEvents'); \
						    evt.initMouseEvent(type, \
						      e.bubbles, e.cancelable, e.view, e.detail, \
						      e.screenX, e.screenY, e.clientX, e.clientY, \
						      e.ctrlKey, e.altKey, e.shiftKey, e.metaKey, \
						      e.button, document.body.parentNode); \
						  } else if (document.createEventObject) { \
						    evt = document.createEventObject(); \
						    for (prop in e) { \
							    evt[prop] = e[prop]; \
							  } \
						    evt.button = { 0:1, 1:4, 2:2 }[evt.button] || evt.button; \
						  } \
						  return evt; \
						} \
						function dispatchEvent (el, evt) { \
						  if (el.dispatchEvent) { \
						    el.dispatchEvent(evt); \
						  } else if (el.fireEvent) { \
						    el.fireEvent('on' + type, evt); \
						  } \
						  return evt; \
						} \
						\
						function mouseEventEx(type, elem, oX, oY) \
						{ \
							var el = document.getElementsByClassName(elem)[0]; \
							var rect = el.getBoundingClientRect(); \
							var x = rect.left + oX; \
							var y = rect.top + oY; \
							return mouseEvent(type, x, y, x, y); \
						} \
						\
						var amt = (" + vol + "/100) * 52; \
						dispatchEvent(document.getElementsByClassName('ytp-volume-slider')[0], mouseEventEx('mousedown', 'ytp-volume-slider', 0, 0)); \
						dispatchEvent(document.getElementsByClassName('ytp-volume-slider')[0], mouseEventEx('mousemove', 'ytp-volume-slider', amt, 0)); \
						dispatchEvent(document.getElementsByClassName('ytp-volume-slider')[0], mouseEventEx('mouseup', 'ytp-volume-slider', amt, 0));");
    }
}

function findTab(msg)
{
	chrome.tabs.query
	(
		{},
		function(tabs)
		{
			for(var tabIndex = 0; tabIndex < tabs.length; tabIndex++)
			{
				if(tabs[tabIndex].audible)
				{
					if(tabs[tabIndex].url.indexOf("youtube.com") > -1)
					{
						tabType = YOUTUBE;
					}
					else if(tabs[tabIndex].url.indexOf("play.spotify.com") > -1)
					{
						tabType = SPOTIFY;
					}
					else if(tabs[tabIndex].url.indexOf("open.spotify.com") > -1)
					{
						tabType = SPOTIFY_NEW;
					}
					else if(tabs[tabIndex].url.indexOf("soundcloud.com") > -1)
					{
						tabType = SOUNDCLOUD;
					}

					tabID = tabs[tabIndex].id;
					break;
				}
			}
		}
	);
}

function scrollToSong()
{
	findTab();

	if(tabID != -1 && tabType == YOUTUBE)
	{
		runCode(tabID, "document.getElementById('items').scrollTop = 6400;");
	}
}

function onNativeMessage(msg)
{
	findTab();

	if(tabID != -1)
	{
		processMessage(msg.text);
	}
}