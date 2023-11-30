


function LoadData(isRefresh) {
    console.log("LoadData");

    // 選択しているファクトリーを取得
    var factoryId = document.getElementById('FactoryId').value;
    // サーバーのデータを取得
    if (isRefresh) {
        UtilSpinnerRefresh.Show();
    } else {
        UtilSpinner.Show();
    }
    Func.ajax({
        type: 'POST',
        url: '/S01002/LoadData',
        data: 'factoryId=' + factoryId,
        async: true,
        cache: false,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {
                DrawMoromi(null);
                DrawSegiku(null);

                if (res.data) {
                    for (let i = 0; i < res.data.length; i++) {
                        if (res.data[i].containerType == 1) {
                            DrawMoromi(res.data[i].locationList);
                        } else {
                            DrawSegiku(res.data[i].locationList);
                        }
                    }
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
        },
    }, true);

    SetCookies();
}

const locationLabel = document.getElementById('location_label').value;
const theah1 = document.getElementById('table_th-1').value;
const theah2 = document.getElementById('table_th-2').value;
const theah3 = document.getElementById('table_th-3').value;

function DrawSegiku(data) {
    let html = '';
    if (data && data.length > 0) {
        for (let i = 0; i < data.length; i++) {
            var locationInfo = data[i];
            // ロケーションの描画
            html += '<div class="row justify-content-center process_room ">';
            html += '    <h6 class="select_title">' + locationLabel+':</h6>';
            html += '    <div>';
            html += '        <select class="select-classic form-control" disabled>';
            html += '            <option>' + locationInfo.locationName + '</option>';
            html += '        </select>';
            html += '    </div>';
            html += '</div>';

            // テーブルの描画
            // colspanMax
            var sensorCntArr = new Array(0);
            var colspanMax = 0;            
            for (let i = 0; i < locationInfo.lotList.length; i++) {
                let sensorCnt = 0;
                for (let j = 0; j < locationInfo.lotList[i].tankList.length; j++) {
                    for (let k = 0; k < locationInfo.lotList[i].tankList[j].terminalList.length; k++) {
                        if (locationInfo.lotList[i].tankList[j].terminalList[k].sensorDataList[0].sensorName != null) {
                            sensorCnt = sensorCnt + 2;
                        }                        
                    }
                }
                sensorCntArr[i] = sensorCnt;
            }

            colspanMax = sensorCntArr[sensorCntArr.sort().length -1];

            html += '<hr width="80%" align="center" color="#808080" />';
            html += '<div class=" row justify-content-center tableL" style="overflow-x:auto;">';
            html += '    <table class="table table-bordered tankTb" style="width:100%">';
            html += '        <thead>';
            html += '            <tr>';
            html += '                <th>' + theah1 + ' </th > ';
            html += '                <th colspan="' + colspanMax +'">' + theah3 + '</th > ';
            html += '            </tr>';
            html += '        </thead>';
            html += '        <tbody>';

            for (let i = 0; i < locationInfo.lotList.length; i++) {
                html += '            <tr>';
                html += '                <td rowspan="2">' + locationInfo.lotList[i].lotCode + '</td > ';
                for (let j = 0; j < locationInfo.lotList[i].tankList.length; j++) {
                    for (let k = 0; k < locationInfo.lotList[i].tankList[j].terminalList.length; k++) {
                        if (locationInfo.lotList[i].tankList[j].terminalList != null &&
                            locationInfo.lotList[i].tankList[j].terminalList[k].sensorDataList != null &&
                            locationInfo.lotList[i].tankList[j].terminalList[k].sensorDataList[0].sensorName != null) {
                            html += '                <td>' + locationInfo.lotList[i].tankList[j].terminalList[k].sensorDataList[0].sensorName + '(Ch.1)</td > ';
                            html += '                <td>' + locationInfo.lotList[i].tankList[j].terminalList[k].sensorDataList[0].sensorName + '(Ch.2)</td > ';
                        } else {
                            html += '                <td></td > ';
                        }
                    }
                }
                html += '            </tr>';
                html += '            <tr>';
                for (let j = 0; j < locationInfo.lotList[i].tankList.length; j++) {
                    for (let k = 0; k < locationInfo.lotList[i].tankList[j].terminalList.length; k++) {
                        if (locationInfo.lotList[i].tankList[j].terminalList != null &&
                            locationInfo.lotList[i].tankList[j].terminalList[k].sensorDataList != null &&
                            locationInfo.lotList[i].tankList[j].terminalList[k].sensorDataList[0].sensorName != null) {
                            if (locationInfo.lotList[i].tankList[j].terminalList[k].sensorDataList[0].sensorTemp != null) {
                                html += '                <td>' + locationInfo.lotList[i].tankList[j].terminalList[k].sensorDataList[0].sensorTemp + '°C</td > ';
                            } else {
                                html += '                <td> - </td > ';
                            }
                            if (locationInfo.lotList[i].tankList[j].terminalList[k].sensorDataList[0].sensorTemp2 != null) {
                                html += '                <td>' + locationInfo.lotList[i].tankList[j].terminalList[k].sensorDataList[0].sensorTemp2 + '°C</td > ';
                            } else {
                                html += '                <td> - </td > ';
                            }
                        } else {
                            html += '                <td> </td > ';
                        }
                    }
                }                
                html += '            </tr>';
            }

            html += '        </tbody>';
            html += '    </table>';
            html += '</div>';
        }
    } 
    document.getElementById('seigiku-content').innerHTML = html;
}

function DrawMoromi(data) {
    let html = '';    
    let imageUrl = document.getElementById('image_url').value;
    if (data && data.length > 0) {
        for (let a = 0; a < data.length; a++) {
            var locationInfo = data[a];
            // ロケーションの描画
            html += '<div class="row justify-content-center process_room ">';
            html += '    <h6 class="select_title">' + locationLabel + ':</h6>';
            html += '    <div>';
            html += '        <select class="select-classic form-control" disabled>';
            html += '            <option>' + locationInfo.locationName + '</option>';
            html += '        </select>';
            html += '    </div>';
            html += '</div>';

            // テーブルの描画  
            html += '<hr width="70%" align="center" color="#808080" />';
            html += '<div class=" row justify-content-center tableL" style="overflow-x:auto;">';
            html += '    <table class="table table-bordered tankTb" style="width:100%">';
            html += '        <thead>';
            html += '            <tr>';
            html += '                <th style="width:16%;">' + theah1 + ' </th> ';
            html += '                <th style="width:16%;">' + theah2 + ' </th> ';
            html += '                <th colspan="' + 3 + '" style="width:68%;">' + theah3 + '</th> ';
            html += '            </tr>';
            html += '        </thead>';
            html += '        <tbody>';            
            for (let i = 0; i < locationInfo.lotList.length; i++) {
                if (locationInfo.lotList[i].tankList.length == 0 || locationInfo.lotList[i].tankList[0] == null) {
                    html += '            <tr>';
                    html += '                <td rowspan="3">' + locationInfo.lotList[i].lotCode + '</td> ';
                    html += '                <td rowspan="2"></td > ';
                    html += '                <td>-</td> ';
                    html += '                <td>-</td> ';
                    html += '                <td>-</td> ';
                    html += '            </tr>';
                    html += '            <tr>';
                    html += '                <td>-</td> ';
                    html += '                <td>-</td> ';
                    html += '                <td>-</td> ';
                    html += '            </tr>';
                    html += '            <tr>';
                    html += '                <td colspan="4">';
                    html += '                </td>';
                    html += '            </tr>';
                } else {
                    html += '            <tr>';
                    html += '                <td rowspan="' + (locationInfo.lotList[i].tankList.length) * 3 + '">' + locationInfo.lotList[i].lotCode + '</td > ';
                    html += '                <td rowspan="2">' + locationInfo.lotList[i].tankList[0].tankCode + '</td> ';
                    if (locationInfo.lotList[i].tankList[0].terminalList.length == 0
                        || locationInfo.lotList[i].tankList[0].terminalList[0] == null
                        || locationInfo.lotList[i].tankList[0].terminalList[0].sensorDataList[0] == null) {
                        html += '                <td>-</td> ';
                        html += '                <td>-</td> ';
                        html += '                <td>-</td> ';
                        html += '            </tr>';
                        html += '            <tr>';
                        html += '                <td>-</td> ';
                        html += '                <td>-</td> ';
                        html += '                <td>-</td> ';
                        html += '            </tr>';
                    } else {
                        let sensorName = locationInfo.lotList[i].tankList[0].terminalList[0].sensorDataList[0].sensorName;
                        let sensorName0 = sensorName != null ? sensorName + '(Ch.1)' : '-';
                        let sensorName1 = sensorName != null ? sensorName + '(Ch.2)' : '-';
                        let sensorName2 = sensorName != null ? sensorName + '(Ch.3)' : '-';
                        html += '                <td>' + sensorName0 + '</td> ';
                        html += '                <td>' + sensorName1 + '</td> ';
                        html += '                <td>' + sensorName2 + '</td> ';
                        html += '            </tr>';
                        html += '            <tr>';
                        let sensorTemp0 = locationInfo.lotList[i].tankList[0].terminalList[0].sensorDataList[0].sensorTemp;
                        sensorTemp0 = sensorTemp0 != null ? sensorTemp0 + '°C' : '-';
                        let sensorTemp1 = locationInfo.lotList[i].tankList[0].terminalList[0].sensorDataList[0].sensorTemp2;
                        sensorTemp1 = sensorTemp1 != null ? sensorTemp1 + '°C' : '-';
                        let sensorTemp2 = locationInfo.lotList[i].tankList[0].terminalList[0].sensorDataList[0].sensorTemp3;
                        sensorTemp2 = sensorTemp2 != null ? sensorTemp2 + '°C' : '-';
                        html += '                <td>' + sensorTemp0 + '</td> ';
                        html += '                <td>' + sensorTemp1 + '</td> ';
                        html += '                <td>' + sensorTemp2 + '</td> ';
                        html += '            </tr>';
                        html += '            <tr style="visibility: collapse;">';
                    }
                    
                    html += '                <td colspan="4">';
                    if (locationInfo.lotList[i].tankList[0].imageList.length > 0 && locationInfo.lotList[i].tankList[0].imageList[0] != null) {
                        html += '<div id="carouselExampleIndicators' + a + i + 0 + '" class="carousel slide" data-ride="carousel"> ';
                        html += '  <ol class="carousel-indicators"> ';
                        html += '    <li data-target="#carouselExampleIndicators' + a + i + 0 + '" data-slide-to="0" class="active"></li> ';
                        for (let image = 1; image < locationInfo.lotList[i].tankList[0].imageList.length; image++) {
                            html += '    <li data-target="#carouselExampleIndicators' + a + i + 0 + '" data-slide-to="' + image + '"></li> ';
                        }
                        html += '  </ol> ';
                        html += '  <div class="carousel-inner"> ';
                        html += '    <div class="carousel-item active"> ';
                        html += '      <img class="d-block w-100 img_scale" src="' + imageUrl + '/' + locationInfo.lotList[i].tankList[0].imageList[0].id + '?width=800" alt="First slide"> ';
                        let svDate = locationInfo.lotList[i].tankList[0].imageList[0].createdOnUnixTimeStamp;
                        let dispCaption = moment(svDate).format('YYYY/MM/DD HH:mm');
                        html += '      <p class="caption_byTime">' + dispCaption + '</p> ';
                        html += '    </div> ';
                        for (let image = 1; image < locationInfo.lotList[i].tankList[0].imageList.length; image++) {
                            html += '    <div class="carousel-item"> ';
                            html += '      <img class="d-block w-100 img_scale" src="' + imageUrl + '/' + locationInfo.lotList[i].tankList[0].imageList[image].id + '?width=800" > ';
                            let svDate = locationInfo.lotList[i].tankList[0].imageList[image].createdOnUnixTimeStamp;
                            let dispCaption = moment(svDate).format('YYYY/MM/DD HH:mm');
                            html += '      <p class="caption_byTime">' + dispCaption + '</p> ';
                            html += '    </div> ';
                        }
                        html += '  </div> ';
                        html += '  <a class="carousel-control-prev" href="#carouselExampleIndicators' + a + i + 0 + '" role="button" data-slide="prev"> ';
                        html += '    <span class="carousel-control-prev-icon" aria-hidden="true">❮</span> ';
                        html += '    <span class="sr-only">Previous</span> ';
                        html += '  </a> ';
                        html += '  <a class="carousel-control-next" href="#carouselExampleIndicators' + a + i + 0 + '" role="button" data-slide="next"> ';
                        html += '    <span class="carousel-control-next-icon" aria-hidden="true">❯</span> ';
                        html += '    <span class="sr-only">Next</span> ';
                        html += '  </a> ';
                        html += '</div> ';
                    }
                    html += '                </td>';
                    html += '            </tr>';
                }                

                for (let j = 1; j < locationInfo.lotList[i].tankList.length; j++) {
                    if (locationInfo.lotList[i].tankList[j] == null) {
                        html += '            <tr>';                        
                        html += '                <td rowspan="2"></td > ';
                        html += '                <td>-</td> ';
                        html += '                <td>-</td> ';
                        html += '                <td>-</td> ';
                        html += '            </tr>';
                        html += '            <tr>';
                        html += '                <td>-</td> ';
                        html += '                <td>-</td> ';
                        html += '                <td>-</td> ';
                        html += '            </tr>';
                        html += '            <tr>';
                        html += '                <td colspan="4">';
                        html += '                </td>';
                        html += '            </tr>';
                    } else {
                        html += '            <tr>';
                        html += '                <td rowspan="2">' + locationInfo.lotList[i].tankList[j].tankCode + '</td > ';
                        if (locationInfo.lotList[i].tankList[j].terminalList.length == 0
                            || locationInfo.lotList[i].tankList[j].terminalList[0] == null
                            || locationInfo.lotList[i].tankList[j].terminalList[0].sensorDataList[0] == null) {                            
                            html += '                <td>-</td> ';
                            html += '                <td>-</td> ';
                            html += '                <td>-</td> ';
                            html += '            </tr>';
                            html += '            <tr>';
                            html += '                <td>-</td> ';
                            html += '                <td>-</td> ';
                            html += '                <td>-</td> ';
                            html += '            </tr>';
                        } else {
                            let sensorName = locationInfo.lotList[i].tankList[j].terminalList[0].sensorDataList[0].sensorName;
                            let sensorName0 = sensorName != null ? sensorName + '(Ch.1)' : '-';
                            let sensorName1 = sensorName != null ? sensorName + '(Ch.2)' : '-';
                            let sensorName2 = sensorName != null ? sensorName + '(Ch.3)' : '-';
                            html += '                <td>' + sensorName0 + '</td> ';
                            html += '                <td>' + sensorName1 + '</td> ';
                            html += '                <td>' + sensorName2 + '</td> ';
                            html += '            </tr>';
                            html += '            <tr>';
                            let sensorTemp0 = locationInfo.lotList[i].tankList[j].terminalList[0].sensorDataList[0].sensorTemp;
                            sensorTemp0 = sensorTemp0 != null ? sensorTemp0 + '°C' : '-';
                            let sensorTemp1 = locationInfo.lotList[i].tankList[j].terminalList[0].sensorDataList[0].sensorTemp2;
                            sensorTemp1 = sensorTemp1 != null ? sensorTemp1 + '°C' : '-';
                            let sensorTemp2 = locationInfo.lotList[i].tankList[j].terminalList[0].sensorDataList[0].sensorTemp3;
                            sensorTemp2 = sensorTemp2 != null ? sensorTemp2 + '°C' : '-';
                            html += '                <td>' + sensorTemp0 + '</td> ';
                            html += '                <td>' + sensorTemp1 + '</td> ';
                            html += '                <td>' + sensorTemp2 + '</td> ';
                            html += '            </tr>';
                        }
                        
                        html += '            <tr style="visibility: collapse;">';
                        html += '                <td colspan="4">';
                        if (locationInfo.lotList[i].tankList[j].imageList.length > 0 && locationInfo.lotList[i].tankList[j].imageList[0] != null) {
                            html += '<div id="carouselExampleIndicators' + a + i + j + '" class="carousel slide" data-ride="carousel"> ';
                            html += '  <ol class="carousel-indicators"> ';
                            html += '    <li data-target="#carouselExampleIndicators' + a + i + j + '" data-slide-to="0" class="active"></li> ';
                            for (let image = 1; image < locationInfo.lotList[i].tankList[j].imageList.length; image++) {
                                html += '    <li data-target="#carouselExampleIndicators' + a + i + j + '" data-slide-to="' + image + '"></li> ';
                            }
                            html += '  </ol> ';
                            html += '  <div class="carousel-inner"> ';
                            html += '    <div class="carousel-item active"> ';
                            html += '      <img class="d-block w-100 img_scale" src="' + imageUrl + '/' + locationInfo.lotList[i].tankList[j].imageList[0].id + '?width=800" alt="First slide"> ';
                            let svDate = locationInfo.lotList[i].tankList[j].imageList[0].createdOnUnixTimeStamp;
                            let dispCaption = moment(svDate).format('YYYY/MM/DD HH:mm');
                            html += '      <p class="caption_byTime">' + dispCaption + '</p> ';
                            html += '    </div> ';
                            for (let image = 1; image < locationInfo.lotList[i].tankList[j].imageList.length; image++) {
                                html += '    <div class="carousel-item"> ';
                                html += '      <img class="d-block w-100 img_scale" src="' + imageUrl + '/' + locationInfo.lotList[i].tankList[j].imageList[image].id + '?width=800" alt="slide"> ';
                                let svDate = locationInfo.lotList[i].tankList[j].imageList[image].createdOnUnixTimeStamp;
                                let dispCaption = moment(svDate).format('YYYY/MM/DD HH:mm');
                                html += '      <p class="caption_byTime">' + dispCaption + '</p> ';
                                html += '    </div> ';
                            }
                            html += '  </div> ';
                            html += '  <a class="carousel-control-prev" href="#carouselExampleIndicators' + a + i + j + '" role="button" data-slide="prev"> ';
                            html += '    <span class="carousel-control-prev-icon" aria-hidden="true">❮</span> ';
                            html += '    <span class="sr-only">Previous</span> ';
                            html += '  </a> ';
                            html += '  <a class="carousel-control-next" href="#carouselExampleIndicators' + a + i + j + '" role="button" data-slide="next"> ';
                            html += '    <span class="carousel-control-next-icon" aria-hidden="true">❯</span> ';
                            html += '    <span class="sr-only">Next</span> ';
                            html += '  </a> ';
                            html += '</div> ';
                        }
                        html += '                </td>';
                        html += '            </tr>';
                    }                    
                }
            }

            html += '        </tbody>';
            html += '    </table>';
            html += '</div>';
        }
    }
    document.getElementById('moromi-content').innerHTML = html;
}

function SetCookies() {
    if (document.getElementById('is_edit_flg') == null) {
        let expires = new Date();
        expires.setFullYear(expires.getFullYear() + 1);

        let cookiesFactoryId = document.getElementById('cookies_FactoryId').value;
        let factoryId = document.getElementById('FactoryId').value;
        document.cookie = cookiesFactoryId + '=' + factoryId + '; expires=' + expires + '; path=/';
    }
}

function InitTable() {
    var table = $('.tankTb').DataTable({
        language: {
            paginate: {
                next: '<i class="bi bi-chevron-double-right"></i>',
                previous: '<i class="bi bi-chevron-double-left"></i>'
            },
            emptyTable: document.getElementById('resource_empty_table').value,
        },
        "scrollX": true,
        "sScrollXInner": "80%",
        "scrollY": "300px",
        "scrollCollapse": true,
        "searching": true,
        "bLengthChange": false,
        "bSort": true,
        "bInfo": false,
        "bPaginate": true,
        "stripeClasses": ['stripe-1'],
        "createdRow": function (row, data, index) {
            $('td', row).css({
                'border': '1px solid white'
            });
        },
    });
    table.columns.adjust().draw();

    // エラー発生時にデフォルトで表示されるメッセージボックスを無効化する。
    $.fn.dataTable.ext.errMode = 'none';

    // errorイベントを受け取るハンドラ（エラーレスポンスの取得はできない。）
    table.on('error.dt', function (e, settings, techNote, message) {
        Func.ErrorHandle(message);
        Func.CheckSessionExpired();
    });
}


$(document).ready(function () {
    InitTable();
    LoadData();

    //var s01002Hub = $.connection.s01002Hub;
    //s01002Hub.client.SendAsync = function (message) {
    //    if (message && message !== '') {
    //        const jsonObj = JSON.parse(message);

    //        if (jsonObj && jsonObj.type === '1') {
    //            LoadData(true);
    //        }
    //    }
    //};

    //$.connection.hub.start().done(function () {
    //    //なし
    //});

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var user = "S01002";

    // Adds to the connection object a handler that receives messages from the hub
    connection.on("ReceiveMessage", function (user, message) {
        try {
            console.log(`${user} says ${message}`);
            const jsonObj = JSON.parse(message);
            if (jsonObj && jsonObj.type === '1') {
                LoadData(true);
            }
        } catch (e) {
            console.error(e.message);
        }
    });

    // starts a connection
    connection.start().then(function () {
        console.log("connected to hub!");
        //sends messages to the hub.
        //var message = "Hello!"
        //connection.invoke("SendMessage", user, message).catch(function (err) {
        //    return console.error(err.toString());
        //});
    }).catch(function (err) {
        return console.error(err.toString());
    });
});