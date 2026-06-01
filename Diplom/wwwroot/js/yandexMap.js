let yandexMap = null;
let yandexMarkers = [];
let yandexMarkerClass = null;
let yandexMapResizeObserver = null;
let yandexMapClickListener = null;

window.initYandexMap = async function (elementId) {
    const container = await waitForMapContainer(elementId);

    await waitForYmaps3();
    await ymaps3.ready;
    await waitForNextPaint();

    resetYandexMap(container);

    const {
        YMap,
        YMapDefaultSchemeLayer,
        YMapDefaultFeaturesLayer,
        YMapMarker,
        YMapListener
    } = ymaps3;

    yandexMarkerClass = YMapMarker;
    yandexMap = new YMap(container, {
        location: {
            center: [37.617644, 55.755819],
            zoom: 10
        }
    });

    yandexMap.addChild(new YMapDefaultSchemeLayer());
    yandexMap.addChild(new YMapDefaultFeaturesLayer());
    yandexMapClickListener = new YMapListener({
        onClick: (_, event) => showCreateUnoPopup(container, event.coordinates)
    });
    yandexMap.addChild(yandexMapClickListener);

    watchMapSize(container);
    refreshYandexMap();
};

window.setYandexMarkers = function (markers) {
    if (!yandexMap || !yandexMarkerClass) {
        console.error("Yandex map is not initialized");
        return;
    }

    clearMarkers();

    markers.forEach(markerData => {
        const marker = new yandexMarkerClass(
            {
                coordinates: [markerData.longitude, markerData.latitude]
            },
            createMarkerElement(markerData)
        );

        yandexMap.addChild(marker);
        yandexMarkers.push(marker);
    });

    refreshYandexMap();
};

async function waitForYmaps3(timeoutMs = 10000) {
    const startedAt = Date.now();

    while (!window.ymaps3 || !window.ymaps3.ready) {
        if (Date.now() - startedAt > timeoutMs) {
            throw new Error("Yandex Maps API was not loaded");
        }

        await delay(100);
    }
}

async function waitForMapContainer(elementId, timeoutMs = 10000) {
    const startedAt = Date.now();

    while (true) {
        const container = document.getElementById(elementId);
        const rect = container ? container.getBoundingClientRect() : null;

        if (container && rect && rect.width > 0 && rect.height > 0) {
            return container;
        }

        if (Date.now() - startedAt > timeoutMs) {
            throw new Error(`Map container '${elementId}' is not ready`);
        }

        await nextAnimationFrame();
    }
}

function resetYandexMap(container) {
    if (yandexMap && typeof yandexMap.destroy === "function") {
        yandexMap.destroy();
    }

    if (yandexMapResizeObserver) {
        yandexMapResizeObserver.disconnect();
    }

    container.innerHTML = "";
    yandexMap = null;
    yandexMarkers = [];
    yandexMarkerClass = null;
    yandexMapResizeObserver = null;
    yandexMapClickListener = null;
}

function watchMapSize(container) {
    yandexMapResizeObserver = new ResizeObserver(() => refreshYandexMap());
    yandexMapResizeObserver.observe(container);
}

function createMarkerElement(markerData) {
    const markerElement = document.createElement("div");
    markerElement.className = "custom-marker";
    markerElement.onclick = () => showUnoPopup(markerElement, markerData);

    const dot = document.createElement("div");
    dot.className = "marker-dot";

    markerElement.appendChild(dot);

    return markerElement;
}

function showUnoPopup(markerElement, markerData) {
    removeMapPopups();

    const popup = document.createElement("div");
    popup.className = "uno-popup";

    const closeButton = document.createElement("button");
    closeButton.className = "uno-popup-close";
    closeButton.type = "button";
    closeButton.textContent = "×";
    closeButton.onclick = event => {
        event.stopPropagation();
        popup.remove();
    };

    const title = document.createElement("h3");
    title.textContent = markerData.title;

    const status = createPopupLine("Статус:", markerData.status);
    const address = createPopupLine("Адрес:", markerData.address);

    const detailsButton = document.createElement("a");
    detailsButton.className = "uno-popup-button";
    detailsButton.href = `/uno/${markerData.id}`;
    detailsButton.textContent = "Перейти к УНО";
    detailsButton.onclick = event => {
        event.stopPropagation();
    };

    popup.append(closeButton, title, status, address, detailsButton);
    markerElement.appendChild(popup);
}

function showCreateUnoPopup(container, coordinates) {
    removeMapPopups();

    if (!coordinates || coordinates.length < 2) {
        return;
    }

    const longitude = Number(coordinates[0]);
    const latitude = Number(coordinates[1]);

    if (!Number.isFinite(latitude) || !Number.isFinite(longitude)) {
        return;
    }

    const popup = document.createElement("div");
    popup.className = "map-create-popup";

    const closeButton = document.createElement("button");
    closeButton.className = "uno-popup-close";
    closeButton.type = "button";
    closeButton.textContent = "×";
    closeButton.onclick = event => {
        event.stopPropagation();
        popup.remove();
    };

    const title = document.createElement("h3");
    title.textContent = "Новая точка";

    const coordinatesLine = document.createElement("p");
    coordinatesLine.textContent = `${latitude.toFixed(7)}, ${longitude.toFixed(7)}`;

    const createButton = document.createElement("a");
    createButton.className = "uno-popup-button";
    createButton.href = `/uno/create?lat=${latitude.toFixed(7)}&lng=${longitude.toFixed(7)}`;
    createButton.textContent = "Создать УНО";
    createButton.onclick = event => event.stopPropagation();

    popup.append(closeButton, title, coordinatesLine, createButton);
    container.appendChild(popup);
}

function removeMapPopups() {
    document.querySelectorAll(".uno-popup, .map-create-popup").forEach(popup => popup.remove());
}

function createPopupLine(label, value) {
    const line = document.createElement("p");
    const strong = document.createElement("b");

    strong.textContent = label;
    line.append(strong, ` ${value || "-"}`);

    return line;
}

function clearMarkers() {
    yandexMarkers.forEach(marker => yandexMap.removeChild(marker));
    yandexMarkers = [];
}

function refreshYandexMap() {
    if (!yandexMap) {
        return;
    }

    [100, 300, 700].forEach(timeout => {
        setTimeout(() => {
            if (yandexMap) {
                yandexMap.update({});
            }
        }, timeout);
    });
}

function delay(timeout) {
    return new Promise(resolve => setTimeout(resolve, timeout));
}

async function waitForNextPaint() {
    await nextAnimationFrame();
    await nextAnimationFrame();
}

function nextAnimationFrame() {
    return new Promise(resolve => requestAnimationFrame(resolve));
}
