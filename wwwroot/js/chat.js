class Message
{
    constructor(senderUsername, receiverUsername, receiverUserId, text, when, IsTransporter, ProductId, TravelId, ChatId)
    {
        this.senderUserName = senderUsername;
        this.receiverUserName = receiverUsername;
        this.receiverUserID = receiverUserId;
        this.text = text;
        this.when = when;
        this.isTransporter = IsTransporter;
        this.productId = ProductId;
        this.travelId = TravelId;
        this.chatId = ChatId;
    }
}

// senderUserName is declared in razor page.
const senderUsername = senderUserName;
const receiverUsername = receiverUserName;
const receiverUserId = receiverUserID;
const IsTransporter = isTransporter;
const ProductId = productId;
const TravelId = travelId;
const ChatId = chatId;
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
    let message = new Message(senderUsername, receiverUsername, receiverUserId, text, IsTransporter, ProductId, TravelId, ChatId);
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