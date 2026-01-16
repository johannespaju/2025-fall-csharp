// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Auto-hide toasts after 5 seconds
document.addEventListener('DOMContentLoaded', function() {
    const toasts = document.querySelectorAll('.toast');
    toasts.forEach(toast => {
        setTimeout(() => {
            const bsToast = new bootstrap.Toast(toast);
            bsToast.hide();
        }, 5000);
    });
});

// Show loading spinner on form submit
document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function() {
            const submitBtn = form.querySelector('button[type="submit"]');
            if (submitBtn && !submitBtn.disabled) {
                submitBtn.disabled = true;
                const originalText = submitBtn.innerHTML;
                submitBtn.innerHTML = `
                    <span class="spinner-border spinner-border-sm me-1" role="status"></span>
                    Processing...
                `;
                
                // Re-enable if form validation fails
                setTimeout(() => {
                    if (form.querySelectorAll('.field-validation-error').length > 0) {
                        submitBtn.disabled = false;
                        submitBtn.innerHTML = originalText;
                    }
                }, 100);
            }
        });
    });
});

// Confirm delete actions
document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('a[href*="/Delete"], button[formaction*="/Delete"]')
        .forEach(btn => {
            btn.addEventListener('click', function(e) {
                const itemName = this.getAttribute('data-item-name') || 'this item';
                if (!confirm(`Are you sure you want to delete ${itemName}?`)) {
                    e.preventDefault();
                    return false;
                }
            });
        });
});

// Dark Mode Toggle
document.addEventListener('DOMContentLoaded', function() {
    const themeToggle = document.getElementById('theme-toggle');
    const themeIcon = document.getElementById('theme-icon');
    
    if (!themeToggle) return;
    
    // Load saved theme
    const savedTheme = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-theme', savedTheme);
    updateThemeIcon(savedTheme);
    
    // Toggle theme on click
    themeToggle.addEventListener('click', function() {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        
        document.documentElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('theme', newTheme);
        updateThemeIcon(newTheme);
    });
    
    function updateThemeIcon(theme) {
        if (theme === 'dark') {
            themeIcon.classList.remove('bi-moon-fill');
            themeIcon.classList.add('bi-sun-fill');
        } else {
            themeIcon.classList.remove('bi-sun-fill');
            themeIcon.classList.add('bi-moon-fill');
        }
    }
});

// Keyboard shortcuts
document.addEventListener('keydown', function(e) {
    // Press 'n' to create new item on index pages
    if (e.key === 'n' && !e.ctrlKey && !e.metaKey && 
        document.activeElement.tagName !== 'INPUT' &&
        document.activeElement.tagName !== 'TEXTAREA') {
        const createLink = document.querySelector('a[href*="/Create"]');
        if (createLink) {
            e.preventDefault();
            createLink.click();
        }
    }
    
    // Press '/' to focus search input
    if (e.key === '/' && 
        document.activeElement.tagName !== 'INPUT') {
        e.preventDefault();
        const searchInput = document.querySelector('input[name="SearchTerm"]');
        if (searchInput) searchInput.focus();
    }
});
