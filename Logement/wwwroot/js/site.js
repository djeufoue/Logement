// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(function () {
    var PlaceHolderElement = $('#PlaceHolderHere');
    $('button[data-toggle="ajax-modal"]').click(function (event) {

        var url = $(this).data('url');
        $.get(url).done(function (data) {
            PlaceHolderElement.html(data);
            PlaceHolderElement.find('.modal').modal('show');
        })
    })

    $('button[data-dismiss="modal"]').click(function () {
        $('.modal').modal("hide");
    });
})



/* When the user clicks on the button,
toggle between hiding and showing the dropdown content */
function myFunction() {
    document.getElementById("myDropdown").classList.toggle("show");
}

// Close the dropdown menu if the user clicks outside of it
window.onclick = function (event) {
    if (!event.target.matches('.dropbtn')) {
        var dropdowns = document.getElementsByClassName("dropdown-content");
        var i;
        for (i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
                openDropdown.classList.remove('show');
            }
        }
    }
}

function addAnotherPicture(button, path) {
    var html = ""
        + "  <div class='row'>\n"
        + "    <div class='col-sm-2'>\n"
        + "      Picture\n"
        + "    </div>\n"
        + "    <div class='col-sm-10'>\n"
        + "      <input class='form-control ChosenImage' name='(path)[(index)].ImageURL' value='(image)' accept='image/*' required/> "
        + "    </div>\n"
        + "  </div>\n"
        + "  <div class='row'>\n"
        + "    <div class='col-sm-2'>\n"
        + "       Which Part\n"
        + "    </div>\n"
        + "    <div class='col-sm-10'>\n"
        + "       <input type='text' readonly='readonly' class='form-control ItemPart' name='(path)[(index)].Part' value='(apartmentPart)' />\n"
        + "    </div>\n"
        + "  </div>\n"
        + "  <div style='text-align: right;'>\n"
        + "    <a class='btn btn-danger' style='margin-bottom:10px;' onclick=\"removePicture(this, '(path)'); updateItemCount(0)\">Remove</a>\n"
        + "  </div>\n";

    var linkedItem = button.parentElement.parentElement;

    var chosenImage = linkedItem.getElementsByClassName("ChosenImage")[0];
    var whichPart = linkedItem.getElementsByClassName("ItemPart")[0];

    var image = chosenImage.value;
    var apartmentPart = whichPart.value;

    if (!image) {
        alert("Please choose a picture");
    }
    else if (!apartmentPart) {
        alert("Please add the apartment part");
    }
    else if (apartmentPart.length > 50) {
        alert("Number of character should be less than 50");
    }
    else
    {
        chosenImage.value = null
        whichPart.value = null
        var linkedItems = linkedItem.parentElement;
        var index = linkedItems.children.length - 1;

        html = html.replaceAll("(path)", path);
        html = html.replaceAll("(index)", index);
        html = html.replaceAll("(image)", image);
        html = html.replaceAll("(apartmentPart)", apartmentPart);

        var div = document.createElement("DIV");
        div.innerHTML = html;
        linkedItems.insertBefore(div, linkedItem);
    }
}

function removePicture(button, path) {
    var linkedItem = button.parentElement.parentElement;
    var linkedItems = linkedItem.parentElement;
    linkedItem.remove();

    for (var i = 0; i < linkedItems.children.length - 1; i++) {
        linkedItem = linkedItems.children[i];

        var chosenImage = linkedItem.getElementsByClassName("ChosenImage")[0];
        var whichPart = linkedItem.getElementsByClassName("ItemPart")[0];

        chosenImage.name = path + "[" + i + "]" + ".ImageURL";
        whichPart.name = path + "[" + i + "]" + ".Part";
    }
}