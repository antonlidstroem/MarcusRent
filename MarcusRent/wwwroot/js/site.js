document.addEventListener("DOMContentLoaded", function () {
    // Bildgalleri
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
            prevBtn.addEventListener('click', () => showImage(index - 1));
            nextBtn.addEventListener('click', () => showImage(index + 1));

            const initialDelay = galleryIndex * 1000 + Math.random() * 1000;
            setTimeout(() => {
                setInterval(() => showImage(index + 1), 5000);
            }, initialDelay);
        }
    });

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

        calculatePrice(); // kör direkt för att visa pris om datum är förifyllda
    });
});
