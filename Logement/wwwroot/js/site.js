const search = document.querySelector('.input-group input'),
    table_rows = document.querySelectorAll('tbody tr'),
    table_headings = document.querySelectorAll('thead th');

// 1. Searching for specific data of HTML table
search.addEventListener('input', searchTable);

function searchTable() {
    table_rows.forEach((row, i) => {
        let table_data = row.textContent.toLowerCase(),
            search_data = search.value.toLowerCase();

        row.classList.toggle('hide', table_data.indexOf(search_data) < 0);
        row.style.setProperty('--delay', i / 25 + 's');
    })

    document.querySelectorAll('tbody tr:not(.hide)').forEach((visible_row, i) => {
        visible_row.style.backgroundColor = (i % 2 == 0) ? 'transparent' : '#0000000b';
    });
}


$(document).ready(function () {
    // Show the modal and add the custom backdrop class
    $("#editButton").on("click", function () {
        $("#editModal").modal("show");
        $(".modal-backdrop").addClass("custom-backdrop");
    });

    // Remove the custom backdrop class when the modal is hidden
    $("#editModal").on("hidden.bs.modal", function () {
        $(".modal-backdrop").removeClass("custom-backdrop");
    });

    // Handle the click event on the modal close button
    $("#editModal .close").on("click", function () {
        // Refresh the page when the modal is closed without saving
        window.location.reload();
    });

    $("#editModal .secondCloseButton").on("click", function () {
        // Refresh the page when the modal is closed without saving
        window.location.reload();
    });

    $("#saveButton").on("click", function (event) {
        event.preventDefault(); 

        var FirstName = $("#firstName").val();
        var LastName = $("#lastName").val();
        var JobTitle = $("#jobTitle").val();
        var countryCode = $("#countryCode").val();
        var PhoneNumber = $("#phoneNumber").val();
        var Email = $("#email").val();

        // Check if required fields are filled
        if (!FirstName || !LastName || !JobTitle || !Email) {
            alert("First Name, Last Name, Job Title, and Email are required fields.");
            return;
        }

        var data = {
            firstName: FirstName,
            lastName: LastName,
            jobTitle: JobTitle,
            phoneNumber: countryCode + PhoneNumber.replace(/\s+/g, ''),
            email: Email
        };

        $.ajax({
            type: "POST",
            url: "/Account/EditProfile",
            data: data,
            success: function (response) {
                if (response.redirectTo) {
                    // Reload the page to perform the redirect
                    location.reload();
                }
            },
            error: function (xhr, status, error) {
                // Display the error message in the modal body
                var errorMessage = xhr.responseText; // Get the error message from the response
                $("#errorMessage").text(errorMessage).removeClass("d-none");

                // Optionally, you can scroll the modal to show the error message
                $("#editModal").animate({ scrollTop: $("#errorMessage").offset().top }, "slow");
            }
        });
    });
});
