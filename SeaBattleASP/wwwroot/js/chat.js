var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (userId, ship, stepType) {
    var msg = ship.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = userId + " ship " + msg + "type " + stepType;
	var li = document.createElement("li");
    li.textContent = encodedMsg;
    console.log("message", ship);
	document.getElementById("messagesList").appendChild(li);
});

connection.start().catch(function (err) {
	return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
	var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var stepType = document.getElementById("stepTypeInput").value;
    console.log("sendButton");
    connection.invoke("SendMessage", user, message, stepType).catch(function (err) {
		return console.error(err.toString());
	});
	event.preventDefault();
});