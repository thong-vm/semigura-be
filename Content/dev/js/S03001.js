$(document).ready(function () {
    viewNumber2place();
    checkInputNumber();
    settingWithByRiceRatio();
    showhideFinish();
    if ($("#S03001_MaterialID").val() == "") {
        $("#iSeimaibuai").hide();
    }
    else {
        $("#iSeimaibuai").show();
    }
    var typetank = $("#Type_Tank").text();
    var typesensor = $("#Type_Sensor").text();
    var btnFinish = $("#S03001_btn_finish").text();

    var table = $('#lotTable').DataTable({
        "sAjaxSource": "/S03001/GetDataTable",
        "fnServerParams": function (aaData) {
            const searchFactory = document.getElementById('S03001_sfactoryID').value;
            const searchLot = document.getElementById('S03001_sLotCode').value;
            const searchTankCode = document.getElementById('S03001_sTankCode').value;
            const searchInUse = document.getElementById('using').checked;
            aaData.push({ "name": "FactoryID", "value": searchFactory });
            aaData.push({ "name": "LotCode", "value": searchLot });
            aaData.push({ "name": "TankCode", "value": searchTankCode });
            aaData.push({ "name": "IsInUse", "value": searchInUse });
        },
        "columns": [
            { "data": "lotId" },
            { "data": "lotId" },
            null,
            { "data": "lotId" },
            null,
            { "data": "lotId" },
            { "data": "lotId" },
            { "data": "lotId" },
            { "data": "lotId" },
            { "data": "lotId" },
            { "data": "lotId" },
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        ],
        rowsGroup: [0, 1, 3, 5, 6, 7, 8, 9, 10],
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
        "scrollY": "500px",
        "scrollCollapse": true,
        "searching": false,
        "bLengthChange": false,
        "bSort": false,
        "bInfo": false,
        "pageLength": 10,
        "bPaginate": true,
        "stripeClasses": ['stripe-1'],
        "createdRow": function (row, data, index) {
            $('td', row).css({
                'border': '1px solid white'
            });
        },
        'columnDefs': [
            {
                'targets': 0,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    //count++;
                    //return count;
                    return full.no;
                }
            }
            ,
            {
                'targets': 1,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    let html = '<span onclick="DirectEdit(\'' + full.lotId + '\')" class="cursor-pointer" ><i class="bi bi-pencil-square"></i></span>';
                    html += '<span onclick="Delete(\'' + full.lotId + '\')" class="cursor-pointer" ><i class="bi bi-trash"></i></span>';

                    return html;
                }
            },
            {
                'targets': 2,
                'visible': false,
                'render': function (data, type, full, meta) {
                    return full.factoryId;
                }
            },
            {
                'targets': 3,
                'visible': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    return full.factoryName;
                }
            },
            {
                'targets': 4,
                'visible': false,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    return full.lotId;
                }
            },
            {
                'targets': 5,
                'name': 'LotCode',
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    return full.lotCode;
                }
            },
            {
                'targets': 6,
                'visible': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    return full.rice;
                }
            },
            {
                'targets': 7,
                'visible': true,
                'className': 'dt-body-right',
                'render': function (data, type, full, meta) {
                    return full.kubunName; //full.kubun;
                }
            },
            {
                'targets': 8,
                'visible': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    if (full.riceRatio != null && full.seimaibuai != null) {
                        return full.seimaibuai + " : " + full.riceRatio + "%";
                    }
                    else if (full.seimaibuai != null) {
                        return full.seimaibuai;
                    }
                    else {
                        return null;
                    }

                }
            },
            {
                'targets': 9,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    if (full.startDateLot == null) {
                        return "";
                    }
                    var Date = moment(full.startDateLotUnixTimeStamp).format("YYYY/MM/DD").toUpperCase();
                    return Date;
                }
            },
            {
                'targets': 10,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    if (full.endDateLot == null) {
                        let html = '<button type="button" onclick="FinishDateLot(\'' + full.lotId + '\')" class="btn btn-primary btn-sub" title="' + btnFinish + '"><i class="bi bi-square-fill"></i></button>';
                        return html;
                    }
                    var Date = moment(full.endDateLotUnixTimeStamp).format("YYYY/MM/DD").toUpperCase();
                    return Date;
                }
            },
            {
                'targets': 11,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    if (full.type == 1) {
                        return typetank;
                    }
                    else if (full.type == 2) {
                        return typesensor;
                    }
                    else {
                        return "";
                    }

                }
            },
            {
                'targets': 12,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    let html = full.tankCode;
                    if (full.deleteFlg) {
                        html = '<del>' + full.tankCode + '</del>';
                    }
                    return html;
                }
            },
            {
                'targets': 13,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-right',
                'render': function (data, type, full, meta) {
                    return full.capacity;
                }
            },
            {
                'targets': 14,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-right',
                'render': function (data, type, full, meta) {
                    return full.height;
                }
            },
            {
                'targets': 15,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    if (full.startDate == null)
                        return "";
                    var Date = moment(full.startDateUnixTimeStamp).format("YYYY/MM/DD").toUpperCase();
                    return Date;
                }
            },
            {
                'targets': 16,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    if (full.endDate == null) {
                        let html = '<button type="button"  onclick="FinishDateLotContainer(\'' + full.id + '\',\'' + full.type + '\')" class="btn btn-primary btn-sub" title="' + btnFinish + '"><i class="bi bi-square-fill"></i></button>';
                        return html;
                    }
                    var Date = moment(full.endDateUnixTimeStamp).format("YYYY/MM/DD").toUpperCase();
                    return Date;
                }
            },
            {
                'targets': 17,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-right',
                'render': function (data, type, full, meta) {
                    return full.tempMin;
                }
            },
            {
                'targets': 18,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-right',
                'render': function (data, type, full, meta) {
                    return full.tempMax;
                }
            },
            {
                'targets': 19,
                'visible': false,
                'render': function (data, type, full, meta) {
                    return full.locationId;
                }
            }
        ]
    });

    // エラー発生時にデフォルトで表示されるメッセージボックスを無効化する。
    $.fn.dataTable.ext.errMode = 'none';

    // errorイベントを受け取るハンドラ（エラーレスポンスの取得はできない。）
    table.on('error.dt', function (e, settings, techNote, message) {
        Func.ErrorHandle(message);
        Func.CheckSessionExpired();
    });

    $("#S03001_startDateID").on("change", function (e) {
        console.log(e);
        var dt = this.value;
        if (this.value != "") {
            this.value = moment(this.value).format("YYYY/MM/DD");
        }
    }).trigger("change");

    $("#S03001_endDateID").on("change", function () {
        if (this.value != "") {
            this.value = moment(this.value).format("YYYY/MM/DD");
        }
    })
});

function SelectDateTank(s) {
    if (s.value != "") {
        s.value = moment(s.value).format("YYYY/MM/DD");
    }
}


function RemoveRow(el) {
    $(el).closest('.row').remove();
}

function AddRowTank() {
    var minTemp = $("#S03001_temperatureMin").text();
    var maxTemp = $("#S03001_temperatureMax").text();

    Func.ajax({
        type: 'POST',
        url: '/S03001/GetListTank',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (data) {
            var select = '<option value=""></option>';
            for (var item of data.result) {
                if (!item.deleteFlg) {
                    select += '<option value="' + item.id + '">' + item.code + '</option>';
                }
            }

            let html = '';
            html += '<div class="row sub-row">';
            html += '    <div class="col-md-2 col-sm-12">';
            html += '        <div class="form-group">';
            html += '            <div class="wapper-input" style="width:100%">';
            html += '                <select class="showhideFinish selectTank select-classic form-control" onchange="LoadInfoTank(this)">';
            html += select;
            html += '                </select>';
            html += '            </div>';
            html += '        </div>';
            html += '    </div>';
            html += '    <div class="S03001_addMinWidth50 col-md-2 col-sm-12">';
            html += '        <div class="form-group">';
            html += '            <div class="">';
            html += '                <div class="input-group">';
            html += '                    <input type="text" class="showhideFinish inputnumber form-control" placeholder="' + minTemp +'" style="text-align:right" aria-describedby="basic-addon1">';
            html += '                    <span class="input-group-text" id="basic-addon1" style="background-color: transparent;">°C</span>';
            html += '                </div>';
            html += '            </div>';
            html += '        </div>';
            html += '    </div>';
            html += '    <div class="S03001_addMinWidth50 col-md-2 col-sm-12">';
            html += '        <div class="form-group">';
            html += '            <div class="">';
            html += '                <div class="input-group">';
            html += '                    <input type="text" class="showhideFinish inputnumber form-control" placeholder="' + maxTemp +'" style="text-align:right" aria-describedby="basic-addon1">';
            html += '                    <span class="input-group-text" id="basic-addon1" style="background-color: transparent;">°C</span>';
            html += '                </div>';
            html += '            </div>';
            html += '        </div>';
            html += '    </div>';
            html += '    <div class="col-md-2 col-sm-12">';
            html += '        <div class="form-group">';
            html += '            <div class="input-group">';
            html += '                <button type="button" class="showhideFinish btn btn-dark btn-sub" title="開始日" disabled><i class="bi-play-btn-fill"></i></button>';
            html += '                <input type="datetime"  class="datepicker  form-control" style="text-align:left" value="' + moment(Date.now()).format('YYYY/MM/DD') + '" onclick="SelectDateTank(this)">';
            html += '            </div>';
            html += '        </div>';
            html += '    </div>';
            html += '    <div class="col-md-2 col-sm-12">';
            html += '        <div class="form-group">';
            html += '            <div class="input-group">';
            html += '                <input type="datetime" class="form-control" style="text-align:left" placeholder= "YYYY/MM/DD" value="" disabled>';
            html += '                <button type="button" class="showhideFinish btn btn-primary btn-sub" title="終了日" disabled><i class="bi bi-square-fill"></i></button>';
            html += '            </div>';
            html += '        </div>';
            html += '    </div>';
            html += '    <div class="col-md-2 col-sm-12">';
            html += '        <div class="form-group">';
            html += '            <div class="reggisterButton input-group">';
            html += '                <button type="button" class="showhideFinish btn btn-danger btn-sub" onclick="RemoveRow(this)" 　title="削除"><i class="bi bi-trash"></i></button>';
            html += '            </div>';
            html += '        </div>';
            html += '    </div>';
            html += '    <div class="EndDateTankFinish" style="display:none"></div>';
            html += '    <div class="S03001_TankIdOld" style="display:none"></div>';
            html += '</div>';
           
            $('#selected_tanks').prepend(html);
            $(".datepicker").datepicker({
                dateFormat: "yy/mm/dd",
                changemonth: true,
                changeyear: true
            });
        }
    });



}

function AddRowSensor() {
    Func.ajax({
        type: 'POST',
        url: '/S03001/GetListSensor',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (data) {
            var select = '<option value=""></option>';
            for (var item of data.result) {
                if (!item.deleteFlg) {
                    if (!item.name || item.name === '') {
                        item.name = '-';
                    }
                    select += '                    <option value="' + item.id + '">' + item.code + "(" + item.name + ")" + '</option>';
                }
            }

            let html = '';
            html += '<div class="row sub-row">';
            html += '    <div class="col-md-3 col-sm-12">';
            html += '        <div class="form-group">';
            html += '            <div class="wapper-input" style="width:100%">';
            html += '                <select class="showhideFinish selectTank select-classic form-control" onchange="LoadInfoSensor(this)">';
            html += select;
            html += '                </select>';
            html += '            </div>';
            html += '        </div>';
            html += '    </div>';
            //html += '    <div class="col-md-3 col-sm-12">';
            //html += '        <div class="form-group">';
            //html += '            <div class="reggisterButton">';
            //html += '                <button type="button" class="showhideFinish btn btn-danger btn-sub" onclick="RemoveRow(this)" 　title="削除"><i class="bi bi-trash"></i></button>';
            //html += '            </div>';
            //html += '        </div>';
            //html += '    </div>';
            html += '    <div class="col-md-3 col-sm-12">';
            html += '        <div class="form-group">';
            html += '            <div class="input-group">';
            html += '                <button type="button" class=" btn btn-dark btn-sub" title="開始日" disabled><i class="bi-play-btn-fill"></i></button>';
            html += '                <input type="datetime"  class="datepicker  form-control" style="text-align:left" value="' + moment(Date.now()).format('YYYY/MM/DD') + '" onclick="SelectDateTank(this)">';
            html += '            </div>';
            html += '        </div>';
            html += '    </div>';
            html += '    <div class="col-md-3 col-sm-12">';
            html += '        <div class="form-group">';
            html += '            <div class="input-group">';
            html += '                <input type="datetime" class="form-control" style="text-align:left" placeholder= "YYYY/MM/DD" value="" disabled>';
            html += '                <button type="button" class="showhideFinish btn btn-primary btn-sub" title="終了日" disabled><i class="bi bi-square-fill"></i></button>';
            html += '            </div>';
            html += '        </div>';
            html += '    </div>';
            html += '    <div class="col-md-2 col-sm-12">';
            html += '        <div class="form-group">';
            html += '            <div class="reggisterButton input-group">';
            html += '                <button type="button" class="showhideFinish btn btn-danger btn-sub" onclick="RemoveRow(this)" 　title="削除"><i class="bi bi-trash"></i></button>';
            html += '            </div>';
            html += '        </div>';
            html += '    </div>';
            html += '    <div class="EndDateSensorFinish" style="display:none"></div>';
            html += '    <div class="S03001_SensorIdOld" style="display:none"></div>';
            html += '</div>';

            $('#selected_sensors').prepend(html);

            $(".datepicker").datepicker({
                dateFormat: "yy/mm/dd",
                changemonth: true,
                changeyear: true
            });
        }
    });

}

function ReloadTableFinish() {
    var table = $('#lotTable').DataTable();
    table.ajax.reload(null, false);
}

function ReloadTable() {
    console.log("ReloadTable");
    Func.ClearErrorMessageForm();
    var table = $('#lotTable').DataTable();
    table.ajax.reload(null, true);
}

function DirectEdit(id) {
    var tblLot = $('#lotTable').DataTable();
    var dataLot = tblLot.data().toArray();
    var rowSelect = dataLot.filter(p => p.lotId == id)
    const href = document.getElementById('edit_action_url').value;
    location.href = href + '?LotId=' + id;
}

function Save() {
    Func.ClearErrorMessageForm();
    $("#inputForm").validate();
    if ($("#inputForm").valid()) {
        var LotID = $("#S03001_lotID").text();
        var StartDate = $("#S03001_startDateID").val();
        var EndDate = $("#S03001_endDateID").val();
        if (EndDate == "" && LotID != "") {
            var lstTankNotEnd = $("#selected_tanks").find('.EndDateTankFinish');
            var lstSensorNotEnd = $("#selected_sensors").find('.EndDateSensorFinish');
            var flgAllDateFinish = true;
            
            if (lstTankNotEnd.length > 0) {
                for (var item of lstTankNotEnd) {
                    if ($(item).text() == "") {
                        flgAllDateFinish = false;
                    }
                }
            }
            if (lstSensorNotEnd.length > 0) {
                for (var item of lstSensorNotEnd) {
                    if ($(item).text() == "") {
                        flgAllDateFinish = false;
                    }
                }
            }
            if (flgAllDateFinish == true) {
                UtilDialog.Confirm({
                    type: UtilDialog.TYPE_PRIMARY,
                    message: $("#S03001_cfm_enddatelot").text(),
                    ok: function () {
                        EndDate = moment(Date.now()).format('YYYY/MM/DD');
                        FinishDateLotEditSave($("#dateFinish"));
                      
                        ProcessSave(EndDate);
                    },
                    close: function () {
                        ProcessSave(EndDate);
                    }
                });
            } else {
                ProcessSave(EndDate);
            }
            
        } else {
         
            ProcessSave(EndDate);
        }
    }
}

function ProcessSave(EndDate) {
    const dataType = 'application/x-www-form-urlencoded; charset=utf-8';
    var LotID = $("#S03001_lotID").text();
    var LotCode = $("#S03001_lotCode").val();
    var FactoryID = $("#S03001_factoryID").val();
    var StartDate = $("#S03001_startDateID").val();
    var RiceID = $("#S03001_MaterialID").val();
    var KubunID = $("#S03001_KubunID").val();
    var SeimaibuaiID = $("#S03001_SeimaibuaiValue").val();
    var RiceRatio = $("#S03001_RicePolishingRatio").val();
    var MinTempSeigiku = $("#RiceInfo_MinTempSeigiku").val();
    var MaxTempSeigiku = $("#RiceInfo_MaxTempSeigiku").val();
    var LocationID = $("#S03001_locationID").val();

    if (checkLocationIDEmpty(LocationID)) {
        $(window).scrollTop(0);
        return;
    }

    if (checkMinMaxRice()) {
        $(window).scrollTop(0);
        return;
    }

    if (SeimaibuaiID == "") {
        RiceRatio = "";
    }

    var data = "&LotCode=" + LotCode + "&LotId="
        + LotID + "&FactoryID=" + FactoryID + "&StartDate="
        + StartDate + "&EndDate=" + EndDate + "&RiceId=" + RiceID
        + "&KubunId=" + KubunID
        + "&SemaibuaiId=" + SeimaibuaiID
        + "&RicePolishingRatio=" + RiceRatio
        + "&LocationID=" + LocationID
        + "&MinTempSeigiku=" + MinTempSeigiku + "&MaxTempSeigiku=" + MaxTempSeigiku;

    var dataTankAndSensor = getDataTankAndSensor();
    if (dataTankAndSensor == "") {
        $(window).scrollTop(0);
        return;
    }
    data += dataTankAndSensor;

    Func.ajax({
        type: 'POST',
        url: '/S03001/CheckPreSave',
        data: data,
        async: true,
        cache: false,
        processData: false,
        contentType: dataType,
        success: function (resCheckPreSave) {
            if (resCheckPreSave.status) {
                CallSave(data);
            } else {
                if (resCheckPreSave.error) {
                    Func.ShowErrorMessageForm(resCheckPreSave.message);
                    $(window).scrollTop(0);
                } else {
                    var msgAlertError = resCheckPreSave.message;
                    var dataTankSensorUsing = resCheckPreSave.msgTankSensorUsing;

                    var msgComfrim = "";
                    if (msgAlertError.length != 0) {
                        msgAlertError.forEach(function (value, i) {
                            if (msgComfrim == "") {
                                msgComfrim = value;
                            }
                            else {
                                msgComfrim = msgComfrim + "\n" + value;
                            }

                        });
                        msgComfrim = msgComfrim + "\n" + $("#S03001_cfrm_endlotcontainer").text();
                        UtilDialog.Confirm({
                            type: UtilDialog.TYPE_PRIMARY,
                            message: msgComfrim,
                            ok: function () {
                                Func.ajax({
                                    type: 'POST',
                                    url: '/S03001/CloseTankOtherLot',
                                    data: JSON.stringify(dataTankSensorUsing),
                                    contentType: "application/json",
                                    success: function (resCloseTankOtherLot) {
                                        if (resCloseTankOtherLot.status) {
                                            if (resCloseTankOtherLot.flgEndLot) {
                                                const confirmEndOtherLot = dataTankSensorUsing[0].lotCode + $("#S03001_cfr_end_otherlot").text();
                                                UtilDialog.Confirm({
                                                    type: UtilDialog.TYPE_PRIMARY,
                                                    message: confirmEndOtherLot,
                                                    ok: function () {
                                                        Func.ClearErrorMessageForm();
                                                        Func.ajax({
                                                            type: 'POST',
                                                            url: '/S03001/FinishDateLot',
                                                            data: { lotId: dataTankSensorUsing[0].lotId },
                                                            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
                                                            success: function (resFinishDateLot) {
                                                                if (resFinishDateLot.status) {
                                                                    CallSave(data);
                                                                } else {
                                                                    Func.ShowErrorMessageForm(resFinishDateLot.message);
                                                                    $(window).scrollTop(0);
                                                                }
                                                            },
                                                            error: function (resFinishDateLot) {
                                                                Func.ShowErrorMessageForm(resFinishDateLot);
                                                                $(window).scrollTop(0);
                                                            }
                                                        });
                                                    },
                                                    close: function () {
                                                        CallSave(data);
                                                    }
                                                });
                                            }
                                            else {
                                                CallSave(data);
                                            }

                                        } else {
                                            var msgAlertError = resCloseTankOtherLot.message;
                                            if (msgAlertError.endsWith(",")) {
                                                msgAlertError = msgAlertError.substring(0, msgAlertError.length - 1) + msgUsing;
                                            }
                                            Func.ShowErrorMessageForm(msgAlertError);
                                            $(window).scrollTop(0);
                                        }
                                    },
                                    error: function (resCloseTankOtherLot) {
                                        Func.ShowErrorMessageForm(resCloseTankOtherLot);
                                        $(window).scrollTop(0);
                                    }
                                });
                            },
                            close: function () {
                                //Nothing
                            }
                        });
                    }
                }
            }
        },
        error: function (resCheckPreSave) {
            Func.ShowErrorMessageForm(resCheckPreSave);
            $(window).scrollTop(0);
        }
    });
}

function CallSave(data) {
    Func.ajax({
        type: 'POST',
        url: '/S03001/Save',
        data: data,
        async: true,
        cache: false,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {
                location.href = document.getElementById('main_page_url').value;
            } else {
                var msgAlertError = res.message;
                if (msgAlertError.endsWith(",")) {
                    msgAlertError = msgAlertError.substring(0, msgAlertError.length - 1) + msgUsing;
                }
                Func.ShowErrorMessageForm(msgAlertError);
                $(window).scrollTop(0);
            }
        },
        error: function (res) {
            Func.ShowErrorMessageForm(res);
            $(window).scrollTop(0);
        }
    });
}

function Delete(id) {
    Func.ClearErrorMessageForm();

    const confirmDeleteMsg = document.getElementById('resource_msg_delete_confirm').value;
    UtilDialog.Confirm({
        type: UtilDialog.TYPE_DANGER,
        message: confirmDeleteMsg,
        ok: function () {
            Func.ajax({
                type: 'POST',
                url: '/S03001/Delete',
                data: 'LotId=' + id,
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


function LoadDataByRiceId() {
    $("#iRiceInfo").empty();
    var riceId = $("#S03001_MaterialID").val();
    if (riceId == "") {
        $("#iSeimaibuai").hide();
        $("#S03001_KubunID").val("");
        $("#S03001_SeimaibuaiID").val("");
        $("#S03001_SeimaibuaiValue").val("");
        $("#S03001_RicePolishingRatio").val("");
    }
    else {
        $("#iSeimaibuai").show();
    }
    var moromi = $("#S03001_Moromi").text();
    var seigiku = $("#S03001_Seigiku").text();

    Func.ajax({
        type: 'POST',
        url: '/S03001/GetInfoRiceByRiceId',
        data: { riceId: riceId },
        success: function (data) {
            var RiceInfo = data.result;

            if (RiceInfo != null) {
                if (RiceInfo.minTempSeigiku == null) {
                    RiceInfo.minTempSeigiku = "";
                }
                if (RiceInfo.maxTempSeigiku == null) {
                    RiceInfo.maxTempSeigiku = "";
                }
                if (RiceInfo.minTempMoromi == null) {
                    RiceInfo.minTempMoromi = "";
                }
                if (RiceInfo.maxTempMoromi == null) {
                    RiceInfo.maxTempMoromi = "";
                }

                //if (RiceInfo.minTempSeigiku != null && RiceInfo.maxTempSeigiku != null) {
                var htmlSeigiku = '';
                htmlSeigiku += '<div class="row sub-row">';
                htmlSeigiku += '    <div class="col-md-3 col-sm-12">';
                htmlSeigiku += '        <div class="form-group">';
                htmlSeigiku += '            <div class="wapper-input" style="width:100%">';
                htmlSeigiku += '                <input type="text" class="selectTank form-control" value="' + seigiku + '" disabled/>';
                htmlSeigiku += '            </div>';
                htmlSeigiku += '        </div>';
                htmlSeigiku += '    </div>';
                htmlSeigiku += '    <div class="S03001_addMinWidth50 col-md-3 col-sm-12">';
                htmlSeigiku += '        <div class="form-group">';
                htmlSeigiku += '            <div class="">';
                htmlSeigiku += '                <div class="input-group">';
                htmlSeigiku += '                        <input type="text" class="showhideFinish inputnumber form-control" style="text-align:right" aria-describedby="basic-addon1" value="' + RiceInfo.minTempSeigiku + '">';
                htmlSeigiku += '                        <span class="input-group-text" id="basic-addon1" style="background-color: transparent;">°C</span>';
                htmlSeigiku += '                </div>';
                htmlSeigiku += '            </div>';
                htmlSeigiku += '        </div>';
                htmlSeigiku += '    </div>';
                htmlSeigiku += '    <div class="S03001_addMinWidth50 col-md-3 col-sm-12">';
                htmlSeigiku += '        <div class="form-group">';
                htmlSeigiku += '            <div class="">';
                htmlSeigiku += '                <div class="input-group">';
                htmlSeigiku += '                            <input type="text" class="showhideFinish inputnumber form-control" style="text-align:right" aria-describedby="basic-addon1" value="' + RiceInfo.maxTempSeigiku + '">';
                htmlSeigiku += '                    <span class="input-group-text" id="basic-addon1" style="background-color: transparent;">°C</span>';
                htmlSeigiku += '                </div>';
                htmlSeigiku += '            </div>';
                htmlSeigiku += '        </div>';
                htmlSeigiku += '    </div>';
                htmlSeigiku += '</div>';
                $('#iRiceInfo').prepend(htmlSeigiku);

                var htmlMoromi = '';
                htmlMoromi += '<div class="row sub-row">';
                htmlMoromi += '    <div class="col-md-3 col-sm-12">';
                htmlMoromi += '        <div class="form-group">';
                htmlMoromi += '            <div class="wapper-input" style="width:100%">';
                htmlMoromi += '                <input type="text" class="selectTank form-control" value="' + moromi + '" disabled/>';
                htmlMoromi += '            </div>';
                htmlMoromi += '        </div>';
                htmlMoromi += '    </div>';
                htmlMoromi += '    <div class="S03001_addMinWidth50 col-md-3 col-sm-12">';
                htmlMoromi += '        <div class="form-group">';
                htmlMoromi += '            <div class="">';
                htmlMoromi += '                 <div class="input-group">';
                htmlMoromi += '                        <input id="RiceInfo_MinTempMoromi" type="text" class="showhideFinish inputnumber form-control" style="text-align:right" aria-describedby="basic-addon1" value="' + RiceInfo.minTempMoromi + '">';
                htmlMoromi += '                        <span class="input-group-text" id="basic-addon1" style="background-color: transparent;">°C</span>';
                htmlMoromi += '                 </div>';
                htmlMoromi += '            </div>';
                htmlMoromi += '        </div>';
                htmlMoromi += '    </div>';
                htmlMoromi += '    <div class="S03001_addMinWidth50 col-md-3 col-sm-12">';
                htmlMoromi += '        <div class="form-group">';
                htmlMoromi += '             <div class="">';
                htmlMoromi += '                <div class="input-group">';
                htmlMoromi += '                            <input id="RiceInfo_MaxTempMoromi"  type="text" class="showhideFinish inputnumber form-control" style="text-align:right" aria-describedby="basic-addon1" value="' + RiceInfo.maxTempMoromi + '">';
                htmlMoromi += '                            <span class="input-group-text" id="basic-addon1" style="background-color: transparent;">°C</span>';
                htmlMoromi += '                </div>';
                htmlMoromi += '            </div>';
                htmlMoromi += '        </div>';
                htmlMoromi += '    </div>';
                htmlMoromi += '</div>';
                $('#iRiceInfo').prepend(htmlMoromi);
                //}
            }
        }
    });
}

function LoadDataBySeimaibuaiId() {
    var other = $("#S03001_other").text();
    var seimaibuai = $("#S03001_SeimaibuaiID").val();
    if (seimaibuai == "") {
        $("#S03001_RicePolishingRatio").val("");
        $("#S03001_SeimaibuaiValue").val("");
    }
    if (seimaibuai == other) {
        $("#S03001_SeimaibuaiValue").prop("disabled", false);
        $("#S03001_SeimaibuaiValue").val("");
    }
    else {
        $("#S03001_SeimaibuaiValue").prop("disabled", true);
        $("#S03001_SeimaibuaiValue").val(seimaibuai);
    }
}

function LoadInfoTank(data) {
    var nextSlect = $(data).closest('.col-md-2').next();
    var minTemp = nextSlect.find('.form-control');
    var maxTemp = nextSlect.next().find('.form-control');

    minTemp.val($("#RiceInfo_MinTempMoromi").val());
    maxTemp.val($("#RiceInfo_MaxTempMoromi").val());

    var endDate = $(data).closest('.row').find(".EndDateTankFinish");
    endDate.text("");

    var btnFinsh = $(data).closest('.row').find('.btn-secondary');
    if (btnFinsh.length > 0) {
        $(btnFinsh).removeClass('btn btn-secondary btn-sub');
        $(btnFinsh).addClass('btn btn-primary btn-sub');
    }
    var btnRestore = $(data).closest('.row').find(".bi");
    if (btnRestore.length > 0) {
        btnRestore.removeClass('bi bi-arrow-counterclockwise');
        btnRestore.addClass('bi bi-square-fill');
    }
    minTemp.prop('disabled', false);
    maxTemp.prop('disabled', false);
}

function LoadInfoSensor(data) {
    var endDate = $(data).closest('.row').find(".EndDateSensorFinish");
    endDate.text("");

    var btnFinsh = $(data).closest('.row').find('.btn-secondary');
    if (btnFinsh.length > 0) {
        $(btnFinsh).removeClass('btn btn-secondary btn-sub');
        $(btnFinsh).addClass('btn btn-primary btn-sub');
    }
}

function LoadDataByFactoryId() {
    // 選択しているファクトリーを取得
    var factoryId = document.getElementById('S03001_factoryID').value;
    // サーバーのデータを取得
    Func.ajax({
        type: 'GET',
        url: '/S03001/LoadDataByFactoryId',
        data: 'factoryId=' + factoryId,
        async: true,
        cache: false,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {
                LocationDataRender(res.data)
            } else {
                Func.ShowErrorMessageForm(res.message);
            }
        },
        error: function (res) {
            Func.ShowErrorMessageForm(res);
        }
    });
}

function LocationDataRender(data) {

    if (data && data.length > 0) {
        let html = '';
        html += '<option value=""></option>';
        for (let i = 0; i < data.length; i++) {
            // ロット選択の描画
            html += '<option value="' + data[i].id + '">' + data[i].name + '</option>';
        }
        document.getElementById('S03001_locationID').innerHTML = html;
    }
    else {
        let html = '';
        html += '<option value=""></option>';
        document.getElementById('S03001_locationID').innerHTML = html;
    }
}

function isDuplicate(arr) {
    var valueArr = arr.map(function (item) { return item.id });
    let findDuplicates = arr => arr.filter((item, index) => arr.indexOf(item) != index);
    var keyAAA = findDuplicates(valueArr);
    if (keyAAA.length > 0) {
        var result = what => arr.find(x => x.id === what);
        return result(keyAAA[0]);
    } else {
        return 0;
    }

}

function FinishDateLot(id) {
    Func.ClearErrorMessageForm();
    Func.ajax({
        type: 'POST',
        url: '/S03001/FinishDateLot',
        data: { lotId: id },
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {
                ReloadTableFinish();
            } else {
                Func.ShowErrorMessageForm(res.message);
            }
        },
        error: function (res) {
            Func.ShowErrorMessageForm(res);
        }
    });
}

function FinishDateLotContainer(id, type) {
    Func.ClearErrorMessageForm();
    var tblLot = $('#lotTable').DataTable();
    var dataLot = tblLot.data().toArray();
    var rowSelect = dataLot.filter(p => p.id == id);
    var lotId = rowSelect[0].lotId;

    Func.ajax({
        type: 'POST',
        url: '/S03001/FinishDateLotContainer',
        data: { lotContainerId: id, type: type, lotId: lotId },
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {
                ReloadTableFinish();
                if (res.flgEndLot) {
                    const confirmDeleteMsg = $('#S03001_cfm_FinishDateLot').text();
                    UtilDialog.Confirm({
                        type: UtilDialog.TYPE_PRIMARY,
                        message: confirmDeleteMsg,
                        ok: function () {
                            FinishDateLot(lotId);
                        },
                        close: function () {
                            // なし
                        }
                    });
                }

            } else {
                Func.ShowErrorMessageForm(res.message);
            }
        },
        error: function (res) {
            Func.ShowErrorMessageForm(res);
        },
        complete: function () {
        },
    });
}

function FinishDateTank(data) {
    var endDate = $(data).prev();
    var endDateHide = $(data).closest('.col-md-2').next().next();
    var startDate = $(data).closest('.col-md-2').prev();
    var maxTemp = startDate.prev();
    var minTemp = maxTemp.prev();
    var tankCode = minTemp.prev();
    var btnRestore = $(data).find(".bi");

    if (endDate.val() == "") {
        var DateFinish = moment(Date.now()).format('YYYY/MM/DD');
        endDate.val(DateFinish);
        endDateHide.text(DateFinish);
        $(data).removeClass('btn btn-primary btn-sub');
        $(data).addClass('btn btn-secondary btn-sub');

        if (btnRestore.length > 0) {
            btnRestore.removeClass('bi bi-square-fill');
            btnRestore.addClass('bi bi-arrow-counterclockwise');
        }
        minTemp.find('.form-control').prop('disabled', true);
        maxTemp.find('.form-control').prop('disabled', true);
        startDate.find('.datepicker').prop('disabled', true);
        tankCode.find(".select-classic").prop('disabled', true);
    }
    else {
        endDate.val("");
        endDateHide.text("");
        endDate.attr("placeholder", 'YYYY/MM/DD');
        $(data).removeClass('btn btn-secondary btn-sub');
        $(data).addClass('btn btn-primary btn-sub');
        if (btnRestore.length > 0) {
            btnRestore.removeClass('bi bi-arrow-counterclockwise');
            btnRestore.addClass('bi bi-square-fill');
        }
        minTemp.find('.form-control').prop('disabled', false);
        maxTemp.find('.form-control').prop('disabled', false);
        startDate.find('.datepicker').prop('disabled', false);
        tankCode.find(".select-classic").prop('disabled', false);
    }
}

function FinishDateSensor(data) {
    var endDate = $(data).prev();
    var endDateHide = $(data).closest('.col-md-3').next().next();
    var startDate = $(data).closest('.col-md-3').prev();
    var tankCode = $(endDate).closest(".row");
    var btnRestore = $(data).find(".bi");
    if (endDate.val() == "") {
        var DateFinish = moment(Date.now()).format('YYYY/MM/DD');
        endDate.val(DateFinish);
        endDateHide.text(DateFinish);
        $(data).removeClass('btn btn-primary btn-sub');
        $(data).addClass('btn btn-secondary btn-sub');
        if (btnRestore.length > 0) {
            btnRestore.removeClass('bi bi-square-fill');
            btnRestore.addClass('bi bi-arrow-counterclockwise');
        }
        tankCode.find(".select-classic").prop('disabled', true);
        startDate.find('.datepicker').prop('disabled', true);
    }
    else {
        endDate.val("");
        endDateHide.text("");
        endDate.attr("placeholder", 'YYYY/MM/DD');
        $(data).removeClass('btn btn-secondary btn-sub');
        $(data).addClass('btn btn-primary btn-sub');
        if (btnRestore.length > 0) {
            btnRestore.removeClass('bi bi-arrow-counterclockwise');
            btnRestore.addClass('bi bi-square-fill');
        }
        tankCode.find(".select-classic").prop('disabled', false);
        startDate.find('.datepicker').prop('disabled', false);
    }

}

function showhideFinish() {
    var endDate = $("#S03001_endDateID").val();
    var showhide = $(".showhideFinish");
    if (endDate == "") {
        showhide.attr("disabled", false);
        if ($("#S03001_SeimaibuaiID").val() != $("#S03001_other").text()) {
            $("#S03001_SeimaibuaiValue").attr("disabled", true);
        }
    }
    else {
        showhide.attr("disabled", true);
    }
}
function FinishDateLotEdit(data) {
    var endDate = $("#S03001_endDateID").val();
    var btnRestore = $(data).find(".bi");
    var showhide = $(".showhideFinish");
    if (endDate == "") {
        Func.ClearErrorMessageForm();
        const confirmEndDateLot = $("#S03001_cfm_enddatelot").text();
        UtilDialog.Confirm({
            type: UtilDialog.TYPE_PRIMARY,
            message: confirmEndDateLot,
            ok: function () {
                showhide.attr("disabled", true);
                var lstTankNotEnd = $("#selected_tanks").find('.btn-primary');
                var lstSensorNotEnd = $("#selected_sensors").find('.btn-primary');
                if (lstTankNotEnd.length > 0) {
                    for (var item of lstTankNotEnd) {
                        FinishDateTank(item);
                    }
                }
                if (lstSensorNotEnd.length > 0) {
                    for (var item of lstSensorNotEnd) {
                        FinishDateSensor(item);
                    }
                }
                var DateFinish = moment(Date.now()).format('YYYY/MM/DD');
                $("#S03001_endDateID").val(DateFinish);
                $(data).removeClass('btn btn-primary btn-sub');
                $(data).addClass('btn btn-secondary btn-sub');
                if (btnRestore.length > 0) {
                    btnRestore.removeClass('bi bi-square-fill');
                    btnRestore.addClass('bi bi-arrow-counterclockwise');
                }
            },
            close: function () {
                // なし
            }
        });
    }
    else {
        showhide.attr("disabled", false);
        if ($("#S03001_SeimaibuaiID").val() != $("#S03001_other").text()) {
            $("#S03001_SeimaibuaiValue").attr("disabled", true);
        }
        $("#S03001_endDateID").val("");
        $(data).removeClass('btn btn-secondary btn-sub');
        $(data).addClass('btn btn-primary btn-sub');
        if (btnRestore.length > 0) {
            btnRestore.removeClass('bi bi-arrow-counterclockwise');
            btnRestore.addClass('bi bi-square-fill');
        }
    }
}

function FinishDateLotEditSave(data) {
    var endDate = $("#S03001_endDateID").val();
    var showhide = $(".showhideFinish");
    if (endDate == "") {
        showhide.attr("disabled", true);
        var DateFinish = moment(Date.now()).format('YYYY/MM/DD');
        $("#S03001_endDateID").val(DateFinish);
        $(data).removeClass('btn btn-primary btn-sub');
        $(data).addClass('btn btn-secondary btn-sub');
    }
    else {
        showhide.attr("disabled", false);
        if ($("#S03001_SeimaibuaiID").val() != $("#S03001_other").text()) {
            $("#S03001_SeimaibuaiValue").attr("disabled", true);
        }
        $("#S03001_endDateID").val("");
        $(data).removeClass('btn btn-secondary btn-sub');
        $(data).addClass('btn btn-primary btn-sub');
    }

}


function checkLocationIDEmpty(LocationID) {
    var messeger = $("#S03001_messageErr").text();
    var LocationLabel = $("#LabelLocation").text();
    LocationLabel = LocationLabel.replace('（*）：', '');
    if (LocationID == "") {
        var element = document.getElementById("S03001_locationID");
        element.classList.add("input-validation-error");

        $('#LocationError').text(LocationLabel + messeger);
        return 1;
    }
    else {
        $('#LocationError').empty();
        return 0;
    }
}

function getDataTankAndSensor() {
    var data = "";
    var listTank = $('#selected_tanks').find('.select-classic');
    var listTankVal = [];
    for (var item of listTank) {
        var objminTemp = $(item).closest('.col-md-2').next();
        var objmaxTemp = objminTemp.next();
        var objStartDate = objmaxTemp.next();
        var objEndDate = objStartDate.next();
        var minTemp = objminTemp.find('.form-control');
        var maxTemp = objminTemp.next().find('.form-control');
        var startDate = objStartDate.find('.datepicker');
        var endDate = objEndDate.find('.form-control');
        var IdOld = $(item).closest('.row').find('.S03001_TankIdOld');
        var obj =
        {
            id: $(item).val(),
            code: $(item).find('option:selected').text(),
            minTemp: minTemp.val(),
            maxTemp: maxTemp.val(),
            startDate: startDate.val(),
            endDate: endDate.val(),
            idOld: IdOld.text()
        }

        listTankVal.push(obj);
    }

    var listSensor = $('#selected_sensors').find('.select-classic');
    var listSensorVal = [];
    for (var item of listSensor) {
        //var endDate = $(item).closest('.row').find('.EndDateSensorFinish');
        var objStartDate = $(item).closest('.col-md-3').next();
        var objEndDate = objStartDate.next();
        var startDate = objStartDate.find('.datepicker');
        var endDate = objEndDate.find('.form-control');
        var IdOld = $(item).closest('.row').find('.S03001_SensorIdOld');

        var obj =
        {
            id: $(item).val(),
            code: $(item).find('option:selected').text(),
            startDate: startDate.val(),
            endDate: endDate.val(),
            idOld: IdOld.text()
        }
        listSensorVal.push(obj);
    }

    var msgAddTankAndSensor = $("#S03001_msg_add_tanksensor").text();
    var msgDuplicate = $("#S03001_msg_duplicate").text();
    var msgSensorEmpty = $("#S03001_msg_sensor_empty").text();
    var msgTankEmpty = $("#S03001_msg_tank_empty").text();
    var msgCheckMinMax = $("#S03001_msg_checkminmax").text();

    for (var item of listTankVal) {
        if (item.id == "") {
            const warningSelectTank = msgTankEmpty;
            Func.ShowErrorMessageForm(warningSelectTank);
            return data;
        }
        if (parseFloat(item.maxTemp) < parseFloat(item.minTemp)) {
            Func.ShowErrorMessageForm(item.code + msgCheckMinMax);
            return data;
        }

    }

    for (var item of listSensorVal) {
        if (item.id == "") {
            const warningSelectSensor = msgSensorEmpty;
            Func.ShowErrorMessageForm(warningSelectSensor);
            return data;
        }

    }

    if (listSensorVal.length == 0 && listTankVal == 0) {
        const warningAddTankAndSensor = msgAddTankAndSensor;
        Func.ShowErrorMessageForm(warningAddTankAndSensor);
        return data;
    }

    if (isDuplicate(listTankVal) != 0) {
        const warningDuplicate = isDuplicate(listTankVal).code + msgDuplicate;
        Func.ShowErrorMessageForm(warningDuplicate);
        return data;
    }
    if (isDuplicate(listSensorVal) != 0) {
        const warningDuplicate = isDuplicate(listSensorVal).code + msgDuplicate;
        Func.ShowErrorMessageForm(warningDuplicate);
        return data;
    }

    listTankVal.forEach(function (value, i) {
        data += "&TankListId[" + i + "].id=" + value.id
            + "&TankListId[" + i + "].minTemp=" + value.minTemp
            + "&TankListId[" + i + "].maxTemp=" + value.maxTemp
            + "&TankListId[" + i + "].startDate=" + value.startDate
            + "&TankListId[" + i + "].endDate=" + value.endDate
            + "&TankListId[" + i + "].idOld=" + value.idOld
    });
    listSensorVal.forEach(function (value, i) {
        data += "&SensorListId[" + i + "].id=" + value.id
            + "&SensorListId[" + i + "].startDate=" + value.startDate
            + "&SensorListId[" + i + "].endDate=" + value.endDate
            + "&SensorListId[" + i + "].idOld=" + value.idOld
    });
    return data;
}

function checkMinMaxRice() {
    var minTempMoromi = $("#RiceInfo_MinTempMoromi").val();
    var maxTempMoromi = $("#RiceInfo_MaxTempMoromi").val();
    var minTempSeigiku = $("#RiceInfo_MinTempSeigiku").val();
    var maxTempSeigiku = $("#RiceInfo_MaxTempSeigiku").val();
    var moromi = $("#S03001_Moromi").text();
    var seigiku = $("#S03001_Seigiku").text();
    var msgCheckMinMax = $("#S03001_msg_checkminmax").text();

    if (parseFloat(maxTempMoromi) < parseFloat(minTempMoromi)) {
        Func.ShowErrorMessageForm(moromi + msgCheckMinMax);
        return true;
    }
    if (parseFloat(maxTempSeigiku) < parseFloat(minTempSeigiku)) {
        Func.ShowErrorMessageForm(seigiku + msgCheckMinMax);
        return true;
    }
    return false;
}

function setInputFilter(textbox, inputFilter) {
    ["input", "keydown", "keyup", "mousedown", "mouseup", "select", "contextmenu", "drop"].forEach(function (event) {
        if (textbox == null) {
            return;
        }
        textbox.addEventListener(event, function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            } else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            } else {
                this.value = "";
            }
        });
    });
}

setInputFilter(document.getElementById("S03001_RicePolishingRatio"), function (value) {
    return /^-?\d*[.,]?\d*$/.test(value) && (value === "" || value <= 100);
});

function checkInputNumber() {
    var inputNumber = document.getElementsByClassName("inputnumber");
    for (var item of inputNumber) {
        setInputFilter(item, function (value) {
            return /^-?\d*[.,]?\d*$/.test(value);
        });
    }

}

function viewNumber2place() {
    
    $(".inputnumber").each(function () {
        var numberval = $(this).val();
        if (numberval != null && numberval != "") {
            if (numberval.substring(numberval.length - 2) == "00") {
                $(this).val(parseFloat(numberval).toFixed(1));
            }
            else if (numberval.substring(numberval.length - 1) == "0") {
                $(this).val(parseFloat(numberval).toFixed(2));
            }
            else {

            }
        }
    });
    if ($("#S03001_RicePolishingRatio").val() != null && $("#S03001_RicePolishingRatio").val() != "") {
        $("#S03001_RicePolishingRatio").val(parseFloat($("#S03001_RicePolishingRatio").val()).toFixed(1));
    }
    
}

function settingWithByRiceRatio() {
    $("#S03001_KubunID").css({
        'width': ($("#S03001_RicePolishingRatio").closest('.input-group').width() + 'px')
    });
    $("#S03001_SeimaibuaiID").css({
        'width': ($("#S03001_RicePolishingRatio").closest('.input-group').width() + 'px')
    });
    $("#S03001_SeimaibuaiValue").css({
        'width': ($("#S03001_RicePolishingRatio").closest('.input-group').width() + 'px')
    });
}

window.addEventListener('resize', function (event) {
    settingWithByRiceRatio();
}, true);

$('#iRiceInfo').on('DOMSubtreeModified', function () {
    settingWithByRiceRatio();
});

