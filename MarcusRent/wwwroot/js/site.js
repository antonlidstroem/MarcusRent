document.addEventListener("DOMContentLoaded", function () {

    // Prisberäkning
    document.querySelectorAll('.price-calculation-container').forEach(form => {
        const pricePerDay = parseFloat(form.dataset.pricePerDay);
        const startDateInput = form.querySelector('input[name="StartDate"]');
        const endDateInput = form.querySelector('input[name="EndDate"]');
        const priceOutput = form.querySelector('.price-output');
        const priceInput = form.querySelector('#price');

        if (!startDateInput || !endDateInput || !priceOutput || isNaN(pricePerDay)) return;

        function calculatePrice() {
            const startDate = new Date(startDateInput.value);
            const endDate = new Date(endDateInput.value);

            if (startDateInput.value && endDateInput.value && endDate > startDate) {
                const diffTime = endDate - startDate;
                const diffDays = diffTime / (1000 * 60 * 60 * 24);
                const totalPrice = diffDays * pricePerDay;

                priceOutput.textContent = totalPrice.toFixed(2) + " kr";
                if (priceInput) {
                    priceInput.value = totalPrice.toFixed(2) + " kr";
                }
            } else {
                priceOutput.textContent = "";
                if (priceInput) {
                    priceInput.value = "";
                }
            }
        }

        startDateInput.addEventListener('change', calculatePrice);
        endDateInput.addEventListener('change', calculatePrice);

        calculatePrice(); 
    });



   //STÄNGER INFORMATIONSRUTAN MED INLOGGNINGSUPPGIFTER EFTER 3 SEKUNDER
   
        setTimeout(function () {
        var alert = document.getElementById('demo-alert');
        if (alert) {
            // Bootstrap 5 dismiss
            var bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
        bsAlert.close();
        }
    }, 3000);



});
