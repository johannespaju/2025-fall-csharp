// Copy game link to clipboard
function copyGameLink(url, event) {
    navigator.clipboard.writeText(url).then(function() {
        // Temporarily change button text to show feedback
        var button = event.target;
        var originalText = button.textContent;
        button.textContent = 'Copied!';
        button.classList.remove('btn-info');
        button.classList.add('btn-success');
        
        setTimeout(function() {
            button.textContent = originalText;
            button.classList.remove('btn-success');
            button.classList.add('btn-info');
        }, 2000);
    }).catch(function(err) {
        console.error('Failed to copy: ', err);
        alert('Failed to copy link to clipboard');
    });
}
