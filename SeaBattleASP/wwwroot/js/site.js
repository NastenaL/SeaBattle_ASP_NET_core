var topText = ['а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с'];
var leftText = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17'];
var addedShips = new Array();
var message;

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

function getCursorCoordinate(x, y) {
    console.log("x = ", x);
    console.log("y = ", y);

    $.ajax({
        type: 'POST',
        url: '/Game/SelectShip',
        data: { x: x, y: y },
        success: function (result) {

        },
    });
}

function createSingleGame() {
    var url = window.location.pathname;
    var playerId = url.substring(url.lastIndexOf('/') + 1);

    $.ajax({
        type: 'POST',
        url: '/Game/CreateGame',
        data: { playerId: playerId },
        success: function (response) {
            window.location.href = response.redirectToUrl;
        },
    });
}

// функция заполняет поле элементами span
// входной параметр - поля(левое или правое)
function emptyCellsToField(field) {
    $(field).empty();
    for (var i = 0; i < width; i++) {
        for (var j = 0; j < height; j++) {
            $(field).append('<span onclick="return getCursorCoordinate(' + i + ',' + j + ')" class="cell cellColor" id=cell' + i + j + '  oncontextmenu="blocker(' + i + j + ');return false" ></span>');
        }
    }
};

function selectShipForShift(shipId) {
    document.getElementById('left').value = shipId;
    document.getElementById('right').value = shipId;
    document.getElementById('up').value = shipId;
    document.getElementById('down').value = shipId;
}


function shiftShip(direction) {
    var b = document.getElementById('left');

    $.ajax({
        type: 'POST',
        url: '/Game/ShiftShip',
        data: { shipId: b.value, direction: direction },
        success: function (ship) {
            repaintShip(ship);
        },
    });
}

function createShipTable() {
    var html = "<table class='shipsTable'>";
    html += "<tr>";
    html += "<td>#</td>";
    html += "<td>Id</td>";
    html += "<td>Type</td>";
    html += "<td>Range</td>";
    html += "<td>Action</td>";
    html += "</tr>";
    for (var i = 0; i < addedShips.length; i++) {
        let options = '';

        options += "<a onclick='makeMovement(" + addedShips[i].id + ",2)'>Move</a>";

        var isMixShip = addedShips[i].type === 'MixShip';
        var isMilitaryShip = addedShips[i].type === 'MilitaryShip';
        var isAuxiliaryShip = addedShips[i].type === 'AuxiliaryShip';

        if (isMixShip || isMilitaryShip) {
            options += "<a onclick='makeMovement(" + addedShips[i].id + ",0)'>Fire</a>";
        }
        if (isMixShip || isAuxiliaryShip) {
            options += "<a onclick='makeMovement(" + addedShips[i].id + ",1)'>Repair</a>";
        }

        html += "<tr>";
        html += "<td style='padding: 2px'><input name='selectShip' onchange='selectShipForShift(" + addedShips[i].id + ");' type='radio' value='" + addedShips[i].isSelectedShip + "'/></td>";
        html += "<td style='padding: 2px'>" + addedShips[i].id + "</td>";
        html += "<td style='padding: 2px'>" + addedShips[i].type + "</td>";
        html += "<td style='padding: 2px'>" + addedShips[i].range + "</td>";
        html += "<td style='padding: 2px'>" +
            "<div class='dropdown'>" +
            "<button style='display: none' id='step" + i + "' onclick='openOptions(" + i + ")' class='dropbtn'>Select</button>" +
            "<div id='myDropdown" + i + "' class='dropdown-content'>" + options + " </div>" +
            "</div >" +
            " </td > ";
        html += "</tr>";

    }
    html += "</table>";
    return html;
}

var getUrlParams = function (url) {
    var params = {};
    (url + '?').split('?')[1].split('&').forEach(
        function (pair) {
            pair = (pair + '=').split('=').map(decodeURIComponent);
            if (pair[0].length) {
                params[pair[0]] = pair[1];
            }
        });

    return params;
};

function changeColor(ship) {

}

function addShipToField(id) {
    var element = document.getElementById(id);
    element.style.display = 'none';

    var parameters = getUrlParams(window.location.href);
    var playerId = parameters.playerId;
    var gameId = parameters.gameId;
    $.ajax({
        type: 'POST',
        url: '/Game/AddShipToField',
        data: { id: id, playerId: playerId, gameId: gameId },
        success: function (mapModel) {
            var convertedPoints = getCellPoint(mapModel);
          
            addedShips.push(mapModel.selectedShip);
            for (var i = 0; i < addedShips.length; i++) {
                paintShip(addedShips[i].deckCells, 'usualShipColor');
            }

            var html = createShipTable();
            document.getElementById("shipsPanel").innerHTML = html;

            var directionPanel = document.getElementById('directionPanel');
            directionPanel.style.display = 'inline';
        },
    });
};

function convertCellsToPoints(ships) {
    var convertedPoints = [];
    for (var i = 0; i < ships.length; i++) {
        for (var j = 0; j < ships[i].deckCells.length; j++) {
            convertedPoints.push({ x: ships[i].deckCells[j].cell.x, y: ships[i].deckCells[j].cell.y });
        }
    }
    return convertedPoints;
}

var selectedShipId;
var stepType;
var user;

function makeMovement(shipId, type) {
    selectedShipId = shipId;
    stepType = type;

    console.log(type);
    switch (type) {
        case 0:
            makeFire(shipId);
            break;
        case 1:
            makeRepair(shipId);
            break;
        case 2:
            makeMove(shipId);
            break;
    }
}

function makeRepair(shipId) {
    $.ajax({
        type: 'POST',
        url: '/Game/MakeRepairStep',
        data: { shipId: shipId },
        success: function (model) {
            console.log(model);
        },
    });
}

function makeFire(shipId) {
    $.ajax({
        type: 'POST',
        url: '/Game/MakeFireStep',
        data: { shipId: shipId },
        success: function (model) {
            console.log(model);
        },
    });
}

function makeMove(shipId) {
    $.ajax({
        type: 'POST',
        url: '/Game/MakeMoveStep',
        data: { shipId: shipId },
        success: function (ship) {
            repaintShip(ship);
        },
    });
}

function repaintShip(ship) {
    var foundIndex = addedShips.findIndex(x => x.id == ship.id);
    addedShips[foundIndex] = ship;

    emptyCellsToField('#leftField');

    for (var i = 0; i < addedShips.length; i++) {
        paintShip(addedShips[i].deckCells, 'usualShipColor');
    }
}

function openOptions(i) {
    document.getElementById("myDropdown" + i).classList.toggle("show");
}

// Close the dropdown menu if the user clicks outside of it
window.onclick = function (event) {
    if (!event.target.matches('.dropbtn')) {

        var dropdowns = document.getElementsByClassName("dropdown-content");
        var i;
        for (i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
                openDropdown.classList.remove('show');
            }
        }
    }
}

function gameOver() {
    var parameters = getUrlParams(window.location.href);
    var gameId = parameters.gameId;
    console.log("Game over", gameId);

    $.ajax({
        type: 'POST',
        url: '/Game/GameOver',
        data: { gameId: gameId },
        success: function () {
        },
    });
}

function changeButtonVisibility() {
    var makeStep = document.getElementById('makeStep');
    makeStep.style.display = 'inline';

    for (var i = 0; i < addedShips.length; i++) {
        var makeStep = document.getElementById('step' + i);
        makeStep.style.display = 'inline';
    }
    var startGame = document.getElementById('startGame');
    startGame.style.display = 'none';
}

function checkWhoseStep() {
    var isPlayersInGame = model.currentGame.player1 != null && model.currentGame.player2 != null;
    var isPl1Step = model.currentGame.isPl1Turn && model.currentGame.player1.id == parameters.playerId;
    var isPl2Step = !model.currentGame.isPl1Turn && model.currentGame.player2.id == parameters.playerId;

    if (isPlayersInGame && (isPl1Step || isPl2Step)) {
        changeButtonVisibility();
    }
}

function startGame() {
    var parameters = getUrlParams(window.location.href);
    var gameId = parameters.gameId;

    $.ajax({
        type: 'POST',
        data: { gameId: gameId },
        url: '/Game/StartGame',
        success: function (model) {
            alert(model.message);

            var directionPanel = document.getElementById('directionPanel');
            directionPanel.style.display = 'none';

            message = model.message;
            checkWhoseStep();
        },
    });
};

function getCellPoint(mapModel) {
    var convertedPoints = [];
    for (var i = 0; i < mapModel.coord.length; i++) {
        convertedPoints[i] = {};
        convertedPoints[i].x = mapModel.coord[i].x;
        convertedPoints[i].y = mapModel.coord[i].y;
    }
    return convertedPoints;
}

function paintShip(deckCells, shipColor) {
    var len = deckCells.length;
    for (var i = 0; i < len; i++) {
        paintDeckShip(deckCells[i], '#leftField', shipColor);
    }
}

function paintDeckShip(deckcell, field, shipColor) {

    $(field + ' #cell' + deckcell.cell.x + deckcell.cell.y).removeClass(shipColor + ' cellColor').addClass(shipColor);
    $(field + ' #cell' + deckcell.cell.x + deckcell.cell.y).addClass('ship');
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

function validateUserName() {
    if (document.getElementById("userName").value === "") {
        document.getElementById('register').disabled = true;
    } else {
        document.getElementById('register').disabled = false;
    }
}

function joinToGame(gameId) {
    var url = window.location.pathname;
    var playerId = url.substring(url.lastIndexOf('/') + 1);
    $.ajax({
        type: 'POST',
        data: { gameId: gameId, playerId: playerId },
        url: '/Game/JoinToGame',
        success: function (response) {
            window.location.href = response.redirectToUrl;
        },
    });
}

//SignalR
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (userId, ship, stepType) {
    var seceltedShip = ship.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = "Player" + userId + "select ship " + seceltedShip + ", type " + stepType;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    console.log("message", ship);
    document.getElementById("messagesList").appendChild(li);
});

connection.on("ReceiveMessage", function (message) {
    var encodedMsg = "Message" + message;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {

    console.log("sendButton");
    connection.invoke("SendMessage", user, selectedShipId, stepType).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("startGame").addEventListener("click", function (event) {

    console.log("startGame");
    connection.invoke("SendShortMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});