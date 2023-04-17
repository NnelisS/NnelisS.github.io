var toggleBtn = document.getElementById("toggle-btn");
      var panel = document.getElementById("panel");
      var body = document.getElementsByTagName("body")[0];

      toggleBtn.addEventListener("click", function() {
        panel.classList.toggle("open");
        body.classList.toggle("panel-open");
        toggleBtn.disabled = !toggleBtn.disabled;
      });

      window.addEventListener("click", function(event) {
        if (!panel.contains(event.target) && !toggleBtn.contains(event.target)) {
          panel.classList.remove("open");
          body.classList.remove("panel-open");
          toggleBtn.disabled = false;
        }
      });  
      
      panel.addEventListener("click", function(event) {
        event.stopPropagation();
      });    