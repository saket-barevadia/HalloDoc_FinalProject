document.getElementById("dark").addEventListener("click", () => {
    document.body.classList.toggle("bg")
    document.querySelector(".header").classList.toggle("shadow")
    document.getElementById("black").classList.toggle("inactive")
    document.getElementById("bright").classList.toggle("inactive")
    document.getElementById("dark").classList.toggle("toggler")
    document.querySelector(".btn2").classList.toggle("toggler")
    document.querySelector(".heading").classList.toggle("white")
})