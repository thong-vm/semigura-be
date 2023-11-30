var LocalCapacityReferData = [];
$(document).ready(function () {
    InitTable();
});

function InitTable() {
    var table = $('#MainTable').DataTable({
        "sAjaxSource": "/S03002/GetDataTable",
        "fnServerParams": function (aaData) {
            const searchFactory = document.getElementById('FactoryId').value;
            const searchLot = document.getElementById('LotId').value;
            const searchLocation = document.getElementById('LocationId').value;
            const searchTank = document.getElementById('Code').value;
            const searchInUse = document.getElementById('IsInUse').checked;
            aaData.push({ "name": "FactoryId", "value": searchFactory });
            aaData.push({ "name": "LotId", "value": searchLot });
            aaData.push({ "name": "LocationId", "value": searchLocation });
            aaData.push({ "name": "Code", "value": searchTank });
            aaData.push({ "name": "IsInUse", "value": searchInUse });
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
        "scrollY": "500px",
        "scrollCollapse": true,
        "searching": false,
        "bLengthChange": false,
        "bSort": false,
        "bInfo": false,
        "pageLength": 10,
        "bPaginate": true,
        'columnDefs': [
            {
                'targets': 0,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    return 0;
                }
            },
            {
                'targets': 1,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    let html = '<span class="table-button" onclick="ShowCapacityConverter(\'' + full.tankId + '\', \'' + full.tankCode + '\')"><i class="bi bi-moisture"></i ></span >';
                    return html;
                }
            }, {
                'targets': 2,
                'visible': true,
                'render': function (data, type, full, meta) {
                    return full.tankCode;
                }
            }, {
                'targets': 3,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    return full.lotCode;
                }
            }, {
                'targets': 4,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    return full.factoryName;
                }
            },
            {
                'targets': 5,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    return full.locationName;
                }
            },
            {
                'targets': 6,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-right',
                'render': function (data, type, full, meta) {
                    return full.capacity;
                }
            },
            {
                'targets': 7,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-right',
                'render': function (data, type, full, meta) {
                    return full.height;
                }
            },
            {
                'targets': 8,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    if (full.startDateUnixTimeStamp == null) return "";
                    var date = moment(full.startDateUnixTimeStamp).format("YYYY-MM-DD").toUpperCase();
                    return date;
                }
            },
            {
                'targets': 9,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    if (full.endDateUnixTimeStamp == null) return "";
                    var date = moment(full.endDateUnixTimeStamp).format("YYYY-MM-DD").toUpperCase();
                    return date;
                }
            },
            {
                'targets': 10,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    return full.rice;
                }
            },
            {
                'targets': 11,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-right',
                'render': function (data, type, full, meta) {
                    return full.tempMin;
                }
            },
            {
                'targets': 12,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-right',
                'render': function (data, type, full, meta) {
                    return full.tempMax;
                }
            },
            {
                'targets': 13,
                'visible': false,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    return full.note;
                }
            }]
    });

    // add auto counter column
    table.on('draw.dt', function () {
        var PageInfo = $('#MainTable').DataTable().page.info();
        table.column(0, { page: 'current' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1 + PageInfo.start;
        });
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
//resize table when change gamen size
window.addEventListener('resize', function (event) {
    var table = $('#MainTable').DataTable();
    table.columns.adjust().draw();
}, true);

function ReloadTable() {
    Func.ClearErrorMessageForm();

    var table = $('#MainTable').DataTable();
    table.ajax.reload();
}

function LoadDataByFactoryId() {
    // 選択しているファクトリーを取得
    var factoryId = document.getElementById('FactoryId').value;
    // サーバーのデータを取得
    Func.ajax({
        type: 'GET',
        url: '/S03002/LoadDataByFactoryId',
        data: 'FactoryId=' + factoryId,
        async: true,
        cache: false,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {
                LocationDataRender(res.data.listLocation)
                LotDataRender(res.data.listLot);
            } else {
                Func.ShowErrorMessageForm(res.message);
            }
        },
        error: function (res) {
            Func.ShowErrorMessageForm(res);
        }
    });
}

/*
 * ロット
 */
function LotDataRender(data) {
    let html = '';
    html += '<option value=""></option>';
    if (data && data.length > 0) {
        for (let i = 0; i < data.length; i++) {
            html += '<option value="' + data[i].id + '">' + data[i].code + '</option>';
        }
    }
    document.getElementById('LotId').innerHTML = html;
}
/*
 * ロケーション
 */
function LocationDataRender(data) {
    let html = '';
    html += '<option value=""></option>';
    if (data && data.length > 0) {
        for (let i = 0; i < data.length; i++) {
            html += '<option value="' + data[i].id + '">' + data[i].name + '</option>';
        }
    }
    document.getElementById('LocationId').innerHTML = html;
}

function ShowCapacityConverter(containerId, containerCode) {
    $("#TankCode").text(containerCode);
    $('#volumeTank').val("");
    $('#depthTank').val("");
    $('#capacity_modal').modal('show');
    DisableBehind($('#inp-depth'), $('#inp-capacity'));
    LoadCapacityData(containerId);
}

function SwapElement() {
    $('#volumeTank').val("");
    $('#depthTank').val("");
    SubSwapElement($('#inp-depth'), $('#inp-capacity'));
}

function SubSwapElement(a, b) {
    var aNext = $('<div>').insertAfter(a);
    a.insertAfter(b);
    b.insertBefore(aNext);
    aNext.remove();
    DisableBehind(a, b);
}

function DisableBehind(a, b) {
    if (a.prevAll(b).length != 0) {
        a[0].childNodes[1].disabled = true;
    }
    else {
        a[0].childNodes[1].disabled = false;
    }
    if (b.prevAll(a).length != 0) {
        b[0].childNodes[1].disabled = true;
    }
    else {
        b[0].childNodes[1].disabled = false;
    }
}

function LoadCapacityData(containerId) {
    LocalCapacityReferData = [];
    // サーバーのデータを取得
    Func.ajax({
        type: 'GET',
        url: '/S03002/GetListCapacityRefer',
        data: 'ContainerId=' + containerId,
        async: true,
        cache: false,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (res) {
            if (res.status) {
                LocalCapacityReferData = res.data;
            } else {
                Func.ShowErrorMessageForm(res.message);
            }
        },
        error: function (res) {
            Func.ShowErrorMessageForm(res);
        }
    });
}

function DepthToCapacity(depthEl) {
    document.getElementById('volumeTank').value = '';
    for (let i = 0; i < LocalCapacityReferData.length; i++) {
        if (parseFloat(LocalCapacityReferData[i].depth) == depthEl.value) {
            document.getElementById('volumeTank').value = parseFloat(LocalCapacityReferData[i].capacity);
            return;
        }
    }
}

function CapacityToDepth(capacityEl) {
    document.getElementById('depthTank').value = '';
    for (let i = 0; i < LocalCapacityReferData.length; i++) {
        if (parseFloat(LocalCapacityReferData[i].capacity) == capacityEl.value) {
            document.getElementById('depthTank').value = parseFloat(LocalCapacityReferData[i].depth);
            return;
        }
    }
}