
// Parādīt vienkāršu paziņojumu

$(function() {
    // Pievienojam dropdown hover
    const dropdownMenu = $(".dropdown-hover .dropdown-menu");
    const dropdownToggle = $(".dropdown-hover .dropdown-toggle");
    let mouseBeenOver = false;
    dropdownToggle.on('hidden.bs.dropdown', function() {
        // Nepieciešams lai ja lietotājs uzspiež uz hover toggle, dropdown-menu-end turpinātu darboties
        $(".dropdown-hover .dropdown-menu").attr('data-bs-popper', 'static');
    });
    // Animācija ja kursors ir virst peles
    dropdownToggle.hover(function() {
        dropdownMenu.show("fast");
        mouseBeenOver = false;
    });
    dropdownMenu.mouseenter(() => mouseBeenOver = true);
    dropdownMenu.mouseleave(function() {
        if (mouseBeenOver)
            dropdownMenu.hide("fast");    
    })
    
    $.fn.showToast = function(message, title, time=1000)
    {
        $("#toastContainerBody").text(message);
        $("#toastContainerTitle").text(title);
        $("#toastContainer").toast({ autohide: true, delay: time }).toast("show")
    }
})

const openMessageModal = function (pdata) {
    $("#messageModalForm > #pdata").val(pdata)
}