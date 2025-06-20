document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll('.image-gallery').forEach((gallery, galleryIndex) => {
        const img = gallery.querySelector('img');
        const urls = gallery.querySelector('.image-urls').value.split(';');
        let index = 0;

        const prevBtn = gallery.querySelector('.prev-btn');
        const nextBtn = gallery.querySelector('.next-btn');

        function showImage(i) {
            if (i < 0) i = urls.length - 1;
            if (i >= urls.length) i = 0;
            index = i;
            img.style.opacity = 0;

            setTimeout(() => {
                img.src = urls[index];
                img.style.opacity = 1;
            }, 300);
        }

        if (prevBtn && nextBtn) {
            prevBtn.addEventListener('click', () => {
                showImage(index - 1);
            });

            nextBtn.addEventListener('click', () => {
                showImage(index + 1);
            });

            // Automatisk bläddring med individuell fördröjning
            const initialDelay = galleryIndex * 1000 + Math.random() * 1000;

            setTimeout(() => {
                setInterval(() => {
                    showImage(index + 1);
                }, 5000);
            }, initialDelay);
        }
    });



    document.querySelectorAll('.price-calculation-container').forEach(form => {
        const pricePerDay = parseFloat(form.dataset.pricePerDay);
        const startDateInput = form.querySelector('input[asp-for="StartDate"], input[name="StartDate"]') || form.querySelector('input[type="date"]:first-of-type');
        const endDateInput = form.querySelector('input[asp-for="EndDate"], input[name="EndDate"]') || form.querySelector('input[type="date"]:last-of-type');
        const priceOutput = form.querySelector('.price-output');

        if (!startDateInput || !endDateInput || !priceOutput || isNaN(pricePerDay)) return;

        function calculatePrice() {
            const startDate = new Date(startDateInput.value);
            const endDate = new Date(endDateInput.value);

            if (startDateInput.value && endDateInput.value && endDate > startDate) {
                const diffTime = endDate - startDate;
                const diffDays = diffTime / (1000 * 60 * 60 * 24);
                const totalPrice = diffDays * pricePerDay;
                priceOutput.value = totalPrice.toFixed(2) + " kr";
            } else {
                priceOutput.value = "";
            }
        }

        startDateInput.addEventListener('change', calculatePrice);
        endDateInput.addEventListener('change', calculatePrice);

        calculatePrice();
    });
});

