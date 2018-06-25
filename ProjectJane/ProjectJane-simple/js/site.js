var curPagePosition = 1;

function saveAction() {
    document.getElementById('saveStatusId').innerText = 'Saving...';
    var content = document.getElementById('jd1').value;
    var fileName = new Date().toISOString();

    var blobUri = 'https://' + 'projectjane' + '.blob.core.windows.net';
    var blobService = AzureStorage.Blob.createBlobServiceWithSas(blobUri, '?sv=2017-11-09&ss=bfqt&srt=sco&sp=rwdlacup&se=2018-07-01T07:49:46Z&st=2018-06-19T23:49:46Z&spr=https,http&sig=UM3IF3tVVvw1CxUk%2FMr5AzB4ftvJyBqA3qQQaPxQy80%3D');

    blobService.createBlockBlobFromText(
        'junestudy',
        fileName,
        content,
        (error, result, response) => {
            if (error) {
                document.getElementById('saveStatusId').innerText = 'Save failed!';
                console.log("Couldn't upload string");
                console.error(error);
            } else {
                document.getElementById('saveStatusId').innerText = 'Last saved: ' + new Date().toLocaleString();
                setTimeout(function () {
                    document.getElementById('saveStatusId').innerText = '';
                }, 5000)
                console.log('String uploaded successfully');
            }
        });
}

function addUserSays() {
    var userSaysDiv = document.createElement("div");
    userSaysDiv.className = "containerChat";
    userSaysDiv.innerHTML = `
        <p class="chatBubble">Hello. How are you today?</p>
        <span class="time-rightChat">User</span>
    `;
    var chatControl = document.getElementById('chatControl');
    chatControl.appendChild(userSaysDiv);

}

function addUserSaysHelper(text, userName) {
    var userSaysDiv = document.createElement("div");
    userSaysDiv.className = "containerChat";
    userSaysDiv.innerHTML = `
        <p class="chatBubble">` + text + `</p>
        <span class="time-rightChat">` + userName + `</span>
    `;
    var chatControl = document.getElementById('chatControl');
    chatControl.appendChild(userSaysDiv);

}

function addBotSaysHelper(text, botName) {
    var botSaysDiv = document.createElement("div");
    botSaysDiv.className = "containerChat darkerChat";
    botSaysDiv.innerHTML = `
        <p class="chatBubble">` + text + `</p>
        <span class="time-leftChat">` + botName + `</span>
    `;
    var chatControl = document.getElementById('chatControl');
    chatControl.appendChild(botSaysDiv);
}

function addBotSays() {
    var botSaysDiv = document.createElement("div");
    botSaysDiv.className = "containerChat darkerChat";
    botSaysDiv.innerHTML = `
        <p class="chatBubble">Hello. How are you today?</p>
        <span class="time-leftChat">Bot</span>
    `;
    var chatControl = document.getElementById('chatControl');
    chatControl.appendChild(botSaysDiv);
}

function genChat() {
    // clear chat window
    var chatControl = document.getElementById('chatControl');
    while (chatControl.firstChild) {
        chatControl.removeChild(chatControl.firstChild);
    }

    // get text from textarea
    var text = document.getElementById('jd1').value;
    var tokenized = text.split(/(user|bot|User|Bot)\s*(:|=)/);
    var userName = 'User';
    var botName = 'Bot';
    for (p = 0; p < tokenized.length; p++) {
        if (tokenized[p]) {
            if (tokenized[p].toLowerCase() === 'user') {
                if(tokenized[p+1] === '=') {
                    userName = tokenized[p+2];
                } else {
                    addUserSaysHelper(tokenized[p + 2].trim(), userName.trim());
                }
                p+=2;
            } else {
                if(tokenized[p+1] === '=') {
                    botName = tokenized[p+2];
                } else {
                    addBotSaysHelper(tokenized[p + 2].trim(), botName.trim());
                }
                p+=2;
            }
        }
    }
}
function registerHandler() {
    var input = document.getElementById("jd1");

    // Execute a function when the user releases a key on the keyboard
    input.addEventListener("keyup", function (event) {
        // Cancel the default action, if needed
        event.preventDefault();
        // Number 13 is the "Enter" key on the keyboard
        if (event.keyCode === 13) {
            genChat();
        }
    });
}

function moveLeft() {
    --curPagePosition; 
    if(curPagePosition < 1) curPagePosition = 1;
    renderPage();
}

function moveRight() {
    ++curPagePosition;
    if(curPagePosition > 41) curPagePosition = 41;
    renderPage();
}
function clearChat() {
    document.getElementById('jd1').value = '';
}
function renderPage() {
    let pageSrc;
    if(curPagePosition < 10) {
        if(curPagePosition === 8) {
            document.getElementById('jd1').style.visibility = "visible";
            document.getElementById('saveButton').style.visibility = "visible";
            document.getElementById('saveStatusId').style.visibility = "visible";
            document.getElementById('chatControl').style.visibility = "visible";
            document.getElementById('refreshButton').style.visibility = "visible";
            document.getElementById('resetButton').style.visibility = "visible";
            pageSrc = "images/src/ProjectJane_0620_Page_0" + curPagePosition + ".png";
            document.getElementById('bgImg').src = pageSrc;
        } else {
            document.getElementById('jd1').style.visibility = "hidden";
            document.getElementById('saveButton').style.visibility = "hidden";
            document.getElementById('saveStatusId').style.visibility = "hidden";
            document.getElementById('chatControl').style.visibility = "hidden";
            document.getElementById('refreshButton').style.visibility = "hidden";
            document.getElementById('resetButton').style.visibility = "hidden";
            pageSrc = "images/src/ProjectJane_0620_Page_0" + curPagePosition + ".png";
            document.getElementById('bgImg').src = pageSrc;
        }        
    } else {
        if(curPagePosition === 10) {
            document.getElementById("page10").style.visibility = "visible";
            document.getElementById("page10scroll").style.visibility = "visible";
        } else {
            document.getElementById("page10").style.visibility = "hidden";
            document.getElementById("page10scroll").style.visibility = "hidden";
        }
        pageSrc = "images/src/ProjectJane_0620_Page_" + curPagePosition + ".png";
        document.getElementById('bgImg').src = pageSrc;
    }
}

function goHome() {
    window.location.assign('6222018_1.html');
}