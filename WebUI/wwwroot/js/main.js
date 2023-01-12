
// Parādīt vienkāršu paziņojumu

$(function() {
    $.fn.showToast = function(message, title, time=1000)
    {
        $("#toastContainerBody").text(message);
        $("#toastContainerTitle").text(title);
        $("#toastContainer").toast({ autohide: true, delay: time }).toast("show")
    }
    let dropdowns = Array();
    const addDropdownHover = function(id)
    {
        dropdowns.push(id);
        const dropdownMenu = $(`${id} .dropdown-menu`);
        
        const dropdownToggle = $(`${id} .dropdown-toggle`);
        if (!dropdownMenu || !dropdownToggle) return;
        
        dropdownToggle.on('hidden.bs.dropdown', function() {
            // Nepieciešams lai ja lietotājs uzspiež uz hover toggle, dropdown-menu-end turpinātu darboties
            $(`${id} .dropdown-menu`).attr('data-bs-popper', 'static');
        });
        // Animācija ja kursors ir virst peles
        dropdownToggle.hover(function() {
            // Paslēpt visus dropdownus, ja id nav vienāds ar pašreizējo iterācijas id
            dropdowns.forEach((_id) => 
            {
                if (_id !== id)
                {
                    $(`${_id} .dropdown-menu`).hide("fast");    
                }
            })
            dropdownMenu.show("fast");
        });
        dropdownMenu.mouseleave(function() {
            dropdownMenu.hide("fast");
        })    
    }
    // Pievienojam dropdown hover
    addDropdownHover("#messageDropdown");
    addDropdownHover("#mainMenuDropdown");
})

const openMessageModal = function (pdata) {
    $("#messageModalForm > #pdata").val(pdata)
}