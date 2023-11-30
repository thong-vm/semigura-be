var IsFirstLoad = true;

$(document).ready(function () {

    InitTable();
   
    FactoryChanged();
    

});

function InitTable() {
    let table = $('#alertTable').DataTable({
        "sAjaxSource": "/S03005/GetDataTable",
        "fnServerParams": function (aaData) {
            const searchFactory = document.getElementById('FactoryId').value;
            const searchLocation = document.getElementById('LocationId').value;
            const searchContainer = document.getElementById('Container').value;
            const searchStatusOptions = document.getElementById('SearchStatus').options;
            
            let parentId = searchContainer;
            if (!parentId || parentId === '') {
                parentId = searchLocation;
            }

            let index = 0;
            for (var option of searchStatusOptions) {
                if (option.selected) {
                    aaData.push({ "name": "SearchStatus[" + index + "]", "value": option.value });
                    index++;
                }
            }

            aaData.push({ "name": "FactoryId", "value": searchFactory });
            aaData.push({ "name": "LocationId", "value": searchLocation });
            aaData.push({ "name": "Container", "value": searchContainer });
            aaData.push({ "name": "ParentId", "value": parentId });
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
                return full.factory;
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
                return full.location;
            }
        }, {
            'targets': 5,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                return full.type;
            }
        }, {
            'targets': 6,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                let result = full.container;
                if (full.containerType == 2) {
                    result = '';
                }
                return result;
            }
        }, {
            'targets': 7,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                return full.title;
            }
        }, {
            'targets': 8,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-center dt-control',
            'render': function (data, type, full, meta) {
                return '';
            }
        }, {
            'targets': 9,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                return full.level;
            }
        }, {
            'targets': 10,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                return full.status;
            }

        }, {
            'targets': 11,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                var date = moment(full.createdOnUnixTimeStamp).format("YYYY-MM-DD HH:mm:ss").toUpperCase();
                return date;
            }
        }, {
            'targets': 12,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                return full.note;
            }
        }, {
            'width': '8%',
            'targets': 13,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-center',
            'render': function (data, type, full, meta) {
                let button = '<button type="button" class="btn btn-primary btn-sub close_button" onclick="CloseNotification(\'' + full.id + '\')">' + document.getElementById('resource_closebtn').value + '</button>';

                if (full.statusVal !== 3) return button;
                return '';
            }
        }]
    });
    table.on('draw.dt', function () {
        var pageInfo = $('#alertTable').DataTable().page.info();
        table.column(0, { page: 'current' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1 + pageInfo.start;
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

    // Add event listener for opening and closing details
    $('#alertTable tbody').on('click', 'td.dt-control', function () {
        var tr = $(this).closest('tr');
        var row = table.row(tr);
        var idx = table.cell(this).index().column;
        var title = table.column(idx).header().textContent;

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            // Open this row
            row.child(format(row.data(), title)).show();
            tr.addClass('shown');
        }
    });

    
}
window.addEventListener('resize', function (event) {
    if (document.getElementById('is_edit_flg') == null) {
        var table = $('#alertTable').DataTable();
        table.columns.adjust().draw();
    }
}, true);

/* Formatting function for row details - modify as you need */
function format(d, title) {
    // `d` is the original data object for the row
    let arrContent = d.Content.split("\r\n");
    let content = '<td>';
    for (let i = 0; i < arrContent.length; i++) {
        content += arrContent[i] + '</br>';
    }
    content += '</td>';
    return '<table cellpadding="5" cellspacing="0" border="0" class = "childTable">' +
                '<tr>' +
                '<td>' + title + ':</td>' + content +                     
                '</tr>' +       
            '</table>';
}

function ReloadTable() {
    Func.ClearErrorMessageForm();

    var table = $('#alertTable').DataTable();
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
            url: '/S03005/Save',
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
                url: '/S03005/Delete',
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


function CloseNotification(id) {

    Func.ClearErrorMessageForm();

    const confirmCloseMsg = document.getElementById('resource_msg_close_confirm').value;
    UtilDialog.Confirm({
        type: UtilDialog.TYPE_PRIMARY,
        message: confirmCloseMsg,
        ok: function () {
            Func.ajax({
                type: 'POST',
                url: '/S03005/CloseNotification',
                data: 'Id=' + id,
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


function DisplayChange() {

    const type = document.getElementById('Type').value;

    if (type && (type === '1')) {
        document.getElementById('dropdown_type').style.display = 'inline-block';
        document.getElementById('dropdown_container').style.display = 'inline-block';
    } else {
        document.getElementById('dropdown_type').style.display = 'none';
        document.getElementById('dropdown_container').style.display = 'none';
    }

    let idEl = document.getElementById('Id');

    if (!(IsFirstLoad && idEl && idEl.value !== '')) {
        document.getElementById('FactoryId').value = '';
        document.getElementById('LocationId').value = '';
        document.getElementById('ContainerId').value = '';
    }

    IsFirstLoad = false;
}


function FactoryChanged() {

    LoadLocation();
    
}



function LoadLocation() {

    var actionUrl = document.getElementById('list_location_action_url').value;
    var factoryId = document.getElementById('FactoryId').value;

    // 空白オプションを初期化
    const location = document.getElementById('LocationId');
    location.innerHTML = '';

    let option = document.createElement('option');
    option.setAttribute('value', '');
    option.textContent = '';

    location.appendChild(option);


    if (factoryId) {
        const dataType = 'application/x-www-form-urlencoded; charset=utf-8';

        let params = {};
        params["FactoryId"] = factoryId;
        let data = Func.ConcatParamOfForm(params);
        
        Func.ajax({
            type: 'POST',
            url: actionUrl,
            data: data,
            async: true,
            cache: false,
            processData: false,
            contentType: dataType,
            success: function (res) {
                if (res.status) {
                    if (res.data) {

                        for (let i = 0; i < res.data.length; i++) {
                            let option = document.createElement('option');
                            option.setAttribute('value', res.data[i].id);
                            option.textContent = res.data[i].name;

                            location.appendChild(option);
                        }

                        location.value = (IsFirstLoad ? $("#hdLocationId").val() : '');
                        IsFirstLoad = false;

                    }
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

