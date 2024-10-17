document.querySelector('#type').value = ''
const label = []
const listTags = []
function changeOperacoes(x) {
    if (label.find(o => o == x.value) != x.value) {
        label.push(x.value)
        document.querySelector(`input[value="${x.value}"]`).setAttribute('checked', true)
    }
    else {
        for (var i = 0; i < label.length; i++) {
            if (label[i] === x.value) {
                label.splice(i, 1)
                document.querySelector(`input[value="${x.value}"]`).removeAttribute('checked')
                i--
            }
        }
    }
    const tags = label.map((level, i) => {
        function switchResult(a) {
            switch (a) {
                case 'Create': return 'Inclusão';
                case 'Update': return 'Atualização';
                case 'Delete': return 'Exclusão';
                default: return '';
            }
            return a
        }
        return switchResult(level)
    })
    listTags.push(tags)
    $('input[data-bs-toggle="dropdown"]').val(tags)
}

function getDateInicial() {
    let d = new Date();
    d.setDate(d.getDate() - 7);
    let date = d.getFullYear() + '-' +
        (d.getMonth() + 1).toString().padStart(2, '0') + '-' +
        d.getDate().toString().padStart(2, '0');
    return date;
}

document.addEventListener('DOMContentLoaded', (event) => {
    document.getElementById('dtInicial').value = getDateInicial();
});

window.onload = function () {

    exibirFiltro('filtroLog', this)

    const param = window.location.search.split('&')
    param.map(function (item, index) {
        const pathname = item.replace('?', '').split('=')[0]
        if (pathname == 'type') {
            const value = decodeURI(item.split('=')[1]).replace('%2C', ',').replace('%2C', ',')
            value.split(',').map(function (i, index) {
                switch (i) {
                    case 'Inclusão': return changeCheckbox("Create");
                    case 'Atualização': return changeCheckbox("Update");
                    case 'Exclusão': return changeCheckbox("Delete");
                    default: return '';
                }
            })
            $('input[id="type"]').val(value)
        }

        if (pathname == 'dtFinal') {
            const oldDtFinal = decodeURI(item.split('=')[1]).split('-')

            const dtFinal = oldDtFinal[0] + "-" + oldDtFinal[1] + "-" + oldDtFinal[2];

            $('input[id="dtFinal"]').val(dtFinal)
        }

        if (pathname == 'dtInicial') {
            const oldDtInicial = decodeURI(item.split('=')[1]).split('-')

            const dtInicial = oldDtInicial[0] + "-" + oldDtInicial[1] + "-" + oldDtInicial[2];

            $('input[id="dtInicial"]').val(dtInicial)
        }
    })

    function changeCheckbox(value) {
        document.querySelector(`input[value="${value}"]`).setAttribute('checked', true)
        label.push(value)
    }
}

function getDateNow() {
    let today = new Date();
    let date = today.getFullYear() + '-' +
        (today.getMonth() + 1).toString().padStart(2, '0') + '-' +
        today.getDate().toString().padStart(2, '0');
    return date;
}

document.addEventListener('DOMContentLoaded', (event) => {
    document.getElementById('dtFinal').value = getDateNow();
});

function sortTable(colNo, tableId, IsAsc) {
    var table, rows, switching, i, x, y;
    table = document.getElementById(tableId);
    switching = true;
    while (switching) {
        switching = false;
        rows = table.rows;
        for (i = 1; i < (rows.length - 1); i++) {
            if (rows[i].getElementsByTagName("TD")[colNo] != undefined && rows[i + 1].getElementsByTagName("TD")[colNo] != undefined) {
                x = rows[i].getElementsByTagName("TD")[colNo].innerHTML.toLowerCase();
                y = rows[i + 1].getElementsByTagName("TD")[colNo].innerHTML.toLowerCase();
                if ((x < y && IsAsc == false) || (x > y && IsAsc == true)) {
                    rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
                    switching = true;
                    break;
                }
            }
        }
    }
}

function exibirFiltro(it, elem) {
    var elems = document.getElementsByClassName("cb");
    var currentState = elem.checked;
    var elemsLength = elems.length;

    for (i = 0; i < elemsLength; i++) {
        if (elems[i].type === "checkbox") {
            elems[i].checked = false;
        }
    }
    elem.checked = currentState;
    var elements = document.getElementsByClassName('ocult');
    for (j = 0; j < elements.length; j++) {
        if (elements[j].id != it || currentState == false) {
            elements[j].style.display = "none";
        } else {
            elements[j].style.display = "block";
        }
    }
}