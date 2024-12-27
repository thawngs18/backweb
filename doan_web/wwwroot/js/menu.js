
// isotope js
$(window).on("load", function () {
    // Xử lý khi bấm vào menu chính (filters_menu)
    $(".filters_menu li").click(function () {
      $(".filters_menu li").removeClass("active");
      $(this).addClass("active");
  
      // Lấy giá trị filter
      var data = $(this).attr("data-filter");
  
      // Kiểm tra nếu filter là 'monchinh', hiển thị filter_monchinh
      if (data === ".monchinh") {
        $(".filters_monchinh").show();
      } else {
        $(".filters_monchinh").hide(); // Ẩn filter_monchinh nếu không phải 'monchinh'
      }
  
      // Áp dụng filter cho isotope
      $grid.isotope({
        filter: data,
      });
    });
  
    // Xử lý các item bên trong filters_monchinh
    $(".filters_monchinh li").click(function () {
      $(".filters_monchinh li").removeClass("active");
      $(this).addClass("active");
  
      var data = $(this).attr("data-filter");
      $grid.isotope({
        filter: data,
      });
    });
  
    // Khởi tạo isotope
    var $grid = $(".grid").isotope({
      itemSelector: ".all",
      percentPosition: false,
      masonry: {
        columnWidth: ".all",
      },
    });
  
    // Ẩn filter_monchinh ban đầu
    $(".filters_monchinh").hide();
  });
  // nice select
$(document).ready(function () {
  $("select").niceSelect();
});
let cartCount = 0; // Biến để lưu số lượng sản phẩm

function addToCart(event) {
  event.preventDefault(); // Ngăn chuyển hướng link
  cartCount++; // Tăng số lượng
  document.getElementById("cart-count").textContent = cartCount; // Cập nhật số hiển thị
}