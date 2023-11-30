var IsFirstLoad = true;

$(document).ready(function () {
    console.log("S09004")
    InitTable();

    DisplayChange();
});

function InitTable() {
    let table = $('#settingTable').DataTable({
        "sAjaxSource": "/S09004/GetDataTable",
        "fnServerParams": function (aaData) {
            const searchCode = document.getElementById('Code').value;
            const searchName = document.getElementById('Name').value;
            const searchType = document.getElementById('Type').value;
            const searchFactory = document.getElementById('FactoryId').value;
            const searchContainer = document.getElementById('ContainerId').value;
            const searchLocation = document.getElementById('LocationId').value;
            const searchIsNotUsed = document.getElementById('IsNotUsed').checked;
            let parentId = searchContainer;
            if (!parentId || parentId === '') {
                parentId = searchLocation;
            }

            aaData.push({ "name": "Code", "value": searchCode });
            aaData.push({ "name": "Name", "value": searchName });
            aaData.push({ "name": "Type", "value": searchType });
            aaData.push({ "name": "FactoryId", "value": searchFactory });
            aaData.push({ "name": "ParentId", "value": parentId });
            aaData.push({ "name": "IsNotUsed", "value": searchIsNotUsed });
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
                return full.code;
            }
        }, {
            'targets': 3,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                return full.name;
            }
        }, {
            'targets': 4,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                return full.typeLabel;
            }
        }, {
            'targets': 5,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                return full.factoryName;
            }
        }, {
            'targets': 6,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                return full.parentName;
            }
        }]
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
    //history.go(-1);
    location.href = document.getElementById('main_page_url').value;
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
            url: '/S09004/Save',
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
                url: '/S09004/Delete',
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


function DisplayChange() {
    const type = document.getElementById('Type').value;

    if (type && (type === '1' || type === '4')) {
        document.getElementById('dropdown_factory').style.display = 'inline-block';
        document.getElementById('dropdown_tank').style.display = 'inline-block';
        document.getElementById('dropdown_location').style.display = 'none';
    } else if (type && (type === '3')) {
        document.getElementById('dropdown_factory').style.display = 'inline-block';
        document.getElementById('dropdown_tank').style.display = 'none';
        document.getElementById('dropdown_location').style.display = 'inline-block';
    } else {
        document.getElementById('dropdown_factory').style.display = 'none';
        document.getElementById('dropdown_tank').style.display = 'none';
        document.getElementById('dropdown_location').style.display = 'none';
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
    if (document.getElementById('dropdown_tank').style.display !== 'none') {
        LoadContainer();
    } else if (document.getElementById('dropdown_location').style.display !== 'none') {
        LoadLocation();
    }
}

function LoadContainer() {
    var actionUrl = document.getElementById('list_container_action_url').value;
    var factoryId = document.getElementById('FactoryId').value;

    // 空白オプションを初期化
    const container = document.getElementById('ContainerId');
    container.innerHTML = '';

    let option = document.createElement('option');
    option.setAttribute('value', '');
    option.textContent = '';

    container.appendChild(option);

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
                            option.textContent = res.data[i].code;

                            container.appendChild(option);
                        }
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





