//dark mode
document.getElementById("dark").addEventListener("click", () => {
    document.body.classList.toggle("toggled")
    document.getElementById("dark").classList.toggle("toggled")
    document.getElementById("black").classList.toggle("inactive")
    document.getElementById("bright").classList.toggle("inactive")
    document.getElementById("dark").classList.toggle("toggler")
})
