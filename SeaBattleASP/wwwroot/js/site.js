var topText = ['а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'к', 'л', 'м', 'н', 'о','п','р','с'];
var leftText = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17'];

// функция нанесения надписей слева и справа от полей
// входной параметр - иднетификтор контрола(left или right)
function addTextToPositioning(id) {
    for (var i = 0; i < width; i++) {
        $('#' + id + 'Text').append('<span>' + topText[i] + '</span>');
    }
    for (var i = 0; i < height; i++) {
        $('#' + id + 'TextLeft').append('<span>' + leftText[i] + '</span>');
    }
}

// функция заполняет поле элементами span
// входной параметр - поля(левое или правое)
function emptyCellsToField(field) {
    $(field).empty();
    for (var i = 0; i < width; i++) {
        for (var j = 0; j < height; j++) {
            $(field).append('<span class="cell cellColor" id=cell' + i + j + '  oncontextmenu="blocker(' + i + j + ');return false" ></span>');
        }     
    }
};

$('#reload').click(function () {//по клику создается поле боя
    emptyCellsToField('#leftField');//очищается левое поле

    var len = points.length;
    for (var i = 0; i < len; i++) {
        addShipPart(points[i], '#leftField');
    }
});

function addShipPart(point, field) {
    
    $(field + ' #cell' + point.x + point.y).removeClass('shipColor cellColor').addClass('shipColor');
    $(field + ' #cell' + point.x + point.y).addClass('ship');
};

emptyCellsToField('#leftField');
emptyCellsToField('#rightField');
$('.cell').removeClass('cellColor');
addTextToPositioning('left');
addTextToPositioning('right');

//Открыть модальное окно для добавления кораблей
$("#btnAddShips").click(function () {
    $('#ModalPopUp').modal('show');
})













