//dark mode
document.getElementById("dark").addEventListener("click", () => {
    document.body.classList.toggle("toggled")
    document.getElementById("dark").classList.toggle("toggled")
    document.getElementById("black").classList.toggle("inactive")
    document.getElementById("bright").classList.toggle("inactive")
    document.getElementById("dark").classList.toggle("toggler")
    document.getElementById("head").classList.toggle("bgBlack")

    document.body.style.backgroundColor = "black"
    document.body.style.color="white"

})





//document.getElementById("me").addEventListener("click", () => {
//    document.getElementById("me").classList.toggle("activee")
//    document.getElementById("someoneElse").classList.toggle("activee")
  
//})

//document.getElementById("someoneElse").addEventListener("click", () => {
//    document.getElementById("someoneElse").classList.toggle("activee")
//    document.getElementById("me").classList.toggle("activee")
//})

//document.getElementById("continue").addEventListener("click", () => {
//    if (document.getElementById("me").classList.contains("activee")) {
//        console.log("hello");
//    }
//    if (document.getElementById("someoneElse").classList.contains("activee")) {
//        window.location.href = 'submitReqSomeElse.cshtml'
//    }
//})


//document.getElementById("prof").addEventListener("click", () => {
//    document.getElementById("profile").classList.add("act")
//    document.getElementById("contentAll").classList.add("none")
//})

//document.getElementById("dash").addEventListener("click", () => {
//    document.getElementById("profile").classList.remove("act")
//    document.getElementById("contentAll").classList.add("act")
//})



//const toggleButton = (curr_btn, redirect_page) => {
//    const buttons = document.getElementsByClassName("common-btn")
//    for (let i = 0; i < buttons.length; ++i) {
//        if (buttons[i] != curr_btn) {
//            buttons[i].classList.remove('create-request-active')
//        }
//        else {
//            buttons[i].classList.add('create-request-active')
//        }
//    }
//    document.getElementById("redirect-value").value = redirect_page;
//}

//const redirect = () => {
//    window.location.assign(`./${document.getElementById("redirect-value").value}`);
//}










