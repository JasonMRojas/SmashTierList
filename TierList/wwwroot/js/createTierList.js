document.addEventListener("DOMContentLoaded", () => {

    document.getElementById("submit").addEventListener("click", () => {
        findAllImages();
    });

    document.querySelectorAll("td.image-spots").forEach((column) => {
        column.addEventListener("dragover", (event) => {
            allowDrop(event);
        })
        column.addEventListener("drop", (event) => {
            drop(event);
        })
    });

    document.querySelectorAll("img.draggable-image").forEach((image) => {
        image.addEventListener("dragstart", (event) => {
            drag(event);
        })
    });
});


function allowDrop(ev) {
    ev.preventDefault();
}

function drag(ev) {
    ev.dataTransfer.setData("text", ev.target.id);
}

function drop(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    ev.target.appendChild(document.getElementById(data));
}

function findAllImages() {
    let allTierListImages = $("tbody.tierlist td > img");
    var valueMyPos = "";

    for (var i = 0; i < allTierListImages.length; i++) {
        valueMyPos += getRowName(allTierListImages[i].offsetParent.attributes.getNamedItem("myPos").value) + "|";
        valueMyPos += allTierListImages[i].attributes.getNamedItem("src").value + "|";
    }

    // ... la la la MAKE THIS LOOK LIKE SOME SERIALZD MODEL, keep in mind we can use the td parent's myPos attr to get the images' pos.
    $("#put-serialized-list-here").attr("value", valueMyPos);
}

function addRow() {

}

function getRowName(rowId) {

    let rows = $(".row-name");

    for (var i = 0; i < rows.length; i++) {

        if (rowId == rows[i].attributes.getNamedItem("myPos").value) {
            var rowName = rows[i].value;
        }
    }


    return rowName;
}