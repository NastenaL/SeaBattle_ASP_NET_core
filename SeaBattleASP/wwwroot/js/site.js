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
    for (var i = 0; i < width * height; i++) {
        $(field).append('<span class="cell cellColor" id=cell' + i + '  oncontextmenu="blocker(' + i + ');return false" ></span>');
    }
};


$('#reload').click(function () {//по клику создается поле боя
    emptyCellsToField('#leftField');//очищается левое поле
    
    //"шторы" для блокировки 
    //нажатия во время чужого хода
    $('#leftField').append('<div id="leftShade"></div>');
    $('#rightField').append('<div id="rightShade"></div>');
    for (lengthShip = 4; lengthShip >= 1; lengthShip--) {
        for (count = (5 - lengthShip); count >= 1; count--) {
            //получается массив координат частей корабля
            var position = randomizer(lengthShip, '#leftField');

            //в цикле вызывается функция для размещения кораблей по частям
            for (var cells = 0; cells < position.length; cells++) {
                addShipPart(position[cells], '#leftField');
            }
        }
    }
});

// функция расстановки частей кораблей
// получает на вход номер ячейки и id поля
function addShipPart(numberCell, field) {
    // массив ближайших ячеек для закрашивания водой
    var arrAround = [numberCell - 1, numberCell - 10, numberCell + 10, numberCell - 1 - 10, numberCell - 1 + 10, numberCell + 1, numberCell + 1 - 10, numberCell + 1 + 10];
    var lengthArrAround;
    if (Math.floor(numberCell % 10) < 9) lengthArrAround = arrAround.length;
    else lengthArrAround = arrAround.length - 3;//если справого края поля
    for (var i = 0; i < lengthArrAround; i++) {
        var cell = $(field + ' #cell' + arrAround[i]);
        cell.hasClass('shipColor') ? 0 : cell.removeClass('shipColor cellColor').addClass('emptyColor');
    }
    $(field + ' #cell' + numberCell).removeClass('shipColor cellColor').addClass('shipColor');
    $(field + ' #cell' + numberCell).addClass('ship');
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













