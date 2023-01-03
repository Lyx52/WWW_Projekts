
// Parādīt vienkāršu paziņojumu
function showToast(message, title, time=1000)
{
    $("#toastContainerBody").text(message);
    $("#toastContainerTitle").text(title);
    $("#toastContainer").toast({ autohide: true, delay: time }).toast("show")
}
function openMessageModal(pdata)
{
    $("#messageModalForm > #pdata").val(pdata)
}