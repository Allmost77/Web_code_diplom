let yandexMap = null;
let yandexMarkers = [];
let YMapDefaultMarkerClass = null;

window.initYandexMap = async function (elementId) {
    console.log("initYandexMap called");

    await ymaps3.ready;

    const {
        YMap,
        YMapDefaultSchemeLayer,
        YMapDefaultFeaturesLayer,
        YMapMarker
    } = ymaps3;

   

    const container = document.getElementById(elementId);



    yandexMap = new YMap(container, {
        location: {
            center: [37.617644, 55.755819],
            zoom: 10
        }
    });

    yandexMap.addChild(new YMapDefaultSchemeLayer());
    yandexMap.addChild(new YMapDefaultFeaturesLayer());

    window.YMapMarkerClass = YMapMarker;
    console.log("Yandex map initialized");
};


window.setYandexMarkers = function (markers) {
    console.log("setYandexMarkers called", markers);
    if (!yandexMap) {
        console.error("Yandex map is not initialized");
        return;
    }

    if (!window.YMapMarkerClass) {
        console.error("YMapMarkerClass is not initialized");
        return;
    }
    clearMarkers();


    markers.forEach(markerData => {

        const markerElement = createMarkerElement(markerData);

        const marker = new window.YMapMarkerClass(
            {
                coordinates: [markerData.longitude, markerData.latitude]
            },
            markerElement
        );

        yandexMap.addChild(marker);

        yandexMarkers.push(marker);
        console.log(`Added ${markers.length} markers`);
    });
};

function createMarkerElement(markerData) {
    const markerElement = document.createElement("div");
    markerElement.className = "custom-marker";

    const dot = document.createElement("div");
    dot.className = "marker-dot";

    markerElement.appendChild(dot);

    markerElement.onclick = () => {
        showUnoPopup(markerElement, markerData);
    };

    return markerElement;
}

function showUnoPopup(markerElement, markerData) {
    document.querySelectorAll(".uno-popup").forEach(p => p.remove());

    const popup = document.createElement("div");
    popup.className = "uno-popup";

    popup.innerHTML = `
        <button class="uno-popup-close">×</button>
        <h3>${markerData.title}</h3>
        <p><b>Статус:</b> ${markerData.status}</p>
        <p><b>Адрес:</b> ${markerData.address}</p>
        <button class="uno-popup-button">Перейти к УНО</button>
    `;

    popup.querySelector(".uno-popup-close").onclick = (event) => {
        event.stopPropagation();
        popup.remove();
    };

    popup.querySelector(".uno-popup-button").onclick = (event) => {
        event.stopPropagation();
        window.location.href = `/uno/${markerData.id}`;
    };

    markerElement.appendChild(popup);
}
function addYandexMarker(markerData) {
    let popupIsOpen = false;

    const marker = new YMapDefaultMarkerClass({
        coordinates: [markerData.longitude, markerData.latitude],
        title: markerData.title,
        subtitle: markerData.status,
        color: getColorByStatus(markerData.status),
        popup: {
            show: popupIsOpen,
            content: () => createPolePopup(markerData)
        },
        onClick: () => {
            popupIsOpen = !popupIsOpen;

            marker.update({
                popup: {
                    show: popupIsOpen,
                    content: () => createPolePopup(markerData)
                }
            });
        }
    });

    yandexMap.addChild(marker);
    yandexMarkers.push(marker);
}

function createPolePopup(markerData) {
    const container = document.createElement("div");
    container.classList.add("map-balloon");

    const title = document.createElement("h3");
    title.textContent = markerData.title;

    const address = document.createElement("p");
    address.innerHTML = `<b>Адрес:</b> ${markerData.address}`;

    const status = document.createElement("p");
    status.innerHTML = `<b>Статус:</b> ${markerData.status}`;

    const button = document.createElement("button");
    button.classList.add("map-balloon-button");
    button.textContent = "Подробнее";

    button.onclick = () => {
        alert(`Открыть УНО ID: ${markerData.id}`);
    };

    container.appendChild(title);
    container.appendChild(address);
    container.appendChild(status);
    container.appendChild(button);

    return container;
}
function clearMarkers() {

    yandexMarkers.forEach(marker => {
        yandexMap.removeChild(marker);
    });

    yandexMarkers = [];
}

function getColorByStatus(status) {
    switch (status) {
        case "В работе":
            return "#ff9800";
        case "Требует проверки":
            return "#f44336";
        case "Исправен":
            return "#4caf50";
        default:
            return "#2196f3";
    }
}

function refreshYandexMap() {
    if (!yandexMap) {
        return;
    }

    setTimeout(() => yandexMap.update({}), 100);
    setTimeout(() => yandexMap.update({}), 300);
    setTimeout(() => yandexMap.update({}), 700);
}