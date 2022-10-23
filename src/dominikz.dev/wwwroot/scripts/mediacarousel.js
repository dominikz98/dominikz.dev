
function changeCarouselScrollLeft(add) {

    const slidesContainer = document.getElementById("carousel-container");
    const slide = document.querySelector(".carouselitem");
    const slideWidth = slide.clientWidth;

    if (add) {
        slidesContainer.scrollLeft += slideWidth;
    } else {
        slidesContainer.scrollLeft -= slideWidth;
    }
}