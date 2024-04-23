
//dark mode
document.getElementById("dark").addEventListener("click", () => {
    document.body.classList.toggle("toggled")
    document.getElementById("dark").classList.toggle("toggled")
    document.getElementById("black").classList.toggle("inactive")
    document.getElementById("bright").classList.toggle("inactive")
    document.getElementById("dark").classList.toggle("toggler")
    document.getElementById("changeType").classList.toggle("inputText")
    document.getElementById("formGroupExampleInput").classList.toggle("inputText")
    document.getElementById("password").classList.toggle("white")
    document.getElementById("password2").classList.toggle("white")
    document.getElementById("user").classList.toggle("white")
    document.getElementById("terms").classList.toggle("white")
})




//change type 

document.getElementById("password").addEventListener("click", () => {
    return [
        document.querySelector("#changeType").type = "text",
        document.getElementById("password").style.display = "none",
        document.getElementById("password2").style.display = "block"
    ]
})

document.getElementById("password2").addEventListener("click", () => {
    return [
        document.querySelector("#changeType").type = "password",
        document.getElementById("password2").style.display = "none",
        document.getElementById("password").style.display = "block",
    ]
})