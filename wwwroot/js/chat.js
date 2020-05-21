const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .build();

let connectionId = "";

document.getElementById("submitForm")
    .addEventListener("click", function (e) {
        e.preventDefault();

        const data = new FormData();
        data.append("product", document.getElementById("productField").value);
        data.append("connectionId", connectionId);

        fetch("/Main/Create", {
            method: "POST",
            body: data
        })
            .catch(error => console.error("Error: ", error));
    });