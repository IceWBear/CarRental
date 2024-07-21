(function () {
    function initial() {
        loadDataChartOrder();
        registerEvents();
    }

    function loadDataChartOrder() {

        $.ajax({
            url: '/Adminstrator/GetFeedbackIcebear', // Thay đổi đường dẫn này thành API endpoint của bạn
            method: 'GET',
            success: function (response) {
                if (!response || response.length == 0) {
                    console.log('Không có dữ liệu trả về từ API');
                    return;
                }

                // Gọi hàm để khởi tạo biểu đồ
                intialChartOrder(response);
            },
            error: function (xhr, status, error) {
                console.error('Lỗi khi tải dữ liệu:', error);
            }
        });
    }

    function intialChartOrder(dataSource) {
        var chartDom = document.getElementById('icehihi');
        var myChart = echarts.init(chartDom);
        var option;

        option = {
            title: {
                text: 'Feedback Report',
                left: 'center'
            },
            tooltip: {
                trigger: 'item'
            },
            legend: {
                orient: 'vertical',
                left: 'left'
            },
            series: [
                {
                    name: 'Access From',
                    type: 'pie',
                    radius: '50%',
                    data: dataSource,
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