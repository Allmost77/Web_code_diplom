function addToMap(file) {
    
}

window.initMap = async function (elementId) {
    // Безопасное ожидание загрузки модулей Яндекса
    await ymaps3.ready;

    const { YMap, YMapDefaultSchemeLayer, YMapDefaultMarker, YMapDefaultFeaturesLayer } = ymaps3;

    // Ищем элемент, который передал Blazor
    const mapContainer = document.getElementById(elementId);
    if (!mapContainer) {
        console.error("Элемент карты не найден в DOM!");
        return;
    }

    const map = new YMap(
        mapContainer,
        {
            location: {
                center: [37.588144, 55.733842], // Долгота, широта
                zoom: 10
            }
        }
    );

    map.addChild(new YMapDefaultSchemeLayer());
    map.addChild(new YMapDefaultFeaturesLayer());
    
    
};
