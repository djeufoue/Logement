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
        var CountryCode = $("#countryCode").val();
        var PhoneNumber = $("#phoneNumber").val();
        var Email = $("#email").val();

        // Check if required fields are filled
        if (!FirstName || !LastName || !Email) {
            alert("First Name, Last Name, and Email are required fields.");
            return;
        }

        var data = {
            firstName: FirstName,
            lastName: LastName,
            countryCode: CountryCode,
            phoneNumber: PhoneNumber.replace(/\s+/g, ''),
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
            error: function (xhr) {
                // Display the error message in the modal body
                var errorMessage = xhr.responseText; // Get the error message from the response
                $("#errorMessage").text(errorMessage).removeClass("d-none");

                // Optionally, you can scroll the modal to show the error message
                $("#editModal").animate({ scrollTop: $("#errorMessage").offset().top }, "slow");
            }
        });
    });
});

//Check for a duplicate images
function checkImageExistence(event) {
    const file = event.target.files[0];
    const formData = new FormData();
    const xhr = new XMLHttpRequest();

    formData.append('file', file);

    xhr.open('POST', '/City/CheckImageExistence', true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                const response = JSON.parse(xhr.responseText);
                if (response === 1) {
                    $("#errorMessage").text("The image already exists in the database. Please choose another image.").removeClass("d-none");

                    // Clear the file input value to prevent uploading the existing image
                    document.getElementById('image-input').value = '';

                } else if (response === 0) {
                    // Image doesn't exist, proceed with the upload
                } else {
                    $("#errorMessage").text("Error checking image existence.Please try again.").removeClass("d - none");
                }
            }
        }
    };
    xhr.send(formData);
}

//Check multiple existence
function CheckMultipleImageExistence(event) {
    const files = event.target.files;

    if (files.length === 0) {
        return; // No files selected
    }

    const formData = new FormData();
    const xhr = new XMLHttpRequest();

    for (let i = 0; i < files.length; i++) {
        formData.append('files', files[i]);
    }

    xhr.open('POST', '/Tenant/CheckMultipleImageExistence', true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                const response = JSON.parse(xhr.responseText);
                if (response.length > 0) {
                    const errorMessageDiv = document.getElementById('errorMessage');
                    errorMessageDiv.innerHTML = `The following file(s) already exist: ${response.join(', ')}`;
                    errorMessageDiv.classList.remove('d-none');

                    document.getElementById('image-input').value = '';
                } else if (response === -1) {
                    alert('Error checking image existence. Please try again.');
                }
            }
        }
    };
    xhr.send(formData);
}
