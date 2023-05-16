// Load the YouTube API
var tag = document.createElement('script');
tag.src = "https://www.youtube.com/embed/9NrTCwsaEm0?";
var firstScriptTag = document.getElementsByTagName('script')[0];
firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

// Create a new player object
var player;

function onYouTubeIframeAPIReady() {
  player = new YT.Player('player', {
    playerVars: {
      controls: 0 // Disable the player's controls
    }
  });
}

if (window.history && window.history.pushState) {
  window.history.pushState("", "", "/Homepage");
}