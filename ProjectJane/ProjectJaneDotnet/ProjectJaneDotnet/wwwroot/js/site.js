// Write your Javascript code.
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
    //alert('adding user');
    var userSaysDiv = document.createElement("div");
    userSaysDiv.className = "containerChat";
    userSaysDiv.innerHTML = `
        <p class="chatBubble">Hello. How are you today?</p>
        <span class="time-rightChat">User</span>
    `;
    var chatControl = document.getElementById('chatControl');
    chatControl.appendChild(userSaysDiv);

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