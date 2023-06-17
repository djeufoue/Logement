var slideImg = document.getElementById("slideImg");
var images = [];

fetch('/Home/GetImages')
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        data.forEach(item => {
            images.push(`data:${item.contentType};base64,${item.data}`);
        });

        var len = images.length;
        var i = 0;

        function slider() {
            if (i > len - 1) {
                i = 0;
            }
            slideImg.src = images[i];
            i++;
            setTimeout(() => slider(), 5000);
        }
        slider();
    })
    .catch(error => {
        console.error('Error fetching images:', error);
        alert('An error occurred while fetching images. Please try again later.');
    });


var TrandingSlider = new Swiper('.tranding-slider', {
    effect: 'coverflow',
    grabCursor: true,
    centeredSlides: true,
    loop: true,
    slidesPerView: 'auto',
    coverflowEffect: {
        rotate: 0,
        stretch: 0,
        depth: 100,
        modifier: 2.5,
    },
    pagination: {
        el: '.swiper-pagination',
        clickable: true,
    },
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
    }
});