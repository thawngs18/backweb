(function($) {
    "use strict";
  
    var ChartHandler = function () {};
  
    ChartHandler.prototype.respChart = function (canvas, chartType, data, options) {
        var ctx = canvas.get(0).getContext("2d");
        var parent = $(canvas).parent();
  
        function adjustCanvasSize() {
            canvas.attr("width", parent.width());
            switch (chartType) {
                case "Line":
                    new Chart(ctx, { type: "line", data: data, options: options });
                    break;
                case "Doughnut":
                    new Chart(ctx, { type: "doughnut", data: data, options: options });
                    break;
                case "Pie":
                    new Chart(ctx, { type: "pie", data: data, options: options });
                    break;
                case "Bar":
                    new Chart(ctx, { type: "bar", data: data, options: options });
                    break;
                case "Radar":
                    new Chart(ctx, { type: "radar", data: data, options: options });
                    break;
                case "PolarArea":
                    new Chart(ctx, { type: "polarArea", data: data, options: options });
                    break;
            }
        }
  
        $(window).resize(adjustCanvasSize);
        adjustCanvasSize();
    };
  
    ChartHandler.prototype.init = function () {
        // this.respChart($("#pie1"), "Pie", {
        //     labels: ["Đang xử lý", "Đã xử lý"],
        //     datasets: [{
        //         data: [80, 50],
        //         backgroundColor: ["#5d6dc3", "#3ec396"],
        //         hoverBackgroundColor: ["#5d6dc3", "#3ec396"],
        //         hoverBorderColor: "#fff"
        //     }]
        // });

        $.ajax({
            url: '/admin/data/pie',
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                $.ChartJs.respChart($("#pie1"), "Pie", data);
            },
            error: function (error) {
                console.error('Lỗi khi tải dữ liệu biểu đồ: ' + error);
            }
        });

        $.ajax({
            url: '/admin/data/bar',
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                var options = {
                    scales: {
                        yAxes: [
                            {
                                ticks: {
                                    min: 0,      // Giá trị tối thiểu trên trục y
                                    max: 10,    // Giá trị tối đa trên trục y
                                    stepSize: 2  // Khoảng cách giữa hai giá trị
                                }
                            }
                        ]
                    }
                };

                $.ChartJs.respChart($("#bar"), "Bar", data, options);
            },
            error: function (error) {
                console.error('Lỗi khi tải dữ liệu biểu đồ: ' + error);
            }
        });
  

    };
  
    $.ChartJs = new ChartHandler;
    $.ChartJs.Constructor = ChartHandler;
  
    // Gọi hàm init của ChartJs
    $.ChartJs.init();
  
    // Khởi tạo DataTable cho bảng có id "datatable"
    $("#datatable").DataTable();

    $("#datatable-buttons").DataTable({
        lengthChange: false,
        buttons: ["copy", "excel", "pdf"],
    });
    // Di chuyển nút xuất và sao chép vào vị trí mong muốn
    $("#datatable-buttons_wrapper .col-md-6:eq(0)").append(
      $("#datatable-buttons_wrapper .dt-buttons")
    );





})(window.jQuery);
