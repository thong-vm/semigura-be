var arrayChart = [];

function LoadDataByFactoryId() {
    // 選択しているファクトリーを取得
    let factoryId = document.getElementById('FactoryId').value;
    // サーバーのデータを取得
    Func.ajax({
        type: 'POST',
        url: '/S02003/LoadLocationByFactoryId',
        data: 'factoryId=' + factoryId,
        async: true,
        cache: false,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {
                LocationDataRender(res.data);
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
 * LocationDataRender
 * Get Location by FactoryId, render data in select element
 */
function LocationDataRender(data) {
    let html = '';
    if (data && data.length > 0) {
        html += '<option value=""></option>';
        for (let i = 0; i < data.length; i++) {
            // ロケーション選択の描画            
            html += '<option value="' + data[i].id + '">' + data[i].name + '</option>';
        }
    }
    document.getElementById('LocationId').innerHTML = html;
    GetDataByLocationId();
}

/*
 * GetDataByLocationId
 * Get All Location Info by LocationId
 */
function GetDataByLocationId(isRefresh) {
    // 選択しているロケーションを取得
    let factoryId = document.getElementById('FactoryId').value;
    let locationId = document.getElementById('LocationId').value;
    if (factoryId != "" && locationId != "") {
        document.getElementById('display_data').style.display = "flex";
        let params = {};
        params["FactoryId"] = factoryId;
        params["LocationId"] = locationId;
        let data = Func.ConcatParamOfForm(params);

        for (let k = 0; k < arrayChart.length; k++) {
            if (arrayChart[k].chart.canvas != null) {
                arrayChart[k].destroy();
            }
        }

        arrayChart = [];

        // サーバーのデータを取得
        if (isRefresh) {
            UtilSpinnerRefresh.Show();
        } else {
            UtilSpinner.Show();
        }
        Func.ajax({
            type: 'GET',
            url: '/S02003/GetDataByLocationId',
            data: data,
            async: true,
            cache: false,
            processData: false,
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            success: function (res) {
                if (res.status) {                    
                    DisplayDataRender(res.data.locationInfoList);
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
        document.getElementById('display_data').style.display = "none";
    }

    SetCookies();
}

/*
 * GetDataByLocationId
 * Get All Data by LocationId and render
 */
function DisplayDataRender(data) {
    let html = '';
    let room_temp = document.getElementById('room_temp').value;
    let room_humi = document.getElementById('room_humi').value;
    let temp_icon = document.getElementById('temp_icon').value;
    let humi_icon = document.getElementById('humi_icon').value;
    let allDate = document.getElementById('allLocation').value;
    let oneDate = document.getElementById('oneDate').value;
    let searchLabel = document.getElementById('searchLabel').value;

    var displayData = new Array();    
    if (data && data.length > 0) {
        for (let i = 0; i < data.length; i++) {            
            if (data[i].locationName != null) {
                let params = {};
                params["locationId"] = data[i].locationId;
                params["locationName"] = data[i].locationName;
                params["locationTemp"] = data[i].locationTemp;
                params["locationHumi"] = data[i].locationHumi;
                params["listMeasureUnixTimeStamp"] = data[i].listMeasureUnixTimeStamp;
                displayData.push(params);
                // ロケーション選択の描画 

                let className = "col-xl-6 col-lg-12 col-md-12 col-sm-12 room_content";
                if (data.length == 1) {
                    className = "col-xl-12 col-lg-12 col-md-12 col-sm-12 room_content";
                }

                let newIdx = data[i].locationTemp.length - 1;
                html += '<div class=' + className + ' > ';
                html += '            <input type="hidden" id="change_mode_' + data[i].locationId + '" class="change_mode" data-index="' + i + '" data-val="allDay" data-location-id="' + data[i].locationId + '"/>';
                html += '            <div class="row sum_content"> ';
                html += '                <div class="w-40 content_block" > ';
                html += '                <h6 style="position: absolute;font-weight:bold;text-align: center;color: blue;top: -8px;width: 160px;">' + data[i].locationName +'</h6> ';
                html += '                    <div class="content"> ';
                html += '                        <div class="content_title"> ';
                html += '                            <p>' + room_temp + '</p> ';
                html += '                        </div> ';
                html += '                        <div class="content_desc"> ';
                let locationTemp = data[i].locationTemp[newIdx] ? data[i].locationTemp[newIdx] : "";
                html += '                            <p>' + locationTemp + '°C</p> ';
                html += '                        </div> ';
                html += '                        <div class="content_icon"> ';
                html += '                            <img src="' + temp_icon + '" alt="temp.png" /> ';
                html += '                        </div> ';
                html += '                    </div> ';
                html += '                </div> ';
                html += '                <div class="w-40 content_block" > ';
                html += '                    <div class="content"> ';
                html += '                        <div class="content_title"> ';
                html += '                            <p>' + room_humi + '</p> ';
                html += '                        </div> ';
                html += '                        <div class="content_desc"> ';
                let locationHumi = data[i].locationHumi[newIdx] ? data[i].locationHumi[newIdx] : "";
                html += '                            <p>' + locationHumi + '%</p> ';
                html += '                        </div> ';
                html += '                        <div class="content_icon"> ';
                html += '                            <img src="' + humi_icon + '" alt="temp.png" /> ';
                html += '                        </div> ';
                html += '                    </div> ';
                html += '                </div> ';
                html += '            </div> ';
                let locationId = data[i].locationId;
                let modeA = "allDay";
                let modeB = "oneDay";
                let modeC = "searchDay";
                let searchDayId = "S02003_search_date" + i;
                html += '            <div class="chart_content" > ';
                html += '               <div class="row searchBlock">';
                html += '                    <button class="btn btn-success search_button" onclick="GetDataByRoomName(\'' + locationId + '\' ' + ',' + '\'' + modeA + '\'' + ',' + '\'' + searchDayId + '\')">';
                html += '                        ' + allDate;
                html += '                    </button>';
                html += '                    <button  class="btn btn-success search_button" onclick="GetDataByRoomName(\'' + locationId + '\' ' + ',' + '\'' + modeB + '\'' + ',' + '\'' + searchDayId + '\')">';
                html += '                        ' + oneDate;
                html += '                    </button>';
                html += '                    <input id="' + searchDayId + '" type="date" class="search_date"';
                html += '                            placeholder="yyyy/MM/dd" />';
                html += '                    <button class="btn btn-warning " onclick="GetDataByRoomName(\'' + locationId + '\' ' + ',' + '\'' + modeC + '\'' + ',' + '\'' + searchDayId + '\')">';
                html += '                        <i class="bi bi-search"></i> '/* + searchLabel*/;
                html += '                    </button>';
                html += '                </div>';
                html += '                <div class="chart_display" >';
                html += '                    <canvas id="' + locationId + '"></canvas> ';
                html += '                </div>';
                html += '            </div> ';
                html += '        </div> ';
            }
        }
    }    
    document.getElementById('display_data').innerHTML = html;
    DisplayChart(displayData);
}

//var changeMode = 0;
function GetDataByRoomName(locationId, mode, id, isRefresh) {
    let searchDate = document.getElementById(id).value;
    let params = {};
    params["LocationId"] = locationId;
    params["SearchMode"] = mode;
    params["SearchDate"] = searchDate;
    let data = Func.ConcatParamOfForm(params);

    if (mode == "oneDay") {
        document.getElementById('change_mode_' + locationId).setAttribute('data-val', mode);
        //changeMode = 1;
        document.getElementById(id).value = '';
    } else if (mode == "searchDay") {
        document.getElementById('change_mode_' + locationId).setAttribute('data-val', mode);
        //changeMode = 2;
    } else {
        document.getElementById('change_mode_' + locationId).setAttribute('data-val', mode);
        //changeMode = 0;
        document.getElementById(id).value = '';
    }

    // サーバーのデータを取得
    if (isRefresh) {
        UtilSpinnerRefresh.Show();
    } else {
        UtilSpinner.Show();
    }

    Func.ajax({
        type: 'GET',
        url: '/S02003/GetDataByRoomName',
        data: data,
        async: true,
        cache: false,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {
                if (res.data.locationInfoList && res.data.locationInfoList[0].locationId) {
                    DisplayChartByRoomName(res.data.locationInfoList);
                } 
                
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

/*
 * Render data to Chart by LocationId
 */
function DisplayChart(data) {    
    let chart_temp = document.getElementById('chart_temp').value;
    let chart_humi = document.getElementById('chart_humi').value;
    if (data && data.length > 0) {
        for (let i = 0; i < data.length; i++) {
            let ctxL1 = document.getElementById(data[i].locationId).getContext('2d');
            let datetimeX = new Array();
            for (let j = 0; j < data[i].listMeasureUnixTimeStamp.length; j++) {
                if (data[i].listMeasureUnixTimeStamp[j] != null) {
                    let date = moment(data[i].listMeasureUnixTimeStamp[j]).format("YY/MM/DD HH:mm").toUpperCase();
                    datetimeX.push(date);
                }                
            }

            for (let k = 0; k < arrayChart.length; k++) {
                if (arrayChart[k].chart.canvas != null) {
                    if (arrayChart[k].chart.canvas.id == data[i].locationId) {
                        arrayChart[k].destroy();
                    }
                }
            }

            arrayChart[i] = new Chart(ctxL1, {
                type: 'line',
                data: {
                    labels: datetimeX,
                    datasets: [{
                        type: 'line',
                        label: chart_temp,
                        yAxisID: 'A',
                        fill: false,
                        borderColor: "#FF3333",
                        data: data[i].locationTemp
                    }, {
                        type: 'line',
                        label: chart_humi,
                        yAxisID: 'A',
                        fill: false,
                        borderColor: "#000080",
                        data: data[i].locationHumi
                    }]
                },
                options: {
                    maintainAspectRatio: false,
                    animation: {
                        duration: 0
                    },
                    interaction: {
                        intersect: false,
                    },
                    scales: {
                        xAxes: [{
                            id: 'xAxis1',
                            type: 'category',
                            ticks: {
                                callback: function (label) {
                                    return label;
                                }
                            }
                        }],
                        yAxes: [{
                            id: 'A',
                            type: 'linear',
                            position: 'left',
                            scaleLabel: {
                                display: true,
                                labelString: ''
                            }
                        }]
                    }
                }
            });
        }
    }    
}

/*
 * Render data to Chart by LocationId
 */

function DisplayChartByRoomName(data) {
    let chart_temp = document.getElementById('chart_temp').value;
    let chart_humi = document.getElementById('chart_humi').value;
    if (data && data.length > 0) {
        for (let i = 0; i < data.length; i++) {
            let ctxL1 = document.getElementById(data[i].locationId).getContext('2d');
            let datetimeX = new Array();
            for (let j = 0; j < data[i].listMeasureUnixTimeStamp.length; j++) {
                if (data[i].listMeasureUnixTimeStamp[j]) {
                    let date = moment(data[i].listMeasureUnixTimeStamp[j]).format("YY/MM/DD HH:mm").toUpperCase();
                    datetimeX.push(date);
                }
            }
            for (let k = 0; k < arrayChart.length; k++) {
                if (arrayChart[k].chart.canvas != null) {
                    if (arrayChart[k].chart.canvas.id == data[i].locationId) {
                        arrayChart[k].destroy();
                    }
                }                
            }
            
            arrayChart[i] = new Chart(ctxL1, {
                type: 'line',
                data: {
                    labels: datetimeX,
                    datasets: [{
                        type: 'line',
                        label: chart_temp,
                        yAxisID: 'A',
                        fill: false,
                        borderColor: "#FF3333",
                        data: data[i].locationTemp
                    }, {
                        type: 'line',
                        label: chart_humi,
                        yAxisID: 'A',
                        fill: false,
                        borderColor: "#000080",
                        data: data[i].locationHumi
                    }]
                },
                options: {
                    maintainAspectRatio: false,
                    animation: {
                        duration: 0
                    },
                    interaction: {
                        intersect: false,
                    },
                    scales: {
                        xAxes: [{
                            id: 'xAxis1',
                            type: 'category',
                            ticks: {
                                callback: function (label) {
                                    return label;
                                }
                            }
                        }],
                        yAxes: [{
                            id: 'A',
                            type: 'linear',
                            position: 'left',
                            scaleLabel: {
                                display: true,
                                labelString: ''
                            }
                        }]
                    }
                }
            });
            arrayChart.push(arrayChart[i]);
        }
    }
}

function SetCookies() {
    if (document.getElementById('is_edit_flg') == null) {
        let expires = new Date();
        expires.setFullYear(expires.getFullYear() + 1);

        let cookiesFactoryId = document.getElementById('cookies_FactoryId').value;
        let factoryId = document.getElementById('FactoryId').value;
        document.cookie = cookiesFactoryId + '=' + factoryId + '; expires=' + expires + '; path=/';

        let cookiesLocationId = document.getElementById('cookies_LocationId').value;
        let locationId = document.getElementById('LocationId').value;
        document.cookie = cookiesLocationId + '=' + locationId + '; expires=' + expires + '; path=/';
    }
}

function CallRefreshData() {
    const items = document.getElementsByClassName('change_mode');

    for (let i = 0; i < items.length; i++) {
        const index = items[i].getAttribute('data-index');
        const locationId = items[i].getAttribute('data-location-id');
        const searchMode = items[i].getAttribute('data-val');

        let searchDayId = "S02003_search_date" + index;

        if (searchMode === 'allDay' || searchMode === 'oneDay') {
            GetDataByRoomName(locationId, searchMode, searchDayId, true);
        }
    }
}

$(document).ready(function () {
    GetDataByLocationId();

    //var s02003Hub = $.connection.s02003Hub;
    //s02003Hub.client.SendAsync = function (message) {
    //    if (message && message !== '') {
    //        const jsonObj = JSON.parse(message);
    //        if (jsonObj && jsonObj.type === '1') {
    //            CallRefreshData();
    //        }
    //    }
    //};

    //$.connection.hub.start().done(function () {
    //    //なし
    //});

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var user = "S02003";

    // Adds to the connection object a handler that receives messages from the hub
    connection.on("ReceiveMessage", function (user, message) {
        try {
            console.log(`${user} says ${message}`);
            const jsonObj = JSON.parse(message);
            if (jsonObj && jsonObj.type === '1') {
                CallRefreshData();
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