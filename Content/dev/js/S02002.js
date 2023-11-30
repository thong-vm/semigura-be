
function LoadDataByFactoryId() {
    // 選択しているファクトリーを取得
    let factoryId = document.getElementById('FactoryId').value;
    let isInUse = document.getElementById('IsInUse').checked;
    
    let params = {};
    params["factoryId"] = factoryId;
    params["isInUse"] = isInUse;
    let data = Func.ConcatParamOfForm(params);

    // サーバーのデータを取得
    Func.ajax({
        type: 'POST',
        url: '/S02002/LoadLotByFactoryId',
        data: data,
        async: true,
        cache: false,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {
                LotDataRender(res.data);
            } else {
                Func.ShowErrorMessageForm(res.message);
            }
        },
        error: function (res) {
            Func.ShowErrorMessageForm(res);
        }
    });

    SetCookies();
}

/*
 * LotDataRender
 * Get Lot by FactoryId, render data in select element
 */
function LotDataRender(data) {
    let html = '';
    if (data && data.length > 0) {
        //html += '<option value=""></option>';
        for (let i = 0; i < data.length; i++) {
            // ロット選択の描画            
            html += '<option value="' + data[i].id + '">' + data[i].code + '</option>';
        }
    }
    document.getElementById('LotId').innerHTML = html;
    GetDataByLotId();
}

/*
 * GetDataByLotId
 * Get All Lot Info by LotId
 */
function GetDataByLotId(isRefresh) {
    chart_changeMode = 0;
    // 選択しているロケーションを取得
    var lotId = document.getElementById('LotId').value;
    if (lotId != "") {
        document.getElementById('process_content').style.display = "flex";
        // サーバーのデータを取得
        if (isRefresh) {
            UtilSpinnerRefresh.Show();
        } else {
            UtilSpinner.Show();
        }
        Func.ajax({            
            type: 'POST',
            url: '/S02002/GetDataByLotId',
            data: 'lotId=' + lotId,
            async: true,
            cache: false,
            processData: false,
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            success: function (res) {                
                if (res.status) {
                    RenderNewestDataByLotId(res.data.newDataByLotId);
                    DisplayChart(res.data.allDataByLotId);
                } else {
                    Func.ShowErrorMessageForm(res.message);
                }
            },
            error: function (res) {
                Func.ShowErrorMessageForm(res);
            },
            complete: function () {
                if (isRefresh) {
                    UtilSpinnerRefresh.Hide();
                } else {
                    UtilSpinner.Hide();
                }
            },
        }, true);
    } else {
        document.getElementById('process_content').style.display = "none";
    }
    document.getElementById('S02002_search_date').value = '';
    SetCookies();
}

function RenderNewestDataByLotId(data) {
    let temp_i = document.getElementById('temp_i').value;
    let room_temp_i = document.getElementById('room_temp_i').value;
    let room_humi_i = document.getElementById('room_humi_i').value;
    let room_temp_label = document.getElementById('room_temp_label').value;
    let room_humi_label = document.getElementById('room_humi_label').value;
    let html = '';
    let html1 = '';
    if (data) {
        if (data.location.length > 0) {
            document.getElementById('S02002_room_block').style.display = "flex";
            html1 += '<div class="content"> ';
            html1 += '                <div class="content_title"> ';
            html1 += '                    <p>&nbsp;' + room_temp_label + '</p> ';
            html1 += '                </div> ';
            html1 += '                <div class="content_desc"> ';
            if (data.location[0].locationTemp != null) {
                html1 += '                    <p>' + data.location[0].locationTemp + '°C</p> ';
            } else {
                html1 += '                    <p>°C</p> ';
            }
            html1 += '                </div> ';
            html1 += '                <div class="content_icon"> ';
            html1 += '                    <img src="' + room_temp_i + '" alt="temp.png" /> ';
            html1 += '                </div> ';
            html1 += '            </div> ';
            html1 += '            <div class="content"> ';
            html1 += '                <div class="content_title"> ';
            html1 += '                    <p>&nbsp;' + room_humi_label + '</p> ';
            html1 += '                </div> ';
            html1 += '                <div class="content_desc"> ';
            if (data.location[0].locationHumi != null) {
                html1 += '                    <p>' + data.location[0].locationHumi + '%</p> ';
            } else {
                html1 += '                    <p>°C</p> ';
            }
            html1 += '                </div> ';
            html1 += '                <div class="content_icon"> ';
            html1 += '                    <img src="' + room_humi_i + '" alt="temp.png" /> ';
            html1 += '                </div> ';
            html1 += '            </div> ';
        }
        else {
            document.getElementById('S02002_room_block').style.display = "flex";
            html1 += '<div class="content"> ';
            html1 += '                <div class="content_title"> ';
            html1 += '                    <p>&nbsp;' + room_temp_label + '</p> ';
            html1 += '                </div> ';
            html1 += '                <div class="content_desc"> ';
            html1 += '                    <p>°C</p> ';
            html1 += '                </div> ';
            html1 += '                <div class="content_icon"> ';
            html1 += '                    <img src="' + room_temp_i + '" alt="temp.png" /> ';
            html1 += '                </div> ';
            html1 += '            </div> ';
            html1 += '            <div class="content"> ';
            html1 += '                <div class="content_title"> ';
            html1 += '                    <p>&nbsp;' + room_humi_label + '</p> ';
            html1 += '                </div> ';
            html1 += '                <div class="content_desc"> ';
            html1 += '                    <p>%</p> ';
            html1 += '                </div> ';
            html1 += '                <div class="content_icon"> ';
            html1 += '                    <img src="' + room_humi_i + '" alt="temp.png" /> ';
            html1 += '                </div> ';
            html1 += '            </div> ';
        }

        if (data.senSor.length > 0) {
            document.getElementById('sensor_block').style.display = "flex";
            for (let i = 0; i < data.senSor.length; i++) {
                if (data.senSor[i].sensorName != null) {
                    html += '<div class="content"> ';
                    html += '                <div class="content_title"> ';
                    html += '                    <p>&nbsp;' + data.senSor[i].sensorName + '(Ch.1)</p> ';
                    html += '                </div> ';
                    html += '                <div class="content_desc"> ';
                    if (data.senSor[i].newestTemp1 != null) {
                        html += '                    <p>' + data.senSor[i].newestTemp1 + '°C</p> ';
                    } else {
                        html += '                    <p>°C</p> ';
                    }
                    html += '                </div> ';
                    html += '                <div class="content_icon"> ';
                    html += '                    <img src="' + temp_i + '" alt="temp.png" /> ';
                    html += '                </div> ';
                    html += '            </div> ';
                    html += '            <div class="content"> ';
                    html += '                <div class="content_title"> ';
                    html += '                    <p>&nbsp;' + data.senSor[i].sensorName + '(Ch.2)</p> ';
                    html += '                </div> ';
                    html += '                <div class="content_desc"> ';
                    if (data.senSor[i].newestTemp2 != null) {
                        html += '                    <p>' + data.senSor[i].newestTemp2 + '°C</p> ';
                    } else {
                        html += '                    <p>°C</p> ';
                    }
                    html += '                </div> ';
                    html += '                <div class="content_icon"> ';
                    html += '                    <img src="' + temp_i + '" alt="temp.png" /> ';
                    html += '                </div> ';
                    html += '            </div> ';
                }                
            }
        } else {
            document.getElementById('sensor_block').style.display = "none";
        }
    }
    document.getElementById('S02002_room_block').innerHTML = html1;
    document.getElementById('sensor_block').innerHTML = html;
}

/*
 * Render data to Chart by LocationId
 */
function DisplayChart(data) {
    if (data) {
        document.getElementById('S02002_chart_content').style.display = "block";
        RenderChart(data);
    } else {
        document.getElementById('S02002_chart_content').style.display = "none";
    }
}
var S02002_chart;
var chart_changeMode = 0;
function RenderChart(data) {
    if (data) {
        document.getElementById('S02002_lineChart').style.display = "block";
        let labelY = document.getElementById('temperatureLabel').value;
        let ctxL1 = document.getElementById('S02002_lineChart').getContext('2d');
        let datetimeX = new Array();
        for (let j = 0; j < data.listDateTimeUnixTimeStamp.length; j++) {

            let date = moment(data.listDateTimeUnixTimeStamp[j]).format("YY/MM/DD HH:mm").toUpperCase();
            datetimeX.push(date);
        }
        let color = ['#DF0029', '#945305', '#C18C00', '#5BBD2B', '#C8E2B1', '#00A06B', '#00A6AD', '#184785', '#322275', '#79378B',
            '#E54646', '#F6B297', '#F5B16D', '#D59B00', '#FCF54C', '#50A625', '#008C5E', '#008489', '#205AA7', '#A095C4',
            '#FF0000', '#FF1493', '#FFB5C5', '#CD2990', '#DA70D6', '#CD96CD', '#9932CC', '#008080', '#CC3399', '#000080'];
        let datasets = [];
        for (let k = 0; k < data.senSor.length; k++) {
            let dataset1 = {};
            dataset1["type"] = 'line';
            dataset1["label"] = data.senSor[k].sensorName + '(Ch.1)';
            dataset1["yAxisID"] = 'A';
            dataset1["borderColor"] = color[k];
            dataset1["fill"] = false;
            dataset1["data"] = data.senSor[k].temperature1;
            datasets.push(dataset1);
            let dataset2 = {};
            dataset2["type"] = 'line';
            dataset2["label"] = data.senSor[k].sensorName + '(Ch.2)';
            dataset2["yAxisID"] = 'A';
            dataset2["borderColor"] = color[color.length - k - 1];
            dataset2["fill"] = false;
            dataset2["data"] = data.senSor[k].temperature2;
            datasets.push(dataset2);
        }
        if (S02002_chart) {
            S02002_chart.destroy();
        }
        S02002_chart = new Chart(ctxL1, {
            type: 'line',
            data: {
                labels: datetimeX,
                datasets: datasets,
            },
            options: {
                animation: {
                    duration: 0
                },
                interaction: {
                    intersect: false,
                },
                maintainAspectRatio: false,
                scales: {
                    xAxes: [{
                        id: 'xAxis1',
                        type: 'category',
                        ticks: {
                            callback: function (label) {
                                return label;
                            }
                        }
                    }
                ],
                    yAxes: [{
                        id: 'A',
                        type: 'linear',
                        position: 'left',
                        scaleLabel: {
                            display: true,
                            labelString: labelY
                        }
                    }]
                }
            }
        });
    } else {
        document.getElementById('S02002_lineChart').style.display = "none";
    }    
}

/*
 * GetDataByLastDay
 * Get All Lot Info by lastDate
 */
function GetDataByLastDay(isRefresh) {
    chart_changeMode = 1;
    // 選択しているロケーションを取得
    var lotId = document.getElementById('LotId').value;
    if (lotId != "") {       
        // サーバーのデータを取得
        if (isRefresh) {
            UtilSpinnerRefresh.Show();
        } else {
            UtilSpinner.Show();
        }

        Func.ajax({
            type: 'POST',
            url: '/S02002/GetDataByLastDay',
            data: 'lotId=' + lotId,
            async: true,
            cache: false,
            processData: false,
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            success: function (res) {
                if (res.status) {
                    RenderChart(res.data.allDataByLotId);
                } else {
                    Func.ShowErrorMessageForm(res.message);
                }
            },
            error: function (res) {
                Func.ShowErrorMessageForm(res);
            },
            complete: function () {
                if (isRefresh) {
                    UtilSpinnerRefresh.Hide();
                } else {
                    UtilSpinner.Hide();
                }
            }
        }, true);
    }
    document.getElementById('S02002_search_date').value = '';
}

/*
 * GetDataBySearchDay
 * Get All Lot Info by searchDate
 */
function GetDataBySearchDay() {
    chart_changeMode = 2;
    // 選択しているロケーションを取得
    const lotId = document.getElementById('LotId').value;
    const searchDate = document.getElementById('S02002_search_date').value;
    if (lotId != "") {
        let params = {};
        params["LotId"] = lotId;
        params["SearchByDate"] = searchDate;
        let data = Func.ConcatParamOfForm(params);
        // サーバーのデータを取得
        Func.ajax({
            type: 'POST',
            url: '/S02002/GetDataBySearchDay',
            data: data,
            async: true,
            cache: false,
            processData: false,
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            success: function (res) {
                if (res.status) {
                    RenderChart(res.data.allDataByLotId);
                } else {
                    Func.ShowErrorMessageForm(res.message);
                }
            },
            error: function (res) {
                Func.ShowErrorMessageForm(res);
            }
        });
    }
}

function SetCookies() {
    if (document.getElementById('is_edit_flg') == null) {
        let expires = new Date();
        expires.setFullYear(expires.getFullYear() + 1);

        let cookiesFactoryId = document.getElementById('cookies_FactoryId').value;
        let factoryId = document.getElementById('FactoryId').value;
        document.cookie = cookiesFactoryId + '=' + factoryId + '; expires=' + expires + '; path=/';

        let cookiesLotId = document.getElementById('cookies_LotId').value;
        let lotId = document.getElementById('LotId').value;
        document.cookie = cookiesLotId + '=' + lotId + '; expires=' + expires + '; path=/';

        let cookiesIsInUse = document.getElementById('cookies_IsInUse').value;
        let isInUse = document.getElementById('IsInUse').checked;
        document.cookie = cookiesIsInUse + '=' + isInUse + '; expires=' + expires + '; path=/';
    }
}

$(document).ready(function () {
    GetDataByLotId();    
    //var s02002Hub = $.connection.s02002Hub;
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var user = "S02002";

    //s02002Hub.client.SendAsync = function (message) {
    //    if (message && message !== '') {
    //        const jsonObj = JSON.parse(message);

    //        if (jsonObj && jsonObj.type === '1') {
    //            if (chart_changeMode == 0) {
    //                GetDataByLotId(true);
    //            } else if (chart_changeMode == 1) {
    //                GetDataByLastDay(true);
    //            }
    //        }
    //    }
    //};
    //$.connection.hub.start().done(function () {
    //    //なし
    //});

    // Adds to the connection object a handler that receives messages from the hub
    connection.on("ReceiveMessage", function (user, message) {
        try {
            console.log(`${user} says ${message}`);
            const jsonObj = JSON.parse(message);

            if (jsonObj && jsonObj.type === '1') {
                if (chart_changeMode == 0) {
                    GetDataByLotId(true);
                } else if (chart_changeMode == 1) {
                    GetDataByLastDay(true);
                }
            }
        } catch (e) {
            console.error(e.message);
        }
    });

    // starts a connection
    connection.start().then(function () {
        console.log("connected to hub!");
    }).catch(function (err) {
        return console.error(err.toString());
    });
});




