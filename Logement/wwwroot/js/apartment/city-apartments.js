
function loadApartments(propertyId) {
    fetch(`/City/GetPropertyApartments?propertyId=${propertyId}`)
        .then(response => {
            if (!response.ok) throw new Error("Failed to fetch apartments");
            return response.json();
        })
        .then(data => {
            const list = document.getElementById("apartmentList");
            list.innerHTML = "";

            if (data.length >= 8) {
                list.classList.add("scrollable");
            }

            data.forEach((apartment, index) => {
                const li = document.createElement("li");
                const a = document.createElement("a");

                a.textContent = apartment.apartmentName;
                a.href = `/Apartment/ApartmentDetails?cityId=${propertyId}&apartmentId=${index + 1}`;
                a.className = "apartment-link";

                li.appendChild(a);
                list.appendChild(li);
            });
        })
        .catch(err => console.error(err));
}

function toggleAccordion(header) {
    header.classList.toggle("active");
    const list = header.nextElementSibling;
    if (list.style.maxHeight && list.style.maxHeight !== "0px") {
        list.style.maxHeight = "0px";
    } else {
        list.style.maxHeight = list.scrollHeight + "px";
    }
}

function openModal(type) {
    currentType = type;
    document.getElementById('modalTitle').innerText = `Add ${type}`;
    document.getElementById('modalInput').value = '';
    document.getElementById('addModal').style.display = 'flex';
    document.getElementById('modalInput').focus();
}

function closeModal() {
    document.getElementById('addModal').style.display = 'none';
}

function submitModal() {
    const value = document.getElementById('modalInput').value.trim();
    if (!value) return;

    const list = document.getElementById("apartmentList");
    const li = document.createElement('li');
    li.textContent = value;
    list.appendChild(li);

    const header = list.previousElementSibling;
    if (!header.classList.contains('active')) {
        toggleAccordion(header);
    }

    closeModal();
}

let currentType = '';
