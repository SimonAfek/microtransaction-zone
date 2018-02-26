document.getElementById("ApproveButton").addEventListener("click", function (evt) {
    if (!confirm("Are you sure you want to approve this submission? It will immediately go live on the site.")) {
        evt.preventDefault();
    }
});

document.getElementById("RejectButton").addEventListener("click", function (evt) {
    if (!confirm("Are you sure you want to reject this submission? It will be permanently deleted.")) {
        evt.preventDefault();
    }
});
