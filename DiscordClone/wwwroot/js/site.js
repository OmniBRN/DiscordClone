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

sterge.addEventListener("click", () => {
    popOutStergere.style.display = "block";
    fog.style.display = "block";
})




fog.addEventListener("click", () => {
    popOutStergere.style.display = "none";
    fog.style.display = "none";
})



//function editare(id) {
//    var mesaj = document.getElementById("message-" + { messageId })
//    var content = mesaj.textContent;
//    var salvare = document.getElementById("edit")
//}


