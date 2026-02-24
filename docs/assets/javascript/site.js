function setLang(lang) {
  document.querySelectorAll('.lang-content').forEach(el => el.style.display = 'none');
  document.getElementById('content-' + lang).style.display = 'block';
  document.getElementById('btn-en').classList.toggle('active', lang === 'en');
  document.getElementById('btn-hu').classList.toggle('active', lang === 'hu');
  localStorage.setItem('lang', lang);
}

document.addEventListener('DOMContentLoaded', () => {
  const saved = localStorage.getItem('lang') || 'en';
  setLang(saved);
});