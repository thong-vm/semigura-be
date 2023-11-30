var LocalCapacityReferData = [];
$(document).ready(function () {
    LoadDataContainer();
});

function LoadDataContainer() {
    var containerId = $("#TankCode option:selected").val();
    $('#volumeTank').val("");
    $('#depthTank').val("");
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
        url: '/S03006/GetListCapacityRefer',
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
        if (parseFloat(LocalCapacityReferData[i]['depth']) == depthEl.value) {
            document.getElementById('volumeTank').value = parseFloat(LocalCapacityReferData[i]['capacity']);
            return;
        }
    }
}

function CapacityToDepth(capacityEl) {
    document.getElementById('depthTank').value = '';
    for (let i = 0; i < LocalCapacityReferData.length; i++) {
        if (parseFloat(LocalCapacityReferData[i]['capacity']) == capacityEl.value) {
            document.getElementById('depthTank').value = parseFloat(LocalCapacityReferData[i]['depth']);
            return;
        }
    }
}

setInputFilter(document.getElementById("depthTank"), function (value) {
    return /^-?\d*[.,]?\d*$/.test(value);
});

setInputFilter(document.getElementById("volumeTank"), function (value) {
    return /^-?\d*[.,]?\d*$/.test(value);
});

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