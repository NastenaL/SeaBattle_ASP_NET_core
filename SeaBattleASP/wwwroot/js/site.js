var topText = ['а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'к', 'л', 'м', 'н', 'о','п','р','с'];
var leftText = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17'];
var addedShips = new Array();

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

function getCursorCoordinate(x,y) {
    console.log("x = ", x);
    console.log("y = ", y);

    $.ajax({
        type: 'POST',
        url: '/Game/SelectShip',
        data: { x: x, y:y },
        success: function (result) {
           
        },
    });
}


// функция заполняет поле элементами span
// входной параметр - поля(левое или правое)
function emptyCellsToField(field) {
    $(field).empty();
    for (var i = 0; i < width; i++) {
        for (var j = 0; j < height; j++) {
            $(field).append('<span onclick="return getCursorCoordinate('+i+','+j+')" class="cell cellColor" id=cell' + i + j + '  oncontextmenu="blocker(' + i + j + ');return false" ></span>');
        }     
    }
};

function selectShip(id) {
    var element = document.getElementById(id);
    element.style.display = 'none';

    $.ajax({
        type: 'POST',
        url: '/Game/AddShipToField',
        data: { id: id },
        success: function (mapModel) {
            var convertedPoints = getCellPoint(mapModel);

            addedShips.push(mapModel.selectedShip);
            paintShip(convertedPoints);

            var html = "<table>";
                html += "<tr>";
                html += "<td>Id</td>";
                html += "<td>Range</td>";
                html += "<td>Action</td>";
                html += "</tr>";
            for (var i = 0; i < addedShips.length; i++) {
                html += "<tr>";
                html += "<td>" + addedShips[i].id + "</td>";
                html += "<td>" + addedShips[i].range + "</td>";
                html += "<td><button id=" + addedShips[i].id +">Select</button></td>";
                html += "</tr>";

            }
            html += "</table>";
            console.log(html);
            document.getElementById("shipsPanel").innerHTML = html;

            
        },
    });
};

function changeButtonVisibility() {
    var makeStep = document.getElementById('makeStep');
    makeStep.style.display = 'inline';

    var startGame = document.getElementById('startGame');
    startGame.style.display = 'none';
}

function startGame() {
    changeButtonVisibility();

    $.ajax({
        type: 'POST',
        url: '/Game/StartGame',
        success: function () {
            
        },
    });
};

function sendMessage(message) {
    console.log("Sending: " + message);

    $.ajax({
        type: 'POST',
        url: '/Game/MakeStep',
        data: { message: message },
        success: function () {
            
        },
    });
}

function getCellPoint(mapModel) {
    var convertedPoints = [];
    for (var i = 0; i < mapModel.coord.length; i++) {
        convertedPoints[i] = {};
        convertedPoints[i].x = mapModel.coord[i].x;
        convertedPoints[i].y = mapModel.coord[i].y;
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
