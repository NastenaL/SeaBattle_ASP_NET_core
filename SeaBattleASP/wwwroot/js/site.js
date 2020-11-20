var width;
var height;
var topText = ['а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с'];
var leftText = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17'];
var addedShips = new Array();
var message;
var selectedShipId;
var stepType;
var player;

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
        html += "<td style='padding: 2px'><input id='selectShip"+i+"' name='selectShip' onchange='selectShipForShift(" + addedShips[i].id + ");' type='radio' value='" + addedShips[i].isSelectedShip + "'/></td>";
        html += "<td style='padding: 2px'>" + addedShips[i].id + "</td>";
        html += "<td style='padding: 2px'>" + addedShips[i].type + "</td>";
        html += "<td style='padding: 2px'>" + addedShips[i].range + "</td>";
        html += "<td style='padding: 2px'>" +
            "<div class='dropdown'>" +
            "<button id='step" + i + "' style='display: none;' onclick='openOptions(" + i + ")' class='dropbtn'>Select</button>" +
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

function changeEnabledShipButton(disableResult) {
    var stepButton = new Array();
    for (var i = 0; i < 4; i++) {
        stepButton.push(document.getElementById(i));
        stepButton[i].disabled = disableResult;
    }
}

function addShipToField(shipId) {
    var element = document.getElementById(shipId);
    element.style.display = 'none';

    var directionPanel = document.getElementById('directionPanel');
    directionPanel.style.display = 'inline';

    changeEnabledShipButton(true);

    var parameters = getUrlParams(window.location.href);
    var playerId = parameters.playerId;
    var gameId = parameters.gameId;
    $.ajax({
        type: 'POST',
        url: '/Game/AddShipToField',
        data: { shipId: shipId, playerId: playerId, gameId: gameId },
        success: function (mapModel) {     
            addedShips.push(mapModel.selectedShip);
            for (var i = 0; i < addedShips.length; i++) {
                paintShip(addedShips[i].deckCells, 'usualShipColor');
            }

            var html = createShipTable();
            document.getElementById("shipsPanel").innerHTML = html;

            changeEnabledShipButton(false);
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

function openOptions(i) {

    document.getElementById("myDropdown" + i).classList.toggle("show");
}

// Close the dropdown menu if the player clicks outside of it
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
    var playerId = parameters.playerId;
   
    message = "Game over";
    $.ajax({
        type: 'POST',
        url: '/Game/GameOver',
        data: { gameId: gameId, playerId: playerId },
        success: function (response) {
            window.location.href = response.redirectToUrl;
        },
    });
}

function changeButtonVisibility(game) {
    var btnAddShips = document.getElementById('btnAddShips');
    btnAddShips.style.display = 'none';
    

    var stepButton = new Array();
    for (var i = 0; i < addedShips.length; i++) {
        var selectShip = document.getElementById('selectShip'+i);
        selectShip.style.display = 'none';
        stepButton.push(document.getElementById('step' + i));
        stepButton[i].style.display = 'inline';
    }

    var startGame = document.getElementById('startGame');
    startGame.style.display = 'none';

    var directionPanel = document.getElementById('directionPanel');
    directionPanel.style.display = 'none';
}

function getEnemy(game, playerId) {

    var enemy;
    if (game.player1 == playerId) {
        enemy = game.player2;
    }
    else {
        enemy = game.player1;
    }
    return enemy;
}

function getEnemyShip(model, enemy) {
    var ships = new Array();
    model.currentGame.playingField.ships.forEach(function (ship) {
        if (ship.player.id == enemy.id) {
            ships.push(ship);
        }
    });
    return ships;
}

function startGame() {
    var parameters = getUrlParams(window.location.href);
    var playerId = parameters.playerId;
    var gameId = parameters.gameId;

    $.ajax({
        type: 'POST',
        data: { gameId: gameId },
        url: '/Game/StartGame',
        success: function (model) {
            message = model.message;
            alert(model.message);
        },
    });
};


function repaintShips(ships, field, color) {
    ships.forEach(function (ship) {
        ship.deckCells.forEach(function (deckCell) {
            paintDeckShip(deckCell, field, color);
        });
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

//Открыть модальное окно для добавления кораблей
$("#btnAddShips").click(function () {
    $('#ModalPopUp').modal('show');
})

$(document).ready(function () {
    $('#userName').keyup(function () {
        validatePlayerName();
    });
});

function validatePlayerName() {
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
//For step

function makeMovement(shipId, type) {
    selectedShipId = shipId;
    stepType = type;

    var sendButton = document.getElementById("sendButton");
    sendButton.click();

    var parameters = getUrlParams(window.location.href);
    var gameId = parameters.gameId;
    switch (type) {
        case 0:
            makeFire(shipId);
            break;
        case 1:
            makeRepair(shipId);
            break;
        case 2:
            makeMove(shipId, gameId);
            break;
    }
}

function makeRepair(shipId) {
    var parameters = getUrlParams(window.location.href);
    var gameId = parameters.gameId;
    var playerId = parameters.playerId;

    $.ajax({
        type: 'POST',
        url: '/Game/MakeRepairStep',
        data: { shipId: shipId, gameId: gameId, playerId: playerId },
        success: function (model) {
            alert(model.message);
        },
    });
}

function makeFire(shipId) {
    var parameters = getUrlParams(window.location.href);
    var gameId = parameters.gameId;
    $.ajax({
        type: 'POST',
        url: '/Game/MakeFireStep',
        data: { shipId: shipId, gameId: gameId },
        success: function (model) {
            alert(model.message);
        },
    });
}

function makeMove(shipId, gameId) {
    $.ajax({
        type: 'POST',
        url: '/Game/MakeMoveStep',
        data: { shipId: shipId, gameId: gameId },
        success: function (model) {
        },
    });
}

//For drawing objects
function paintShip(deckCells, shipColor) {
    var len = deckCells.length;
    for (var i = 0; i < len; i++) {
        paintDeckShip(deckCells[i], '#leftField', shipColor);
    }
}

function paintDeckShip(deckcell, field, shipColor) {

    $(field + ' #cell' + deckcell.cell.y + deckcell.cell.x).removeClass(shipColor + ' cellColor').addClass(shipColor);
    $(field + ' #cell' + deckcell.cell.y + deckcell.cell.x).addClass('ship');
};

function repaintShip(ship) {
    var foundIndex = addedShips.findIndex(x => x.id == ship.id);
    addedShips[foundIndex] = ship;

    emptyCellsToField('#leftField');

    for (var i = 0; i < addedShips.length; i++) {
        paintShip(addedShips[i].deckCells, 'usualShipColor');
    }
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

emptyCellsToField('#leftField');
emptyCellsToField('#rightField');
$('.cell').removeClass('cellColor');
addTextToPositioning('left');
addTextToPositioning('right');

//SignalR -------

//For game state
var stateGameHubconnection = new signalR.HubConnectionBuilder().withUrl("/stateGameHub").build();

stateGameHubconnection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("startGame").addEventListener("click", function (event) {

    stateGameHubconnection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("gameOver").addEventListener("click", function (event) {

    stateGameHubconnection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});


stateGameHubconnection.on("startGameSignalR", function (game) {
    var parameters = getUrlParams(window.location.href);
    var playerId = parameters.playerId;
    var myShips = getShipsByPlayerId(game, playerId);

    emptyCellsToField('#leftField');

    repaintShips(myShips, '#leftField', 'usualShipColor');

    changeButtonVisibility(game);
    var encodedMsg = "Message: The game is start";
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

var delayInMilliseconds = 2000;

stateGameHubconnection.on("gameOverSignalR", function () {
    var parameters = getUrlParams(window.location.href);
    var playerId = parameters.playerId;

    var encodedMsg = "Message: The game is over";
    alert("Message: The game is over");
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);


    setTimeout(function () {
        window.location.href = '/Game/Index/' + playerId;
    }, delayInMilliseconds);

});

//For step
var stepHubconnection = new signalR.HubConnectionBuilder().withUrl("/stepHub").build();

stepHubconnection.on("ReceiveMessage", function (playerId, ship, stepType) {
    var seceltedShip = ship.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = "Player" + playerId + "select ship " + seceltedShip + ", type " + stepType;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

stepHubconnection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {

    stepHubconnection.invoke("SendMessage", player, selectedShipId, stepType).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function getShipsByPlayerId(game, playerId) {
    var ships = new Array();
    game.playingField.ships.forEach(function (ship) {
        if (ship.player.id == playerId) {
            ships.push(ship);
        }
    });
    return ships;
}

stateGameHubconnection.on("makeStepSignalR", function (game) {
    repaintAllShips(game);
});

function repaintAllShips(game) {
    var parameters = getUrlParams(window.location.href);
    var playerId = parameters.playerId;

    var myShips = getShipsByPlayerId(game, playerId);

    var enemy = getEnemy(game, playerId);

    var enemyShips = getShipsByPlayerId(game, enemy.id);
    var hurtedShips = getFiredEnemyShips(enemyShips);

    emptyCellsToField('#leftField');
    emptyCellsToField('#rightField');

    repaintShips(hurtedShips, '#rightField', 'shipFiredColor');
    repaintShips(myShips, '#leftField', 'usualShipColor');
}

stateGameHubconnection.on("makeStepFireSignalR", function (game) {
    var parameters = getUrlParams(window.location.href);
    var playerId = parameters.playerId;

    var enemy = getEnemy(game, playerId);
    var enemyShips = getShipsByPlayerId(game, enemy.id);
    var hurtedShips = getFiredEnemyShips(enemyShips);

    emptyCellsToField('#rightField');
    repaintShips(hurtedShips, '#rightField', 'shipFiredColor');
});

function getFiredEnemyShips(enemyShips) {
    var hurtedShips = new Array();

    enemyShips.forEach(function (ship) {
        ship.deckCells.forEach(function (deckCell) {
            console.log(deckCell.deck.state);
            if (deckCell.deck.state == 1) {
              
                hurtedShips.push(ship);
            }
        });

    });
    return hurtedShips;
}

stateGameHubconnection.on("makeRepairStepSignalR", function (game) {
    repaintAllShips(game);
});


