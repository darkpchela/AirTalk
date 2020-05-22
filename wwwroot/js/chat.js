"use strict";

import { signalR } from "./signalr/dist/browser/signalr";

//var hubConnection = new signalR.HubConnectionBuilder().WithUrl("/chat").Build();
var hubConnection = new signalR.
let connectionId = "";
document.getElementById("btnSend").addEventListener("click", function (e) {
    e.preventDefault();
    let data = new FormData();
    data.append("product", document.getElementById("textBox").value);
    data.append("connectionId", connectionId);
    hubConnection.invoke("Create", data);
});
                //fetch("/Main/Create", {
        //method: "POST",
        //body: data
        //})
        //.catch(error => console.error("Error: ", error));
        //});
hubConnection.on("Notify", function (message) {
    // создает элемент <p> для сообщения пользователя
    let elem = document.createElement("div");
    elem.classList.add("message");
    elem.appendChild(document.createTextNode(message));
    document.getElementById("chat").appendChild(elem);
        });
hubConnection.start().then(() => {
    // после соединения получаем id подключения
    console.log(hubConnection.connectionId);
    connectionId = hubConnection.connectionId;
                });