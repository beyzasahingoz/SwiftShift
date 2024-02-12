class Message
{
    constructor(senderUsername, receiverUsername, receiverUserId, text, when)
    {
        this.senderUserName = senderUsername;
        this.receiverUserName = receiverUsername;
        this.receiverUserID = receiverUserId;
        this.text = text;
        this.when = when;
    }
}

// senderUserName is declared in razor page.
const senderUsername = senderUserName;
const receiverUsername = receiverUserName;
const receiverUserId = receiverUserID;
const textInput = document.getElementById('messageText');
const whenInput = document.getElementById('when');
const chat = document.getElementById('chat');
const messagesQueue = [];

document.getElementById('submitButton').addEventListener('click', () =>
{
    var currentdate = new Date();
    when.innerHTML =
        (currentdate.getMonth() + 1) + "/"
        + currentdate.getDate() + "/"
        + currentdate.getFullYear() + " "
        + currentdate.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true })
});

function clearInputField()
{
    messagesQueue.push(textInput.value);
    textInput.value = "";
}

function sendMessage()
{
    let text = messagesQueue.shift() || "";
    if (text.trim() === "") return;

    let when = new Date();
    let message = new Message(senderUsername, receiverUsername, receiverUserId, text);
    sendMessageToHub(message);
}

function addMessageToChat(message)
{
    let isCurrentUserMessage = message.senderUserName === senderUsername;

    let container = document.createElement('div');
    container.className = isCurrentUserMessage ? "container darker" : "container";

    let sender = document.createElement('p');
    sender.className = "sender";
    sender.innerHTML = message.senderUserName;
    let text = document.createElement('p');
    text.innerHTML = message.text;

    let when = document.createElement('span');
    when.className = isCurrentUserMessage ? "time-left" : "time-right";
    var currentdate = new Date();
    when.innerHTML =
        (currentdate.getMonth() + 1) + "/"
        + currentdate.getDate() + "/"
        + currentdate.getFullYear() + " "
        + currentdate.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true })

    container.appendChild(sender);
    container.appendChild(text);
    container.appendChild(when);
    chat.appendChild(container);
}