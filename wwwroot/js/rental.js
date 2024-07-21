(function () {
    function initial() {
        loadDataChartOrder();
        registerEvents();
    }

    function loadDataChartOrder() {

        $.ajax({
            url: '/Adminstrator/GetRentalIcebear', // Thay đổi đường dẫn này thành API endpoint của bạn
            method: 'GET',
            success: function (response) {
                if (!response || response.length == 0) {
                    console.log('Không có dữ liệu trả về từ API');
                    return;
                }

                // Khởi tạo mảng dataSource và dateSource
                var dataSource = [];
                var dateSource = [];

                // Duyệt qua các transaction và thêm dữ liệu vào mảng
                response.forEach(function (item) {
                    dateSource.push(item.issueDate); // Đảm bảo IssueDate là kiểu Date
                    dataSource.push(item.action);
                });

                // Gọi hàm để khởi tạo biểu đồ
                intialChartOrder(dataSource, dateSource);
            },
            error: function (xhr, status, error) {
                console.error('Lỗi khi tải dữ liệu:', error);
            }
        });
    }

    function intialChartOrder(dataSource, dateSource) {
        var chartDom = document.getElementById('icehehe');
        var myChart = echarts.init(chartDom);
        var option;

        option = {
            xAxis: {
                type: 'category',
                data: dateSource
            },
            yAxis: {
                type: 'value'
            },
            series: [
                {
                    data: dataSource,
                    type: 'line',
                    smooth: true
                }
            ]
        };

        option && myChart.setOption(option);
    }

    function registerEvents() {

    };

    $(document).ready(function () {
        initial();
    });


})();