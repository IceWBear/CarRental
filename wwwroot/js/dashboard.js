(function () {
    function initial() {
        loadDataChartOrder();
        registerEvents();
    }

    function loadDataChartOrder() {

        $.ajax({
            url: '/Adminstrator/GetdataIcebear', // Thay đổi đường dẫn này thành API endpoint của bạn
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
        var chartDom = document.getElementById('icehuhu');
        var myChart = echarts.init(chartDom, 'light');
        var option;

        option = {
            color: ['#50697f'],
            tooltip: {
                trigger: 'axis',
                axisPointer: {
                    type: 'light'
                }
            },
            grid: {
                left: '3%',
                right: '4%',
                bottom: '3%',
                containLabel: true
            },
            xAxis: [
                {
                    type: 'category',
                    data: dateSource,
                    axisTick: {
                        alignWithLabel: true
                    },
                    axisLine: {
                        lineStyle: {
                            color: '#999' // Màu của đường dọc trên trục x
                        }
                    },
                    axisLabel: {
                        textStyle: {
                            color: '#999' // Màu của nhãn trên trục x
                        }
                    }
                }
            ],
            yAxis: [
                {
                    type: 'value',
                    splitLine: {
                        lineStyle: {
                            type: 'dashed',
                            color: '#ccc' // Màu của đường chia trên trục y
                        }
                    },
                    axisLabel: {
                        textStyle: {
                            color: '#999' // Màu của nhãn trên trục y
                        }
                    }
                }
            ],
            series: [
                {
                    name: 'Action',
                    type: 'bar',
                    barWidth: '60%',
                    data: dataSource,
                    itemStyle: {
                        normal: {
                            color: '#50697f' // Màu của từng cột trong biểu đồ cột
                        }
                    }
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