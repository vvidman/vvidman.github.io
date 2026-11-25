(function () {
  'use strict';

  var root = document.documentElement;

  function store(key, value) {
    try { localStorage.setItem(key, value); } catch (e) { /* private mode / blocked storage */ }
  }

  // ── Language ────────────────────────────
  function applyLang(lang, persist) {
    if (lang !== 'en' && lang !== 'hu') return;
    root.setAttribute('data-lang', lang);
    root.setAttribute('lang', lang);

    var buttons = document.querySelectorAll('[data-set-lang]');
    for (var i = 0; i < buttons.length; i++) {
      buttons[i].setAttribute('aria-pressed', String(buttons[i].getAttribute('data-set-lang') === lang));
    }

    var title = root.getAttribute('data-title-' + lang);
    if (title) document.title = title;

    if (persist) store('lang', lang);
  }

  // ── Theme ───────────────────────────────
  function applyTheme(theme, persist) {
    root.setAttribute('data-theme', theme);
    if (persist) store('theme', theme);
  }

  // ── Code blocks: frame, language label, copy button ──
  function languageOf(pre) {
    var wrapper = pre.parentElement;
    // ColorCode: <div class="csharp"><pre>…</pre></div>
    if (wrapper && wrapper.tagName === 'DIV' && wrapper.children.length === 1 && wrapper.className && wrapper.className.indexOf('code-') !== 0) {
      return { label: wrapper.className.split(/\s+/)[0], target: wrapper };
    }
    // Markdig: <pre><code class="language-bash">
    var code = pre.querySelector('code');
    if (code) {
      var m = /(?:^|\s)language-([\w#+-]+)/.exec(code.className);
      if (m) return { label: m[1], target: pre };
    }
    return { label: 'text', target: pre };
  }

  function setCopyLabel(button, en, hu) {
    var spans = button.querySelectorAll('[data-l]');
    for (var i = 0; i < spans.length; i++) {
      spans[i].textContent = spans[i].getAttribute('data-l') === 'hu' ? hu : en;
    }
  }

  function copyText(text) {
    if (navigator.clipboard && navigator.clipboard.writeText) {
      return navigator.clipboard.writeText(text);
    }
    return new Promise(function (resolve, reject) {
      var ta = document.createElement('textarea');
      ta.value = text;
      ta.setAttribute('readonly', '');
      ta.style.position = 'fixed';
      ta.style.opacity = '0';
      document.body.appendChild(ta);
      ta.select();
      try { document.execCommand('copy') ? resolve() : reject(); } catch (e) { reject(e); }
      document.body.removeChild(ta);
    });
  }

  function decorateCode() {
    var pres = document.querySelectorAll('.prose pre');
    for (var i = 0; i < pres.length; i++) {
      var pre = pres[i];
      if (pre.closest('.code-frame')) continue;

      var info = languageOf(pre);
      var target = info.target;

      var frame = document.createElement('div');
      frame.className = 'code-frame';

      var head = document.createElement('div');
      head.className = 'code-head';

      var label = document.createElement('span');
      label.textContent = info.label;

      var button = document.createElement('button');
      button.type = 'button';
      button.innerHTML = '<span data-l="en">Copy</span><span data-l="hu">Másolás</span>';

      head.appendChild(label);
      head.appendChild(button);

      target.parentNode.insertBefore(frame, target);
      frame.appendChild(head);
      frame.appendChild(target);

      (function (btn, source) {
        var timer;
        btn.addEventListener('click', function () {
          copyText(source.textContent).then(function () {
            setCopyLabel(btn, 'Copied', 'Másolva');
            clearTimeout(timer);
            timer = setTimeout(function () { setCopyLabel(btn, 'Copy', 'Másolás'); }, 1500);
          }, function () { /* clipboard denied: leave the label unchanged */ });
        });
      })(button, pre);
    }
  }

  // ── Series tree: collapsible on mobile, always open on desktop ──
  function initSeriesTree() {
    var tree = document.querySelector('details.series-tree');
    if (!tree || !window.matchMedia) return;
    var mq = window.matchMedia('(max-width: 760px)');
    function sync() {
      if (mq.matches) tree.removeAttribute('open'); else tree.setAttribute('open', '');
    }
    sync();
    if (mq.addEventListener) mq.addEventListener('change', sync);
    // keyboard toggling must not collapse the tree on desktop
    tree.addEventListener('toggle', function () {
      if (!mq.matches && !tree.open) tree.setAttribute('open', '');
    });
  }

  document.addEventListener('DOMContentLoaded', function () {
    initSeriesTree();
    // The inline <head> script already set data-lang / data-theme; sync the UI to it.
    applyLang(root.getAttribute('data-lang') || 'en', false);

    var langButtons = document.querySelectorAll('[data-set-lang]');
    for (var i = 0; i < langButtons.length; i++) {
      langButtons[i].addEventListener('click', function (e) {
        applyLang(e.currentTarget.getAttribute('data-set-lang'), true);
      });
    }

    var themeButtons = document.querySelectorAll('[data-toggle-theme]');
    for (var j = 0; j < themeButtons.length; j++) {
      themeButtons[j].addEventListener('click', function () {
        applyTheme(root.getAttribute('data-theme') === 'dark' ? 'light' : 'dark', true);
      });
    }

    decorateCode();
  });
})();
