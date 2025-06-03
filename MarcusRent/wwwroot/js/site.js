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

        prevBtn.addEventListener('click', () => {
            showImage(index - 1);
        });

        nextBtn.addEventListener('click', () => {
            showImage(index + 1);
        });

        // Automatisk bläddring med individuell startfördröjning
        const initialDelay = galleryIndex * 1000 + Math.random() * 1000; // förskjutning 0–(N sekunder)

        setTimeout(() => {
            setInterval(() => {
                showImage(index + 1);
            }, 5000);
        }, initialDelay);
    });
});
