$(document).ready(function () {
    InitTable();
});

function InitTable() {
    var table = $('#MainTable').DataTable({
        "sAjaxSource": "/S09003/GetDataTable",
        "fnServerParams": function (aaData) {
            const searchTankCode = document.getElementById('Code').value;
            const searchCapacityStart = document.getElementById('CapSearch_Start').value;
            const searchCapacityEnd = document.getElementById('CapSearch_End').value;
            const searchHeightStart = document.getElementById('HeightSearch_Start').value;
            const searchHeightEnd = document.getElementById('HeightSearch_End').value;
            aaData.push({ "name": "Code", "value": searchTankCode });
            aaData.push({ "name": "CapSearch_Start", "value": searchCapacityStart });
            aaData.push({ "name": "CapSearch_End", "value": searchCapacityEnd });
            aaData.push({ "name": "HeightSearch_Start", "value": searchHeightStart });
            aaData.push({ "name": "HeightSearch_End", "value": searchHeightEnd });
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
            }
            ,
            {
                'targets': 1,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    let html = '<span onclick="DirectEdit(\'' + full.id + '\')" class="cursor-pointer" ><i class="bi bi-pencil-square"></i></span>';
                    html += '<span onclick="Delete(\'' + full.id + '\')" class="cursor-pointer" ><i class="bi bi-trash"></i></span>';

                    return html;
                }
            }, {
                'targets': 2,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    return full.factoryName;
                }
            }, {
                'targets': 3,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    return full.locationName;
                }
            },
            {
                'targets': 4,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-left',
                'render': function (data, type, full, meta) {
                    return full.code;
                }
            },
            {
                'targets': 5,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-right',
                'render': function (data, type, full, meta) {
                    return full.capacity;
                }
            },
            {
                'targets': 6,
                'searchable': true,
                'orderable': true,
                'className': 'dt-body-right',
                'render': function (data, type, full, meta) {
                    return full.height;
                }
            },
            {
                'targets': 7,
                'visible': false,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    var Date = moment(full.createdOn).format("YYYY-MM-DD HH:mm:ss").toUpperCase();
                    return Date;
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

    // エラー発生時にデフォルトで表示されるメッセージボックスを無効化する。
    $.fn.dataTable.ext.errMode = 'none';

    table.columns.adjust().draw();

    // errorイベントを受け取るハンドラ（エラーレスポンスの取得はできない。）
    table.on('error.dt', function (e, settings, techNote, message) {
        Func.ErrorHandle(message);
        Func.CheckSessionExpired();
    });
}
//resize table when change gamen size
window.addEventListener('resize', function (event) {
    if (document.getElementById('is_edit_flg') == null) {
        var table = $('#MainTable').DataTable();
        table.columns.adjust().draw();
    }
}, true);

function ReloadTable() {
    Func.ClearErrorMessageForm();

    var table = $('#MainTable').DataTable();
    table.ajax.reload();
}

function DirectEdit(id) {
    //const form = document.createElement('form');
    //form.action = document.getElementById('edit_action_url').value;
    //form.method = 'post';
    //const idEl = document.createElement('input');
    //idEl.setAttribute('type', 'hidden');
    //idEl.setAttribute('name', 'Id');
    //idEl.setAttribute('value', id);
    //form.appendChild(idEl);

    //document.body.appendChild(form);
    //form.submit();

    const href = document.getElementById('edit_action_url').value;
    location.href = href + '?Id=' + id;
}

function Save() {
    Func.ClearErrorMessageForm();
    $("#inputForm").validate();
    if ($("#inputForm").valid()) {
        const dataType = 'application/x-www-form-urlencoded; charset=utf-8';
        let data = Func.GetValueFormForPost('inputForm');
        Func.ajax({
            type: 'POST',
            url: '/S09003/Save',
            data: data,
            async: true,
            cache: false,
            processData: false,
            contentType: dataType,
            success: function (res) {
                if (res.status) {
                    location.href = document.getElementById('main_page_url').value;
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
                url: '/S09003/Delete',
                data: 'id=' + id,
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

function LoadDataByFactoryId() {
    // 選択しているファクトリーを取得
    var factoryId = document.getElementById('FactoryId').value;
    // サーバーのデータを取得
    Func.ajax({
        type: 'GET',
        url: '/S09003/LoadDataByFactoryId',
        data: 'FactoryId=' + factoryId,
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

/*
 * ロケーションのドロップボックスの描画
 */
function LocationDataRender(data) {
    if (data && data.length > 0) {
        let html = '';
        html += '<option value=""></option>';
        for (let i = 0; i < data.length; i++) {
            html += '<option value="' + data[i].id + '">' + data[i].name + '</option>';
        }
        document.getElementById('LocationId').innerHTML = html;
    } else {
        let html = '';
        html += '<option value=""></option>';
        document.getElementById('LocationId').innerHTML = html;
    }
}

    

