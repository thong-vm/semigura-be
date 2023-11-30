var _SlideIndex = 1;
var _DeviceCode = ''
var _ListTableColumnData = [];

/*↓LuanLV Hide Media at 2022/08/30↓*/
function ChangViewMode(mode) {
    if (mode === 1) {
        let src = '';
        if (_DeviceCode && _DeviceCode != '') {
            src = 'api/mjpeg/live?DeviceId=' + _DeviceCode + '&rand=' + new Date().getTime();
            document.getElementById('live-frame').getElementsByTagName('img')[0].setAttribute('src', src);
            document.getElementById('live-frame').style.display = "block";
        }

        document.getElementById('img-frame').style.display = "none";
    } else {
        document.getElementById('live-frame').getElementsByTagName('img')[0].setAttribute('src', '');

        document.getElementById('img-frame').style.display = "block";
        document.getElementById('live-frame').style.display = "none";
    }
}
/*↑LuanLV Hide Media at 2022/08/30↑*/

function plusSlides(n) {
    ShowSlides(_SlideIndex += n);
}

function currentSlide(n) {
    ShowSlides(_SlideIndex = n);
}

function ShowSlides(n) {
    var i;
    var slides = document.getElementsByClassName("mySlides");
    var dots = document.getElementsByClassName("demo");
    var captionText = document.getElementById("caption");
    if (n > slides.length) { _SlideIndex = 1 }
    if (n < 1) { _SlideIndex = slides.length }
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    for (i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace(" active", "");
    }
    slides[_SlideIndex - 1].style.display = "block";
    dots[_SlideIndex - 1].className += " active";
    captionText.innerHTML = dots[_SlideIndex - 1].alt;
}


// reload table when delete or add new data
function ReloadTable() {
    var table = $('#moromiTable').DataTable();
    table.ajax.reload();

    // エラー発生時にデフォルトで表示されるメッセージボックスを無効化する。
    $.fn.dataTable.ext.errMode = 'none';

    // errorイベントを受け取るハンドラ（エラーレスポンスの取得はできない。）
    table.on('error.dt', function (e, settings, techNote, message) {
        Func.ErrorHandle(message);
        Func.CheckSessionExpired();
    });
}



function LoadDataByFactoryId() {
    // 選択しているファクトリーを取得
    var factoryId = document.getElementById('FactoryId').value;
    let isInUse = document.getElementById('IsInUse').checked;

    let params = {};
    params["factoryId"] = factoryId;
    params["isInUse"] = isInUse;
    let data = Func.ConcatParamOfForm(params);
    // サーバーのデータを取得
    Func.ajax({
        type: 'GET',
        url: '/S02001/LoadListLotByFactoryId',
        data: data,
        async: true,
        cache: false,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {
                LotDataRender(res.data);                
                GetListTankByLotID();
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

function LotDataRender(data) {
    let html = '';
   /* html += '<option value=""></option>';*/
    if (data && data.length > 0) {
        for (let i = 0; i < data.length; i++) { 
            // ロット選択の描画
            html += '<option value="'+data[i].id+'">'+data[i].code+'</option>';
        }
    }
    document.getElementById('LotId').innerHTML = html;
}


function TankDataRender(data) {
    let html = '';
   /* html += '<option value=""></option>';*/
    if (data && data.length > 0) {
        for (let i = 0; i < data.length; i++) {
            // ロット選択の描画
            html += '<option value="' + data[i].lotContainerId + '">' + data[i].code + '</option>';
        }
    }
    document.getElementById('LotContainerId').innerHTML = html;    
    GetDataByTankId();
}

function GetListTankByLotID() {
    // 選択しているロットとロケーションを取得
    var lotId = document.getElementById('LotId').value;

    let params = {};
    params["LotId"] = lotId;
    let data = Func.ConcatParamOfForm(params);

    // サーバーのデータを取得
    Func.ajax({
        type: 'GET',
        url: '/S02001/GetListTankByLotID',
        data: data,
        async: true,
        cache: false,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {                
                TankDataRender(res.data);
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
 * GetDataByTankId
 * Get Newest Location temperature, humidity and render 
 */
function GetDataByTankId(isRefresh) {
    _DeviceCode = '';
    // 選択しているタンクを取得
    let lotContainerId = document.getElementById('LotContainerId').value;
    //countTemp = 0;
    if (lotContainerId) {
        
        // サーバーのデータを取得
        if (isRefresh) {
            UtilSpinnerRefresh.Show();
        } else {
            UtilSpinner.Show();
        }
        
        Func.ajax({
            type: 'GET',
            url: '/S02001/GetAllDataByLotContainer',
            data: 'LotContainerId=' + lotContainerId,
            async: true,
            cache: false,
            processData: false,
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            success: function (res) {
                if (res.status) {
                    $('#displayByTankId').css("display", "block");
                    RenderLastestTempData(res.data.lastestTempData);
                    _ListTableColumnData = res.data.listTableColumnData;
                    RenderListTableColumnData(res.data.listTableColumnData);
                    RenderChartData(res.data.chartData);
                    /*↓LuanLV Hide Media at 2022/08/30↓*/
                    //Vien disable media
                    //RenderMedia(res.data.ListMedia);

                     /*カメラのコード*/
                    _DeviceCode = res.data.cameraDeviceCode;
                    if (res.data.lotContainerEndDate || !_DeviceCode) {
                        document.getElementById('liveBtn').disabled  = true;
                    } else {
                        document.getElementById('liveBtn').disabled  = false;
                    }

                    if (!isRefresh) ChangViewMode(0);
                    /*↑LuanLV Hide Media at 2022/08/30↑*/
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
    }
    else {
        $('#displayByTankId').css("display", "none");
        document.getElementById('LocationId').innerHTML = "";
    }

    SetCookies();
}


function RenderLastestTempData(lastestTempData) {
    // データ入力画面へボータン
    if (lastestTempData && lastestTempData.lotEndDate != null) {
        document.getElementById('S02001_editBtn').disabled = true;
    } else {
        document.getElementById('S02001_editBtn').disabled = false;
    }

    // ロケーションの情報の表示
    document.getElementById('LocationId').innerHTML = '';
    if (lastestTempData && lastestTempData.locationName) {
        let option = '<option>' + lastestTempData.locationName + '</option>';
        document.getElementById('LocationId').innerHTML = option;
    }

    // 温度の設定
    document.getElementById('LocationTemperature').innerHTML = '';
    if (lastestTempData && lastestTempData.locationTemperature) {
        document.getElementById('LocationTemperature').innerHTML = lastestTempData.locationTemperature;
    }

    document.getElementById('LocationHumidity').innerHTML = '';
    if (lastestTempData && lastestTempData.locationHumidity) {
        document.getElementById('LocationHumidity').innerHTML = lastestTempData.locationHumidity;
    }

    document.getElementById('ProductTemperature1').innerHTML = '';
    if (lastestTempData && lastestTempData.productTemperature1) {
        document.getElementById('ProductTemperature1').innerHTML = lastestTempData.productTemperature1;
    }

    document.getElementById('ProductTemperature2').innerHTML = '';
    if (lastestTempData && lastestTempData.productTemperature2) {
        document.getElementById('ProductTemperature2').innerHTML = lastestTempData.productTemperature2;
    }

    document.getElementById('ProductTemperature3').innerHTML = '';
    if (lastestTempData && lastestTempData.productTemperature3) {
        document.getElementById('ProductTemperature3').innerHTML = lastestTempData.productTemperature3;
    }

    document.getElementById('ProductTemperatureAvg').innerHTML = '';
    if (lastestTempData && lastestTempData.productTemperatureAvg) { 
        document.getElementById('ProductTemperatureAvg').innerHTML = lastestTempData.productTemperatureAvg;
    }       

    document.getElementById('BaumeDegree').innerHTML = '';
    if (lastestTempData && lastestTempData.baumeDegree) {
        document.getElementById('BaumeDegree').innerHTML = lastestTempData.baumeDegree;
    }

    document.getElementById('AlcoholDegree').innerHTML = '';
    if (lastestTempData && lastestTempData.alcoholDegree) {
        document.getElementById('AlcoholDegree').innerHTML = lastestTempData.alcoholDegree;
    }

    document.getElementById('Acid').innerHTML = '';
    if (lastestTempData && lastestTempData.acid) {
        document.getElementById('Acid').innerHTML = lastestTempData.acid;
    }

    document.getElementById('AminoAcid').innerHTML = '';
    if (lastestTempData && lastestTempData.aminoAcid) {
        document.getElementById('AminoAcid').innerHTML = lastestTempData.aminoAcid;
    }

    const timePoint = document.getElementById('timePoint').value;
    const yearMD = document.getElementById('yearMD').value;
    document.getElementById('DataEntryMeasuareDate').innerHTML = yearMD;
    if (lastestTempData && lastestTempData.dataEntryMeasuareDateUnixTimeStamp) {
        let dispContent = moment(lastestTempData.dataEntryMeasuareDateUnixTimeStamp).format('YYYY/MM/DD HH:mm:ss ') + timePoint;
        document.getElementById('DataEntryMeasuareDate').innerHTML = dispContent;
    }
}

function RenderListTableColumnData(listTableColumnData) {
    const table = document.getElementById('displayDataTable');
    const tbody = table.getElementsByTagName('tbody')[0];
    tbody.innerHTML = '';

    if (listTableColumnData) {
        const row1 = document.createElement('tr'); row1.className = 'odd';
        const row2 = document.createElement('tr'); row2.className = 'even';
        const row3 = document.createElement('tr'); row3.className = 'odd';
        const row4 = document.createElement('tr'); row4.className = 'even';
        const row5 = document.createElement('tr'); row5.className = 'odd';
        const row6 = document.createElement('tr'); row6.className = 'even';
        const row7 = document.createElement('tr'); row7.className = 'odd';
        const row8 = document.createElement('tr'); row8.className = 'even';
        const row9 = document.createElement('tr'); row9.className = 'odd';
        const row10 = document.createElement('tr'); row10.className = 'even';
        const row11 = document.createElement('tr'); row11.className = 'odd';
        const row12 = document.createElement('tr'); row12.className = 'even';
        const row13 = document.createElement('tr'); row13.className = 'odd';
        const row14 = document.createElement('tr'); row14.className = 'even';
        const row15 = document.createElement('tr'); row15.className = 'odd';
        const row16 = document.createElement('tr'); row16.className = 'even';
        const row17 = document.createElement('tr'); row17.className = 'odd';
        const row18 = document.createElement('tr'); row18.className = 'even';

        for (let i = 0; i < listTableColumnData.length; i++) {            
            let td = document.createElement('td'); td.className = 'display_table_header1';
            td.innerHTML = listTableColumnData[i].dayNum;  
            row1.appendChild(td);

            td = document.createElement('td'); td.className = 'display_table_header2';
            if (listTableColumnData[i].measuareDateUnixTimeStamp) {
                td.innerHTML = moment(listTableColumnData[i].measuareDateUnixTimeStamp).format('YYYY/MM/DD');
            }
            row2.appendChild(td);

            td = document.createElement('td'); $(td).prop('contenteditable', true);
            td.innerHTML = listTableColumnData[i].locationTemperature;
            td.onclick = function () {
                document.execCommand('selectAll', false, null);
            };
            td.focusin = function () {
                $(this).data('val', $(this).text());
                $(this).data('text', '');
            };
            td.onkeydown = function (e) {
                var startPostion = e.target.selectionStart;
            };
            td.onkeypress = function (e) {
                var charCode = (e.which) ? e.which : event.keyCode;
                var chrInput = String.fromCharCode(charCode);
                if (chrInput.match(/[^0-9.-]/g))
                    return false;
                if ($(this).data('val') != $(this).text()) {
                    if (chrInput == "-") {
                        var txtInput = $(this).text();
                        if (txtInput != '')
                            return false;
                    } else if (chrInput == ".") {
                        var txtInput = $(this).text();
                        if (txtInput.indexOf(".") != -1)
                            return false;
                    }
                }
            };
            td.focusout = function(){
                var prev = $(this).data('val');
                var current = $(this).text();
                if (isNaN(current) || (current == "" && prev != "")) {
                    $(this).text(prev);
                }
            };
            row3.appendChild(td);

            td = document.createElement('td'); $(td).prop('contenteditable', true);
            let temAvp = Math.round(listTableColumnData[i].productTemperatureAvg * 10) / 10;
            td.innerHTML = temAvp;
            td.onclick = function () {
                document.execCommand('selectAll', false, null);
            };
            td.focusin = function () {
                $(this).data('val', $(this).text());
                $(this).data('text', '');
            };
            td.onkeypress = function (e) {
                var charCode = (e.which) ? e.which : event.keyCode;
                var chrInput = String.fromCharCode(charCode);
                if (chrInput.match(/[^0-9.-]/g))
                    return false;
                if ($(this).data('val') != $(this).text()) {
                    if (chrInput == "-") {
                        var txtInput = $(this).text();
                        if (txtInput != '')
                            return false;
                    } else if (chrInput == ".") {
                        var txtInput = $(this).text();
                        if (txtInput.indexOf(".") != -1)
                            return false;
                    }
                }
            };
            td.focusout = function () {
                var prev = $(this).data('val');
                var current = $(this).text();
                if (isNaN(current) || (current == "" && prev != "")) {
                    $(this).text(prev);
                }
            };
            row4.appendChild(td);

            td = document.createElement('td');
            td.innerHTML = listTableColumnData[i].baumeDegree;
            $(td).prop('contenteditable', true);
            td.onclick = function () {
                document.execCommand('selectAll', false, null);
            };
            td.focusin = function () {
                $(this).data('val', $(this).text());
                $(this).data('text', '');
            };
            td.onkeydown = function (e) {
                var startPostion = e.target.selectionStart;
            };
            td.onkeypress = function (e) {
                var charCode = (e.which) ? e.which : event.keyCode;
                var chrInput = String.fromCharCode(charCode);
                if (chrInput.match(/[^0-9.-]/g))
                    return false;
                if ($(this).data('val') != $(this).text()) {
                    if (chrInput == "-") {
                        var txtInput = $(this).text();
                        if (txtInput != '')
                            return false;
                    } else if (chrInput == ".") {
                        var txtInput = $(this).text();
                        if (txtInput.indexOf(".") != -1)
                            return false;
                    }
                }
            };
            td.focusout = function () {
                var prev = $(this).data('val');
                var current = $(this).text();
                if (isNaN(current) || (current == "" && prev != "")) {
                    $(this).text(prev);
                }
            };
            row5.appendChild(td);

            td = document.createElement('td');
            td.innerHTML = listTableColumnData[i].baumeSakeDegree;
            row6.appendChild(td);

            td = document.createElement('td');
            td.innerHTML = listTableColumnData[i].alcoholDegree;
            $(td).prop('contenteditable', true);
            td.onclick = function () {
                document.execCommand('selectAll', false, null);
            };
            td.focusin = function () {
                $(this).data('val', $(this).text());
                $(this).data('text', '');
            };
            td.onkeydown = function (e) {
                var startPostion = e.target.selectionStart;
            };
            td.onkeypress = function (e) {
                var charCode = (e.which) ? e.which : event.keyCode;
                var chrInput = String.fromCharCode(charCode);
                if (chrInput.match(/[^0-9.-]/g))
                    return false;
                if ($(this).data('val') != $(this).text()) {
                    if (chrInput == "-") {
                        var txtInput = $(this).text();
                        if (txtInput != '')
                            return false;
                    } else if (chrInput == ".") {
                        var txtInput = $(this).text();
                        if (txtInput.indexOf(".") != -1)
                            return false;
                    }
                }
            };
            td.focusout = function () {
                var prev = $(this).data('val');
                var current = $(this).text();
                if (isNaN(current) || (current == "" && prev != "")) {
                    $(this).text(prev);
                }
            };
            row7.appendChild(td);

            td = document.createElement('td');
            td.innerHTML = listTableColumnData[i].acid;
            $(td).prop('contenteditable', true);
            td.onclick = function () {
                document.execCommand('selectAll', false, null);
            };
            td.focusin = function () {
                $(this).data('val', $(this).text());
                $(this).data('text', '');
            };
            td.onkeydown = function (e) {
                var startPostion = e.target.selectionStart;
            };
            td.onkeypress = function (e) {
                var charCode = (e.which) ? e.which : event.keyCode;
                var chrInput = String.fromCharCode(charCode);
                if (chrInput.match(/[^0-9.-]/g))
                    return false;
                if ($(this).data('val') != $(this).text()) {
                    if (chrInput == "-") {
                        var txtInput = $(this).text();
                        if (txtInput != '')
                            return false;
                    } else if (chrInput == ".") {
                        var txtInput = $(this).text();
                        if (txtInput.indexOf(".") != -1)
                            return false;
                    }
                }
            };
            td.focusout = function () {
                var prev = $(this).data('val');
                var current = $(this).text();
                if (isNaN(current) || (current == "" && prev != "")) {
                    $(this).text(prev);
                }
            };
            row8.appendChild(td);

            td = document.createElement('td');
            td.innerHTML = listTableColumnData[i].aminoAcid;
            $(td).prop('contenteditable', true);
            td.onclick = function () {
                document.execCommand('selectAll', false, null);
            };
            td.focusin = function () {
                $(this).data('val', $(this).text());
                $(this).data('text', '');
            };
            td.onkeydown = function (e) {
                var startPostion = e.target.selectionStart;
            };
            td.onkeypress = function (e) {
                var charCode = (e.which) ? e.which : event.keyCode;
                var chrInput = String.fromCharCode(charCode);
                if (chrInput.match(/[^0-9.-]/g))
                    return false;
                if ($(this).data('val') != $(this).text()) {
                    if (chrInput == "-") {
                        var txtInput = $(this).text();
                        if (txtInput != '')
                            return false;
                    } else if (chrInput == ".") {
                        var txtInput = $(this).text();
                        if (txtInput.indexOf(".") != -1)
                            return false;
                    }
                }
            };
            td.focusout = function () {
                var prev = $(this).data('val');
                var current = $(this).text();
                if (isNaN(current) || (current == "" && prev != "")) {
                    $(this).text(prev);
                }
            };
            row9.appendChild(td);

            td = document.createElement('td');
            td.innerHTML = listTableColumnData[i].glucose;
            $(td).prop('contenteditable', true);
            td.onclick = function () {
                document.execCommand('selectAll', false, null);
            };
            td.focusin = function () {
                $(this).data('val', $(this).text());
                $(this).data('text', '');
            };
            td.onkeydown = function (e) {
                var startPostion = e.target.selectionStart;
            };
            td.onkeypress = function (e) {
                var charCode = (e.which) ? e.which : event.keyCode;
                var chrInput = String.fromCharCode(charCode);
                if (chrInput.match(/[^0-9.-]/g))
                    return false;
                if ($(this).data('val') != $(this).text()) {
                    if (chrInput == "-") {
                        var txtInput = $(this).text();
                        if (txtInput != '')
                            return false;
                    } else if (chrInput == ".") {
                        var txtInput = $(this).text();
                        if (txtInput.indexOf(".") != -1)
                            return false;
                    }
                }
            };
            td.focusout = function () {
                var prev = $(this).data('val');
                var current = $(this).text();
                if (isNaN(current) || (current == "" && prev != "")) {
                    $(this).text(prev);
                }
            };
            row10.appendChild(td);

            td = document.createElement('td'); td.style.display = "none";
            td.innerHTML = listTableColumnData[i].id_LocationTemperature;
            row11.appendChild(td);

            td = document.createElement('td'); td.style.display = "none";
            td.innerHTML = listTableColumnData[i].id_TemperatureAvg;
            row12.appendChild(td);

            td = document.createElement('td'); td.style.display = "none";
            td.innerHTML = listTableColumnData[i].idTerminal_LocationTemperature;
            row13.appendChild(td);

            td = document.createElement('td'); td.style.display = "none";
            td.innerHTML = listTableColumnData[i].idTerminal_TemperatureAvg;
            row14.appendChild(td);

            td = document.createElement('td'); td.style.display = "none";
            td.innerHTML = listTableColumnData[i].lotcontainerId_LocationTemperature;
            row15.appendChild(td);

            td = document.createElement('td'); td.style.display = "none";
            td.innerHTML = listTableColumnData[i].lotcontainerId_TemperatureAvg;
            row16.appendChild(td);

            td = document.createElement('td'); td.style.display = "none";
            td.innerHTML = listTableColumnData[i].id_DataEntry;
            row17.appendChild(td);

            td = document.createElement('td'); td.style.display = "none";
            td.innerHTML = listTableColumnData[i].id_Container;
            row18.appendChild(td);
        }

        tbody.appendChild(row1);
        tbody.appendChild(row2);
        tbody.appendChild(row3);
        tbody.appendChild(row4);
        tbody.appendChild(row5);
        tbody.appendChild(row6);
        tbody.appendChild(row7);
        tbody.appendChild(row8);
        tbody.appendChild(row9);
        tbody.appendChild(row10);
        tbody.appendChild(row11);
        tbody.appendChild(row12);
        tbody.appendChild(row13);
        tbody.appendChild(row14);
        tbody.appendChild(row15);
        tbody.appendChild(row16);
        tbody.appendChild(row17);
        tbody.appendChild(row18);

        AdjustmentHeigthRowOfTable();
        // add update ScrollX to the end table
        UpdateScroll();
    }
}
// scroll to end table
function UpdateScroll() {
    var element = document.getElementsByClassName("dataTables_scrollBody");
        element[0].scrollLeft = element[0].scrollWidth;
}

var S02001_chart;
//var countTemp = 0;
function RenderChartData(chartData) {
    if (chartData) {
        document.getElementById('S02001_lineChart').style.display = "block";
        const labelYRoomTemp = document.getElementById('roomTemp').value;
        const labelAvTemp = document.getElementById('avgTempCol').value;
        const labelBaume = document.getElementById('baume2').value;
        const labelAlcohol = document.getElementById('alcohol').value;
        let ctxL1 = document.getElementById('S02001_lineChart').getContext('2d');
        let datetimeX = new Array();
        for (let j = 0; j < chartData.listMeasuareUnixTimestamp.length; j++) {
            let dispContent = moment(chartData.listMeasuareUnixTimestamp[j]).format('YYYY/MM/DD');
            datetimeX.push(dispContent);
        }
        
        if (S02001_chart) {
            S02001_chart.destroy();
        }
        S02001_chart = new Chart(ctxL1, {
            type: 'line',
            data: {
                labels: datetimeX,
                datasets: [
                    {
                        type: 'line',
                        label: labelYRoomTemp,
                        yAxisID: 'A',
                        fill: false,
                        borderColor: "#FF3333",
                        data: chartData.listLocationTemperature
                    }, {
                        type: 'line',
                        label: labelAvTemp,
                        yAxisID: 'A',
                        fill: false,
                        borderColor: "#000080",
                        data: chartData.listProductTemperatureAvg
                    }, {
                        type: 'line',
                        label: labelBaume,
                        yAxisID: 'A',
                        fill: false,
                        borderColor: "#2E8B57",
                        data: chartData.listBaumeDegree
                    }, {
                        type: 'line',
                        label: labelAlcohol,
                        yAxisID: 'A',
                        fill: false,
                        borderColor: "#EEEE00",
                        data: chartData.listAlcoholDegree
                    }
                ]
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
                                var index;
                                for (let i = 0; i < datetimeX.length; i++) {
                                    if (label === datetimeX[i]) {
                                        index = i + 1;
                                    }
                                }
                                return index;
                            }
                        }
                    }, {
                        id: 'xAxis2',
                        type: "category",
                        gridLines: {
                            drawOnChartArea: false,
                        },
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
    } else {
        document.getElementById('S02001_lineChart').style.display = "none";
    }
}

function RenderMedia(listMedia) {
    let html = '';
    let imageUrl = document.getElementById('image_url').value;
    if (listMedia && listMedia.length > 0) {
        for (let i = 0; i < listMedia.length; i++) {
            let count1 = i + 1;
            html += ' <div class="mySlides"> ';
            html += '     <div class="numbertext">' + count1 + ' / ' + listMedia.length + '</div> ';
            html += '     <img src="' + imageUrl + '/' + listMedia[i].id + '" style="width:100%"> ';
            html += ' </div> ';
        }
        html += ' <a class="prev prevBtn" onclick="plusSlides(-1)">❮</a> ';
        html += ' <a class="next nextBtn" onclick="plusSlides(1)">❯</a> ';
        html += ' <div class="caption-container"><p id="caption"></p></div> ';
        html += ' <div class = "slide_bar"> ';
        for (let j = 0; j < listMedia.length; j++) {
            let count2 = j + 1;
            let dispCaption = moment(listMedia[j].createdOnUnixTimeStamp).format('YYYY/MM/DD HH:mm');
            html += '  <div class="column"> ';
            html += '      <img class="demo cursor" src="' + imageUrl + '/' + listMedia[j].id + '" style="width:100%" onclick="currentSlide(\'' + count2 + '\')" alt="' + dispCaption + '"> ';
            html += '  </div> ';
        }
        html += ' </div> ';
    } else {
        _SlideIndex = 1;
        html += ' <div class="mySlides" style ="display:none"> ';
        html += ' </div> ';
        html += ' <div class="caption-container" style ="display:none"><p id="caption"></p></div> ';
        html += '      <img class="demo cursor" style ="display:none"> ';
    }

    /*↓LuanLV Hide Media at 2022/08/30↓*/
    document.getElementById('img-frame').innerHTML = html;
    ShowSlides(1);
    /*↑LuanLV Hide Media at 2022/08/30↑*/
}


function AdjustmentHeigthRowOfTable() {
    var headerTableRows = document.getElementById('headerTable').getElementsByTagName('tbody')[0].rows;
    var displayDataTableRows = document.getElementById('displayDataTable').getElementsByTagName('tbody')[0].rows;

    for (let i = 0; i < headerTableRows.length; i++) {
        if (displayDataTableRows.length > i) {
            displayDataTableRows[i].style.height = headerTableRows[i].offsetHeight + 'px';
        }
    }
}
window.addEventListener('resize', function (event) {
    if (document.getElementById('is_edit_flg') == null) {
        AdjustmentHeigthRowOfTable();
    } else {
        var table = $('#moromiTable').DataTable();
        table.columns.adjust().draw();
    }
}, true);

function DirectEdit() {
    //const form = document.createElement('form');
    //form.action = document.getElementById('edit_action_url').value;
    //form.method = 'post';

    //const lotContainerId = document.getElementById('LotContainerId').value;
    //const lotContainerIdEl = document.createElement('input');
    //lotContainerIdEl.setAttribute('type', 'hidden');
    //lotContainerIdEl.setAttribute('name', 'LotContainerId');
    //lotContainerIdEl.setAttribute('value', lotContainerId);
    //form.appendChild(lotContainerIdEl);

    //const factoryId = document.getElementById('FactoryId').value;
    //const factoryIdEl = document.createElement('input');
    //factoryIdEl.setAttribute('type', 'hidden');
    //factoryIdEl.setAttribute('name', 'FactoryId');
    //factoryIdEl.setAttribute('value', factoryId);
    //form.appendChild(factoryIdEl);

    //const lotId = document.getElementById('LotId').value;
    //const lotIdEl = document.createElement('input');
    //lotIdEl.setAttribute('type', 'hidden');
    //lotIdEl.setAttribute('name', 'LotId');
    //lotIdEl.setAttribute('value', lotId);
    //form.appendChild(lotIdEl);

    //document.body.appendChild(form);
    //form.submit();

    const lotContainerId = document.getElementById('LotContainerId').value;
    const href = document.getElementById('edit_action_url').value;
    location.href = href + '?LotContainerId=' + lotContainerId;
}


// ↓ DataEntry --------------------------------------------------------------------------
function DirectIndex() {
    //const form = document.createElement('form');
    //form.action = document.getElementById('index_action_url').value;
    //form.method = 'post';

    //const lotContainerId = document.getElementById('LotContainerId').value;
    //const lotContainerIdEl = document.createElement('input');
    //lotContainerIdEl.setAttribute('type', 'hidden');
    //lotContainerIdEl.setAttribute('name', 'LotContainerId');
    //lotContainerIdEl.setAttribute('value', lotContainerId);
    //form.appendChild(lotContainerIdEl);

    //const factoryId = document.getElementById('FactoryId').value;
    //const factoryIdEl = document.createElement('input');
    //factoryIdEl.setAttribute('type', 'hidden');
    //factoryIdEl.setAttribute('name', 'FactoryId');
    //factoryIdEl.setAttribute('value', factoryId);
    //form.appendChild(factoryIdEl);

    //const lotId = document.getElementById('LotId').value;
    //const lotIdEl = document.createElement('input');
    //lotIdEl.setAttribute('type', 'hidden');
    //lotIdEl.setAttribute('name', 'LotId');
    //lotIdEl.setAttribute('value', lotId);
    //form.appendChild(lotIdEl);

    //document.body.appendChild(form);
    //form.submit();

    const href = document.getElementById('index_action_url').value;
    location.href = href;
}

function Delete(id) {
    Func.ClearErrorMessageForm();

    const confirmDeleteMsg = document.getElementById('resource_msg_delete_confir').value;
    UtilDialog.Confirm({
        type: UtilDialog.TYPE_DANGER,
        message: confirmDeleteMsg,
        ok: function () {
            Func.ajax({
                type: 'POST',
                url: '/S02001/DeleteDataEntry',
                data: 'DataEntryId=' + id,
                async: true,
                cache: false,
                processData: false,
                contentType: 'application/x-www-form-urlencoded; charset=utf-8',
                success: function (res) {
                    if (res.status) {
                        ReloadTable();
                    } else {
                        Func.ShowErrorMessageForm(res.message);
                    }
                },
                error: function (res) {
                    Func.ShowErrorMessageForm(res);
                }
            });
        },
        close: function () {
            // なし
        }
    });
}

function EditSave() {
    let lotContainerId = document.getElementById('LotContainerId').value;
    Func.ClearErrorMessageForm();
    $("#dataEntryForm").validate();
    if ($("#dataEntryForm").valid()) {
        const dataType = 'application/x-www-form-urlencoded; charset=utf-8';
        let data = Func.GetValueFormForPost('dataEntryForm');
        data = data + "&DataEntry.LotContainerId=" + lotContainerId;

        Func.ajax({
            type: 'POST',
            url: '/S02001/AddDataEntry',
            data: data,
            async: true,
            cache: false,
            processData: false,
            contentType: dataType,
            success: function (res) {
                if (res.status) {
                    UtilAlert.Show({
                        type: UtilAlert.TYPE_SUCCESS,
                        message: document.getElementById('resource_msg_regist_completed').value,
                    });
                    ReloadTable();
                } else {
                    Func.ShowErrorMessageForm(res.message);
                }
            },
            error: function (res) {
                Func.ShowErrorMessageForm(res);
            }
        });
    } else {
        Func.ScrollIntoViewError();
    }
    $("#dataEntryForm")[0].reset();
}

function InitTable() {

    var table = $('#moromiTable').DataTable({
        "sAjaxSource": "/S02001/EditGetDataTable",
        "fnServerParams": function (aaData) {
            const lotContainerId = document.getElementById('LotContainerId').value;
            const searchDate = document.getElementById('search_input').value;
            aaData.push({ "name": "LotContainerId", "value": lotContainerId });
            aaData.push({ "name": "SearchByDate", "value": searchDate });
        },
        "bServerSide": true,
        "bProcessing": true,
        "language": {
            paginate: {
                next: '<i class="bi bi-chevron-double-right"></i>',
                previous: '<i class="bi bi-chevron-double-left"></i>'
            },
            emptyTable: document.getElementById('resource_empty_table').value,
        },
        "scrollX": true,
        "sScrollXInner": "100%",
        "scrollY": "300px",
        "scrollCollapse": true,
        "searching": false,
        "bLengthChange": false,
        "bSort": false,
        "bInfo": false,
        "pageLength": 10,
        "bPaginate": true,
        'columnDefs': [{
            'targets': 0,
            'searchable': true,
            'orderable': true,
            'render': function (data, type, full, meta) {
                return 0;
            }
        }, {
            'targets': 1,
            'searchable': true,
            'orderable': true,
            'render': function (data, type, full, meta) {
                let html = '<span onclick="Delete(\'' + full.id + '\')" class="cursor-pointer" ><i class="bi bi-trash"></i></span>';
                return html;
            }
        }, {
            'targets': 2,
            'searchable': true,
            'orderable': true,
            'render': function (data, type, full, meta) {
                return full.baumeDegree;
            }
        }, {
            'targets': 3,
            'searchable': true,
            'orderable': true,
            'render': function (data, type, full, meta) {
                return full.alcoholDegree;
            }
        }, {
            'targets': 4,
            'searchable': true,
            'orderable': true,
            'render': function (data, type, full, meta) {
                return full.acid;
            }
        }, {
            'targets': 5,
            'searchable': true,
            'orderable': true,
            'render': function (data, type, full, meta) {
                return full.aminoAcid;
            }
        }, {
            'targets': 6,
            'searchable': true,
            'orderable': true,
            'render': function (data, type, full, meta) {
                return full.glucose;
            }
        }, {
            'targets': 7,
            'width': '17%',
            'searchable': true,
            'orderable': true,
            'render': function (data, type, full, meta) {
                var Date = moment(full.measureDateUnixTimeStamp).format("YYYY-MM-DD HH:mm").toUpperCase();
                return Date;
            }
        }]
    });
    // add auto counter column
    table.on('draw.dt', function () {
        var PageInfo = $('#moromiTable').DataTable().page.info();
        table.column(0, { page: 'current' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1 + PageInfo.start;
        });
    });

    table.columns.adjust().draw();

    // highlight for row table
    $('#moromiTable tbody')
        .on('mouseenter', 'td', function () {
            var colIdx = table.cell(this).index().column;
            var rowIdx = table.cell(this).index().row;

            $(table.rows().nodes()).removeClass('highlight');
            $(table.cells().nodes()).removeClass('highlight');
            //$(t.column(colIdx).nodes()).addClass('highlight');
            $(table.row(rowIdx).nodes()).addClass('highlight');
        });

    // display data table in Days
    $('#displayDataTable').DataTable({
        language: {
            paginate: {
                next: '<i class="bi bi-chevron-double-right"></i>',
                previous: '<i class="bi bi-chevron-double-left"></i>'
            }
        },
        "scrollX": true,
        "sScrollXInner": "100%",
        "scrollY": "100%",
        "scrollCollapse": true,
        "searching": false,
        "bLengthChange": false,
        "bSort": false,
        "bInfo": false,
        "bPaginate": false,
    });

    // display data table Header
    $('#headerTable').DataTable({
        "searching": false,
        "bLengthChange": false,
        "bSort": false,
        "bInfo": false,
        "bPaginate": false,
    });
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

        let cookiesLotContainerId = document.getElementById('cookies_LotContainerId').value;
        let lotContainerId = document.getElementById('LotContainerId').value;
        document.cookie = cookiesLotContainerId + '=' + lotContainerId + '; expires=' + expires + '; path=/';

        let cookiesIsInUse = document.getElementById('cookies_IsInUse').value;
        let isInUse = document.getElementById('IsInUse').checked;
        document.cookie = cookiesIsInUse + '=' + isInUse + '; expires=' + expires + '; path=/';

        IsInUse
    }
}

function SaveDataUpdate() {
    var $rows = $("#displayDataTable tbody tr");
    var data = [];
    var i_row = 0;
    $rows.each(function (row, v) {
        if (row == 1 || row == 2 || row == 3 || row == 10 || row == 11 || row == 12 || row == 13 || row == 14 || row == 15) {
            data[i_row] = [];
            $(this).find("td").each(function (cell, v) {
                data[i_row][cell] = $(this).text();
            });
            i_row++;
        }
    });
    var newData = transpose(data);
    var oldData = getListTableColumnDataBase();
    var temperateChange = CompareTemperateEdit(oldData, newData);
    var tempetareAvgChange = CompareTemperateAvgEdit(oldData, newData);

    var data_Entry = [];
    var i_row = 0;
    $rows.each(function (row, v) {
        if (row == 1 || row == 4 || row == 6 || row == 7 || row == 8 || row == 9 || row == 14 || row == 16|| row == 17) {
            data_Entry[i_row] = [];
            $(this).find("td").each(function (cell, v) {
                data_Entry[i_row][cell] = $(this).text();
            });
            i_row++;
        }
    });

    var newDataEntry = transpose(data_Entry);
    var oldDataEntry = getListTableColumnDataBaseForDataEntry();
    var dataEntryChange = CompareDataEntryEdit(oldDataEntry, newDataEntry);

    if (temperateChange.length > 0 || tempetareAvgChange.length > 0 || dataEntryChange.length > 0) {
        UtilDialog.Confirm({
            type: UtilDialog.TYPE_PRIMARY,
            message: $("#S02001_cfm_SaveEdit").text(),
            ok: function () {
                SaveTemperateDataEntryEdit(temperateChange, tempetareAvgChange, dataEntryChange);
            },
            close: function () {

            }
        });
    }
    
}

function CompareTemperateEdit(oldData, newData, temperateChange)
{
    let lotContainerId = document.getElementById('LotContainerId').value;
    var temperateChange = [];
    var id_TerminalLocation = "";
    var id_LotcontainerIdLocationTemperature = "";
    for (let i = 0; i < oldData.length; i++) {
        if (oldData[i][5] != "") {
            id_TerminalLocation = oldData[i][5];
        }
        if (oldData[i][7] != "") {
            id_LotcontainerIdLocationTemperature = oldData[i][7];
        }
        if (oldData[i][1] != newData[i][1]) {
            var ojb = {
                id_LocationTemperature: newData[i][3],
                locationTemperature: newData[i][1],
                id_TemperatureAvg: newData[i][4],
                productTemperatureAvg: newData[i][2],
                measuareDateUnixTimeStamp: newData[i][0],
                idTerminal_LocationTemperature: newData[i][5],
                idTerminal_TemperatureAvg: newData[i][6],
                lotcontainerId_LocationTemperature: newData[i][7],
                lotcontainerId_TemperatureAvg: newData[i][8]
            }
            temperateChange.push(ojb);
        }
    }
    for (let i = 0; i < temperateChange.length; i++) {
        temperateChange[i].idTerminal_LocationTemperature = id_TerminalLocation;
        temperateChange[i].lotcontainerId_LocationTemperature = lotContainerId;
    }
    return temperateChange;
}

function CompareTemperateAvgEdit(oldData, newData) {
    let lotContainerId = document.getElementById('LotContainerId').value;
    var tempetareAvgChange = [];
    var id_TerminalTemperatureAvg = "";
    var id_LotcontainerIdTemperatureAvg = "";
    for (let i = 0; i < oldData.length; i++) {
        if (oldData[i][6] != "") {
            id_TerminalTemperatureAvg = oldData[i][6];
        }
        if (oldData[i][8] != "") {
            id_LotcontainerIdTemperatureAvg = oldData[i][8];
        }
        if (oldData[i][2] != newData[i][2]) {
            var newojb = {
                id_LocationTemperature: newData[i][3],
                locationTemperature: newData[i][1],
                id_TemperatureAvg: newData[i][4],
                productTemperatureAvg: newData[i][2],
                measuareDateUnixTimeStamp: newData[i][0],
                idTerminal_LocationTemperature: newData[i][5],
                idTerminal_TemperatureAvg: newData[i][6],
                lotcontainerId_LocationTemperature: newData[i][7],
                lotcontainerId_TemperatureAvg: newData[i][8]
            }
            tempetareAvgChange.push(newojb);
        }
    }
    for (let i = 0; i < tempetareAvgChange.length; i++) {
        tempetareAvgChange[i].idTerminal_TemperatureAvg = id_TerminalTemperatureAvg;
        tempetareAvgChange[i].lotcontainerId_TemperatureAvg = lotContainerId;
    }
    return tempetareAvgChange;
}

function CompareDataEntryEdit(oldData, newData, dataEntryChange) {
    let lotContainerId = document.getElementById('LotContainerId').value;
    var dataEntryChange = [];
    var id_ContainerId = "";
    for (let i = 0; i < oldData.length; i++) {
        if (oldData[i][8] != "") {
            id_ContainerId = oldData[i][7];
        }
        if (oldData[i][1] != newData[i][1] || oldData[i][2] != newData[i][2] || oldData[i][3] != newData[i][3] || oldData[i][4] != newData[i][4] || oldData[i][5] != newData[i][5]) {
            var ojb = {
                id: newData[i][7],
                containerId: newData[i][8],
                baumeDegree: newData[i][1],
                alcoholDegree: newData[i][2],
                acid: newData[i][3],
                aminoAcid: newData[i][4],
                note: null,
                measureDate: newData[i][0],
                createdOnte: null,
                createdById: null,
                modifiedOn: null,
                modifiedById: null,
                glucose: newData[i][5],
                lotContainerId: newData[i][6]
            }
            dataEntryChange.push(ojb);
        }
    }
    for (let i = 0; i < dataEntryChange.length; i++) {
        dataEntryChange[i].containerId = id_ContainerId;
        dataEntryChange[i].lotContainerId = lotContainerId;
    }
    return dataEntryChange;
}

function SaveTemperateDataEntryEdit(temperateChange, temperateAvgChange, dataEntryChange) {
    Func.ajax({
        type: 'POST',
        url: '/S02001/SaveTemperateDataEntryEdit',
        data: { temperateChange: temperateChange, temperateAvgChange: temperateAvgChange, dataEntryChange: dataEntryChange },
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {
                //TankDataRender(res.data);
                location.reload();
                console.log("Finish!");
            } else {
                Func.ShowErrorMessageForm(res.message);
            }
        },
        error: function (res) {
            Func.ShowErrorMessageForm(res);
        }
    });
}

function transpose(matrix) {
    let [row] = matrix
    return row.map((value, column) => matrix.map(row => row[column]))
}

function getListTableColumnDataBase() {
    var data = [];
    for (var i = 0; i < _ListTableColumnData.length; i++) {
        data[i] = [];
        data[i][0] = _ListTableColumnData[i].measuareDateUnixTimeStamp != null ? _ListTableColumnData[i].measuareDateUnixTimeStamp : "";
        data[i][1] = _ListTableColumnData[i].locationTemperature != null ? _ListTableColumnData[i].locationTemperature : "";
        data[i][2] = _ListTableColumnData[i].productTemperatureAvg != null ? _ListTableColumnData[i].productTemperatureAvg : "0";
        data[i][3] = _ListTableColumnData[i].id_LocationTemperature != null ? _ListTableColumnData[i].id_LocationTemperature : "";
        data[i][4] = _ListTableColumnData[i].id_TemperatureAvg != null ? _ListTableColumnData[i].id_TemperatureAvg : "";
        data[i][5] = _ListTableColumnData[i].idTerminal_LocationTemperature != null ? _ListTableColumnData[i].idTerminal_LocationTemperature : "";
        data[i][6] = _ListTableColumnData[i].idTerminal_TemperatureAvg != null ? _ListTableColumnData[i].idTerminal_TemperatureAvg : "";
        data[i][7] = _ListTableColumnData[i].lotcontainerId_LocationTemperature != null ? _ListTableColumnData[i].lotcontainerId_LocationTemperature : "";
        data[i][8] = _ListTableColumnData[i].lotcontainerId_TemperatureAvg != null ? _ListTableColumnData[i].lotcontainerId_TemperatureAvg : "";
        data[i][9] = _ListTableColumnData[i].id_DataEntry != null ? _ListTableColumnData[i].id_DataEntry : "";
        data[i][10] = _ListTableColumnData[i].baumeDegree != null ? _ListTableColumnData[i].baumeDegree : "";
        data[i][11] = _ListTableColumnData[i].alcoholDegree != null ? _ListTableColumnData[i].alcoholDegree : "";
        data[i][12] = _ListTableColumnData[i].acid != null ? _ListTableColumnData[i].acid : "";
        data[i][13] = _ListTableColumnData[i].aminoAcid != null ? _ListTableColumnData[i].aminoAcid : "";
        data[i][14] = _ListTableColumnData[i].glucose != null ? _ListTableColumnData[i].glucose : "";
        data[i][15] = _ListTableColumnData[i].id_Container != null ? _ListTableColumnData[i].id_Container : "";

    }
    return data;
}

function getListTableColumnDataBaseForDataEntry() {
    var data = [];
    for (var i = 0; i < _ListTableColumnData.length; i++) {
        data[i] = [];
        data[i][0] = _ListTableColumnData[i].measuareDateUnixTimeStamp != null ? _ListTableColumnData[i].measuareDateUnixTimeStamp : "";
        data[i][1] = _ListTableColumnData[i].baumeDegree != null ? _ListTableColumnData[i].baumeDegree : "";
        data[i][2] = _ListTableColumnData[i].alcoholDegree != null ? _ListTableColumnData[i].alcoholDegree : "";
        data[i][3] = _ListTableColumnData[i].acid != null ? _ListTableColumnData[i].acid : "";
        data[i][4] = _ListTableColumnData[i].aminoAcid != null ? _ListTableColumnData[i].aminoAcid : "";
        data[i][5] = _ListTableColumnData[i].glucose != null ? _ListTableColumnData[i].glucose : "";
        data[i][6] = _ListTableColumnData[i].lotcontainerId_LocationTemperature != null ? _ListTableColumnData[i].lotcontainerId_LocationTemperature : "";
        data[i][7] = _ListTableColumnData[i].id_DataEntry != null ? _ListTableColumnData[i].id_DataEntry : "";
        data[i][8] = _ListTableColumnData[i].id_Container != null ? _ListTableColumnData[i].id_Container : "";

    }
    return data;
}


$(document).ready(function () {
    InitTable();

    if (document.getElementById('is_edit_flg') == null) {
        GetDataByTankId();
    }


    //var s02001Hub = $.connection.s02001Hub;
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var user = "S02001";
    //s02001Hub.client.SendAsync = function (message) {
    //    if (message && message !== '') {
    //        const jsonObj = JSON.parse(message);

    //        if (jsonObj && jsonObj.type === '1') {
    //            if (!document.getElementById('is_edit_flg')) {
    //                GetDataByTankId(true);
    //            }
    //        }
    //    }
    //};
    //$.connection.hub.start().done(function () {
    //    //なし
    //});
    
    connection.on("ReceiveMessage", function (user, message) {
        try {
            console.log(`${user} says ${message}`);
            const jsonObj = JSON.parse(message);

            if (jsonObj && jsonObj.type === '1') {
                if (!document.getElementById('is_edit_flg')) {
                    GetDataByTankId(true);
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


