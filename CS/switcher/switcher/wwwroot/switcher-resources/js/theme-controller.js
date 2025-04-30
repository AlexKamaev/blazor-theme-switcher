"use strict";

export const ThemeController = (function () {
    function setCookie(name, value, date) {
        document.cookie = escape(name) + '=' + escape(value.toString()) + '; expires=' + date.toGMTString() + '; path=/';
    }

    function setThemeName(cookieName, themeName) {
        var date = new Date();
        date.setFullYear(date.getFullYear() + 1);
        setCookie(cookieName, themeName, date);
    }
    
    async function switchTheme(isFluent, isBootstrapDark, name, cookie, reference) {
        
        const html = document.documentElement;
        const body = document.body;
        
        html.removeAttribute("data-bs-theme");
        body.classList.remove("dxbl-theme-fluent");
        
        if(isBootstrapDark)
            html.setAttribute("data-bs-theme", "dark");

        if(isFluent)
            body.classList.add("dxbl-theme-fluent");
        
        setThemeName(cookie, name);

        await reference.invokeMethodAsync("ThemeLoadedAsync");
    }

    return { switchTheme }
})();
