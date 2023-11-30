$(document).ready(function () {

    setWidthSlectclassic();

    viewNumber2place();

    InitTable();

    // 編集画面での工程ドロップボックスに設定
    SettingDropdownType();

});

function InitTable() {
    let table = $('#settingTable').DataTable({
        "sAjaxSource": "/S09001/GetDataTable",
        "fnServerParams": function (aaData) {
            const searchName = document.getElementById('Name').value;
            aaData.push({ "name": "Name", "value": searchName });
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
        "stripeClasses": ['stripe-1'],
        "createdRow": function (row, data, index) {
            $('td', row).css({
                'border': '1px solid white'
            });
        },
        "columns": [
            { "data": "name" },
            { "data": "name" },
            { "data": "name" },
            null,
            null,
            null,
            { "data": "name" },
        ],
        'rowsGroup': [0,1,2,6],
        'columnDefs': [{
            'targets': 0,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-center',
            'render': function (data, type, full, meta) {
                return full.no;
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
                return full.name;
            }
        }, {
            'targets': 3,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                return full.typeLabel;
            }
        }, {
            'targets': 4,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-right',
            'render': function (data, type, full, meta) {
                let text = '';
                if (full.tempMin != null) text = parseFloat(full.tempMin).toFixed(1) + "°C";
                return text;
            }
        }, {
            'targets': 5,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-right',
            'render': function (data, type, full, meta) {
                let text = '';
                if (full.tempMax != null) text = parseFloat(full.tempMax).toFixed(1) + "°C";
                return text;
            }
        }, {
            'targets': 6,
            'searchable': true,
            'orderable': true,
            'className': 'dt-body-left',
            'render': function (data, type, full, meta) {
                return full.note;
            }
        }]
    });

    table.columns.adjust().draw();

    // エラー発生時にデフォルトで表示されるメッセージボックスを無効化する。
    $.fn.dataTable.ext.errMode = 'none';

    // errorイベントを受け取るハンドラ（エラーレスポンスの取得はできない。）
    table.on('error.dt', function (e, settings, techNote, message) {
        Func.ErrorHandle(message);
        Func.CheckSessionExpired();
    });

    //table.on('draw.dt', function () {
    //    var pageInfo = $('#settingTable').DataTable().page.info();
    //    table.column(0, { page: 'current' }).nodes().each(function (cell, i) {
    //        cell.innerHTML = i + 1 + pageInfo.start;
    //    });
    //});
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

function SettingDropdownType() {
    // 編集画面での工程ドロップボックスに設定
    var els = document.getElementsByClassName('edit-cache-type');
    if (els && els.length > 0) {
        for (let i = 0; i < els.length; i++) {
            const value = els[i].value;
            const id = els[i].id;

            $('select#' + id).val(value);
        }

        // 臨時アイテムを削除
        while (els.length > 0) {
            els[0].remove();
        }
    }
}


function RemoveRow(el) {
    $(el).closest('.row').remove();
}

function AddRow() {
    let newRow = $('#template_row').clone(true);
    newRow.removeAttr('style');
    newRow.addClass('material-stand-row');
    newRow.appendTo('#temperature-data');

    ResetIdAndNameOfMaterialStandVal();

    // 追加アイテムに検証を適用
    $('#inputForm').removeData("validator");
    $('#inputForm').removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($('#inputForm'));
}

function ResetIdAndNameOfMaterialStandVal() {
    let rows = document.getElementsByClassName('material-stand-row');
    for (let i = 0; i < rows.length; i++) {
        // 工程
        if (rows[i].querySelectorAll('select').length >= 1) {
            rows[i].querySelectorAll('select')[0].id = 'MaterialStandValList_' + i + '__Type';
            rows[i].querySelectorAll('select')[0].name = 'MaterialStandValList[' + i + '].Type';
            let validationEls = rows[i].querySelectorAll('span.field-validation-valid');
            if (validationEls.length >= 1) {
                validationEls[0].setAttribute('data-valmsg-for', 'MaterialStandValList[' + i + '].Type');
            }
        }

        // 最低温度
        if (rows[i].querySelectorAll('input').length >= 1) {
            rows[i].querySelectorAll('input')[0].id = 'MaterialStandValList_' + i + '__TempMin';
            rows[i].querySelectorAll('input')[0].name = 'MaterialStandValList[' + i + '].TempMin';
            let validationEls = rows[i].querySelectorAll('span.field-validation-valid');
            if (validationEls.length >= 2) {
                validationEls[1].setAttribute('data-valmsg-for', 'MaterialStandValList[' + i + '].TempMin');
            }
        }

        // 最大温度
        if (rows[i].querySelectorAll('input').length >= 2) {
            rows[i].querySelectorAll('input')[1].id = 'MaterialStandValList_' + i + '__TempMax';
            rows[i].querySelectorAll('input')[1].name = 'MaterialStandValList[' + i + '].TempMax';
            let validationEls = rows[i].querySelectorAll('span.field-validation-valid');
            if (validationEls.length >= 3) {
                validationEls[2].setAttribute('data-valmsg-for', 'MaterialStandValList[' + i + '].TempMax');
            }
        }
    }
}


//function ChangeRicePolishingRatio() {
//    const ricePolishingRatioType = document.getElementById('RicePolishingRatioType');
//    if (ricePolishingRatioType.value === '99') {
//        document.getElementById('RicePolishingRatioName').removeAttribute("readonly");
//        document.getElementById('RicePolishingRatioName').value = '';
//    } else if (ricePolishingRatioType.value === '') {
//        document.getElementById('RicePolishingRatioName').setAttribute("readonly", "readonly");
//        document.getElementById('RicePolishingRatioName').value = '';
//    } else {
//        document.getElementById('RicePolishingRatioName').setAttribute("readonly", "readonly");
//        document.getElementById('RicePolishingRatioName').value = ricePolishingRatioType.options[ricePolishingRatioType.selectedIndex].innerHTML;;
//    }

//    if ($('.validation-summary-errors li').length > 0) {
//        $("#inputForm").validate();
//        $("#inputForm").valid();
//    }
//}

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

    //$("#inputForm").validate();
    //if ($("#inputForm").valid())
    if (true)
    {
        ResetIdAndNameOfMaterialStandVal();

        const dataType = 'application/x-www-form-urlencoded; charset=utf-8';
        let data = Func.GetValueFormForPost('inputForm');

        Func.ajax({
            type: 'POST',
            url: '/S09001/Save',
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
                url: '/S09001/Delete',
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

}

function setWidthSlectclassic() {
    var width = $("#MaterialStandValList_0__TempMin").width() + $(".input-group-text").width() + 50;
    $(".select-classic").css({
        'width': (width + 'px')
    });
}

window.addEventListener('resize', function (event) {
    setWidthSlectclassic();
}, true);
