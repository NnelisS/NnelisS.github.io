function executeFunctionOnPageLoad() {
  const box = document.querySelector('.logo');
  box.classList.add('normal');
}

if (document.URL.includes('index.html')) {
  executeFunctionOnPageLoad();
}

function executeFunctionOnPageLoad() {
  const box = document.querySelector('.nav-buttons');
  box.classList.add('normal');
}

if (document.URL.includes('index.html')) {
  executeFunctionOnPageLoad();
}