function TestService(idFeed) {
    try {

        $.ajax({
            type: "POST",
            url: "http://localhost:17864/WSSemanticSearch.asmx/RetornarJsonSensor",
            data: "{'IdSensor':'" + idFeed + "'}", // if ur method take parameters
            contentType: "application/json; charset=utf-8",
            success: SuccessTestService,
            dataType: "json",
            failure: ajaxCallFailed
        });
    }
    catch (e) {
        alert('failed to call web service. Error: ' + e);
    }
}

function SuccessTestService(responce) {
    alert(eval(responce.d));
}


function ajaxCallFailed(error) {
    alert('error: ' + error);

}