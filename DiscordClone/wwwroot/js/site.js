//[COMENTARIU DE LA TUDOR PENTRU MARIO]: Fa onpageload sa ia marimea window-ului si sa updateze max-heightul pagini.

var body = document.body
var inaltime = body.scrollHeight - 139;

var continut = document.getElementsByClassName("messages");
var maxim = document.getElementsByClassName("maxim");
console.log(inaltime);

if (!localStorage.getItem("inaltime")) {
    localStorage.setItem("inaltime", inaltime);
}

var toata_pagina = document.getElementById("original")
toata_pagina.style.maxHeight = ( inaltime + 139 ) + 'px'

let b = parseInt(localStorage.getItem("inaltime"));

var creare_grup = document.getElementById("creare-grup")

if (continut.length > 0)
{
    //continut[0].style.maxHeight = ( b - 38 ) + "px";
    var sidebar = document.getElementById("sidebar")
    sidebar.style.maxHeight = ( inaltime + 139 ) + 'px'
    var mesaje = document.getElementsByClassName("info-grup")
    console.log(mesaje[mesaje.length - 1 ])
    if(mesaje.length > 0)
    mesaje[mesaje.length - 1 ].scrollIntoView({ block: "end", inline: "nearest" });

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

function inchide_kick(i)
{
    var optiune = document.getElementById("kick " + i);
    var blackdrop = document.getElementById("blackdrop " + i);

    optiune.style.display = 'none';
    blackdrop.style.display = 'none';
}

function kick_membru(i)
{
    console.log(i);
    var optiune = document.getElementById("kick " + i);
    optiune.style.display = 'block';
    
    var blackdrop = document.createElement("div");
    blackdrop.classList.add("fog")
    blackdrop.style.display = 'block';
    blackdrop.id = ("blackdrop " + i);
    blackdrop.addEventListener("click", function () {
        optiune.style.display = 'none';
        blackdrop.style.display = 'none';
    })

    ecran = document.getElementsByTagName("body")
    ecran[0].appendChild(blackdrop);
    
}



function buton_cancel(i)
{
    var cancel = document.getElementById("e " + i);
    cancel.style.display = "none";
}






var popOutStergere = document.getElementById("popOutStergere")
var popOutShow = document.getElementById("popOutShow");
var kick = document.getElementById("kick")
var leave = document.getElementById("leave")

var leave_btn = document.getElementById("leave-button");
var kick_btn = document.getElementById("kick-button")
var buton = document.getElementById("efectuare")




var fog = document.getElementById("fog")

var sterge = document.getElementById("sterge")

if( sterge)
sterge.addEventListener("click", () => {
    popOutStergere.style.display = "block";
    fog.style.display = "block";
})

if(leave_btn)
{
    leave_btn.addEventListener("click", () => {
        console.log('a intrat')
        leave.style.display = "block";
        fog.style.display = "block";
    })
}

if(kick_btn)
{
    kick_btn.addEventListener("click", () => {
        kick.style.display = "block";
        fog.style.display = "block";
        fog.style.zIndex = 1;
    })
}



if(fog)
fog.addEventListener("click", () => {
    if (popOutStergere && popOutStergere.style.display === "block") {
        popOutStergere.style.display = "none";
    }
    if (kick && kick.style.display === "block") {
        kick.style.display = "none";
    }
    if (leave && leave.style.display === "block") {
        leave.style.display = "none";
    }
    if(creare_grup && creare_grup.style.display === "block") {
        creare_grup.style.display = "none";
    }
    if( popOutShow && popOutShow.style.display === "block") {
        popOutShow.style.display = "none";
    }
    fog.style.display = "none";
})

function cont(){
    popOutShow.style.display = "block";
    fog.style.display = "block";
}


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
                document.getElementById('uploadedImage-edit').src = e.target.result;
            };
            reader.readAsDataURL(poza); 
        } else {
            alert("File upload failed");
        }
    };

    xhr.send(formData);
    
})

var creare_cont = document.getElementById("Input_ProfilePicture")

if(creare_cont)
    creare_cont.addEventListener("input", function(){
        var poza = creare_cont.files[0];

        if(!poza)
        {
            return;
        }

        console.log(poza);

        const formData = new FormData();
        formData.append('file', poza);

        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Categories/UploadFile' ,true)

        xhr.onload = function () {
            if (xhr.status === 200) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    document.getElementById('profil-provizoriu').src = e.target.result;
                };
                reader.readAsDataURL(poza);
            } else {
                console.error("File upload failed. Status:", xhr.status, "Response:", xhr.responseText);
                alert("File upload failed");
            }
        };

        xhr.send(formData);

    })

var pro = document.getElementById("ProfilePicture")

console.log("ceva " + pro)
if(pro)
    pro.addEventListener("input", () => {

        var poza = pro.files[0];
        console.log("vdde")
        if(!poza)
        {
            return;
        }

        console.log("ceva" + poza);

        const formData = new FormData();
        formData.append('file', poza);

        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/EditProfile/UploadFile' ,true)

        xhr.onload = function () {
            if (xhr.status === 200) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    document.getElementById('poza-editare-profil').src = e.target.result;
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


var creare = document.getElementById("adaugare");
if(creare)
creare.addEventListener("click", function(){
    fog.style.display = "block";
   creare_grup.style.display = "block";
   document.getElementById("modal-creare").style.display = "block";
})


//function editare(id) {
//    var mesaj = document.getElementById("message-" + { messageId })
//    var content = mesaj.textContent;
//    var salvare = document.getElementById("edit")
//}


