



//dark mode
document.getElementById("dark").addEventListener("click", () => {
    document.body.classList.toggle("toggled")
    document.getElementById("dark").classList.toggle("toggled")
    document.querySelector(".box").classList.toggle("toggled")
    document.getElementById("black").classList.toggle("inactive")
    document.getElementById("bright").classList.toggle("inactive")
    document.getElementById("dark").classList.toggle("toggler")
    document.querySelector(".header").classList.toggle("header-black")
    document.querySelector(".btn2").classList.toggle("toggler")
})

