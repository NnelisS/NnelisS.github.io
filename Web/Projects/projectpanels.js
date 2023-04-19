var toggleBtn = document.getElementById("toggle-btn");
      var panel = document.getElementById("panel");
      var body = document.getElementsByTagName("body")[0];
      var overlay = document.getElementsByClassName("overlay")[0];

      toggleBtn.addEventListener("click", function() {
        panel.classList.toggle("open");
        body.classList.toggle("panel-open");
        overlay.classList.toggle("opened")
        toggleBtn.disabled = !toggleBtn.disabled;
      });

      window.addEventListener("click", function(event) {
        if (!panel.contains(event.target) && !toggleBtn.contains(event.target)) {
          panel.classList.remove("open");
          body.classList.remove("panel-open");
          overlay.classList.remove("opened")
          toggleBtn.disabled = false;
        }
      });  

      panel.addEventListener("click", function(event) {
        event.stopPropagation();
      });    

      var playButton = document.getElementById("play-button");
var videoContainer = document.getElementById("video-container");
var videos = videoContainer.getElementsByClassName("video");

playButton.addEventListener("click", function() {
  // Hide the play button
  playButton.style.display = "none";
  
  // Show each video iframe
  for (var i = 0; i < videos.length; i++) {
    videos[i].style.display = "block";
  }
});