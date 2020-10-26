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

function selectShip(id) {
    $.ajax({
        type: 'POST',
        url: '/Game/AddShipToField',
        data: { id: id },
        success: function (points) {
            var convertedPoints = getCellPoint(points);
            paintShip(convertedPoints);
        },
    });
};

function getCellPoint(points) {
    var convertedPoints = [];
    var cell = points.coord;
    for (var i = 0; i < points.coord.length; i++) {
        convertedPoints[i] = {};
        [convertedPoints[i].x, convertedPoints[i].y] = cell[i].coordinate.split(',').filter(c => c !== '').map(c => Number(c));
    }
    return convertedPoints;
}

function paintShip(convertedPoints) {
    var len = convertedPoints.length;
    for (var i = 0; i < len; i++) {
        paintDeckShip(convertedPoints[i], '#leftField');
    }
}

function paintDeckShip(point, field) {
    
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
