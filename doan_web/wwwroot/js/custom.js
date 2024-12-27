// Lấy năm hiện tại
function getYear() {
  const currentYear = new Date().getFullYear();
  document.querySelector("#displayYear").textContent = currentYear;
}
getYear();

// Thay đổi hình ảnh tự động
const images = [
  "images/banner/b2.png",
  "images/banner/b3.png",
  "images/banner/b4.png",
  "images/banner/b5.png",
  "images/banner/b6.png",
  "images/banner/b7.png",
];
let currentIndex = 0;

function changeImage() {
  currentIndex = (currentIndex + 1) % images.length;
  document.getElementById("myImage").src = images[currentIndex];
}
setInterval(changeImage, 2500);

// Isotope JS
$(window).on("load", function () {
  const $grid = $(".grid").isotope({
    itemSelector: ".all",
    percentPosition: true,
    masonry: { columnWidth: ".all" },
  });

  $(".filters_menu li").click(function () {
    $(".filters_menu li").removeClass("active");
    $(this).addClass("active");
    const filterValue = $(this).attr("data-filter");
    if (filterValue === ".monchinh") {
      $(".filters_monchinh").show();
    } else {
      $(".filters_monchinh").hide();
    }
    $grid.isotope({ filter: filterValue });
  });

  $(".filters_monchinh li").click(function () {
    $(".filters_monchinh li").removeClass("active");
    $(this).addClass("active");
    const filterValue = $(this).attr("data-filter");
    $grid.isotope({ filter: filterValue });
  });

  $(".filters_monchinh").hide();
});

// Nice Select
$(document).ready(function () {
  $("select").niceSelect();
});

// Google Map
function myMap() {
  const mapProp = {
    center: new google.maps.LatLng(40.712775, -74.005973),
    zoom: 18,
  };
  new google.maps.Map(document.getElementById("googleMap"), mapProp);
}

// Owl Carousel
$(".client_owl-carousel").owlCarousel({
  loop: true,
  margin: 0,
  nav: true,
  autoplay: true,
  autoplayHoverPause: true,
  navText: [
    '<i class="fa fa-angle-left" aria-hidden="true"></i>',
    '<i class="fa fa-angle-right" aria-hidden="true"></i>',
  ],
  responsive: {
    0: { items: 1 },
    768: { items: 2 },
    1000: { items: 2 },
  },
});

// Giỏ hàng
let cartCount = 0;

function addToCart(event) {
  event.preventDefault();
  cartCount++;
  document.getElementById("cart-count").textContent = cartCount;
}

// Cập nhật tổng tiền
function updatePrice() {
  let totalPrice = 0;
  const cartItems = document.querySelectorAll(".cart-item");
  cartItems.forEach((item) => {
    const priceText = item.children[1].innerText.replace(/[^\d]/g, "");
    const quantity = parseInt(item.querySelector("input").value) || 0;
    totalPrice += Number(priceText) * quantity;
  });
  document.getElementById("total-price").textContent =
    new Intl.NumberFormat("vi-VN").format(totalPrice) + "đ";
}

// Chuyển đến form thanh toán
// Đảm bảo form thông tin bị ẩn khi trang được tải
window.onload = function () {
  const checkoutForm = document.querySelector(".checkout-form");
  const cart = document.querySelector(".cart");

  // Kiểm tra nếu các phần tử tồn tại trước khi thao tác
  if (checkoutForm && cart) {
    checkoutForm.style.display = "none"; // Ẩn form thông tin đơn hàng
    cart.style.display = "block"; // Hiển thị giỏ hàng
  }
};

// Hàm chuyển từ giỏ hàng sang form thông tin đơn hàng
function checkout() {
  const checkoutForm = document.querySelector(".checkout-form");
  const cart = document.querySelector(".cart");

  if (cart && checkoutForm) {
    const cartItems = document.querySelectorAll(".cart-item");
    if (cartItems.length === 0) {
      alert("Giỏ hàng của bạn đang trống!");
      return;
    }

    cart.style.display = "none"; // Ẩn giỏ hàng
    checkoutForm.style.display = "block"; // Hiển thị form thông tin
  }
}

