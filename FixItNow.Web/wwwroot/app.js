window.scrollToBottom = (id) => {
    const el = document.getElementById(id);
    if (el) el.scrollTop = el.scrollHeight;
};

window.scrollToElement = (id) => {
    const el = document.getElementById(id);
    const navbar = document.querySelector('.app-topbar') // your navbar class
    if (el) {
        const offset = navbar ? navbar.offsetHeight : 80;
        const top = el.getBoundingClientRect().top + window.scrollY - offset;
        window.scrollTo({ top, behavior: 'smooth' });
    }
};