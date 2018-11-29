var geocoder;
var map;
var infowindow = new google.maps.InfoWindow({
    content: ''
});

function GeolocalizarMapa() {
    // Try HTML5 geolocation
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = new google.maps.LatLng(position.coords.latitude,
            position.coords.longitude);

            var infowindow = new google.maps.InfoWindow({
                map: map,
                position: pos,
                content: 'Localización encontrada utilizando HTML5.'
            });

            map.setCenter(pos);
        }, function () {
            handleNoGeolocation(true);
        });
    } else {
        // Browser doesn't support Geolocation
        handleNoGeolocation(false);
    }
}

function handleNoGeolocation(errorFlag) {
    if (errorFlag) {
        var content = 'Error: El servicio de Geolocalización fallo.';
    } else {
        var content = 'Error: Su navegador no soporta geolocalización.';
    }

    var options = {
        map: map,
        position: new google.maps.LatLng(60, 105),
        content: content
    };

    var infowindow = new google.maps.InfoWindow(options);
    map.setCenter(options.position);
}

function codeAddress() {
    var address = document.getElementById('address').value;
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            map.setCenter(results[0].geometry.location);
            var marker = new google.maps.Marker({
                map: map,
                position: results[0].geometry.location
            });
        } else {
            alert('Geocodigo no fue exitoso por la siguiente razón: ' + status);
        }
    });
}

function codeLatLng() {
    //var input = document.getElementById('latlng').value;
    if($('#map').is(':visible')) {
        var posicion = document.getElementById('ctl00_ContentPlaceHolder1_drplLugares').options.selectedIndex; //posicion
        var input = document.getElementById('ctl00_ContentPlaceHolder1_drplLugares').options[posicion].value; //valor
        var latlngStr = input.split(',', 2);
        var lat = parseFloat(latlngStr[0]);
        var lng = parseFloat(latlngStr[1]);
        geocoder = new google.maps.Geocoder();
        var latlng = new google.maps.LatLng(lat, lng);
        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (results[1]) {
                    var mapOptions = {
                        zoom: 8,
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                    };
                    map = new google.maps.Map(document.getElementById('map'), mapOptions);

                    map.setCenter(results[1].geometry.location);
                    map.setZoom(11);
                    marker = new google.maps.Marker({
                        position: latlng,
                        map: map
                    });
                    infowindow.setContent(results[1].formatted_address);
                    infowindow.open(map, marker);
                    ColocarMarcadordes();
                } else {
                    alert('No se encontraron resultados');
                }
            } else {
                alert('Geocoder falló debido a: ' + status);
            }
        });
    }
}

function ColocarMarcadordes() 
{
    //Obtener la lista de Marcadores del grid de resultados
    var puntos = new Array();
    var indicepuntos = 0;
    var filas = document.getElementById('ctl00_ContentPlaceHolder1_gridResultados').rows.length;
    for (var i=1;i<filas-1;i++) 
    {
        var punto = new Object();
        var columnas = document.getElementById('ctl00_ContentPlaceHolder1_gridResultados').rows[i].cells.length;
        if (columnas - 2 != -1) {
            for (var j = 5; j < columnas - 2; j++) {
                //La primera columna es la latitud
                if (j == 5) punto.lat = document.getElementById('ctl00_ContentPlaceHolder1_gridResultados').rows[i].cells[j].innerHTML;
                //La segunda columna es la longitud
                else punto.lng = document.getElementById('ctl00_ContentPlaceHolder1_gridResultados').rows[i].cells[j].innerHTML;
            }
            //Generamos los datos de ls ventana de información
            punto.Titulo = document.getElementById('ctl00_ContentPlaceHolder1_gridResultados').rows[i].cells[2].innerHTML;
            punto.IdSensor = document.getElementById('ctl00_ContentPlaceHolder1_gridResultados').rows[i].cells[1].innerHTML
            puntos[indicepuntos] = punto;
            indicepuntos++;
        }
    }

    for (i = 0; i < puntos.length; i++) {
        MarcarSensor(puntos[i].lat, puntos[i].lng, '<strong>Id: ' + puntos[i].IdSensor + '</strong> - ' + puntos[i].Titulo);
    }
}

function MarcarSensor(lat, lng, titulo){
    var latlng = new google.maps.LatLng(lat, lng);
    var contenido = titulo;
    var marker = new google.maps.Marker({
        position: latlng
              , map: map
              , title: 'Clic para más información'
              , icon: 'http://gmaps-samples.googlecode.com/svn/trunk/markers/green/blank.png'
              , cursor: 'default'
              , draggable: false
    });
    (function (marker, contenido) {
        google.maps.event.addListener(marker, 'click', function () {
            infowindow.setContent(contenido);
            infowindow.open(map, marker);
        });
    })(marker, contenido);
}

