//[COMENTARIU DE LA TUDOR PENTRU MARIO]: Fa onpageload sa ia marimea window-ului si sa updateze max-heightul pagini.

var body = document.body
var inaltime = body.scrollHeight - 99;

var continut = document.getElementsByClassName("messages");
var maxim = document.getElementsByClassName("maxim");
console.log(inaltime);

if (!localStorage.getItem("inaltime")) {
    localStorage.setItem("inaltime", inaltime);
}

let b = parseInt(localStorage.getItem("inaltime"));

if (continut.length > 0)
{
    continut[0].style.maxHeight = ( b - 1 ) + "px";

    console.log("Inaltime: " + b);
}
else
    console.log("e ok")


if (maxim.length > 0) {
    maxim[0].style.maxHeight = (b - 1) + "px";

    console.log("Inaltime: " + b);
}
else
    console.log("e ok")

function poza_mare( i, link )
{
    console.log(i);
    e = document.getElementById(i);
    console.log(e);
    ecran = document.getElementsByTagName("body")
    console.log(ecran);
    let ok = 0;

    nou = document.createElement("img");
    nou.src = link;
    nou.classList.add("apasat")
    ecran[0].appendChild(nou);

    ceata = document.createElement("div");
    ceata.classList.add("fog");
    ecran[0].appendChild(ceata);

    ceata.addEventListener("click", function () {
        nou.remove();
        ceata.style.visibility = "hidden";
    });
}


function afisare_inline(i)
{
    var input = document.getElementById("e " + i);
    console.log(("e + " + i))
    input.style.display = 'block';

}

function buton_cancel(i)
{
    var cancel = document.getElementById("e " + i);
    cancel.style.display = "none";
}


;



var popOutStergere = document.getElementById("popOutStergere")

var buton = document.getElementById("efectuare")
var fog = document.getElementById("fog")

var sterge = document.getElementById("sterge")

if( sterge)
sterge.addEventListener("click", () => {
    popOutStergere.style.display = "block";
    fog.style.display = "block";
})



if(fog)
fog.addEventListener("click", () => {
    popOutStergere.style.display = "none";
    fog.style.display = "none";
})

var profil = document.getElementById("ImageRPath")

console.log(profil)
if(profil)
profil.addEventListener("input", () => {
    
    var poza = profil.files[0];
    
    if(!poza)
    {
        return;
    }
    
    console.log(poza);

    const formData = new FormData();
    formData.append('file', poza);
    
    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Groups/UploadFile' ,true)

    xhr.onload = function () {
        if (xhr.status === 200) {
            const reader = new FileReader();
            reader.onload = function (e) {
                document.getElementById('uploadedImage').src = e.target.result;
            };
            reader.readAsDataURL(poza); 
        } else {
            alert("File upload failed");
        }
    };

    xhr.send(formData);
    
})
const fileInput = document.getElementById('fileInput');
if(fileInput)
fileInput.addEventListener('input', function () {
    
    const file = fileInput.files[0];
    if (!file) {
        alert('Please select a file.');
        return;
    }
    document.getElementById('progress-bar').style.display = 'block';
    const formData = new FormData();
    formData.append('file', file);

    const xhr = new XMLHttpRequest();
    // Update the URL to match the MVC or Web API route
    xhr.open('POST', '/Channels/UploadFile', true);
    
    

    // Update the progress bar
    xhr.upload.onprogress = function (event) {
        if (event.lengthComputable) {
            const percentage = Math.floor((event.loaded / event.total) * 100);
            document.querySelector('#progress-bar div').style.width = percentage + '%';
            document.getElementById('procentaj-upload').innerText = percentage + '%'
        }
    };

    // Show completion status
    xhr.onload = function () {
        if (xhr.status === 200) {
            const response = JSON.parse(xhr.responseText);
            document.getElementById('status').textContent = response.message;
        } else {
            document.getElementById('status').textContent = 'Upload failed.';
        }
    };

    xhr.onerror = function () {
        document.getElementById('status').textContent = 'An error occurred during the upload.';
    };

    xhr.send(formData);
});

function inchide()
{
    var alerta = document.getElementById("alerta");
    alerta.style.display = 'none';
}




//function editare(id) {
//    var mesaj = document.getElementById("message-" + { messageId })
//    var content = mesaj.textContent;
//    var salvare = document.getElementById("edit")
//}


