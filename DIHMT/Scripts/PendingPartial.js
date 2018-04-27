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

document.getElementById("TimeOutButton").addEventListener("click", function (evt) {
    if (!confirm("Are you sure you want to give this IP a time-out? They will be prevented from submitting ratings until tomorrow.")) {
        evt.preventDefault();
    }
});

document.getElementById("PermabanButton").addEventListener("click", function (evt) {
    if (!confirm("!!! ARE YOU ABSOLUTELY SURE ABOUT THIS? THIS WILL PERMANENTLY PREVENT THIS IP FROM SUBMITTING TO THE SITE !!!")) {
        evt.preventDefault();
    }
});
