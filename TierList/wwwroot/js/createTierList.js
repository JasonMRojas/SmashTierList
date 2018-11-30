document.addEventListener("DOMContentLoaded", () => {

    document.getElementById("submit").addEventListener("click", () => {
        findAllImages();
    });

    document.querySelectorAll(".image-spots").forEach((column) => {
        column.addEventListener("dragover", (event) => {
            allowDrop(event);
        })
        column.addEventListener("drop", (event) => {
            drop(event);
        })
    });

    document.querySelectorAll(".row-name").forEach((rowName) => {
        rowName.addEventListener("click", (event) => {
            toggleEdit(event.currentTarget);
        })
    });

    document.getElementById("add-row").addEventListener("click", (event) => {
        event.preventDefault();
        addRow();
    });

    document.querySelectorAll("input.row-name-input").forEach((rowText) => {
        rowText.addEventListener("blur", (event) => {
            saveEdit(event.currentTarget);
        })
        rowText.addEventListener("keypress", (event) => {
            if (event.keyCode == 13) {
                saveEdit(event.currentTarget);
            }
        })
    });

    document.querySelectorAll("img.draggable-image").forEach((image) => {
        image.addEventListener("dragstart", (event) => {
            drag(event);
        })
    });

    document.getElementById("tier-list-name").addEventListener("click", (event) => {
        toggleEdit(event.currentTarget);
    });

    document.getElementById("tier-list-name-input").addEventListener("blur", (event) => {
        saveEdit(event.currentTarget);
    });
    document.getElementById("tier-list-name-input").addEventListener("keypress", (event) => {
        if (event.keyCode == 13) {
            saveEdit(event.currentTarget);
        }
    });

    document.querySelectorAll("span.gear").forEach((gear) => {
        gear.addEventListener("click", (event) => {
            if (gear.previousElementSibling === null) {
                showColors(event.currentTarget);
                showDeleteButton(event.currentTarget);
            }
        })
    });
});

function attachNewRowEventHandlers(newRow) {
    let gear = newRow.querySelector("span.gear");
    gear.addEventListener("click", (event) => {
        if (gear.previousElementSibling === null) {
            showColors(event.currentTarget);
            showDeleteButton(event.currentTarget);
        }
    });

    let rowName = newRow.querySelector(".row-name");
    rowName.addEventListener("click", (event) => {
        toggleEdit(event.currentTarget);
    });

    let rowText = newRow.querySelector("input.row-name-input");

    rowText.addEventListener("blur", (event) => {
        saveEdit(event.currentTarget);
    });
    rowText.addEventListener("keypress", (event) => {
        if (event.keyCode == 13) {
            saveEdit(event.currentTarget);
        }
    });
}

function attatchDeleteButtonHandlers(deleteButton) {
    deleteButton.addEventListener("click", (event) => {
        event.preventDefault();
        deleteRow(event.currentTarget);
    });
}

function attatchColorEventHandlers(colorOptions) {
    colorOptions.childNodes.forEach((color) => {
        color.addEventListener("click", (event) => {
            changeColor(event.currentTarget);
        })
    });
}

function showDeleteButton(gear) {
    let deleteButton = getElementFromTemplate("delete-row-template");
    gear.insertAdjacentElement("afterend", deleteButton);

    attatchDeleteButtonHandlers(deleteButton);

}

function deleteRow(deleteButton) {
    let currentRow = deleteButton.parentNode.parentNode;
    let tableBody = deleteButton.parentNode.parentNode.parentNode;
    tableBody.removeChild(currentRow);
}

function changeColor(colorOption) {
    let colorClass = colorOption.classList[0];

    colorOption.parentNode.parentNode.nextElementSibling.className = 'row-name-column';
    colorOption.parentNode.parentNode.nextElementSibling.classList.add(colorClass);

    colorOption.parentNode.parentNode.className = 'row-name-column options-select';
    colorOption.parentNode.parentNode.classList.add(colorClass);

    colorOption.parentNode.parentNode.nextElementSibling

    removeColors(colorOption);
}

function removeColors(colorOption) {
    let colorOptions = colorOption.parentNode;
    let deletebutton = colorOptions.nextElementSibling.nextElementSibling;


    colorOptions.parentNode.removeChild(colorOptions);
    deletebutton.parentNode.removeChild(deletebutton);
}

/*
 * Takes in a gear span and changes the color of the row name and the span's background color 
 */
function showColors(gear) {
    let colorOptions = getElementFromTemplate("color-template");

    gear.insertAdjacentElement("beforebegin", colorOptions);

    attatchColorEventHandlers(colorOptions);
}

function addRow() {
    let templateRow = document.getElementById("psuedo-template-row");
    let newRow = templateRow.cloneNode(true);

    newRow.className = '';
    templateRow.parentNode.insertBefore(newRow, templateRow);

    attachNewRowEventHandlers(newRow);
}

function getElementFromTemplate(id) {
    let domNode = document.importNode(document.getElementById(id).content, true).firstElementChild;

    return domNode;
}


function saveEdit(textBox) {
    let rowName = textBox.previousElementSibling;

    rowName.innerText = textBox.value;

    textBox.classList.add('d-none');
    rowName.classList.remove('d-none');

    rowName.attributes.getNamedItem("myPos").value = textBox.value;
    let currentColumn = rowName.parentNode;
    while (currentColumn.nextElementSibling !== null) {
        currentColumn.nextElementSibling.attributes.getNamedItem("myPos").value = textBox.value;
        currentColumn = currentColumn.nextElementSibling;
    }
}

function toggleEdit(rowName) {
    let textBox = rowName.nextElementSibling;

    textBox.classList.remove('d-none');
    rowName.classList.add('d-none');

    textBox.focus();
}

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

function getRowName(rowId) {

    let rows = $(".row-name");

    for (var i = 0; i < rows.length; i++) {

        if (rowId == rows[i].attributes.getNamedItem("myPos").value) {
            var rowName = rows[i].innerText;
        }
    }


    return rowName;
}