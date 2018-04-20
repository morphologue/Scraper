// Put everything inside an immediately invoked function to avoid using the global namespace. This
// is not of great importance seeing as libraries aren't allowed, but still tidier IMO.
(function () {
    var form = document.getElementsByTagName('form')[0];
    var inputs = form.getElementsByTagName('input');
    var result = document.getElementsByTagName('p')[0];
    var strong_result = result.getElementsByTagName('strong')[0];
    var normal_result = result.getElementsByTagName('span')[0];
    var checking_interval;

    function setResult(strong, normal) {
        strong_result.textContent = strong;
        normal_result.textContent = normal;
    }
    
    function showResult() {
        if (result.classList.contains('hidden'))
            result.classList.remove('hidden');
    }

    // Set the result to "Checking" followed by a soothing pattern of 1-5 dots. Note that this does
    // not actually show the result.
    function startChecking() {
        var dot_count = 1;
        function stepDots() {
            setResult('Checking' + new Array(++dot_count).join('.'));
            if (dot_count === 6)
                dot_count = 1;    
        }

        stepDots();
        checking_interval = setInterval(stepDots, 800);
    }

    // Stop the dots (mobilised by startChecking()) from moving, if they are moving.
    function stopChecking() {
        checking_interval && clearInterval(checking_interval);
    }

    // Simulate form submission via AJAX (= no page load).
    form.addEventListener('submit', function(evt) {
        evt.preventDefault();

        var query = inputs[0].value;
        if (!query) {
            alert('Please enter search terms.');
            inputs[1].blur();
            return;
        }

        // Disable the form until the server returns.
        for (var i = 0; i < inputs.length; i++)
            inputs[i].disabled = true;
        
        // Show a textual "spinner".
        startChecking();
        showResult();
        
        var xhr = new XMLHttpRequest();
        xhr.open('POST', 'CheckRankings');
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.onload = function () {
            stopChecking();

            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                setResult(response.strong || '', response.normal || '');
            } else
                setResult('Error: ', 'Status code ' + xhr.status + ' was returned. This might be resolved by trying again.');

            // Re-enable the form.
            for (var i = 0; i < inputs.length; i++)
                inputs[i].disabled = false;
        };
        xhr.send('query=' + encodeURIComponent(query));
    }, false);
})();
