var Func = {};

Func.ajax = function (obj, noSpinnerFlg) {
    try {
        let complete = obj.complete;
        let success = obj.success;

        // completeイベント処理定義
        let _complete = function (jqxhr) {

            if (!noSpinnerFlg) {
                UtilSpinner.Hide();
            }

            if (complete) {
                complete(jqxhr);
            }

            Func.CheckSessionExpired(jqxhr);
        }

        // successイベント処理定義
        let _success = function (res) {
            try {
                if (success) {
                    success(res);
                }
            } catch (e) {
                //例外エラーが起きた時に実行する処理
                Func.ErrorHandle(e);
            }
        }

        obj.complete = _complete;
        obj.success = _success;

        if (!noSpinnerFlg) {
            UtilSpinner.Show();
        }

        $.ajax(obj);
    } catch (e) {
        //例外エラーが起きた時に実行する処理
        Func.ErrorHandle(e);
    }
}

Func.escape_html = function (string) {
    try {
        if (typeof string !== 'string') {
            return string;
        }
        return string.replace(/[&'`"<>]/g, function (match) {
            return {
                '&': '&amp;',
                "'": '&#x27;',
                '`': '&#x60;',
                '"': '&quot;',
                '<': '&lt;',
                '>': '&gt;',
            }[match]
        });
    } catch (e) {
        //例外エラーが起きた時に実行する処理
        Func.ErrorHandle(e);
    }
}

var UtilSpinnerRefresh = {
    Show: function () {
        try {
            const spinner = document.getElementById("spinner_refresh");
            if (spinner) spinner.style.display = 'block';
        } catch (e) {
            //例外エラーが起きた時に実行する処理
            Func.ErrorHandle(e);
        }
    },

    Hide: function () {
        try {
            const spinner = document.getElementById("spinner_refresh");
            if (spinner) spinner.style.display = 'none';
        } catch (e) {
            //例外エラーが起きた時に実行する処理
            Func.ErrorHandle(e);
        }
    }
}

var UtilSpinner = {
    Show: function () {
        try {
            const spinner = document.getElementById("spinner");
            if (!spinner) {
                $('body').append("<div class='loading' id='spinner'>" +
                    "<div class='spinner-background'></div>" +
                    "<div class='spinner-center'>" +
                    "<div class='spinner-border' role='status'><span class='sr-only'>Loading...</span></div>" +
                    "</div>" +
                    "</div>");
            }

            document.getElementById("spinner").style.display = 'block';
        } catch (e) {
            //例外エラーが起きた時に実行する処理
            Func.ErrorHandle(e);
        }
    },

    Hide: function () {
        try {
            const spinner = document.getElementById("spinner");
            if (spinner) spinner.style.display = 'none';
        } catch (e) {
            //例外エラーが起きた時に実行する処理
            Func.ErrorHandle(e);
        }
    }
}

var UtilAlert = {
    TYPE_SUCCESS: 'alert-success',
    TYPE_WARNING: 'alert-warning',
    TYPE_DANGER: 'alert-danger',
    Show: function (inpObj) {
        try {
            if (!inpObj.type) {
                inpObj.type = '';
            }

            if (!inpObj.message) {
                inpObj.message = '';
            }

            let html = '';
            html += '<div class="alert ' + inpObj.type + ' alert-dismissible fade show" role="alert" style="display: none;">';
            html += '    <span class="notification_title">' + inpObj.message + '</span>';
            html += '    <button type="button" class="close" data-dismiss="alert" aria-label="Close">';
            html += '        <span aria-hidden="true">&times;</span>';
            html += '    </button>';
            html += '</div>';

            let alert = $(html);
            $('#wapper_alert').append($(alert));

            $(alert).fadeTo(2000, 500).slideUp(500, function () {
                $(this).remove();
            });
        } catch (e) {
            //例外エラーが起きた時に実行する処理
            Func.ErrorHandle(e);
        }
    }
}

var UtilDialog = {
    TYPE_DEFAULT : '',
    TYPE_INFO: 'bg-info',
    TYPE_PRIMARY: 'bg-primary',
    TYPE_SUCCESS: 'bg-success',
    TYPE_WARNING: 'bg-warning',
    TYPE_DANGER: 'bg-danger',

    /*Confirm: function (type, message, okCallback, closeCallback) {*/
    Confirm: function (inpObj) {
        try {
            const dialog = document.getElementById("ConfirmDialogId");
            let headerStyle = 'color: white';

            if (!inpObj.type) {
                inpObj.type = '';
                headerStyle = '';
            }

            if (!inpObj.message) {
                inpObj.message = '';
            }

            if (dialog) {
                dialog.parentNode.removeChild(dialog);
            }

            const title = document.getElementById('resource_dialog_confirm_title').value;
            const okbtn = document.getElementById('resource_dialog_confirm_okbtn').value;
            const closebtn = document.getElementById('resource_dialog_confirm_closebtn').value;

            $('body').append(
                '<div id="ConfirmDialogId" class="modal" tabindex="-1" role="dialog">' +
                '  <div class="modal-dialog" role="document">' +
                '    <div class="modal-content">' +
                '      <div class="modal-header ' + inpObj.type + '">' +
                '        <h5 class="modal-title" style="' + headerStyle + '">' + title + '</h5 > ' +
                '        <button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
                '          <span aria-hidden="true">&times;</span > ' +
                '        </button>' +
                '      </div>' +
                '      <div class="modal-body" style="font-size: medium; white-space: pre-wrap;">' +
                '        <p>' + inpObj.message + '</p > ' +
                '      </div>' +
                '      <div class="modal-footer">' +
                '        <button type="button" class="btn btn-primary">' + okbtn + '</button > ' +
                '        <button type="button" class="btn btn-secondary" data-dismiss="modal">' + closebtn + '</button > ' +
                '      </div>' +
                '    </div>' +
                '  </div>' +
                '</div>');

            $('#ConfirmDialogId').modal('show');
            $('#ConfirmDialogId').on('hidden.bs.modal', function (e) {
                //なし
            });

            $('#ConfirmDialogId .btn-secondary').click(function () {
                if (inpObj.close) {
                    inpObj.close();
                }
            });

            $('#ConfirmDialogId .btn-primary').click(function () {
                $('#ConfirmDialogId').modal('hide');

                if (inpObj.ok) {
                    inpObj.ok();
                }
            });
        } catch (e) {
            //例外エラーが起きた時に実行する処理
            Func.ErrorHandle(e);
        }
    },

    Alert: function (inpObj) {
        try {
            const dialog = document.getElementById("AlertDialogId");

            if (!inpObj.type) {
                inpObj.type = '';
            }

            if (!inpObj.message) {
                inpObj.message = '';
            }

            if (dialog) {
                dialog.parentNode.removeChild(dialog);
            }

            const title = document.getElementById('resource_dialog_alert_title').value;
            const closebtn = document.getElementById('resource_dialog_alert_closebtn').value;

            $('body').append(
                '<div id="AlertDialogId" class="modal" tabindex="-1" role="dialog">' +
                '  <div class="modal-dialog" role="document">' +
                '    <div class="modal-content">' +
                '      <div class="modal-header ' + inpObj.type + '">' +
                '        <h5 class="modal-title">' + title + '</h5 > ' +
                '        <button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
                '          <span aria-hidden="true">&times;</span > ' +
                '        </button>' +
                '      </div>' +
                '      <div class="modal-body">' +
                '        <p>' + inpObj.message + '</p > ' +
                '      </div>' +
                '      <div class="modal-footer">' +
                '        <button type="button" class="btn btn-secondary" data-dismiss="modal">' + closebtn + '</button > ' +
                '      </div>' +
                '    </div>' +
                '  </div>' +
                '</div>');

            $('#AlertDialogId').modal('show');
            $('#AlertDialogId').on('hidden.bs.modal', function (e) {
                if (inpObj.close) {
                    inpObj.close();
                }
            });

            $('#AlertDialogId .btn-secondary').click(function () {
                if (inpObj.close) {
                    inpObj.close();
                }
            });
        } catch (e) {
            //例外エラーが起きた時に実行する処理
            Func.ErrorHandle(e);
        }
    }
}





function ResizeColumn() {
    try {
        $($.fn.dataTable.tables(true)).DataTable().columns.adjust();
    } catch (e) {
        //例外エラーが起きた時に実行する処理
        Func.ErrorHandle(e);
    }
}


// フォームのエラーメッセージが表示
Func.ShowErrorMessageForm = function (message, noScrollIntoView) {
    try {
        let item = document.getElementsByClassName('validation-summary-valid');
        item = item && item.length > 0 ? item : document.getElementsByClassName('validation-summary-errors');
    
        if (item && item.length > 0 && message) {
            const ulEl = document.createElement('ul');
            const liEl = document.createElement('li');
            liEl.innerHTML = message;
            ulEl.append(liEl);
            item[0].innerHTML = '';
            item[0].append(ulEl);

            if (!noScrollIntoView) {
                item[0].scrollIntoView();
            }
        }
    } catch (e) {
        //例外エラーが起きた時に実行する処理
        Func.ErrorHandle(e);
    }
}

// フォームのエラーメッセージがクリア
Func.ClearErrorMessageForm = function () {
    try {
        let item = document.getElementsByClassName('validation-summary-valid');
        item = item && item.length > 0 ? item : document.getElementsByClassName('validation-summary-errors');
        if (item && item.length > 0) {
            const ulEl = document.createElement('ul');
            item[0].innerHTML = '';
            item[0].append(ulEl);
        }
    } catch (e) {
        //例外エラーが起きた時に実行する処理
        Func.ErrorHandle(e);
    }
}

Func.ScrollIntoViewError = function () {
    try {
        let item = document.getElementsByClassName('validation-summary-valid');
        item = item && item.length > 0 ? item : document.getElementsByClassName('validation-summary-errors');
        if (item && item.length > 0) {
            item[0].scrollIntoView();
        }
    } catch (e) {
        //例外エラーが起きた時に実行する処理
        Func.ErrorHandle(e);
    }
}

// フォームのデータを取得
Func.GetValueFormForPost = function (formId) {
    try {
        let params = {};
        let result = '';
        // input
        let items = $('#' + formId + ' input[type="text"],#' + formId + ' input[type="hidden"],#' + formId + ' input[type="password"],#' + formId + ' input[type="datetime-local"],#' + formId + ' select, #' + formId + ' textarea');
        for (let i = 0; i < items.length; i++) {
            params[items[i].name] = items[i].value;
        }

        // checkbox
        items = $('#' + formId + ' input[type="checkbox"]');
        for (let i = 0; i < items.length; i++) {
            params[items[i].name] = items[i].checked;
        }

        // ユーザーのTimezoneOffsetを追加
        var timezoneOffset = new Date().getTimezoneOffset();
        params['ClientTimezoneOffset'] = timezoneOffset;
    
        result = Func.ConcatParamOfForm(params);

        return result;
    } catch (e) {
        //例外エラーが起きた時に実行する処理
        Func.ErrorHandle(e);
    }
}

// フォームのデータを取得
Func.ConcatParamOfForm = function (params) {
    try {
        let result = '';
   
        if (params) {
            for (let key in params) {
                if (result !== '') {
                    result += '&';
                }
                result += key + '=' + params[key];
            }
        }

        return result;
    } catch (e) {
        //例外エラーが起きた時に実行する処理
        Func.ErrorHandle(e);
    }
}

Func.CheckSessionExpired = function (jqxhr) {
    try {
        let showLoginForm = false;
        if (jqxhr) {
            var parser = new DOMParser();
            var doc = parser.parseFromString(jqxhr.responseText, 'text/html');
            if (doc.getElementById('is_login_page') != null) {
                showLoginForm = true;
            }
        } else {
            Func.ajax({
                type: 'GET',
                url: '/S01001/index',
                async: true,
                cache: false,
            }, true);
        }

        if (showLoginForm) {
            // ログインフォームを表示
            $('#login_modal').modal('show');

            // エラーメッセージをクリア
            try {
                let item = document.getElementsByClassName('login-validation-summary-errors');
                if (item && item.length > 0) {
                    const ulEl = document.createElement('ul');
                    item[0].innerHTML = '';
                    item[0].append(ulEl);
                }
            } catch (e) {
                //例外エラーが起きた時に実行する処理
                Func.ErrorHandle(e);
            }
        }
        
    } catch (e) {
        //例外エラーが起きた時に実行する処理
        Func.ErrorHandle(e);
    }
}

Func.ErrorHandle = function (e) {
    //例外エラーがおきたら、コンソールにログを出力する
    console.error("エラー：", e);
}
