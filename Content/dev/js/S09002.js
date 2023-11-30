$(document).ready(function () {
    InitTable();
});

function InitTable() {
    let table = $('#settingTable').DataTable({
        "sAjaxSource": "/S09002/GetDataTable",
        "fnServerParams": function (aaData) {
            const searchEmail = document.getElementById('Email').value;
            aaData.push({ "name": "Email", "value": searchEmail });
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
        "bPaginate": true,
        'columnDefs': [{
            'targets': 0,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-center',
            'render': function (data, type, full, meta) {
                return 0;
            }
        }, {
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
                return full.email;
            }
        }]
        //{
        //    'targets': 3,
        //    'searchable': true,
        //    'orderable': true,
        //    'className': 'dt-body-center',
        //    'render': function (data, type, full, meta) {
        //        if (full.IsSender) {
        //            return '<input type="checkbox" class="form-control align-center" checked="true" onclick="return false;" />';
        //        }
        //        return '<input type="checkbox" class="form-control align-center" onclick="return false;" />';
        //    }
        //}
    });

    // 自動カウンター列を追加
    table.on('draw.dt', function () {
        var pageInfo = $('#settingTable').DataTable().page.info();
        table.column(0, { page: 'current' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1 + pageInfo.start;
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
        var table = $('#settingTable').DataTable();
        table.columns.adjust().draw();
    }
}, true);

function ReloadTable() {
    Func.ClearErrorMessageForm();

    var table = $('#settingTable').DataTable();
    table.ajax.reload();
}

function Back() {
    history.go(-1);
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
    $("#alertemail-form").validate();
    if ($("#alertemail-form").valid()) {
        const dataType = 'application/x-www-form-urlencoded; charset=utf-8';
        let data = Func.GetValueFormForPost('alertemail-form');

        Func.ajax({
            type: 'POST',
            url: '/S09002/Save',
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
                url: '/S09002/Delete',
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


